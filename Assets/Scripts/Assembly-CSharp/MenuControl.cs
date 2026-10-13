using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.PlayerData;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem.InitializersWindows.PersonWindowInitializers;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

public class MenuControl : MonoBehaviour
{
	private const int minSuspensionStiffness = 8000;

	public GameObject ABS;

	public Color ambientColor;

	public GameObject awd;

	private Axles axles;

	public GameObject buyButton;

	private GameObject car;

	public Transform carPosInGarage;

	private float coff = 5f;

	private float coff_suspention = 0.3f;

	public Transform colorPicker;

	private Powertrain dr;

	public GameObject engines;

	public GameObject ESP;

	public GameObject Forhide;

	public GameObject fwd;

	public GameObject[] gears;

	public GameObject[] labels;

	private int lengthLevels;

	private CoreGameManager _coreGameManager;

	public GameObject[] menuItems;

	public GameObject[] moneyLabel;

	public GameObject NotOpenWolrdMainMenu;

	public GameObject OpnenWorldMainMenu;

	public GameObject others;

	public Transform[] personsForSelect;

	private Vector2 posTouch;

	private float _currentRenderScale = 0.6f;

	public int prevMenuIndex = -1;

	public bool firstLoading = true;

	public GameObject rwd;

	public GameObject[] sBars;

	public GameObject SelectedItem;

	public int SelectingPersonId;

	public bool selectingPersonNow;

	public string skinPrice = "";

	public GameObject[] sLabels;

	public Transform tempTarget;

	public Transform tempTargetFollowPoint;

	public GameObject[] tunning_menu;

	public GameObject WheelsRow;

	public GameObject StickersRow;

	public GameObject garageBoxLoaded;

	public List<int> engineLimitCarsV10;

	public List<int> engineLimitCarsV8;

	public GameObject personSceneParent;

	public bool insideGarage;

	public GameObject[] mainSceneAll;

	public GameObject w16OfferButton;

	public GameObject[] hideForF1Car;

	public GameObject signInRequiredWindow;

	[Header("Garage objects")]
	[Space]
	public GameObject garageMenu;

	public GameObject disableBackOnGarage;

	private PlayerDataStorage playerDataStorage;

	private DataBase dataBase;

	private IAccountController accountController;

	private MultiPlayerMenu multiPlayerMenu;

	private ManagerUI _managerUI;

	private GarageController _garageController;

	private Tweener _tempTargetMoveTween;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private MenuLevelParent _menuLevelParent;

	private ICharacterEquipmentDataService _characterEquipmentDataService;

	private IGarageSelectorService _garageSelectorService;

	private ProgressContainer _progressContainer;

	private IGarageBuyService _garageBuyService;

	private OpenWorldAnalytic _openWorldAnalytic;

	[Header("Update logs settings")]
	[Space]
	private readonly string lastLogsShownVersionKey = "lastLogsVersion";

	private InputMapSetEvent _inputMapSetEvent;

	private readonly string _makePoliceLabelDescriptionPart = " will be equipped with police lights.\n-You can fine players for speeding.";

	public DynoRunController DynoRunController { get; set; }

	public bool LoadingWaitingInProgress { get; set; }

	public event Action<int, int> OnMenuChanged;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, DataBase dataBase, MenuLevelParent menuLevelParent, ICharacterEquipmentDataService characterEquipmentDataService, IAccountController accountController, MultiPlayerMenu multiPlayerMenu, ManagerUI managerUI, IGarageSelectorService garageSelectorService, ProgressContainer progressContainer, IGarageBuyService garageBuyService, InputMapSetEvent inputMapSetEvent, OpenWorldAnalytic openWorldAnalytic, OptionalDependency<GarageController> garageController)
	{
		_characterEquipmentDataService = characterEquipmentDataService;
		_menuLevelParent = menuLevelParent;
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_garageController = garageController.Value;
		this.playerDataStorage = playerDataStorage;
		this.dataBase = dataBase;
		this.accountController = accountController;
		this.multiPlayerMenu = multiPlayerMenu;
		_managerUI = managerUI;
		_garageSelectorService = garageSelectorService;
		_progressContainer = progressContainer;
		_garageBuyService = garageBuyService;
		_openWorldAnalytic = openWorldAnalytic;
		_inputMapSetEvent = inputMapSetEvent;
	}

	private void Awake()
	{
		_garageSelectorService.Register(_progressContainer);
		_garageBuyService.Init();
		_levelsProvider.TryClearProvider();
	}

	private void Start()
	{
		_coreGameManager = GetComponent<CoreGameManager>();
		if ((bool)_menuLevelParent && (bool)_menuLevelParent.makePoliceWindow && (bool)_menuLevelParent.makePoliceWindow.transform.GetChild(2) && menuItems.Length >= 9)
		{
			menuItems[9].SetActive(value: false);
		}
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void updateReference()
	{
		car = V.Car;
		if ((bool)car)
		{
			dr = car.GetComponent<Powertrain>();
			axles = V.Car.GetComponent<Axles>();
			coff = (car.GetComponent<VehicleData>().isTruck ? 12 : 6);
			Initilize();
		}
	}

	private void SetBars()
	{
		if (!V.Car)
		{
			return;
		}
		int num = 7;
		for (int i = 0; i < 9; i++)
		{
			if (i >= num && i != 8)
			{
				gears[i].SetActive(value: false);
			}
		}
	}

	public void SetTogle()
	{
		if ((bool)V.Car)
		{
			awd.SetActive(value: true);
			rwd.SetActive(value: true);
			fwd.SetActive(value: true);
		}
	}

	public void HideElements(int i)
	{
		Forhide.SetActive(value: false);
		for (int j = 0; j < sBars.Length; j++)
		{
			if (sBars[j] != sBars[i])
			{
				sBars[j].SetActive(value: false);
			}
		}
	}

	public void ShowAll()
	{
		Forhide.SetActive(value: true);
		for (int i = 0; i < sBars.Length; i++)
		{
			sBars[i].SetActive(value: true);
		}
	}

	public void Hide0()
	{
		HideElements(0);
	}

	public void Hide1()
	{
		HideElements(1);
	}

	public void Hide2()
	{
		HideElements(2);
	}

	public void Hide3()
	{
		HideElements(3);
	}

	public void Hide4()
	{
		HideElements(4);
	}

	public void Hide5()
	{
		HideElements(5);
	}

	public void Hide6()
	{
		HideElements(6);
	}

	public void Hide7()
	{
		HideElements(7);
	}

	public void Hide8()
	{
		HideElements(8);
	}

	public void Hide9()
	{
		HideElements(9);
	}

	public void Hide10()
	{
		HideElements(10);
	}

	public void Initilize()
	{
		SetBars();
		ShowAll();
		SetTogle();
	}

	public void ResetDefault()
	{
		SetBars();
	}

	private void setGarageMenu()
	{
		if (FlagsStorage.InGarageMenu && V.Car.TryGetComponent<VehicleData>(out var component))
		{
			component.StopCar();
		}
		else if (!insideGarage)
		{
			V.CoreGameManager.SetTargetAfrter();
			insideGarage = true;
			_freeDriveProvider.CarCameras.yAngle = 20f;
			_freeDriveProvider.CarCameras.xAngle = 140f;
		}
	}

	public void ShowMainMenu()
	{
		ShowMainMenuAsync(base.destroyCancellationToken).Forget();
	}

	public async UniTask ShowMainMenuAsync(CancellationToken cancellationToken)
	{
		await UniTask.WaitUntil(() => !LoadingWaitingInProgress, PlayerLoopTiming.Update, cancellationToken);
		if (!cancellationToken.IsCancellationRequested)
		{
			V.CoreGameManager.CheckDeviceForShowUpdateAndroidWindow();
			_managerUI.ShowMainMenuWindow();
			OpenMenu(0);
			ObscuredPrefs.GetInt("SelectedPerson");
		}
	}

	public void GoSinglePlayerGame()
	{
		OpenMenu(2);
	}

	public void GoMultiPlayerGame()
	{
		OpenMenu(2);
	}

	public void ShowParkingModes()
	{
		OpenMenu(2);
	}

	public void OpenDyno()
	{
		DynoRunController.OnDynoOpen();
	}

	public void CloseRewardDayWindow()
	{
		ObscuredPrefs.SetInt("RewardedShowed", 1);
		ShowMainMenu();
	}

	public void OpenRewardedDayWindow()
	{
		OpenMenu(14);
	}

	public void OpenAdsConsentWindow()
	{
		OpenMenu(20);
	}

	public void OpenWorldSellingCarMenu()
	{
	}

	public void OpenWorldSellingCarMenuRev()
	{
		OpenMenu(23);
	}

	public void ConsentAgree()
	{
		PlayerPrefs.SetInt("Consent", 1);
		ShowMainMenu();
	}

	public void ConsentDisAgree()
	{
		PlayerPrefs.SetInt("Consent", 0);
		ShowMainMenu();
	}

	public void ShowCarSettingMenu()
	{
		OpenMenu(5);
	}

	public void ShowJustSettingMenu()
	{
		_managerUI.CloseWindow("MainMenuWindow");
		ShowJustSettingMenuInternal().Forget();
	}

	private async UniTaskVoid ShowJustSettingMenuInternal()
	{
		_managerUI.ShowSettingsWindow();
		if (await accountController.GetServiceAvailability() != ServiceAvailability.Available)
		{
			_menuLevelParent.manageAccuntButton.SetActive(value: true);
		}
	}

	public void ShowTuningMenu()
	{
		OpenMenu(4);
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(30);
		Scene sceneByBuildIndex2 = SceneManager.GetSceneByBuildIndex(_garageSelectorService.CurrentSceneIndex);
		if (sceneByBuildIndex == scene)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			personSceneParent = rootGameObjects[0];
			personSceneParent.transform.position = new Vector3(1532.6f, -37.118f, 2372.72f);
			_menuLevelParent.loadingOnMainMenu.SetActive(value: false);
			_managerUI.PersonWindowInitializer = new PersonWindowDefaultInitializer();
			_managerUI.ShowPersonCustomizationWindow();
		}
		else if (sceneByBuildIndex2 == scene)
		{
			if (!ScenesHelper.IsFreeDriveSceneActive())
			{
				SceneManager.SetActiveScene(scene);
			}
			_garageController.Init(V.Car);
		}
	}

	private void TeleportToGarage()
	{
		Transform transform = garageBoxLoaded.transform.Find("SpawnPoint");
		V.Car.transform.position = transform.position;
	}

	public void LoadPersonEditScene(string additionalData = "")
	{
		if ((bool)V.Car || (bool)V.DriverPerson)
		{
			_managerUI.PersonWindowInitializer = new PersonWindowDefaultInitializer();
			_managerUI.ShowPersonCustomizationWindow(additionalData);
		}
	}

	public void ShowGearBoxSetting()
	{
		OpenMenu(6);
	}

	public void ShowSuspentionSetting()
	{
		OpenMenu(5);
	}

	public void ShowCogSettings()
	{
		OpenMenu(8);
	}

	public void ShowNotEnoughMoney()
	{
		_managerUI.ShowMoneyPurchaseWindow();
	}

	public async UniTask ShowLoading(bool alwaysOnTop = true)
	{
		LoadingWaitingInProgress = true;
		if (!alwaysOnTop)
		{
			await _managerUI.ShowWaitingWindowDark(set: true);
		}
		else
		{
			await _managerUI.ShowWaitingWindow(set: true);
		}
		LoadingWaitingInProgress = false;
		if (!alwaysOnTop)
		{
			_managerUI.CloseWindow("SelectLocationWindow");
		}
		prevMenuIndex = V.MenuIndex;
		V.MenuIndex = 32;
		OnMenuChanged?.Invoke(prevMenuIndex, 32);
	}

	public void ShowIsMultiMenu()
	{
		OpenMenu(11);
	}

	public void ShowEnterName()
	{
		OpenMenu(12);
	}

	public void ShowSelectDriver(string windowAdditionalData = "")
	{
		LoadPersonEditScene(windowAdditionalData);
	}

	public void ShowCoinPurchase()
	{
		OpenMenu(22);
	}

	public void ShowFreeDriveMenu()
	{
		OpenMenu(16);
	}

	public void ShowMultiFreeMenu()
	{
		OpenMenu(17);
	}

	public void ShowWriteReviewWindow()
	{
		OpenMenu(19);
	}

	public void CloseWriteReviewWindow()
	{
		PlayerPrefs.SetInt("WriteReviewShown", 1);
		ShowMainMenu();
	}

	private void ShowWillRow()
	{
		StickersRow.SetActive(value: false);
		if (V.Car.GetComponent<VehicleData>().isTruck)
		{
			WheelsRow.SetActive(value: false);
		}
		else
		{
			WheelsRow.SetActive(value: true);
		}
	}

	public void OnWriteReviewButtonPressed()
	{
		CloseWriteReviewWindow();
		string identifier = Application.identifier;
		Analytics.CustomEvent("WriteReview", new Dictionary<string, object>
		{
			{ "Shown", 0 },
			{ "Pressed", 1 }
		});
		if (Application.platform == RuntimePlatform.Android)
		{
			Application.OpenURL("market://details?id=" + identifier);
			return;
		}
		_ = Application.platform;
		_ = 8;
	}

	public void ClosePersons(bool showMainMenuAfter = true)
	{
		if ((bool)personSceneParent)
		{
			personSceneParent.SetActive(value: false);
		}
		if (ScenesHelper.IsMenuSceneActive() && (bool)V.Car)
		{
			V.Car.gameObject.SetActive(value: true);
		}
		closePerson2(showMainMenuAfter);
	}

	public void CloseGarage()
	{
		_garageController.OnGarageClose();
		V.SceneControl.UnloadMainGarage();
	}

	public void OpenGarage()
	{
	}

	public void SitInCar()
	{
		if (FlagsStorage.OutOfCar)
		{
			if ((bool)_coreGameManager.CarOutInControl.CarToEnter)
			{
				_coreGameManager.CarOutInControl.InCar(V.DriverPerson, _coreGameManager.CarOutInControl.CarToEnter).Forget();
			}
		}
		else
		{
			_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car);
		}
	}

	private void closePerson2(bool showMainMenuAfter = true)
	{
		_freeDriveProvider.CarCameras.Target = (V.Car ? V.Car.transform : V.DriverPerson.transform);
		Transform[] array = personsForSelect;
		foreach (Transform transform in array)
		{
			if ((bool)transform)
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
		}
		selectingPersonNow = false;
		V.CoreGameManager.ClearAssets();
		if (showMainMenuAfter)
		{
			ShowMainMenu();
		}
	}

	public void GoRacing()
	{
	}

	public void closeAccountRequiredWindow()
	{
		signInRequiredWindow.SetActive(value: false);
	}

	public void OpenDailyTasksMenu()
	{
		if (ManagerKeysForPrefs.Email == "0")
		{
			V.CoreGameManager.menuControl.ShowSignInRequiredWindow(0);
		}
		else
		{
			OpenMenu(25);
		}
	}

	public void OpenDailyRewardsMenu()
	{
		if (ManagerKeysForPrefs.Email == "0")
		{
			V.CoreGameManager.menuControl.ShowSignInRequiredWindow(1);
		}
		else
		{
			OpenMenu(26);
		}
	}

	public void OpenShopWindow()
	{
		if (FlagsStorage.IsPurchaserInitialized)
		{
			_managerUI.CloseCurrentWindow();
			OpenMenu(28);
		}
	}

	private void CheckAndShowUpdateLogs()
	{
		Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
		{
			if (!string.Equals(PlayerPrefs.GetString(lastLogsShownVersionKey, ""), Application.version))
			{
				OpenUpdateLogsWindow();
				PlayerPrefs.SetString(lastLogsShownVersionKey, Application.version);
			}
		}).AddTo(this);
	}

	public void OpenUpdateLogsWindow()
	{
		OpenMenu(29);
	}

	public void OpenMenu(int menuIndex)
	{
		prevMenuIndex = V.MenuIndex;
		V.MenuIndex = menuIndex;
		DeactivateAllMenuExceptOne(menuIndex);
		ParticularCases(menuIndex);
		OnMenuChanged?.Invoke(prevMenuIndex, menuIndex);
	}

	public void ShowSignInRequiredWindow(int i)
	{
		_ = (string)(i switch
		{
			0 => new LocalizedString("MenuControl.InOrderToGetDailyTasksPleaseSi"), 
			1 => new LocalizedString("MenuControl.InOrderToGetDailyRewardsPlease"), 
			2 => new LocalizedString("MenuControl.InOrderToSaveYourProgressPleas"), 
			_ => throw new NotImplementedException($"case {i} is not implemented"), 
		});
		if ((bool)signInRequiredWindow)
		{
			signInRequiredWindow.SetActive(value: true);
		}
	}

	public void ShowOnlyMessagesMenu()
	{
		OpenMenu(27);
	}

	private void DeactivateAllMenuExceptOne(int activeMenuIndex)
	{
		if ((bool)signInRequiredWindow)
		{
			signInRequiredWindow.SetActive(value: false);
		}
		GameObject[] array = menuItems;
		foreach (GameObject gameObject in array)
		{
			if ((bool)gameObject)
			{
				if (activeMenuIndex == -1 || gameObject != menuItems[activeMenuIndex])
				{
					gameObject.SetActive(value: false);
				}
				else
				{
					gameObject.SetActive(value: true);
				}
			}
		}
		if (activeMenuIndex != 32)
		{
			_managerUI.CloseWaitingWindow();
			firstLoading = false;
		}
	}

	private void ParticularCases(int menuIndex)
	{
		_managerUI.ShowUserBalanceWindow();
		CurrencyManagerController.Instance.UpdateLabelsEvents();
		FlagsStorage.IgnoreTouchesOnMenu = menuIndex == 4 || menuIndex == 2 || menuIndex == 6 || menuIndex == 7 || menuIndex == 8 || menuIndex == 11 || menuIndex == 16 || menuIndex == 24;
		switch (menuIndex)
		{
		case 2:
			_coreGameManager.UpdateOverAllProgress();
			break;
		case 5:
			updateAlliTemes();
			break;
		case 6:
		case 7:
			updateReference();
			break;
		case 16:
			HideMakePoliceWindow();
			if ((bool)_coreGameManager.SellCarModel)
			{
				_coreGameManager.FromSellCars();
			}
			break;
		case 21:
			if ((bool)_freeDriveProvider.LeaderWindow)
			{
				_freeDriveProvider.LeaderWindow.OnLeaderBoardOpen();
			}
			break;
		case 25:
			_managerUI.ShowAchievmentsWindow();
			break;
		}
		if ((bool)V.Car && FlagsStorage.IsMainMenu && base.transform.GetChild(0).TryGetComponent<AudioSource>(out var component))
		{
			component.Play();
		}
	}

	private void AfterSetCarChar()
	{
		_managerUI.ShowUserBalanceWindow();
		CurrencyManagerController.Instance.UpdateLabelsEvents();
	}

	public void Check(GameObject parent)
	{
		int num = int.Parse(parent.name);
		if ((bool)SelectedItem)
		{
			int.Parse(SelectedItem.name);
		}
		buyButton.SetActive(value: true);
		if (num != 0 && SelectedItem != parent)
		{
			SelectedItem = parent;
		}
	}

	public void BuyItem(GameObject item)
	{
		if ((bool)SelectedItem)
		{
			int num = int.Parse(SelectedItem.name);
			PlayerPrefs.SetInt($"{num}_{V.CarId}", 1);
			CurrencyManagerController.Instance.UpdateLabelsEvents();
		}
	}

	public void updateAlliTemes()
	{
		foreach (Transform item in others.transform)
		{
			item.GetComponent<isBought>().updateStat();
		}
		foreach (Transform item2 in engines.transform)
		{
			item2.GetComponent<isBought>().updateStat();
			item2.gameObject.SetActive(value: true);
			int num = int.Parse(item2.name);
			bool flag = false;
			int num2 = 6;
			if ((int)V.CarId == 59 || (int)V.CarId == 133)
			{
				num2 = 4;
				flag = true;
			}
			if (engineLimitCarsV10.Contains(V.CarId))
			{
				num2 = 7;
				flag = true;
			}
			if (engineLimitCarsV8.Contains(V.CarId))
			{
				num2 = 6;
				flag = true;
			}
			if (num > num2)
			{
				V.CoreGameManager.GetArrayIndexInCarOreder(V.CarId);
				if (flag)
				{
					item2.gameObject.SetActive(value: false);
				}
			}
		}
	}

	public void ShowMakePoliceWindow()
	{
		_menuLevelParent.makePoliceWindow.SetActive(value: true);
		_menuLevelParent.FreeDriveMainParts.SetActive(value: false);
	}

	public void HideMakePoliceWindow()
	{
		_menuLevelParent.makePoliceWindow.SetActive(value: false);
		_menuLevelParent.FreeDriveMainParts.SetActive(value: true);
		if (ObscuredPrefs.GetInt($"CarPoliceBought{V.CarId}") == 1)
		{
			_menuLevelParent.makePoliceButton.SetActive(value: false);
			_menuLevelParent.enableDisablePoliceCar.SetActive(value: true);
		}
		else
		{
			_menuLevelParent.makePoliceButton.SetActive(value: true);
			_menuLevelParent.enableDisablePoliceCar.SetActive(value: false);
		}
		if ((bool)V.Car)
		{
			VehicleData component = V.Car.GetComponent<VehicleData>();
			component.LightsComposition.IsPolice = V.Car.GetComponent<VehicleData>().LightsComposition.IsPolice;
			if (component.LightsComposition.IsPolice)
			{
				component.LightsComposition.MakePoliceCar();
				ObscuredPrefs.SetInt("CarPoliceSet" + V.CarId.ToString(), 1);
				component.LightsComposition.PoliceLights.TurnOnLightsShort();
			}
			else
			{
				component.LightsComposition.DisablePolice();
				ObscuredPrefs.SetInt("CarPoliceSet" + V.CarId.ToString(), 0);
			}
		}
	}

	public void show_menu_suspention(GameObject obj)
	{
		int num = int.Parse(obj.name);
		int num2 = 0;
		GameObject[] array = tunning_menu;
		foreach (GameObject gameObject in array)
		{
			if (tunning_menu[num] == gameObject)
			{
				tunning_menu[num].SetActive(value: true);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
			num2++;
		}
	}

	private void UnloadAllScene()
	{
		int sceneCount = SceneManager.sceneCount;
		for (int i = 0; i < sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.IsValid() && sceneAt.isLoaded)
			{
				SceneManager.UnloadSceneAsync(sceneAt);
			}
		}
	}

	public void LoadFreeDrive(bool loadingWindowOnTop = true)
	{
		FlagsStorage.IsMultiFree = false;
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarFree, setRoot: true);
		StartLoadingFreeDriveAsync(loadingWindowOnTop).Forget();
	}

	private async UniTask StartLoadingFreeDriveAsync(bool loadingWindowOnTop = true)
	{
		ShowLoading(alwaysOnTop: false);
		Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex(_garageSelectorService.CurrentSceneIndex);
		if (sceneByBuildIndex.IsValid() && sceneByBuildIndex.isLoaded)
		{
			SceneManager.UnloadSceneAsync(sceneByBuildIndex);
		}
		if ((bool)V.Car)
		{
			UnityEngine.Object.Destroy(V.Car);
		}
		if ((bool)V.DriverPerson)
		{
			UnityEngine.Object.Destroy(V.DriverPerson);
		}
		UnloadAllScene();
		V.CoreGameManager.ClearAssets();
		Observable.Timer(TimeSpan.FromSeconds(0.5), Scheduler.MainThreadIgnoreTimeScale).Subscribe((long _) =>
		{
			LoadFreeDrive2().Forget();
		}).AddTo(this);
	}

	public void LoadMultiFreeDrive()
	{
		if (!ObscuredPrefs.HasKey("PlayerName"))
		{
			ShowEnterName();
		}
		else
		{
			OpenMultiFreeMenu();
		}
	}

	public void ShowSingleFreeDriveMenu()
	{
		OpenMenu(18);
		multiPlayerMenu.BuyHouseCancel();
	}

	public void ShowSelect3GameMode()
	{
		OpenMenu(20);
	}

	public void ShowLoadingAndLoadMultiFreeDrive()
	{
		FlagsStorage.IsMultiFree = true;
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.CarFree, setRoot: true);
		ShowLoading(alwaysOnTop: false);
		SceneManager.GetSceneByBuildIndex(_garageSelectorService.CurrentSceneIndex);
		Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			LoadFreeDrive2().Forget();
		}).AddTo(this);
	}

	public void OpenMultiFreeMenu()
	{
		ShowMultiFreeMenu();
		multiPlayerMenu.OpenMultiPlayerMenu();
	}

	public async UniTaskVoid LoadFreeDrive2()
	{
		if (dataBase.SynchronizingNow.Value)
		{
			Debug.Log("Saving progress... so waiting before load free drive");
			if (await UniTask.WaitUntil(() => !dataBase.SynchronizingNow.Value).TimeoutWithoutException(TimeSpan.FromSeconds(60.0)))
			{
				Debug.LogWarning("DataBase sync still running after 60s — loading Free Drive anyway.");
			}
		}
		_levelsProvider.CurrentLevelData = null;
		V.CoreGameManager = null;
		WheelConstructor.DynamicCanvasTire = null;
		WheelConstructor.DynamicCanvasNormal = null;
		AsyncSceneSwitcher.Load(13);
	}

	public void ShowAccountWindow()
	{
		_managerUI.CloseCurrentWindow();
		OpenMenu(21);
	}

	public void ShowLeaderBoard()
	{
		_managerUI.CloseCurrentWindow();
		OpenMenu(21);
	}

	public void ShowRatingWindow()
	{
		_managerUI.CloseCurrentWindow();
		OpenMenu(24);
	}

	public void Back()
	{
		menuItems[9].gameObject.SetActive(value: false);
		FlagsStorage.OnExteriorMenu = false;
		if (FlagsStorage.InGarageMenu && (bool)garageMenu)
		{
			garageMenu.SetActive(value: true);
		}
	}

	public void ResetColorPicker()
	{
		colorPicker.transform.localPosition = new Vector3(-96f, 123f, 0f);
	}

	public void MoveTempTarget(Vector3 pos, float duration = 0.5f)
	{
		_tempTargetMoveTween?.Kill();
		_tempTargetMoveTween = tempTarget.DOMove(pos, duration);
	}
}
