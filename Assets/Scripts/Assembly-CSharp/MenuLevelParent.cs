using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AlmostEngine.Screenshot;
using CPM.Ads.Policy;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using GME_voice_chat;
using Photon.Pun;
using Service;
using Services.Drone;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Ads.UI;
using _Car_Parking.Scripts.Analytics;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Multiplayer.PhotonCustom;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.UIElement;

public class MenuLevelParent : MonoBehaviour
{
	public enum MenuIndexEnum
	{
		MainMenu = 0,
		LevelWin = 4,
		LevelFail = 5
	}

	public const string LowerVolumeOnVoiceChatPrefsKey = "lowerVolumeOnVoiceChat";

	public bool isMainMenu;

	public GameObject phonebtn;

	public GameObject[] menus;

	public TextMeshProUGUI LabelTop;

	public TextMeshProUGUI LabelMiddle;

	public GameObject LabelTopParent;

	public GameObject LabelMiddleParent;

	public TextMeshProUGUI timeLeftLabel;

	public TextMeshProUGUI FailLabel;

	public TextMeshProUGUI DoneLabel;

	public TextMeshProUGUI topDoneLabel;

	public TextMeshProUGUI topDoneLabel2;

	public FlexibleTextPanel TitleLabel;

	private float dashFuel;

	public GameObject FreeDriveWayponts;

	public GameObject[] ManualModeSprites;

	public GameObject LeftRight;

	public GameObject CancelShowRewardButton;

	public GameObject RewardedVideWatchButton;

	public GameObject ReverseSign;

	[SerializeField]
	private Button AdButton;

	[SerializeField]
	private GameObject AdvertisingPanel;

	[SerializeField]
	private CanvasGroup AdButtonCanvasGroup;

	[SerializeField]
	private TMP_Text AdCooldownTimerLabel;

	public Image LeftIndicatorArrowTC;

	public Image RightIndicatorArrowTC;

	public Image EmergencyTC;

	public Image HeadLampIconUI;

	public Sprite nearHeadLampTexture;

	public Sprite farHeadLampTexture;

	public Sprite noHeadLightTexture;

	public GameObject OhterLevelsButtonFailMenu;

	public GameObject NextLevelButtonFailMenu;

	public GameObject IndicatorsParent;

	public TextMeshProUGUI MoneyWonLevelLabel;

	[Space]
	public GameObject InsideCamControllParent;

	public GameObject InsideCamElements;

	public GameObject FixedCamElements;

	public GameObject exitRaceButton;

	public GameObject RecordButton;

	public GameObject mainMenuRewardVideoWindow;

	public GameObject makePoliceButton;

	public GameObject makePoliceWindow;

	public GameObject enableDisablePoliceCar;

	public GameObject FreeDriveMainParts;

	public float startTime;

	public ScreenshotManager screenshotManager;

	public AudioSource screenShotSound;

	public GameObject city;

	public GameObject city2;

	public GameObject Mountain;

	public GameObject carPriceEnterWindow;

	public GameObject GetAllCarButton;

	public GameObject[] pauseMenuParts;

	public Image[] pauseMenuButtons;

	public Color selectedPauseButton;

	public Color NotSelectedPauseButton;

	public GameObject endlessTimeCrashWindow;

	public GameObject removeAdsButtonLevels;

	public Shader treeHighShader;

	public Shader treeLowShader;

	public Material treeMat;

	public GameObject haloParent;

	public GameObject lightParent;

	public Transform[] ObjectsToMoveForIphoneX;

	[HideInInspector]
	public bool justPlayerConnected;

	public GameObject customLightSettingsMenu;

	public GameObject dynamicLightColorPicker;

	public Toggle focusToCar;

	public GameObject manageAccuntButton;

	public GameObject loadingOnMainMenu;

	public Texture[] TexturesOfPersons;

	public GameObject[] hideWhenChangingFarClip;

	public GameObject[] requestCircles;

	public GameObject[] allRequestWindows;

	private Addressable<Texture> textureAddressable;

	public ServiceLabel serviceLabels;

	public List<Service.Timer> timers = new List<Service.Timer>();

	public List<GameObject> sunnySkyboxToggles;

	public GameObject driftSign;

	public TextMeshProUGUI driftDistanceLabel;

	public GameObject filter;

	public GameObject carCharacteristics;

	public GameObject max8000Rpm;

	public GameObject max1600Rpm;

	public GameObject MoneyIconSellCar;

	public GameObject CoinIconSellCar;

	[HideInInspector]
	public JoyStickControl JoyStickControl;

	private bool alreadyPressedMenu;

	private bool _isCheckingOrientation;

	private UniTask _startLevelTask;

	[SerializeField]
	public GameObject parkingMenu;

	private MultiPlayer _multiPlayer;

	private OptionalDependency<MultiPlayer> _multiPlayerDep;

	private OptionalDependency<SkyboxBlender> _skyboxBlenderDep;

	private OptionalDependency<SceneControl> _sceneControlDep;

	private OptionalDependency<LevelScript> _levelScriptDep;

	private IObjectResolver _resolver;

	private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

	private OptionalDependency<QuickCarSettings> _quickCarSettingsDep;

	private OptionalDependency<CarSellTriggersCollection> _carSellTriggersCollectionDep;

	private OptionalDependency<LevelsAnalytic> _levelsAnalyticDep;

	private OptionalDependency<ChatControl> _chatControlDep;

	private OptionalDependency<IBusinessProfitControl> _businessProfitControlDep;

	private CoreGameManager _coreGameManager;

	private SkyboxBlender _skyboxBlender;

	private SceneControl _sceneControl;

	private LevelScript _levelScript;

	private IAdOfferService _adOffers;

	private AdsAnalytic _adsAnalytic;

	private IClientAdRewardConsumeService _adRewardConsume;

	private AdOfferTracker _adTracker;

	private bool _rewardedInProgress;

	private bool _adOfferSubscribed;

	private AdCooldownTimerView _adCooldownTimer;

	private readonly AdOfferMessages _adMessages = new AdOfferMessages();

	private const float DisabledAdAlpha = 0.4f;

	private static bool nativeResolutionTaken;

	private ManagerUI _managerUI;

	private IPrefabFactory _prefabFactory;

	private IDisposable _disposableafrerMainMenu;

	private IDisposable _disposableOnVoiceRecordReleased;

	private IDisposable _askPermissionDisposable;

	private IDisposable _updateYelloIconTick;

	private IDisposable _stopBlinkPlusDisposable;

	public bool isSteamDeck;

	private ChatControl _chatControl;

	private LevelRatingSender _levelsRatingSender;

	private QuickCarSettings _quickCarSettings;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private CarSellTriggersCollection _carSellTriggersCollection;

	private GlobalCollectionsProvider _globalCollectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private InputItemsProvider _inputItemsProvider;

	private LevelsAnalytic _levelsAnalytic;

	private OpenWorldAnalytic _openWorldAnalytic;

	private CarInputManager _carInputManager;

	private RacingProvider _racingProvider;

	private IBusinessProfitControl _businessProfitControl;

	private EventRegistry _eventRegistry;

	private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

	private bool voicePressed;

	private bool _isVolumesLowered;

	public bool dynamicLightColor;

	public byte Dummy;

	private AdCooldownTimerView AdCooldownTimer => _adCooldownTimer ?? (_adCooldownTimer = new AdCooldownTimerView(AdCooldownTimerLabel));

	public bool UseLowerVolumeOnVoice => PlayerPrefs.GetInt("lowerVolumeOnVoiceChat", 1) == 1;

	public bool isDroneEnabled { get; set; }

	public bool IsHidingHUD { get; set; }

	public bool RoundEdgeIphone { get; private set; }

	public bool IOSPlayer { get; private set; }

	public ParkingMenuUI ParkingMenuUI { get; private set; }

	public FPSDisplay2 FPSDisplay2 { get; set; }

	public Action OnCarStopped { get; set; }

	public Action OnCarStartMoving { get; set; }

	public UGUISteeringWheel SteeringWheel => _carInputManager.UGUISteeringWheel;

	private string ParkingContinueContextId => $"level:{_levelsProvider.CurrentLevelData?.ID}";

	private bool AdContinueDisabledForLevel
	{
		get
		{
			if ((bool)_levelsProvider.CurrentLevelData)
			{
				return _levelsProvider.CurrentLevelData.DisableAdContinue;
			}
			return false;
		}
	}

	public bool IsChangingQuality { get; set; }

	public event Action StartedLevel;

	public event Action RestartedLevel;

	public event Action VoiceRecordStarted;

	public event Action VoiceRecordReleased;

	[Inject]
	public void Construct(FreeDriveProvider freeDriveProvider, OnlineCollectionsProvider onlineCollectionsProvider, LevelsProvider levelsProvider, InputItemsProvider inputItemsProvider, ManagerUI managerUI, IPrefabFactory prefabFactory, LevelRatingSender ratingSender, GlobalCollectionsProvider globalCollectionsProvider, OpenWorldAnalytic openWorldAnalytic, CarInputManager carInputManager, RacingProvider racingProvider, EventRegistry eventRegistry, OptionalDependency<MultiPlayer> multiPlayer, OptionalDependency<SkyboxBlender> skyboxBlender, OptionalDependency<SceneControl> sceneControl, OptionalDependency<LevelScript> levelScript, IObjectResolver resolver, OptionalDependency<ParkingMenuUI> parkingMenuUI, OptionalDependency<QuickCarSettings> quickCarSettings, OptionalDependency<CarSellTriggersCollection> carSellTriggersCollection, OptionalDependency<LevelsAnalytic> levelsAnalytic, OptionalDependency<ChatControl> chatControl, OptionalDependency<IBusinessProfitControl> businessProfitControlDep, IAdOfferService adOffers, AdsAnalytic adsAnalytic, IClientAdRewardConsumeService adRewardConsume)
	{
		_adOffers = adOffers;
		_adsAnalytic = adsAnalytic;
		_adRewardConsume = adRewardConsume;
		_chatControlDep = chatControl;
		_inputItemsProvider = inputItemsProvider;
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_globalCollectionsProvider = globalCollectionsProvider;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_carSellTriggersCollectionDep = carSellTriggersCollection;
		_levelsRatingSender = ratingSender;
		_parkingMenuUIDep = parkingMenuUI;
		_resolver = resolver;
		_levelScriptDep = levelScript;
		_sceneControlDep = sceneControl;
		_skyboxBlenderDep = skyboxBlender;
		_multiPlayerDep = multiPlayer;
		_carInputManager = carInputManager;
		_quickCarSettingsDep = quickCarSettings;
		_managerUI = managerUI;
		_prefabFactory = prefabFactory;
		_levelsAnalyticDep = levelsAnalytic;
		_openWorldAnalytic = openWorldAnalytic;
		_racingProvider = racingProvider;
		_eventRegistry = eventRegistry;
		_businessProfitControlDep = businessProfitControlDep;
	}

	private void Awake()
	{
		_chatControl = _chatControlDep?.Value;
		_carSellTriggersCollection = _carSellTriggersCollectionDep?.Value;
		ParkingMenuUI = _parkingMenuUIDep?.Value;
		_levelScript = _levelScriptDep?.Value;
		_sceneControl = _sceneControlDep?.Value;
		_skyboxBlender = _skyboxBlenderDep?.Value;
		_multiPlayer = _multiPlayerDep?.Value;
		_quickCarSettings = _quickCarSettingsDep?.Value;
		_levelsAnalytic = _levelsAnalyticDep?.Value;
		_businessProfitControl = _businessProfitControlDep?.Value;
		PhotonNetwork.PrefabPool = new VContainerPrefabPool(_resolver);
		if (!nativeResolutionTaken)
		{
			nativeResolutionTaken = true;
		}
		InitAwake();
	}

	public void InitAwake()
	{
		_isCheckingOrientation = true;
		OrientationChecker().Forget();
		timers.Add(serviceLabels.mapTimer);
		timers.Add(serviceLabels.textTimer);
		RoundEdgeIphone = false;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			IOSPlayer = true;
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			IOSPlayer = false;
		}
		if ((bool)FreeDriveWayponts)
		{
			FreeDriveWayponts.gameObject.SetActive(value: true);
		}
		if ((bool)OhterLevelsButtonFailMenu && (bool)NextLevelButtonFailMenu)
		{
			OhterLevelsButtonFailMenu.SetActive(value: true);
			NextLevelButtonFailMenu.SetActive(value: false);
		}
		if ((bool)_chatControl)
		{
			ChatControl chatControl = _chatControl;
			chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Combine(chatControl.OnChatOpened, new Action<RectTransform, bool>(OnChatOpened));
		}
		int num = 0;
		num = 1;
		PlayerPrefs.GetInt("gamepad", num);
		if (SteamManager.IsInitialized())
		{
			SteamManager.IsSteamRunningOnSteamDeck();
		}
	}

	private async UniTaskVoid OrientationChecker()
	{
	}

	private void OnApplicationQuit()
	{
		SaveAllProfit();
		if ((bool)V.AnalyticWheres)
		{
			V.AnalyticWheres.Save();
		}
	}

	private void Start()
	{
		InitStart();
	}

	public void InitStart()
	{
		if (_levelScript == null)
		{
			_levelScript = _levelsProvider.LevelScript;
		}
		if (_sceneControl == null)
		{
			_sceneControl = V.SceneControl;
		}
		if (_skyboxBlender == null)
		{
			_skyboxBlender = TimeProvider.skyboxBlender;
		}
		if (_coreGameManager == null)
		{
			_coreGameManager = V.CoreGameManager;
		}
		InitAdOffer();
		startTime = Time.time;
		if ((bool)endlessTimeCrashWindow && ObscuredPrefs.GetInt("endlessCrashTime") == 1)
		{
			endlessTimeCrashWindow.gameObject.SetActive(value: false);
		}
		dynamicLightColor = true;
		if (sunnySkyboxToggles.Count > 0)
		{
			if (sunnySkyboxToggles[0] == null)
			{
				return;
			}
			int selectedSkyboxPrefs = TimeProvider.SelectedSkyboxPrefs;
			foreach (GameObject sunnySkyboxToggle in sunnySkyboxToggles)
			{
				sunnySkyboxToggle.SetActive(value: false);
			}
			sunnySkyboxToggles[selectedSkyboxPrefs].SetActive(value: true);
			TimeProvider.skyboxSelectedRuntime = selectedSkyboxPrefs;
			Debug.LogWarning("Skybox selected - set:" + selectedSkyboxPrefs);
		}
		FlagsStorage.HideAllTopLabel = PlayerPrefs.GetInt("hideAllTopLabel", 0) == 1;
		FlagsStorage.HideSkidSmoke = PlayerPrefs.GetInt("hideSkidSmoke", 0) == 1;
		FlagsStorage.HideWheelDirt = PlayerPrefs.GetInt("hideWheelDirt", 0) == 1;
		FlagsStorage.HideSkidMarks = PlayerPrefs.GetInt("hideSkidmarks", 0) == 1;
		_onMenuHandle.Get(_eventRegistry).Subscribe(OpenLevelPauseMenu);
	}

	private void CalcTraffic()
	{
		Debug.LogError("Menu level CalcTraffic not implemented");
	}

	private string FormatMileage(float mileage)
	{
		int num = (int)mileage;
		mileage -= (float)num;
		if (mileage > 0.95f)
		{
			num++;
		}
		return num.ToString("D4") + mileage.ToString("F1").Remove(0, 1);
	}

	private void CheckMileageReward(int mileage)
	{
		if (!FlagsStorage.IsFreeDriveMode || mileage == 0 || !V.Car || !(V.Car == _freeDriveProvider.FirstCarMultiPlayer))
		{
			return;
		}
		if (mileage % 1000 == 0)
		{
			int carId = V.Car.GetComponent<VehicleData>().CarId;
			if (ObscuredPrefs.GetInt("car" + carId + "MileageReward" + mileage) == 0)
			{
				ObscuredPrefs.SetInt("car" + carId + "MileageReward" + mileage, 1);
				_coreGameManager.SetMessage(string.Concat(mileage.ToString(), new LocalizedString("MenuLevelParent.KmMileageReward"), "\n+100 000!"), 4f);
				ObscuredInt value = 100000;
				CurrencyManagerController.Instance.AddMoney(new AdvancedInt(value));
			}
		}
		else if (mileage % 100 == 0)
		{
			int carId2 = V.Car.GetComponent<VehicleData>().CarId;
			if (ObscuredPrefs.GetInt("car" + carId2 + "MileageReward" + mileage) == 0)
			{
				ObscuredPrefs.SetInt("car" + carId2 + "MileageReward" + mileage, 1);
				_coreGameManager.SetMessage(string.Concat(mileage.ToString(), new LocalizedString("MenuLevelParent.KmMileageReward"), "\n+10 000!"), 4f);
				ObscuredInt value2 = 10000;
				CurrencyManagerController.Instance.AddMoney(new AdvancedInt(value2));
			}
		}
	}

	public void blinkPlus()
	{
		justPlayerConnected = true;
		_stopBlinkPlusDisposable?.Dispose();
		_stopBlinkPlusDisposable = Observable.Timer(TimeSpan.FromSeconds(4.0)).Subscribe((long _) =>
		{
			stopBlinkPlus();
		}).AddTo(this);
	}

	public void stopBlinkPlus()
	{
		justPlayerConnected = false;
	}

	public void OpenFailMenu()
	{
		_levelScript.OpenFailMenu();
	}

	public void OpenOtherLevelsMenu()
	{
		string additionalData = null;
		foreach (object value in Enum.GetValues(typeof(MenuIndexEnum)))
		{
			if (IsMenuActive((int)value))
			{
				additionalData = ((int)value).ToString();
				break;
			}
		}
		_levelScript.OpenOtherLevelsMenu();
		_managerUI.ShowParkingLevelsWindow(additionalData);
	}

	public void OpenLoadingMenu()
	{
		_levelScript.OpenLoadingMenu();
	}

	public void ShowConnectingMenu()
	{
		_levelScript.ShowConnectingMenu();
	}

	public void OpenSettingMenu()
	{
		_levelScript.OpenSettingMenu();
	}

	public void OpenFirstMenu()
	{
		_levelScript.OpenFirstMenu();
	}

	public void OpenSelectionManualShiftingMode()
	{
		_levelScript.OpenSelectionManualShiftingMode();
	}

	public void NextLevel()
	{
		_levelScript.NextLevel();
	}

	public void OpenMainMenu(bool withLog = true)
	{
		OpenMainMenuInternal(withLog).Forget();
	}

	private async UniTaskVoid OpenMainMenuInternal(bool withLog = true)
	{
		if (alreadyPressedMenu)
		{
			return;
		}
		alreadyPressedMenu = true;
		bool logFinish = withLog && !ScenesHelper.IsLapRacingSceneActive() && !ScenesHelper.IsFreeDriveSceneActive();
		CancellationToken lifetimeToken = this.GetCancellationTokenOnDestroy();
		if (FlagsStorage.IsFreeDriveMode)
		{
			await _managerUI.ShowWaitingWindowDark(set: true);
		}
		SaveAllProfit();
		if ((bool)_freeDriveProvider.FreeDriveDB)
		{
			_freeDriveProvider.FreeDriveDB.SendRating(toMainMenu: true).Forget();
		}
		if (FlagsStorage.IsMultiFree)
		{
			await UniTask.Delay(TimeSpan.FromMilliseconds(2000.0), ignoreTimeScale: false, PlayerLoopTiming.Update, lifetimeToken);
		}
		if (FlagsStorage.IsFreeDriveMode && IsLowRam())
		{
			_managerUI.ShowWaitingWindowDark(set: true);
			await UniTask.Delay(1000, ignoreTimeScale: true, PlayerLoopTiming.Update, lifetimeToken);
			if ((bool)V.SceneControl)
			{
				V.SceneControl.UnloadAll();
			}
			Debug.Log("unloading all becaus of low ram");
			Time.timeScale = 0f;
			if ((bool)V.LastUsedCar)
			{
				UnityEngine.Object.Destroy(V.LastUsedCar);
			}
			await UniTask.Delay(1000, ignoreTimeScale: true, PlayerLoopTiming.Update, lifetimeToken);
			PlayerDatas.UnloadUnusedAssets("LowRam OpenMainMenu");
			await UniTask.Delay(5000, ignoreTimeScale: true, PlayerLoopTiming.Update, lifetimeToken);
		}
		_levelScript.OpenMainMenu();
		if (logFinish)
		{
			_levelsAnalytic.LogFinishLevel("quit_manual");
		}
	}

	private bool IsLowRam()
	{
		int num = 1600;
		return SystemInfo.systemMemorySize <= num;
	}

	private void AfterMainMenu()
	{
		_levelScript.OpenMainMenu();
	}

	public void OpenLevelDoneMenu()
	{
		_levelScript.OpenLevelDoneMenu();
	}

	public void ChangeCam()
	{
		ChangeCamInternal();
	}

	private void ChangeCamInternal()
	{
		if ((bool)_levelScript)
		{
			_levelScript.ChangeCam();
		}
	}

	public void CancelRewardVideo()
	{
		_levelScript.CancelRewardVideo();
	}

	public void Back()
	{
		if (FlagsStorage.IsLevelScene)
		{
			_levelScript.Back();
		}
	}

	public void OpenPauseMenu()
	{
	}

	public void OpenLevelPauseMenu()
	{
		if (FlagsStorage.IsLevelScene)
		{
			SoundEventsDataHelper.SetGroupState(SoundEventsDataHelper.StateReverPause, SoundEventsDataHelper.OffReverState);
			_levelScript.IsLevelPaused = true;
			_managerUI.ShowLevelPauseWindow();
			_levelScript.CleanScreen();
			if ((bool)ParkingMenuUI)
			{
				ParkingMenuUI.Hide();
			}
		}
	}

	public void Resume()
	{
		_levelScript.Resume();
	}

	public void WatchAndContinue()
	{
		_levelScript.WatchAndContinue1();
	}

	private void InitAdOffer()
	{
		if (!_adOfferSubscribed && (bool)AdButton)
		{
			AdButton.onClick.AddListener(WatchAdAndContinue);
			_adOffers.Changed += RefreshAdOffer;
			if ((bool)_levelScript)
			{
				_levelScript.OnLevelFailMenuOpen += RefreshAdOffer;
			}
			_adOfferSubscribed = true;
			if (_adTracker == null)
			{
				_adTracker = _adsAnalytic.Track(AdPlacement.ParkingContinue, "MenuLevelParent");
			}
			RefreshAdOffer();
			PollAdOfferAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
	}

	private void DisposeAdOffer()
	{
		if (_adOfferSubscribed)
		{
			_adTracker?.Dispose();
			_adTracker = null;
			if ((bool)AdButton)
			{
				AdButton.onClick.RemoveListener(WatchAdAndContinue);
			}
			_adOffers.Changed -= RefreshAdOffer;
			if ((bool)_levelScript)
			{
				_levelScript.OnLevelFailMenuOpen -= RefreshAdOffer;
			}
			_adOfferSubscribed = false;
		}
	}

	private void RefreshAdOffer()
	{
		if ((bool)AdvertisingPanel)
		{
			if (AdContinueDisabledForLevel)
			{
				AdvertisingPanel.SetActive(value: false);
				SetAdButtonEnabled(enabled: false);
				AdCooldownTimer.SetCooldown(0f);
			}
			else
			{
				OfferState state = _adOffers.CanOffer(AdPlacement.ParkingContinue, ParkingContinueContextId);
				_adTracker?.Present(state);
				AdCooldownTimer.SetCooldown(_adMessages.CooldownSeconds(state));
				AdvertisingPanel.SetActive(state.IsOfferable);
				SetAdButtonEnabled(state.CanShowNow);
			}
		}
	}

	private void SetAdButtonEnabled(bool enabled)
	{
		if ((bool)AdButtonCanvasGroup)
		{
			AdButtonCanvasGroup.alpha = (enabled ? 1f : 0.4f);
		}
	}

	private void ShowBlockedMessage(OfferState state)
	{
		if (_adMessages.TryGetBlockedKey(state, out var key))
		{
			_coreGameManager.SetMessage(new LocalizedString(key), 3f);
		}
	}

	private async UniTaskVoid PollAdOfferAsync(CancellationToken token)
	{
		try
		{
			while (true)
			{
				await UniTask.Delay(1000, ignoreTimeScale: true, PlayerLoopTiming.Update, token);
				if (!_rewardedInProgress && (bool)AdvertisingPanel && AdvertisingPanel.activeInHierarchy)
				{
					RefreshAdOffer();
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void WatchAdAndContinue()
	{
		WatchAdAndContinueAsync().Forget();
	}

	private async UniTaskVoid WatchAdAndContinueAsync()
	{
		if (_rewardedInProgress)
		{
			return;
		}
		if (AdContinueDisabledForLevel)
		{
			RefreshAdOffer();
			return;
		}
		OfferState state = _adOffers.CanOffer(AdPlacement.ParkingContinue, ParkingContinueContextId);
		if (!state.CanShowNow)
		{
			ShowBlockedMessage(state);
			return;
		}
		_rewardedInProgress = true;
		_adTracker?.Accepted();
		try
		{
			OfferResult result = await _adOffers.TryShowAsync(AdPlacement.ParkingContinue, ParkingContinueContextId);
			_adTracker?.ShowFinished(result);
			if (result.IsGranted)
			{
				if ((bool)_levelScript)
				{
					_levelScript.WatchAndContinue2();
				}
				OfferRewards.TryTimeSeconds(result.Rewards, out var seconds);
				_adTracker?.RewardGranted("Time", (seconds > 0) ? new int?(seconds) : ((int?)null));
				_adRewardConsume.ConsumeInBackground(AdPlacement.ParkingContinue);
			}
		}
		finally
		{
			_rewardedInProgress = false;
			if ((bool)this)
			{
				RefreshAdOffer();
			}
		}
	}

	public void StartLevel()
	{
		if (_startLevelTask.Status != UniTaskStatus.Pending)
		{
			_levelScript.AttemptNumber = 0;
			_levelsAnalytic?.LogStartLevel();
			_startLevelTask = _levelScript.StartLevel();
			StartedLevel?.Invoke();
		}
	}

	public void StartLevelReplay()
	{
		if (_startLevelTask.Status != UniTaskStatus.Pending)
		{
			_inputItemsProvider.BrakeSliderControl?.transform.Rebuild().Forget();
			_levelsAnalytic?.LogRestartLevel();
			_startLevelTask = _levelScript.StartLevel();
			RestartedLevel?.Invoke();
		}
	}

	public void CleenForPersonView()
	{
		_levelScript.CleanForPersonView();
	}

	public void UnCleanScreen()
	{
		_levelScript.UnCleanScreen();
	}

	public void SetSteering()
	{
		if (_carInputManager.IgnoreAllInput)
		{
			SetVisualSteerControlOff();
			return;
		}
		bool active = false;
		bool active2 = false;
		if (!FlagsStorage.OutOfCar && !FlagsStorage.IsPassenger)
		{
			switch (V.Steering)
			{
			case 0:
				active2 = true;
				break;
			case 3:
				active = true;
				break;
			}
		}
		if ((bool)LeftRight)
		{
			LeftRight.SetActive(active);
		}
		if ((bool)_carInputManager.UGUISteeringWheel)
		{
			_carInputManager.UGUISteeringWheel.gameObject.SetActive(active2);
		}
		if (FlagsStorage.RacingGameMode && _carInputManager.FollowPoints)
		{
			SetVisualSteerControlOff();
		}
	}

	public void SetVisualSteerControlOff()
	{
		if ((bool)LeftRight)
		{
			LeftRight.SetActive(value: false);
		}
		if ((bool)_carInputManager.UGUISteeringWheel)
		{
			_carInputManager.UGUISteeringWheel.gameObject.SetActive(value: false);
		}
	}

	public void OnFarClipPressed()
	{
		if (V.MenuIndex != 2 || !hideWhenChangingFarClip[0].activeSelf)
		{
			return;
		}
		GameObject[] array = hideWhenChangingFarClip;
		foreach (GameObject gameObject in array)
		{
			if (!gameObject.name.Equals("far clip"))
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	public void OnFarClipChanged()
	{
		if (V.MenuIndex == 2)
		{
			GameObject[] array = hideWhenChangingFarClip;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: true);
			}
			ObscuredPrefs.SetBool("AutoFarClip", value: false);
			FPSDisplay2.AutoFarClip = false;
		}
	}

	public void EnableObjectInArray(GameObject[] array, int indexToEnable)
	{
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(indexToEnable == num);
			num++;
		}
	}

	public void OnSunnySkyboxChanged2()
	{
	}

	public void CheckForDisablePostProcessing()
	{
	}

	public void OnColorFilterChanged()
	{
	}

	private void MoveToCentr(float AddXPos, Transform[] toMove)
	{
		for (int i = 0; i < toMove.Length; i++)
		{
			toMove[i].transform.localPosition += new Vector3(AddXPos, 0f, 0f);
		}
	}

	public void OnVoiceRecordPressed()
	{
		VoiceRecordStart();
	}

	public void VoiceRecordStart()
	{
		if (PhotonNetwork.IsConnected && (bool)V.DriverPerson)
		{
			ChangeVolumesOnVoiceChat(lowerVolumes: true);
			_disposableOnVoiceRecordReleased?.Dispose();
			_disposableOnVoiceRecordReleased = Observable.Timer(TimeSpan.FromSeconds(15.0)).Subscribe((long _) =>
			{
				OnVoiceRecordReleased();
			}).AddTo(this);
			if (PlayerPrefs.GetInt("FirstSpeak", 0) == 0)
			{
				PlayerPrefs.SetInt("FirstSpeak", 1);
				_coreGameManager.SetMessage(new LocalizedString("MenuLevelParent.HoldAndSpeak"), 3f);
			}
			VoiceRecordStarted?.Invoke();
		}
	}

	public void OnVoiceRecordReleased()
	{
		if (PlayerPrefs.GetInt("VoiceChat", GMEVoiceChatManager.VoiceChatDefault) == 1 && PhotonNetwork.IsConnected && (bool)V.DriverPerson)
		{
			voicePressed = false;
			ChangeVolumesOnVoiceChat(lowerVolumes: false);
			_disposableOnVoiceRecordReleased?.Dispose();
			VoiceRecordReleased?.Invoke();
		}
	}

	public void OnDoubleMoneyPressed()
	{
		if (!Application.isEditor)
		{
			_levelScript.DoubleMoneyButton();
		}
	}

	public void UpdateFixedCamPos()
	{
		_coreGameManager.carCamerasController.SetPostionForCamStaticMode();
	}

	public void MakeScreenShot()
	{
		_coreGameManager.MakeScreenShot();
	}

	public void CloseCarSellWindow()
	{
		if ((bool)carPriceEnterWindow)
		{
			carPriceEnterWindow.gameObject.SetActive(value: false);
		}
	}

	public void OpenPausePart(GameObject obj)
	{
		PlayerDatas.DisableEnableAll(isEnable: false, pauseMenuParts);
		obj.SetActive(value: true);
		int num = 0;
		if (obj == pauseMenuParts[1])
		{
			num = 1;
		}
		else if (obj == pauseMenuParts[2])
		{
			num = 2;
		}
		else if (obj == pauseMenuParts[3])
		{
			num = 3;
		}
		if (pauseMenuButtons.Length != 0 && pauseMenuButtons[0] == null)
		{
			pauseMenuButtons[0].color = NotSelectedPauseButton;
			pauseMenuButtons[1].color = NotSelectedPauseButton;
			pauseMenuButtons[2].color = NotSelectedPauseButton;
			pauseMenuButtons[3].color = NotSelectedPauseButton;
			pauseMenuButtons[num].color = selectedPauseButton;
			pauseMenuButtons[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
			pauseMenuButtons[1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
			pauseMenuButtons[2].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
			pauseMenuButtons[3].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
			pauseMenuButtons[num].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
		}
		PlayerPrefs.SetInt("lastPauseWindow", num);
		if (FlagsStorage.IsFreeDriveMode)
		{
			_freeDriveProvider.QuickCarSettings.PreSetValues();
			OnFarClipChanged();
		}
	}

	public void SetTextForRacing()
	{
	}

	public void ChangeTreeShader()
	{
		treeMat.shader = ((treeMat.shader == treeLowShader) ? treeHighShader : treeLowShader);
	}

	public void ENABLEDISABELHALO()
	{
		haloParent.gameObject.SetActive(!haloParent.activeSelf);
	}

	public void ENABLEDISABLight()
	{
		lightParent.gameObject.SetActive(!lightParent.activeSelf);
	}

	public void OnFocusCarPressed(bool isOn)
	{
		_freeDriveProvider.CarCameras.Target = (isOn ? V.Car.transform : null);
	}

	public void HideUI(float? alpha = null)
	{
		if (!alpha.HasValue)
		{
			if (!IsHidingHUD)
			{
				IsHidingHUD = true;
			}
			else
			{
				IsHidingHUD = false;
			}
		}
	}

	private void OnChatOpened(RectTransform chatRect, bool open)
	{
		if (open)
		{
			HideEmoji();
		}
	}

	public void HideEmoji()
	{
		_managerUI.CloseWindow("EmoteLockerWindow");
	}

	public void ShowEmoji()
	{
		if (_managerUI.IsWindowActive("EmoteLockerWindow"))
		{
			HideEmoji();
		}
		else
		{
			_managerUI.ShowEmoteLockerWindow();
		}
	}

	public void PlayEmoji(int id)
	{
		V.DriverPerson.GetComponent<Emoji>().EmojiController(id);
		if ((bool)_freeDriveProvider.FreeDriveDB)
		{
			++_freeDriveProvider.FreeDriveDB.emotions;
		}
		HideEmoji();
	}

	public void DisableInputObject(GameObject obj)
	{
		if ((bool)obj)
		{
			obj.gameObject.SetActive(value: false);
		}
		if (obj == requestCircles[1])
		{
			MonoBehaviour.print("make car 2 null");
			_freeDriveProvider.RacingCar2 = null;
			_freeDriveProvider.FreeDriveControl.inviteToRaceButton.SetActive(active: true);
		}
	}

	public void ChangeVolumesOnVoiceChat(bool lowerVolumes)
	{
		if (UseLowerVolumeOnVoice && lowerVolumes != _isVolumesLowered && (lowerVolumes || !_onlineCollectionsProvider.PlayersOnlineNow.Any((MultiPersonControl mpc) => (bool)mpc && mpc.isSpeaking)))
		{
			_isVolumesLowered = !_isVolumesLowered;
			if (lowerVolumes)
			{
				Audio.SetAllVolumeBySavedVolume(0.125f);
			}
			else
			{
				Audio.ResetAllVolume();
			}
		}
	}

	public void ShowPhone()
	{
		if (FlagsStorage.InGarageFree)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("MenuLevelParent.CanTUseInGarage"), 3f);
			return;
		}
		_levelScript.SetActiveFalseEachMenu(17);
		_levelScript.CleanScreen();
		if ((bool)_freeDriveProvider.FreeDriveControl.chatControl)
		{
			_freeDriveProvider.FreeDriveControl.chatControl.HideAllChat();
		}
		Time.timeScale = 1f;
		Audio.SetAllVolumeBySavedVolume(0.5f);
	}

	public void ActiveDrone()
	{
		_levelScript.SetActiveFalseEachMenu(20);
		_levelScript.CleanScreen();
		if ((bool)_freeDriveProvider.MapCamera)
		{
			_freeDriveProvider.MapCamera.HideBigMap();
		}
		if ((bool)_freeDriveProvider.FreeDriveControl.chatControl)
		{
			if (FlagsStorage.IsChatAllowed)
			{
				_freeDriveProvider.FreeDriveControl.chatControl.ShowPartiallyChat();
			}
			else
			{
				_freeDriveProvider.FreeDriveControl.chatControl.HideAllChat();
			}
		}
		Time.timeScale = 1f;
		_freeDriveProvider.CarCameras.GetComponent<FreeCameraController>().OnOpen();
		Audio.ResetAllVolume();
	}

	public void DeactiveDrone()
	{
		FlagsStorage.IsEnabledJoy = true;
		isDroneEnabled = false;
		_levelScript.SetActiveFalseEachMenu(1);
		_levelScript.CleanScreen();
		if ((bool)_freeDriveProvider.FreeDriveControl.chatControl)
		{
			if (FlagsStorage.IsChatAllowed)
			{
				_freeDriveProvider.FreeDriveControl.chatControl.ShowPartiallyChat();
			}
			else
			{
				_freeDriveProvider.FreeDriveControl.chatControl.HideAllChat();
			}
		}
		Time.timeScale = 1f;
		_carInputManager.ClearPedals();
		UnCleanScreen();
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			HideEmoji();
		}
		HideUI(1f);
		_sceneControl.mainCamera.fieldOfView = 60f;
		if (FlagsStorage.OutOfCar)
		{
			_coreGameManager.carCamerasController.SetCamera(V.PersonCamIndex, V.DriverPerson.transform, newTarget: true);
		}
		else
		{
			_coreGameManager.carCamerasController.SetCamera(V.CurrentCamIndex, V.Car.transform, newTarget: false);
		}
	}

	private void OnDestroy()
	{
		_isCheckingOrientation = false;
		Time.timeScale = 1f;
		_carInputManager.ClearPedals();
		AnalyticStorage.TallRewardInSession = 0;
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			HideUI(1f);
		}
		if (Camera.main != null)
		{
			Camera.main.fieldOfView = 60f;
		}
		_onMenuHandle.Get(_eventRegistry).Unsubscribe(OpenLevelPauseMenu);
		if ((bool)_chatControl)
		{
			ChatControl chatControl = _chatControl;
			chatControl.OnChatOpened = (Action<RectTransform, bool>)Delegate.Remove(chatControl.OnChatOpened, new Action<RectTransform, bool>(OnChatOpened));
		}
		DisposeAdOffer();
	}

	private void SaveAllProfit()
	{
		_businessProfitControl?.Save();
	}

	public void DisableTaxi()
	{
		if (ObscuredPrefs.GetInt("taxi", 1) == 0 && PhotonNetwork.IsConnected)
		{
			phonebtn.SetActive(value: false);
		}
	}

	public void onChangeLightTogglePressed()
	{
		if (!customLightSettingsMenu)
		{
			return;
		}
		if (!customLightSettingsMenu.activeSelf)
		{
			customLightSettingsMenu.SetActive(value: true);
			FlagsStorage.LightTransformChangeMode = true;
			if (TimeProvider.skyboxSelectedRuntime == 2 || TimeProvider.skyboxSelectedRuntime == 3)
			{
				if (!_coreGameManager)
				{
					_coreGameManager = V.CoreGameManager;
				}
				if ((bool)_coreGameManager && (bool)_coreGameManager.directionalLight)
				{
					_coreGameManager.directionalLight.SetActive(value: true);
				}
			}
			RenderSettings.ambientMode = AmbientMode.Flat;
		}
		else
		{
			customLightSettingsMenu.SetActive(value: false);
		}
	}

	public void onChangeLightColorMode()
	{
		if (dynamicLightColor)
		{
			dynamicLightColor = false;
		}
		else
		{
			dynamicLightColor = true;
		}
	}

	public void onShowColorWindow()
	{
		if (dynamicLightColorPicker.activeSelf)
		{
			dynamicLightColorPicker.SetActive(value: false);
		}
	}

	public void OnResetLight()
	{
	}

	public void onSaveLight()
	{
		Color color = _coreGameManager.directionalLight.GetComponent<Light>().color;
		PlayerPrefs.SetFloat("lightColor_r", color.r);
		PlayerPrefs.SetFloat("lightColor_g", color.g);
		PlayerPrefs.SetFloat("lightColor_b", color.b);
		color = RenderSettings.ambientLight;
		PlayerPrefs.SetFloat("lightColorAmbient_r", color.r);
		PlayerPrefs.SetFloat("lightColorAmbient_g", color.g);
		PlayerPrefs.SetFloat("lightColorAmbient_b", color.b);
		Vector3 eulerAngles = _coreGameManager.directionalLight.transform.eulerAngles;
		PlayerPrefs.SetFloat("lightHeight", eulerAngles.x);
		PlayerPrefs.SetFloat("lightRotation", eulerAngles.y);
		PlayerPrefs.SetInt("isSetLight", FlagsStorage.LightTransformChangeMode ? 1 : 0);
	}

	private bool IsMenuActive(int index)
	{
		if (menus != null && menus.Length > index && menus[index] != null)
		{
			return menus[index].gameObject.activeSelf;
		}
		return false;
	}
}
