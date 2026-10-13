using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.Scripts.Garage;
using Cinemachine;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DataEntities.Vinyl;
using Persistence.Systems;
using Racing.CameraManagement;
using Racing.CameraManagement.SpecificTypes;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Steam.InputHints;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.Vinyls.UIElements;
using _Car_Parking.Scripts.VisualTuningFavourite;
using _Car_Parking.Scripts.WindowSystem;

namespace CPM.Scripts.VinylDrawer
{
	public class VinylsEditor : WindowScript, IBackableWindow
	{
		private enum CameraLock
		{
			None = 0,
			AutoY = 1,
			Edit = 2,
			Full = 3
		}

		private enum ScrollUpdateDirection
		{
			None = 0,
			Up = 1,
			Down = 2
		}

		private enum RowTypes
		{
			Vinyls = 0,
			PlateVinyls = 1,
			WindowVinyls = 2
		}

		private enum PlatesType
		{
			None = 0,
			Front = 1,
			Rear = 2
		}

		private static readonly Color RemoveAllVinylsConfirmWindowBackgroundColor = new Color(0f, 0f, 0f, 0.975f);

		private const float RemoveAllVinylsConfirmWindowConfirmTime = 10f;

		private const float MinScale = 0.1f;

		private const string DegreeSign = "°";

		public const int MaxAllowedStickers = 300;

		public const int MaxAllowedPlateVinyls = 100;

		public const int MaxAllowedWindowVinyls = 150;

		[SerializeField]
		private FavouriteVinylsController FavouriteVinylsController = default;

		[SerializeField]
		private GameObject VinylsSwitchPanel = default;

		[SerializeField]
		private GameObject vinylsUIScroll = default;

		[SerializeField]
		private GameObject stickersScrollParent = default;

		[SerializeField]
		private GameObject planePrefab = default;

		[SerializeField]
		private GameObject visualIconsPrefab = default;

		[SerializeField]
		private GameObject stickerParentPrefab = default;

		[SerializeField]
		private GameObject stickerPrefab = default;

		[SerializeField]
		private Material editLineMaterial = default;

		[SerializeField]
		private Transform joystickSpriteHolder = default;

		[SerializeField]
		private Transform allStickersHolder = default;

		[SerializeField]
		private UiScrollRectOcclusiion stickersScrollOcclusion = default;

		[SerializeField]
		private ScrollRect stickersScrollRect = default;

		[SerializeField]
		private ScrollRect VinylsScrollRect = default;

		[SerializeField]
		private AnimationCurve scaleCurve = default;

		[SerializeField]
		private Scrollbar stickersScrollBar = default;

		[SerializeField]
		private TMP_Text vinylsCountLabel = default;

		[SerializeField]
		private Transform animatedSticker = default;

		[SerializeField]
		private GameObject editUI = default;

		[SerializeField]
		private GameObject clickIndicator = default;

		[SerializeField]
		private TMP_InputField inputText = default;

		[SerializeField]
		private Toggle italicToggle = default;

		[SerializeField]
		private Toggle boldToggle = default;

		[SerializeField]
		private GameObject vinylTextUI = default;

		[SerializeField]
		private FontsInitializer fontsInitializer = default;

		[SerializeField]
		private Transform fieldOfViewUI = default;

		[SerializeField]
		private Joystick movePlateCameraJoystick = default;

		[SerializeField]
		private PlateCameraMoveConfig plateCameraMoveConfig = default;

		[SerializeField]
		private Transform rightButtonsParent = default;

		[SerializeField]
		private int countToGroup = 50;

		[SerializeField]
		private PlateFreeLookWindow PlateFreeLookWindow = default;

		[Header("Camera Lock")]
		[SerializeField]
		[Space]
		private CameraLock LockLevel;

		[SerializeField]
		private Button lockLevelButton = default;

		[SerializeField]
		private TMP_Text lockLevelLabel = default;

		[SerializeField]
		private AnimationCurve buttonsScaleCurve = default;

		[SerializeField]
		[Header("Buttons and Sliders")]
		private VinylJoystick vinylJoystick = default;

		[SerializeField]
		private ResettingSlider scaleSlider = default;

		[SerializeField]
		private ResettingSlider scaleSliderX = default;

		[SerializeField]
		private ResettingSlider scaleSliderY = default;

		[SerializeField]
		private ResettingSlider rotationSlider = default;

		[SerializeField]
		private ResettingSlider zoomSlider = default;

		[Space]
		[SerializeField]
		private Button deleteAllStickersButton = default;

		[SerializeField]
		private Button groupingButton = default;

		[SerializeField]
		private Button saveButton = default;

		[SerializeField]
		private Button deleteButton = default;

		[SerializeField]
		private Button copyButton = default;

		[SerializeField]
		private Button forwardButton = default;

		[SerializeField]
		private Button backwardButton = default;

		[SerializeField]
		private Button flipButton = default;

		[SerializeField]
		private Button rollBackButton = default;

		[SerializeField]
		private Button FreeLookPlateCameraButton = default;

		[Space]
		[SerializeField]
		private ColorPicker colorPicker = default;

		[SerializeField]
		private Slider alphaSlider = default;

		[SerializeField]
		private TMP_InputField hexInputField = default;

		[Space]
		[SerializeField]
		private Toggle showVynilScalesToggle = default;

		[SerializeField]
		private GameObject VinylTransformEditPanel = default;

		[SerializeField]
		private TMP_InputField AngleInputField = default;

		[SerializeField]
		private TMP_InputField ScaleXInputField = default;

		[SerializeField]
		private TMP_InputField ScaleYInputField = default;

		[SerializeField]
		[Header("For text vinyl steam logic")]
		private GameObject TextToggleInputHint = default;

		private VinylsEditorUI _editorUI;

		private int _indicatorShownCount;

		private GameObject _currentVinyl;

		private SpriteRenderer _currentVinylRenderer;

		private TextMesh _currentTextMesh;

		private VinylsType _currentVinylsType;

		private string _currentVinylsPath = "";

		private VinylsEditorOperation _currentOperation;

		private Transform _currentStickersParent;

		private GameObject[] _currentVinylsCollection;

		private StickerItem _currentSticker;

		private Transform _currentPivotTransform;

		private VynilStickerUI _selectedUIVinyl;

		private int _lastVinylIndex;

		private Vector3 _lastNonTextVinylScale = new Vector3(5f, 5f, 1f);

		private int _lastFontIndex;

		private int _lastVinylOrder;

		private VehicleData _vehicleData;

		private bool _isEditingExisting;

		private bool _isMerged;

		private int _lastSavedFrame = -1;

		private bool _saved;

		private Transform _sideCheckParent;

		private GameObject _scaleQuad;

		private Dictionary<VinylsType, Transform> _allStickersParents = new Dictionary<VinylsType, Transform>(3);

		private List<StickerItem> _allStickers = new List<StickerItem>();

		private List<StickerItem> _editingStickers = new List<StickerItem>();

		private MeshCollider[] _meshColliders = new MeshCollider[1];

		private readonly List<CameraLock> _availableLockLevels = new List<CameraLock>
		{
			CameraLock.Edit,
			CameraLock.Full
		};

		private Transform _groupParent;

		private bool _isDragging;

		private Vector3 _editPlaneLastPos;

		private Vector3 _newFocusPos;

		private Transform _draggingEditCollider;

		private Vector3 _oldScalePos;

		private Vector3 _newScalePos;

		private int _sideIndex;

		private Vector3 _initialTouch;

		private Coroutine _blinkRoutine;

		private GameObject _blinkingVinyl;

		private readonly WaitForSeconds _blinkDelay = new WaitForSeconds(0.2f);

		private int _currentLayerMask;

		private string _showMessage = "";

		private int _maxVinylsCount;

		private Color _startingVinylColor;

		private GameObject _freeLookFrontPlate;

		private GameObject _freeLookRearPlate;

		private bool _isFreeLook;

		private const float VINYL_SCALE = 5f;

		private float SCALE_COEF = 15f;

		private PlayerDatas.VinylsDataUtility _vinylsDataUtility;

		private IPrefabFactory _prefabFactory;

		private MenuControl _menuControl;

		private CoreGameManager _coreGameManager;

		private CarCameras _carCameras;

		private ES3File _cachedVinylsFile;

		private VinylDrawer _vinylDrawer;

		private CameraManager _cameraManager;

		private FreeLookCameraHandler _vinylCameraHandler;

		private int _touchCount;

		private float _camYValue = 0.5f;

		public const float DEFAULT_FOV = 50f;

		public const float MAX_FOV = 100f;

		public const float MIN_FOV = 10f;

		private const float TouchDeltaDistance = 20f;

		private float _currentFOV = 50f;

		private bool _isVinylsChanged;

		private int _lastLockLevel;

		private CinemachineCore.AxisInputDelegate _lastAxisInputDelegate;

		private CinemachineBrain.UpdateMethod _previousBrainUpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;

		private ScreenFader _screenFader;

		private GameObject _overrideAnchor;

		private CurrencyManagerController _currencyManagerController;

		private IDisposable _disposableStopBlinkingPointer;

		private Tweener _blinkTween;

		private TMP_Text _placeholderText;

		private IPricesService _pricesService;

		private Camera _mainCam;

		private int _currentPlateIndex;

		private ExteriorTuning _exteriorTuning;

		private VinylsEditorDependencyProvider _vinylsEditorDependencyProvider;

		private GameObject _pointToLook;

		private CinemachineFreeLook _vinylsCam;

		private CinemachineVirtualCamera _benchmarkCam;

		private CinemachineCameraOffset _benchmarkCamOffset;

		private FreeDriveProvider _freeDriveProvider;

		private ISteamAchievementService _steamAchievementService;

		private readonly Dictionary<VinylsEditorInputState, InputGroupsEnum> _correspondingInputGroups = new Dictionary<VinylsEditorInputState, InputGroupsEnum>
		{
			[VinylsEditorInputState.Default] = InputGroupsEnum.None,
			[VinylsEditorInputState.Selection] = InputGroupsEnum.PaintSelection,
			[VinylsEditorInputState.MultiSelection] = InputGroupsEnum.PaintMultiSelection,
			[VinylsEditorInputState.MultiSelected] = InputGroupsEnum.PaintMultiDone,
			[VinylsEditorInputState.OneSelected] = InputGroupsEnum.PaintSelectedOne,
			[VinylsEditorInputState.ColorChange] = InputGroupsEnum.PaintColor,
			[VinylsEditorInputState.SizeChange] = InputGroupsEnum.PaintSize,
			[VinylsEditorInputState.TextChange] = InputGroupsEnum.PaintText,
			[VinylsEditorInputState.PlateTypes] = InputGroupsEnum.PaintPlates,
			[VinylsEditorInputState.Plate] = InputGroupsEnum.PaintSelection,
			[VinylsEditorInputState.Windows] = InputGroupsEnum.PaintSelection,
			[VinylsEditorInputState.CameraOnly] = InputGroupsEnum.PaintNone
		};

		private EventRegistry _eventRegistry;

		private InputMapSetEvent _inputMapSetEvent;

		private UiNavigationValueEvent _uiNavigationValueEvent;

		private InputDeviceDetector _detector;

		private RegistryHandle<BoolEvent> _onConfirmClickHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnConfirmClick);

		private RegistryHandle<ScriptableEvent> _cancelHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Cancel);

		private RegistryHandle<ScrollValueEvent> _scrollValueEventHandle = new RegistryHandle<ScrollValueEvent>(EventRegistryIDs.ScrollValueEvent);

		private RegistryHandle<BoolEvent> _onLayerUpDownHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLayerUpDown);

		private RegistryHandle<ScriptableEvent> _toggleToSizeHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleToSize);

		private RegistryHandle<ScriptableEvent> _toggleToColorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleToColor);

		private RegistryHandle<ScriptableEvent> _copyHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Copy);

		private RegistryHandle<ScriptableEvent> _deleteAllHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.DeleteAll);

		private RegistryHandle<ScriptableEvent> _deleteSelectedHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.DeleteSelected);

		private RegistryHandle<ScriptableEvent> _mirrorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Mirror);

		private RegistryHandle<ScriptableEvent> _setColorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.SetColor);

		private RegistryHandle<ScriptableEvent> _setSizeHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.SetSize);

		private RegistryHandle<ScriptableEvent> _blockCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.BlockCamera);

		private RegistryHandle<ScriptableEvent> _toggleMultiSelectHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleMultiSelect);

		private RegistryHandle<ScriptableEvent> _toggleUiHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleUi);

		private RegistryHandle<FloatVariable> _opacityChangeHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.OpacityChange);

		private RegistryHandle<FloatVariable> _turnLeftRightHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.TurnLeftRight);

		private RegistryHandle<Vector2Variable> _colorChangeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.ColorChange);

		private RegistryHandle<Vector2Variable> _sizeChangeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.SizeChange);

		private RegistryHandle<Vector2Variable> _positionChangeHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.PositionChange);

		private RegistryHandle<ScriptableEvent> _onCancelChangesHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnCancelChanges);

		private RegistryHandle<ScriptableEvent> _onUnselectOrBackHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnUnselectOrBack);

		private RegistryHandle<ScriptableEvent> _acceptHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.Accept);

		private RegistryHandle<ScriptableEvent> _onStartColorMoveHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnStartColorMove);

		private RegistryHandle<ScriptableEvent> _onStartTintMoveHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnStartTintMove);

		private RegistryHandle<ScriptableEvent> _onSetTextHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnSetText);

		private RegistryHandle<ScriptableEvent> _toggleToTextHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleToText);

		private RegistryHandle<ScriptableEvent> _toggleBoldHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleBold);

		private RegistryHandle<ScriptableEvent> _toggleItalicsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleItalics);

		private RegistryHandle<BoolEvent> _onFontUpDownHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnFontUpDown);

		private RegistryHandle<Vector2Variable> _lookHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.Look);

		private RegistryHandle<Vector2Variable> _lookVinylHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.LookVinyl);

		private RegistryHandle<FloatVariable> _zoomVinylHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.ZoomVinyl);

		private RegistryHandle<ScriptableEvent> _onToggleFrontPlateHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnToggleFrontPlate);

		private RegistryHandle<ScriptableEvent> _onToggleBackPlateHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnToggleBackPlate);

		private RegistryHandle<ScriptableEvent> _onEnterPlateTypesHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnEnterPlateTypes);

		private bool _suppressCameraFollow;

		private CancellationTokenSource _selectionCtx;

		private const float ZoomSpeedAlter = 10f;

		private const float FreeLookZoomStep = 70f;

		private float _zoomFovTarget = -1f;

		private float _zoomFovVel;

		private bool _applyingSmoothZoom;

		private const float ZoomFovSmoothTime = 0.12f;

		private const float GamepadZoomScale = 0.5f;

		protected const float MinLookFactor = 0.15f;

		private static readonly Vector2 Multiplier = new Vector2(7.5f, 3.45f);

		private static readonly Vector2 GamepadLookScale = new Vector2(0.65f, 0.77f);

		private const float PositionSpeed = 0.4f;

		private const float ScaleSpeed = 3f;

		private const float RotationSpeed = 90f;

		private const float GamepadRotateScale = 0.4f;

		private const float OpacitySpeed = 1f;

		private const float ColorSpeed = 0.5f;

		private const float SphereCastRadius = 0.015f;

		private const float SphereCastRangeOffset = 0.3f;

		private const float MinCamDot = 0.05f;

		private const float PlateCamMultiplier = 0.1f;

		private readonly Color _debugColor = new Color(0.69f, 0.84f, 0f, 1f);

		private readonly List<VinylsEditorInputState> _editorStateChain = new List<VinylsEditorInputState>();

		private bool _isPointerDown;

		private bool _rotateNeedsRelease;

		private bool _rotateSeenHeld;

		private int _sizeEnterFrame = -1;

		private bool _isColorChangeActive;

		private int _navigateNewIndex;

		private int _navigateExistingIndex;

		private readonly Dictionary<VinylsType, int> _newVinylIndexPerType = new Dictionary<VinylsType, int>();

		private bool _suppressIndexStore;

		private int _lastSizeInfoIndex;

		private readonly RaycastHit[] _sphereCastHits = new RaycastHit[4];

		private readonly List<RaycastResult> _uiRaycastResults = new List<RaycastResult>();

		private float _lastRaycastHitDistance;

		private readonly List<Action> _subscriptionRemovers = new List<Action>();

		[SerializeField]
		[Space]
		[Header("UI Parts")]
		private Button ChangeUIStateButton = default;

		[SerializeField]
		private Button NextTypeButton = default;

		[SerializeField]
		private Button PrevTypeButton = default;

		[SerializeField]
		private GameObject StickerSelectPrefab = default;

		[SerializeField]
		private UiScrollRectOcclusiion VinylsScrollOcclusion = default;

		[SerializeField]
		private Transform ParentOfStickers = default;

		[SerializeField]
		private Transform ParentOfFavouriteStickers = default;

		[SerializeField]
		private TMP_Text CurrentModeLabel = default;

		[SerializeField]
		private Material OriginPlateMaterial = default;

		[SerializeField]
		private Scrollbar AllVinylsBar = default;

		[SerializeField]
		private GameObject[] HideWhen2dEdit = default;

		[SerializeField]
		private GameObject VisualPanel = default;

		[SerializeField]
		private Color AmbientColor = default;

		[SerializeField]
		private VisualTuningWindow VisualTuningWindow = default;

		[Header("Plate Related")]
		[SerializeField]
		[Space]
		private Transform ParentOfPlates = default;

		[SerializeField]
		private GameObject PlateSelectPrefab = default;

		[SerializeField]
		private Button ShowPlatesButton = default;

		[SerializeField]
		private GameObject[] Plates = new GameObject[2];

		[SerializeField]
		private Toggle FrontPlateToggle = default;

		[SerializeField]
		private Toggle RearPlateToggle = default;

		[SerializeField]
		private GameObject PlatesUI = default;

		private Transform _plateCreatorTableTr;

		private Material _plateMat;

		private int _prevRowSelected = -1;

		private int _currentRowSelected;

		private int _currPlateNum = 1;

		private bool _isExiting;

		private bool _isChangingRow;

		private GarageController _garageController;

		private bool _changingPlate;

		private VynilStickerUI _lastSelectedPlate;

		private RenderTexture _plateTempRT;

		private bool _isUIEnabled = true;

		private bool _isVisibleFreeLook = true;

		private List<VinylsEditorModule> _modules = new List<VinylsEditorModule>();

		private PlatesType _lastToggledPlate;

		private const float DelayToClick = 0.5f;

		private bool _delayedNextPrevVinylsTypeClick;

		private Dictionary<GameObject, Transform> _initialParentBodyParts = new Dictionary<GameObject, Transform>();

		private string _lastFrontPlateOwner;

		private string _lastRarePlateOwner;

		private VinylsEditorOperation CurrentOperation
		{
			get
			{
				return _currentOperation;
			}
			set
			{
				_ = _currentOperation;
				_currentOperation = value;
			}
		}

		private float CamYValue
		{
			get
			{
				return _camYValue;
			}
			set
			{
				_camYValue = value;
			}
		}

		public int GroupXSign { get; set; } = 1;

		public bool UseJoystickControl { get; set; }

		public bool IsGrouping { get; private set; }

		public bool IsCreatingTemplate { get; set; }

		public int CurrentSelectedTemplate { get; set; } = -1;

		public bool IsReplacingTemplate { get; private set; }

		public float CurrentFOV => _currentFOV;

		public VinylsType CurrentVinylsType => _currentVinylsType;

		public GameObject CurrentVinyl
		{
			get
			{
				return _currentVinyl;
			}
			private set
			{
				HasVinyl = value != null;
				_currentVinyl = value;
			}
		}

		public bool IsVinylsChanged
		{
			set
			{
				_isVinylsChanged = value;
				if ((bool)rollBackButton)
				{
					if (_editingStickers.Any((StickerItem sticker) => sticker.oldIndex == -1))
					{
						rollBackButton.gameObject.SetActive(value: false);
					}
					else
					{
						rollBackButton.gameObject.SetActive(_isVinylsChanged);
					}
				}
			}
		}

		public static bool HasVinyl { get; private set; }

		private VinylsEditorInputState? PreviousState
		{
			get
			{
				VinylsEditorInputState value;
				if (_editorStateChain.Count < 2)
				{
					value = CurrentState;
				}
				else
				{
					List<VinylsEditorInputState> editorStateChain = _editorStateChain;
					value = editorStateChain[editorStateChain.Count - 2];
				}
				return value;
			}
		}

		private VinylsEditorInputState CurrentState
		{
			get
			{
				return _editorStateChain.LastOrDefault();
			}
			set
			{
				if (_editorStateChain.LastOrDefault() != value && _correspondingInputGroups.TryGetValue(value, out var value2))
				{
					_inputMapSetEvent.TrySetGroup(value2);
					bool flag = _detector.CurrentDeviceType == InputDeviceType.KeyboardMouse;
					showVynilScalesToggle.isOn = !flag && value == VinylsEditorInputState.SizeChange;
					if ((bool)EventSystem.current)
					{
						EventSystem.current.SetSelectedGameObject(null);
					}
					if (value != VinylsEditorInputState.Default)
					{
						_editorStateChain.Add(value);
					}
					OnEditorStateChanged?.Invoke(value);
				}
			}
		}

		public event Action<VinylsEditorInputState> OnEditorStateChanged;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, MenuControl menuControl, CoreGameManager coreGameManager, CarCameras carCameras, ExteriorTuning exteriorTuning, PlayerDatas.VinylsDataUtility vinylsDataUtility, VinylDrawer vinylDrawer, CameraManager cameraManager, ScreenFader screenFader, CurrencyManagerController currencyManagerController, IPricesService pricesService, ManagerUI managerUI, GarageController garageController, VinylsEditorDependencyProvider vinylsEditorDependencyProvider, FreeDriveProvider freeDriveProvider, ISteamAchievementService steamAchievementService)
		{
			_steamAchievementService = steamAchievementService;
			_freeDriveProvider = freeDriveProvider;
			_vinylsEditorDependencyProvider = vinylsEditorDependencyProvider;
			_exteriorTuning = exteriorTuning;
			_managerUI = managerUI;
			_pricesService = pricesService;
			_currencyManagerController = currencyManagerController;
			_screenFader = screenFader;
			_cameraManager = cameraManager;
			_vinylDrawer = vinylDrawer;
			_carCameras = carCameras;
			_coreGameManager = coreGameManager;
			_menuControl = menuControl;
			_prefabFactory = prefabFactory;
			_vinylsDataUtility = vinylsDataUtility;
			_garageController = garageController;
		}

		protected override void Awake()
		{
			base.Awake();
			if (_mainCam == null)
			{
				_mainCam = Camera.main;
			}
			if (!_vinylsCam)
			{
				_vinylCameraHandler = _cameraManager.TryGetCamera(RacingCameraType.VinylEditor) as FreeLookCameraHandler;
				_vinylsCam = _vinylCameraHandler?.GetComponent<CinemachineFreeLook>();
			}
			if (!_benchmarkCam)
			{
				_benchmarkCam = (_cameraManager.TryGetCamera(RacingCameraType.VinylBenchmark) as MonoBehaviour)?.GetComponent<CinemachineVirtualCamera>();
			}
			if (!_benchmarkCamOffset && (bool)_benchmarkCam)
			{
				_benchmarkCamOffset = _benchmarkCam.GetComponent<CinemachineCameraOffset>();
			}
			_sideCheckParent = _vinylsEditorDependencyProvider.sideCheckParent;
			_scaleQuad = _vinylsEditorDependencyProvider.scaleQuad;
			base.gameObject.SetActive(value: false);
			vinylsUIScroll.SetActive(value: true);
			_pointToLook = new GameObject("pointToLook");
			_scaleQuad.GetComponent<AllwaysLookAtCam>().cam = _pointToLook.transform;
			_placeholderText = inputText.placeholder.GetComponent<TMP_Text>();
			alphaSlider.SetValueWithoutNotify(1f);
			_vehicleData = V.Car.GetComponent<VehicleData>();
			UpdateEditUI(toShow: false);
			italicToggle.SetIsOnWithoutNotify(value: false);
			boldToggle.SetIsOnWithoutNotify(value: false);
			showVynilScalesToggle.SetIsOnWithoutNotify(value: false);
			_plateCreatorTableTr = GameObject.Find("Plate Creator Table").transform;
			for (int i = 0; i < Plates.Length; i++)
			{
				Plates[i] = UnityEngine.Object.Instantiate(Plates[i], _plateCreatorTableTr);
				Plates[i].transform.localPosition = new Vector3(0f, 0f, 0f);
				Plates[i].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				Plates[i].SetActive(value: false);
			}
			if (_editorUI == null)
			{
				_editorUI = new VinylsEditorUI(this, editLineMaterial, joystickSpriteHolder, scaleCurve, animatedSticker, zoomSlider, _cameraManager, vinylJoystick, scaleSlider, scaleSliderX, scaleSliderY, rotationSlider, _vinylCameraHandler, buttonsScaleCurve, movePlateCameraJoystick, _benchmarkCamOffset, plateCameraMoveConfig, _freeDriveProvider);
			}
			InitButtons();
			InitUIParts();
		}

		private void InitButtons()
		{
			groupingButton.onClick.AddListener(OnGroupButtonPressed);
			saveButton.onClick.AddListener(SaveVinyl);
			deleteButton.onClick.AddListener(CancelOrDelete);
			copyButton.onClick.AddListener(CopyVinyls);
			forwardButton.onClick.AddListener(ForwardZ);
			backwardButton.onClick.AddListener(BackZ);
			flipButton.onClick.AddListener(FlipX);
			rollBackButton.onClick.AddListener(RollBackAllChanges);
			inputText.onValueChanged.AddListener(OnTextEntered);
			italicToggle.onValueChanged.AddListener((bool _) =>
			{
				SetFontStyle();
			});
			boldToggle.onValueChanged.AddListener((bool _) =>
			{
				SetFontStyle();
			});
			fontsInitializer.onValueChanged.AddListener(ChangeFontIndex);
			colorPicker.onColorChanged.AddListener(OnColorChanged);
			alphaSlider.onValueChanged.AddListener(OnAlphaChanged);
			hexInputField.onEndEdit.AddListener(OnHexChanged);
			lockLevelButton.onClick.AddListener(UpdateLockLevel);
			deleteAllStickersButton.onClick.AddListener(OnDeleteAllStickersClick);
			FreeLookPlateCameraButton.onClick.AddListener(PlateFreeLookToggle);
			showVynilScalesToggle.onValueChanged.AddListener(VinylsInfoActive);
			AngleInputField.onValueChanged.AddListener(OnAngleInputChanged);
			ScaleXInputField.onValueChanged.AddListener(OnWidthInputChanged);
			ScaleXInputField.onSubmit.AddListener(OnWidthInputSubmit);
			ScaleYInputField.onValueChanged.AddListener(OnHeightInputChanged);
			ScaleYInputField.onSubmit.AddListener(OnHeightInputSubmit);
			InitAlterInputScheme();
		}

		private void Update()
		{
			if (_editorUI != null)
			{
				_editorUI.UpdateEditControl();
				if ((bool)CurrentVinyl)
				{
					_editorUI.UpdateJoystickControl(IsGrouping ? _groupParent : CurrentVinyl.transform);
				}
			}
			NewUpdate();
		}

		private void OldUpdate()
		{
			if (CreateByTouch())
			{
				Operate();
			}
			if (((!_isDragging && LockLevel != CameraLock.Full) || _currentVinylsType == VinylsType.PlateVinyls) && TempCameraRotationDisabler.IsTouching && _touchCount == 2 && (bool)_currentPivotTransform)
			{
				ResetFocusPos();
				_cameraManager.InvokeCameraZoom();
			}
			UpdateCurrentVinyl();
		}

		private bool CreateByTouch()
		{
			if (Input.touchCount != 1)
			{
				CountTouches();
				return false;
			}
			_touchCount = 1;
			if ((bool)_currentVinyl && CurrentOperation != VinylsEditorOperation.None)
			{
				return true;
			}
			bool result = false;
			Touch touch = Input.touches[0];
			switch (touch.phase)
			{
			case UnityEngine.TouchPhase.Began:
				_initialTouch = touch.position;
				if (!TempCameraRotationDisabler.IsTouching)
				{
					_initialTouch = Vector3.zero;
				}
				break;
			case UnityEngine.TouchPhase.Ended:
				result = Vector2.Distance(_initialTouch, touch.position) <= 20f;
				break;
			}
			return result;
		}

		private void CountTouches()
		{
			Touch[] touches = Input.touches;
			int num = 0;
			int i = 0;
			for (int num2 = touches.Length; i < num2; i++)
			{
				Touch touch = touches[i];
				if (touch.phase == UnityEngine.TouchPhase.Moved)
				{
					num++;
				}
				if (num > 1)
				{
					break;
				}
			}
			_touchCount = num;
			_touchCount = num;
		}

		private void LateUpdate()
		{
			if ((bool)_cameraManager && (bool)_cameraManager.CurrentBrain && ((!_isDragging && LockLevel != CameraLock.Full) || _currentVinylsType == VinylsType.PlateVinyls))
			{
				_cameraManager.CurrentBrain.ManualUpdate();
			}
		}

		public void SetActiveScroll(bool enable)
		{
			stickersScrollRect.transform.parent.gameObject.SetActive(enable);
		}

		public void Init(VinylsType vinylsType)
		{
			stickersScrollParent.SetActive(value: true);
			if ((vinylsType == VinylsType.PlateVinyls && _currentVinylsType != VinylsType.PlateVinyls) || (_currentVinylsType == VinylsType.PlateVinyls && vinylsType != VinylsType.PlateVinyls))
			{
				_screenFader.FadeOut(0.5f);
			}
			_currentVinylsType = vinylsType;
			if (_cameraManager.CurrentBrain.m_UpdateMethod != CinemachineBrain.UpdateMethod.ManualUpdate)
			{
				_previousBrainUpdateMethod = _cameraManager.CurrentBrain.m_UpdateMethod;
			}
			_cameraManager.CurrentBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.ManualUpdate;
			_vehicleData = V.Car.GetComponent<VehicleData>();
			_lastAxisInputDelegate = CinemachineCore.GetInputAxis;
			_lastLockLevel = _availableLockLevels.Count - 1;
			movePlateCameraJoystick.gameObject.SetActive(vinylsType == VinylsType.PlateVinyls);
			FreeLookPlateCameraButton.gameObject.SetActive(vinylsType == VinylsType.PlateVinyls);
			_benchmarkCamOffset.m_Offset = new Vector3(0f, 0f, _benchmarkCamOffset.m_Offset.z);
			switch (vinylsType)
			{
			case VinylsType.CarVinyls:
				_currentLayerMask = 134217728;
				_showMessage = new LocalizedString("VinylsEditor.ClickonthecartoplaceVinyl");
				_currentVinylsPath = DirectoryHelper.GetCarVinylsPath(_vehicleData.carInfo.CarInstanceId);
				_meshColliders = _vehicleData.CarBodyReferences.Body.GetComponentsInChildren<MeshCollider>();
				_currentPivotTransform = V.Car.transform.Find("CarCommonCameraTarget");
				if (!_currentPivotTransform)
				{
					_currentPivotTransform = V.Car.transform;
				}
				_cameraManager.ActivateCamera(RacingCameraType.VinylEditor, _currentPivotTransform);
				SCALE_COEF = 15f;
				_startingVinylColor = Color.white;
				_maxVinylsCount = 300;
				break;
			case VinylsType.PlateVinyls:
			{
				_currentLayerMask = 8;
				_showMessage = new LocalizedString("VinylsEditor.ClickontheplatetoplaceVinyl");
				_currentVinylsPath = DirectoryHelper.GetPlateVinylsPath(_currentPlateIndex.ToString());
				_meshColliders = Plates[(_currentPlateIndex - 1) % 2].GetComponentsInChildren<MeshCollider>();
				MeshCollider[] meshColliders = _meshColliders;
				for (int i = 0; i < meshColliders.Length; i++)
				{
					meshColliders[i].convex = false;
				}
				_currentPivotTransform = _plateCreatorTableTr;
				_cameraManager.ActivateCamera(RacingCameraType.VinylBenchmark, _currentPivotTransform);
				SCALE_COEF = 30f;
				_startingVinylColor = Color.black;
				fieldOfViewUI.SetParent(base.transform);
				FreeLookPlateCameraButton.transform.SetParent(base.transform);
				movePlateCameraJoystick.transform.SetParent(base.transform);
				_maxVinylsCount = 100;
				break;
			}
			case VinylsType.WindowVinyls:
			{
				_currentLayerMask = 8;
				_showMessage = new LocalizedString("VinylsEditor.ClickonthewindowtoplaceVinyl");
				_currentVinylsPath = DirectoryHelper.GetWindowVinylsPath(_vehicleData.carInfo.CarInstanceId);
				GameObject windowBody = _vehicleData.WindowComposition.WindowBody;
				if ((bool)windowBody)
				{
					windowBody.layer = 3;
					List<MeshCollider> list = windowBody.GetComponents<MeshCollider>().ToList();
					if (list.Count == 0)
					{
						list.Add(windowBody.AddComponent<MeshCollider>());
					}
					if ((bool)_vehicleData.MainModel && _vehicleData.MainModel.TryGetComponent<BodyMarker>(out var component))
					{
						GameObject[] additionalWindowElements = component.AdditionalWindowElements;
						foreach (GameObject gameObject in additionalWindowElements)
						{
							if ((bool)gameObject && gameObject.TryGetComponent<MeshRenderer>(out var _))
							{
								gameObject.gameObject.layer = 3;
								if (!gameObject.TryGetComponent<MeshCollider>(out var component3))
								{
									component3 = gameObject.gameObject.AddComponent<MeshCollider>();
								}
								list.Add(component3);
							}
						}
						additionalWindowElements = component.AdditionalElements;
						foreach (GameObject gameObject2 in additionalWindowElements)
						{
							if (gameObject2.transform.childCount <= 0)
							{
								continue;
							}
							foreach (Transform item in gameObject2.transform)
							{
								if (item.name.Contains("window"))
								{
									item.gameObject.layer = 3;
									if (!item.TryGetComponent<MeshCollider>(out var component4))
									{
										component4 = item.gameObject.AddComponent<MeshCollider>();
									}
									if (!list.Contains(component4))
									{
										list.Add(component4);
									}
								}
							}
						}
					}
					_meshColliders = list.ToArray();
					if (!_overrideAnchor && windowBody.TryGetComponent<MeshRenderer>(out var component5))
					{
						_overrideAnchor = new GameObject("OverrideVinylsAnchor");
						_overrideAnchor.transform.position = component5.bounds.center;
						_overrideAnchor.transform.parent = windowBody.transform;
					}
					_currentPivotTransform = (_overrideAnchor ? _overrideAnchor.transform : windowBody.transform);
					_cameraManager.ActivateCamera(RacingCameraType.VinylEditor, _currentPivotTransform);
				}
				SCALE_COEF = 15f;
				_startingVinylColor = Color.white;
				_maxVinylsCount = 150;
				break;
			}
			}
			if (!EasySaveDataHandler.TryGetOrCreateFile(_currentVinylsPath, out _cachedVinylsFile))
			{
				Debug.LogError("can't get save file");
			}
			_cachedVinylsFile.Sync();
			_currentVinylsCollection = new GameObject[_maxVinylsCount];
			_vinylsDataUtility.LoadAllVinyls(_cachedVinylsFile, _currentVinylsType, _currentVinylsCollection);
			CurrentOperation = VinylsEditorOperation.None;
			_menuControl.tempTarget.rotation = _currentPivotTransform.rotation;
			SetActive(isActive: true);
			UpdateStickers();
			Observable.TimerFrame(2).Subscribe((long l) =>
			{
				UpdateStickersScroll(_allStickers.Count);
			}).AddTo(this);
			colorPicker.SetColor(_startingVinylColor, withAction: false);
			alphaSlider.SetValueWithoutNotify(1f);
			RestoreNewVinylSelection();
			if (_groupParent == null)
			{
				_groupParent = new GameObject("Group").transform;
			}
			_groupParent.SetParent(_vinylDrawer.GetVinylsParent(_currentVinylsType));
			UpdateLockLevel();
			IsVinylsChanged = false;
			UpdateDeleteAllStickersButtonVisible();
		}

		public void UpdateLockLevel()
		{
			int num = _availableLockLevels.Count - 1;
			_lastLockLevel++;
			if (_lastLockLevel > num)
			{
				_lastLockLevel = 0;
			}
			LockLevel = _availableLockLevels[_lastLockLevel];
			if (LockLevel == CameraLock.Full)
			{
				_cameraManager.ChangeTarget(null);
			}
			else
			{
				_cameraManager.ChangeTarget(_currentVinyl ? _menuControl.tempTarget : _currentPivotTransform);
			}
			TMP_Text tMP_Text = lockLevelLabel;
			tMP_Text.text = LockLevel switch
			{
				CameraLock.None => new LocalizedString("VinylsEditor.None"), 
				CameraLock.AutoY => new LocalizedString("VinylsEditor.Yrotation"), 
				CameraLock.Edit => new LocalizedString("VinylsEditor.Editvinyl"), 
				CameraLock.Full => new LocalizedString("VinylsEditor.Full"), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		public void ResetFocusPos()
		{
			_newFocusPos = _currentPivotTransform.position;
		}

		private float GetInputAxis(string axisName)
		{
			if (!TempCameraRotationDisabler.IsTouching)
			{
				return 0f;
			}
			if (_touchCount == 2)
			{
				return 0f;
			}
			if (_currentVinylsType != VinylsType.PlateVinyls && LockLevel != CameraLock.Full && _touchCount <= 2 && !_isDragging)
			{
				VinylsEditorOperation currentOperation = _currentOperation;
				if (currentOperation == VinylsEditorOperation.Create || currentOperation == VinylsEditorOperation.None)
				{
					if (!(axisName == "Mouse X"))
					{
						if (axisName == "Mouse Y")
						{
							if (Input.touchCount > 0)
							{
								return Input.touches[0].deltaPosition.y / 10f;
							}
							return Input.GetAxis(axisName);
						}
						Debug.LogError("Input <" + axisName + "> not recognyzed.", this);
						return 0f;
					}
					if (Input.touchCount > 0)
					{
						return Input.touches[0].deltaPosition.x / 10f;
					}
					return Input.GetAxis(axisName);
				}
			}
			return 0f;
		}

		private void SetActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
			_vinylDrawer.SetCamState(isActive);
			_vinylDrawer.FocusOnVinyls(isActive ? _currentVinylsType : VinylsType.None);
		}

		private void Operate()
		{
			if (UseJoystickControl || _isFreeLook)
			{
				return;
			}
			Vector3 uvLocalPosition = Vector3.zero;
			if ((CurrentOperation != VinylsEditorOperation.Create && CurrentOperation != VinylsEditorOperation.Move) || !HitTestUVPosition(ref uvLocalPosition, out var hit))
			{
				return;
			}
			if (!CurrentVinyl)
			{
				if (!IsGrouping)
				{
					if (_lastVinylOrder + 1 >= _maxVinylsCount)
					{
						_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.VinylIsFull"), 3f);
						return;
					}
					CreateVinyl();
					CurrentState = VinylsEditorInputState.OneSelected;
					MoveVinyl(uvLocalPosition, hit.point);
					_editorUI.HideSomeEditIcons(VinylsEditorOperation.Create);
				}
			}
			else
			{
				MoveVinyl(uvLocalPosition, hit.point);
			}
		}

		private void MoveVinyl(Vector3 uvLocalPosition, Vector3 targetPosition)
		{
			if (IsGrouping)
			{
				PositionGroup(uvLocalPosition);
			}
			else
			{
				PositionCurrentVinyl(uvLocalPosition);
			}
			_editorUI.CreateEditIcons(targetPosition, IsCreatingTemplate ? _groupParent.gameObject : CurrentVinyl);
			if (_currentVinylsType != VinylsType.PlateVinyls && LockLevel < CameraLock.AutoY)
			{
				_vinylCameraHandler.YAxisValue = Mathf.Lerp(_vinylCameraHandler.YAxisValue, CamYValue, Time.deltaTime);
			}
			IsVinylsChanged = true;
		}

		private bool HitTestUVPosition(ref Vector3 uvLocalPosition, out RaycastHit hit)
		{
			if (!Physics.Raycast(_mainCam.ScreenPointToRay(GetPointerScreenPosition()), out hit, _carCameras.distance + 3f, _currentLayerMask))
			{
				return false;
			}
			if (!(hit.collider as MeshCollider))
			{
				return false;
			}
			Vector2 vector = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
			float orthographicSize = _vinylDrawer.Camera.orthographicSize;
			uvLocalPosition.x = vector.x - orthographicSize;
			uvLocalPosition.y = vector.y - orthographicSize;
			uvLocalPosition.z = 0f;
			if (_currentVinylsType == VinylsType.FlagVinyls)
			{
				throw new NotImplementedException("No flags implemented");
			}
			if (!CurrentVinyl || (CurrentOperation != VinylsEditorOperation.Move && CurrentOperation != VinylsEditorOperation.Create))
			{
				return true;
			}
			if (_currentVinylsType == VinylsType.PlateVinyls)
			{
				float z = CurrentVinyl.transform.localEulerAngles.z;
				Vector3 eulerAngles = new Vector3(90f, 0f - z, 0f);
				_scaleQuad.transform.eulerAngles = eulerAngles;
				return true;
			}
			CheckSide(hit.point, hit.normal);
			return true;
		}

		private Vector3 CheckSide(Vector3 origin, Vector3 direction)
		{
			int layerMask = 1048576;
			Ray ray = new Ray
			{
				direction = direction,
				origin = origin
			};
			float z = CurrentVinyl.transform.localEulerAngles.z;
			Vector3 vector = Vector3.zero;
			if (Physics.Raycast(ray, out var hitInfo, 200f, layerMask))
			{
				switch (hitInfo.transform.name)
				{
				case "L":
					vector = new Vector3(0f, 0f, 0f - z);
					CamYValue = 0.5f;
					_sideIndex = 0;
					break;
				case "R":
					vector = new Vector3(0f, 180f, 0f - z);
					CamYValue = 0.5f;
					_sideIndex = 1;
					break;
				case "T":
					vector = new Vector3(90f, 0f - z, 0f);
					CamYValue = 0.8f;
					_sideIndex = 2;
					break;
				case "F":
					vector = new Vector3(0f, 90f, 0f - z);
					CamYValue = 0.5f;
					_sideIndex = 3;
					break;
				case "Rr":
					vector = new Vector3(180f, 90f, 0f - z);
					CamYValue = 0.5f;
					_sideIndex = 3;
					break;
				}
				_scaleQuad.transform.eulerAngles = vector;
			}
			return vector;
		}

		private void CreateVinyl()
		{
			CreateVinyl(_lastVinylOrder + 1);
			ChangeCurrentVinylSprite(_lastVinylIndex);
			_currentSticker.sprite.sprite = V.AllVinyls[_currentSticker.stckerIndex].sprite;
			_currentSticker.backround.color = Color.green;
			UpdateStickersScroll(_allStickers.Count);
			Observable.TimerFrame(2).Subscribe((long _) =>
			{
				stickersScrollBar.value = 0f;
			}).AddTo(this);
		}

		private void OnDeleteAllStickersClick()
		{
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(setBackground: true, RemoveAllVinylsConfirmWindowBackgroundColor, new ConfirmWindow.AdditionalData.ConfirmTimerData(isEnabled: true, 10f));
			_ = _managerUI.ShowConfirmWindow(DeleteAllStickers, new LocalizedString("Visualtuningwindow.DeleteAllVynils"), new LocalizedString("Visualtuningwindow.SureDeleteAllVynils"), null, "", null, additionalData);
		}

		public void PlateFreeLookToggle()
		{
			_isFreeLook = !_isFreeLook;
			ToggleUIAssociatedWithPlates(!_isFreeLook);
			if (_isFreeLook)
			{
				_managerUI.OnActivateBalancePanel?.Invoke(obj: false);
				InstantiatePlates();
				GameObject[] plates = _vehicleData.PlatesComposition.Plates;
				foreach (GameObject gameObject in plates)
				{
					if ((bool)gameObject)
					{
						gameObject.SetActive(value: false);
					}
				}
				SetCameraOnPlate();
				PlateFreeLookWindow.Open(this);
				CurrentState = VinylsEditorInputState.CameraOnly;
			}
			else
			{
				_managerUI.OnActivateBalancePanel?.Invoke(obj: true);
				ResetCamera();
				UnityEngine.Object.Destroy(_freeLookFrontPlate?.gameObject);
				UnityEngine.Object.Destroy(_freeLookRearPlate?.gameObject);
				SetPreviousInputGroup();
			}
		}

		private void ToggleUIAssociatedWithPlates(bool isActive)
		{
			List<GameObject> gameObjects = new List<GameObject> { ChangeUIStateButton.gameObject, movePlateCameraJoystick.gameObject, fieldOfViewUI.gameObject, FreeLookPlateCameraButton.gameObject };
			SetActiveGameObjects(gameObjects, isActive);
			ChangeVisibleFreeLook();
		}

		private void InstantiatePlates()
		{
			if (FrontPlateToggle.isOn)
			{
				_freeLookFrontPlate = UnityEngine.Object.Instantiate(Plates[(_currentPlateIndex - 1) % 2], _vehicleData.PlatesComposition.FrontPlatePoint);
				if ((bool)_vehicleData.PlatesComposition.FrontPlatePos)
				{
					_freeLookFrontPlate.transform.position = _vehicleData.PlatesComposition.FrontPlatePos.position;
				}
				else
				{
					_freeLookFrontPlate.transform.localPosition = Vector3.zero;
				}
				_freeLookFrontPlate.transform.localRotation = Quaternion.identity;
			}
			if (RearPlateToggle.isOn)
			{
				_freeLookRearPlate = UnityEngine.Object.Instantiate(Plates[(_currentPlateIndex - 1) % 2], _vehicleData.PlatesComposition.RearPlatePoint);
				if ((bool)_vehicleData.PlatesComposition.RearPlatePos)
				{
					_freeLookRearPlate.transform.position = _vehicleData.PlatesComposition.RearPlatePos.position;
				}
				else
				{
					_freeLookRearPlate.transform.localPosition = Vector3.zero;
				}
				_freeLookRearPlate.transform.localRotation = Quaternion.identity;
			}
			GameObject[] plates = _vehicleData.PlatesComposition.Plates;
			foreach (GameObject gameObject in plates)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(value: false);
				}
			}
		}

		private void SetCameraOnPlate()
		{
			if (_lastToggledPlate == PlatesType.Front)
			{
				_cameraManager.ActivateCamera(RacingCameraType.VisualTuning, null, null, (CinemachineVirtualCameraBase vc) =>
				{
					vc.GetComponent<VisualTuningCameraHandler>().SetCamMode(BodyKitType.FrontBumper, ignoreTween: false, limiting: false);
				});
			}
			else if (_lastToggledPlate == PlatesType.Rear)
			{
				_cameraManager.ActivateCamera(RacingCameraType.VisualTuning, null, null, (CinemachineVirtualCameraBase vc) =>
				{
					vc.GetComponent<VisualTuningCameraHandler>().SetCamMode(BodyKitType.RearBumper, ignoreTween: false, limiting: false);
				});
			}
			else
			{
				_cameraManager.ActivateCamera(RacingCameraType.VisualTuning, V.Car.transform);
			}
			_currentVinylsType = VinylsType.CarVinyls;
		}

		private void ResetCamera()
		{
			_cameraManager.ActivateCamera(RacingCameraType.VinylBenchmark);
			_currentVinylsType = VinylsType.PlateVinyls;
		}

		private void SetActiveGameObjects(List<GameObject> gameObjects, bool active)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if ((bool)gameObject)
				{
					gameObject.SetActive(active);
				}
			}
		}

		private void CreateVinyl(int order)
		{
			CurrentVinyl = _vinylDrawer.CreateVinyl(_currentVinylsType);
			Transform transform = CurrentVinyl.transform;
			_currentVinylRenderer = CurrentVinyl.GetComponent<SpriteRenderer>();
			_currentTextMesh = transform.GetChild(0).GetComponent<TextMesh>();
			transform.localEulerAngles = Vector3.zero;
			transform.localScale = Vector3.one * 5f;
			_isEditingExisting = false;
			CurrentOperation = VinylsEditorOperation.None;
			_currentSticker = CreateSticker(_allStickers.Count, order);
			_currentSticker.ThisVinyl = transform;
			_editingStickers.Add(_currentSticker);
			SpriteRenderer currentVinylRenderer = _currentVinylRenderer;
			Color color = (_currentTextMesh.color = (_currentSticker.color = _startingVinylColor));
			currentVinylRenderer.color = color;
			UpdateInputFieldsInfo();
			UpdateEditUI(toShow: true);
			UpdateTextToggleInputHint();
		}

		private void PositionCurrentVinyl(Vector3 uvLocalPosition)
		{
			if ((bool)_currentSticker)
			{
				uvLocalPosition.z = (float)_currentSticker.Order * -0.01f;
			}
			if ((bool)CurrentVinyl)
			{
				CurrentVinyl.transform.localPosition = uvLocalPosition;
			}
		}

		private void PositionGroup(Vector3 uvLocalPosition)
		{
			_groupParent.localPosition = uvLocalPosition;
		}

		public void SelectVinylUI(VynilStickerUI vinylUI)
		{
			FavouriteVinylsController.OnSelectItem(vinylUI.Index);
			if (IsGrouping)
			{
				_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.PermittedWhileWorkingWithGroup"), 3f);
				return;
			}
			if ((bool)_selectedUIVinyl)
			{
				_selectedUIVinyl.Select(isOn: false);
			}
			_selectedUIVinyl = vinylUI;
			_navigateNewIndex = vinylUI.transform.GetSiblingIndex();
			if (!_suppressIndexStore)
			{
				_newVinylIndexPerType[_currentVinylsType] = _navigateNewIndex;
			}
			_selectedUIVinyl.Select(isOn: true);
			int index = vinylUI.Index;
			if ((bool)CurrentVinyl)
			{
				IsVinylsChanged = true;
			}
			ChangeCurrentVinylSprite(index);
			_lastVinylIndex = index;
		}

		private void ChangeCurrentVinylSprite(int index)
		{
			if ((bool)CurrentVinyl)
			{
				Color white = Color.white;
				if (index == 1)
				{
					_currentTextMesh.gameObject.SetActive(value: true);
					_currentTextMesh.text = GetInputText();
					ChangeFontIndex(_lastFontIndex);
					fontsInitializer.SetValueWithoutNotify(_lastFontIndex);
					_currentVinylRenderer.enabled = false;
					if (CurrentVinyl.transform.localScale.x < 2f)
					{
						CurrentVinyl.transform.localScale = Vector3.one * 5f;
					}
					PlayerDatas.ItalicOrBold(italicToggle.isOn, boldToggle.isOn, _currentTextMesh);
					white = _currentTextMesh.color;
				}
				else
				{
					_currentVinylRenderer.sprite = V.AllVinyls[index].sprite;
					white = _currentVinylRenderer.color;
					if (!_currentVinylRenderer.enabled)
					{
						Vector3 lastNonTextVinylScale = _lastNonTextVinylScale;
						lastNonTextVinylScale.z = 1f;
						lastNonTextVinylScale = Vector3.ClampMagnitude(lastNonTextVinylScale, 100f);
						CurrentVinyl.transform.localScale = lastNonTextVinylScale;
					}
					_lastNonTextVinylScale = CurrentVinyl.transform.localScale;
					_currentTextMesh.gameObject.SetActive(value: false);
					_currentVinylRenderer.enabled = true;
				}
				Image component = joystickSpriteHolder.GetComponent<Image>();
				component.sprite = V.AllVinyls[index].sprite;
				white.a = 1f;
				component.color = white;
				_currentSticker.stckerIndex = index;
				_currentSticker.isText = index == 1;
				vinylTextUI.SetActive(index == 1 && !IsGrouping);
				UpdateTextToggleInputHint();
				return;
			}
			CurrentOperation = VinylsEditorOperation.Create;
			if ((!_uiNavigationValueEvent || _uiNavigationValueEvent.StoredValue != UiNavigationEnum.Keys) && _indicatorShownCount < 3 && !clickIndicator.activeSelf)
			{
				_coreGameManager.SetMessage(_showMessage, 4f);
				StartBlinkingPointer();
				_disposableStopBlinkingPointer = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
				{
					StopBlinkingPointer();
				}).AddTo(this);
				_indicatorShownCount++;
			}
		}

		public void SelectSticker(StickerItem sticker)
		{
			SelectStickerLate(sticker).Forget();
		}

		private async UniTask SelectStickerLate(StickerItem sticker)
		{
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, _selectionCtx.Token);
			if (Time.frameCount - _lastSavedFrame <= 1)
			{
				return;
			}
			if ((bool)sticker)
			{
				_navigateExistingIndex = _allStickers.IndexOf(sticker);
			}
			ShowVinylsSwitchPanel(isActive: false);
			if (!sticker)
			{
				return;
			}
			if (!IsGrouping)
			{
				IsVinylsChanged = false;
				EditStickerVinyl(sticker);
				if (LockLevel != CameraLock.Full)
				{
					_cameraManager.ChangeTarget(_menuControl.tempTarget);
				}
				VinylsType currentVinylsType = CurrentVinylsType;
				if (currentVinylsType == VinylsType.CarVinyls || currentVinylsType == VinylsType.WindowVinyls)
				{
					_editorUI.editPlane.transform.LookAt(_currentPivotTransform);
					_menuControl.tempTarget.rotation = _editorUI.editPlane.transform.rotation;
					_vinylCameraHandler.XAxisValue = 0f;
				}
				_editingStickers.Add(_currentSticker);
				CurrentState = VinylsEditorInputState.OneSelected;
				return;
			}
			if (_editingStickers.Count == 0)
			{
				EditStickerVinyl(sticker);
			}
			if (_editingStickers.Contains(sticker))
			{
				if (sticker.oldIndex == -1 || sticker == _currentSticker)
				{
					return;
				}
				RemoveFromGroup(sticker);
				UnselectSticker(sticker);
			}
			else
			{
				AddToGroup(sticker);
			}
			_selectionCtx.Dispose();
			_selectionCtx = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, base.destroyCancellationToken);
		}

		private void UnselectSticker(StickerItem sticker)
		{
			if (!sticker)
			{
				return;
			}
			ObscuredInt price = V.AllVinyls[sticker.oldIndex].price;
			if ((int)V.AllVinyls[sticker.stckerIndex].price > (int)price)
			{
				RollBackVinyl(sticker);
				return;
			}
			int stckerIndex = sticker.stckerIndex;
			SpriteRenderer component = sticker.ThisVinyl.GetComponent<SpriteRenderer>();
			TextMesh component2 = sticker.ThisVinyl.GetChild(0).GetComponent<TextMesh>();
			sticker.isText = stckerIndex == 1;
			sticker.sprite.sprite = V.AllVinyls[stckerIndex].sprite;
			sticker.positionIcon = _currentPivotTransform.InverseTransformPoint(_editorUI.editPlane.transform.position);
			sticker.scaleQuadEuler = _scaleQuad.transform.eulerAngles;
			sticker.xAngle = _carCameras.xAngle;
			sticker.yAngle = _carCameras.yAngle;
			if (stckerIndex == 1)
			{
				component2.gameObject.SetActive(value: true);
				component.enabled = false;
			}
			else
			{
				component.sprite = V.AllVinyls[stckerIndex].sprite;
				component2.gameObject.SetActive(value: false);
				component.enabled = true;
			}
			SaveOneVinyl(sticker);
			if (CurrentState == VinylsEditorInputState.OneSelected)
			{
				SetEditorStateWithCurrentRow();
			}
		}

		private void RollBackVinyl(StickerItem sticker, bool save = true)
		{
			sticker.stckerIndex = sticker.oldIndex;
			sticker.color = sticker.oldColor;
			VinylData vinylDataFromSticker = VinylsUtility.GetVinylDataFromSticker(sticker);
			_vinylsDataUtility.ApplyVinylData(vinylDataFromSticker, sticker.ThisVinyl.gameObject);
			if (save)
			{
				SaveOneVinyl(sticker);
			}
		}

		private void RollBackAllChanges()
		{
			if (_editingStickers.Count == 0)
			{
				return;
			}
			if (_editingStickers.Any((StickerItem sticker) => sticker.oldIndex == -1))
			{
				_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.SaveDeleteCurrentVinylS"), 3f);
				return;
			}
			IsVinylsChanged = false;
			foreach (StickerItem editingSticker in _editingStickers)
			{
				if (IsGrouping)
				{
					UnchildFromGroup(editingSticker);
				}
				RollBackVinyl(editingSticker, save: false);
			}
			if (_editingStickers[0].positionIcon != Vector3.zero)
			{
				_editorUI.CreateEditIcons(_currentPivotTransform.TransformPoint(_editingStickers[0].positionIcon), CurrentVinyl);
			}
			else
			{
				_editorUI.CreateEditIcons(Vector3.zero, CurrentVinyl);
				_editorUI.UpdateAllEditIcons(isEnabled: false);
			}
			if (IsGrouping)
			{
				Vector3 localPosition = _currentSticker.ThisVinyl.localPosition;
				localPosition.z = 0f;
				_groupParent.localPosition = localPosition;
				_groupParent.localRotation = Quaternion.identity;
				_groupParent.localScale = Vector3.one;
			}
			else
			{
				if (_currentSticker.isText)
				{
					vinylTextUI.SetActive(value: true);
					inputText.text = _currentSticker.text;
					fontsInitializer.SetValueWithoutNotify(_currentSticker.fontIndex);
					italicToggle.SetIsOnWithoutNotify(_currentSticker.italic);
					boldToggle.SetIsOnWithoutNotify(_currentSticker.bold);
					_lastFontIndex = _currentSticker.fontIndex;
					fontsInitializer.UpdateDropdownItemFonts(_lastFontIndex);
				}
				colorPicker.SetColor(_currentSticker.color, withAction: false);
				alphaSlider.SetValueWithoutNotify(_currentSticker.color.a);
			}
			foreach (StickerItem editingSticker2 in _editingStickers)
			{
				if (IsGrouping)
				{
					ChildToGroup(editingSticker2);
				}
			}
		}

		private void AddToGroup(StickerItem sticker, bool resetTransform = true)
		{
			AddToGroupLate(sticker, resetTransform).Forget();
		}

		private async UniTask AddToGroupLate(StickerItem sticker, bool resetTransform = true)
		{
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, _selectionCtx.Token);
			if ((_groupParent.childCount == 0) & resetTransform)
			{
				Vector3 localPosition = sticker.ThisVinyl.localPosition;
				localPosition.z = 0f;
				_groupParent.localPosition = localPosition;
				_groupParent.localRotation = Quaternion.identity;
				_groupParent.localScale = Vector3.one;
			}
			sticker.backround.color = ((_editingStickers.Count == 0) ? Color.yellow : Color.magenta);
			sticker.checkMark.SetActive(value: true);
			_editingStickers.Add(sticker);
			_editingStickers = _editingStickers.OrderBy((StickerItem item) => item.Order).ToList();
			StartBlinking(sticker.ThisVinyl.gameObject);
			ChildToGroup(sticker);
			_isMerged = false;
			_selectionCtx.Dispose();
			_selectionCtx = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, base.destroyCancellationToken);
		}

		private void ClearGroup()
		{
			foreach (StickerItem editingSticker in _editingStickers)
			{
				editingSticker.backround.color = editingSticker.InitialColorBackround;
				editingSticker.checkMark.SetActive(value: false);
				UnchildFromGroup(editingSticker);
			}
			_editingStickers.Clear();
		}

		private void RemoveFromGroup(StickerItem sticker)
		{
			if (!(sticker == _currentSticker))
			{
				sticker.backround.color = sticker.InitialColorBackround;
				sticker.checkMark.SetActive(value: false);
				UnchildFromGroup(sticker);
				_editingStickers.Remove(sticker);
				_isMerged = false;
			}
		}

		private void UnchildFromGroup(StickerItem sticker)
		{
			sticker.ThisVinyl.SetParent(_vinylDrawer.GetVinylsParent(_currentVinylsType));
		}

		private void ChildToGroup(StickerItem sticker)
		{
			sticker.ThisVinyl.SetParent(_groupParent);
		}

		private void EditStickerVinyl(StickerItem sticker)
		{
			if (!sticker || !sticker.ThisVinyl)
			{
				return;
			}
			if ((bool)_currentSticker && _currentSticker != sticker)
			{
				if (_currentSticker.oldIndex == -1)
				{
					CancelOrDelete();
				}
				else
				{
					UnselectSticker(_currentSticker);
				}
			}
			_editingStickers.Clear();
			CurrentVinyl = sticker.ThisVinyl.gameObject;
			UpdateInputFieldsInfo();
			_currentVinylRenderer = CurrentVinyl.GetComponent<SpriteRenderer>();
			_currentTextMesh = CurrentVinyl.transform.GetChild(0).GetComponent<TextMesh>();
			if (sticker.positionIcon != Vector3.zero)
			{
				_editorUI.CreateEditIcons(_currentPivotTransform.TransformPoint(sticker.positionIcon), CurrentVinyl);
			}
			else
			{
				_editorUI.CreateEditIcons(Vector3.zero, CurrentVinyl);
				_editorUI.UpdateAllEditIcons(isEnabled: false);
			}
			_isEditingExisting = true;
			if ((bool)_currentSticker)
			{
				_currentSticker.backround.color = _currentSticker.InitialColorBackround;
			}
			_currentSticker = sticker;
			sticker.backround.color = Color.green;
			UpdateTextToggleInputHint();
			if (joystickSpriteHolder.TryGetComponent<Image>(out var component))
			{
				component.sprite = sticker.sprite.sprite;
				Color color = _currentVinylRenderer.color;
				color.a = 1f;
				component.color = color;
			}
			colorPicker.SetColor(_currentSticker.color, withAction: false);
			alphaSlider.SetValueWithoutNotify(_currentSticker.color.a);
			_startingVinylColor = _currentSticker.color;
			StartBlinking(sticker.ThisVinyl.gameObject);
			if (sticker.isText)
			{
				vinylTextUI.SetActive(value: true);
				inputText.text = sticker.text;
				fontsInitializer.SetValueWithoutNotify(sticker.fontIndex);
				italicToggle.SetIsOnWithoutNotify(sticker.italic);
				boldToggle.SetIsOnWithoutNotify(sticker.bold);
				_lastFontIndex = sticker.fontIndex;
				fontsInitializer.UpdateDropdownItemFonts(_lastFontIndex);
			}
			else
			{
				vinylTextUI.SetActive(value: false);
			}
			if (sticker.xAngle != 0f)
			{
				_carCameras.xAngle = sticker.xAngle;
			}
			if (sticker.yAngle != 0f)
			{
				_carCameras.yAngle = sticker.yAngle;
			}
			_scaleQuad.transform.eulerAngles = sticker.scaleQuadEuler;
			CurrentOperation = VinylsEditorOperation.None;
			_editorUI.UpdateAllEditIcons(isEnabled: true);
			UpdateEditUI(toShow: true);
		}

		public void OnColorChanged(Color color)
		{
			ChangeColor(color);
		}

		public void OnAlphaChanged(float alpha)
		{
			Color white = Color.white;
			white.a = alpha;
			ChangeColor(white, onlyAlpha: true);
		}

		public void OnHexChanged(string hex)
		{
			colorPicker.SetColor(hex);
		}

		public void ChangeColor(Color color, bool onlyAlpha = false)
		{
			if ((bool)_currentVinylRenderer && !IsGrouping)
			{
				Color startingVinylColor;
				if (onlyAlpha)
				{
					startingVinylColor = (_currentVinylRenderer.enabled ? _currentVinylRenderer.color : _currentTextMesh.color);
					startingVinylColor.a = color.a;
				}
				else
				{
					startingVinylColor = color;
					startingVinylColor.a = _startingVinylColor.a;
				}
				SpriteRenderer currentVinylRenderer = _currentVinylRenderer;
				Color color2 = (_currentTextMesh.color = (_currentSticker.color = (_startingVinylColor = startingVinylColor)));
				currentVinylRenderer.color = color2;
				IsVinylsChanged = true;
			}
		}

		public (GameObject, GameObject) InstantiateEditIcons()
		{
			return (UnityEngine.Object.Instantiate(planePrefab), UnityEngine.Object.Instantiate(visualIconsPrefab));
		}

		private void UpdateCurrentVinyl()
		{
			if (_isFreeLook)
			{
				return;
			}
			if (!_suppressCameraFollow)
			{
				if ((bool)_editorUI.editPlane && _editorUI.editPlane.activeSelf)
				{
					_editPlaneLastPos = _editorUI.editPlane.transform.position;
					_menuControl.tempTarget.position = Vector3.Lerp(_menuControl.tempTarget.position, _editPlaneLastPos, Time.deltaTime * 3f);
				}
				else if (_editPlaneLastPos != Vector3.zero)
				{
					_menuControl.tempTarget.position = Vector3.Lerp(_menuControl.tempTarget.position, _editPlaneLastPos, Time.deltaTime * 3f);
				}
				else
				{
					_menuControl.tempTarget.position = Vector3.Lerp(_menuControl.tempTarget.position, (_currentVinylsType == VinylsType.PlateVinyls) ? _plateCreatorTableTr.position : _newFocusPos, Time.deltaTime * 3f);
				}
			}
			if (_isPointerDown && !_isDragging)
			{
				Ray ray = _mainCam.ScreenPointToRay(GetPointerScreenPosition());
				int layerMask = 524288;
				if (Physics.Raycast(ray, out var hitInfo, _carCameras.distance + 3f, layerMask, QueryTriggerInteraction.Collide) && hitInfo.collider.CompareTag("Draggable"))
				{
					_cameraManager.ChangeTarget((LockLevel >= CameraLock.Edit) ? null : _menuControl.tempTarget);
					_draggingEditCollider = hitInfo.transform;
					_isDragging = true;
					if (_draggingEditCollider == _editorUI.posCollider)
					{
						CurrentOperation = VinylsEditorOperation.Move;
					}
					else if (_draggingEditCollider == _editorUI.rotCollider)
					{
						CurrentOperation = VinylsEditorOperation.Rotate;
					}
					else if (_draggingEditCollider == _editorUI.scaleCollider)
					{
						CurrentOperation = VinylsEditorOperation.Scale;
						_oldScalePos = Vector3.one;
					}
					if ((bool)_editorUI.visualIcons)
					{
						_editorUI.HideSomeEditIcons(CurrentOperation);
					}
				}
			}
			if (_isPointerDown && _isDragging && CurrentOperation != VinylsEditorOperation.Create)
			{
				Vector3 pointerScreenPosition = GetPointerScreenPosition();
				Ray ray2 = _mainCam.ScreenPointToRay(pointerScreenPosition);
				int layerMask2 = 262144;
				if (Physics.Raycast(ray2, out var hitInfo2, _carCameras.distance + 3f, layerMask2))
				{
					_draggingEditCollider.transform.position = hitInfo2.point;
				}
				switch (CurrentOperation)
				{
				case VinylsEditorOperation.Rotate:
					Rotating();
					break;
				case VinylsEditorOperation.Scale:
					Scaling();
					break;
				case VinylsEditorOperation.Move:
				{
					Vector3 uvLocalPosition = Vector3.zero;
					if (!_currentSticker && !_editingStickers.IsNullOrEmpty())
					{
						_currentSticker = _editingStickers[0];
					}
					if (HitTestUVPosition(ref uvLocalPosition, out var hit))
					{
						MoveVinyl(uvLocalPosition, hit.point);
					}
					break;
				}
				}
			}
			if ((bool)_editorUI.editPlane && (bool)_editorUI.visualIcons)
			{
				Vector3 position = _editorUI.editPlane.transform.position;
				_editorUI.visualIcons.transform.position = position;
				_scaleQuad.transform.position = position;
			}
			else if ((bool)_editorUI.editLine1 && (bool)_editorUI.editLine2)
			{
				_editorUI.editLine1.gameObject.SetActive(value: false);
				_editorUI.editLine2.gameObject.SetActive(value: false);
			}
			if ((bool)_sideCheckParent && (bool)V.Car)
			{
				_sideCheckParent.transform.position = V.Car.transform.position;
				_sideCheckParent.transform.eulerAngles = V.Car.transform.eulerAngles + new Vector3(0f, 90f, 0f);
			}
		}

		public void SaveVinyl()
		{
			int count = _editingStickers.Count;
			StopBlinking();
			int num = 0;
			foreach (StickerItem editingSticker in _editingStickers)
			{
				if (editingSticker.oldIndex == -1)
				{
					num += (int)V.AllVinyls[editingSticker.stckerIndex].price;
					continue;
				}
				int num2 = (int)V.AllVinyls[editingSticker.oldIndex].price - (int)V.AllVinyls[editingSticker.stckerIndex].price;
				if (num2 > 0)
				{
					num += num2;
				}
			}
			if ((int)CurrencyManagerController.Money < num)
			{
				_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.NotEnoughMoney"), 3f);
				_saved = false;
				return;
			}
			if (IsGrouping)
			{
				float orthographicSize = _vinylDrawer.Camera.orthographicSize;
				Vector2[] array = new Vector2[count];
				Vector2 vector = default;
				for (int i = 0; i < count; i++)
				{
					StickerItem stickerItem = _editingStickers[i];
					UnchildFromGroup(stickerItem);
					Vector3 localPosition = stickerItem.ThisVinyl.transform.localPosition;
					vector.x = localPosition.x + orthographicSize;
					vector.y = localPosition.y + orthographicSize;
					array[i] = vector;
				}
				MeshOperators.TrianglePosNorm[] array2 = MeshOperators.UvTo3D(array, _meshColliders);
				for (int j = 0; j < count; j++)
				{
					if (array2[j].position != Vector3.zero)
					{
						_editingStickers[j].positionIcon = _currentPivotTransform.InverseTransformPoint(array2[j].position);
						Vector3 vector2 = CheckSide(array2[j].position, array2[j].normal);
						if (vector2 != Vector3.zero)
						{
							_editingStickers[j].scaleQuadEuler = vector2;
						}
					}
					else
					{
						_editingStickers[j].positionIcon = _currentPivotTransform.InverseTransformPoint(_editorUI.editPlane.transform.position);
						_editingStickers[j].scaleQuadEuler = _scaleQuad.transform.eulerAngles;
					}
				}
			}
			foreach (StickerItem editingSticker2 in _editingStickers)
			{
				editingSticker2.backround.color = editingSticker2.InitialColorBackround;
				editingSticker2.checkMark.SetActive(value: false);
				editingSticker2.xAngle = _carCameras.xAngle;
				editingSticker2.yAngle = _carCameras.yAngle;
				if (!IsGrouping && (bool)_editorUI.editPlane)
				{
					editingSticker2.positionIcon = _currentPivotTransform.InverseTransformPoint(_editorUI.editPlane.transform.position);
					editingSticker2.scaleQuadEuler = _scaleQuad.transform.eulerAngles;
				}
				SaveOneVinyl(editingSticker2);
			}
			EasySaveDataHandler.SaveFile(_currentVinylsPath);
			if (count > 0)
			{
				_steamAchievementService?.Unlock("ACH_VINYL_SIGNATURE");
			}
			if (count > 0)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQDw", "vinyl_editor");
			}
			if ((bool)_currentSticker)
			{
				_editorUI.AnimateSticker(V.AllVinyls[_currentSticker.stckerIndex].sprite, _currentSticker.sprite.color, save: true);
			}
			CurrentVinyl = null;
			_currentVinylRenderer = null;
			_currentTextMesh = null;
			_lastSavedFrame = Time.frameCount;
			_currentSticker = null;
			UpdateEditUI(toShow: false);
			_editorUI.UpdateAllEditIcons(isEnabled: false);
			IsVinylsChanged = false;
			CurrentOperation = (_selectedUIVinyl ? VinylsEditorOperation.Create : VinylsEditorOperation.None);
			FocusCamOnInitTarget();
			UpdateTextToggleInputHint();
			int count2 = _allStickers.Count;
			UpdateStickersScroll(count2, _isEditingExisting ? stickersScrollBar.value : 0f);
			if (_currentVinylsType != VinylsType.PlateVinyls)
			{
				SetCamFOV(50f);
			}
			_editingStickers.Clear();
			if (IsGrouping)
			{
				ChangeGroupingState();
			}
			_lastVinylOrder = count2 - 1;
			_saved = true;
			if (count2 > 0)
			{
				_navigateExistingIndex = count2 - 1;
			}
			SetEditorStateWithCurrentRow();
		}

		private void SaveOneVinyl(StickerItem sticker)
		{
			if (sticker.Order != -1)
			{
				int num = 0;
				if (sticker.oldIndex == -1)
				{
					num += (int)V.AllVinyls[sticker.stckerIndex].price;
				}
				else
				{
					int num2 = (int)V.AllVinyls[sticker.stckerIndex].price - (int)V.AllVinyls[sticker.oldIndex].price;
					if (num2 > 0)
					{
						num += num2;
					}
				}
				AdvancedInt price = new AdvancedInt(num);
				_currencyManagerController.TryMoneySpend(out var _, price, () =>
				{
					VinylData value = VinylsUtility.ParseVinylToVinylData(sticker, resetBackColor: true);
					if (_cachedVinylsFile != null)
					{
						EasySaveDataHandler.SaveValue(value, _cachedVinylsFile, sticker.Order.ToString());
					}
					else
					{
						Debug.LogError("Can't find save file");
					}
					_currentVinylsCollection[sticker.Order] = sticker.ThisVinyl.gameObject;
				});
			}
			else
			{
				_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.VinylIsFull"), 3f);
			}
		}

		private void DeleteAllStickers()
		{
			CancelOrDelete();
			for (int i = 0; i < _allStickers.Count; i++)
			{
				DeleteOneSticker(_allStickers[i]);
				UnityEngine.Object.Destroy(_allStickers[i].gameObject);
			}
			_allStickers.Clear();
			DeleteStickers();
			UpdateStickers();
			UpdateStickersScroll(_allStickers.Count);
			UpdateDeleteAllStickersButtonVisible();
		}

		private void DeleteOneSticker(StickerItem sticker, bool destroy = true)
		{
			EasySaveDataHandler.DeleteValue(_currentVinylsPath, sticker.Order.ToString());
			if (destroy)
			{
				UnityEngine.Object.Destroy(sticker.ThisVinyl.gameObject);
			}
			if (sticker.Order < _currentVinylsCollection.Length)
			{
				_currentVinylsCollection[sticker.Order] = null;
			}
		}

		public void CancelOrDelete()
		{
			StopBlinking();
			if (_editingStickers.Count == 0)
			{
				return;
			}
			_editorUI.AnimateSticker(V.AllVinyls[_currentSticker.stckerIndex].sprite, (_currentSticker.stckerIndex != 1) ? _currentVinylRenderer.color : _currentTextMesh.color, save: false);
			int order = _editingStickers[0].Order;
			foreach (StickerItem editingSticker in _editingStickers)
			{
				_allStickers.Remove(editingSticker);
				DeleteOneSticker(editingSticker);
				UnityEngine.Object.Destroy(editingSticker.gameObject);
			}
			_editingStickers.Clear();
			if (IsGrouping)
			{
				ChangeGroupingState();
			}
			if (_allStickers.Count > order)
			{
				int num = order;
				for (int i = num; i < _allStickers.Count; i++)
				{
					StickerItem stickerItem = _allStickers[i];
					DeleteOneSticker(stickerItem, destroy: false);
					stickerItem.Order = num++;
					Vector3 localPosition = stickerItem.ThisVinyl.localPosition;
					localPosition.z = (float)stickerItem.Order * -0.01f;
					stickerItem.ThisVinyl.localPosition = localPosition;
					stickerItem.transform.localPosition = new Vector3(0f, stickerItem.Order * -130, 0f);
					SaveOneVinyl(stickerItem);
				}
			}
			UpdateStickersScroll(_allStickers.Count, stickersScrollBar.value);
			EasySaveDataHandler.SaveFile(_currentVinylsPath);
			_currentVinylRenderer = null;
			_currentTextMesh = null;
			CurrentVinyl = null;
			_currentSticker = null;
			FocusCamOnInitTarget();
			_editorUI.UpdateAllEditIcons(isEnabled: false);
			IsVinylsChanged = false;
			UpdateEditUI(toShow: false);
			UpdateTextToggleInputHint();
			CurrentOperation = (_selectedUIVinyl ? VinylsEditorOperation.Create : VinylsEditorOperation.None);
			if (_currentVinylsType != VinylsType.PlateVinyls)
			{
				SetCamFOV(50f);
			}
			_lastVinylOrder = _allStickers.Count - 1;
			UpdateDeleteAllStickersButtonVisible();
			SetEditorStateWithCurrentRow();
		}

		public void CopyVinyls()
		{
			if (_editingStickers.Count == 0)
			{
				return;
			}
			if (_editingStickers.Count > _maxVinylsCount - _allStickers.Count)
			{
				_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.NotEnoughSpaceForNewVinylS"), 3f);
				return;
			}
			StopBlinking();
			StickerItem[] array = new StickerItem[_editingStickers.Count];
			StickerItem currentSticker = _currentSticker;
			_editingStickers.CopyTo(array);
			VinylsEditorInputState currentState = CurrentState;
			SaveVinyl();
			if (!_saved)
			{
				return;
			}
			if (array.Length > 1)
			{
				ChangeGroupingState();
			}
			int num = 0;
			Color color = Color.white;
			int index = 0;
			StickerItem[] array2 = array;
			foreach (StickerItem stickerItem in array2)
			{
				Transform thisVinyl = stickerItem.ThisVinyl;
				SpriteRenderer component = thisVinyl.GetComponent<SpriteRenderer>();
				TextMesh component2 = thisVinyl.transform.GetChild(0).GetComponent<TextMesh>();
				CreateVinyl(_lastVinylOrder + 1 + num++);
				ChangeCurrentVinylSprite(stickerItem.stckerIndex);
				bool flag = stickerItem.stckerIndex == 1;
				color = (flag ? component2.color : component.color);
				_currentVinylRenderer.enabled = !flag;
				_currentVinylRenderer.color = color;
				_currentVinylRenderer.flipX = component.flipX;
				_currentTextMesh.gameObject.SetActive(flag);
				_currentTextMesh.text = component2.text;
				_currentTextMesh.fontStyle = component2.fontStyle;
				_currentTextMesh.font = component2.font;
				_currentTextMesh.GetComponent<Renderer>().material = component2.GetComponent<Renderer>().material;
				_currentTextMesh.color = color;
				CurrentVinyl.transform.rotation = thisVinyl.rotation;
				CurrentVinyl.transform.localScale = thisVinyl.localScale;
				_currentSticker.sprite.sprite = stickerItem.sprite.sprite;
				_currentSticker.sprite.color = stickerItem.sprite.color;
				if (stickerItem == currentSticker)
				{
					index = _editingStickers.IndexOf(_currentSticker);
				}
				PositionCurrentVinyl(thisVinyl.transform.localPosition);
				if (IsGrouping)
				{
					ChildToGroup(_currentSticker);
					_isMerged = true;
					_currentSticker.backround.color = Color.magenta;
					_currentSticker.checkMark.SetActive(value: true);
				}
			}
			if (IsGrouping)
			{
				_currentSticker = _editingStickers[index];
				CurrentVinyl = _currentSticker.ThisVinyl.gameObject;
				_currentVinylRenderer = _currentSticker.ThisVinyl.GetComponent<SpriteRenderer>();
				_currentTextMesh = _currentSticker.ThisVinyl.GetChild(0).GetComponent<TextMesh>();
				_currentSticker.backround.color = Color.yellow;
				UpdateTextToggleInputHint();
				_currentSticker.checkMark.SetActive(value: true);
			}
			else
			{
				_currentSticker.backround.color = Color.green;
				_currentSticker.checkMark.SetActive(value: false);
			}
			colorPicker.SetColor(color, withAction: false);
			alphaSlider.SetValueWithoutNotify(color.a);
			_editorUI.editPlane.SetActive(value: true);
			_editorUI.UpdateAllEditIcons(isEnabled: true);
			IsVinylsChanged = false;
			int num2 = Mathf.Max(1, _allStickers.Count - 1);
			UpdateStickersScroll(_allStickers.Count, 1f - (float)_currentSticker.Order / (float)num2);
			Observable.TimerFrame(2).Subscribe((long _) =>
			{
				stickersScrollBar.value = 0f;
			}).AddTo(this);
			CurrentState = currentState;
		}

		public void BackZ()
		{
			if (_editingStickers.Count == 0)
			{
				return;
			}
			if (_editingStickers.Count == 1)
			{
				StickerItem stickerItem = _editingStickers[0];
				if (stickerItem.Order != 0)
				{
					stickerItem.Order--;
					if (IsGrouping)
					{
						UnchildFromGroup(stickerItem);
					}
					if ((bool)_editorUI.editPlane)
					{
						stickerItem.positionIcon = _currentPivotTransform.InverseTransformPoint(_editorUI.editPlane.transform.position);
					}
					ReorderSticker(stickerItem);
					if (IsGrouping)
					{
						ChildToGroup(stickerItem);
					}
					StickerItem stickerItem2 = _allStickers[stickerItem.Order];
					stickerItem2.Order++;
					ReorderSticker(stickerItem2);
					_allStickers[stickerItem.Order] = stickerItem;
					_allStickers[stickerItem2.Order] = stickerItem2;
					stickerItem.backround.color = (IsGrouping ? Color.yellow : Color.green);
					stickerItem.checkMark.SetActive(IsGrouping);
					int num = Mathf.Max(1, _allStickers.Count - 1);
					UpdateStickersScroll(_allStickers.Count, (float)stickerItem.Order / (float)num, ScrollUpdateDirection.Down);
					IsVinylsChanged = true;
				}
				return;
			}
			StickerItem stickerItem3 = _editingStickers[_editingStickers.Count - 1];
			if (_isMerged)
			{
				if (_editingStickers[0].Order == 0)
				{
					return;
				}
				StickerItem stickerItem4 = _allStickers[_editingStickers[0].Order - 1];
				foreach (StickerItem editingSticker in _editingStickers)
				{
					editingSticker.Order--;
					UnchildFromGroup(editingSticker);
					ReorderSticker(editingSticker);
					ChildToGroup(editingSticker);
					_allStickers[editingSticker.Order] = editingSticker;
					editingSticker.backround.color = ((editingSticker == _currentSticker) ? Color.yellow : Color.magenta);
					editingSticker.checkMark.SetActive(value: true);
				}
				stickerItem4.Order = stickerItem3.Order + 1;
				ReorderSticker(stickerItem4);
				_allStickers[stickerItem4.Order] = stickerItem4;
				int num2 = Mathf.Max(1, _allStickers.Count - 1);
				UpdateStickersScroll(_allStickers.Count, (float)stickerItem4.Order / (float)num2, ScrollUpdateDirection.Down);
				return;
			}
			int order = _editingStickers[0].Order;
			int num3 = _editingStickers.IndexOf(_currentSticker);
			int num4 = _currentSticker.Order;
			for (int num5 = num3; num5 >= 0; num5--)
			{
				num4 = (_editingStickers[num5].Order = num4 - 1);
			}
			num4 = _currentSticker.Order;
			for (int i = num3 + 1; i < _editingStickers.Count; i++)
			{
				num4 = (_editingStickers[i].Order = num4 + 1);
			}
			_isMerged = true;
			StickerItem[] array = new StickerItem[_allStickers.Count];
			bool flag = _editingStickers[0].Order < 0;
			foreach (StickerItem editingSticker2 in _editingStickers)
			{
				if (flag)
				{
					editingSticker2.Order++;
				}
				UnchildFromGroup(editingSticker2);
				ReorderSticker(editingSticker2);
				ChildToGroup(editingSticker2);
				_allStickers.Remove(editingSticker2);
				array[editingSticker2.Order] = editingSticker2;
				editingSticker2.backround.color = ((editingSticker2 == _currentSticker) ? Color.yellow : Color.magenta);
				editingSticker2.checkMark.SetActive(value: true);
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (!(array[j] != null))
				{
					StickerItem stickerItem5 = _allStickers[0];
					if (stickerItem5.Order != j)
					{
						stickerItem5.Order = j;
						ReorderSticker(stickerItem5);
					}
					array[j] = stickerItem5;
					_allStickers.Remove(stickerItem5);
				}
			}
			_allStickers = array.ToList();
			int num6 = Mathf.Max(1, _allStickers.Count - 1);
			UpdateStickersScroll(_allStickers.Count, (float)order / (float)num6, ScrollUpdateDirection.Down);
			IsVinylsChanged = true;
		}

		public void ForwardZ()
		{
			if (_editingStickers.Count == 0)
			{
				return;
			}
			if (_editingStickers.Count == 1)
			{
				StickerItem stickerItem = _editingStickers[0];
				if (stickerItem.Order != _allStickers.Count - 1)
				{
					stickerItem.Order++;
					if (IsGrouping)
					{
						UnchildFromGroup(stickerItem);
					}
					if ((bool)_editorUI.editPlane)
					{
						stickerItem.positionIcon = _currentPivotTransform.InverseTransformPoint(_editorUI.editPlane.transform.position);
					}
					ReorderSticker(stickerItem);
					if (IsGrouping)
					{
						ChildToGroup(stickerItem);
					}
					StickerItem stickerItem2 = _allStickers[stickerItem.Order];
					stickerItem2.Order--;
					ReorderSticker(stickerItem2);
					_allStickers[stickerItem.Order] = stickerItem;
					_allStickers[stickerItem2.Order] = stickerItem2;
					stickerItem.backround.color = (IsGrouping ? Color.yellow : Color.green);
					stickerItem.checkMark.SetActive(IsGrouping);
					int num = Mathf.Max(1, _allStickers.Count - 1);
					UpdateStickersScroll(_allStickers.Count, (float)stickerItem.Order / (float)num, ScrollUpdateDirection.Up);
					IsVinylsChanged = true;
				}
				return;
			}
			StickerItem stickerItem3 = _editingStickers[_editingStickers.Count - 1];
			if (_isMerged)
			{
				if (stickerItem3.Order == _allStickers.Count - 1)
				{
					return;
				}
				StickerItem stickerItem4 = _allStickers[stickerItem3.Order + 1];
				foreach (StickerItem editingSticker in _editingStickers)
				{
					editingSticker.Order++;
					UnchildFromGroup(editingSticker);
					ReorderSticker(editingSticker);
					ChildToGroup(editingSticker);
					_allStickers[editingSticker.Order] = editingSticker;
					editingSticker.backround.color = ((editingSticker == _currentSticker) ? Color.yellow : Color.magenta);
					editingSticker.checkMark.SetActive(value: true);
				}
				stickerItem4.Order = _editingStickers[0].Order - 1;
				ReorderSticker(stickerItem4);
				_allStickers[stickerItem4.Order] = stickerItem4;
				int num2 = Mathf.Max(1, _allStickers.Count - 1);
				UpdateStickersScroll(_allStickers.Count, (float)stickerItem4.Order / (float)num2, ScrollUpdateDirection.Up);
				return;
			}
			int order = _editingStickers[0].Order;
			int num3 = _editingStickers.IndexOf(_currentSticker);
			int num4 = _currentSticker.Order;
			for (int i = num3; i < _editingStickers.Count; i++)
			{
				num4 = (_editingStickers[i].Order = num4 + 1);
			}
			num4 = _currentSticker.Order;
			for (int num5 = num3 - 1; num5 >= 0; num5--)
			{
				num4 = (_editingStickers[num5].Order = num4 - 1);
			}
			_isMerged = true;
			StickerItem[] array = new StickerItem[_allStickers.Count];
			bool flag = stickerItem3.Order > _allStickers.Count - 1;
			foreach (StickerItem editingSticker2 in _editingStickers)
			{
				if (flag)
				{
					editingSticker2.Order--;
				}
				UnchildFromGroup(editingSticker2);
				ReorderSticker(editingSticker2);
				ChildToGroup(editingSticker2);
				_allStickers.Remove(editingSticker2);
				array[editingSticker2.Order] = editingSticker2;
				editingSticker2.backround.color = ((editingSticker2 == _currentSticker) ? Color.yellow : Color.magenta);
				editingSticker2.checkMark.SetActive(value: true);
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (!(array[j] != null))
				{
					StickerItem stickerItem5 = _allStickers[0];
					if (stickerItem5.Order != j)
					{
						stickerItem5.Order = j;
						ReorderSticker(stickerItem5);
					}
					array[j] = stickerItem5;
					_allStickers.Remove(stickerItem5);
				}
			}
			_allStickers = array.ToList();
			int num6 = Mathf.Max(1, _allStickers.Count - 1);
			UpdateStickersScroll(_allStickers.Count, (float)order / (float)num6, ScrollUpdateDirection.Up);
			IsVinylsChanged = true;
		}

		private void ReorderSticker(StickerItem sticker)
		{
			Vector3 localPosition = sticker.ThisVinyl.localPosition;
			localPosition.z = (float)sticker.Order * -0.01f;
			sticker.ThisVinyl.localPosition = localPosition;
			sticker.position = localPosition;
			sticker.transform.SetSiblingIndex(sticker.Order);
			sticker.UpdateOrder();
			if (sticker.oldIndex != -1)
			{
				SaveOneVinyl(sticker);
			}
		}

		public void FlipX()
		{
			if (IsGrouping)
			{
				Transform obj = _groupParent.transform;
				Vector3 localScale = obj.localScale;
				localScale.x = 0f - localScale.x;
				GroupXSign = Math.Sign(localScale.x);
				obj.localScale = localScale;
			}
			else if ((bool)CurrentVinyl)
			{
				SpriteRenderer component = CurrentVinyl.GetComponent<SpriteRenderer>();
				component.flipX = !component.flipX;
				if ((bool)_currentTextMesh)
				{
					Vector3 localScale2 = _currentTextMesh.gameObject.transform.localScale;
					localScale2.x *= -1f;
					_currentTextMesh.gameObject.transform.localScale = localScale2;
				}
			}
			IsVinylsChanged = true;
		}

		public void OnTextEntered(string text)
		{
			if ((bool)_currentTextMesh)
			{
				_currentTextMesh.text = GetInputText();
			}
			IsVinylsChanged = true;
		}

		private string GetInputText()
		{
			if (!string.IsNullOrEmpty(inputText.text))
			{
				return inputText.text;
			}
			return _placeholderText.text;
		}

		public void SetFontStyle()
		{
			if ((bool)_currentTextMesh)
			{
				PlayerDatas.ItalicOrBold(italicToggle.isOn, boldToggle.isOn, _currentTextMesh);
			}
			IsVinylsChanged = true;
		}

		public void ChangeFontIndex(int fontIndex)
		{
			_lastFontIndex = fontIndex;
			if ((bool)_currentTextMesh)
			{
				Font font = V.Fonts[fontIndex];
				if ((bool)font)
				{
					_currentTextMesh.font = font;
					_currentTextMesh.GetComponent<Renderer>().sharedMaterial = V.FontMaterials[fontIndex];
				}
			}
			IsVinylsChanged = true;
		}

		public void OnGroupButtonPressed()
		{
			ToggleGroupingVinyls();
		}

		private bool ChangeGroupingState(bool checkCount = true)
		{
			if (checkCount && _editingStickers.Count > 0)
			{
				_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.SaveDeleteCurrentVinylS"), 2f);
				return false;
			}
			IsGrouping = !IsGrouping;
			ColorBlock colors = groupingButton.colors;
			Color color = (colors.selectedColor = (IsGrouping ? Color.magenta : Color.white));
			Color normalColor = (colors.pressedColor = color);
			colors.normalColor = normalColor;
			groupingButton.colors = colors;
			GroupXSign = 1;
			return true;
		}

		private void UpdateEditUI(bool toShow, bool withToggleVinylsPanel = true)
		{
			editUI.SetActive(toShow);
			if (withToggleVinylsPanel)
			{
				ShowVinylsSwitchPanel(!toShow);
			}
			if (!toShow)
			{
				vinylTextUI.SetActive(value: false);
			}
		}

		private void StartBlinking(GameObject vinyl)
		{
			if ((bool)_blinkingVinyl)
			{
				_blinkingVinyl.SetActive(value: true);
			}
			if (_blinkRoutine != null)
			{
				StopCoroutine(_blinkRoutine);
			}
			_blinkingVinyl = vinyl;
			_blinkRoutine = StartCoroutine(BlinkRoutine());
		}

		private void StopBlinking()
		{
			if (_blinkRoutine != null)
			{
				StopCoroutine(_blinkRoutine);
			}
			_blinkRoutine = null;
			if ((bool)_blinkingVinyl)
			{
				_blinkingVinyl.SetActive(value: true);
			}
		}

		private IEnumerator BlinkRoutine()
		{
			for (int i = 0; i < 6; i++)
			{
				if (!_blinkingVinyl)
				{
					break;
				}
				_blinkingVinyl.SetActive(!_blinkingVinyl.activeSelf);
				yield return _blinkDelay;
			}
			if ((bool)_blinkingVinyl)
			{
				_blinkingVinyl.SetActive(value: true);
			}
			_blinkRoutine = null;
		}

		private void StartBlinkingPointer()
		{
			_disposableStopBlinkingPointer?.Dispose();
			_blinkTween?.Kill();
			clickIndicator.SetActive(value: true);
			clickIndicator.transform.localScale = Vector3.one;
			_blinkTween = clickIndicator.transform.DOScale(Vector3.one * 0.4f, 0.5f).SetLoops(-1, LoopType.Yoyo);
		}

		private void StopBlinkingPointer()
		{
			_blinkTween?.Kill();
			clickIndicator.SetActive(value: false);
		}

		public void UpdateCamFOV(float delta)
		{
			_currentFOV += delta;
			_currentFOV = Mathf.Clamp(_currentFOV, 10f, 100f);
			SetCamFOV(_currentFOV);
		}

		private void SetCamFOV(float fov)
		{
			_currentFOV = fov;
			if (!_applyingSmoothZoom)
			{
				_zoomFovTarget = fov;
			}
			LensSettings lens = _vinylsCam.m_Lens;
			lens.FieldOfView = fov;
			_vinylsCam.m_Lens = lens;
			LensSettings lens2 = _benchmarkCam.m_Lens;
			lens2.FieldOfView = fov;
			_benchmarkCam.m_Lens = lens2;
		}

		public void DeleteStickers(bool all = false)
		{
			if (all)
			{
				foreach (KeyValuePair<VinylsType, Transform> allStickersParent in _allStickersParents)
				{
					UnityEngine.Object.Destroy(allStickersParent.Value.gameObject);
				}
				_allStickersParents.Clear();
			}
			else if (_allStickersParents.ContainsKey(_currentVinylsType))
			{
				Transform obj = _allStickersParents[_currentVinylsType];
				_allStickersParents.Remove(_currentVinylsType);
				UnityEngine.Object.Destroy(obj.gameObject);
			}
		}

		private void UpdateStickers()
		{
			foreach (KeyValuePair<VinylsType, Transform> allStickersParent in _allStickersParents)
			{
				if ((bool)allStickersParent.Value)
				{
					allStickersParent.Value.gameObject.SetActive(value: false);
				}
			}
			if (!_allStickersParents.ContainsKey(_currentVinylsType))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(stickerParentPrefab, allStickersHolder);
				stickersScrollRect.content = gameObject.GetComponent<RectTransform>();
				gameObject.name = _currentVinylsType.ToString();
				_allStickersParents.Add(_currentVinylsType, gameObject.transform);
				_currentStickersParent = gameObject.transform;
				int num = 0;
				int num2 = 0;
				GameObject[] currentVinylsCollection = _currentVinylsCollection;
				foreach (GameObject gameObject2 in currentVinylsCollection)
				{
					if (gameObject2 != null)
					{
						StickerItem stickerData = CreateSticker(num, num2);
						LoadStickerData(stickerData, num2, gameObject2);
						num++;
					}
					num2++;
				}
			}
			_currentStickersParent = _allStickersParents[_currentVinylsType];
			_currentStickersParent.gameObject.SetActive(value: true);
			_lastVinylOrder = _allStickers.Count - 1;
		}

		private void UpdateStickersScroll(int count, float value = 0f, ScrollUpdateDirection direction = ScrollUpdateDirection.None)
		{
			vinylsCountLabel.text = $"{count}/{_maxVinylsCount}";
			vinylsCountLabel.transform.parent.gameObject.SetActive(count != 0);
			stickersScrollRect.gameObject.SetActive(count != 0);
			groupingButton.transform.parent.gameObject.SetActive(count > 1);
			if (stickersScrollBar.gameObject.activeSelf)
			{
				if (direction == ScrollUpdateDirection.None)
				{
					stickersScrollBar.value = ((count <= 3) ? 1f : value);
				}
				else
				{
					CenterElement(direction);
				}
			}
			stickersScrollOcclusion.RebuildOcclusion().Forget();
		}

		private void CenterElement(ScrollUpdateDirection direction)
		{
			if ((bool)_currentSticker)
			{
				RectTransform component = _currentSticker.GetComponent<RectTransform>();
				RectTransform content = stickersScrollRect.content;
				float height = stickersScrollRect.viewport.rect.height;
				float height2 = content.rect.height;
				float height3 = component.rect.height;
				float y = component.localPosition.y;
				float value = Mathf.Abs(y) - height * 0.5f + direction switch
				{
					ScrollUpdateDirection.Up => height3, 
					ScrollUpdateDirection.Down => 0f - height3, 
					_ => 0f, 
				};
				Vector3 localPosition = content.localPosition;
				localPosition.y = Mathf.Clamp(value, 0f, height2 - height);
				content.localPosition = localPosition;
			}
		}

		public void UpdateTextToggleInputHint()
		{
			if ((bool)TextToggleInputHint)
			{
				bool active = (bool)_currentSticker && _currentSticker.isText;
				TextToggleInputHint.SetActive(active);
			}
		}

		private StickerItem CreateSticker(int count, int order)
		{
			StickerItem component = _prefabFactory.Instantiate(stickerPrefab, _currentStickersParent).GetComponent<StickerItem>();
			component.Init(this);
			component.backround.color = Color.white;
			component.Order = order;
			component.UpdateOrder();
			_allStickers.Add(component);
			UpdateDeleteAllStickersButtonVisible();
			return component;
		}

		private void LoadStickerData(StickerItem stickerData, int order, GameObject vinyl)
		{
			if (EasySaveDataHandler.TryGetFile(_currentVinylsPath, out var file))
			{
				VinylsUtility.ParseVinylDataToStickerItem(file.Load<VinylData>(order.ToString()), stickerData, order, vinyl);
			}
		}

		public void UpdateInputFieldsInfo()
		{
			UpdateVinylAngleInfo();
			UpdateVinylWidthInfo();
			UpdateVinylHeightInfo();
		}

		private void Scaling()
		{
			if (!_draggingEditCollider || !_editorUI.scaleIcon)
			{
				return;
			}
			Vector3 vector = (_editorUI.scaleIcon.position = _draggingEditCollider.position);
			Vector3 position2 = vector;
			if (_oldScalePos == Vector3.one && (bool)_scaleQuad)
			{
				_oldScalePos = _scaleQuad.transform.InverseTransformPoint(position2);
			}
			else if ((bool)_scaleQuad)
			{
				_newScalePos = _scaleQuad.transform.InverseTransformPoint(position2);
				int num = ((Mathf.Abs(_newScalePos.x) > Mathf.Abs(_oldScalePos.x)) ? 1 : (-1));
				int num2 = ((Mathf.Abs(_newScalePos.y) > Mathf.Abs(_oldScalePos.y)) ? 1 : (-1));
				Transform obj = (IsGrouping ? _groupParent : CurrentVinyl.transform);
				Vector3 localScale = obj.localScale;
				localScale.x = Mathf.Abs(localScale.x);
				float num3 = Mathf.Abs(_newScalePos.x - _oldScalePos.x) * SCALE_COEF * (float)num;
				localScale.x += num3;
				localScale.x = Mathf.Clamp(localScale.x, 0.1f, 90f);
				num3 = Mathf.Abs(_newScalePos.y - _oldScalePos.y) * SCALE_COEF * (float)num2;
				localScale.y += num3;
				localScale.y = Mathf.Clamp(localScale.y, 0.1f, 90f);
				if (IsGrouping)
				{
					localScale.x = (localScale.y = Mathf.Min(localScale.x, localScale.y));
					localScale.x *= GroupXSign;
				}
				obj.localScale = localScale;
				_oldScalePos = _newScalePos;
				UpdateVinylWidthInfo();
				UpdateVinylHeightInfo();
				IsVinylsChanged = true;
			}
		}

		private void Rotating()
		{
			Vector3 vector = _editorUI.editPlane.transform.InverseTransformPoint(_draggingEditCollider.position);
			float angle = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
			OnRotating(angle);
			UpdateVinylAngleInfo();
			IsVinylsChanged = true;
		}

		private void OnRotating(float angle)
		{
			if ((bool)_editorUI.visualIcons)
			{
				Transform transform = (IsGrouping ? _groupParent : CurrentVinyl.transform);
				Vector3 localEulerAngles = transform.localEulerAngles;
				localEulerAngles.z = angle;
				transform.localEulerAngles = localEulerAngles;
				localEulerAngles = _editorUI.visualIcons.transform.localEulerAngles;
				localEulerAngles.z = 0f - angle;
				_editorUI.visualIcons.transform.eulerAngles = localEulerAngles;
				Vector3 eulerAngles = _scaleQuad.transform.eulerAngles;
				if (_sideIndex == 2)
				{
					eulerAngles.y = 0f - transform.localEulerAngles.z;
				}
				else
				{
					eulerAngles.z = 0f - transform.localEulerAngles.z;
				}
				_scaleQuad.transform.eulerAngles = eulerAngles;
			}
		}

		private void OnAngleInputChanged(string value)
		{
			if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
			{
				OnRotating(result);
			}
		}

		private void OnWidthInputChanged(string value)
		{
			if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
			{
				float value2 = Mathf.Abs(result);
				float num = Mathf.Sign(result);
				result = Mathf.Clamp(value2, 0.1f, 90f) * num;
				Vector3 localScale = CurrentVinyl.transform.localScale;
				if (IsGrouping)
				{
					localScale = _groupParent.localScale;
					localScale.x = (localScale.y = result);
					_groupParent.localScale = localScale;
					ScaleYInputField.text = value;
				}
				else
				{
					localScale.x = result;
					CurrentVinyl.transform.localScale = localScale;
				}
			}
		}

		private void OnWidthInputSubmit(string value)
		{
			string text = (IsGrouping ? _groupParent : CurrentVinyl.transform).localScale.x.ToString(CultureInfo.InvariantCulture);
			ScaleXInputField.text = text;
			if (IsGrouping)
			{
				ScaleYInputField.text = text;
			}
		}

		private void OnHeightInputChanged(string value)
		{
			if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
			{
				float value2 = Mathf.Abs(result);
				float num = Mathf.Sign(result);
				result = Mathf.Clamp(value2, 0.1f, 90f) * num;
				Vector3 localScale = CurrentVinyl.transform.localScale;
				if (IsGrouping)
				{
					localScale = _groupParent.localScale;
					localScale.x = (localScale.y = result);
					_groupParent.localScale = localScale;
					ScaleXInputField.text = value;
				}
				else
				{
					localScale.y = result;
					CurrentVinyl.transform.localScale = localScale;
				}
			}
		}

		private void OnHeightInputSubmit(string value)
		{
			string text = (IsGrouping ? _groupParent : CurrentVinyl.transform).localScale.y.ToString(CultureInfo.InvariantCulture);
			ScaleYInputField.text = text;
			if (IsGrouping)
			{
				ScaleXInputField.text = text;
			}
		}

		public void UpdateVinylAngleInfo()
		{
			Transform transform = (IsGrouping ? _groupParent : CurrentVinyl.transform);
			AngleInputField.text = transform.localEulerAngles.z.ToString("F2") + "°";
			AngleInputField.ForceLabelUpdate();
		}

		public void UpdateVinylWidthInfo()
		{
			Transform transform = (IsGrouping ? _groupParent : CurrentVinyl.transform);
			ScaleXInputField.text = transform.localScale.x.ToString("F2");
			ScaleXInputField.ForceLabelUpdate();
		}

		public void UpdateVinylHeightInfo()
		{
			Transform transform = (IsGrouping ? _groupParent : CurrentVinyl.transform);
			ScaleYInputField.text = transform.localScale.y.ToString("F2");
			ScaleYInputField.ForceLabelUpdate();
		}

		private void VinylsInfoActive(bool isOn)
		{
			VinylTransformEditPanel.SetActive(isOn);
		}

		private void OnRelease()
		{
			if (!CurrentVinyl)
			{
				return;
			}
			CurrentOperation = VinylsEditorOperation.None;
			_isDragging = false;
			if ((bool)_editorUI.editPlane)
			{
				_editorUI.rotCollider.position = _editorUI.rotIcon.position;
				_editorUI.scaleIcon.localPosition = _editorUI.initScaleIconPos;
				_editorUI.scaleCollider.position = _editorUI.scaleIcon.position;
				_editorUI.posCollider.position = _editorUI.posIcon.position;
				if ((bool)CurrentVinyl)
				{
					_editorUI.HideSomeEditIcons(VinylsEditorOperation.Create);
				}
				else
				{
					_editorUI.UpdateAllEditIcons(isEnabled: false);
				}
				_cameraManager.ChangeTarget((LockLevel == CameraLock.Full) ? null : _menuControl.tempTarget);
			}
		}

		private void FocusCamOnInitTarget()
		{
			if ((bool)_editorUI.editPlane)
			{
				_editorUI.editPlane.SetActive(value: false);
			}
			_editPlaneLastPos = Vector3.zero;
			_newFocusPos = _menuControl.tempTarget.position;
		}

		private async UniTaskVoid ReorderAllStickers()
		{
			if (_cachedVinylsFile == null)
			{
				Debug.LogError("Can't find save file");
				return;
			}
			try
			{
				_vinylDrawer.DeleteVinylContext(_currentVinylsType);
				DeleteStickers();
				_allStickers.Clear();
				await UniTask.Yield();
				List<VinylData> list = new List<VinylData>();
				string[] keys = _cachedVinylsFile.GetKeys();
				foreach (string key in keys)
				{
					VinylData vinylData = _cachedVinylsFile.Load<VinylData>(key);
					if (vinylData != null)
					{
						list.Add(vinylData);
					}
				}
				list = list.OrderBy((VinylData vinyl) => vinyl.position.z).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					VinylData vinylData2 = list[num];
					EasySaveDataHandler.SaveValue(vinylData2, _cachedVinylsFile, num.ToString());
					_vinylsDataUtility.CreateVinyl(vinylData2, _currentVinylsType);
				}
				_cachedVinylsFile.Sync();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		protected override void OnDestroy()
		{
			CleanupAlterInputScheme();
			if ((bool)_cameraManager && (bool)_cameraManager.CurrentBrain)
			{
				_cameraManager.CurrentBrain.m_UpdateMethod = _previousBrainUpdateMethod;
			}
			GameObject[] plates = Plates;
			for (int i = 0; i < plates.Length; i++)
			{
				UnityEngine.Object.Destroy(plates[i]);
			}
			if (_editorUI != null)
			{
				_editorUI.FinishAllTweens();
				UnityEngine.Object.Destroy(_editorUI.editPlane);
				UnityEngine.Object.Destroy(_editorUI.visualIcons);
				if ((bool)_editorUI.editLine1)
				{
					UnityEngine.Object.Destroy(_editorUI.editLine1.gameObject);
				}
				if ((bool)_editorUI.editLine2)
				{
					UnityEngine.Object.Destroy(_editorUI.editLine2.gameObject);
				}
				_editorUI = null;
			}
			UnityEngine.Object.Destroy(_plateMat);
			UnityEngine.Object.Destroy(_pointToLook);
			if ((bool)_groupParent)
			{
				UnityEngine.Object.Destroy(_groupParent.gameObject);
			}
			base.OnDestroy();
		}

		public void CleanStuff(bool isCompletely, bool withToggleVinylsPanel = true)
		{
			CinemachineCore.GetInputAxis = _lastAxisInputDelegate;
			if (!IsGrouping)
			{
				if ((bool)_currentSticker && _currentSticker.oldIndex != -1)
				{
					UnselectSticker(_currentSticker);
					_editingStickers.Remove(_currentSticker);
				}
			}
			else
			{
				foreach (StickerItem item in _editingStickers.Where((StickerItem sticker) => sticker.oldIndex != -1).ToList())
				{
					UnchildFromGroup(item);
					UnselectSticker(item);
					_editingStickers.Remove(item);
				}
			}
			CancelOrDelete();
			CurrentSelectedTemplate = -1;
			IsCreatingTemplate = false;
			IsReplacingTemplate = false;
			if (IsGrouping)
			{
				ChangeGroupingState();
			}
			if ((bool)_selectedUIVinyl)
			{
				_selectedUIVinyl.Select(isOn: false);
			}
			zoomSlider.slider.value = 0.5f;
			UpdateEditUI(toShow: false, withToggleVinylsPanel);
			_currentVinylRenderer = null;
			_currentTextMesh = null;
			CurrentVinyl = null;
			_currentSticker = null;
			UpdateTextToggleInputHint();
			_carCameras.distance = 5f;
			_editPlaneLastPos = Vector3.zero;
			SetCamFOV(50f);
			_cameraManager.ForceSetCameraZoom(0f);
			fieldOfViewUI.SetParent(rightButtonsParent);
			FreeLookPlateCameraButton.transform.SetParent(rightButtonsParent);
			movePlateCameraJoystick.transform.SetParent(rightButtonsParent);
			_editorUI?.UpdateAllEditIcons(isEnabled: false);
			if (_cachedVinylsFile.Size() == 0)
			{
				EasySaveDataHandler.DeleteFile(_currentVinylsPath);
			}
			else
			{
				EasySaveDataHandler.SaveFile(_currentVinylsPath);
			}
			if (isCompletely)
			{
				_cameraManager.CurrentBrain.m_UpdateMethod = _previousBrainUpdateMethod;
				stickersScrollParent.SetActive(value: false);
				base.gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(_overrideAnchor);
				_overrideAnchor = null;
				DeleteStickers(all: true);
				_isUIEnabled = true;
				UpdateUIState();
				Observable.TimerFrame(3).Subscribe((long _) =>
				{
					_vinylDrawer.DeleteVinylContext(VinylsType.CarVinyls);
					_vinylDrawer.DeleteVinylContext(VinylsType.PlateVinyls);
					_vinylDrawer.DeleteVinylContext(VinylsType.FlagVinyls);
					_vinylDrawer.DeleteVinylContext(VinylsType.WindowVinyls);
					_vinylDrawer.SetCamState(isActive: false);
				});
				EasySaveDataHandler.ReleaseFile(_currentVinylsPath);
				_plateCreatorTableTr.gameObject.SetActive(value: true);
			}
			else
			{
				if ((bool)_currentStickersParent)
				{
					_currentStickersParent.gameObject.SetActive(value: false);
				}
				CurrentOperation = VinylsEditorOperation.None;
			}
			_allStickers.Clear();
			UpdateDeleteAllStickersButtonVisible();
		}

		private void UpdateDeleteAllStickersButtonVisible()
		{
			deleteAllStickersButton.gameObject.SetActive(_editingStickers.Count > 0 || _allStickers.Count > 0);
		}

		public void ShowVinylsSwitchPanel(bool isActive)
		{
			VinylsSwitchPanel.SetActive(isActive);
		}

		public void TryShowVinylsSwitchPanel()
		{
			if (!editUI.activeSelf)
			{
				ShowVinylsSwitchPanel(isActive: true);
			}
		}

		public void ShowAllVinyls()
		{
			VinylsEditorInputState currentState = CurrentState;
			if (currentState == VinylsEditorInputState.Selection || currentState == VinylsEditorInputState.Windows || currentState == VinylsEditorInputState.Plate || currentState == VinylsEditorInputState.PlateTypes || currentState == VinylsEditorInputState.OneSelected)
			{
				VinylsScrollRect.content = ParentOfStickers.GetComponent<RectTransform>();
				ParentOfStickers.gameObject.SetActive(value: true);
				ParentOfFavouriteStickers.gameObject.SetActive(value: false);
				_navigateNewIndex = 0;
			}
		}

		public void ShowFavouriteVinyls()
		{
			VinylsEditorInputState currentState = CurrentState;
			if (currentState == VinylsEditorInputState.Selection || currentState == VinylsEditorInputState.Windows || currentState == VinylsEditorInputState.Plate || currentState == VinylsEditorInputState.PlateTypes || currentState == VinylsEditorInputState.OneSelected)
			{
				VinylsScrollRect.content = ParentOfFavouriteStickers.GetComponent<RectTransform>();
				ParentOfStickers.gameObject.SetActive(value: false);
				ParentOfFavouriteStickers.gameObject.SetActive(value: true);
				_navigateNewIndex = 0;
			}
		}

		public void UpdateFavouriteVinyls()
		{
			foreach (Transform parentOfFavouriteSticker in ParentOfFavouriteStickers)
			{
				UnityEngine.Object.Destroy(parentOfFavouriteSticker.gameObject);
			}
			int num = 0;
			OneVinylData[] allVinyls = V.AllVinyls;
			foreach (OneVinylData vinylData in allVinyls)
			{
				VynilStickerUI vynilStickerUI = CreateVinylForFavourite(num, vinylData);
				num++;
				if ((bool)vynilStickerUI && FavouriteVinylsController.SelectedItemId == vynilStickerUI.Index)
				{
					vynilStickerUI.Select(isOn: true);
					if ((bool)_selectedUIVinyl)
					{
						_selectedUIVinyl.Select(isOn: false);
					}
					_selectedUIVinyl = vynilStickerUI;
					_selectedUIVinyl.Select(isOn: true);
				}
			}
		}

		[Inject]
		public void Construct(EventRegistry eventRegistry, InputMapSetEvent inputMapSetEvent, UiNavigationValueEvent uiNavigationValueEvent)
		{
			_eventRegistry = eventRegistry;
			_inputMapSetEvent = inputMapSetEvent;
			_uiNavigationValueEvent = uiNavigationValueEvent;
		}

		private void HandleCameraZoom()
		{
			if (LockLevel == CameraLock.Full)
			{
				return;
			}
			if (_isFreeLook && CurrentState == VinylsEditorInputState.CameraOnly)
			{
				float num = 0f - _zoomVinylHandle.Get(_eventRegistry).Value;
				if (_detector != null && _detector.CurrentDeviceType != InputDeviceType.KeyboardMouse)
				{
					num *= 0.5f;
				}
				if (Mathf.Abs(num) > 1E-05f)
				{
					_cameraManager.InvokeCameraZoom(num * 70f);
				}
				return;
			}
			if (_zoomFovTarget < 0f)
			{
				_zoomFovTarget = _currentFOV;
			}
			float num2 = (0f - _zoomVinylHandle.Get(_eventRegistry).Value) * 10f;
			if (_detector != null && _detector.CurrentDeviceType != InputDeviceType.KeyboardMouse)
			{
				num2 *= 0.5f;
			}
			if (Mathf.Abs(num2) > 1E-05f)
			{
				_zoomFovTarget = Mathf.Clamp(_zoomFovTarget + num2, 10f, 100f);
			}
			if (Mathf.Abs(_currentFOV - _zoomFovTarget) > 0.01f)
			{
				_applyingSmoothZoom = true;
				SetCamFOV(Mathf.SmoothDamp(_currentFOV, _zoomFovTarget, ref _zoomFovVel, 0.12f));
				_applyingSmoothZoom = false;
			}
		}

		private void NewUpdate()
		{
			HandleCameraZoom();
			float value = (_currentFOV - 10f) / 90f;
			float num = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value));
			Vector2 vector = _lookVinylHandle.Get(_eventRegistry).Value;
			if (_detector != null && _detector.CurrentDeviceType != InputDeviceType.KeyboardMouse)
			{
				vector = Vector2.Scale(vector, GamepadLookScale);
			}
			if (_currentVinylsType == VinylsType.PlateVinyls && (bool)_benchmarkCamOffset && CurrentState != VinylsEditorInputState.CameraOnly)
			{
				vector = num * vector;
				UpdatePlateCameraFromLook(vector);
			}
			else
			{
				vector = ((LockLevel == CameraLock.Full) ? Vector2.zero : (num * Vector2.Scale(Multiplier, vector)));
				_lookHandle.Get(_eventRegistry).Value = vector;
			}
			UpdateCurrentVinyl();
			UpdateCurrentVinylWithAlterInput();
		}

		private void UpdatePlateCameraFromLook(Vector2 look)
		{
			if (!(look.sqrMagnitude < 0.0001f))
			{
				float num = plateCameraMoveConfig.VelocityByFOV.GetValue(CurrentFOV) * 0.1f;
				Vector3 offset = _benchmarkCamOffset.m_Offset + new Vector3(look.x * num, look.y * num);
				offset.x = Mathf.Clamp(offset.x, 0f - plateCameraMoveConfig.ClampXPosition, plateCameraMoveConfig.ClampXPosition);
				offset.y = Mathf.Clamp(offset.y, plateCameraMoveConfig.PosYMin, plateCameraMoveConfig.PosYMax);
				_benchmarkCamOffset.m_Offset = offset;
			}
		}

		private void InitAlterInputScheme()
		{
			SubscribeToActions();
			if ((bool)ParentOfFavouriteStickers)
			{
				ParentOfFavouriteStickers.gameObject.SetActive(value: false);
			}
			if ((bool)stickersScrollRect && !stickersScrollRect.TryGetComponent<GamepadScrollCenter>(out var component))
			{
				stickersScrollRect.gameObject.AddComponent<GamepadScrollCenter>();
			}
			if ((bool)VinylsScrollRect && !VinylsScrollRect.TryGetComponent<GamepadScrollCenter>(out component))
			{
				VinylsScrollRect.gameObject.AddComponent<GamepadScrollCenter>();
			}
			_selectionCtx = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, base.destroyCancellationToken);
		}

		private void CleanupAlterInputScheme()
		{
			UnsubscribeFromActions();
			_selectionCtx?.Cancel();
			_selectionCtx?.Dispose();
			_selectionCtx = null;
		}

		private void SubscribeToActions()
		{
			SubWithClick(_onLayerUpDownHandle.Get(_eventRegistry), LayerUpDown);
			ScrollValueEvent sv = _scrollValueEventHandle.Get(_eventRegistry);
			Action<ScrollEnum> onScroll = OnScrollDispatch;
			sv?.Subscribe(onScroll);
			_subscriptionRemovers.Add(() =>
			{
				sv?.Unsubscribe(onScroll);
			});
			SubWithClick(_toggleToSizeHandle.Get(_eventRegistry), EditSize);
			SubWithClick(_toggleToColorHandle.Get(_eventRegistry), EditColors);
			SubWithClick(_toggleToTextHandle.Get(_eventRegistry), EditText);
			SubWithClick(_copyHandle.Get(_eventRegistry), CopyVinyls);
			SubWithClick(_deleteAllHandle.Get(_eventRegistry), OnDeleteAllStickersClick);
			SubWithClick(_deleteSelectedHandle.Get(_eventRegistry), OnDeleteSelectedSticker);
			SubWithClick(_mirrorHandle.Get(_eventRegistry), FlipX);
			SubWithClick(_setSizeHandle.Get(_eventRegistry), ToggleSizeInfo);
			SubWithClick(_setColorHandle.Get(_eventRegistry), SetColorHex);
			SubWithClick(_onSetTextHandle.Get(_eventRegistry), SetText);
			SubWithClick(_toggleBoldHandle.Get(_eventRegistry), ToggleBold);
			SubWithClick(_toggleItalicsHandle.Get(_eventRegistry), ToggleItalics);
			SubWithClick(_onFontUpDownHandle.Get(_eventRegistry), FontUpDown);
			SubWithClick(_blockCameraHandle.Get(_eventRegistry), BlockCamera);
			SubWithClick(_toggleMultiSelectHandle.Get(_eventRegistry), ToggleGroupingVinyls);
			SubWithClick(_acceptHandle.Get(_eventRegistry), OnSaveAction);
			SubWithClick(_onCancelChangesHandle.Get(_eventRegistry), OnRevertAction);
			SubWithClick(_onUnselectOrBackHandle.Get(_eventRegistry), UnselectOrGoBackAction);
			SubWithClick(_onStartColorMoveHandle.Get(_eventRegistry), StartColorChange);
			SubWithClick(_onStartTintMoveHandle.Get(_eventRegistry), StartTintChange);
			_onConfirmClickHandle.Get(_eventRegistry)?.Subscribe(StartCreatingByClick);
			SubWithClick(_onEnterPlateTypesHandle.Get(_eventRegistry), EditPlateTypes);
			SubWithClick(_onToggleFrontPlateHandle.Get(_eventRegistry), ToggleFrontPlate);
			SubWithClick(_onToggleBackPlateHandle.Get(_eventRegistry), ToggleBackPlate);
			SubWithClick(_toggleUiHandle.Get(_eventRegistry), ChangeUIState);
			_detector = InputDeviceDetector.Instance;
			if (_detector != null)
			{
				_detector.OnDeviceTypeChanged += HideShowUiElementsWithInputDevice;
				HideShowUiElementsWithInputDevice(_detector.CurrentDeviceType);
			}
		}

		private void PlayClick()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.SmallClickSwitch, base.gameObject);
		}

		private void SubWithClick(ScriptableEvent ev, Action handler)
		{
			if (!(ev == null))
			{
				Action wrapped = () =>
				{
					PlayClick();
					handler();
				};
				ev.Subscribe(wrapped);
				_subscriptionRemovers.Add(() =>
				{
					ev.Unsubscribe(wrapped);
				});
			}
		}

		private void SubWithClick(BoolEvent ev, Action<bool> handler)
		{
			if (!(ev == null))
			{
				Action<bool> wrapped = (bool b) =>
				{
					PlayClick();
					handler(b);
				};
				ev.Subscribe(wrapped);
				_subscriptionRemovers.Add(() =>
				{
					ev.Unsubscribe(wrapped);
				});
			}
		}

		private void UnsubscribeFromActions()
		{
			foreach (Action subscriptionRemover in _subscriptionRemovers)
			{
				subscriptionRemover();
			}
			_subscriptionRemovers.Clear();
			_onConfirmClickHandle.Get(_eventRegistry)?.Unsubscribe(StartCreatingByClick);
			if (_detector != null)
			{
				_detector.OnDeviceTypeChanged -= HideShowUiElementsWithInputDevice;
			}
		}

		private void StartCreatingByClick(bool isStarted)
		{
			UiNavigationEnum storedValue = _uiNavigationValueEvent.StoredValue;
			if (storedValue != UiNavigationEnum.Mouse && storedValue != UiNavigationEnum.Stick)
			{
				return;
			}
			bool flag = IsPointerOverInteractableUI();
			if ((isStarted & flag) || Time.frameCount - _lastSavedFrame <= 1)
			{
				return;
			}
			if (_isPointerDown && !isStarted && !flag)
			{
				VinylsEditorInputState currentState = CurrentState;
				if (currentState == VinylsEditorInputState.Selection || currentState == VinylsEditorInputState.Windows || currentState == VinylsEditorInputState.Plate)
				{
					Operate();
				}
			}
			if (!isStarted && _isDragging)
			{
				OnRelease();
			}
			_isPointerDown = isStarted;
		}

		private bool IsPointerOverInteractableUI()
		{
			if (!EventSystem.current)
			{
				return false;
			}
			PointerEventData eventData = new PointerEventData(EventSystem.current)
			{
				position = GetPointerScreenPosition()
			};
			_uiRaycastResults.Clear();
			EventSystem.current.RaycastAll(eventData, _uiRaycastResults);
			for (int i = 0; i < _uiRaycastResults.Count; i++)
			{
				if (_uiRaycastResults[i].module is GraphicRaycaster)
				{
					GameObject gameObject = _uiRaycastResults[i].gameObject;
					if ((bool)gameObject.GetComponentInParent<Selectable>() || (bool)gameObject.GetComponentInParent<ScrollRect>())
					{
						return true;
					}
				}
			}
			return false;
		}

		private void ResetOnCloseVinylsEditor()
		{
			if (_correspondingInputGroups.TryGetValue(VinylsEditorInputState.Default, out var value))
			{
				_inputMapSetEvent.TrySetGroup(value);
				showVynilScalesToggle.isOn = false;
				OnEditorStateChanged?.Invoke(VinylsEditorInputState.Default);
				_editorStateChain.Clear();
			}
		}

		private void ToggleSizeInfo()
		{
			if (CurrentState == VinylsEditorInputState.SizeChange)
			{
				TMP_InputField tMP_InputField = null;
				string placeHolderText = "";
				switch (_lastSizeInfoIndex % 3)
				{
				case 0:
					tMP_InputField = AngleInputField;
					placeHolderText = "AngleInputField";
					break;
				case 1:
					tMP_InputField = ScaleXInputField;
					placeHolderText = "ScaleXInputField";
					break;
				case 2:
					tMP_InputField = ScaleYInputField;
					placeHolderText = "ScaleYInputField";
					break;
				}
				if ((bool)EventSystem.current && (bool)tMP_InputField)
				{
					EventSystem.current.SetSelectedGameObject(tMP_InputField.gameObject);
					tMP_InputField.ActivateInputField();
					UISelectionCursor.OpenKeyboardStatic(tMP_InputField, placeHolderText);
					_lastSizeInfoIndex = _lastSizeInfoIndex++ % 3;
				}
			}
		}

		private void SetEditorStateWithCurrentRow()
		{
			VinylsEditorInputState currentState;
			VinylsType vinylsType;
			switch (_currentRowSelected)
			{
			case 2:
				currentState = VinylsEditorInputState.Windows;
				vinylsType = VinylsType.WindowVinyls;
				break;
			case 1:
				currentState = VinylsEditorInputState.Plate;
				vinylsType = VinylsType.PlateVinyls;
				break;
			case 0:
				currentState = VinylsEditorInputState.Selection;
				vinylsType = VinylsType.CarVinyls;
				break;
			default:
				currentState = _editorStateChain.FirstOrDefault();
				vinylsType = VinylsType.CarVinyls;
				break;
			}
			_editorStateChain.Clear();
			CurrentState = currentState;
			_vinylDrawer.FocusOnVinyls(vinylsType);
			_carCameras.xAngle = 0f;
			_carCameras.yAngle = 0f;
		}

		public void OnNextPreviousEditor(bool isNext)
		{
			if (isNext)
			{
				NextVinylsType();
			}
			else
			{
				PrevVinylsType();
			}
		}

		private void NavigatePlates(ScrollEnum dir)
		{
			int childCount = ParentOfPlates.childCount;
			if (childCount != 0)
			{
				bool num = dir == ScrollEnum.Right;
				int num2 = _currPlateNum - 1;
				int index = (num ? ((num2 + 1) % childCount) : ((num2 - 1 + childCount) % childCount));
				Transform child = ParentOfPlates.GetChild(index);
				ChangePlate(child.GetComponent<VynilStickerUI>());
				PlayClick();
			}
		}

		private void OnScrollDispatch(ScrollEnum dir)
		{
			switch (CurrentState)
			{
			case VinylsEditorInputState.Selection:
			case VinylsEditorInputState.OneSelected:
			case VinylsEditorInputState.Plate:
			case VinylsEditorInputState.Windows:
				switch (dir)
				{
				case ScrollEnum.Left:
				case ScrollEnum.Right:
					NavigateNewVinyls(dir);
					break;
				case ScrollEnum.Up:
				case ScrollEnum.Down:
					NavigateExistingStickers(dir);
					break;
				}
				break;
			case VinylsEditorInputState.MultiSelection:
			case VinylsEditorInputState.MultiSelected:
				if (dir == ScrollEnum.Up || dir == ScrollEnum.Down)
				{
					NavigateExistingStickers(dir);
				}
				break;
			case VinylsEditorInputState.PlateTypes:
				if (dir == ScrollEnum.Left || dir == ScrollEnum.Right)
				{
					NavigatePlates(dir);
				}
				break;
			case VinylsEditorInputState.ColorChange:
			case VinylsEditorInputState.SizeChange:
			case VinylsEditorInputState.TextChange:
				break;
			}
		}

		private void ToggleFrontPlate()
		{
			FrontPlateToggle.isOn = !FrontPlateToggle.isOn;
		}

		private void ToggleBackPlate()
		{
			RearPlateToggle.isOn = !RearPlateToggle.isOn;
		}

		private void HideShowUiElementsWithInputDevice(InputDeviceType inputDeviceType)
		{
			bool flag = inputDeviceType == InputDeviceType.KeyboardMouse;
			showVynilScalesToggle.isOn = !flag && CurrentState == VinylsEditorInputState.SizeChange;
			if (!flag)
			{
				VinylsEditorInputState currentState = CurrentState;
				if (currentState != VinylsEditorInputState.MultiSelected && currentState != VinylsEditorInputState.OneSelected)
				{
					ToggleEditorUiForCurrentState();
					goto IL_0060;
				}
			}
			CurrentOperation = VinylsEditorOperation.Create;
			if (_editorStateChain.Count > 0)
			{
				_editorUI.HideSomeEditIcons(CurrentOperation);
			}
			goto IL_0060;
			IL_0060:
			OnEditorStateChanged?.Invoke((!flag) ? CurrentState : VinylsEditorInputState.Default);
		}

		private bool SetPreviousInputGroup()
		{
			if (CurrentState == VinylsEditorInputState.Default)
			{
				return false;
			}
			if (CurrentState == VinylsEditorInputState.MultiSelected)
			{
				_editorStateChain.RemoveAt(_editorStateChain.Count - 1);
			}
			_editorStateChain.RemoveAt(_editorStateChain.Count - 1);
			VinylsEditorInputState currentState = CurrentState;
			if (!_correspondingInputGroups.TryGetValue(currentState, out var value))
			{
				return false;
			}
			bool result = _inputMapSetEvent.TrySetGroup(value);
			bool flag = _detector.CurrentDeviceType == InputDeviceType.KeyboardMouse;
			showVynilScalesToggle.isOn = !flag && currentState == VinylsEditorInputState.SizeChange;
			Action<VinylsEditorInputState> action = OnEditorStateChanged;
			if (action != null)
			{
				action(currentState);
				return result;
			}
			return result;
		}

		private void EditText()
		{
			if (CurrentState == VinylsEditorInputState.OneSelected && (_currentSticker.isText || _currentSticker.stckerIndex == 1))
			{
				CurrentState = VinylsEditorInputState.TextChange;
				ToggleEditorUiForCurrentState();
				UpdateTextToggleInputHint();
			}
		}

		private void EditColors()
		{
			if (CurrentState == VinylsEditorInputState.OneSelected)
			{
				CurrentState = VinylsEditorInputState.ColorChange;
				ToggleEditorUiForCurrentState();
			}
		}

		private void EditSize()
		{
			VinylsEditorInputState currentState = CurrentState;
			if (currentState == VinylsEditorInputState.OneSelected || currentState == VinylsEditorInputState.MultiSelected)
			{
				CurrentState = VinylsEditorInputState.SizeChange;
				_rotateNeedsRelease = true;
				_rotateSeenHeld = false;
				_sizeEnterFrame = Time.frameCount;
				ToggleEditorUiForCurrentState();
			}
		}

		private void EditPlateTypes()
		{
			VinylsEditorInputState currentState = CurrentState;
			if (currentState == VinylsEditorInputState.Plate || currentState == VinylsEditorInputState.PlateTypes)
			{
				ShowPlatesScroll();
				ToggleEditorUiForCurrentState();
			}
		}

		private void BlockCamera()
		{
			if (CurrentState == VinylsEditorInputState.CameraOnly)
			{
				_cancelHandle.Get(_eventRegistry)?.Raise();
				return;
			}
			VinylsEditorInputState currentState = CurrentState;
			if (currentState != VinylsEditorInputState.Plate && currentState != VinylsEditorInputState.PlateTypes)
			{
				UpdateLockLevel();
			}
			else
			{
				PlateFreeLookToggle();
			}
		}

		private void ToggleGroupingVinyls()
		{
			switch (CurrentState)
			{
			case VinylsEditorInputState.Selection:
			case VinylsEditorInputState.Plate:
			case VinylsEditorInputState.Windows:
				if (ChangeGroupingState(checkCount: false))
				{
					CurrentState = VinylsEditorInputState.MultiSelection;
				}
				break;
			case VinylsEditorInputState.MultiSelection:
				if (_detector.CurrentDeviceType != InputDeviceType.KeyboardMouse)
				{
					if (_editingStickers.Count > 0)
					{
						CurrentState = VinylsEditorInputState.MultiSelected;
					}
					else if (ChangeGroupingState(checkCount: false))
					{
						SetEditorStateWithCurrentRow();
					}
				}
				else if (ChangeGroupingState())
				{
					SetEditorStateWithCurrentRow();
				}
				break;
			case VinylsEditorInputState.MultiSelected:
				if (ChangeGroupingState())
				{
					SetEditorStateWithCurrentRow();
				}
				break;
			default:
				_coreGameManager.SetMessage(new LocalizedString("VinylsEditor.SaveDeleteCurrentVinylS"), 2f);
				break;
			}
		}

		private void StartTintChange()
		{
			_isColorChangeActive = false;
		}

		private void StartColorChange()
		{
			_isColorChangeActive = true;
		}

		private void OnRevertAction()
		{
			if ((bool)_currentSticker)
			{
				RollBackAllChanges();
			}
		}

		private void OnDeleteSelectedSticker()
		{
			VinylsEditorInputState currentState = CurrentState;
			if ((currentState == VinylsEditorInputState.OneSelected || currentState == VinylsEditorInputState.MultiSelected) && (bool)_currentSticker)
			{
				CancelOrDelete();
			}
		}

		private void UnselectOrGoBackAction()
		{
			switch (CurrentState)
			{
			case VinylsEditorInputState.Selection:
			case VinylsEditorInputState.Plate:
			case VinylsEditorInputState.Windows:
				_cancelHandle.Get(_eventRegistry)?.Raise();
				break;
			case VinylsEditorInputState.OneSelected:
				UnselectOrClearCurrentSticker();
				break;
			case VinylsEditorInputState.MultiSelection:
			case VinylsEditorInputState.MultiSelected:
				if (!_isVinylsChanged)
				{
					UnselectOrClearCurrentSticker();
					ChangeGroupingState(checkCount: false);
					SetEditorStateWithCurrentRow();
				}
				else if (ChangeGroupingState())
				{
					SetEditorStateWithCurrentRow();
					if (LockLevel == CameraLock.Full)
					{
						_cameraManager.ChangeTarget(null);
					}
					else
					{
						_cameraManager.ChangeTarget(_currentVinyl ? _menuControl.tempTarget : _currentPivotTransform);
					}
				}
				break;
			case VinylsEditorInputState.PlateTypes:
				ShowPlatesScroll();
				break;
			case VinylsEditorInputState.ColorChange:
			case VinylsEditorInputState.SizeChange:
			case VinylsEditorInputState.TextChange:
				SetPreviousInputGroup();
				CurrentOperation = VinylsEditorOperation.Create;
				_editorUI.HideSomeEditIcons(CurrentOperation);
				break;
			}
		}

		private void UnselectOrClearCurrentSticker()
		{
			if (!_currentSticker)
			{
				return;
			}
			if (_currentSticker.oldIndex == -1)
			{
				CancelOrDelete();
				return;
			}
			UnselectSticker(_currentSticker);
			CurrentVinyl = null;
			_currentVinylRenderer = null;
			_currentTextMesh = null;
			_currentSticker = null;
			ClearGroup();
			UpdateEditUI(toShow: false);
			_editorUI.UpdateAllEditIcons(isEnabled: false);
			IsVinylsChanged = false;
			FocusCamOnInitTarget();
			CurrentOperation = (_selectedUIVinyl ? VinylsEditorOperation.Create : VinylsEditorOperation.None);
			_isPointerDown = false;
			if (LockLevel == CameraLock.Full)
			{
				_cameraManager.ChangeTarget(null);
			}
			else
			{
				_cameraManager.ChangeTarget(_menuControl.tempTarget);
			}
		}

		private void OnSaveAction()
		{
			if (_inputMapSetEvent.CurrentInputGroup == InputGroupsEnum.Ui)
			{
				return;
			}
			switch (CurrentState)
			{
			case VinylsEditorInputState.ColorChange:
			case VinylsEditorInputState.SizeChange:
			case VinylsEditorInputState.TextChange:
				SetPreviousInputGroup();
				CurrentOperation = VinylsEditorOperation.Create;
				_editorUI.HideSomeEditIcons(CurrentOperation);
				break;
			case VinylsEditorInputState.MultiSelected:
			case VinylsEditorInputState.OneSelected:
				if ((bool)_currentSticker && (_isDragging || _isVinylsChanged))
				{
					SaveVinyl();
					_isPointerDown = false;
					if (LockLevel == CameraLock.Full)
					{
						_cameraManager.ChangeTarget(null);
					}
					else
					{
						_cameraManager.ChangeTarget(_currentVinyl ? _menuControl.tempTarget : _currentPivotTransform);
					}
					_selectionCtx.Cancel();
					_selectionCtx.Dispose();
					_selectionCtx = CancellationTokenSource.CreateLinkedTokenSource(new CancellationTokenSource().Token, base.destroyCancellationToken);
				}
				break;
			}
		}

		private void LayerUpDown(bool isLayerDown)
		{
			if (isLayerDown)
			{
				BackZ();
			}
			else
			{
				ForwardZ();
			}
		}

		private void NavigateExistingStickers(ScrollEnum dir)
		{
			if (_allStickers.Count == 0)
			{
				return;
			}
			bool flag = dir == ScrollEnum.Up;
			int count = _allStickers.Count;
			int num = Mathf.Clamp(_navigateExistingIndex, 0, count - 1);
			GameObject gameObject = (EventSystem.current ? EventSystem.current.currentSelectedGameObject : null);
			if ((bool)gameObject && gameObject.TryGetComponent<StickerItem>(out var component))
			{
				int num2 = _allStickers.IndexOf(component);
				if (num2 >= 0)
				{
					num = num2;
				}
			}
			else if ((bool)_currentSticker)
			{
				int num3 = _allStickers.IndexOf(_currentSticker);
				if (num3 >= 0)
				{
					num = num3;
				}
			}
			int num4 = (flag ? (num - 1) : (num + 1));
			num4 = (num4 % count + count) % count;
			if (num4 != num || !_currentSticker)
			{
				_navigateExistingIndex = num4;
				StickerItem stickerItem = _allStickers[_navigateExistingIndex];
				PlayClick();
				UISelectionCursor.ShowCursor(stickerItem.gameObject);
			}
		}

		private void NavigateNewVinyls(ScrollEnum dir)
		{
			bool flag = dir == ScrollEnum.Right;
			Transform content = VinylsScrollRect.content;
			if (!content || content.childCount == 0)
			{
				return;
			}
			int childCount = content.childCount;
			_navigateNewIndex = Mathf.Clamp(_navigateNewIndex, 0, childCount - 1);
			int num = (flag ? (_navigateNewIndex + 1) : (_navigateNewIndex - 1));
			num = (num % childCount + childCount) % childCount;
			_navigateNewIndex = num;
			Transform child = content.GetChild(_navigateNewIndex);
			if (child.TryGetComponent<VynilStickerUI>(out var component))
			{
				VinylsEditorInputState currentState = CurrentState;
				if (currentState == VinylsEditorInputState.OneSelected || currentState == VinylsEditorInputState.Selection || currentState == VinylsEditorInputState.Windows || currentState == VinylsEditorInputState.Plate)
				{
					SelectVinylUI(component);
					PlayClick();
					UISelectionCursor.ShowCursor(child.gameObject);
				}
			}
		}

		private void RestoreNewVinylSelection()
		{
			if ((bool)ParentOfStickers && ParentOfStickers.childCount != 0)
			{
				int value = (_newVinylIndexPerType.TryGetValue(_currentVinylsType, out var value2) ? value2 : 0);
				value = Mathf.Clamp(value, 0, ParentOfStickers.childCount - 1);
				Transform child = ParentOfStickers.GetChild(value);
				_suppressIndexStore = true;
				SelectVinylUI(child.GetComponent<VynilStickerUI>());
				_suppressIndexStore = false;
				UISelectionCursor.ShowCursor(child.gameObject);
			}
		}

		private void SetColorHex()
		{
			if (CurrentState == VinylsEditorInputState.ColorChange && (bool)EventSystem.current && (bool)hexInputField)
			{
				EventSystem.current.SetSelectedGameObject(hexInputField.gameObject);
				hexInputField.ActivateInputField();
				UISelectionCursor.OpenKeyboardStatic(hexInputField, "EnterColorHex");
			}
		}

		private void SetText()
		{
			if (CurrentState == VinylsEditorInputState.TextChange && (bool)EventSystem.current && (bool)inputText)
			{
				EventSystem.current.SetSelectedGameObject(inputText.gameObject);
				inputText.ActivateInputField();
				UISelectionCursor.OpenKeyboardStatic(inputText);
			}
		}

		private void ToggleBold()
		{
			if (CurrentState == VinylsEditorInputState.TextChange)
			{
				boldToggle.isOn = !boldToggle.isOn;
			}
		}

		private void ToggleItalics()
		{
			if (CurrentState == VinylsEditorInputState.TextChange)
			{
				italicToggle.isOn = !italicToggle.isOn;
			}
		}

		private void FontUpDown(bool isUp)
		{
			if (CurrentState == VinylsEditorInputState.TextChange)
			{
				int count = fontsInitializer.options.Count;
				if (count != 0)
				{
					int num = (isUp ? (_lastFontIndex + 1) : (_lastFontIndex - 1));
					num = (num % count + count) % count;
					fontsInitializer.value = num;
				}
			}
		}

		private void MoveVinylPosition(Vector2 positionChange, Transform editTransform)
		{
			if (positionChange.sqrMagnitude < 0.0001f)
			{
				_suppressCameraFollow = false;
				if (CurrentOperation == VinylsEditorOperation.Move)
				{
					ToggleEditorUiForCurrentState();
					CurrentOperation = VinylsEditorOperation.None;
				}
				return;
			}
			_suppressCameraFollow = true;
			if ((bool)_editorUI.editPlane && !_editorUI.editPlane.activeSelf)
			{
				_editorUI.editPlane.SetActive(value: true);
				_editorUI.editPlane.transform.position = editTransform.position;
			}
			Vector3 right = _mainCam.transform.right;
			Vector3 up = _mainCam.transform.up;
			Vector3 vector = (right * positionChange.x + up * positionChange.y) * (0.4f * Time.deltaTime);
			_editorUI.editPlane.transform.position += vector;
			Vector3 pos = _mainCam.WorldToScreenPoint(_editorUI.editPlane.transform.position);
			Ray ray = _mainCam.ScreenPointToRay(pos);
			if (!Physics.Raycast(ray, out var hitInfo, _carCameras.distance + 3f, _currentLayerMask) || !(hitInfo.collider is MeshCollider))
			{
				float maxDistance = _lastRaycastHitDistance + 0.3f;
				int num = Physics.SphereCastNonAlloc(ray, 0.015f, _sphereCastHits, maxDistance, _currentLayerMask);
				bool flag = false;
				float num2 = -1f;
				for (int i = 0; i < num; i++)
				{
					ref RaycastHit reference = ref _sphereCastHits[i];
					if (reference.collider is MeshCollider)
					{
						float num3 = Vector3.Dot(reference.normal, -ray.direction);
						if (!(num3 < 0.05f) && num3 > num2)
						{
							num2 = num3;
							hitInfo = reference;
							flag = true;
						}
					}
				}
				if (!flag)
				{
					_editorUI.editPlane.transform.position -= vector;
					return;
				}
			}
			Vector2 textureCoord = hitInfo.textureCoord;
			float orthographicSize = _vinylDrawer.Camera.orthographicSize;
			Vector3 uvLocalPosition = new Vector3(textureCoord.x - orthographicSize, textureCoord.y - orthographicSize, 0f);
			if (IsGrouping)
			{
				PositionGroup(uvLocalPosition);
			}
			else
			{
				PositionCurrentVinyl(uvLocalPosition);
			}
			_editorUI.editPlane.transform.position = hitInfo.point;
			CheckSide(hitInfo.point, hitInfo.normal);
			_lastRaycastHitDistance = hitInfo.distance;
			CurrentOperation = VinylsEditorOperation.Move;
			ToggleEditorUiForCurrentOperation();
			IsVinylsChanged = true;
		}

		private void ChangeVinylTransform(Vector2 sizeChange, float turnLeftRight, Transform editTransform)
		{
			float deltaTime = Time.deltaTime;
			if (sizeChange.sqrMagnitude > 0.0001f)
			{
				Vector3 localScale = editTransform.localScale;
				float value = Mathf.Abs(localScale.x) + sizeChange.x * 3f * deltaTime;
				float value2 = Mathf.Abs(localScale.y) + sizeChange.y * 3f * deltaTime;
				localScale.x = Mathf.Clamp(value, 0.1f, 90f);
				localScale.y = Mathf.Clamp(value2, 0.1f, 90f);
				if (IsGrouping)
				{
					localScale.x = (localScale.y = Mathf.Min(localScale.x, localScale.y));
					localScale.x *= GroupXSign;
				}
				editTransform.localScale = localScale;
				UpdateVinylWidthInfo();
				UpdateVinylHeightInfo();
				CurrentOperation = VinylsEditorOperation.Scale;
				ToggleEditorUiForCurrentOperation();
				IsVinylsChanged = true;
			}
			else if (CurrentOperation == VinylsEditorOperation.Scale)
			{
				ToggleEditorUiForCurrentState();
				CurrentOperation = VinylsEditorOperation.None;
			}
			if (Mathf.Abs(turnLeftRight) > 0.001f)
			{
				float angle = editTransform.localEulerAngles.z + turnLeftRight * 90f * deltaTime;
				OnRotating(angle);
				UpdateVinylAngleInfo();
				CurrentOperation = VinylsEditorOperation.Rotate;
				ToggleEditorUiForCurrentOperation();
				IsVinylsChanged = true;
			}
			else if (CurrentOperation == VinylsEditorOperation.Rotate)
			{
				ToggleEditorUiForCurrentState();
				CurrentOperation = VinylsEditorOperation.None;
			}
		}

		private void ToggleEditorUiForCurrentState()
		{
			if (_editorUI != null)
			{
				_editorUI.UpdateAllEditIcons(isEnabled: true);
				_ = (bool)_editorUI.rotIcon;
				if ((bool)_editorUI.rotIcon)
				{
					_editorUI.rotIcon.gameObject.SetActive(CurrentState == VinylsEditorInputState.SizeChange);
				}
				if ((bool)_editorUI.scaleIcon)
				{
					_editorUI.scaleIcon.gameObject.SetActive(CurrentState == VinylsEditorInputState.SizeChange);
				}
				if ((bool)_editorUI.posIcon)
				{
					GameObject obj = _editorUI.posIcon.gameObject;
					VinylsEditorInputState currentState = CurrentState;
					obj.SetActive(currentState == VinylsEditorInputState.OneSelected || currentState == VinylsEditorInputState.MultiSelected);
				}
				if ((bool)_editorUI.middlePoint && CurrentState == VinylsEditorInputState.PlateTypes)
				{
					_editorUI.middlePoint.gameObject.SetActive(value: false);
				}
			}
		}

		private void ToggleEditorUiForCurrentOperation()
		{
			if (_editorUI != null)
			{
				_ = (bool)_editorUI.rotIcon;
				if ((bool)_editorUI.rotIcon)
				{
					_editorUI.rotIcon.gameObject.SetActive(CurrentOperation == VinylsEditorOperation.Rotate);
				}
				if ((bool)_editorUI.scaleIcon)
				{
					_editorUI.scaleIcon.gameObject.SetActive(CurrentOperation == VinylsEditorOperation.Scale);
				}
				if ((bool)_editorUI.posIcon)
				{
					_editorUI.posIcon.gameObject.SetActive(CurrentOperation == VinylsEditorOperation.Move);
				}
			}
		}

		private void ChangeVinylColor(Vector2 colorChange, float opacityChange)
		{
			float deltaTime = Time.deltaTime;
			if (Mathf.Abs(opacityChange) > 0.001f)
			{
				Color color = (_currentVinylRenderer.enabled ? _currentVinylRenderer.color : _currentTextMesh.color);
				color.a = Mathf.Clamp01(color.a + opacityChange * 1f * deltaTime);
				ChangeColor(color, onlyAlpha: true);
				alphaSlider.SetValueWithoutNotify(color.a);
			}
			if (colorChange.sqrMagnitude > 0.0001f)
			{
				Color rgbColor = (_currentVinylRenderer.enabled ? _currentVinylRenderer.color : _currentTextMesh.color);
				Color.RGBToHSV(rgbColor, out var H, out var S, out var V);
				if (_isColorChangeActive)
				{
					H = Mathf.Repeat(H - colorChange.x * 0.5f * deltaTime, 1f);
				}
				else
				{
					S = Mathf.Clamp01(S + colorChange.x * 0.5f * deltaTime);
					V = Mathf.Clamp01(V + colorChange.y * 0.5f * deltaTime);
				}
				Color color2 = Color.HSVToRGB(H, S, V);
				color2.a = rgbColor.a;
				colorPicker.SetColor(color2, withAction: true);
			}
		}

		private void UpdateCurrentVinylWithAlterInput()
		{
			if (!CurrentVinyl || !_currentSticker || _isPointerDown)
			{
				return;
			}
			Transform editTransform = (IsGrouping ? _groupParent : CurrentVinyl.transform);
			Vector2 value = _positionChangeHandle.Get(_eventRegistry).Value;
			VinylsEditorInputState currentState = CurrentState;
			if (currentState == VinylsEditorInputState.OneSelected || currentState == VinylsEditorInputState.MultiSelected)
			{
				MoveVinylPosition(value, editTransform);
			}
			Vector2 value2 = _sizeChangeHandle.Get(_eventRegistry).Value;
			float num = _turnLeftRightHandle.Get(_eventRegistry).Value;
			if (_detector != null && _detector.CurrentDeviceType != InputDeviceType.KeyboardMouse)
			{
				num *= 0.4f;
			}
			if (_rotateNeedsRelease)
			{
				bool flag = Mathf.Abs(num) > 0.01f;
				if (flag)
				{
					_rotateSeenHeld = true;
				}
				bool num2 = _rotateSeenHeld && !flag;
				bool flag2 = !_rotateSeenHeld && Time.frameCount - _sizeEnterFrame > 10;
				if (num2 | flag2)
				{
					_rotateNeedsRelease = false;
				}
				num = 0f;
			}
			if (CurrentState == VinylsEditorInputState.SizeChange)
			{
				ChangeVinylTransform(value2, num, editTransform);
			}
			float value3 = _opacityChangeHandle.Get(_eventRegistry).Value;
			Vector2 value4 = _colorChangeHandle.Get(_eventRegistry).Value;
			if (CurrentState == VinylsEditorInputState.ColorChange)
			{
				ChangeVinylColor(value4, value3);
			}
		}

		private Vector3 GetPointerScreenPosition()
		{
			if (Pointer.current == null)
			{
				return new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
			}
			return Pointer.current.position.ReadValue();
		}

		private void InitUIParts()
		{
			ChangeUIStateButton.onClick.AddListener(ChangeUIState);
			NextTypeButton.onClick.AddListener(NextVinylsType);
			PrevTypeButton.onClick.AddListener(PrevVinylsType);
			ShowPlatesButton.onClick.AddListener(ShowPlatesScroll);
			FrontPlateToggle.onValueChanged.AddListener(OnFrontPlateToggle);
			RearPlateToggle.onValueChanged.AddListener(OnRearPlateToggle);
			CreateStickersToSelect();
			CreatePlatesToSelect();
			ShowAllVinyls();
			_modules.Clear();
			_modules.Add(new VinylsEditorModuleSaveConvexMeshCollider(this));
			_modules.Add(new VinylsEditorModuleDestroyNewMeshCollider(this));
			_lastFrontPlateOwner = string.Empty;
			_lastRarePlateOwner = string.Empty;
		}

		private void CreateStickersToSelect()
		{
			int num = 0;
			foreach (Transform parentOfSticker in ParentOfStickers)
			{
				UnityEngine.Object.Destroy(parentOfSticker.gameObject);
			}
			foreach (Transform parentOfFavouriteSticker in ParentOfFavouriteStickers)
			{
				UnityEngine.Object.Destroy(parentOfFavouriteSticker.gameObject);
			}
			OneVinylData[] allVinyls = V.AllVinyls;
			foreach (OneVinylData vinylData in allVinyls)
			{
				CreateInitVinyl(num, vinylData, ParentOfStickers);
				CreateVinylForFavourite(num, vinylData);
				num++;
			}
			FavouriteVinylsController.CurrentFavouriteCount = ParentOfFavouriteStickers.childCount;
			VinylsScrollOcclusion.Init();
		}

		private VynilStickerUI CreateVinylForFavourite(int count, OneVinylData vinylData)
		{
			VynilStickerUI result = null;
			if (FavouriteVinylsController.IsItemFavourite(count))
			{
				result = CreateInitVinyl(count, vinylData, ParentOfFavouriteStickers);
			}
			return result;
		}

		private VynilStickerUI CreateInitVinyl(int count, OneVinylData vinylData, Transform parent)
		{
			GameObject obj = _prefabFactory.Instantiate(StickerSelectPrefab, parent);
			obj.name = count.ToString();
			VynilStickerUI component = obj.GetComponent<VynilStickerUI>();
			component.Init(this);
			component.Index = count;
			component.SetSprite(vinylData.sprite);
			component.SetText(vinylData.price.ToString());
			return component;
		}

		private void CreatePlatesToSelect()
		{
			foreach (Transform item in ParentOfPlates.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
			for (int i = 0; i < 6; i++)
			{
				Transform obj = _prefabFactory.Instantiate(PlateSelectPrefab, ParentOfPlates).transform;
				obj.name = i.ToString();
				VynilStickerUI component = obj.GetComponent<VynilStickerUI>();
				component.Init(this);
				component.Index = i;
				component.SetText((i + 1).ToString());
			}
			ParentOfPlates.gameObject.SetActive(value: false);
		}

		private bool IsDelayedNextPrevVinylsTypeClick()
		{
			if (!_delayedNextPrevVinylsTypeClick)
			{
				_delayedNextPrevVinylsTypeClick = true;
				Observable.Timer(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
				{
					_delayedNextPrevVinylsTypeClick = false;
				}).AddTo(this);
				return false;
			}
			return true;
		}

		private void NextVinylsType()
		{
			if (!IsDelayedNextPrevVinylsTypeClick() && !_isChangingRow)
			{
				_currentRowSelected = _currentRowSelected switch
				{
					0 => 2, 
					2 => 1, 
					_ => 0, 
				};
				ChangeRow(_currentRowSelected);
			}
		}

		private void PrevVinylsType()
		{
			if (!IsDelayedNextPrevVinylsTypeClick() && !_isChangingRow)
			{
				_currentRowSelected = _currentRowSelected switch
				{
					1 => 2, 
					2 => 0, 
					_ => 1, 
				};
				ChangeRow(_currentRowSelected);
			}
		}

		private void ChangeRow(int row)
		{
			if (!_isChangingRow)
			{
				_isChangingRow = true;
				_currentRowSelected = row;
				TryShowVinylsSwitchPanel();
				FavouriteVinylsController.ResetToAllView();
				UpdateRow().Forget();
			}
		}

		private async UniTask UpdateRow()
		{
			if (_prevRowSelected == 0 && _currentRowSelected != 0)
			{
				DeleteStickers();
				CleanStuff(isCompletely: false);
				await UniTask.DelayFrame(2);
				_vinylDrawer.DeleteVinylContext(VinylsType.CarVinyls);
			}
			if (_currentRowSelected != 1 && _prevRowSelected == 1)
			{
				ShowPlatesButton.SetActive(active: false);
				_vinylDrawer.SetCamState(isActive: false);
				CleanStuff(isCompletely: false);
				DeleteStickers();
				_vinylDrawer.DeleteVinylContext(VinylsType.PlateVinyls);
				await SetAndRenderPlates();
				if ((bool)PlatesUI)
				{
					PlatesUI.SetActive(value: false);
				}
				ParentOfStickers.gameObject.SetActive(value: true);
				ParentOfPlates.gameObject.SetActive(value: false);
				_plateCreatorTableTr.gameObject.SetActive(value: false);
			}
			if (_currentRowSelected != 2 && _prevRowSelected == 2)
			{
				DeleteStickers();
				CleanStuff(isCompletely: false);
				await UniTask.DelayFrame(2);
				_vinylDrawer.DeleteVinylContext(VinylsType.WindowVinyls);
			}
			_prevRowSelected = _currentRowSelected;
			ParentOfStickers.gameObject.SetActive(value: true);
			if (_currentRowSelected == 0)
			{
				CurrentModeLabel.text = new LocalizedString("Visualtuningwindow.Vynils");
				RenderTexture renderTexture = _vehicleData.VinylsComposition.RenderTexture;
				Material carMat = _vehicleData.CarMat;
				RenderTexture mainTexture = (_vehicleData.VinylsComposition.RenderTexture = _vinylDrawer.GetRenderTexture(VinylsType.CarVinyls));
				carMat.mainTexture = mainTexture;
				Init(VinylsType.CarVinyls);
				SetActiveScroll(enable: true);
				_vinylDrawer.FocusOnVinyls(VinylsType.CarVinyls);
				if (renderTexture != _vehicleData.VinylsComposition.RenderTexture)
				{
					CommonExtensions.DeleteRenderTex(renderTexture);
				}
			}
			else if (_currentRowSelected == 1)
			{
				CurrentModeLabel.text = new LocalizedString("VinylsEditor.PlateVinyls");
				ShowPlatesButton.SetActive(active: true);
				if ((bool)PlatesUI)
				{
					PlatesUI.SetActive(value: true);
				}
				if (_plateCreatorTableTr == null)
				{
					_plateCreatorTableTr = GameObject.Find("Plate Creator Table").transform;
				}
				_plateCreatorTableTr.gameObject.SetActive(value: true);
				if (_currPlateNum > 0)
				{
					if (ParentOfPlates.childCount > 0)
					{
						Transform child = ParentOfPlates.GetChild(_currPlateNum - 1);
						if ((bool)child)
						{
							ChangePlate(child.GetComponent<VynilStickerUI>());
						}
					}
					else
					{
						GameObject obj = Plates[(_currPlateNum - 1) % 2];
						obj.SetActive(value: true);
						Renderer component = obj.GetComponent<Renderer>();
						_plateMat = component.material;
						_vinylDrawer.ChangePlaneState(OriginPlateMaterial.mainTexture, Color.white);
					}
				}
			}
			else if (_currentRowSelected == 2)
			{
				CurrentModeLabel.text = new LocalizedString("VinylsEditor.WindowVinyls");
				RenderTexture windowRT = _vehicleData.WindowComposition.WindowRT;
				_vehicleData.WindowComposition.WindowRT = _vinylDrawer.CreateVinylContext(VinylsType.WindowVinyls);
				_vehicleData.WindowComposition.WindowMat.mainTexture = _vehicleData.WindowComposition.WindowRT;
				_vinylDrawer.ChangePlaneState(null, Color.clear);
				Init(VinylsType.WindowVinyls);
				SetActiveScroll(enable: true);
				_vinylDrawer.FocusOnVinyls(VinylsType.WindowVinyls);
				if (windowRT != _vehicleData.WindowComposition.WindowRT)
				{
					CommonExtensions.DeleteRenderTex(windowRT);
				}
			}
			_isChangingRow = false;
			SetEditorStateWithCurrentRow();
		}

		public void ShowPlatesScroll()
		{
			ParentOfStickers.gameObject.SetActive(value: false);
			ParentOfFavouriteStickers.gameObject.SetActive(value: false);
			if (ParentOfPlates.gameObject.activeSelf)
			{
				CurrentModeLabel.text = new LocalizedString("VinylsEditor.PlateVinyls");
				TryShowVinylsSwitchPanel();
				if (FavouriteVinylsController.IsShowAll)
				{
					ShowAllVinyls();
				}
				else
				{
					ShowFavouriteVinyls();
				}
			}
			else
			{
				CurrentModeLabel.text = new LocalizedString("VinylsEditor.NumberPlates");
				AllVinylsBar.value = 0f;
				ShowVinylsSwitchPanel(isActive: false);
			}
			bool activeSelf = ParentOfPlates.gameObject.activeSelf;
			ParentOfPlates.gameObject.SetActive(!activeSelf);
			if (!activeSelf)
			{
				CurrentState = VinylsEditorInputState.PlateTypes;
			}
			else
			{
				SetPreviousInputGroup();
			}
		}

		private void OnFrontPlateToggle(bool isOn)
		{
			TrySaveLastPlateOwner(ref _lastFrontPlateOwner, isFrontPlate: true);
			PlateDataUtility.UpdatePlateData(_currPlateNum, isOn ? _vehicleData.carInfo.CarInstanceId : _lastFrontPlateOwner, isFrontPlate: true);
			if (isOn)
			{
				_lastToggledPlate = PlatesType.Front;
			}
			else if (RearPlateToggle.isOn)
			{
				_lastToggledPlate = PlatesType.Rear;
			}
			else
			{
				_lastToggledPlate = PlatesType.None;
			}
		}

		private void OnRearPlateToggle(bool isOn)
		{
			TrySaveLastPlateOwner(ref _lastRarePlateOwner, isFrontPlate: false);
			PlateDataUtility.UpdatePlateData(_currPlateNum, isOn ? _vehicleData.carInfo.CarInstanceId : _lastRarePlateOwner);
			if (isOn)
			{
				_lastToggledPlate = PlatesType.Rear;
			}
			else if (FrontPlateToggle.isOn)
			{
				_lastToggledPlate = PlatesType.Front;
			}
			else
			{
				_lastToggledPlate = PlatesType.None;
			}
		}

		private void TrySaveLastPlateOwner(ref string lastFrontPlateOwner, bool isFrontPlate)
		{
			if (PlateDataUtility.TryGetCarUidByPlateId(_currPlateNum, out var carUid, isFrontPlate) && !(isFrontPlate ? RearPlateToggle : FrontPlateToggle).isOn && carUid != _vehicleData.carInfo.CarInstanceId)
			{
				lastFrontPlateOwner = carUid;
			}
		}

		private void OnVinylsMenuOpen(bool openFromUI)
		{
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 3f);
			_isExiting = false;
			_freeDriveProvider.CarCameras.IsRotating = true;
			FlagsStorage.OnExteriorMenu = true;
			_vehicleData = V.Car.GetComponent<VehicleData>();
			CommonExtensions.DeleteRenderTex(_vehicleData.VinylsComposition.RenderTexture);
			Material carMat = _vehicleData.CarMat;
			RenderTexture mainTexture = (_vehicleData.VinylsComposition.RenderTexture = _vinylDrawer.GetRenderTexture(VinylsType.CarVinyls));
			carMat.mainTexture = mainTexture;
			PrepareBody(_vehicleData).Forget();
			base.enabled = true;
			ChangeRow(0);
			SetEditorStateWithCurrentRow();
		}

		private async UniTaskVoid PrepareBody(VehicleData vehicleData)
		{
			await UniTask.Delay(2000);
			if (_isExiting)
			{
				return;
			}
			for (int i = 0; i < _modules.Count; i++)
			{
				_modules[i].PrepareBodyStarted(vehicleData);
			}
			_garageController.EnableCarPhysics(vehicleData, isEnabled: false);
			vehicleData.CarBodyReferences.Body.transform.parent = null;
			if ((bool)vehicleData.CarBodyReferences && (bool)vehicleData.CarBodyReferences.Body)
			{
				if (!vehicleData.CarBodyReferences.Body.GetComponent<MeshCollider>())
				{
					vehicleData.CarBodyReferences.Body.gameObject.AddComponent<MeshCollider>();
					vehicleData.CarBodyReferences.Body.gameObject.layer = 27;
					BodyMarker component = vehicleData.MainModel.GetComponent<BodyMarker>();
					if ((bool)component)
					{
						if ((bool)component.frontLightLeft && (bool)component.frontLightRight)
						{
							if (component.frontLightLeft.TryGetComponent<MeshRenderer>(out var component2))
							{
								component2.material = vehicleData.CarMat;
								component.frontLightLeft.AddComponent<MeshCollider>();
							}
							if (component.frontLightRight.TryGetComponent<MeshRenderer>(out var component3))
							{
								component3.material = vehicleData.CarMat;
								component.frontLightRight.AddComponent<MeshCollider>();
							}
							_initialParentBodyParts[component.frontLightLeft] = component.frontLightLeft.transform.parent;
							_initialParentBodyParts[component.frontLightRight] = component.frontLightRight.transform.parent;
							component.frontLightLeft.transform.parent = vehicleData.CarBodyReferences.Body.transform;
							component.frontLightRight.transform.parent = vehicleData.CarBodyReferences.Body.transform;
							component.frontLightLeft.layer = 27;
							component.frontLightRight.layer = 27;
						}
						if ((bool)component.hood)
						{
							if (component.hood.TryGetComponent<Animator>(out var component4))
							{
								component4.enabled = false;
							}
							if (component.hood.TryGetComponent<MeshRenderer>(out var component5))
							{
								component5.material = vehicleData.CarMat;
								component.hood.AddComponent<MeshCollider>();
							}
							_initialParentBodyParts[component.hood] = component.hood.transform.parent;
							component.hood.transform.parent = vehicleData.CarBodyReferences.Body.transform;
							component.hood.layer = 27;
						}
						GameObject[] additionalElements = component.AdditionalElements;
						foreach (GameObject gameObject in additionalElements)
						{
							if ((bool)gameObject && gameObject.TryGetComponent<MeshRenderer>(out var component6))
							{
								component6.sharedMaterial = vehicleData.CarMat;
								gameObject.layer = 27;
								_initialParentBodyParts[gameObject] = gameObject.transform.parent;
								gameObject.transform.parent = vehicleData.CarBodyReferences.Body.transform;
								if (!gameObject.TryGetComponent<MeshCollider>(out var _))
								{
									gameObject.AddComponent<MeshCollider>();
								}
							}
						}
					}
				}
				vehicleData.CarBodyReferences.Body.GetComponent<MeshCollider>().enabled = true;
				BodyKits component8 = V.Car.GetComponent<BodyKits>();
				if ((bool)component8)
				{
					if ((bool)component8.InstalledFrontBumper)
					{
						_initialParentBodyParts[component8.InstalledFrontBumper] = component8.InstalledFrontBumper.transform.parent;
					}
					component8.SetForVinylPlace(component8.InstalledFrontBumper);
					if ((bool)component8.InstalledRearBumper)
					{
						_initialParentBodyParts[component8.InstalledRearBumper] = component8.InstalledRearBumper.transform.parent;
					}
					component8.SetForVinylPlace(component8.InstalledRearBumper);
					if ((bool)component8.installedHood)
					{
						_initialParentBodyParts[component8.installedHood] = component8.installedHood.transform.parent;
					}
					component8.SetForVinylPlace(component8.installedHood);
					if ((bool)component8.installedFender)
					{
						_initialParentBodyParts[component8.installedFender] = component8.installedFender.transform.parent;
					}
					component8.SetForVinylPlace(component8.installedFender);
					if ((bool)component8.installedSkirts)
					{
						_initialParentBodyParts[component8.installedSkirts] = component8.installedSkirts.transform.parent;
					}
					component8.SetForVinylPlace(component8.installedSkirts);
				}
			}
			for (int k = 0; k < _modules.Count; k++)
			{
				_modules[k].PrepareBodyCompleted(vehicleData);
			}
		}

		public void ChangePlate(VynilStickerUI plateUI)
		{
			if (!_changingPlate)
			{
				_changingPlate = true;
				Plates[0].SetActive(value: false);
				Plates[1].SetActive(value: false);
				if ((bool)_lastSelectedPlate)
				{
					_lastSelectedPlate.Select(isOn: false);
				}
				int num = int.Parse(plateUI.name);
				_lastSelectedPlate = plateUI;
				_lastSelectedPlate.Select(isOn: true);
				_currentPlateIndex = (_currPlateNum = ++num);
				OnPlateChanged().Forget();
			}
		}

		private async UniTaskVoid OnPlateChanged()
		{
			CleanStuff(isCompletely: false, withToggleVinylsPanel: false);
			bool state = _vinylDrawer.Camera.enabled;
			_vinylDrawer.Camera.enabled = false;
			await UniTask.Delay(TimeSpan.FromMilliseconds(50.0));
			_vinylDrawer.DeleteVinylContext(VinylsType.PlateVinyls);
			DeleteStickers();
			if (_plateMat != null)
			{
				_plateMat.mainTexture = null;
			}
			UnityEngine.Object.Destroy(_plateTempRT);
			_plateTempRT = null;
			await UniTask.Delay(TimeSpan.FromMilliseconds(50.0));
			_vinylDrawer.Camera.enabled = state;
			_plateTempRT = _vinylDrawer.CreateVinylContext(VinylsType.PlateVinyls);
			if (_currPlateNum > 0)
			{
				GameObject obj = Plates[(_currPlateNum - 1) % 2];
				obj.SetActive(value: true);
				Renderer component = obj.GetComponent<Renderer>();
				_plateMat = component.material;
				_vinylDrawer.ChangePlaneState(OriginPlateMaterial.mainTexture, Color.white);
			}
			Init(VinylsType.PlateVinyls);
			if (_plateMat != null)
			{
				_plateMat.mainTexture = _plateTempRT;
			}
			FrontPlateToggle.SetIsOnWithoutNotify(PlateDataUtility.TryGetPlateIdForCar(_vehicleData.carInfo.CarInstanceId, isFrontPlate: true) == _currPlateNum);
			RearPlateToggle.SetIsOnWithoutNotify(PlateDataUtility.TryGetPlateIdForCar(_vehicleData.carInfo.CarInstanceId) == _currPlateNum);
			if (FrontPlateToggle.isOn && RearPlateToggle.isOn)
			{
				_lastToggledPlate = PlatesType.None;
			}
			_changingPlate = false;
		}

		private async UniTask SetAndRenderPlates()
		{
			_vehicleData.VinylsComposition.DeletePlates();
			await CheckAndCleanContainer();
			int num = PlateDataUtility.TryGetPlateIdForCar(_vehicleData.carInfo.CarInstanceId, isFrontPlate: true);
			if (num != 0)
			{
				PlateConstructor.InstantiatePlate(isFront: true, num, _vehicleData);
				_vinylDrawer.SetCamState(isActive: false);
				_vehicleData.VinylsComposition.RenderVinyls(VinylsType.PlateVinyls, isFrontPlate: true);
			}
			await CheckAndCleanContainer();
			int num2 = PlateDataUtility.TryGetPlateIdForCar(_vehicleData.carInfo.CarInstanceId);
			if (num2 != 0)
			{
				PlateConstructor.InstantiatePlate(isFront: false, num2, _vehicleData);
				_vinylDrawer.SetCamState(isActive: false);
				_vehicleData.VinylsComposition.RenderVinyls(VinylsType.PlateVinyls, isFrontPlate: false);
			}
			_vinylDrawer.SetCamState(isActive: false);
			await CheckAndCleanContainer();
		}

		private async UniTask CheckAndCleanContainer()
		{
			int attempts = 0;
			while (_vinylDrawer.ContainerChildCount > 0)
			{
				await UniTask.DelayFrame(3);
				attempts++;
				if (attempts > 10)
				{
					_vinylDrawer.ForceCleanContainer();
					await UniTask.DelayFrame(3);
					break;
				}
			}
		}

		public void ShowVinylsWindow(bool openFromUI = true)
		{
			InitBackButton();
			if ((bool)V.Car)
			{
				OnVinylsMenuOpen(openFromUI);
				RenderSettings.ambientLight = AmbientColor;
				V.CoreGameManager.GaragLight.GetComponent<Light>().enabled = false;
			}
		}

		public void CloseVinylsWindow()
		{
			for (int i = 0; i < _modules.Count; i++)
			{
				_modules[i].CloseVinylsWindowStarted();
			}
			_isExiting = true;
			_vehicleData.CarBodyReferences.Body.transform.parent = _vehicleData.MainModel.transform;
			ResetParentsBodyParts();
			if ((bool)_vehicleData.MainModel && _vehicleData.MainModel.TryGetComponent<BodyMarker>(out var component) && (bool)component.hood && component.hood.TryGetComponent<Animator>(out var component2))
			{
				component2.enabled = true;
			}
			if (_menuControl.mainSceneAll.Length != 0)
			{
				PlayerDatas.DisableEnableAll(isEnable: true, _menuControl.mainSceneAll);
			}
			if ((bool)PlatesUI)
			{
				PlatesUI.SetActive(value: false);
			}
			if (_vehicleData.CarBodyReferences.Body.TryGetComponent<MoveBodyOnExterior>(out var component3))
			{
				UnityEngine.Object.Destroy(component3);
			}
			if (_vehicleData.CarBodyReferences.Body.TryGetComponent<MeshCollider>(out var component4))
			{
				UnityEngine.Object.Destroy(component4);
			}
			foreach (Transform item in _vehicleData.CarBodyReferences.Body.transform)
			{
				if (item.TryGetComponent<MeshCollider>(out var component5))
				{
					UnityEngine.Object.Destroy(component5);
				}
			}
			V.CoreGameManager.GaragLight.GetComponent<Light>().enabled = true;
			CleanStuff(isCompletely: true);
			if (_currentRowSelected == 1)
			{
				SetAndRenderPlates().Forget();
			}
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 1f);
				_garageController.EnableCarPhysics(_vehicleData, isEnabled: true);
			}).AddTo(_vehicleData);
			_prevRowSelected = -1;
			_currentRowSelected = 0;
			AllVinylsBar.value = 0f;
			ParentOfStickers.gameObject.SetActive(value: true);
			ParentOfPlates.gameObject.SetActive(value: false);
			ShowPlatesButton.SetActive(active: false);
			for (int num = 0; num < _modules.Count; num++)
			{
				_modules[num].CloseVinylsWindowCompleted();
			}
		}

		private void ResetParentsBodyParts()
		{
			foreach (KeyValuePair<GameObject, Transform> initialParentBodyPart in _initialParentBodyParts)
			{
				if ((bool)initialParentBodyPart.Key && (bool)initialParentBodyPart.Value)
				{
					initialParentBodyPart.Key.transform.parent = initialParentBodyPart.Value;
				}
			}
		}

		private void ChangeVisibleFreeLook()
		{
			_isVisibleFreeLook = !_isVisibleFreeLook;
			UpdateUIState();
		}

		private void ChangeUIState()
		{
			_isUIEnabled = !_isUIEnabled;
			UpdateUIState();
			if (!_isUIEnabled)
			{
				CurrentState = VinylsEditorInputState.CameraOnly;
			}
			else
			{
				SetPreviousInputGroup();
			}
		}

		private void UpdateUIState()
		{
			PlayerDatas.DisableEnableAll(_isUIEnabled && _isVisibleFreeLook, HideWhen2dEdit);
			ChangeUIStateButton.transform.GetChild(0).rotation = Quaternion.Euler(0f, 0f, (_isUIEnabled && _isVisibleFreeLook) ? (-90) : 90);
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			CloseVinylsWindow();
			base.gameObject.SetActive(value: false);
			VisualPanel.SetActive(value: true);
			VisualTuningWindow.SetFirstTuningType();
			FlagsStorage.OnExteriorMenu = false;
			ResetOnCloseVinylsEditor();
		}
	}
}
