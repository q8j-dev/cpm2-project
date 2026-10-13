using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CPM.Scripts.Garage.GarageBuyer;
using CodeStage.AntiCheat.Common;
using CodeStage.AntiCheat.Genuine.CodeHash;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.Connection;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Persistence.Migration;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.VisualTuningFavourite;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;
using _Car_Parking.WwisePrefabs.WiseSoundScripts;

namespace CPM.Database
{
	public class DataBase : IInitializable, IDisposable
	{
		public const bool ChangeLocalIDFromServer = true;

		private readonly IGarageBuyService _garageBuyService;

		public ReactiveProperty<bool> SynchronizingNow;

		private PlayerDatas.FriendsUtility friendsUtility;

		private PlayerDataStorage playerDataStorage;

		private IDatabaseSaveLoadService databaseSaveLoadService;

		private IAccountController _accountController;

		private IDatabaseCarsService _databaseCarsService;

		private IConnectionManager _connectionManager;

		private IRemoteConfigManager _remoteConfigManager;

		private ConfigValuesStorage _configStorage;

		private PersonKitConfig _malePersonKitConfig;

		private PersonKitConfig _femalePersonKitConfig;

		public bool savePreset;

		private IPlayerStorageService _playerStorageService;

		private PresetGrantSession _presetGrantSession;

		private ICarInfoService _carInfoService;

		private OptionalDependency<ManagerUI> _managerUIDep;

		private ManagerUI _managerUI;

		private PartsInventorySaveLoadService _partsInventorySaveLoadService;

		private EngineSaveLoadService _engineSaveLoadService;

		private PlatesSaveLoadService _platesSaveLoadService;

		private FriendsSaveLoadService _friendsSaveLoadService;

		private IInventoryPartsService _inventoryPartsService;

		private IEngineKitsInventoryService _engineKitsInventoryService;

		private ProgressContainer _progressContainer;

		private ISlotService _slotService;

		private CurrencyManagerController _currencyManagerController;

		private ITimerTuningService _timerTuningService;

		private IEngineTuningTimerService _engineTuningTimerService;

		private bool _isHashChecked;

		private SlotsOnDatabaseSaveLoadService _slotsOnDatabaseSaveLoadService;

		private OptionalDependency<FreeDriveDB> _freeDriveDBDep;

		private FreeDriveDB _freeDriveDB;

		private AllWheelsCounterConfig _allWheelsCounterConfig;

		private IPoliceSystem _policeSystem;

		private readonly IObjectResolver _objectResolver;

		public IDatabaseSaveLoadService DatabaseSaveLoadService => databaseSaveLoadService;

		public IDatabaseCarsService DatabaseCarsService => _databaseCarsService;

		public ICarInfoService CarInfoService => _carInfoService;

		public ISlotService SlotService => _slotService;

		private bool loadingCarsInProgress { get; set; }

		[Inject]
		public DataBase(PlayerDatas.FriendsUtility friendsUtility, PlayerDataStorage playerDataStorage, IDatabaseSaveLoadService databaseSaveLoadService, IAccountController accountController, IDatabaseCarsService databaseCarsService, IConnectionManager connectionManager, ExternalSaveableDataAdapter externalSaveableDataAdapter, IRemoteConfigManager remoteConfigManager, PersonKitConfigs personKitConfigs, IPlayerStorageService playerStorageService, ICarInfoService carInfoService, ProgressContainer progressContainer, PartsInventorySaveLoadService partsInventorySaveLoadService, EngineSaveLoadService engineSaveLoadService, PlatesSaveLoadService platesSaveLoadService, IInventoryPartsService inventoryPartsService, IEngineKitsInventoryService engineKitsInventoryService, ISlotService slotService, CurrencyManagerController currencyManagerController, FriendsSaveLoadService friendsSaveLoadService, ITimerTuningService timerTuningService, IEngineTuningTimerService engineTuningTimerService, SlotsOnDatabaseSaveLoadService slotsOnDatabaseSaveLoadService, AllWheelsCounterConfig allWheelsCounterConfig, IPoliceSystem policeSystem, IGarageBuyService garageBuyService, IObjectResolver objectResolver, PresetGrantSession presetGrantSession, OptionalDependency<FreeDriveDB> freeDriveDB, OptionalDependency<ManagerUI> managerUI)
		{
			_allWheelsCounterConfig = allWheelsCounterConfig;
			_freeDriveDBDep = freeDriveDB;
			_slotsOnDatabaseSaveLoadService = slotsOnDatabaseSaveLoadService;
			_engineTuningTimerService = engineTuningTimerService;
			_timerTuningService = timerTuningService;
			_friendsSaveLoadService = friendsSaveLoadService;
			_currencyManagerController = currencyManagerController;
			_progressContainer = progressContainer;
			_engineKitsInventoryService = engineKitsInventoryService;
			_inventoryPartsService = inventoryPartsService;
			_platesSaveLoadService = platesSaveLoadService;
			_engineSaveLoadService = engineSaveLoadService;
			_partsInventorySaveLoadService = partsInventorySaveLoadService;
			_managerUIDep = managerUI;
			_carInfoService = carInfoService;
			_playerStorageService = playerStorageService;
			_remoteConfigManager = remoteConfigManager;
			_databaseCarsService = databaseCarsService;
			this.friendsUtility = friendsUtility;
			this.playerDataStorage = playerDataStorage;
			this.databaseSaveLoadService = databaseSaveLoadService;
			_accountController = accountController;
			_connectionManager = connectionManager;
			_malePersonKitConfig = personKitConfigs.Male;
			_femalePersonKitConfig = personKitConfigs.Female;
			_slotService = slotService;
			_policeSystem = policeSystem;
			_garageBuyService = garageBuyService;
			_objectResolver = objectResolver;
			_presetGrantSession = presetGrantSession;
			InitSubscriptions();
			SynchronizingNow = new ReactiveProperty<bool>();
		}

		public void Initialize()
		{
			_freeDriveDB = _freeDriveDBDep.Value;
			_managerUI = _managerUIDep.Value;
			Init().Forget();
		}

		public void Dispose()
		{
			if (_accountController != null)
			{
				_accountController.OnSignInAnon -= OnSignInAnonHandler;
			}
			if (_connectionManager != null)
			{
				_connectionManager.OnHashStatusChange -= OnHashStatusChange;
			}
			_freeDriveDB = null;
			_managerUI = null;
		}

		private void InitSubscriptions()
		{
			_accountController.OnSignInAnon -= OnSignInAnonHandler;
			_accountController.OnSignInAnon += OnSignInAnonHandler;
			_connectionManager.OnHashStatusChange -= OnHashStatusChange;
			_connectionManager.OnHashStatusChange += OnHashStatusChange;
		}

		public void ReInit()
		{
			_databaseCarsService.Init();
		}

		private async UniTaskVoid Init()
		{
			_configStorage = _remoteConfigManager.GetDefaultConfigStorage();
			_configStorage = await _remoteConfigManager.GetConfigValuesStorage();
			BlockAppStatusUtil.CheckNeedsToUpdateGame(_configStorage.needToUpdateGame == 1);
		}

		private void OnSignInAnonHandler()
		{
			OnSignInAnonHandlerAsync().Forget();
		}

		private async UniTaskVoid OnSignInAnonHandlerAsync()
		{
			Debug.Log("OnSignInAnonHandler");
			await UniTask.WaitUntil(() => _isHashChecked);
			await SendAllRecords(saveCars: false, fastSave: true);
		}

		private void OnHashStatusChange()
		{
			_isHashChecked = true;
		}

		private void OnSlotCreatedHandler(int obj)
		{
			TrySaveAfterSlotChanged().Forget();
		}

		public async UniTask<FunctionCallResult<FunctionResult>> SendPlayerRecordsAsync(bool fullData, bool isAnon)
		{
			PlayerStorage playerStorage = FetchPlayerRecordsForUpload().PlayerStorage;
			return await databaseSaveLoadService.SendPlayerRecordsPartially(playerStorage, fullData, isAnon);
		}

		public async UniTask<bool> SendAllRecords(bool saveCars = true, bool fastSave = false, bool useEndProgressContainer = true, float progressKof = 1f)
		{
			Debug.Log("SendAllRecords : saveCars = " + saveCars + " fastSave = " + fastSave);
			if (SynchronizingNow.Value)
			{
				return false;
			}
			if (!_accountController.Authorized)
			{
				return false;
			}
			if ((int)_connectionManager.H == 1)
			{
				Debug.Log("Unknown hash. Wait. ");
				await UniTask.Delay(TimeSpan.FromSeconds(5.0));
			}
			if ((int)_connectionManager.H == 1)
			{
				Debug.Log("Unknown hash 2. Dont save.");
				SynchronizingNow.Value = false;
				return false;
			}
			if ((int)_connectionManager.H == 0)
			{
				if (!fastSave)
				{
					ManagerUIHelper.SetMessage(new LocalizedString("DataBase.CantsaveprogressPleasedownloadlatestgv"), 5f);
				}
				SynchronizingNow.Value = false;
				return false;
			}
			try
			{
				SynchronizingNow.Value = true;
				_progressContainer.SetProgress(0.2f * progressKof, new LocalizedString("DataBase.SavingData"));
				PlayerStorageFromDataBase fetchPlayerRecordsForUpload = FetchPlayerRecordsForUpload();
				PlayerStorage playerStorage = fetchPlayerRecordsForUpload.PlayerStorage;
				bool isAnonPlayer = _accountController.IsAnonymous;
				FunctionCallResult<FunctionResult> functionCallResult;
				if (fastSave)
				{
					functionCallResult = await databaseSaveLoadService.SendPlayerRecordsPartially(playerStorage, fullData: false, isAnonPlayer);
				}
				else
				{
					_progressContainer.SetProgress(0.5f * progressKof, new LocalizedString("DataBase.SavingData"));
					functionCallResult = await databaseSaveLoadService.SendPlayerRecordsPartially(playerStorage, fullData: true, isAnonPlayer);
				}
				_playerStorageService.Data.ResetNameAndLocalId();
				_progressContainer.SetProgress(0.6f * progressKof, new LocalizedString("DataBase.SavingData"));
				if (functionCallResult.IsFail && functionCallResult.Status != FunctionStatus.LocalNoDataToSend)
				{
					Debug.LogError($"SendAllRecords: player records save failed. Status={functionCallResult.Status} ({(int)functionCallResult.Status}) Error={functionCallResult.ErrorMessage}");
					if (useEndProgressContainer)
					{
						await _progressContainer.EndAfterDelay(new LocalizedString("DataBase.CanTSaveProgress"), success: false);
					}
					ManagerUIHelper.SetMessage(new LocalizedString("DataBase.CantsaveprogressUnknownissue"), 5f);
					SynchronizingNow.Value = false;
					return false;
				}
				try
				{
					_progressContainer.SetProgress(0.7f * progressKof, new LocalizedString("DataBase.SavePlayerData"));
					await databaseSaveLoadService.SendWalletData(fetchPlayerRecordsForUpload.WalletData);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				if (!isAnonPlayer & saveCars)
				{
					await SendSlotsCollection();
				}
				if (!isAnonPlayer && saveCars)
				{
					await SendAllCars(sendNow: true, sendFull: false);
				}
				_progressContainer.SetProgress(0.9f * progressKof, new LocalizedString("DataBase.SavePlayerData"));
				if (!isAnonPlayer)
				{
					await UniTask.WhenAll(new List<UniTask>
					{
						SendInventory(),
						SendEngines(),
						SendPlates(),
						SendFriends()
					});
				}
				PlayerPrefs.SetInt("LastSyncDay", V.PassedDay);
				_progressContainer.SetProgress(0.95f * progressKof, new LocalizedString("DataBase.SavePlayerData"));
				if (useEndProgressContainer)
				{
					await _progressContainer.EndAfterDelay(new LocalizedString("DataBase.SavingComplete"));
				}
			}
			catch (Exception message2)
			{
				Debug.LogError(message2);
				if (useEndProgressContainer)
				{
					await _progressContainer.EndAfterDelay(new LocalizedString("DataBase.ErrorWhileSaving"), success: false);
				}
			}
			finally
			{
				SynchronizingNow.Value = false;
			}
			return true;
		}

		private async UniTask SendInventory()
		{
			try
			{
				if ((await _partsInventorySaveLoadService.Send(InventoryPartsToSave())).IsServerFail)
				{
					Debug.LogError("DATABASE: Error while saving parts inventory");
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private async UniTask SendEngines()
		{
			try
			{
				if ((await _engineSaveLoadService.Send(EngineDataToSave())).IsServerFail)
				{
					Debug.LogError("DATABASE: Error while saving engine inventory");
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private InventoryData InventoryPartsToSave()
		{
			InventoryData inventoryData = new InventoryData();
			inventoryData.Inventory = new List<PartData>();
			inventoryData.Inventory.AddRange(_inventoryPartsService.InventoryData.Inventory);
			List<PartData> partsInTimers = _timerTuningService.GetPartsInTimers();
			inventoryData.Inventory.AddRange(partsInTimers);
			List<PartData> partsInTimers2 = _engineTuningTimerService.GetPartsInTimers();
			inventoryData.Inventory.AddRange(partsInTimers2);
			return inventoryData;
		}

		private EngineKitsInventoryData EngineDataToSave()
		{
			EngineKitsInventoryData engineKitsInventoryData = new EngineKitsInventoryData();
			engineKitsInventoryData.Inventory = new List<EngineKitData>();
			engineKitsInventoryData.Inventory.AddRange(_engineKitsInventoryService.InventoryData.Inventory);
			List<EngineKitData> enginesInTimers = _timerTuningService.GetEnginesInTimers();
			engineKitsInventoryData.Inventory.AddRange(enginesInTimers);
			return engineKitsInventoryData;
		}

		private async UniTask SendPlates()
		{
			try
			{
				if ((await _platesSaveLoadService.Send(PlatesDataForDatabase.GetPlatesDataFromAccount())).IsServerFail)
				{
					Debug.LogError("DATABASE: Error while saving plates");
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private async UniTask SendFriends()
		{
			try
			{
				if (friendsUtility != null)
				{
					if (friendsUtility.Friends == null)
					{
						friendsUtility.RefillFriendsList();
					}
					if ((await _friendsSaveLoadService.Send(PrefsDataGeneratorForDatabase.GetFriendItems(friendsUtility))).IsServerFail)
					{
						Debug.LogError("DATABASE: Error while saving friends");
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		public async UniTask SendSlotsCollection(bool fullData = false)
		{
			try
			{
				_ = (await _slotsOnDatabaseSaveLoadService.TrySend(_slotService.GetSlotsForDatabase(), fullData)).IsServerFail;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private PlayerStorageFromDataBase FetchPlayerRecordsForUpload()
		{
			PlayerStorageFromDataBase playerStorageFromDataBase = new PlayerStorageFromDataBase();
			PlayerStorage data = _playerStorageService.Data;
			playerStorageFromDataBase.PlayerStorage = data;
			playerStorageFromDataBase.PlayerStorage.SetNameAndLocalId(_playerStorageService.Name, _playerStorageService.LocalID);
			playerStorageFromDataBase.WalletData = new WalletData
			{
				Money = CurrencyManagerController.Money
			};
			AdditionalPlayerProgress other = new PrefsDataGeneratorForDatabase().GenerateOtherData(friendsUtility, _policeSystem, _slotService);
			playerStorageFromDataBase.PlayerStorage.Other = other;
			VisualTuningFavouriteData visualTuningFavouriteData = new VisualTuningFavouriteData(Extensions.GetFavouriteVinyls(V.AllVinyls), _allWheelsCounterConfig.GetFavouriteWheels());
			playerStorageFromDataBase.PlayerStorage.VisualTuningFavouriteData = visualTuningFavouriteData;
			return playerStorageFromDataBase;
		}

		public async UniTask SendAllCars(bool sendNow, bool sendFull)
		{
			Dictionary<int, CarInfoForDatabase> allCars = _carInfoService.GetAllCars();
			await _databaseCarsService.SaveAllCarsInCache(allCars, sendFull);
			if (sendNow)
			{
				IDisposable updateProgress = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					_progressContainer.SetCarsProgress(_databaseCarsService.Progress.ProgressValue);
				});
				await _databaseCarsService.SendCachedData();
				updateProgress?.Dispose();
			}
		}

		public async UniTask<List<string>> GetAllCars(string accountId)
		{
			Dictionary<int, CarInfoForDatabase> allCars = _carInfoService.GetAllCars();
			return await _databaseCarsService.GetAllCarsJson(allCars, accountId);
		}

		public async UniTask<(string playerRecords, string wallet)> GetRecordsAndWalletAsync(string accountId)
		{
			await UniTask.Yield();
			PlayerStorageFromDataBase playerStorageFromDataBase = FetchPlayerRecordsForUpload();
			string playerRecords;
			using (_presetGrantSession?.Exclude(playerStorageFromDataBase.PlayerStorage))
			{
				playerRecords = GetPlayerRecords(accountId, playerStorageFromDataBase.PlayerStorage);
			}
			string walletData = GetWalletData(accountId, playerStorageFromDataBase.WalletData);
			return (playerRecords, walletData);
		}

		private string GetWalletData(string accountId, WalletData walletData)
		{
			string empty = string.Empty;
			if (walletData == null)
			{
				return empty;
			}
			new WalletDataHashes().CalculateChangedValues(walletData, out var changedData, fullData: true);
			if (changedData.Count == 0)
			{
				return empty;
			}
			return new MemoryPackDictionarySaveWalletData().Serialize(changedData, accountId);
		}

		private string GetPlayerRecords(string accountId, PlayerStorage playerStorage)
		{
			string empty = string.Empty;
			if (playerStorage == null)
			{
				return empty;
			}
			new PlayerStorageHashes().CalculateChangedValues(playerStorage, out var changedData, fullData: true);
			if (changedData.Count == 0)
			{
				return empty;
			}
			return new MemoryPackDictionarySavePlayerRecords().Serialize(changedData, accountId);
		}

		public void SignIN()
		{
			if (ManagerKeysForPrefs.HasEmail)
			{
				_accountController.SignInWithPrefs();
			}
		}

		private void SetPlayerRecords(PlayerStorageFromDataBase newPlayerStorageFromDataBase)
		{
			PlayerStorage playerStorage = newPlayerStorageFromDataBase.PlayerStorage;
			CurrencyManagerController.SetMoneyPrefs(new AdvancedInt(newPlayerStorageFromDataBase.WalletData.Money));
			MoneyChangedSound moneyChangedSound = _objectResolver?.Resolve<MoneyChangedSound>();
			if ((bool)moneyChangedSound)
			{
				moneyChangedSound.ResetMoneySound();
			}
			_playerStorageService.Data = playerStorage;
			PlayerPrefs.SetInt("hideMultiplayer", 0);
			_garageBuyService.ChangeToDefault();
			Dictionary<int, bool> buyingGaragesStatus = _garageBuyService.BuyingGaragesStatus;
			foreach (int userGarage in newPlayerStorageFromDataBase.UserGarages)
			{
				if (userGarage != 0)
				{
					int key = userGarage;
					buyingGaragesStatus[key] = true;
				}
			}
			_garageBuyService.SaveBuyingProgress();
			playerDataStorage.SetBasicPlayerData(playerStorage.Name, playerStorage.LocalID);
			playerStorage.ResetNameAndLocalId();
			PrefsDataGeneratorForDatabase.SavePrefsData(playerStorage.Other, friendsUtility, _policeSystem, _slotService);
			if (playerStorage.VisualTuningFavouriteData != null)
			{
				if (playerStorage.VisualTuningFavouriteData.FavouriteVinyls != null)
				{
					PrefsDataGeneratorForDatabase.SaveFavouriteVinylsDataToPrefs(playerStorage.VisualTuningFavouriteData.FavouriteVinyls);
				}
				if (playerStorage.VisualTuningFavouriteData.FavouriteWheels != null)
				{
					PrefsDataGeneratorForDatabase.SaveFavouriteWheelsDataToPrefs(playerStorage.VisualTuningFavouriteData.FavouriteWheels);
				}
			}
			_slotService.ResizeCollection();
			playerStorage.Other = null;
			_playerStorageService.Save();
			PlayerPrefs.Save();
			ObscuredPrefs.Save();
		}

		private void RestartMainMenu()
		{
			SceneManager.LoadScene(1, LoadSceneMode.Single);
		}

		public async UniTask LoadCars()
		{
			if ((bool)KeepAliveBehaviour<CodeHashGenerator>.Instance && KeepAliveBehaviour<CodeHashGenerator>.Instance.LastResult != null)
			{
				_ = KeepAliveBehaviour<CodeHashGenerator>.Instance.LastResult.SummaryHash;
			}
			_progressContainer.SetProgress(0.5f, new LocalizedString("DataBase.SavingCars"));
			List<CarInfoForDatabase> collection = new List<CarInfoForDatabase>();
			try
			{
				loadingCarsInProgress = true;
				RunFaceProgressTask().Forget();
				collection = await _databaseCarsService.GenericGetAllCars<CarInfoForDatabase>();
				_databaseCarsService.InitCarHashesPool();
			}
			catch (MigrationException ex)
			{
				Debug.LogError(ex);
				_ = ex.ExceptionCode;
				string text = string.Concat(new LocalizedString("DataBase.Somethingiswrongsendtodevs"), ":", ex.ExceptionCode.ToString());
				_ = _managerUI.ShowNotificationWindow(text, 5f, NotificationType.Error);
				_accountController.SignOut();
				return;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				LocalizedString localizedString = new LocalizedString("DataBase.ErrorWhileLoadingCarsTryAgainL");
				_ = _managerUI.ShowNotificationWindow(localizedString, 5f, NotificationType.Error);
				_accountController.SignOut();
				return;
			}
			finally
			{
				loadingCarsInProgress = false;
			}
			_progressContainer.SetProgress(0.95f, new LocalizedString("DataBase.SavingCars"));
			await LoadAllCarValues(collection);
		}

		private async UniTask RunFaceProgressTask()
		{
			while (loadingCarsInProgress)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0f, 3f)));
				float value = _progressContainer.Value + 0.01f;
				_progressContainer.SetProgress(value, new LocalizedString("DataBase.SavingCars"));
			}
		}

		public bool ApplyPlayerRecords(PlayerStorageFromDataBase records)
		{
			if (records == null)
			{
				return false;
			}
			try
			{
				SetPlayerRecords(records);
				_inventoryPartsService.SetData(records.InventoryData);
				_engineKitsInventoryService.SetData(records.EngineKits);
				PlateDataUtility.SavePlates(records.PlatesData);
				PrefsDataGeneratorForDatabase.SetFriends(records.FriendsCollection, friendsUtility);
				try
				{
					_slotService.SetSlotsFromDatabase(records.SlotsOnDatabaseCollection);
				}
				catch (Exception ex)
				{
					Debug.LogError("Set slots collection error:" + ex);
				}
				finally
				{
					if (records.SlotsOnDatabaseCollection == null || records.SlotsOnDatabaseCollection.Slots.IsNullOrEmpty())
					{
						PlayerPrefs.SetInt("ForceSaveSlots", 1);
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async UniTask LoadCarSlots()
		{
			string hash = "0";
			if ((bool)KeepAliveBehaviour<CodeHashGenerator>.Instance && KeepAliveBehaviour<CodeHashGenerator>.Instance.LastResult != null)
			{
				hash = KeepAliveBehaviour<CodeHashGenerator>.Instance.LastResult.SummaryHash;
			}
			SlotsOnDatabaseCollection slotsOnDatabaseCollection = await databaseSaveLoadService.GetSlotsOnDatabaseCollection(hash);
			if (slotsOnDatabaseCollection != null)
			{
				try
				{
					_slotService.SetSlotsFromDatabase(slotsOnDatabaseCollection);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
		}

		public async UniTask ApplyCarsAsync(List<CarInfoForDatabase> collection)
		{
			_databaseCarsService.InitCarHashesPool();
			await LoadAllCarValues(collection);
		}

		private async UniTask LoadAllCarValues(List<CarInfoForDatabase> collection)
		{
			if (collection == null)
			{
				collection = new List<CarInfoForDatabase>();
			}
			if (collection.Count == 0)
			{
				Debug.LogError("Empty list of cars fromDatabase. Cars was deleted from backend. Don't sign out.");
				if ((bool)_managerUI)
				{
					string text = new LocalizedString("DataBase.NoCarsOnAccount");
					_managerUI.ShowNotificationWindow(text, 5f, NotificationType.Error).Forget();
				}
			}
			else
			{
				await _carInfoService.SaveCarsFromDatabase(collection);
			}
		}

		private async UniTaskVoid TrySaveAfterSlotChanged()
		{
			if (!(await ShouldLogOutOnThisDevice()))
			{
				SendAllRecords(saveCars: false, fastSave: true).Forget();
			}
		}

		public async UniTask<bool> ShouldLogOutOnThisDevice()
		{
			return await _accountController.CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType.SaveData);
		}
	}
}
