using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Balance;
using CPM.Database;
using CPM.Scripts.Garage;
using CPMEngine;
using Cinemachine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DanielLochner.Assets.SimpleScrollSnap;
using Newtonsoft.Json;
using Racing.CameraManagement;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

public class DealerWindow : WindowScript, IBackableWindow
{
	public enum FilterType
	{
		CarType = 0,
		CarRegion = 1,
		CarClass = 2
	}

	private const float MainWindowFadeDuration = 0.2f;

	[FormerlySerializedAs("newCarUIElementPrefab")]
	[Header("CarSlider")]
	[SerializeField]
	private DealerUIElement dealerUIElementPrefab;

	[SerializeField]
	private FaderCanvas MainFader;

	[SerializeField]
	private ConfirmPopupBuyNewCar ConfirmPopupBuyNewCar;

	[SerializeField]
	private Transform parent;

	[SerializeField]
	private Sprite goldIcon;

	[SerializeField]
	private Color goldColor;

	[SerializeField]
	private Sprite moneyIcon;

	[SerializeField]
	private Color moneyColor;

	[SerializeField]
	private TextMeshProUGUI currentCarPrice;

	[SerializeField]
	private Image currentCarMoneyTypeIcon;

	[SerializeField]
	private SimpleScrollSnap SimpleScrollSnapPrefab;

	[SerializeField]
	private Transform ScrollRectPosition;

	[SerializeField]
	private DealerWindowFiltersController DealerWindowFiltersController;

	[SerializeField]
	private Scrollbar Scrollbar;

	[SerializeField]
	private ScrollSnapBar ScrollSnapBar;

	[SerializeField]
	private TextMeshProUGUI CounterText;

	[Header("CurrentCarSpec")]
	[SerializeField]
	private TextMeshProUGUI hpText;

	[SerializeField]
	private TextMeshProUGUI torqueText;

	[SerializeField]
	private TextMeshProUGUI maxSpeedText;

	[SerializeField]
	private TextMeshProUGUI maxAccText;

	[SerializeField]
	private TextMeshProUGUI transmissionText;

	[SerializeField]
	private TextMeshProUGUI weightText;

	[SerializeField]
	private TextMeshProUGUI AvalibleEnginesText;

	[SerializeField]
	private TextMeshProUGUI AvalibleGearboxesText;

	[SerializeField]
	private TextMeshProUGUI ClassText;

	[SerializeField]
	private CanvasGroup CanvasGroup;

	[SerializeField]
	[Header("CurrentCarStyleChange")]
	private GameObject StyleRoot;

	[SerializeField]
	private Button nextStyle;

	[SerializeField]
	private Button prevStyle;

	[SerializeField]
	private TextMeshProUGUI styleText;

	[SerializeField]
	private ArrowsLeftRight BodyKitsArrows;

	[SerializeField]
	private Toggle ShowBodyKitsToggle;

	[SerializeField]
	private Button ResetStyleAndBodyButton;

	[SerializeField]
	private Button EngineSoundButton;

	[Header("CarPlaceholder")]
	[SerializeField]
	private Transform carPlaceholderParent;

	[SerializeField]
	private RawImage carPlaceholderImage;

	private bool _suppressPlaceholderForClick;

	private bool _clickSnapInProgress;

	private int _clickSnapTargetCarId = -1;

	private int _lastRequestedLoadCarId = -1;

	[SerializeField]
	private Image EngineSoundButtonImage;

	[SerializeField]
	private ButtonBuyNewCar BuyDefaultButton;

	[SerializeField]
	private Button backButton;

	[SerializeField]
	private Button removePreset;

	[SerializeField]
	private Button getFromWW;

	[SerializeField]
	private Button savePriceButton;

	[SerializeField]
	private TextMeshProUGUI priceInput;

	[SerializeField]
	private Button showPresetSettingButton;

	[SerializeField]
	private TMP_Text OfferTimerText;

	[SerializeField]
	private GameObject OfferTimerRoot;

	[SerializeField]
	private ScrollSnapSoundComponent ScrollSnapSound;

	private readonly int _createPerFrame = 10;

	private const int VisibleRadius = 8;

	private readonly Dictionary<int, Texture> _carTextures = new Dictionary<int, Texture>();

	private readonly List<RectTransform> _panelPlaceholders = new List<RectTransform>();

	private readonly List<DealerUIElement> _cellPool = new List<DealerUIElement>();

	private readonly Stack<DealerUIElement> _freeCells = new Stack<DealerUIElement>();

	private readonly List<int> _bindTmp = new List<int>();

	private Transform _poolHolder;

	private CancellationTokenSource _textureCts;

	private readonly Dictionary<ObscuredInt, NewVehicleData> _newCarsDict = new Dictionary<ObscuredInt, NewVehicleData>();

	private readonly Dictionary<CarType, List<int>> _carTypeDic = new Dictionary<CarType, List<int>>();

	private readonly Dictionary<CarRegion, List<int>> _carRegionDic = new Dictionary<CarRegion, List<int>>();

	private Dictionary<int, DealerUIElement> _newCarUIElements = new Dictionary<int, DealerUIElement>();

	private VehicleStockTuningData _currentVehicleStockTuningData;

	private SimpleScrollSnap _simpleScrollSnap;

	private CarType _currentCarType;

	private CarRegion _currentCarCountry;

	private int _maxCarsCount;

	private int _lastNearestPanelId;

	private ObscuredInt _lastCarId = -1;

	private ObscuredInt _pricePreset;

	private ResourceRequest _carEntityGm;

	private bool _ifFirstInit;

	private bool _haveEmptySlot;

	private bool _toDestroy;

	private Tween _tween;

	private AllVinylPreset _lastPreset;

	private DealerUIElement _currentDealerUIElement;

	private ICarTuningFactory _carTuningFactory;

	private CoreGameManager _coreGameManager;

	private IPrefabFactory _prefabFactory;

	private ISlotService _slotService;

	private IPurchaseSystem _purchaseSystem;

	private GarageController _garage;

	private CameraManager _cameraManager;

	private AllCarsInfo _allCarsInfo;

	private NewVehicleData _currentNearestCarData;

	private GameObject _cachedSellModel;

	private SellCarValues _cachedSellValues;

	private IOfferSystem _offerSystem;

	private DealerWindowAdditionalData _dealerWindowAdditionalData;

	private NewCarsMarkController _newCarsMarkController;

	private OfferTimerController _offerTimerController;

	private VirtualPurchaseAnalytic _virtualPurchaseAnalytic;

	private AnalyticEventFactory _analyticEventFactory;

	private EngineSound _engineSound;

	private CancellationTokenSource _engineSoundCts;

	private IOfflineModeSystem _offlineModeSystem;

	private IGuestAccountCreator _guestAccountCreator;

	private EventRegistry _eventRegistry;

	private RegistryHandle<BoolEvent> _onThrottleHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnThrottle);

	private ISteamAchievementService _steamAchievementService;

	private CarShopVisualService _carShopVisualService;

	private CarShopPurchaseService _carShopPurchaseService;

	private const string AlterDragCatcherName = "AlterDragCatcher";

	private float _carouselWheelCooldown;

	private int _forcedTargetCarId;

	private bool _creatingCellsNow;

	private (List<VehicleStockTuningData>, bool)? _args;

	private DataBase _dataBase;

	private CurrencyManagerController _currencyManagerController;

	private CancellationTokenSource _cancellationTokenSourceCreateCells;

	private float _fakeRedline;

	private float _fakeTq;

	private float _fakeInertia;

	private TurboType _fakeTurboType;

	private float _fakeMaxTqRpm;

	private float _fakeDisplacementCc;

	private bool _fakeIsRotary;

	private float[] _fakeTorqueCurve;

	private float _fakeIdleRpm = 800f;

	private const float DesktopRevealTimeoutSeconds = 4f;

	private const int DesktopRevealReadyRadius = 3;

	private const int DesktopRevealBrainSettleFrames = 2;

	private bool _desktopRevealInFlight;

	private const string SteamAcquiredClassMaskKey = "steam_garage_classes_mask";

	private const int SteamVehicleClassesToCollect = 5;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.DynoRun;

	public bool ShowBodyKits
	{
		get
		{
			if (ShowBodyKitsToggle.isOn)
			{
				return ShowBodyKitsToggle.gameObject.activeInHierarchy;
			}
			return false;
		}
	}

	public bool ConfirmPopupActive => ConfirmPopupBuyNewCar.gameObject.activeSelf;

	public event Action<bool> OnShowBodyKitsChanged;

	[Inject]
	public void Construct(ICarTuningFactory carTuningFactory, CoreGameManager coreGameManager, IPrefabFactory prefabFactory, ISlotService slotService, ManagerUI managerUI, GarageController garage, CameraManager cameraManager, DataBase dataBase, AllCarsInfo allCarsInfo, EventRegistry eventRegistry, CurrencyManagerController currencyManagerController, IPurchaseSystem purchaseSystem, IOfferSystem offerSystem, NewCarsMarkController newCarsMarkController, VirtualPurchaseAnalytic virtualPurchaseAnalytic, IOfflineModeSystem offlineModeSystem, AnalyticEventFactory analyticEventFactory, IGuestAccountCreator guestAccountCreator, CarShopVisualService carShopVisualService, CarShopPurchaseService carShopPurchaseService, ISteamAchievementService steamAchievementService)
	{
		_carShopVisualService = carShopVisualService;
		_carShopPurchaseService = carShopPurchaseService;
		_steamAchievementService = steamAchievementService;
		_currencyManagerController = currencyManagerController;
		_dataBase = dataBase;
		_cameraManager = cameraManager;
		_garage = garage;
		_eventRegistry = eventRegistry;
		_coreGameManager = coreGameManager;
		_carTuningFactory = carTuningFactory;
		_prefabFactory = prefabFactory;
		_slotService = slotService;
		_managerUI = managerUI;
		_allCarsInfo = allCarsInfo;
		_purchaseSystem = purchaseSystem;
		_offerSystem = offerSystem;
		_newCarsMarkController = newCarsMarkController;
		_virtualPurchaseAnalytic = virtualPurchaseAnalytic;
		_offlineModeSystem = offlineModeSystem;
		_analyticEventFactory = analyticEventFactory;
		_offerTimerController = new OfferTimerController(OfferTimerText, null, OfferTimerRoot);
		_guestAccountCreator = guestAccountCreator;
	}

	protected override void Awake()
	{
		int num = PlayerPrefs.GetInt("verPreset", 0);
		num++;
		PlayerPrefs.SetInt("verPreset", num);
		_coreGameManager.dealerWindow = this;
		backButton.onClick.AddListener(Back);
		nextStyle.onClick.AddListener(OnNextStylePressed);
		prevStyle.onClick.AddListener(OnPrevStylePressed);
		removePreset.onClick.AddListener(RemoveThisPreset);
		getFromWW.onClick.AddListener(GetMostLikedCarsFromWw);
		showPresetSettingButton.onClick.AddListener(ShowEditPreset);
		savePriceButton.onClick.AddListener(SavePricePreset);
		showPresetSettingButton.SetActive(Application.isEditor && _dataBase.savePreset);
		savePriceButton.transform.parent.gameObject.SetActive(value: false);
		_managerUI.OnSettingsButtonActivated?.Invoke(obj: false);
		_managerUI.OnAchievmentsButtonActivated?.Invoke(obj: false);
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		V.OnCarLiveryEnd = (Action)Delegate.Combine(V.OnCarLiveryEnd, new Action(GoToNextCar));
		if (_garage.LastSelectedCar == -1)
		{
			_garage.LastSelectedCar = _coreGameManager.CarOrder[0];
		}
		if (_slotService.HaveCarInAnyGarage || !_offlineModeSystem.Disabled)
		{
			InitBackButton();
		}
		if (_dealerWindowAdditionalData != null)
		{
			_forcedTargetCarId = _dealerWindowAdditionalData.TargetCarId;
			if (_dealerWindowAdditionalData.UseFilter && _dealerWindowAdditionalData.ShowOnlySpecial)
			{
				DealerWindowFiltersController.ShowOnlySpecial();
			}
			else
			{
				DealerWindowFiltersController.ShowAll();
			}
		}
		Init(DealerWindowFiltersController.GetCars(), isFirstInit: true);
		base.gameObject.SetActive(value: false);
		_managerUI.CurrentDealerWindow = this;
		_haveEmptySlot = _slotService.SlotCollection.TryGetEmptySlot(out var _);
	}

	protected override void OnEnable()
	{
		ShowBodyKitsToggle.onValueChanged.AddListener(ShowBodyKitsToggleChanged);
		ResetStyleAndBodyButton.onClick.AddListener(OnResetStyleAndBodyClicked);
		EngineSoundButton.onClick.AddListener(EngineSoundCheck);
		_onThrottleHandle.Get(_eventRegistry).Subscribe(EngineSoundAlterInput);
		_coreGameManager.OnSellCarValuesRefChanged += OnSellCarValuesRefChanged;
		ArrowsLeftRight bodyKitsArrows = BodyKitsArrows;
		bodyKitsArrows.OnClick = (Action<ArrowsLeftRight, int>)Delegate.Combine(bodyKitsArrows.OnClick, new Action<ArrowsLeftRight, int>(OnBodyKitArrowClick));
		ConfirmPopupBuyNewCar.Confirmed += BuyNewCarConfirmPressed;
		ConfirmPopupBuyNewCar.Canceled += BuyNewCarCancelPressed;
		DealerWindowFiltersController.OnFilterChanged += FilterChangedCallback;
		OnSellCarValuesRefChanged(new CoreGameManager.SellCarValuesRefChangedData(_coreGameManager.SellCarValues, null));
		base.OnEnable();
	}

	private void OnDisable()
	{
		ShowBodyKitsToggle.onValueChanged.RemoveListener(ShowBodyKitsToggleChanged);
		ResetStyleAndBodyButton.onClick.RemoveListener(OnResetStyleAndBodyClicked);
		EngineSoundButton.onClick.RemoveListener(EngineSoundCheck);
		_onThrottleHandle.Get(_eventRegistry)?.Unsubscribe(EngineSoundAlterInput);
		UISelectionCursor.SetPreferredFallbackSelectable(null);
		_coreGameManager.OnSellCarValuesRefChanged -= OnSellCarValuesRefChanged;
		if ((bool)_coreGameManager.SellCarValues)
		{
			_coreGameManager.SellCarValues.OnChanged -= OnSellCarValuesChanged;
		}
		ArrowsLeftRight bodyKitsArrows = BodyKitsArrows;
		bodyKitsArrows.OnClick = (Action<ArrowsLeftRight, int>)Delegate.Remove(bodyKitsArrows.OnClick, new Action<ArrowsLeftRight, int>(OnBodyKitArrowClick));
		ConfirmPopupBuyNewCar.Confirmed -= BuyNewCarConfirmPressed;
		ConfirmPopupBuyNewCar.Canceled -= BuyNewCarCancelPressed;
		DealerWindowFiltersController.OnFilterChanged -= FilterChangedCallback;
	}

	private void UpdatePreferredGamepadCar()
	{
		if (!(_simpleScrollSnap == null) && _newCarUIElements.TryGetValue(_simpleScrollSnap.CenteredPanel, out var value) && (bool)value)
		{
			UISelectionCursor.SetPreferredFallbackSelectable(value.SelectableObject);
		}
	}

	private void OnResetStyleAndBodyClicked()
	{
		_coreGameManager.SellCarValues.ResetCustomization();
	}

	private void EngineSoundAlterInput(bool isStarted = false)
	{
		if (isStarted && (bool)_engineSound)
		{
			_engineSound.StartDemoRevCycle();
		}
	}

	private void SetCarouselVisible(bool visible)
	{
		if (!(_simpleScrollSnap == null) && !(_simpleScrollSnap.ScrollRect == null))
		{
			CanvasGroup component = _simpleScrollSnap.ScrollRect.transform.GetComponent<CanvasGroup>();
			if (!(component == null))
			{
				component.alpha = (visible ? 1f : 0f);
				component.blocksRaycasts = visible;
				component.interactable = visible;
			}
		}
	}

	private void HandleCarouselWheel()
	{
		if (_carouselWheelCooldown > 0f)
		{
			_carouselWheelCooldown -= Time.unscaledDeltaTime;
		}
		if (_simpleScrollSnap == null || _simpleScrollSnap.ScrollRect == null || ConfirmPopupActive)
		{
			return;
		}
		Mouse current = Mouse.current;
		if (current == null)
		{
			return;
		}
		float y = current.scroll.ReadValue().y;
		if (Mathf.Abs(y) < 0.01f || _carouselWheelCooldown > 0f || !IsPointerOverCarousel(current.position.ReadValue()))
		{
			return;
		}
		int numberOfPanels = _simpleScrollSnap.NumberOfPanels;
		if (numberOfPanels > 0)
		{
			int num = ((!(y > 0f)) ? 1 : (-1));
			int num2 = Mathf.Clamp(_simpleScrollSnap.CenteredPanel + num, 0, numberOfPanels - 1);
			if (num2 != _simpleScrollSnap.CenteredPanel)
			{
				_simpleScrollSnap.GoToPanel(num2);
				_carouselWheelCooldown = 0.12f;
			}
		}
	}

	private bool IsPointerOverCarousel(Vector2 screenPos)
	{
		if (!(_simpleScrollSnap.ScrollRect.transform is RectTransform rectTransform))
		{
			return false;
		}
		RectTransform rect = (rectTransform.Find("AlterDragCatcher") as RectTransform) ?? rectTransform;
		Canvas componentInParent = rectTransform.GetComponentInParent<Canvas>();
		Camera cam = (((bool)componentInParent && componentInParent.renderMode != RenderMode.ScreenSpaceOverlay) ? componentInParent.worldCamera : null);
		return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, cam);
	}

	private static void EnsureViewportCatchesDrag(ScrollRect scrollRect)
	{
		if (scrollRect == null)
		{
			return;
		}
		RectTransform rectTransform = scrollRect.transform as RectTransform;
		if (!(rectTransform == null) && !(rectTransform.Find("AlterDragCatcher") != null))
		{
			CanvasGroup canvasGroup = rectTransform.GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
			}
			canvasGroup.ignoreParentGroups = true;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.interactable = true;
			GameObject obj = new GameObject("AlterDragCatcher", typeof(RectTransform), typeof(Image));
			RectTransform obj2 = (RectTransform)obj.transform;
			obj2.SetParent(rectTransform, worldPositionStays: false);
			obj2.SetAsFirstSibling();
			obj2.anchorMin = Vector2.zero;
			obj2.anchorMax = Vector2.one;
			obj2.offsetMin = new Vector2(-1500f, -90f);
			obj2.offsetMax = new Vector2(1500f, 60f);
			Image component = obj.GetComponent<Image>();
			component.color = new Color(0f, 0f, 0f, 0f);
			component.raycastTarget = true;
			component.maskable = false;
		}
	}

	private void EngineSoundCheck()
	{
		if ((bool)_engineSound)
		{
			_engineSound.StartDemoRevCycle();
		}
	}

	private void ShowBodyKitsToggleChanged(bool isSelect)
	{
		OnShowBodyKitsChanged?.Invoke(ShowBodyKits);
	}

	private void OnSellCarValuesRefChanged(CoreGameManager.SellCarValuesRefChangedData sellCarValuesRefChangedData)
	{
		if ((bool)sellCarValuesRefChangedData.OldValue)
		{
			sellCarValuesRefChangedData.OldValue.OnChanged -= OnSellCarValuesChanged;
		}
		if ((bool)sellCarValuesRefChangedData.NewValue)
		{
			sellCarValuesRefChangedData.NewValue.OnChanged += OnSellCarValuesChanged;
			OnSellCarValuesChanged(sellCarValuesRefChangedData.NewValue);
			sellCarValuesRefChangedData.NewValue.SetFake(_fakeRedline, _fakeTq, _fakeInertia, _fakeTurboType, _fakeMaxTqRpm, _fakeDisplacementCc, _fakeIsRotary, _fakeTorqueCurve, _fakeIdleRpm);
		}
	}

	private void OnBodyKitArrowClick(ArrowsLeftRight arrows, int addIndex)
	{
		if ((bool)_coreGameManager && (bool)_coreGameManager.SellCarModel)
		{
			SellCarValues component = _coreGameManager.SellCarModel.GetComponent<SellCarValues>();
			if ((bool)component && (bool)component.BuyCarBodyKits)
			{
				component.BuyCarBodyKits.SlideAllBodyKits(addIndex);
			}
		}
	}

	private void FilterChangedCallback()
	{
		PrepareCollections();
	}

	private void OnSellCarValuesChanged(SellCarValues sellCarValues)
	{
		UpdateBodyKitsText();
		UpdatePriceLabel();
		UpdateStyleText();
		_lastNearestPanelId = -1;
		if ((bool)sellCarValues)
		{
			CombinedPrice price = sellCarValues.GetPrice();
			int styleCount = ((_lastPreset != null) ? _lastPreset.allPresetList.Count : 0);
			bool flag = IsOneDefaultStyle(sellCarValues, styleCount);
			ResetStyleAndBodyButton.gameObject.SetActive((sellCarValues.indexStyle != 0 || ((bool)sellCarValues.BuyCarBodyKits && sellCarValues.BuyCarBodyKits.HaveChanges)) && !price.IsRealСurrency && !flag);
			ShowBodyKitsToggle.gameObject.SetActive(!price.IsRealСurrency);
			sellCarValues.CarID.Get(out int value);
			_newCarsMarkController.TrySetOldCar(value);
			HandleEngineSoundAsync(sellCarValues).Forget();
		}
		else
		{
			_newCarsMarkController.StopTryOldCar();
			ResetStyleAndBodyButton.gameObject.SetActive(value: false);
		}
	}

	private async UniTask HandleEngineSoundAsync(SellCarValues sellCarValues)
	{
		CommonExtensions.CancelAndDisposeToken(ref _engineSoundCts);
		_engineSoundCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
		CancellationToken token = _engineSoundCts.Token;
		while ((bool)this && (bool)sellCarValues)
		{
			EngineSound engineSound = sellCarValues.GetEngineSound();
			_engineSound = (engineSound ? engineSound : null);
			RefreshEngineSoundButton(sellCarValues);
			if (await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow())
			{
				return;
			}
		}
		if ((bool)this && !token.IsCancellationRequested)
		{
			_engineSound = null;
			RefreshEngineSoundButton(null);
		}
	}

	private void RefreshEngineSoundButton(SellCarValues sellCarValues)
	{
		bool flag = (bool)_engineSound && _engineSound.IsBankLoaded && IsCenteredCar(sellCarValues);
		if ((bool)EngineSoundButton && EngineSoundButton.interactable != flag)
		{
			EnableDisableEngineSoundButton(flag);
		}
	}

	private bool IsCenteredCar(SellCarValues sellCarValues)
	{
		if (!sellCarValues)
		{
			return false;
		}
		if (_simpleScrollSnap == null || !_newCarsDict.TryGetValue(_simpleScrollSnap.CenteredPanel, out var value) || value?.Data == null)
		{
			return true;
		}
		sellCarValues.CarID.Get(out int value2);
		return value2 == (int)value.Data.Serial;
	}

	private void GoToNextCar()
	{
		ObscuredInt obscuredInt = (int)_lastCarId + 1;
		if (_newCarsDict.ContainsKey(obscuredInt))
		{
			_simpleScrollSnap.GoToPanel(obscuredInt);
		}
	}

	protected override void OnDestroy()
	{
		_toDestroy = true;
		_newCarsDict.Clear();
		_offerTimerController?.Dispose();
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCreateCells);
		CommonExtensions.CancelAndDisposeToken(ref _textureCts);
		CommonExtensions.CancelAndDisposeToken(ref _engineSoundCts);
		if ((bool)_engineSound)
		{
			_engineSound.OnBankLoaded -= EnableDisableEngineSoundButton;
		}
	}

	private void ShowEditPreset()
	{
		bool activeSelf = removePreset.transform.parent.gameObject.activeSelf;
		removePreset.transform.parent.gameObject.SetActive(!activeSelf);
	}

	private void PrepareCollections()
	{
		UpdateBuyButtonsInteractable(interactable: false);
		List<VehicleStockTuningData> cars = DealerWindowFiltersController.GetCars();
		Init(cars);
	}

	public override void SetAdditionalData(string data)
	{
		base.SetAdditionalData(data);
		if (!string.IsNullOrEmpty(data))
		{
			_dealerWindowAdditionalData = JsonConvert.DeserializeObject<DealerWindowAdditionalData>(data);
		}
		if (_dealerWindowAdditionalData != null && (_creatingCellsNow || _simpleScrollSnap != null))
		{
			ApplyAdditionalDataFilter();
		}
	}

	private void ApplyAdditionalDataFilter()
	{
		_forcedTargetCarId = _dealerWindowAdditionalData.TargetCarId;
		if (_dealerWindowAdditionalData.UseFilter && _dealerWindowAdditionalData.ShowOnlySpecial)
		{
			DealerWindowFiltersController.ShowOnlySpecial();
		}
		else
		{
			DealerWindowFiltersController.ShowAll();
		}
		Init(DealerWindowFiltersController.GetCars(), isFirstInit: true);
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
		if (!_slotService.HaveCarInAnyGarage && _offlineModeSystem.Disabled)
		{
			_managerUI.OnSettingsButtonActivated?.Invoke(obj: false);
			_managerUI.OnAchievmentsButtonActivated?.Invoke(obj: false);
			_managerUI.OnBackButtonActivated?.Invoke(obj: false);
		}
	}

	public void Back()
	{
		if (ConfirmPopupBuyNewCar.gameObject.activeSelf)
		{
			MainFader.Fade(1f, 0.2f);
			ConfirmPopupBuyNewCar.gameObject.SetActive(value: false);
			SetCarouselVisible(visible: true);
		}
		else
		{
			EClose();
		}
	}

	private void DestroyChilds()
	{
		if ((bool)_simpleScrollSnap)
		{
			UnityEngine.Object.Destroy(_simpleScrollSnap.gameObject);
		}
		if ((bool)_currentDealerUIElement)
		{
			_currentDealerUIElement = null;
		}
		_lastCarId = -1;
	}

	private void Init(List<VehicleStockTuningData> cars, bool isFirstInit = false)
	{
		_coreGameManager.carImage = carPlaceholderImage.gameObject;
		if (_creatingCellsNow)
		{
			_args = (cars, isFirstInit);
		}
		else
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCreateCells);
			_cancellationTokenSourceCreateCells = new CancellationTokenSource();
			CreateCells(cars, _cancellationTokenSourceCreateCells.Token, isFirstInit).Forget();
		}
		ScrollSnapSound.Init(cars.Count);
	}

	private async UniTaskVoid CreateCells(List<VehicleStockTuningData> cars, CancellationToken cancellationToken, bool isFirstInit = false)
	{
		_creatingCellsNow = true;
		UnbindAll();
		DestroyChilds();
		Scrollbar.value = 0f;
		_simpleScrollSnap = _prefabFactory.Instantiate(SimpleScrollSnapPrefab, ScrollRectPosition);
		_simpleScrollSnap.ScrollRect.onValueChanged.AddListener(ScrollSnapSound.OnScrollValueChanged);
		_simpleScrollSnap.ScrollRect.horizontalScrollbar = Scrollbar;
		ScrollSnapBar.ScrollSnap = _simpleScrollSnap;
		_simpleScrollSnap.transform.SetAsFirstSibling();
		_simpleScrollSnap.gameObject.SetActive(value: false);
		EnsureViewportCatchesDrag(_simpleScrollSnap.ScrollRect);
		_simpleScrollSnap.SnapSpeed = 22f;
		if ((bool)_simpleScrollSnap.ScrollRect)
		{
			_simpleScrollSnap.ScrollRect.decelerationRate = 0.004f;
			_simpleScrollSnap.ScrollRect.gameObject.AddComponent<DealerScrollFlingLimiter>().Init(_simpleScrollSnap.ScrollRect, 500f, 130f);
		}
		EnsurePool();
		CommonExtensions.CancelAndDisposeToken(ref _textureCts);
		_textureCts = new CancellationTokenSource();
		int num = 0;
		_newCarUIElements.Clear();
		_newCarsDict.Clear();
		_carTextures.Clear();
		_panelPlaceholders.Clear();
		_lastRequestedLoadCarId = -1;
		_clickSnapInProgress = false;
		int correctIndex = 0;
		Transform content = _simpleScrollSnap.ScrollRect.content;
		foreach (VehicleStockTuningData stockTuningData in cars)
		{
			if (num == _createPerFrame)
			{
				await UniTask.Yield();
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				num = 0;
			}
			else
			{
				num++;
			}
			VehicleStockTuningData vehicleStockTuningData = stockTuningData;
			if (vehicleStockTuningData.Nfs || (bool)vehicleStockTuningData.IsRewardCar || _allCarsInfo.IsBannedCar(vehicleStockTuningData.Serial))
			{
				continue;
			}
			if (!_ifFirstInit)
			{
				foreach (CarType carType in vehicleStockTuningData.CarTypes)
				{
					if (_carTypeDic.TryGetValue(carType, out var value))
					{
						value.Add(vehicleStockTuningData.Serial);
						continue;
					}
					List<int> value2 = new List<int> { vehicleStockTuningData.Serial };
					_carTypeDic.Add(carType, value2);
				}
				if (_carRegionDic.TryGetValue(vehicleStockTuningData.region, out var value3))
				{
					value3.Add(vehicleStockTuningData.Serial);
				}
				else
				{
					List<int> value4 = new List<int> { vehicleStockTuningData.Serial };
					_carRegionDic.Add(vehicleStockTuningData.region, value4);
				}
			}
			if ((bool)_simpleScrollSnap)
			{
				RectTransform rectTransform = (RectTransform)new GameObject($"Panel{correctIndex}", typeof(RectTransform)).transform;
				rectTransform.SetParent(content, worldPositionStays: false);
				_panelPlaceholders.Add(rectTransform);
				NewVehicleData value5 = new NewVehicleData
				{
					Id = new AdvancedInt(correctIndex),
					Data = vehicleStockTuningData,
					MoneyColor = (vehicleStockTuningData.GetPriceTypeUI ? goldColor : moneyColor),
					MoneySprite = (vehicleStockTuningData.GetPriceTypeUI ? goldIcon : moneyIcon),
					CarTexture = null
				};
				_newCarsDict.Add(correctIndex, value5);
				correctIndex++;
			}
		}
		_maxCarsCount = correctIndex;
		_ifFirstInit = true;
		if (_newCarsDict.Count > 0)
		{
			_simpleScrollSnap.gameObject.SetActive(value: true);
			int carId = ((_forcedTargetCarId > 0) ? _forcedTargetCarId : _garage.LastSelectedCar);
			int index = (isFirstInit ? GetLastSelectedCarIndex(carId) : 0);
			_simpleScrollSnap.ScrollRect.inertia = true;
			await UniTask.Yield();
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			_lastNearestPanelId = index;
			_simpleScrollSnap.StartingPanel = index;
			_simpleScrollSnap.Setup();
			_simpleScrollSnap.GoToPanel(index);
			BindWindow(index);
			if (isFirstInit)
			{
				OnFirstInitIgnoreCarImage().Forget();
			}
			_simpleScrollSnap.OnPanelSelected.AddListener(OnCarSelected);
			OnCarSelected();
		}
		_creatingCellsNow = false;
		if (_args.HasValue)
		{
			Init(_args.Value.Item1, _args.Value.Item2);
			_args = null;
		}
	}

	private void EnsurePool()
	{
		if (_poolHolder == null)
		{
			GameObject gameObject = new GameObject("DealerCellPool", typeof(RectTransform));
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			gameObject.SetActive(value: false);
			_poolHolder = gameObject.transform;
		}
		int num = 17;
		while (_cellPool.Count < num)
		{
			DealerUIElement dealerUIElement = _prefabFactory.Instantiate(dealerUIElementPrefab, _poolHolder);
			dealerUIElement.gameObject.SetActive(value: false);
			_cellPool.Add(dealerUIElement);
			_freeCells.Push(dealerUIElement);
		}
	}

	private void BindWindow(int center)
	{
		int count = _newCarsDict.Count;
		if (count == 0)
		{
			return;
		}
		int num = Mathf.Max(0, center - 8);
		int num2 = Mathf.Min(count - 1, center + 8);
		_bindTmp.Clear();
		_bindTmp.AddRange(_newCarUIElements.Keys);
		foreach (int item in _bindTmp)
		{
			if (item < num || item > num2)
			{
				Unbind(item);
			}
		}
		for (int i = num; i <= num2; i++)
		{
			if (!_newCarUIElements.ContainsKey(i))
			{
				Bind(i);
			}
		}
	}

	private void Bind(int index)
	{
		if (index >= 0 && index < _panelPlaceholders.Count && _newCarsDict.TryGetValue(index, out var value) && _freeCells.Count != 0)
		{
			DealerUIElement dealerUIElement = _freeCells.Pop();
			RectTransform obj = (RectTransform)dealerUIElement.transform;
			obj.SetParent(_panelPlaceholders[index], worldPositionStays: false);
			obj.anchorMin = Vector2.zero;
			obj.anchorMax = Vector2.one;
			obj.offsetMin = Vector2.zero;
			obj.offsetMax = Vector2.zero;
			obj.localScale = Vector3.one;
			dealerUIElement.gameObject.SetActive(value: true);
			dealerUIElement.Init(this, value);
			_carShopVisualService.ApplyCardVisual(dealerUIElement);
			_newCarUIElements[index] = dealerUIElement;
			if (_carTextures.TryGetValue(index, out var value2) && (bool)value2)
			{
				dealerUIElement.SetCarTexture(value2);
			}
			else
			{
				LoadTextureForIndex(index, dealerUIElement, _textureCts.Token).Forget();
			}
		}
	}

	private void Unbind(int index)
	{
		if (_newCarUIElements.TryGetValue(index, out var value))
		{
			_newCarUIElements.Remove(index);
			if (_currentDealerUIElement == value)
			{
				_currentDealerUIElement = null;
			}
			if ((bool)value)
			{
				value.gameObject.SetActive(value: false);
				value.transform.SetParent(_poolHolder, worldPositionStays: false);
				_freeCells.Push(value);
			}
		}
	}

	private void UnbindAll()
	{
		if (_newCarUIElements.Count == 0)
		{
			return;
		}
		_bindTmp.Clear();
		_bindTmp.AddRange(_newCarUIElements.Keys);
		foreach (int item in _bindTmp)
		{
			Unbind(item);
		}
	}

	private async UniTask LoadTextureForIndex(int index, DealerUIElement cell, CancellationToken ct)
	{
		if (!_newCarsDict.TryGetValue(index, out var data))
		{
			return;
		}
		ResourceRequest request = Resources.LoadAsync<Texture>(PathGetterHelper.GetCarPreviewPath(data.Data.Serial));
		if (!(await UniTask.WaitUntil(() => request.isDone, PlayerLoopTiming.Update, ct).SuppressCancellationThrow()) && !ct.IsCancellationRequested && !_toDestroy && (bool)cell && _newCarUIElements.TryGetValue(index, out var value) && !(value != cell))
		{
			Texture texture = request.asset as Texture;
			_carTextures[index] = texture;
			data.CarTexture = texture;
			cell.SetCarTexture(texture);
			if ((bool)_simpleScrollSnap && index == _simpleScrollSnap.CenteredPanel)
			{
				ShowPlaceholder(texture);
			}
		}
	}

	private int GetLastSelectedCarIndex(int carId)
	{
		int result = 0;
		foreach (KeyValuePair<ObscuredInt, NewVehicleData> item in _newCarsDict)
		{
			if ((int)item.Value.Data.Serial == carId)
			{
				result = item.Key;
			}
		}
		return result;
	}

	public void OnCarPreSetLoaded()
	{
		if (!_coreGameManager || !_coreGameManager.SellCarModel)
		{
			return;
		}
		SellCarValues component = _coreGameManager.SellCarModel.GetComponent<SellCarValues>();
		if ((bool)component)
		{
			_lastPreset = component.allVinylsPreset;
			if ((bool)_lastPreset)
			{
				CombinedPrice price = component.GetPrice();
				int count = _lastPreset.allPresetList.Count;
				bool flag = IsOneDefaultStyle(component, count);
				StyleRoot.SetActive(count != 0 && !price.IsRealСurrency && !flag);
				UpdateBodyKitsText();
				styleText.text = ((count == 0) ? "" : ((string)new LocalizedString("DealerWindow.Stock")));
			}
			if (!_currentVehicleStockTuningData)
			{
				component.CarID.Get(out int value);
				_currentVehicleStockTuningData = _carTuningFactory.CreateDefault(value);
			}
			UpdatePriceLabel();
		}
	}

	private bool IsOneDefaultStyle(SellCarValues sellCarValues, int styleCount)
	{
		bool result = false;
		if ((bool)sellCarValues)
		{
			result = sellCarValues.StyleDefault() == 0 && styleCount == 1;
		}
		return result;
	}

	private async UniTaskVoid OnFirstInitIgnoreCarImage()
	{
		carPlaceholderImage.enabled = false;
		OnCarSelected();
		await UniTask.Delay(1000);
		if (!_toDestroy)
		{
			carPlaceholderImage.enabled = true;
		}
	}

	public void OnCarSelected(int id = -1)
	{
		CounterText.text = $"{_simpleScrollSnap.CenteredPanel + 1}/{_maxCarsCount}";
		_newCarsDict[_simpleScrollSnap.CenteredPanel].Id.Get(out int value);
		ObscuredInt obscuredInt = value;
		_garage.LastSelectedCar = _newCarsDict[_simpleScrollSnap.CenteredPanel].Data.Serial;
		UpdatePreferredGamepadCar();
		int num = _newCarsDict[_simpleScrollSnap.CenteredPanel].Data.Serial;
		if (_clickSnapInProgress)
		{
			if (num == _clickSnapTargetCarId)
			{
				_clickSnapInProgress = false;
			}
		}
		else if (_lastRequestedLoadCarId != num)
		{
			_lastRequestedLoadCarId = num;
			_coreGameManager.LoadSellCarFromOutside(num);
		}
		if ((int)_lastCarId == (int)obscuredInt)
		{
			return;
		}
		if ((bool)_currentDealerUIElement)
		{
			_currentDealerUIElement.OnFocus(state: false);
		}
		if (_newCarUIElements.TryGetValue(_simpleScrollSnap.CenteredPanel, out var value2))
		{
			_currentDealerUIElement = value2;
			_currentDealerUIElement.OnFocus(state: true);
		}
		AvalibleEnginesText.text = "-";
		AvalibleGearboxesText.text = "-";
		weightText.text = "-";
		styleText.text = "";
		NewVehicleData newVehicleData = _newCarsDict[obscuredInt];
		CarTuningData tuningData = newVehicleData.Data.Tuning.GetTuningData();
		CarEntity carEntity = _carTuningFactory.AssemblyNewCar(tuningData, newVehicleData.Data, obscuredInt);
		_lastCarId = obscuredInt;
		if (_carTextures.TryGetValue(value, out var value3))
		{
			ShowPlaceholder(value3);
		}
		_currentVehicleStockTuningData = newVehicleData.Data;
		_lastPreset = null;
		UpdatePriceLabel();
		StyleRoot.SetActive(value: false);
		BodyKitsArrows.gameObject.SetActive(value: false);
		_carEntityGm = Resources.LoadAsync<GameObject>($"car{newVehicleData.Data.Serial}");
		_carEntityGm.completed += (AsyncOperation operation) =>
		{
			try
			{
				GameObject obj = (GameObject)_carEntityGm.asset;
				obj.TryGetComponent<Powertrain>(out var component);
				if ((bool)component)
				{
					component.ManualInit();
				}
				if (obj.TryGetComponent<VehicleEntityProvider>(out var component2))
				{
					component2.ApplyEngineValue(carEntity.GetValue());
					component2.ApplySuspensionValue(carEntity.GetValue());
				}
				if ((bool)component)
				{
					component.CalcMaxPowerMaxTorque(out var maxHp, out var maxHpRpm, out _fakeTq, out _fakeMaxTqRpm, out var maxWhp);
					_fakeRedline = component.RedlineRpm;
					weightText.text = Utils.GetMass(carEntity.ChassisData.Weight + carEntity.GetWeight(), round: true) ?? "";
					hpText.text = Utils.GetPower(maxHp);
					torqueText.text = Utils.GetTorque(_fakeTq) + " " + Utils.GetTorquePostfix;
					transmissionText.text = new LocalizedString(GearBoxTypeFilterUtility.LocalizationKey(component.GearboxKind));
					component.GetEngineTurboParams(out maxWhp, out maxHpRpm, out var blowOff, out _fakeTurboType);
					_fakeInertia = component.EngineInertiaRatio;
					_fakeDisplacementCc = component.EngineDisplacementCc;
					_fakeIsRotary = component.IsRotaryEngine;
					_fakeIdleRpm = component.EngineIdleRpm;
					_fakeTorqueCurve = new float[32];
					for (int i = 0; i < 32; i++)
					{
						float rpm = Mathf.Lerp(_fakeIdleRpm, _fakeRedline, (float)i / 31f);
						component.GetDynoGraphAtRpm(rpm, out var torq, out blowOff, out maxHpRpm, out maxWhp);
						_fakeTorqueCurve[i] = Mathf.Max(torq, 1f);
					}
				}
				if ((bool)_currentVehicleStockTuningData)
				{
					ClassText.text = _currentVehicleStockTuningData.GetClassName;
				}
			}
			catch (Exception arg)
			{
				EClose(clear: true);
				Debug.LogError($"Car {newVehicleData.Data.Serial} params not loaded {arg}");
			}
		};
		_lastNearestPanelId = -1;
		if (!base.gameObject.activeSelf)
		{
			EnableAfterCarLoaded().Forget();
		}
	}

	public void EClose(bool clear = false)
	{
		if (clear)
		{
			_garage.LastSelectedCar = -1;
		}
		_coreGameManager.UnloadCurrentSellCar();
		_managerUI.ShowGarageWindow();
		_garage.FocusOnCarOnSlot(V.SlotID).Forget();
		Close();
	}

	private async UniTask EnableAfterCarLoaded()
	{
		while (!V.CoreGameManager.SellCarModel)
		{
			await UniTask.Yield();
			if (_toDestroy)
			{
				return;
			}
		}
		if (_toDestroy || _desktopRevealInFlight)
		{
			return;
		}
		_desktopRevealInFlight = true;
		await WaitForDesktopRevealReady();
		_desktopRevealInFlight = false;
		if (!_toDestroy)
		{
			RefreshVisibleCardPrices();
			_cameraManager.garageCam.ResetCameraForSellCar();
			await UniTask.Delay(100);
			if (!_toDestroy)
			{
				base.gameObject.SetActive(value: true);
				UpdateBodyKitsText();
			}
		}
	}

	private async UniTask WaitForDesktopRevealReady()
	{
		CancellationToken ct = this.GetCancellationTokenOnDestroy();
		float deadline = Time.unscaledTime + 4f;
		await UniTask.WaitUntil(() => _toDestroy || Time.unscaledTime >= deadline || IsSellCarMoveFinished(), PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
		if (_toDestroy)
		{
			return;
		}
		await UniTask.DelayFrame(2, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
		if (!_toDestroy)
		{
			await UniTask.WaitUntil(() => _toDestroy || Time.unscaledTime >= deadline || IsDesktopRevealReady(), PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
			if (!_toDestroy)
			{
				_ = Time.unscaledTime;
				_ = deadline;
			}
		}
	}

	private bool IsSellCarMoveFinished()
	{
		if ((bool)_cameraManager && _cameraManager.CustomMoveCameraToTarget != null)
		{
			return !_cameraManager.CustomMoveCameraToTarget.Moving;
		}
		return true;
	}

	private bool IsDesktopRevealReady()
	{
		if (!IsSellCarMoveFinished())
		{
			return false;
		}
		if ((bool)_cameraManager)
		{
			CinemachineBrain currentBrain = _cameraManager.CurrentBrain;
			if ((bool)currentBrain && currentBrain.IsBlending)
			{
				return false;
			}
		}
		if (_offerSystem != null && !_offerSystem.IsDataReady)
		{
			return false;
		}
		if (_purchaseSystem != null && !_purchaseSystem.IsInitialized)
		{
			return false;
		}
		return AreVisibleCardsReady();
	}

	private bool AreVisibleCardsReady()
	{
		if (_simpleScrollSnap == null || _panelPlaceholders.Count == 0)
		{
			return true;
		}
		int centeredPanel = _simpleScrollSnap.CenteredPanel;
		int num = Mathf.Max(0, centeredPanel - 3);
		int num2 = Mathf.Min(_panelPlaceholders.Count - 1, centeredPanel + 3);
		for (int i = num; i <= num2; i++)
		{
			if (!_newCarUIElements.ContainsKey(i))
			{
				return false;
			}
			if (!_carTextures.ContainsKey(i))
			{
				return false;
			}
		}
		return true;
	}

	private void RefreshVisibleCardPrices()
	{
		foreach (KeyValuePair<int, DealerUIElement> newCarUIElement in _newCarUIElements)
		{
			if ((bool)newCarUIElement.Value)
			{
				newCarUIElement.Value.UpdatePrice(newCarUIElement.Value.CarId);
			}
		}
	}

	private void BuyCarPressed(ButtonBuyNewCar buttonBuyNewCar)
	{
		if (_offlineModeSystem.Enabled)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f, NotificationType.Error);
			return;
		}
		_coreGameManager.SellCarValues.CarID.Get(out int value);
		if (_carShopPurchaseService.TryHandle(value, HideForAnotherWindow))
		{
			return;
		}
		if (CarIdStorage.IsCarLockedOnThisPlatform(value))
		{
			V.CoreGameManager.SetMessage(new LocalizedString("CarShop.AvailableOnlyOnSteam"), 3f, NotificationType.Error);
			return;
		}
		CombinedPrice price = _coreGameManager.SellCarValues.GetPrice();
		bool mustBuySlot = false;
		if (!_haveEmptySlot)
		{
			price.AddCoin(VehicleSlotPaymentService.Price);
			mustBuySlot = true;
		}
		ObscuredInt obscuredInt = Mathf.Abs(price.Coin);
		ObscuredInt obscuredInt2 = Mathf.Abs(price.Money);
		AdvancedBoolean advancedBoolean = new AdvancedBoolean((int)obscuredInt <= 0) || _currencyManagerController.IsEnoughCoins(obscuredInt, withMessage: true);
		if (_currencyManagerController.IsEnoughMoney(obscuredInt2, withMessage: true) && advancedBoolean)
		{
			if (price.IsRealСurrency)
			{
				TryBuyCar();
			}
			else
			{
				MainFader.Fade(0f, 0.2f);
				ConfirmPopupBuyNewCar.gameObject.SetActive(value: true);
				SetCarouselVisible(visible: false);
				_managerUI.OnBackButtonActivated?.Invoke(obj: false);
				ConfirmPopupBuyNewCar.Init(price, _coreGameManager.SellCarValues.CarID, mustBuySlot);
			}
			_virtualPurchaseAnalytic.LogPurchaseVirtual_NewCars(mustBuySlot, price, PurchaseStatus.PURCHASE_CLICKED);
		}
		else
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_NewCars(mustBuySlot, price, PurchaseStatus.INSUFFICIENT_FUNDS);
		}
	}

	private void HideForAnotherWindow()
	{
		if (TryGetComponent<TweenerCanvas>(out var component))
		{
			component.Hide();
		}
	}

	private void BuyNewCarConfirmPressed(ConfirmPopup confirmPopup)
	{
		MainFader.Fade(1f, 0.2f);
		ConfirmPopupBuyNewCar.gameObject.SetActive(value: false);
		SetCarouselVisible(visible: true);
		TryBuyCar();
	}

	private void BuyNewCarCancelPressed(ConfirmPopup confirmPopup)
	{
		MainFader.Fade(1f, 0.2f);
		ConfirmPopupBuyNewCar.gameObject.SetActive(value: false);
		SetCarouselVisible(visible: true);
		UpdatePriceLabel();
	}

	private void TryBuyCar()
	{
		if (!_coreGameManager.SellCarModel || !_coreGameManager.SellCarModel.activeInHierarchy)
		{
			return;
		}
		SellCarValues component = _coreGameManager.SellCarModel.GetComponent<SellCarValues>();
		if (!component || component.CarID != _currentVehicleStockTuningData.Serial)
		{
			EClose(clear: true);
			return;
		}
		_allCarsInfo.ResolveCarMeta(_currentVehicleStockTuningData.Serial, out var result);
		if (math.all(result == Utils.NotValid))
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("DealerWindow.YouCantBuyThisCar"), 3f, NotificationType.Error).Forget();
			EClose(clear: true);
			return;
		}
		if (_slotService.OccupiedSlotCount == _slotService.MaxSlotCount)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("DealerWindow.YourGarageHasNoEmptySpot"), 3f);
			return;
		}
		CombinedPrice price = _coreGameManager.SellCarValues.GetPrice();
		if ((bool)_currentVehicleStockTuningData)
		{
			if (_haveEmptySlot || price.IsRealСurrency)
			{
				BuyCarAsync().Forget();
			}
			else
			{
				TryBuy().Forget();
			}
		}
	}

	private void TrackAcquiredVehicleClass(VehicleClass vehicleClass)
	{
		GooglePlayAchievements.ReportMaskProgress("CgkIuKqW0PcZEAIQNQ", "gpg_garage_classes_mask", (int)vehicleClass, 5, "dealer_purchase");
	}

	private async UniTask TryBuy()
	{
		await _managerUI.ShowWaitingWindow(set: true);
		if (await _slotService.TryBuySlot())
		{
			_virtualPurchaseAnalytic.LogPurchaseVirtual_NewCars(mustBuySlot: true, new CombinedPrice(VehicleSlotPaymentService.Price, 0), PurchaseStatus.PURCHASE_COMPLETED);
			await BuyCarAsync();
		}
	}

	private async UniTask BuyCarAsync()
	{
		if (!_coreGameManager.SellCarModel)
		{
			return;
		}
		int id = _currentVehicleStockTuningData.Serial;
		SellCarValues sellCarValues = _coreGameManager.SellCarModel.GetComponent<SellCarValues>();
		if (sellCarValues.CarID != id)
		{
			EClose(clear: true);
		}
		else
		{
			if (!(await _guestAccountCreator.CreateGuestAccountIfNeeds(showWaitingScreen: true, showErrorMessage: true)))
			{
				return;
			}
			CombinedPrice price = sellCarValues.GetPrice();
			if (!CarPriceProvider.IsRealCurrencyCar(id) || price.IsRealСurrency)
			{
				if (price.IsRealСurrency)
				{
					_offerSystem.SetCurrentOfferID(price.RealСurrencyPrice.OfferID);
					_purchaseSystem.BuyProductID(price.RealСurrencyPrice.ProductId);
				}
				else
				{
					TryBuy(sellCarValues, id).Forget();
				}
			}
		}
	}

	private async UniTaskVoid TryBuy(SellCarValues sellCarValues, int validatedCarId)
	{
		if (!(await _coreGameManager.BuyCarById(sellCarValues, validatedCarId)))
		{
			return;
		}
		GooglePlayAchievements.ReportProgress("CgkIuKqW0PcZEAIQHw", "gpg_ach_cars_acquired", 2, 1, "dealer_purchase");
		if ((bool)_currentVehicleStockTuningData)
		{
			TrackAcquiredVehicleClass(_currentVehicleStockTuningData.GetClass);
		}
		if (ManagerKeysForPrefs.IsSteamLinked)
		{
			int current = _steamAchievementService.GetStatInt("steam_cars_acquired") + 1;
			_steamAchievementService.ReportProgress("ACH_SECOND_SET_OF_KEYS", "steam_cars_acquired", 2, current);
			if (_currentVehicleStockTuningData != null)
			{
				TrackAcquiredVehicleClassForSteam(_currentVehicleStockTuningData.GetClass);
			}
		}
		_virtualPurchaseAnalytic.LogPurchaseVirtual_NewCars(mustBuySlot: false, sellCarValues.GetPrice(), PurchaseStatus.PURCHASE_COMPLETED);
		_analyticEventFactory.CreateEvent<GarageInfoEvent, DefaultContext>(new DefaultContext()).Send();
		_coreGameManager.UnloadCurrentSellCar();
		_coreGameManager.ResetCarId();
		Close();
	}

	private void TrackAcquiredVehicleClassForSteam(VehicleClass vehicleClass)
	{
		if (vehicleClass < VehicleClass.A || vehicleClass >= (VehicleClass)31)
		{
			return;
		}
		int num = PlayerPrefs.GetInt("steam_garage_classes_mask", 0);
		int num2 = 1 << (int)vehicleClass;
		if ((num & num2) == 0)
		{
			num |= num2;
			PlayerPrefs.SetInt("steam_garage_classes_mask", num);
			if (CountSetBits(num) >= 5)
			{
				_steamAchievementService?.Unlock("ACH_GARAGE_VARIETY");
			}
		}
	}

	private static int CountSetBits(int value)
	{
		int num = 0;
		while (value != 0)
		{
			num += value & 1;
			value >>= 1;
		}
		return num;
	}

	public void RecordStyleForNewPurchasedCar(CarInfo carInfo)
	{
		if (carInfo == null || !_coreGameManager || !_coreGameManager.SellCarModel)
		{
			return;
		}
		SellCarValues component = _coreGameManager.SellCarModel.GetComponent<SellCarValues>();
		if ((bool)component)
		{
			int num = component.indexStyle - 1;
			if (num != -1 && component.allVinylsPreset.allPresetList.Count > num)
			{
				OneVinylsPreset oneVinylsPreset = component.allVinylsPreset.allPresetList[num];
				NewCarVisualData newCarVisualData = new NewCarVisualData();
				newCarVisualData.CarPaintColor = component.PurchasedColor;
				newCarVisualData.VynilOneCar = oneVinylsPreset.vynilOneCarList;
				newCarVisualData.WindowVynilOneCar = oneVinylsPreset.windowVynilOneCarList;
				CarExtension.RecordStyleForNewPurchasedCar(carInfo, newCarVisualData);
			}
		}
	}

	private void UpdatePriceLabel()
	{
		if (!_coreGameManager.SellCarModel)
		{
			return;
		}
		SellCarValues sellCarValues = _coreGameManager.SellCarValues;
		CombinedPrice price = sellCarValues.GetPrice();
		if (!_haveEmptySlot)
		{
			price.AddCoin(VehicleSlotPaymentService.Price);
		}
		BuyDefaultButton.SetPrice(price, !_haveEmptySlot).Forget();
		_coreGameManager.SellCarValues.CarID.Get(out int carId);
		_carShopVisualService.ApplyBuyButtonVisual(BuyDefaultButton, carId);
		_offerTimerController.SetOffer(sellCarValues.CarPriceHelperV2.GetRealСurrencyPrice(carId).Item2, () =>
		{
			foreach (KeyValuePair<int, DealerUIElement> newCarUIElement in _newCarUIElements)
			{
				if ((bool)newCarUIElement.Value)
				{
					newCarUIElement.Value.UpdatePrice(carId);
				}
			}
			UpdatePriceLabel();
		});
		ButtonBuyNewCar buyDefaultButton = BuyDefaultButton;
		buyDefaultButton.OnClick = (Action<ButtonBuyNewCar>)Delegate.Remove(buyDefaultButton.OnClick, new Action<ButtonBuyNewCar>(BuyCarPressed));
		ButtonBuyNewCar buyDefaultButton2 = BuyDefaultButton;
		buyDefaultButton2.OnClick = (Action<ButtonBuyNewCar>)Delegate.Combine(buyDefaultButton2.OnClick, new Action<ButtonBuyNewCar>(BuyCarPressed));
	}

	private void UpdateBodyKitsText()
	{
		if ((bool)_coreGameManager && (bool)_coreGameManager.SellCarModel && _coreGameManager.SellCarModel.activeInHierarchy)
		{
			SellCarValues sellCarValues = _coreGameManager.SellCarValues;
			if ((bool)sellCarValues && (bool)sellCarValues.BuyCarBodyKits)
			{
				int currentSetIndex = sellCarValues.BuyCarBodyKits.CurrentSetIndex;
				BodyKitsArrows.TitleText = ((currentSetIndex == 0) ? ((string)new LocalizedString("DealerWindow.StockBodyKits")) : string.Format("{0} {1}/{2}", new LocalizedString("Newcarswindow.Bodykits"), currentSetIndex, sellCarValues.BuyCarBodyKits.SetAmount - 1));
				BodyKitsArrows.gameObject.SetActive(sellCarValues.BuyCarBodyKits.SetAmount > 1);
			}
			else
			{
				BodyKitsArrows.gameObject.SetActive(value: false);
			}
		}
		else
		{
			BodyKitsArrows.gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		HandleCarouselWheel();
		if (_newCarsDict == null)
		{
			return;
		}
		if ((bool)_simpleScrollSnap && (bool)_coreGameManager && _newCarsDict.TryGetValue(_simpleScrollSnap.NearestPanel, out _currentNearestCarData))
		{
			_currentNearestCarData.Id.Get(out int value);
			if (value == _lastNearestPanelId)
			{
				return;
			}
			_lastNearestPanelId = value;
			BindWindow(value);
			if ((int)_lastCarId != _lastNearestPanelId)
			{
				UpdateVisiblePreviewImage(isVisible: true);
			}
			else if ((bool)_coreGameManager.SellCarModel)
			{
				if (_cachedSellModel != _coreGameManager.SellCarModel)
				{
					_cachedSellModel = _coreGameManager.SellCarModel;
					_cachedSellValues = _cachedSellModel.GetComponent<SellCarValues>();
				}
				if ((bool)_cachedSellValues && _cachedSellValues.CarID == _currentNearestCarData.Data.Serial)
				{
					UpdateVisiblePreviewImage(isVisible: false);
				}
				else
				{
					UpdateVisiblePreviewImage(isVisible: true);
				}
			}
			else
			{
				UpdateVisiblePreviewImage(isVisible: true);
			}
		}
		else if (_newCarsDict.Count == 0)
		{
			_lastNearestPanelId = -1;
			carPlaceholderImage.gameObject.SetActive(value: false);
			if ((bool)_coreGameManager && (bool)_coreGameManager.SellCarModel)
			{
				_coreGameManager.SellCarModel.SetActive(value: false);
			}
		}
	}

	public void OnNextStylePressed()
	{
		_coreGameManager.SellCarValues.OnStyleChanged(1);
		UpdatePriceLabel();
	}

	public void OnPrevStylePressed()
	{
		_coreGameManager.SellCarValues.OnStyleChanged(-1);
		UpdatePriceLabel();
	}

	private void UpdateVisiblePreviewImage(bool isVisible)
	{
		if (isVisible)
		{
			if (_carTextures.TryGetValue(_lastNearestPanelId, out var value))
			{
				ShowPlaceholder(value);
			}
		}
		else
		{
			GameObject gameObject = carPlaceholderImage.gameObject;
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(value: false);
			}
			_suppressPlaceholderForClick = false;
		}
		if ((bool)_coreGameManager.SellCarModel)
		{
			_coreGameManager.SellCarModel.SetActive(!isVisible && _coreGameManager.IsSellCarModelReady);
		}
		BuyDefaultButton.Interactable = !isVisible;
	}

	private void UpdateStyleText()
	{
		if ((bool)_coreGameManager.SellCarValues && (bool)_coreGameManager.SellCarValues.allVinylsPreset)
		{
			int indexStyle = _coreGameManager.SellCarValues.indexStyle;
			int count = _coreGameManager.SellCarValues.allVinylsPreset.allPresetList.Count;
			styleText.text = ((indexStyle == 0) ? ((string)new LocalizedString("DealerWindow.Stock")) : string.Format("{0} {1}/{2}", new LocalizedString("Newcarswindow.style"), indexStyle, count));
		}
	}

	private void ShowPlaceholder(Texture texture)
	{
		if (!RamDevice.AllowHeavyPreload() && !_suppressPlaceholderForClick)
		{
			carPlaceholderImage.texture = texture;
			GameObject gameObject = carPlaceholderImage.gameObject;
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(value: true);
			}
		}
	}

	public void RemoveThisPreset()
	{
	}

	public void SavePricePreset()
	{
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			V.OnCarLiveryEnd = (Action)Delegate.Remove(V.OnCarLiveryEnd, new Action(GoToNextCar));
			_managerUI.ScreenFade.FadeOut(0.75f);
			_cameraManager.garageCam.ResetCamToDefault();
			base.Close();
		}
	}

	private void GetMostLikedCarsFromWw()
	{
	}

	public async UniTask SnapToCar(int index)
	{
		if (_simpleScrollSnap.CenteredPanel != index)
		{
			EnableDisableEngineSoundButton(enable: false);
		}
		ScrollSnapSound.IgnoreSound = true;
		if (_newCarsDict.TryGetValue(index, out var value) && value?.Data != null)
		{
			_suppressPlaceholderForClick = true;
			_clickSnapInProgress = true;
			_clickSnapTargetCarId = value.Data.Serial;
			_lastRequestedLoadCarId = value.Data.Serial;
			_coreGameManager.LoadSellCarFromOutside(value.Data.Serial, immediate: true);
		}
		_simpleScrollSnap.GoToPanel(index);
		await UniTask.Yield(this.GetCancellationTokenOnDestroy());
		ScrollSnapSound.IgnoreSound = false;
	}

	private void UpdateBuyButtonsInteractable(bool interactable)
	{
		BuyDefaultButton.Interactable = interactable;
	}

	private void EnableDisableEngineSoundButton(bool enable)
	{
		if ((bool)EngineSoundButton && (bool)EngineSoundButtonImage)
		{
			EngineSoundButton.interactable = enable;
			Color color = EngineSoundButtonImage.color;
			color.a = (enable ? 1f : 0.5f);
			EngineSoundButtonImage.color = color;
		}
	}
}
