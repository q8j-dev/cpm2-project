using System;
using System.Threading;
using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using CPM.PlayerData;
using CPM.Rating;
using CPM.Rating.Entities;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Service;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

namespace CPM.Database
{
	public class FreeDriveDB : MonoBehaviour
	{
		private class car2ID
		{
			public int carID = default;

			public string carGeneratedID = default;
		}

		private const string AllGiftsSendKey = "AllGiftsCollectedSend";

		private const int MaxGifts = 88;

		private PlayerDataStorage _playerDataStorage;

		private IAccountController _accountController;

		private RatingSender _ratingController;

		private DailyTasksManager _dailyTasksManager;

		private IConnectionManager _connectionManager;

		private IDatabaseCarsService _databaseCarsService;

		private ManagerUI _managerUI;

		private ISlotService _slotService;

		public CarIDnStatus allStatusGlobal;

		private OnePlayerInListData _lastOnePlayerInListData;

		private MultiPlayerPosControl _multiPlayerPosControl;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private IDatabaseSaveLoadService _databaseSaveLoadService;

		private float _lastWalletFlushTime;

		private int _lastFlushedMoney = int.MinValue;

		private const float WalletFlushThrottleSec = 30f;

		public bool signedIn;

		public ObscuredInt CarToDelete;

		public int actionAfter;

		private bool _oneStatusReceived;

		public bool authenticated;

		private string uid;

		public float lastSendTime;

		private ObscuredInt _internalStateA;

		private ObscuredInt _systemFlagB;

		public ObscuredInt blockPost;

		private ObscuredInt _internalStateC;

		public ObscuredInt speedBanner;

		private ObscuredInt _trackingIdD;

		public ObscuredInt dragWins;

		public ObscuredInt police;

		private ObscuredInt _hiddenValueX;

		private ObscuredInt _systemFlagY;

		public ObscuredInt tireBurnt;

		public ObscuredInt fuelConsumed;

		private ObscuredInt _internalCounterZ;

		public ObscuredInt carWash;

		private ObscuredInt _auxiliaryStateA;

		public ObscuredInt carRepair;

		private ObscuredInt _controlValueB;

		public ObscuredInt emotions;

		public ObscuredInt r;

		private ObscuredInt _reservedFlagC;

		private ObscuredInt _internalStateD;

		public ObscuredInt push_ups;

		public ObscuredInt slicer_cut;

		private ObscuredInt _systemFlagE;

		public ObscuredInt car_collided;

		private MenuLevelParent _menuLevelParent;

		private CancellationTokenSource _ratingIntervalCts;

		private CancellationToken _destroyToken;

		private ISteamAchievementService _steamAchievementService;

		[Inject]
		public void Construct(IAccountController accountController, OnlineCollectionsProvider onlineCollectionsProvider, PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider, RatingSender ratingController, MenuLevelParent menuLevelParent, DailyTasksManager dailyTasksManager, IConnectionManager connectionManager, IDatabaseCarsService databaseCarsService, ManagerUI managerUI, ISlotService slotService, ISteamAchievementService steamAchievementService, IDatabaseSaveLoadService databaseSaveLoadService)
		{
			_databaseSaveLoadService = databaseSaveLoadService;
			_menuLevelParent = menuLevelParent;
			_steamAchievementService = steamAchievementService;
			_freeDriveProvider = freeDriveProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_slotService = slotService;
			_databaseCarsService = databaseCarsService;
			_connectionManager = connectionManager;
			_accountController = accountController;
			_playerDataStorage = playerDataStorage;
			_ratingController = ratingController;
			_dailyTasksManager = dailyTasksManager;
			_managerUI = managerUI;
		}

		private void Start()
		{
			CheckGiftsCollected().Forget();
		}

		private void Awake()
		{
			_destroyToken = this.GetCancellationTokenOnDestroy();
			_freeDriveProvider.FreeDriveDB = this;
			r = UnityEngine.Random.Range(0, 100);
			allStatusGlobal = new CarIDnStatus();
			ResetAll();
			uid = ManagerKeysForPrefs.AccountId;
			_connectionManager.CorrectHash.Get(out var value);
			if (value)
			{
				_ratingIntervalCts = CancellationTokenSource.CreateLinkedTokenSource(_destroyToken);
				RatingIntervalLoop(_ratingIntervalCts.Token).Forget();
			}
		}

		private async UniTaskVoid CheckGiftsCollected()
		{
			int num = 0;
			for (int i = 0; i < 100; i++)
			{
				if (ObscuredPrefs.GetInt("egg" + i) == 1)
				{
					num++;
				}
			}
			if (num == 88 && PlayerPrefs.GetInt("AllGiftsCollectedSend", 0) == 0)
			{
				int? gifts = num;
				SendRatingData(new RatingData(null, null, null, gifts), forceSend: true).Forget();
				PlayerPrefs.SetInt("AllGiftsCollectedSend", 1);
			}
		}

		private void ResetAll()
		{
			blockPost = r;
			speedBanner = r;
			police = r;
			dragWins = r;
			tireBurnt = r;
			carRepair = r;
			carWash = r;
			emotions = r;
			fuelConsumed = 0;
			push_ups = r;
			slicer_cut = r;
			car_collided = r;
		}

		private bool HasDataToSend()
		{
			if ((int)blockPost - (int)r > 0)
			{
				return true;
			}
			if ((int)speedBanner - (int)r > 0)
			{
				return true;
			}
			if ((int)police - (int)r > 0)
			{
				return true;
			}
			if ((int)dragWins - (int)r > 0)
			{
				return true;
			}
			if ((int)tireBurnt - (int)r > 0)
			{
				return true;
			}
			if ((int)carRepair - (int)r > 0)
			{
				return true;
			}
			if ((int)carWash - (int)r > 0)
			{
				return true;
			}
			if ((int)emotions - (int)r > 0)
			{
				return true;
			}
			if ((int)push_ups - (int)r > 0)
			{
				return true;
			}
			if ((int)slicer_cut - (int)r > 0)
			{
				return true;
			}
			return (int)car_collided - (int)r > 0;
		}

		private void OnDestroy()
		{
			_ratingIntervalCts?.Cancel();
			_ratingIntervalCts?.Dispose();
			_ratingIntervalCts = null;
			if (_freeDriveProvider != null)
			{
				_freeDriveProvider.FreeDriveDB = null;
			}
			_ratingController = null;
			_accountController = null;
			_playerDataStorage = null;
			_dailyTasksManager = null;
			_connectionManager = null;
			_databaseCarsService = null;
			_managerUI = null;
			_slotService = null;
			_multiPlayerPosControl = null;
			_onlineCollectionsProvider = null;
			_menuLevelParent = null;
			_freeDriveProvider = null;
		}

		public async UniTaskVoid ShowRankNotificationWindow(int rank)
		{
			PopupNotificationWindow popupNotificationWindow = await _managerUI.ShowPopupNotificationWindow();
			if (!_destroyToken.IsCancellationRequested)
			{
				Texture midTexture = UtilsRank.LoadRankTextureFromResources((UtilsRank.RankType)rank);
				popupNotificationWindow.InitRankNotification(rank, new LocalizedString("FreeDriveDB.NewRank"), midTexture);
			}
		}

		private void ResetMainMenuButton()
		{
		}

		public async UniTask SignINAndDeleteCar(int ToDelete, int action, MultiPlayerPosControl mp)
		{
			if (Application.isEditor)
			{
				MonoBehaviour.print("SignINAndDeleteCar: " + action);
			}
			CarToDelete = ToDelete;
			_multiPlayerPosControl = mp;
			actionAfter = action;
			IAccountController accountController = _accountController;
			_playerDataStorage.AccountID = ManagerKeysForPrefs.AccountId;
			string email = accountController.Email;
			if (await accountController.GetServiceAvailability() == ServiceAvailability.Available)
			{
				if (email == "0" && Application.isEditor)
				{
					MonoBehaviour.print("no account");
				}
				signedIn = await accountController.SignInWithPrefs();
				if (signedIn)
				{
					authenticated = true;
				}
				DelayedActionAfterDeleteCar(_destroyToken).Forget();
			}
		}

		private async UniTaskVoid DelayedActionAfterDeleteCar(CancellationToken ct)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			ActionAfterDeleteCar();
		}

		public async UniTask SignIN()
		{
			IAccountController accountController = _accountController;
			PlayerDataStorage playerDataStorage = _playerDataStorage;
			try
			{
				playerDataStorage.AccountID = ManagerKeysForPrefs.AccountId;
				if (!(ManagerKeysForPrefs.Email == "0") && await accountController.SignInWithPrefs())
				{
					authenticated = true;
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private void ActionAfterDeleteCar()
		{
		}

		private async UniTaskVoid SendTaskInternal(RatingData rating)
		{
			IAccountController accountController = _accountController;
			RatingSender ratingController = _ratingController;
			DailyTasksManager dailyTasksManager = _dailyTasksManager;
			if (accountController.IsAnonymous)
			{
				return;
			}
			if (!authenticated)
			{
				await SignIN();
			}
			if (accountController.Authorized)
			{
				string text = rating.ToJson();
				if (text != "}")
				{
					await ratingController.SendRating(text);
					dailyTasksManager.UpdateData(rating).Forget();
				}
			}
		}

		public void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus && !FlagsStorage.IsMainMenu)
			{
				SendRating(toMainMenu: false).Forget();
				TryFlushWalletToBackend();
			}
		}

		private void TryFlushWalletToBackend()
		{
			if (_databaseSaveLoadService == null || _accountController == null || !_accountController.Authorized)
			{
				return;
			}
			int num = CurrencyManagerController.Money;
			if (num != _lastFlushedMoney)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (!(realtimeSinceStartup - _lastWalletFlushTime < 30f))
				{
					_lastWalletFlushTime = realtimeSinceStartup;
					_lastFlushedMoney = num;
					_databaseSaveLoadService.SendWalletData(new WalletData(num)).Forget();
				}
			}
		}

		public void SendRatingNext(long _)
		{
			SendRating(toMainMenu: false).Forget();
		}

		private async UniTaskVoid RatingIntervalLoop(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested)
			{
				try
				{
					await UniTask.Delay(TimeSpan.FromMinutes(5.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				SendRating(toMainMenu: false).Forget();
			}
		}

		public async UniTaskVoid SendRating(bool toMainMenu)
		{
			_connectionManager.CorrectHash.Get(out var value);
			if (!_accountController.Authorized || !value || !FlagsStorage.IsFreeDriveMode || _freeDriveProvider.FirstCarMultiPlayer == null || V.DriverPerson == null)
			{
				return;
			}
			float num = Time.time - lastSendTime;
			bool flag = HasDataToSend();
			if (num < 30f && !flag && !toMainMenu)
			{
				return;
			}
			lastSendTime = Time.time;
			Powertrain component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<Powertrain>();
			float num2 = component.DistanceSession;
			component.DistanceSession = 0f;
			float num3 = component.DistanceDrifted;
			component.DistanceDrifted = 0f;
			float num4 = component.DistanceOffroadTravelled;
			component.DistanceOffroadTravelled = 0f;
			MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
			foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
			{
				if ((bool)multiPlayerPosControl && multiPlayerPosControl.gameObject != _freeDriveProvider.FirstCarMultiPlayer)
				{
					Powertrain component2 = multiPlayerPosControl.GetComponent<Powertrain>();
					if ((bool)component2)
					{
						num2 += (float)component2.DistanceSession;
						num3 += (float)component2.DistanceDrifted;
						num4 += (float)component2.DistanceOffroadTravelled;
						component2.DistanceSession = 0f;
						component2.DistanceDrifted = 0f;
						component2.DistanceOffroadTravelled = 0f;
					}
				}
			}
			PersonMileageTracker component3 = V.DriverPerson.GetComponent<PersonMileageTracker>();
			RatingData ratingData = new RatingData();
			if ((int)blockPost != (int)r)
			{
				ratingData.block_post = (int)blockPost - (int)r;
			}
			if ((int)speedBanner != (int)r)
			{
				ratingData.speed_banner = (int)speedBanner - (int)r;
			}
			if ((int)dragWins != (int)r)
			{
				ratingData.race_win = (int)dragWins - (int)r;
			}
			if ((int)police != (int)r)
			{
				ratingData.police = (int)police - (int)r;
			}
			if ((int)tireBurnt != (int)r)
			{
				ratingData.burnt_tire = (int)tireBurnt - (int)r;
			}
			if ((int)fuelConsumed != 0)
			{
				ratingData.fuel = (int)fuelConsumed;
			}
			if ((int)carWash != (int)r)
			{
				ratingData.car_wash = (int)carWash - (int)r;
			}
			if ((int)carRepair != (int)r)
			{
				ratingData.car_fix = (int)carRepair - (int)r;
			}
			if ((int)emotions != (int)r)
			{
				ratingData.reactions = (int)emotions - (int)r;
			}
			if ((int)push_ups != (int)r)
			{
				ratingData.push_ups = (int)push_ups - (int)r;
			}
			if ((int)slicer_cut != (int)r)
			{
				ratingData.slicer_cut = ((int)slicer_cut - (int)r) / 2;
			}
			if ((int)car_collided != (int)r)
			{
				ratingData.car_collided = (int)car_collided - (int)r;
			}
			if (num2 > 0.1f)
			{
				ratingData.t_distance = (float)Math.Round(num2, 3);
			}
			if (num2 > 0f)
			{
				GooglePlayAchievements.ReportProgress("CgkIuKqW0PcZEAIQFQ", "gpg_ach_free_drive_km", AchievementProgress.EffectiveFreeDriveKmTarget, num2, "free_drive_mileage");
			}
			if (ManagerKeysForPrefs.IsSteamLinked && num2 > 0f)
			{
				float current = _steamAchievementService.GetStatFloat("steam_free_drive_km") + num2;
				_steamAchievementService.ReportProgress("ACH_OPEN_ROADS", "steam_free_drive_km", 25f, current);
			}
			if (num3 > 0.01f)
			{
				ratingData.drift = (float)Math.Round(num3, 3);
			}
			if (num4 > 0.1f)
			{
				ratingData.offroad = (float)Math.Round(num4, 3);
			}
			if (num4 > 0f)
			{
				GooglePlayAchievements.ReportProgress("CgkIuKqW0PcZEAIQFw", "gpg_ach_offroad_km", AchievementProgress.EffectiveOffroadKmTarget, num4, "free_drive_mileage");
			}
			if (ManagerKeysForPrefs.IsSteamLinked && num4 > 0f)
			{
				float current2 = _steamAchievementService.GetStatFloat("steam_offroad_km") + num4;
				_steamAchievementService.ReportProgress("ACH_NO_PAVEMENT_NO_PROBLEM", "steam_offroad_km", 5f, current2);
			}
			ratingData.run = (float)Math.Round((float)component3.distanceTravelled, 3);
			if ((float)component3.mileagePassenger > 0f)
			{
				ratingData.passanger_distance = (float)Math.Round((float)component3.mileagePassenger, 5);
			}
			if ((float)component3.mileagePassenger > 0f)
			{
				GooglePlayAchievements.ReportProgress("CgkIuKqW0PcZEAIQIw", "gpg_ach_passenger_km", AchievementProgress.EffectivePassengerKmTarget, component3.mileagePassenger, "free_drive_mileage");
			}
			if ((float)component3.mileagePassenger > 0f)
			{
				float current3 = _steamAchievementService.GetStatFloat("steam_passenger_km") + (float)component3.mileagePassenger;
				_steamAchievementService.ReportProgress("ACH_RIDE_ALONG", "steam_passenger_km", 10f, current3);
			}
			if (toMainMenu)
			{
				int num5 = 0;
				for (int j = 0; j < 200; j++)
				{
					if (ObscuredPrefs.HasKey($"bestResultID{j}"))
					{
						num5++;
					}
				}
				if (num5 > 0)
				{
					ratingData.levels = num5;
				}
				ratingData.cars = _slotService.OccupiedSlotCount;
				int num6 = EstateCount();
				if (num6 != 0)
				{
					ratingData.real_estate = num6;
				}
			}
			component3.distanceTravelled = 0f;
			component3.mileagePassenger = 0f;
			float num7 = (float)V.AnalyticWheres.multiFreeDrive + (float)V.AnalyticWheres.singleFree + V.AnalyticWheres.level + (float)V.AnalyticWheres.DragRacing + (float)V.AnalyticWheres.CircuitRacing;
			if (num7 > 1f)
			{
				ratingData.time = (int)num7;
			}
			ResetAll();
			V.AnalyticWheres.Save();
			if (toMainMenu)
			{
				Global.PopUpWindows.Lock();
			}
			_freeDriveProvider.FreeDriveDB.SendRatingData(ratingData).Forget();
		}

		private int EstateCount()
		{
			int num = 0;
			for (int i = 0; i < 9; i++)
			{
				if (ObscuredPrefs.GetInt("FastFoodBuilding" + (i + 1)) == 1)
				{
					num++;
				}
			}
			if (ObscuredPrefs.GetInt("homeOpen0") == 1)
			{
				num++;
			}
			if (ObscuredPrefs.GetInt("homeOpen1") == 1)
			{
				num++;
			}
			if (ObscuredPrefs.GetInt("homeOpen2") == 1)
			{
				num++;
			}
			if (ObscuredPrefs.GetInt("homeOpen3") == 1)
			{
				num++;
			}
			return num;
		}

		public async UniTask<RatingResultAll> GetUserRating(string uid, CancellationToken cancellationToken)
		{
			RatingSender ratingController = _ratingController;
			if (!authenticated)
			{
				await SignIN();
				if (cancellationToken.IsCancellationRequested)
				{
					return null;
				}
			}
			RatingResultAll result = await ratingController.GetRating(uid);
			if (cancellationToken.IsCancellationRequested)
			{
				return null;
			}
			return result;
		}

		public async UniTaskVoid SendRatingData(RatingData rating, bool forceSend = false)
		{
			_connectionManager.CorrectHash.Get(out var value);
			if (value)
			{
				SendTaskInternal(rating).Forget();
			}
		}

		public async UniTask<bool> SignInForCheck()
		{
			if (Application.isEditor)
			{
				MonoBehaviour.print("SignIn");
			}
			IAccountController accountController = _accountController;
			_playerDataStorage.AccountID = ManagerKeysForPrefs.AccountId;
			string email = accountController.Email;
			if (await accountController.GetServiceAvailability() != ServiceAvailability.Available)
			{
				return false;
			}
			if (email == "0" && Application.isEditor)
			{
				MonoBehaviour.print("no account");
			}
			signedIn = await accountController.SignInWithPrefs();
			if (signedIn)
			{
				authenticated = true;
			}
			return true;
		}

		public void SyncRatingDataWithCorrespondingData(RatingResultAll ratingData)
		{
			if (ratingData != null)
			{
				PopulateEggs(ratingData.general);
			}
		}

		private void PopulateEggs(RatingData ratingData)
		{
			if (ratingData == null || !ratingData.gifts.HasValue)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < 88; i++)
			{
				if (ObscuredPrefs.GetInt($"egg{i}") == 1)
				{
					num++;
				}
			}
			int num2 = (ratingData.gifts - num).Value;
			for (int j = 0; j < 88; j++)
			{
				if (num2 <= 0)
				{
					break;
				}
				if (ObscuredPrefs.GetInt($"egg{j}") == 0)
				{
					ObscuredPrefs.SetInt($"egg{j}", 1);
					num2--;
				}
			}
		}
	}
}
