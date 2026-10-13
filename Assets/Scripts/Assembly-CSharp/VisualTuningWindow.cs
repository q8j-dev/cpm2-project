using System;
using System.Collections.Generic;
using System.Linq;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.Scripts.Garage;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Flags;
using Racing.CameraManagement;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Interior;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.VinylsAi;
using _Car_Parking.Scripts.VisualTuning.Wheels;
using _Car_Parking.Scripts.VisualTuningFavourite;

public class VisualTuningWindow : WindowScript, IBackableWindow
{
	private const float OperationDelay = 0.01f;

	private const string WheelDiskTexturesConfigPath = "WheelDiskTexturesConfig";

	public CanvasGroup visualTuningCanvasGroup;

	public VisualTuningElementSpawner VisualTuningElementSpawner;

	public ToggleGroup toggleGroup;

	public UiScrollRectOcclusiion UiScrollRectOcclusiion;

	public GameObject spawner;

	public Transform content;

	public GameObject controlButtonsPanel;

	[HideInInspector]
	public MonoBehaviour spawnerScript;

	[SerializeField]
	private VisualTuningToggle[] _visualTuningToggles = default;

	[SerializeField]
	private Transform ScrollRectPlace = default;

	[SerializeField]
	private ScrollRect ScrollRectPrefab = default;

	[SerializeField]
	private VisualItemElement VisualItemElementPrefab = default;

	[SerializeField]
	private DownloadAssetButton DownloadAssetButtonPrefab = default;

	[SerializeField]
	private Button applyButton = default;

	[SerializeField]
	private Button discardButton = default;

	[SerializeField]
	private Button SetDefaultButton = default;

	[SerializeField]
	private TextMeshProUGUI axleSideText = default;

	[SerializeField]
	private Button leftArrow = default;

	[SerializeField]
	private Button rightArrow = default;

	[SerializeField]
	private GameObject LeftRightArrowsParent = default;

	[SerializeField]
	private GameObject BrakesControlPanel = default;

	[SerializeField]
	private WheelUIChangerPanel WheelUIChangerPanel = default;

	[SerializeField]
	private GameObject colorPanel = default;

	[SerializeField]
	private VisualColorPanel viusalColorPanel = default;

	[SerializeField]
	private VisualTuningBodyKitsPanel visualTuningBodyKitsPanel = default;

	[SerializeField]
	private VisualPolicePanel VisualPolicePanel = default;

	[SerializeField]
	private BrakesViusalTuningPanel BrakesViusalTuningPanel = default;

	[SerializeField]
	private CalipersTuningPanel CalipersVisualTuningPanel = default;

	[SerializeField]
	private RectTransform LayoutGroup = default;

	[SerializeField]
	private GameObject LeftPanel = default;

	[SerializeField]
	private VinylsEditor VinylsPanel = default;

	[SerializeField]
	private FavouriteWheelsController FavouriteWheelsController = default;

	[SerializeField]
	private Texture InteriorIcon = default;

	[SerializeField]
	private Sprite coinIcon = default;

	[SerializeField]
	private Sprite moneyIcon = default;

	[SerializeField]
	private Button rideCarButton = default;

	private ScrollRect _scrollRect;

	private FlagsDataSO _flagsDataSo;

	private VisualTuningTypes _currentVisualTuningTypes;

	private VisualItemElement _selectedVisualItemElement;

	private ICarInfoService _carInfoService;

	private CarInfo _carInfo;

	private VisualTuningPricesService _visualTuningPricesService;

	private ExteriorTuning _exteriorTuning;

	private BrakeTuningConfig _brakeTuningConfig;

	private MenuControl _menuControl;

	private IPaintDataService _paintDataService;

	private PlayerVinylCensorChecker _playerVinylCensorChecker;

	private GarageController _garageController;

	private bool _mustSpawnFavourite;

	private bool _operationSkip;

	private bool _isDestroyed;

	private List<VisualItemElement> _cachedVisualTuningElements;

	private VehicleData _vehicleData;

	private Powertrain _powertrain;

	private IVisualTuningViewModelComplexOne _visualTuningViewModelComplexOne;

	private IVisualTuningViewModelComplexParams _visualTuningViewModelComplexParams;

	private IVisualTuningUpdateAction _visualTuningUpdateAction;

	private IWheelsDataService _wheelsDataService;

	private GraphicRaycaster _graphicRaycaster;

	private ITimerTuningService _timerTuningService;

	private CameraManager _cameraManager;

	private WheelsDiskTuningConfig _wheelsDiskTuningConfig;

	private IPrefabFactory _prefabFactory;

	private WheelDiskTexturesConfig _wheelDiskTexturesConfig;

	private EventRegistry _eventRegistry;

	private RegistryHandle<BoolEvent> _onNextPrevious = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnNextPrevious);

	private RegistryHandle<Vector2Variable> _lookHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.Look);

	private RegistryHandle<Vector2Variable> _lookVinylHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookVinyl);

	private RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

	private RegistryHandle<FloatVariable> _zoomVinylHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.ZoomVinyl);

	private List<VisualTuningToggle> _actualToggles = new List<VisualTuningToggle>();

	private InteriorsVisualTuningPart _interiorsTuning;

	private ICarTuningFactory _carTuningFactory;

	private SoundBankLoadService _soundBankService;

	private ISteamAchievementService _steamAchievementService;

	private const float PaintCompareEpsilon = 0.002f;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.VisualTuning;

	protected override bool ChangeRootGameInputGroup => true;

	public PoliceVisualScope PoliceVisualScope
	{
		get
		{
			VisualTuningScope scope = _visualTuningViewModelComplexOne.GetScope(VisualTuningTypes.Police);
			if (scope == null)
			{
				return null;
			}
			return scope as PoliceVisualScope;
		}
	}

	public BodyKitsVisualScope BodyKitsVisualScope => _visualTuningViewModelComplexOne.GetScope(VisualTuningTypes.BodyKits) as BodyKitsVisualScope;

	public VisualTuningTypes VisualTuningTypes => _currentVisualTuningTypes;

	private VisualTuningScope CurrentVisualScope => _visualTuningViewModelComplexOne.GetScope(VisualTuningTypes);

	public event Action OnTuningValuesChanged;

	[Inject]
	public void Construct(WheelsDiskTuningConfig wheelsDiskTuningConfig, FlagsDataSO flagsDataSo, IPrefabFactory prefabFactory, IVisualTuningViewModelComplexOne visualTuningViewModelComplexOne, IVisualTuningViewModelComplexParams visualTuningViewModelComplexParams, IVisualTuningUpdateAction visualTuningUpdateAction, ManagerUI managerUI, ICarInfoService carInfoService, VisualTuningPricesService visualTuningPricesService, ExteriorTuning exteriorTuning, MenuControl menuControl, BrakeTuningConfig brakeTuningConfig, IPaintDataService paintDataService, IWheelsDataService wheelsDataService, ITimerTuningService timerTuningService, CameraManager cameraManager, EventRegistry eventRegistry, PlayerVinylCensorChecker playerVinylCensorChecker, GarageController garageController, ISteamAchievementService steamAchievementService, ICarTuningFactory carTuningFactory, SoundBankLoadService soundBankService)
	{
		_soundBankService = soundBankService;
		_carTuningFactory = carTuningFactory;
		_cameraManager = cameraManager;
		_timerTuningService = timerTuningService;
		_wheelsDataService = wheelsDataService;
		_paintDataService = paintDataService;
		_menuControl = menuControl;
		_exteriorTuning = exteriorTuning;
		_visualTuningPricesService = visualTuningPricesService;
		_carInfoService = carInfoService;
		_visualTuningViewModelComplexOne = visualTuningViewModelComplexOne;
		_visualTuningViewModelComplexParams = visualTuningViewModelComplexParams;
		_visualTuningUpdateAction = visualTuningUpdateAction;
		_flagsDataSo = flagsDataSo;
		_wheelsDiskTuningConfig = wheelsDiskTuningConfig;
		_prefabFactory = prefabFactory;
		_managerUI = managerUI;
		_brakeTuningConfig = brakeTuningConfig;
		_eventRegistry = eventRegistry;
		_playerVinylCensorChecker = playerVinylCensorChecker;
		_garageController = garageController;
		_steamAchievementService = steamAchievementService;
	}

	protected override void OnEnable()
	{
		_onNextPrevious.Get(_eventRegistry).Subscribe(NextPreviousTuning);
		base.OnEnable();
	}

	private void OnDisable()
	{
		_onNextPrevious.Get(_eventRegistry).Unsubscribe(NextPreviousTuning);
	}

	private void Update()
	{
		if (!VinylsPanel.gameObject.activeInHierarchy)
		{
			_zoomHandle.Get(_eventRegistry).Value = _zoomVinylHandle.Get(_eventRegistry).Value;
			_lookHandle.Get(_eventRegistry).Value = _lookVinylHandle.Get(_eventRegistry).Value;
		}
	}

	private void NextPreviousTuning(bool isNext)
	{
		VisualTuningTypes currentVisualTuningTypes = _currentVisualTuningTypes;
		if (currentVisualTuningTypes == VisualTuningTypes.Vinyls || currentVisualTuningTypes == VisualTuningTypes.PlateVinyls || currentVisualTuningTypes == VisualTuningTypes.WindowVinyls)
		{
			VinylsPanel.OnNextPreviousEditor(isNext);
		}
		else
		{
			if (_actualToggles.Count == 0)
			{
				return;
			}
			int num = -1;
			int i;
			for (i = 0; i < _actualToggles.Count; i++)
			{
				if (_actualToggles[i].togglePanelState == _currentVisualTuningTypes)
				{
					num = (isNext ? ((i + 1) % _actualToggles.Count) : ((i - 1 + _actualToggles.Count) % _actualToggles.Count));
					break;
				}
			}
			if (num != -1)
			{
				_actualToggles[i].SetIsOnWithoutNotify(value: false);
				_actualToggles[num].SetIsOnWithoutNotify(value: true);
				ChangePanel(_actualToggles[num].togglePanelState);
			}
		}
	}

	protected override void Start()
	{
		base.Start();
		_soundBankService?.TryLoadBank(SoundBankType.CarSirens).Forget();
		_cameraManager.garageCam.ResetCameraForVisualTuning();
		if (V.Car.TryGetComponent<VehicleData>(out _vehicleData))
		{
			_vehicleData.racingState = RacingState.OnVisualTuning;
			_garageController.EnableCarPhysics(_vehicleData, isEnabled: true);
		}
		if (V.Car.TryGetComponent<Powertrain>(out _powertrain))
		{
			_powertrain.SetAlignCar(state: true);
			_powertrain.ResetTransmission(resetGear: false);
		}
		_graphicRaycaster = _managerUI.GetComponent<GraphicRaycaster>();
		InitBackButton();
		_menuControl.tempTarget.position = V.Car.transform.position;
		WheelUIChangerPanel.currentWheelAxleType = WheelTuningAxleType.Both;
		AddButtonsListeners();
		HideUnusedTuningToggles();
		OnClickExecute(VisualTuningTypes.BodyKits, "InitCarInfo");
		SetFirstTuningType();
		_visualTuningUpdateAction.OnValuesChanged += OnVisualTuningValuesChanged;
		FavouriteWheelsController.OnFavouriteWheelsShow += ShowFavouriteWheels;
		FavouriteWheelsController.OnFavouriteWheelsUpdated += UpdateFavouriteWheels;
		FlagsStorage.OnVisualTuning = true;
		FlagsStorage.IsCarInService = true;
		LeftPanel.SetActive(value: true);
		VinylsPanel.SetActive(active: false);
		LoadWheelDiskTexturesConfig().Forget();
		_interiorsTuning = _prefabFactory.Create<InteriorsVisualTuningPart>(Array.Empty<object>());
		_interiorsTuning.Init(this, _vehicleData, VisualItemElementPrefab, InteriorIcon, DownloadAssetButtonPrefab);
	}

	protected override void OnDestroy()
	{
		_soundBankService?.TryUnloadBank(SoundBankType.CarSirens).Forget();
		_visualTuningUpdateAction.OnValuesChanged -= OnVisualTuningValuesChanged;
		FavouriteWheelsController.OnFavouriteWheelsShow -= ShowFavouriteWheels;
		FavouriteWheelsController.OnFavouriteWheelsUpdated -= UpdateFavouriteWheels;
		FlagsStorage.OnVisualTuning = false;
		FlagsStorage.IsCarInService = false;
		_isDestroyed = true;
		_cameraManager.garageCam.ResetCamToDefault();
	}

	private void HideUnusedTuningToggles()
	{
		_actualToggles.Clear();
		VisualTuningToggle[] visualTuningToggles = _visualTuningToggles;
		foreach (VisualTuningToggle visualTuningToggle in visualTuningToggles)
		{
			if (VisualTuningExclude.IsDisabledFeatureType(visualTuningToggle.togglePanelState))
			{
				visualTuningToggle.SetActive(active: false);
			}
			else if (CarIdStorage.IsCarWithoutBodyWheelTuning(_vehicleData.CarId) && VisualTuningExclude.IsF1ExcludedType(visualTuningToggle.togglePanelState))
			{
				visualTuningToggle.SetActive(active: false);
				Navigation navigation = visualTuningToggle.navigation;
				navigation.mode = Navigation.Mode.None;
				visualTuningToggle.navigation = navigation;
			}
			else
			{
				_actualToggles.Add(visualTuningToggle);
			}
		}
	}

	private async UniTask LoadWheelDiskTexturesConfig()
	{
		AsyncOperationHandle<WheelDiskTexturesConfig> asyncOperationHandle = Addressables.LoadAssetAsync<WheelDiskTexturesConfig>("WheelDiskTexturesConfig").BindTo(base.gameObject);
		await asyncOperationHandle.Task;
		if (!_isDestroyed)
		{
			_wheelDiskTexturesConfig = asyncOperationHandle.Result;
		}
	}

	private void CreateContentPanel(float pivot, bool offsetByY = false)
	{
		DestroyContentPanel();
		_scrollRect = _prefabFactory.Instantiate(ScrollRectPrefab, ScrollRectPlace);
		_scrollRect.content.pivot = new Vector2(pivot, _scrollRect.content.pivot.y);
		content = _scrollRect.content;
		if (offsetByY)
		{
			RectTransform component = content.GetComponent<RectTransform>();
			Vector2 anchoredPosition = component.anchoredPosition;
			component.anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + 10f);
		}
		UiScrollRectOcclusiion = _scrollRect.gameObject.GetComponent<UiScrollRectOcclusiion>();
		toggleGroup = content.GetComponent<ToggleGroup>();
	}

	public void DestroyContentPanel()
	{
		if ((bool)_scrollRect)
		{
			UnityEngine.Object.Destroy(_scrollRect.gameObject);
		}
	}

	public void SetFirstTuningType()
	{
		VisualTuningToggle[] visualTuningToggles = _visualTuningToggles;
		foreach (VisualTuningToggle visualTuningToggle in visualTuningToggles)
		{
			if (visualTuningToggle.gameObject.activeSelf)
			{
				visualTuningToggle.SetIsOnWithoutNotify(value: true);
				ChangePanel(visualTuningToggle.togglePanelState);
				break;
			}
		}
	}

	private void Init(VisualTuningTypes tuningTypes)
	{
		SetDefaultButton.gameObject.SetActive(value: false);
		FavouriteWheelsController.OnFavouriteSwitchPanelEnabled?.Invoke(tuningTypes == VisualTuningTypes.Wheels);
		RevertCurrentCategory();
		SetActiveControlButtons(isActive: false);
		LeftRightArrowsParent.SetActive(value: false);
		BrakesControlPanel.SetActive(value: false);
		CalipersVisualTuningPanel.SetActive(active: false);
		VisualPolicePanel.Disable();
		CreateContentPanel((tuningTypes == VisualTuningTypes.Colors) ? 0.5f : 0f, tuningTypes == VisualTuningTypes.Wheels);
		bool active = tuningTypes == VisualTuningTypes.Wheels;
		WheelUIChangerPanel.transform.parent.gameObject.SetActive(active);
		if (tuningTypes == VisualTuningTypes.Vinyls)
		{
			LeftPanel.SetActive(value: false);
			VinylsPanel.SetActive(active: true);
			VinylsPanel.ShowVinylsWindow();
			_visualTuningToggles.First().SetIsOnWithoutNotify(value: true);
			_visualTuningToggles.Last().SetIsOnWithoutNotify(value: false);
		}
		if (tuningTypes != VisualTuningTypes.Vinyls)
		{
			InputMapSetEvent.TrySetGroup(WindowInputGroup);
		}
		colorPanel.SetActive(tuningTypes == VisualTuningTypes.Colors);
		_carInfo = _carInfoService.GetFromSlot(V.SlotID);
		if (tuningTypes != VisualTuningTypes.Vinyls)
		{
			VisualScopesInits(tuningTypes);
		}
		_currentVisualTuningTypes = tuningTypes;
		if (VisualTuningTypes.Colors == tuningTypes)
		{
			viusalColorPanel.colorPicker.enabled = true;
		}
	}

	private void RevertCurrentCategory()
	{
		if (_currentVisualTuningTypes == VisualTuningTypes.Interiors)
		{
			_interiorsTuning.ClosePanel();
		}
		if (_currentVisualTuningTypes == VisualTuningTypes.Brakes || _currentVisualTuningTypes == VisualTuningTypes.Calipers)
		{
			OnClickExecute(_currentVisualTuningTypes, "OnChangeScope");
		}
		else
		{
			OnClickExecute(_currentVisualTuningTypes, "Revert");
		}
	}

	private List<WheelDiskData> TryInitWheelsForF1(List<WheelDiskData> wheelsToSpawn)
	{
		VehicleStockTuningData vehicleStockTuningData = (_vehicleData ? CarTuningFactory.GetDefaultCarTuningDataConfig(_vehicleData.CarId) : null);
		if (vehicleStockTuningData == null || vehicleStockTuningData.GetClass != VehicleClass.F1)
		{
			return wheelsToSpawn;
		}
		WheelDiskData wheelData = WheelsTuningScope.GetWheelData(vehicleStockTuningData.Tuning.FrontWheelData.RimModelId, _wheelsDiskTuningConfig.AllWheels);
		wheelData.IsInteractableUiItem = false;
		wheelData.ShowPrice = false;
		GetComponent<VisualTuningFavouriteControllerBase>().OnFavouriteSwitchPanelEnabled?.Invoke(obj: false);
		return new List<WheelDiskData> { wheelData };
	}

	private void VisualScopesInits(VisualTuningTypes tuningTypes)
	{
		ClearCollections();
		OnClickExecute(tuningTypes, "Init");
		switch (tuningTypes)
		{
		case VisualTuningTypes.Wheels:
		{
			SetDefaultButton.SetActive(active: true);
			LeftRightArrowsParent.SetActive(value: true);
			ApplyWheelRadiusLimits();
			WheelUIChangerPanel.Init(_carInfoService.GetFromSlot(V.SlotID).TuningData.FrontWheelData, _visualTuningViewModelComplexOne, this);
			ChangeSpawner();
			SetActiveControlButtons(isActive: false);
			List<WheelDiskData> favouriteWheels = GetFavouriteWheels();
			List<WheelDiskData> wheelsToSpawn = (_mustSpawnFavourite ? favouriteWheels : _wheelsDiskTuningConfig.AllWheels.ToList());
			wheelsToSpawn = TryInitWheelsForF1(wheelsToSpawn);
			VisualTuningElementSpawner.SpawnList(wheelsToSpawn, VisualItemElementPrefab, InitWheels);
			FavouriteWheelsController.CurrentFavouriteCount = favouriteWheels.Count;
			break;
		}
		case VisualTuningTypes.Flags:
			VisualTuningElementSpawner.SpawnList(_flagsDataSo.flags.ToList(), VisualItemElementPrefab, InitFlags);
			break;
		case VisualTuningTypes.Colors:
			viusalColorPanel.DisableOrEnableWheelUIElements(state: false);
			viusalColorPanel.Init(_carInfo, ColorsScope.ColorsType.Car);
			SetActiveControlButtons(isActive: false);
			break;
		case VisualTuningTypes.BodyKits:
			VisualTuningElementSpawner.ResetVisualTuningElementsCollection();
			visualTuningBodyKitsPanel.Init();
			break;
		case VisualTuningTypes.Brakes:
			SetDefaultButton.SetActive(active: true);
			BrakesViusalTuningPanel.gameObject.SetActive(value: true);
			BrakesControlPanel.SetActive(value: true);
			SetActiveControlButtons(isActive: false);
			break;
		case VisualTuningTypes.Police:
			VisualPolicePanel.Init(this, _vehicleData);
			break;
		case VisualTuningTypes.Interiors:
			_interiorsTuning.OpenPanel();
			break;
		default:
			Debug.LogError($"Unkown visual scope type {tuningTypes}");
			break;
		}
	}

	private List<WheelDiskData> GetFavouriteWheels()
	{
		List<WheelDiskData> list = new List<WheelDiskData>();
		WheelDiskData[] allWheels = _wheelsDiskTuningConfig.AllWheels;
		foreach (WheelDiskData wheelDiskData in allWheels)
		{
			if (FavouriteWheelsController.IsItemFavourite(wheelDiskData.index))
			{
				list.Add(wheelDiskData);
			}
		}
		return list;
	}

	private void ShowFavouriteWheels(bool showFavourite)
	{
		if (CurrentVisualScope is WheelsTuningScope)
		{
			_mustSpawnFavourite = showFavourite;
			Init(VisualTuningTypes.Wheels);
		}
	}

	private void UpdateFavouriteWheels()
	{
		if (CurrentVisualScope is WheelsTuningScope && _mustSpawnFavourite)
		{
			Init(VisualTuningTypes.Wheels);
		}
	}

	private void AddButtonsListeners()
	{
		applyButton.onClick.AddListener(TryApply);
		discardButton.onClick.AddListener(Discard);
		leftArrow.onClick.AddListener(() =>
		{
			ChangeSpawner(WheelUIChangerPanel.OnArrowPressed(isLeft: true));
		});
		rightArrow.onClick.AddListener(() =>
		{
			ChangeSpawner(WheelUIChangerPanel.OnArrowPressed(isLeft: false));
		});
		rideCarButton.onClick.AddListener(RideCar);
		SetDefaultButton.onClick.AddListener(SetDefault);
	}

	private void TryApply()
	{
		if (CanShowConfirmationWindow())
		{
			visualTuningCanvasGroup.alpha = 0f;
			string format = new LocalizedString("VisualTuningWindow.Buythisitemfor");
			string arg = ((_currentVisualTuningTypes == VisualTuningTypes.Colors) ? _visualTuningPricesService.GetColorPrice().ToString() : (_selectedVisualItemElement ? _selectedVisualItemElement.elementPrice.text : string.Empty));
			if (CurrentVisualScope != null)
			{
				CombinedPrice boughtItemPrice = CurrentVisualScope.GetBoughtItemPrice();
				if (boughtItemPrice != null && (int)boughtItemPrice.Coin > 0)
				{
					arg = ((int)boughtItemPrice.Coin).ToString();
				}
			}
			_managerUI.ShowConfirmWindow(Apply, new LocalizedString("Playerprofilewindow.Message"), string.Format(format, arg), () =>
			{
				visualTuningCanvasGroup.alpha = 1f;
			}).Forget();
		}
		else
		{
			Apply();
		}
	}

	private bool ColorsPurchaseNeedsConfirmation()
	{
		if (!viusalColorPanel)
		{
			return false;
		}
		ColorsScope.ColorsType currentColorType = viusalColorPanel.currentColorType;
		if (currentColorType == ColorsScope.ColorsType.Car || currentColorType == ColorsScope.ColorsType.Wheels || currentColorType == ColorsScope.ColorsType.BodyKits)
		{
			return !_paintDataService.CheckPaintBought(viusalColorPanel.currentColorMatType);
		}
		return false;
	}

	private bool CanShowConfirmationWindow()
	{
		if (_currentVisualTuningTypes == VisualTuningTypes.Colors)
		{
			return ColorsPurchaseNeedsConfirmation();
		}
		if (CurrentVisualScope != null)
		{
			CombinedPrice boughtItemPrice = CurrentVisualScope.GetBoughtItemPrice();
			if (boughtItemPrice != null)
			{
				return (int)boughtItemPrice.Coin > 0;
			}
		}
		int num = 0;
		if ((bool)_selectedVisualItemElement)
		{
			num = BodyKitsService.GetCoinPriceForKit(_selectedVisualItemElement.bodyKitType, _selectedVisualItemElement.index);
		}
		int num2;
		if ((bool)_selectedVisualItemElement)
		{
			VisualTuningTypes currentVisualTuningTypes = _currentVisualTuningTypes;
			if (currentVisualTuningTypes == VisualTuningTypes.BodyKits || currentVisualTuningTypes == VisualTuningTypes.Interiors)
			{
				num2 = ((num != 0) ? 1 : 0);
				goto IL_0092;
			}
		}
		num2 = 0;
		goto IL_0092;
		IL_0092:
		bool flag = (byte)num2 != 0;
		bool flag2 = (bool)_selectedVisualItemElement && _currentVisualTuningTypes == VisualTuningTypes.Wheels && WheelsTuningScope.GetWheelData(_selectedVisualItemElement.index, _wheelsDiskTuningConfig.AllWheels).isCoinPrice;
		bool num3 = (_currentVisualTuningTypes == VisualTuningTypes.Calipers || _currentVisualTuningTypes == VisualTuningTypes.Brakes || _currentVisualTuningTypes == VisualTuningTypes.Flags || _currentVisualTuningTypes == VisualTuningTypes.Police) | flag2 | flag;
		bool flag3 = (bool)_selectedVisualItemElement && !_selectedVisualItemElement.IsBought;
		return num3 & flag3;
	}

	private void Apply()
	{
		visualTuningCanvasGroup.alpha = 1f;
		OnClickExecute(_currentVisualTuningTypes, "Apply");
		if (_currentVisualTuningTypes == VisualTuningTypes.Interiors)
		{
			_interiorsTuning.Apply();
		}
		SetActiveControlButtons(isActive: false);
		GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQDQ", "visual_tuning");
		if (_steamAchievementService != null)
		{
			_steamAchievementService.Unlock("ACH_FIRST_CUSTOM_LOOK");
		}
	}

	private void Discard()
	{
		OnClickExecute(_currentVisualTuningTypes, "Discard");
		if (_currentVisualTuningTypes == VisualTuningTypes.Interiors)
		{
			_interiorsTuning.Discard();
		}
		if (_currentVisualTuningTypes == VisualTuningTypes.Flags)
		{
			RefreshFlagsSelection();
		}
		_managerUI.ShowNotificationWindow(new LocalizedString("VisualTuningWindow.Detailreset!"), 3f).Forget();
		if (_currentVisualTuningTypes == VisualTuningTypes.Colors)
		{
			viusalColorPanel.RevertColor();
		}
		SetActiveControlButtons(isActive: false);
		RevertUIElements();
	}

	private void RefreshFlagsSelection()
	{
		_selectedVisualItemElement = null;
		toggleGroup.SetAllTogglesOff(sendCallback: false);
		int num = (_vehicleData ? _vehicleData.carInfo.AdditionalData.FlagId : 0);
		foreach (VisualItemElement cachedVisualTuningElement in _cachedVisualTuningElements)
		{
			if ((bool)cachedVisualTuningElement && cachedVisualTuningElement.index == num)
			{
				cachedVisualTuningElement.Button.SetIsOnWithoutNotify(value: true);
			}
		}
	}

	private void SetDefault()
	{
		OnClickExecute(_currentVisualTuningTypes, "SetDefault");
		RevertUIElements(toDefault: true);
	}

	private void RevertUIElements(bool toDefault = false)
	{
		SetActiveControlButtons(isActive: false);
		switch (_currentVisualTuningTypes)
		{
		case VisualTuningTypes.Wheels:
			switch (WheelUIChangerPanel.currentWheelAxleType)
			{
			case WheelTuningAxleType.Front:
			case WheelTuningAxleType.Both:
				WheelUIChangerPanel.InitWithoutNotify(_carInfoService.GetFromSlot(V.SlotID).TuningData.FrontWheelData);
				break;
			case WheelTuningAxleType.Rear:
				WheelUIChangerPanel.InitWithoutNotify(_carInfoService.GetFromSlot(V.SlotID).TuningData.RearWheelData);
				break;
			}
			break;
		case VisualTuningTypes.Brakes:
			if (toDefault)
			{
				if (_carTuningFactory.CreateDefault(_carInfo.CarID).Tuning.FrontWheelData.Clone() is FrontWheelData frontWheelData2)
				{
					BrakesViusalTuningPanel.SetSliderValue(frontWheelData2.BrakesData.Diameter);
				}
			}
			else
			{
				BrakesViusalTuningPanel.InitWithoutNotify();
			}
			break;
		case VisualTuningTypes.Calipers:
			if (toDefault)
			{
				if (_carTuningFactory.CreateDefault(_carInfo.CarID).Tuning.FrontWheelData.Clone() is FrontWheelData frontWheelData)
				{
					CalipersVisualTuningPanel.SetSliderValue(frontWheelData.BrakesData);
				}
			}
			else
			{
				CalipersVisualTuningPanel.InitWithoutNotify();
			}
			break;
		}
	}

	private void OnVisualTuningValuesChanged()
	{
		CheckBoughtElement();
		OnTuningValuesChanged?.Invoke();
	}

	private void CheckBoughtElement()
	{
		if (!_selectedVisualItemElement)
		{
			return;
		}
		switch (_currentVisualTuningTypes)
		{
		case VisualTuningTypes.Wheels:
			if (WheelUIChangerPanel.GetWheelPaid(_selectedVisualItemElement.index))
			{
				_selectedVisualItemElement.elementPrice.text = "0";
				SetActiveControlButtons(isActive: false);
				_selectedVisualItemElement.IsBought = true;
			}
			break;
		case VisualTuningTypes.Flags:
			if (FlagConstructor.FlagsBought.ContainsKey(_selectedVisualItemElement.index))
			{
				_selectedVisualItemElement.elementPrice.text = "0";
				SetActiveControlButtons(isActive: false);
				_selectedVisualItemElement.IsBought = true;
			}
			break;
		case VisualTuningTypes.Colors:
			if (_selectedVisualItemElement is VisualItemColorElement visualItemColorElement && _paintDataService.CheckPaintBought(visualItemColorElement.carMatType))
			{
				_selectedVisualItemElement.elementPrice.text = "0";
				SetActiveControlButtons(isActive: false);
				_selectedVisualItemElement.IsBought = true;
			}
			break;
		case VisualTuningTypes.BodyKits:
			if (_carInfo.BoughtBodyKits.GetBodyKitPaidStatus(_selectedVisualItemElement.bodyKitType, _selectedVisualItemElement.index))
			{
				_selectedVisualItemElement.elementPrice.text = "0";
				SetActiveControlButtons(isActive: false);
				_selectedVisualItemElement.IsBought = true;
			}
			break;
		case VisualTuningTypes.Interiors:
			if (_carInfo.BoughtBodyKits.GetBodyKitPaidStatus(_selectedVisualItemElement.bodyKitType, _selectedVisualItemElement.index))
			{
				_selectedVisualItemElement.elementPrice.text = "0";
				SetActiveControlButtons(isActive: false);
				_selectedVisualItemElement.IsBought = true;
			}
			break;
		case VisualTuningTypes.Brakes:
			UpdateBrakePriceLabel(_selectedVisualItemElement, WheelsTuningScope.GetBrakeDataData(_selectedVisualItemElement.index, _brakeTuningConfig.AllBrakes));
			break;
		case VisualTuningTypes.Calipers:
			if (_wheelsDataService.IsCaliperBought(_selectedVisualItemElement.index))
			{
				_selectedVisualItemElement.elementPrice.text = "";
				SetActiveControlButtons(isActive: false);
				_selectedVisualItemElement.IsBought = true;
				_selectedVisualItemElement.elementPrice.gameObject.SetActive(value: false);
			}
			else if (CalipersVisualTuningPanel.GetCalipersPaid(_selectedVisualItemElement.index))
			{
				_selectedVisualItemElement.elementPrice.text = "";
				SetActiveControlButtons(isActive: false);
				_selectedVisualItemElement.IsBought = true;
				_selectedVisualItemElement.elementPrice.gameObject.SetActive(value: false);
			}
			break;
		}
	}

	public CarInfo GetCarInfo()
	{
		return _carInfo;
	}

	private bool IsCarPaintChanged(CarPaintColor pending)
	{
		if (_carInfo == null || _carInfo.Colors == null)
		{
			return true;
		}
		CarPaintColor carColor = _carInfo.Colors.CarColor;
		if (pending.CarMatType != carColor.CarMatType)
		{
			return true;
		}
		if (!SameColorWithinEpsilon(pending.Color, carColor.Color))
		{
			return true;
		}
		CarMatType carMatType = pending.CarMatType;
		if (carMatType != CarMatType.Iridescent && carMatType != CarMatType.ChromeIridescent && carMatType != CarMatType.MatteIridescent)
		{
			return false;
		}
		if (!(Mathf.Abs(pending.IridescenceThickness - carColor.IridescenceThickness) >= 0.002f))
		{
			return Mathf.Abs(pending.IridescencePower - carColor.IridescencePower) >= 0.002f;
		}
		return true;
	}

	private static bool SameColorWithinEpsilon(Color a, Color b)
	{
		if (Mathf.Abs(a.r - b.r) < 0.002f && Mathf.Abs(a.g - b.g) < 0.002f && Mathf.Abs(a.b - b.b) < 0.002f)
		{
			return Mathf.Abs(a.a - b.a) < 0.002f;
		}
		return false;
	}

	public void ChangeColor(Color color)
	{
		float iridescenceThickness = 0f;
		float iridescencePower = 0f;
		CarMatType currentColorMatType = viusalColorPanel.currentColorMatType;
		bool flag = currentColorMatType == CarMatType.Iridescent || currentColorMatType == CarMatType.ChromeIridescent || currentColorMatType == CarMatType.MatteIridescent;
		if (viusalColorPanel.currentColorType == ColorsScope.ColorsType.Smoke)
		{
			flag = false;
		}
		viusalColorPanel.ChangeSliderStates(flag);
		viusalColorPanel.ChangeAlphaStates(viusalColorPanel.currentColorType == ColorsScope.ColorsType.Windows);
		if (flag)
		{
			(iridescenceThickness, iridescencePower) = viusalColorPanel.GetThicknessAndPower();
		}
		if ((bool)viusalColorPanel.elementImage)
		{
			viusalColorPanel.elementImage.color = color;
		}
		CarPaintColor carPaintColor = new CarPaintColor
		{
			CarMatType = viusalColorPanel.currentColorMatType,
			Color = color,
			IridescenceThickness = iridescenceThickness,
			IridescencePower = iridescencePower
		};
		SetActiveControlButtons(viusalColorPanel.currentColorType != ColorsScope.ColorsType.Car || IsCarPaintChanged(carPaintColor));
		switch (viusalColorPanel.currentColorType)
		{
		case ColorsScope.ColorsType.Car:
			OnClickExecute(VisualTuningTypes.Colors, "SelectCarColor", carPaintColor);
			break;
		case ColorsScope.ColorsType.Wheels:
			OnClickExecuteWithParams(VisualTuningTypes.Colors, "SelectWheelColor", carPaintColor, viusalColorPanel.currentWheelsColorType);
			break;
		case ColorsScope.ColorsType.Windows:
			carPaintColor.Color.a = viusalColorPanel.WindowAlphaFromSlider(viusalColorPanel.GetAlpha());
			OnClickExecute(VisualTuningTypes.Colors, "SelectWindowColor", carPaintColor);
			break;
		case ColorsScope.ColorsType.Smoke:
			OnClickExecute(VisualTuningTypes.Colors, "SelectSmokeColor", carPaintColor);
			break;
		case ColorsScope.ColorsType.BodyKits:
			OnClickExecute(VisualTuningTypes.Colors, "SelectBodyKitColor", carPaintColor);
			break;
		}
	}

	public void DestroySpawnerScript()
	{
		if ((bool)spawnerScript)
		{
			UnityEngine.Object.Destroy(spawnerScript);
		}
	}

	public void ChangeSpawner(int index = 2)
	{
		WheelUIChangerPanel.currentWheelAxleType = (WheelTuningAxleType)index;
		WheelUIChangerPanel.IgnoreExecute = true;
		axleSideText.text = new LocalizedString("VisualColorPanel." + WheelUIChangerPanel.currentWheelAxleType);
		switch (WheelUIChangerPanel.currentWheelAxleType)
		{
		case WheelTuningAxleType.Front:
		case WheelTuningAxleType.Both:
			WheelUIChangerPanel.Init(_carInfoService.GetFromSlot(V.SlotID).TuningData.FrontWheelData, _visualTuningViewModelComplexOne, this);
			break;
		case WheelTuningAxleType.Rear:
			WheelUIChangerPanel.Init(_carInfoService.GetFromSlot(V.SlotID).TuningData.RearWheelData, _visualTuningViewModelComplexOne, this);
			break;
		}
		OnClickExecute(VisualTuningTypes.Wheels, "SetAxleType", WheelUIChangerPanel.currentWheelAxleType);
	}

	private void ChangeMaterialType(CarMatType carMatType, RawImage image)
	{
		viusalColorPanel.currentColorMatType = carMatType;
		viusalColorPanel.elementImage = image;
		ChangeColor(viusalColorPanel.colorPicker.color);
	}

	public void ChangeIridescent(float value)
	{
		ChangeColor(viusalColorPanel.colorPicker.color);
	}

	public void ChangeAlpha(float arg0)
	{
		ChangeColor(viusalColorPanel.colorPicker.color);
	}

	public void ResetColor(VehicleStockTuningData vehicleStockTuningData, ColorsScope.WheelsColorType wheelsColorType)
	{
		OnClickExecuteWithParams(VisualTuningTypes.Colors, "SetDefaultColor", vehicleStockTuningData, wheelsColorType);
	}

	public void Revert()
	{
		OnClickExecute(_currentVisualTuningTypes, "Revert");
		SetActiveControlButtons(isActive: false);
	}

	public void PlaySound(uint key)
	{
		SoundEventsDataHelper.PlayUISound(key, base.gameObject);
	}

	public void ShowNotification(string message, float duration = 3f)
	{
		_managerUI.ShowNotificationWindow(message, duration).Forget();
	}

	public void SetSelectedVisualItemElement(VisualItemElement element)
	{
		_selectedVisualItemElement = element;
	}

	public void InitColorMatType(GameObject go, object carMatTypes)
	{
		CarMatType castedCarMatType = (CarMatType)carMatTypes;
		VisualItemColorElement element = go.GetComponent<VisualItemColorElement>();
		element.colorMatName.gameObject.SetActive(value: true);
		if (_paintDataService.CheckPaintBought(castedCarMatType))
		{
			element.elementPrice.text = "0";
			element.IsBought = true;
		}
		else
		{
			element.elementPrice.text = _visualTuningPricesService.GetColorPrice().ToString();
		}
		element.carMatType = castedCarMatType;
		element.colorMatName.text = new LocalizedString("Colors." + carMatTypes);
		element.Button.group = toggleGroup;
		element.index = (int)castedCarMatType;
		element.Button.onValueChanged.AddListener((bool x) =>
		{
			if (x)
			{
				PlaySound(SoundEventsDataHelper.TuningSelectPaintSwitch);
				SetActiveControlButtons(isActive: true);
				_selectedVisualItemElement = element;
				ChangeMaterialType(castedCarMatType, element.elementImage);
			}
		});
		bool flag = castedCarMatType == viusalColorPanel.currentColorMatType;
		if (flag)
		{
			viusalColorPanel.elementImage = element.elementImage;
			element.elementImage.color = viusalColorPanel.colorPicker.color;
			_selectedVisualItemElement = element;
		}
		element.Button.SetIsOnWithoutNotify(flag);
	}

	public void InitWheels(GameObject go, object data)
	{
		WheelDiskData castedDiskData = (WheelDiskData)data;
		VisualItemElement element = go.GetComponent<VisualItemElement>();
		element.Button.interactable = castedDiskData.IsInteractableUiItem;
		element.elementPrice.gameObject.SetActive(castedDiskData.ShowPrice);
		element.SpecialStickerImage.gameObject.SetActive(castedDiskData.isTwoPartDisk);
		element.moneyIcon.sprite = (castedDiskData.isCoinPrice ? coinIcon : moneyIcon);
		if ((bool)_wheelDiskTexturesConfig)
		{
			TextureData textureData = _wheelDiskTexturesConfig.AllTextureData.FirstOrDefault((TextureData d) => d.ID == castedDiskData.index);
			if (textureData != null)
			{
				element.elementImage.texture = textureData.Texture;
			}
		}
		element.Button.group = toggleGroup;
		element.index = castedDiskData.index;
		GlobalCost wheelPrice = _visualTuningPricesService.GetWheelPrice(castedDiskData.index);
		TextMeshProUGUI elementPrice = element.elementPrice;
		ObscuredInt price = wheelPrice.Price;
		elementPrice.text = price.ToString() ?? "";
		element.Button.onValueChanged.AddListener((bool x) =>
		{
			if (x)
			{
				FavouriteWheelsController.OnSelectItem(castedDiskData.index);
				PlaySound(SoundEventsDataHelper.TuningSelectRimSwitch);
				SetActiveControlButtons(isActive: true);
				_selectedVisualItemElement = element;
				_ = OnClickExecuteWithDelay(VisualTuningTypes.Wheels, "Select", castedDiskData.index);
			}
		});
		element.Button.SetIsOnWithoutNotify(castedDiskData.index == WheelUIChangerPanel.GetCurrentWheelId());
	}

	public void InitCalipersMesh(GameObject caliperGO, object caliperModel)
	{
		BrakeDiskItemData castedDiskItemData = (BrakeDiskItemData)caliperModel;
		VisualItemElement element = caliperGO.GetComponent<VisualItemElement>();
		element.moneyIcon.sprite = (castedDiskItemData.isCoinPrice ? coinIcon : moneyIcon);
		element.elementImage.texture = castedDiskItemData.PreviewTexture;
		GlobalCost calipersPrice = _visualTuningPricesService.GetCalipersPrice(castedDiskItemData.index);
		element.IsBought = (int)calipersPrice.Price == 0;
		element.elementPrice.text = (element.IsBought ? "" : calipersPrice.Price.ToString());
		element.elementPrice.gameObject.SetActive(!element.IsBought);
		element.Button.group = toggleGroup;
		element.index = castedDiskItemData.index;
		element.Button.onValueChanged.AddListener((bool x) =>
		{
			if (x)
			{
				PlaySound(SoundEventsDataHelper.TuningSelectBrakesSwitch);
				if (BrakesViusalTuningPanel.CurrentAxleType == WheelTuningAxleType.Both)
				{
					if (_carInfo.TuningData.FrontWheelData.BrakesData.CaliperMeshIndex != _carInfo.TuningData.RearWheelData.BrakesData.CaliperMeshIndex)
					{
						SetActiveControlButtons(isActive: true);
					}
					else
					{
						ShowControlPanel(element.index, _carInfo.TuningData.FrontWheelData.BrakesData.CaliperMeshIndex);
					}
				}
				else
				{
					WheelTuningAxleType currentAxleType2 = BrakesViusalTuningPanel.CurrentAxleType;
					int selectedIndex = ((currentAxleType2 == WheelTuningAxleType.Front || currentAxleType2 == WheelTuningAxleType.Both) ? _carInfo.TuningData.FrontWheelData.BrakesData.CaliperMeshIndex : _carInfo.TuningData.RearWheelData.BrakesData.CaliperMeshIndex);
					ShowControlPanel(element.index, selectedIndex);
				}
				_selectedVisualItemElement = element;
				OnClickExecuteWithParams(VisualTuningTypes.Calipers, "SelectCaliperMesh", castedDiskItemData.index, false);
			}
		});
		WheelTuningAxleType currentAxleType = BrakesViusalTuningPanel.CurrentAxleType;
		bool flag = currentAxleType == WheelTuningAxleType.Front || currentAxleType == WheelTuningAxleType.Both;
		int num = (flag ? _carInfo.TuningData.FrontWheelData.BrakesData.CaliperMeshIndex : _carInfo.TuningData.RearWheelData.BrakesData.CaliperMeshIndex);
		element.Button.SetIsOnWithoutNotify(castedDiskItemData.index == num);
		CheckToggleState(element, flag, _carInfo.TuningData.FrontWheelData.BrakesData.CaliperMeshIndex, _carInfo.TuningData.RearWheelData.BrakesData.CaliperMeshIndex);
		_cachedVisualTuningElements.Add(element);
	}

	public void InitBrakesDisc(GameObject caliperGO, object brakesDisc)
	{
		BrakeDiskItemData castedDiskItemData = (BrakeDiskItemData)brakesDisc;
		VisualItemElement element = caliperGO.GetComponent<VisualItemElement>();
		element.moneyIcon.sprite = (castedDiskItemData.isCoinPrice ? coinIcon : moneyIcon);
		element.elementImage.texture = castedDiskItemData.PreviewTexture;
		UpdateBrakePriceLabel(element, castedDiskItemData);
		element.titleText.transform.parent.gameObject.SetActive(value: true);
		element.titleText.text = castedDiskItemData.BrakeDiscType.ToString();
		element.Button.group = toggleGroup;
		element.index = castedDiskItemData.index;
		element.Button.onValueChanged.AddListener((bool x) =>
		{
			if (x)
			{
				PlaySound(SoundEventsDataHelper.TuningSelectBrakesSwitch);
				if (BrakesViusalTuningPanel.CurrentAxleType == WheelTuningAxleType.Both)
				{
					if (_carInfo.TuningData.FrontWheelData.BrakesData.DiscType != _carInfo.TuningData.RearWheelData.BrakesData.DiscType)
					{
						SetActiveControlButtons(isActive: true);
					}
					else
					{
						ShowControlPanel(element.index, (int)_carInfo.TuningData.FrontWheelData.BrakesData.DiscType);
					}
				}
				else
				{
					int selectedIndex = (int)((BrakesViusalTuningPanel.CurrentAxleType == WheelTuningAxleType.Front) ? _carInfo.TuningData.FrontWheelData.BrakesData.DiscType : _carInfo.TuningData.RearWheelData.BrakesData.DiscType);
					ShowControlPanel(element.index, selectedIndex);
				}
				_selectedVisualItemElement = element;
				OnClickExecuteWithParams(VisualTuningTypes.Brakes, "SelectDiscTexture", (int)castedDiskItemData.BrakeDiscType, false);
			}
		});
		_cachedVisualTuningElements.Add(element);
	}

	private void ShowControlPanel(int index, int selectedIndex)
	{
		SetActiveControlButtons(index != selectedIndex);
	}

	public void CheckInstalledElement(List<VisualItemElement> visualTuningElements, bool isFront, int frontIndex, int rearIndex)
	{
		foreach (VisualItemElement visualTuningElement in visualTuningElements)
		{
			CheckToggleState(visualTuningElement, isFront, frontIndex, rearIndex);
		}
	}

	public void ClearCollections()
	{
		_cachedVisualTuningElements = new List<VisualItemElement>();
		LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutGroup);
	}

	public List<VisualItemElement> GetCachedElements()
	{
		return _cachedVisualTuningElements;
	}

	public void CheckToggleState(VisualItemElement element, bool isFront, int frontIndex, int rearIndex)
	{
		int num = (isFront ? frontIndex : rearIndex);
		element.Button.SetIsOnWithoutNotify(element.index == num);
	}

	public void InitFlags(GameObject go, object data)
	{
		FlagItem castedData = (FlagItem)data;
		VisualItemElement element = go.GetComponent<VisualItemElement>();
		element.elementImage.texture = castedData.texture;
		element.Button.group = toggleGroup;
		element.index = castedData.data.id;
		bool flag = castedData.data.id == 0 || FlagConstructor.FlagsBought.ContainsKey(element.index);
		int num = ((!flag) ? ((int)castedData.price) : 0);
		element.IsBought = flag;
		element.elementPrice.text = num.ToString();
		element.Button.onValueChanged.AddListener((bool x) =>
		{
			if (x)
			{
				PlaySound(SoundEventsDataHelper.SmallClickSwitch);
				SetActiveControlButtons(isActive: true);
				_selectedVisualItemElement = element;
				_ = OnClickExecuteWithDelay(VisualTuningTypes.Flags, "Select", castedData.data.id);
			}
		});
		element.Button.SetIsOnWithoutNotify(_carInfo.AdditionalData.FlagId == castedData.data.id);
		_cachedVisualTuningElements.Add(element);
	}

	public void OnClickExecute(VisualTuningTypes scopeType, string method, object argument = null)
	{
		_visualTuningViewModelComplexOne.Execute(scopeType, method, argument);
	}

	public async UniTaskVoid OnClickExecuteWithDelay(VisualTuningTypes scopeType, string method, object argument = null)
	{
		if (!_operationSkip)
		{
			_graphicRaycaster.enabled = false;
			_operationSkip = true;
			_visualTuningViewModelComplexOne.Execute(scopeType, method, argument);
			await UniTask.Delay(TimeSpan.FromSeconds(0.009999999776482582));
			_operationSkip = false;
			_graphicRaycaster.enabled = true;
		}
	}

	public void OnClickExecuteWithParams(VisualTuningTypes scopeType, string method, params object[] argument)
	{
		_visualTuningViewModelComplexParams.Execute(scopeType, method, argument);
	}

	public async UniTaskVoid OnClickExecuteWithParamsWithDelay(VisualTuningTypes scopeType, string method, params object[] argument)
	{
		if (!_operationSkip)
		{
			_operationSkip = true;
			_graphicRaycaster.enabled = false;
			_visualTuningViewModelComplexParams.Execute(scopeType, method, argument);
			await UniTask.Delay(TimeSpan.FromSeconds(0.009999999776482582));
			_operationSkip = false;
			_graphicRaycaster.enabled = true;
		}
	}

	private void ChangeSelectedItemPrice()
	{
		if ((bool)_selectedVisualItemElement)
		{
			_selectedVisualItemElement.elementPrice.text = "0";
		}
	}

	public override void Close()
	{
		if (!base.IsCloseCanceled)
		{
			base.Close();
			if ((bool)_vehicleData)
			{
				_vehicleData.racingState = RacingState.None;
			}
			_graphicRaycaster.enabled = true;
			_managerUI.ShowCarTuningWindow();
			OnClickExecute(_currentVisualTuningTypes, "Revert");
			FlagsStorage.IsCarInService = false;
			_powertrain.SetAlignCar(state: false);
		}
	}

	private void RideCar()
	{
		if (_timerTuningService.IsCarBlocked(V.SlotID))
		{
			_ = _managerUI.ShowNotificationWindow(new LocalizedString("VisualTuningWindow.Yourcarcurrentlyisinservice"), 3f);
		}
		else
		{
			_exteriorTuning.ExitVisualTuning(closeFromUI: false);
		}
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: true);
		_managerUI.SetCurrentWindow(this);
	}

	public void Back()
	{
		_playerVinylCensorChecker.CheckCurrentCar();
		FlagsStorage.OnVisualTuning = false;
		_exteriorTuning.ExitVisualTuning();
		_interiorsTuning.ClosePanel();
		Close();
	}

	public void ChangePanel(VisualTuningTypes togglePanelState)
	{
		_mustSpawnFavourite = false;
		FavouriteWheelsController.ResetToAllView();
		Init(togglePanelState);
	}

	public void SetCurrentVisualTuningTypes(VisualTuningTypes tuningType)
	{
		_currentVisualTuningTypes = tuningType;
	}

	private void UpdateBrakePriceLabel(VisualItemElement element, BrakeDiskItemData castedDiskItemData)
	{
		if ((bool)element && castedDiskItemData != null)
		{
			bool num = _wheelsDataService.IsBrakeStock(castedDiskItemData.BrakeDiscType, _carInfo.CarID);
			GlobalCost globalCost = default;
			if (num)
			{
				element.IsBought = true;
			}
			else
			{
				globalCost = _visualTuningPricesService.GetBrakePrice(castedDiskItemData.index);
				element.IsBought = (int)globalCost.Price == 0;
			}
			element.elementPrice.text = globalCost.Price.ToString();
			element.elementPrice.SetActive(!element.IsBought);
		}
	}

	public void SetActiveControlButtons(bool isActive)
	{
		controlButtonsPanel.SetActive(isActive);
	}

	private void ApplyWheelRadiusLimits()
	{
		if (_carInfo != null)
		{
			VehicleStockTuningData obj = _carTuningFactory?.CreateDefault(_carInfo.CarID);
			float valueOrDefault = (obj?.Tuning?.FrontWheelDataLimits?.MaxWheelRadius).GetValueOrDefault();
			float valueOrDefault2 = (obj?.Tuning?.RearWheelDataLimits?.MaxWheelRadius).GetValueOrDefault();
			WheelUIChangerPanel.SetWheelRadiusLimits(valueOrDefault, valueOrDefault2);
		}
	}
}
