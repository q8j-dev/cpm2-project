using System;
using System.Threading;
using CPM.Account;
using CPM.RemoteConfig;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Genuine.CodeHash;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Connection;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Other.Helpers;

namespace CPM.Connection
{
	public class ConnectionHandler : IAuthorizationDependant, IConnectionManager, IConnectionManagerBase, IAuthorizationFailureDependant, IDisposable
	{
		private const int CheckHashAmountMax = 2;

		private const float DelayToCheckNextHash = 30f;

		private readonly IConnectionService _connectionService;

		private readonly PhotonConnectionHandler _photonConnectionHandler;

		private readonly RemoteConfigDataHandler _remoteConfigDataHandler;

		private readonly IRemoteConfigManager _remoteConfigManager;

		private readonly SemaphoreSlim _hashCheckSemaphore = new SemaphoreSlim(1, 1);

		private IConnectionDataHandler _connectionDataHandler;

		private int _tryCheckHashAmountNow;

		private CancellationTokenSource _cancellationTokenSourceCheckHashWithDelay;

		private CancellationTokenSource _mainCts;

		private BlockAppStatusUtil _blockAppStatusUtil;

		public ObscuredInt HashStatus { get; private set; }

		public ObscuredInt Hash { get; private set; }

		public ObscuredInt H { get; private set; }

		public ObscuredInt S1 { get; private set; }

		public bool CheckHashLimitExceeded => _tryCheckHashAmountNow >= 2;

		public AdvancedBoolean CorrectHash => new AdvancedBoolean((int)H == 2);

		private ObscuredBool NotLicensedApp { get; set; }

		public event Action OnHashStatusChange;

		public event Action OnCheckHashLimitExceeded;

		[Inject]
		public ConnectionHandler(PhotonConnectionHandler photonConnectionHandler, IRemoteConfigManager remoteConfigManager, RemoteConfigDataHandler remoteConfigDataHandler, CheckBackendService checkBackendService)
		{
			_connectionService = new FirebaseConnectionService(checkBackendService);
			_photonConnectionHandler = photonConnectionHandler;
			_remoteConfigManager = remoteConfigManager;
			_remoteConfigDataHandler = remoteConfigDataHandler;
			H = 1;
			_blockAppStatusUtil = new BlockAppStatusUtil(0);
			_mainCts = new CancellationTokenSource();
		}

		public IConnectionService GetConnectionService()
		{
			return _connectionService;
		}

		public void Initialize()
		{
			CalculateAndCheckHashAsync(_mainCts.Token).Forget();
			CodeHashGenerator.Generate();
		}

		public void Dispose()
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _mainCts);
			_hashCheckSemaphore?.Dispose();
			_cancellationTokenSourceCheckHashWithDelay?.Dispose();
		}

		private static bool IsCalculationFinished()
		{
			if ((bool)KeepAliveBehaviour<CodeHashGenerator>.Instance)
			{
				return !KeepAliveBehaviour<CodeHashGenerator>.Instance.IsCalculating;
			}
			return false;
		}

		public void RunCheckHashManually(bool authorized)
		{
			Run(authorized).Forget();
		}

		private async UniTaskVoid Run(bool authorized)
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckHashWithDelay);
			await CalculateAndCheckHashAsync(_mainCts.Token);
			CorrectHash.Get(out var correctHash);
			if (correctHash)
			{
				_connectionDataHandler.ReProcessCachedData().Forget();
			}
			ConfigValuesStorage configValuesStorage = await _remoteConfigManager.GetConfigValuesStorage();
			_remoteConfigDataHandler.ProcessRemoteConfigData(configValuesStorage, correctHash);
			_blockAppStatusUtil = new BlockAppStatusUtil(configValuesStorage.blockAppStatus);
			if ((int)H == 0 && _blockAppStatusUtil.BlockBadHashApp())
			{
				CommonLogsBus.PrintError("Not correct app");
				LoadSceneWithBadHash();
				TryCheckHashIfNeeds(authorized);
				return;
			}
			if ((bool)NotLicensedApp)
			{
				if (_blockAppStatusUtil.BlockNotLicensedApp())
				{
					CommonLogsBus.PrintError("Not licensed app");
					LoadSceneWithBadHash();
					TryCheckHashIfNeeds(authorized);
					return;
				}
				H = 3;
				OnHashStatusChange?.Invoke();
			}
			TryCheckHashIfNeeds(authorized);
		}

		public void SubscribeForConnectionDataHandle(IConnectionDataHandler connectionDataHandler)
		{
			_connectionDataHandler = connectionDataHandler;
		}

		public async UniTaskVoid ReconnectPhoton(PhotonConnectionProfile profile)
		{
			ConfigValuesStorage configValuesStorage = await _remoteConfigManager.GetConfigValuesStorage();
			PhotonNetwork.Disconnect();
			_photonConnectionHandler.MakeConnection(configValuesStorage, CorrectHash, profile).Forget();
		}

		public async UniTaskVoid OnSuccessfulAuthorization()
		{
			Run(authorized: true).Forget();
		}

		public void OnSignOut()
		{
			Run(authorized: false).Forget();
		}

		public void SetNotLicensedApp()
		{
			Debug.Log("Not licensed app 0");
			NotLicensedApp = true;
		}

		public void LoadSceneWithBadHash()
		{
			ScenesHelper.LoadNotificationScene(SceneNotificationReason.NeedOfficialVersion);
		}

		public UniTaskVoid OnAuthorizationFailure()
		{
			Run(authorized: false).Forget();
			return default;
		}

		private async UniTask CalculateAndCheckHashAsync(CancellationToken cancellationToken)
		{
			await UniTask.Yield();
			await _hashCheckSemaphore.WaitAsync(cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			try
			{
				CorrectHash.Get(out var value);
				if (!value)
				{
					await CalculateHashCoreAsync();
				}
			}
			finally
			{
				_hashCheckSemaphore.Release();
			}
		}

		private async UniTask CalculateHashCoreAsync()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(2.0));
			CommonLogsBus.Print("[CalculateHashCoreAsync] 1");
			H = 2;
			CommonLogsBus.Print("5 Connection manager: " + (((int)H == 2) ? UnityEngine.Random.Range(50, 100) : UnityEngine.Random.Range(101, 150)));
			OnHashStatusChange?.Invoke();
		}

		private void TryCheckHashIfNeeds(bool authorized)
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckHashWithDelay);
			CorrectHash.Get(out var value);
			if (!value)
			{
				if (CheckHashLimitExceeded)
				{
					CommonLogsBus.Print("CheckHashLimitExceeded");
					OnCheckHashLimitExceeded?.Invoke();
				}
				else
				{
					_cancellationTokenSourceCheckHashWithDelay = new CancellationTokenSource();
					CheckHashWithDelayAsync(authorized, _cancellationTokenSourceCheckHashWithDelay.Token).Forget();
				}
			}
		}

		private async UniTaskVoid CheckHashWithDelayAsync(bool authorized, CancellationToken cancellationToken)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(30.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				CorrectHash.Get(out var value);
				if (!value)
				{
					_tryCheckHashAmountNow++;
					Run(authorized).Forget();
				}
			}
		}
	}
}
