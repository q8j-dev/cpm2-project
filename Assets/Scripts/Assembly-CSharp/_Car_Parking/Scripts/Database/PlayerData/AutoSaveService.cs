using System;
using System.Threading;
using CPM.Account;
using CPM.Database;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	public class AutoSaveService : IAutoSaveService, IDisposable
	{
		public enum SaveResult
		{
			NoNeedToSave = 0,
			Saved = 1,
			ErrorShouldLogOutOnThisDevice = 2,
			Canceled = 3
		}

		public enum AutoSaveFrom
		{
			MainMenu = 0,
			MultiPlayerMenu = 1
		}

		private IDatabaseCarsService _databaseCarsService;

		private DataBase _dataBase;

		private CancellationTokenSource _autoSaveCts;

		private bool _isSaving;

		private int _pendingDelayedSaves;

		private bool _isPaused;

		private bool _wasSavedInMainMenuOnce;

		private readonly IRemoteConfigManager _remoteConfigManager;

		private static ConfigValuesStorage _remoteConfigData;

		public bool AutoSaveFromMainMenu => _remoteConfigData.autoSaveFromMainMenu;

		public bool AutoSaveFromMainMenuOneTime => _remoteConfigData.autoSaveFromMainMenuOneTime;

		public bool AutoSaveFromMainOnlinePlayerMenu => _remoteConfigData.autoSaveFromMainOnlinePlayerMenu;

		public static bool AutoSaveAllCarsAfterGetAllCars => _remoteConfigData.autoSaveAllCarsAfterGetAllCars;

		public static bool SaveCarsInAutoSafe => _remoteConfigData.saveCarsInAutoSave;

		public static bool AutoSaveCarAfterBuy => _remoteConfigData.autoSaveCarAfterBuy;

		private float DelayToAutoSaveInSec => _remoteConfigData.delayInAutoSaveInSec;

		private bool IsSaveIdle
		{
			get
			{
				if (!_isSaving)
				{
					return _pendingDelayedSaves == 0;
				}
				return false;
			}
		}

		[Inject]
		public AutoSaveService(IDatabaseCarsService databaseCarsService, DataBase dataBase, IRemoteConfigManager remoteConfigManager)
		{
			_remoteConfigManager = remoteConfigManager;
			_dataBase = dataBase;
			_databaseCarsService = databaseCarsService;
			Init().Forget();
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _autoSaveCts);
			GC.SuppressFinalize(this);
		}

		private async UniTask Init()
		{
			_remoteConfigData = _remoteConfigManager.GetDefaultConfigStorage();
			_remoteConfigData = await _remoteConfigManager.GetConfigValuesStorage();
		}

		public async UniTask<bool> WaitForOngoingSaveAsync(float timeoutSec)
		{
			CommonExtensions.CancelAndDisposeToken(ref _autoSaveCts);
			return !(await UniTask.WaitUntil(() => IsSaveIdle).TimeoutWithoutException(TimeSpan.FromSeconds(timeoutSec)));
		}

		public void Pause()
		{
			_isPaused = true;
		}

		public void Resume()
		{
			_isPaused = false;
		}

		public bool TrySendSaveData(AutoSaveFrom autoSaveFrom)
		{
			if (_isPaused)
			{
				return false;
			}
			if (!CheckNeedToSave(autoSaveFrom))
			{
				return false;
			}
			CommonExtensions.CancelAndDisposeToken(ref _autoSaveCts);
			_autoSaveCts = new CancellationTokenSource();
			SaveAfterDelay(autoSaveFrom, _autoSaveCts.Token).Forget();
			return true;
		}

		private async UniTask SaveAfterDelay(AutoSaveFrom autoSaveFrom, CancellationToken token)
		{
			_pendingDelayedSaves++;
			try
			{
				if (!(await UniTask.Delay(TimeSpan.FromSeconds(DelayToAutoSaveInSec), ignoreTimeScale: false, PlayerLoopTiming.Update, token).SuppressCancellationThrow()))
				{
					await Save(autoSaveFrom, token);
				}
			}
			finally
			{
				_pendingDelayedSaves--;
			}
		}

		public async UniTask<SaveResult> SaveDataIfNeedAsync(AutoSaveFrom autoSaveFrom, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!CheckNeedToSave(autoSaveFrom))
			{
				return SaveResult.NoNeedToSave;
			}
			return await Save(autoSaveFrom, cancellationToken);
		}

		private async UniTask<SaveResult> Save(AutoSaveFrom autoSaveFrom, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_isPaused)
			{
				return SaveResult.NoNeedToSave;
			}
			if (LoginInProgressContainer.IsLoginInProgress)
			{
				return SaveResult.NoNeedToSave;
			}
			_isSaving = true;
			try
			{
				if (!_dataBase.SynchronizingNow.Value)
				{
					if (await _dataBase.ShouldLogOutOnThisDevice())
					{
						return SaveResult.ErrorShouldLogOutOnThisDevice;
					}
					if (cancellationToken.IsCancellationRequested)
					{
						return SaveResult.Canceled;
					}
					await _dataBase.SendAllRecords(SaveCarsInAutoSafe, fastSave: true);
					if (cancellationToken.IsCancellationRequested)
					{
						return SaveResult.Canceled;
					}
					_dataBase.SendSlotsCollection().Forget();
					await _databaseCarsService.SendCachedData();
				}
				return SaveResult.Saved;
			}
			finally
			{
				_isSaving = false;
			}
		}

		private bool CheckNeedToSave(AutoSaveFrom autoSaveFrom)
		{
			switch (autoSaveFrom)
			{
			case AutoSaveFrom.MainMenu:
				if (AutoSaveFromMainMenuOneTime && !_wasSavedInMainMenuOnce)
				{
					_wasSavedInMainMenuOnce = true;
				}
				else if (!AutoSaveFromMainMenu)
				{
					return false;
				}
				break;
			case AutoSaveFrom.MultiPlayerMenu:
				if (!AutoSaveFromMainOnlinePlayerMenu)
				{
					return false;
				}
				break;
			}
			return true;
		}
	}
}
