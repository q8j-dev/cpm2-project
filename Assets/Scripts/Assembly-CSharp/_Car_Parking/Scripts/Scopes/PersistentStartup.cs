using System;
using System.Globalization;
using CPM.Connection;
using CPM.Database;
using CPM.RemoteConfig;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Persistence.Migration;
using _Car_Parking.Scripts.VinylsAi;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	public class PersistentStartup : IStartable, IDisposable
	{
		private const string VoiceChatAgeMigrationPrefKey = "VoiceChatEnabledForConfirmedAge";

		private readonly ConnectionHandler _connectionHandler;

		private readonly RideableMeshesGraph _rideableMeshesGraph;

		private readonly CoroutineHelper _coroutineHelper;

		private readonly IMigrationManager _migrationManager;

		private readonly PlayerDatas _playerDatas;

		private readonly PlayerDatas.FriendsUtility _friendsUtility;

		private readonly IConnectionDataHandler _connectionDataHandler;

		private readonly RuntimePlayerDataHandler _runtimePlayerDataHandler;

		private readonly IDatabaseSaveLoadService _databaseSaveLoadService;

		private readonly PartsInventorySaveLoadService _partsInventory;

		private readonly EngineSaveLoadService _engineSaveLoad;

		private readonly PlatesSaveLoadService _platesSaveLoad;

		private readonly PlayerStorageSaveLoadService _playerStorageSaveLoad;

		private readonly PlayerAnonStorageSaveLoadService _playerAnonStorageSaveLoad;

		private readonly WalletSaveLoadService _walletSaveLoad;

		private readonly FriendsSaveLoadService _friendsSaveLoad;

		private readonly SlotsOnDatabaseSaveLoadService _slotsSaveLoad;

		private readonly VinylsCheckApi _vinylsCheckApi;

		private readonly IRemoteConfigManager _remoteConfigManager;

		public PersistentStartup(ConnectionHandler connectionHandler, RideableMeshesGraph rideableMeshesGraph, CoroutineHelper coroutineHelper, IMigrationManager migrationManager, PlayerDatas playerDatas, PlayerDatas.FriendsUtility friendsUtility, IConnectionDataHandler connectionDataHandler, RuntimePlayerDataHandler runtimePlayerDataHandler, IDatabaseSaveLoadService databaseSaveLoadService, PartsInventorySaveLoadService partsInventory, EngineSaveLoadService engineSaveLoad, PlatesSaveLoadService platesSaveLoad, PlayerStorageSaveLoadService playerStorageSaveLoad, PlayerAnonStorageSaveLoadService playerAnonStorageSaveLoad, WalletSaveLoadService walletSaveLoad, FriendsSaveLoadService friendsSaveLoad, SlotsOnDatabaseSaveLoadService slotsSaveLoad, VinylsCheckApi vinylsCheckApi, IRemoteConfigManager remoteConfigManager)
		{
			_connectionHandler = connectionHandler;
			_rideableMeshesGraph = rideableMeshesGraph;
			_coroutineHelper = coroutineHelper;
			_migrationManager = migrationManager;
			_playerDatas = playerDatas;
			_friendsUtility = friendsUtility;
			_connectionDataHandler = connectionDataHandler;
			_runtimePlayerDataHandler = runtimePlayerDataHandler;
			_databaseSaveLoadService = databaseSaveLoadService;
			_partsInventory = partsInventory;
			_engineSaveLoad = engineSaveLoad;
			_platesSaveLoad = platesSaveLoad;
			_playerStorageSaveLoad = playerStorageSaveLoad;
			_playerAnonStorageSaveLoad = playerAnonStorageSaveLoad;
			_walletSaveLoad = walletSaveLoad;
			_friendsSaveLoad = friendsSaveLoad;
			_slotsSaveLoad = slotsSaveLoad;
			_vinylsCheckApi = vinylsCheckApi;
			_remoteConfigManager = remoteConfigManager;
		}

		public void Start()
		{
			EasySaveDataHandler.Initialize();
			PhotonCustomSerializer.Init();
			_connectionHandler.Initialize();
			WheelSurfaceConfigContainer.rideableMeshesGraph = _rideableMeshesGraph;
			_coroutineHelper.SetScope(CoroutineHelper.Scope.Global);
			_migrationManager.Initialize();
			_playerDatas.Init();
			_ = _friendsUtility;
			_ = _connectionDataHandler;
			_ = _runtimePlayerDataHandler;
			_databaseSaveLoadService.Init();
			_partsInventory.Init();
			_engineSaveLoad.Init();
			_platesSaveLoad.Init();
			_playerStorageSaveLoad.Init();
			_playerAnonStorageSaveLoad.Init();
			_walletSaveLoad.Init();
			_friendsSaveLoad.Init();
			_slotsSaveLoad.Init();
			InitVinylsCheckApi().Forget();
			JsonConvertCustomSettingsForDatabase.Init(PartDataExtensions.CheckPartData);
			EnableVoiceChatForConfirmedAgeOnce();
			TryCatchPrefsSubscription();
			CultureInfo.DefaultThreadCurrentUICulture = (CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US"));
			DebugManager.instance.enableRuntimeUI = false;
		}

		public void Dispose()
		{
			ObscuredPrefs.OnAlterationDetected -= OnAlterationDetected;
			ObscuredPrefs.OnPossibleForeignSavesDetected -= OnPossibleForeignSavesDetected;
		}

		private void EnableVoiceChatForConfirmedAgeOnce()
		{
			if (PlayerPrefs.GetInt("VoiceChatEnabledForConfirmedAge", 0) != 1)
			{
				PlayerPrefs.SetInt("VoiceChatEnabledForConfirmedAge", 1);
				if (PlayerPrefs.GetInt("privacyAgeConfirmed", 0) == 1)
				{
					PlayerPrefs.SetInt("VoiceChat", 1);
				}
			}
		}

		private void TryCatchPrefsSubscription()
		{
			ObscuredPrefs.OnAlterationDetected += OnAlterationDetected;
			ObscuredPrefs.OnPossibleForeignSavesDetected += OnPossibleForeignSavesDetected;
		}

		private async UniTaskVoid InitVinylsCheckApi()
		{
			_vinylsCheckApi.ApplyValues(_remoteConfigManager.GetDefaultConfigStorage());
			ConfigValuesStorage config = await _remoteConfigManager.GetConfigValuesStorage();
			_vinylsCheckApi.ApplyValues(config);
			await UniTask.Delay(TimeSpan.FromSeconds(3.0));
			if (config.modelVersion != 0)
			{
				_vinylsCheckApi.CheckAndUpdateModel($"model{config.modelVersion}").Forget();
			}
		}

		private static void OnPossibleForeignSavesDetected()
		{
		}

		private static void OnAlterationDetected()
		{
		}
	}
}
