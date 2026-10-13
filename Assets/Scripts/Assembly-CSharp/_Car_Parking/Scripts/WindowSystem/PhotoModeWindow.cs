using System;
using System.Collections.Generic;
using CPMEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PhotoMode;
using _Car_Parking.Scripts.PhotoMode.UI;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhotoModeWindow : WindowScript, IBackableWindow
	{
		public const string ShutterAvailableSingleLocalizedKey = "PhotoMode.ShutterSpeedSingleOnly";

		private const float CaptureVibrateDuration = 0.3f;

		private readonly int _apertureIndex = 1;

		private readonly int _shutterSpeedIndex = 2;

		private readonly int _exposureIndex = 3;

		private readonly float _miniInfoOpenDuration = 0.1f;

		[SerializeField]
		private GameObject PhotoModePanel;

		[SerializeField]
		private Button ResetOptionButton;

		[SerializeField]
		private Button PauseButton;

		[SerializeField]
		private Button CaptureButton;

		[SerializeField]
		private PhotoModePrototype PhotoMode;

		[SerializeField]
		private RawImage ResultDisplay;

		[SerializeField]
		private CanvasGroup ControlPanelCanvasGroup;

		[SerializeField]
		private FloatingJoystick MovingJoystick;

		[SerializeField]
		private ZoomPhotoModeUI ZoomUI;

		[SerializeField]
		private ClickDetector ZoomClickDetector;

		[SerializeField]
		private ClickDetector CircleClickDetector;

		[SerializeField]
		private ClickDetector UpButton;

		[SerializeField]
		private ClickDetector DownButton;

		[SerializeField]
		private float VerticalSpeed = 0.5f;

		[SerializeField]
		private Image FocusImage;

		[SerializeField]
		private TouchDetector TouchDetector;

		[SerializeField]
		private GameObject CapturePanel;

		[SerializeField]
		private Button SaveButton;

		[SerializeField]
		private Button DeleteButton;

		[SerializeField]
		private GameObject DeleteConfirmPanel;

		[SerializeField]
		private Button CancelButton;

		[SerializeField]
		private Button ConfirmButton;

		[SerializeField]
		private TextMeshProUGUI CameraOptionsValue;

		[SerializeField]
		private Button GoToExposureButton;

		[SerializeField]
		private Button GoToApertureButton;

		[SerializeField]
		private Button GoToShutterSpeedButton;

		[SerializeField]
		private PhotoModeScroll ApertureScroll;

		[SerializeField]
		private PhotoModeScroll ShutterSpeedScroll;

		[SerializeField]
		private PhotoModeScroll ExposureScroll;

		[SerializeField]
		private Button OpenInfoButton;

		[SerializeField]
		private Button OpenMoreInfoButton;

		[SerializeField]
		private TextMeshProUGUI InfoTitle;

		[SerializeField]
		private PhotoModeInfoPanel OptionsInfoPanel;

		[SerializeField]
		private Transform MiniInfoPanel;

		[SerializeField]
		private Transform MiniInfoArrowIcon;

		[SerializeField]
		private Transform MiniInfoWarningIcon;

		[SerializeField]
		private CanvasGroup MiniInfoPanelCanvasGroup;

		[SerializeField]
		private RectTransform InfoBackground;

		[SerializeField]
		private Image MiniInfoImage;

		[SerializeField]
		private PhotoModeTutorPlayer TutorPlayer;

		[SerializeField]
		private float InfoBackDefaultHeight = 60f;

		[SerializeField]
		private float InfoBackOpenedHeight = 260f;

		[SerializeField]
		private int PanningInfoDelay = 1;

		[SerializeField]
		[Header("Categories")]
		private PhotoModeCategory CameraOptionsCategory;

		[SerializeField]
		private PhotoModeCategory EffectsCategory;

		[SerializeField]
		private PhotoModeCategory CameraPanningCategory;

		[SerializeField]
		private PhotoModeCameraParameter ShutterSpeed;

		[Header("Tutorial video clips")]
		[SerializeField]
		private List<VideoClip> TutorVideoClips = new List<VideoClip>();

		private bool _isInfoShowed;

		private IDisposable _setHeightDisposable;

		private LevelScript _levelScript;

		private PhotoModeConfig _config;

		private ParameterInfo _currentParameter;

		private PhotoModeCategory _currentCategory;

		private PhotoModeCategory _prevCategory;

		private OnlineCollectionsProvider _onlineCollections;

		private CarSellTriggersCollection _carSellTriggersCollection;

		private PhotoModeObserver _photoModeObserver;

		private OptionalDependency<LevelScript> _levelScriptDep;

		private OptionalDependency<CarSellTriggersCollection> _carSellTriggersCollectionDep;

		private OptionalDependency<PhotoModeObserver> _photoModeObserverDep;

		private EventRegistry _eventRegistry;

		private bool _isLookStarted;

		private bool _isFocusChanging;

		private bool _isCameraMoveStarted;

		private bool _isCameraHeightStarted;

		private bool _isCameraTiltStarted;

		private RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

		private RegistryHandle<Vector2Variable> _lookHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.Look);

		private RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

		private RegistryHandle<FloatVariable> _droneCameraHeightHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.DroneCameraHeight);

		private RegistryHandle<Vector2Variable> _droneCameraMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.DroneCameraMove);

		private RegistryHandle<FloatVariable> _droneCameraTiltHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.DroneCameraTilt);

		private RegistryHandle<ScriptableEvent> _onSnapshotHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnSnapshot);

		private RegistryHandle<ScriptableEvent> _onResetDroneCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnResetDroneCamera);

		private RegistryHandle<ScriptableEvent> _onResetFilterHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnResetFilter);

		private RegistryHandle<ScriptableEvent> _onPhotoHelpHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoHelp);

		private RegistryHandle<BoolEvent> _navigatePhotoSettingsHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.NavigatePhotoSettings);

		private RegistryHandle<BoolEvent> _setPhotoSettingsHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.SetPhotoSettings);

		private RegistryHandle<ScriptableEvent> _onPhotoColorHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoColor);

		private RegistryHandle<ScriptableEvent> _onPhotoOpticsHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoOptics);

		private RegistryHandle<ScriptableEvent> _onPhotoCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnPhotoCamera);

		private RegistryHandle<BoolEvent> _onCameraMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraMove);

		private RegistryHandle<BoolEvent> _onCameraHeightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraHeight);

		private RegistryHandle<BoolEvent> _onCameraTiltHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraTilt);

		private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

		private RegistryHandle<ScriptableEvent> _toggleFocusHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleFocus);

		private const float FocusMoveSpeed = 300f;

		private const float VerticalSpeedAlter = 7f;

		private const float ZoomSpeedAlter = 5f;

		private const float GamepadLookYScale = 0.6f;

		private const float VerticalButtonScale = 0.06f;

		private const float VerticalMoveScale = 0.12f;

		private const float MinLookFactor = 0.15f;

		private static readonly Vector2 Multiplier = new Vector2(20f, 12f);

		private const float MoveAlter = 64f;

		private const float GamepadLookScale = 0.8f;

		private DesktopControlsSettingsSO _controlsSettings;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.PhotoMod;

		protected override bool ChangeRootGameInputGroup => true;

		public PhotoModePrototype PhotoModePrototype => PhotoMode;

		[Inject]
		public void Construct(PhotoModeConfig config, OnlineCollectionsProvider onlineCollections, EventRegistry eventRegistry, OptionalDependency<DesktopControlsSettingsSO> settingsDep, OptionalDependency<LevelScript> levelScript, OptionalDependency<CarSellTriggersCollection> carSellTriggersCollection, OptionalDependency<PhotoModeObserver> photoModeObserver)
		{
			_carSellTriggersCollectionDep = carSellTriggersCollection;
			_onlineCollections = onlineCollections;
			_config = config;
			_eventRegistry = eventRegistry;
			if (settingsDep.HasValue)
			{
				_controlsSettings = settingsDep.Value;
			}
			_levelScriptDep = levelScript;
			_photoModeObserverDep = photoModeObserver;
		}

		protected override void Awake()
		{
			base.Awake();
			_levelScript = _levelScriptDep?.Value;
			_carSellTriggersCollection = _carSellTriggersCollectionDep?.Value;
			_photoModeObserver = _photoModeObserverDep?.Value;
		}

		private Vector2 ScaleDroneLook(Vector2 look)
		{
			if (!_controlsSettings)
			{
				return look;
			}
			Vector2 lookScale = _controlsSettings.GetLookScale(CameraSettingsContext.Drone);
			look.x *= lookScale.x;
			look.y *= lookScale.y;
			return look;
		}

		private void SubscribeToActions()
		{
			_onLookHandle.Get(_eventRegistry).Subscribe(OnLook);
			_onCameraMoveHandle.Get(_eventRegistry).Subscribe(OnCameraMove);
			_onCameraHeightHandle.Get(_eventRegistry).Subscribe(OnCameraHeight);
			_onCameraTiltHandle.Get(_eventRegistry).Subscribe(OnCameraTilt);
			_onSnapshotHandle.Get(_eventRegistry).Subscribe(Capture);
			_onResetDroneCameraHandle.Get(_eventRegistry).Subscribe(ResetZoom);
			_onResetFilterHandle.Get(_eventRegistry).Subscribe(ResetCurrentParameter);
			_onPhotoHelpHandle.Get(_eventRegistry).Subscribe(OnPhotoHelp);
			_navigatePhotoSettingsHandle.Get(_eventRegistry).Subscribe(OnNavigatePhotoSettings);
			_setPhotoSettingsHandle.Get(_eventRegistry).Subscribe(OnSetPhotoSettings);
			_onPhotoColorHandle.Get(_eventRegistry).Subscribe(ChooseEffects);
			_onPhotoOpticsHandle.Get(_eventRegistry).Subscribe(ChooseOptics);
			_onPhotoCameraHandle.Get(_eventRegistry).Subscribe(ChoosePanning);
			_onMenuHandle.Get(_eventRegistry).Subscribe(OpenPauseWindow);
			_toggleFocusHandle.Get(_eventRegistry).Subscribe(StartChangingFocus);
		}

		private void UnsubscribeToActions()
		{
			_onLookHandle.Get(_eventRegistry)?.Unsubscribe(OnLook);
			_onCameraMoveHandle.Get(_eventRegistry)?.Unsubscribe(OnCameraMove);
			_onCameraHeightHandle.Get(_eventRegistry)?.Unsubscribe(OnCameraHeight);
			_onCameraTiltHandle.Get(_eventRegistry)?.Unsubscribe(OnCameraTilt);
			_onSnapshotHandle.Get(_eventRegistry)?.Unsubscribe(Capture);
			_onResetDroneCameraHandle.Get(_eventRegistry)?.Unsubscribe(ResetZoom);
			_onResetFilterHandle.Get(_eventRegistry)?.Unsubscribe(ResetCurrentParameter);
			_onPhotoHelpHandle.Get(_eventRegistry)?.Unsubscribe(OnPhotoHelp);
			_navigatePhotoSettingsHandle.Get(_eventRegistry)?.Unsubscribe(OnNavigatePhotoSettings);
			_setPhotoSettingsHandle.Get(_eventRegistry)?.Unsubscribe(OnSetPhotoSettings);
			_onPhotoColorHandle.Get(_eventRegistry)?.Unsubscribe(ChooseEffects);
			_onPhotoOpticsHandle.Get(_eventRegistry)?.Unsubscribe(ChooseOptics);
			_onPhotoCameraHandle.Get(_eventRegistry)?.Unsubscribe(ChoosePanning);
			_onMenuHandle.Get(_eventRegistry)?.Unsubscribe(OpenPauseWindow);
			_toggleFocusHandle.Get(_eventRegistry)?.Unsubscribe(StartChangingFocus);
		}

		private void StartChangingFocus()
		{
			_isFocusChanging = !_isFocusChanging;
		}

		private void OnLook(bool isStarted)
		{
			_isLookStarted = isStarted;
		}

		private void OnCameraMove(bool isStarted)
		{
			_isCameraMoveStarted = isStarted;
			PhotoMode.IsJoystickPressed = isStarted;
			if (!isStarted && _currentCategory.CategoryType == PhotoModeCategoryType.Panning)
			{
				OnImageClick(FocusImage.transform.position);
			}
		}

		private void OnCameraHeight(bool isStarted)
		{
			_isCameraHeightStarted = isStarted;
			if (!isStarted)
			{
				PhotoMode.RaiseCamera(0f);
			}
		}

		private void OnCameraTilt(bool isStarted)
		{
			_isCameraTiltStarted = isStarted;
		}

		private void OnPhotoHelp()
		{
			if (!OptionsInfoPanel.gameObject.activeSelf && _currentCategory.CategoryType == PhotoModeCategoryType.Panning)
			{
				OpenMoreInfoPanel();
			}
		}

		private void ResetZoom()
		{
			ZoomUI.SetScrollPosition(60f);
		}

		private void MoveFocusPoint(Vector2 move)
		{
			if (!(move == Vector2.zero))
			{
				Vector3 position = FocusImage.transform.position;
				position += (Vector3)(move * 300f);
				position.x = Mathf.Clamp(position.x, 0f, Screen.width);
				position.y = Mathf.Clamp(position.y, 0f, Screen.height);
				FocusImage.transform.position = position;
			}
		}

		private void UpdateInfoPanelVisibility()
		{
			bool flag = (bool)_currentCategory && _currentCategory.CategoryType == PhotoModeCategoryType.Panning;
			OpenInfoButton.SetActive(flag);
			if (flag)
			{
				if (!_isInfoShowed)
				{
					OpenInfoMini();
				}
			}
			else if (_isInfoShowed)
			{
				_isInfoShowed = false;
				TutorPlayer.StopVideo();
			}
		}

		private void OnNavigatePhotoSettings(bool isNext)
		{
			_currentCategory.NavigateParameter((!isNext) ? 1 : (-1));
		}

		private void OnSetPhotoSettings(bool isIncrement)
		{
			_currentCategory.AdjustCurrentValue(isIncrement ? 1 : (-1));
		}

		private void ChooseEffects()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
			EffectsCategory.SetToggleState(isOn: true);
			CameraOptionsCategory.SetToggleState(isOn: false);
			if (!FlagsStorage.IsMultiFree)
			{
				CameraPanningCategory.SetToggleState(isOn: false);
			}
		}

		private void ChooseOptics()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
			CameraOptionsCategory.SetToggleState(isOn: true);
			EffectsCategory.SetToggleState(isOn: false);
			if (!FlagsStorage.IsMultiFree)
			{
				CameraPanningCategory.SetToggleState(isOn: false);
			}
		}

		private void ChoosePanning()
		{
			if (!FlagsStorage.IsMultiFree)
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
				CameraPanningCategory.SetToggleState(isOn: true);
				CameraOptionsCategory.SetToggleState(isOn: false);
				EffectsCategory.SetToggleState(isOn: false);
			}
		}

		protected override void OnEnable()
		{
			SubscribeToActions();
			base.OnEnable();
		}

		private void Update()
		{
			float num = (0f - _zoomHandle.Get(_eventRegistry).Value) * 5f;
			if (num != 0f)
			{
				float scrollPosition = Mathf.Clamp(PhotoMode.CameraZoom + num, 6f, 100f);
				ZoomUI.SetScrollPosition(scrollPosition);
				OnImageClick(FocusImage.transform.position);
			}
			if (_isLookStarted)
			{
				Vector2 value = _lookHandle.Get(_eventRegistry).Value;
				float value2 = (PhotoMode.CameraZoom - 6f) / 94f;
				value = Mathf.Lerp(1f, 0.15f, Mathf.Clamp01(value2)) * Vector2.Scale(b: (InputDeviceDetector.Instance != null && InputDeviceDetector.Instance.CurrentDeviceType != InputDeviceType.KeyboardMouse) ? new Vector2(0.8f, 0.48000002f) : Vector2.one, a: Vector2.Scale(Multiplier, value));
				value = ScaleDroneLook(value);
				if (value != Vector2.zero)
				{
					PhotoMode.ApplyLookDelta(value);
				}
			}
			if (_isCameraMoveStarted)
			{
				Vector2 value3 = _droneCameraMoveHandle.Get(_eventRegistry).Value;
				if (_isFocusChanging)
				{
					MoveFocusPoint(value3);
				}
				else
				{
					PhotoMode.MoveCamera(value3 * _controlsSettings.GetUniqueSlider(UniqueSliders.DroneMove).Current);
				}
			}
			if (_isCameraHeightStarted)
			{
				float value4 = _droneCameraHeightHandle.Get(_eventRegistry).Value;
				PhotoMode.RaiseCamera(value4 * _controlsSettings.GetUniqueSlider(UniqueSliders.DroneVertical).Current * 0.12f);
			}
			if (_isCameraTiltStarted)
			{
				float value5 = _droneCameraTiltHandle.Get(_eventRegistry).Value;
				if (value5 != 0f)
				{
					PhotoMode.TiltDelta(value5 * _controlsSettings.GetUniqueSlider(UniqueSliders.DroneTilt).Current);
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			_managerUI.PhotoModeWindow = this;
			FlagsStorage.IsPhotoMode = true;
			ActivateParktronik(isActive: false);
			CaptureButton.onClick.AddListener(Capture);
			PauseButton.onClick.AddListener(OpenPauseWindow);
			SaveButton.onClick.AddListener(Save);
			DeleteButton.onClick.AddListener(ShowConfirmPanel);
			CancelButton.onClick.AddListener(ShowCapturePanel);
			ConfirmButton.onClick.AddListener(DeleteCapture);
			ResetOptionButton.onClick.AddListener(ResetCurrentParameter);
			PhotoMode.MovingJoystick = MovingJoystick;
			CircleClickDetector.OnPressed += (bool pressed) =>
			{
				PhotoMode.IsJoystickPressed = pressed;
			};
			UpButton.OnPressed += OnUpButton;
			DownButton.OnPressed += OnDownButton;
			TouchDetector.OnFocusPoint += OnImageClick;
			ExposureScroll.OnValueChanged += (float _) =>
			{
				SetCameraInfo();
			};
			ApertureScroll.OnValueChanged += (float _) =>
			{
				SetCameraInfo();
			};
			ShutterSpeedScroll.OnValueChanged += (float _) =>
			{
				SetCameraInfo();
			};
			OptionsInfoPanel.OnClose += () =>
			{
				ShowPhotoModeControlPanel(show: true);
			};
			PhotoMode.PhotoModeWindow = this;
			PhotoMode.RealtimeMode = FlagsStorage.IsMultiFree;
			PhotoMode.InitCamera();
			PhotoMode.EnterPhotoMode();
			InitCategories();
			GoToExposureButton.onClick.AddListener(ChooseExposure);
			GoToApertureButton.onClick.AddListener(ChooseAperture);
			GoToShutterSpeedButton.onClick.AddListener(ChooseShutterSpeed);
			SetCameraInfo();
			InitInfoPanel();
			UpdateInfoPanelVisibility();
			ShowUI(state: true);
			if (FlagsStorage.IsMultiFree)
			{
				_Car_Parking.Scripts.Audio.Audio.SetPauseSoundFilter(on: true);
			}
			else
			{
				_Car_Parking.Scripts.Audio.Audio.MuteExcludeUI();
			}
			ShowPlayersLabel(show: false);
		}

		public void Back()
		{
			if ((bool)CapturePanel && CapturePanel.activeInHierarchy)
			{
				ShowConfirmPanel();
			}
			else if ((bool)DeleteConfirmPanel && DeleteConfirmPanel.activeInHierarchy)
			{
				ShowCapturePanel();
			}
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			PhotoMode.ExitPhotoMode();
			_Car_Parking.Scripts.Audio.Audio.ResetAllVolume();
			UpButton.OnPressed -= OnUpButton;
			DownButton.OnPressed -= OnDownButton;
			TouchDetector.OnFocusPoint -= OnImageClick;
			PhotoMode.OnCaptureCompleted -= OnCaptureComplete;
			FlagsStorage.IsPhotoMode = false;
			TutorPlayer.StopVideo();
			ActivateParktronik(FlagsStorage.ShowParkingSensors);
			ShowPlayersLabel(show: true);
			_managerUI.PhotoModeWindow = null;
			_photoModeObserver?.OnClosePhotoMode();
		}

		private void OnDisable()
		{
			if ((bool)V.CoreGameManager)
			{
				V.PlayerPointLOD = V.CoreGameManager.transform;
			}
			UnsubscribeToActions();
		}

		private void InitCategories()
		{
			CameraPanningCategory.SetActive(!FlagsStorage.IsMultiFree);
			if (!FlagsStorage.IsMultiFree)
			{
				PhotoModeCategory cameraPanningCategory = CameraPanningCategory;
				cameraPanningCategory.OnParameterSelected = (Action<ParameterInfo>)Delegate.Combine(cameraPanningCategory.OnParameterSelected, (Action<ParameterInfo>)((ParameterInfo parameter) =>
				{
					SetCurrentParameter(parameter, CameraPanningCategory);
				}));
				CameraPanningCategory.Init(PhotoMode, _config);
				CameraPanningCategory.Activate(isActive: false);
			}
			PhotoModeCategory cameraOptionsCategory = CameraOptionsCategory;
			cameraOptionsCategory.OnParameterSelected = (Action<ParameterInfo>)Delegate.Combine(cameraOptionsCategory.OnParameterSelected, (Action<ParameterInfo>)((ParameterInfo parameter) =>
			{
				SetCurrentParameter(parameter, CameraOptionsCategory);
			}));
			PhotoModeCategory effectsCategory = EffectsCategory;
			effectsCategory.OnParameterSelected = (Action<ParameterInfo>)Delegate.Combine(effectsCategory.OnParameterSelected, (Action<ParameterInfo>)((ParameterInfo parameter) =>
			{
				SetCurrentParameter(parameter, EffectsCategory);
			}));
			EffectsCategory.Init(PhotoMode, _config);
			CameraOptionsCategory.Init(PhotoMode, _config);
			EffectsCategory.Activate(isActive: false);
			CameraOptionsCategory.Activate(isActive: true);
			CameraOptionsCategory.OnCategoryChosen(isOn: true);
			_currentCategory = CameraOptionsCategory;
		}

		private void SetCurrentParameter(ParameterInfo parameter, PhotoModeCategory category)
		{
			_currentParameter = parameter;
			_prevCategory = _currentCategory;
			_currentCategory = category;
			SetCurrentTutorTitle();
			if (_isInfoShowed)
			{
				SetCurrentCategoryTutor();
			}
			else
			{
				CheckCurrentParameterFirstOpen();
			}
			UpdateInfoPanelVisibility();
		}

		private void SetCurrentTutorTitle()
		{
			string localizationKey = ((_currentCategory.CategoryType == PhotoModeCategoryType.Panning) ? new LocalizedString("PhotoMode.PanningModes") : new LocalizedString("PhotoMode.CameraControls"));
			InfoTitle.text = new LocalizedString(localizationKey);
		}

		private void CheckCurrentParameterFirstOpen()
		{
			if (PlayerPrefs.GetInt("FirstOpenPhotoModeParameter" + _currentParameter.Id, 1) == 1)
			{
				PlayerPrefs.SetInt("FirstOpenPhotoModeParameter" + _currentParameter.Id, 0);
				OpenInfoMini();
			}
		}

		public void ResetResultDisplay()
		{
			ResultDisplay.color = Color.white;
		}

		public void ResetAll()
		{
			SetShutterSpeed(_config.DefaultShutterSpeed);
			SetAperture(_config.DefaultAperture);
			PhotoMode.CameraExposure = _config.DefaultExposure;
			PhotoMode.CameraDistortion = _config.DefaultDistortion;
			PhotoMode.TiltCamera(_config.DefaultCameraTilt);
			PhotoMode.WhiteBallanceTint = _config.DefaultColorCorrection;
			PhotoMode.WhiteBallance = _config.DefaultTemperature;
			PhotoMode.ColorCorrection = (CPMGraphicsSettings.ColorCorrectionLut)_config.DefaultColorCorrection;
			if (!FlagsStorage.IsMultiFree)
			{
				PhotoMode.m_CameraPanning = (CameraPanning)_config.DefaultPanningMode;
				CameraPanningCategory.ResetAll();
			}
			EffectsCategory.ResetAll();
			CameraOptionsCategory.ResetAll();
			ResetResultDisplay();
		}

		private void ShowPlayersLabel(bool show)
		{
			if ((bool)_managerUI && (bool)_managerUI.AppVersion)
			{
				_managerUI.AppVersion.EnableText(show);
			}
			if (!ScenesHelper.IsFreeDriveSceneActive() || !FlagsStorage.IsMultiFree)
			{
				return;
			}
			if (show)
			{
				show = PlayerPrefs.GetInt("hideAllTopLabel", 0) == 0;
			}
			FlagsStorage.HideAllTopLabel = !show;
			if (_onlineCollections != null && _onlineCollections.PlayersOnlineNow != null)
			{
				MultiPersonControl[] playersOnlineNow = _onlineCollections.PlayersOnlineNow;
				foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
				{
					if ((bool)multiPersonControl && (bool)multiPersonControl.topLabel)
					{
						multiPersonControl.topLabel.SetActive(show && !multiPersonControl.currentCar);
					}
				}
			}
			if (_onlineCollections == null || _onlineCollections.CarsOnlineNow == null)
			{
				return;
			}
			MultiPlayerPosControl[] carsOnlineNow = _onlineCollections.CarsOnlineNow;
			foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
			{
				if ((bool)multiPlayerPosControl && (bool)multiPlayerPosControl.VehicleData && multiPlayerPosControl.VehicleData.VehicleMembersController.PlacesCount > 0 && (!V.Car || !(V.Car == multiPlayerPosControl.gameObject)))
				{
					multiPlayerPosControl.updateLabel(multiPlayerPosControl.VehicleData.Driver, show, !show);
				}
			}
		}

		private void ResetCurrentParameter()
		{
			_currentCategory.ResetCurrentParameter();
		}

		private void SetCameraInfo()
		{
			string text = (FlagsStorage.IsMultiFree ? ("<color=#808080>" + ShutterSpeedScroll.CurrentText + "</color>") : ShutterSpeedScroll.CurrentText);
			CameraOptionsValue.text = "EV " + ExposureScroll.CurrentText + "          " + ApertureScroll.CurrentText + "          " + text;
		}

		private void ChooseAperture()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
			CameraOptionsCategory.ChooseParameter(_apertureIndex);
			SetCurrentParameter(CameraOptionsCategory.CurrentInfo, CameraOptionsCategory);
			EffectsCategory.SetToggleState(isOn: false);
			if (!FlagsStorage.IsMultiFree)
			{
				CameraPanningCategory.SetToggleState(isOn: false);
			}
		}

		private void ChooseShutterSpeed()
		{
			if (FlagsStorage.IsMultiFree)
			{
				if ((bool)V.CoreGameManager)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("PhotoMode.ShutterSpeedSingleOnly"), 2f);
				}
				return;
			}
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
			CameraOptionsCategory.ChooseParameter(_shutterSpeedIndex);
			SetCurrentParameter(CameraOptionsCategory.CurrentInfo, CameraOptionsCategory);
			EffectsCategory.SetToggleState(isOn: false);
			if (!FlagsStorage.IsMultiFree)
			{
				CameraPanningCategory.SetToggleState(isOn: false);
			}
		}

		private void ChooseExposure()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeSettingsSwitch, base.gameObject);
			EffectsCategory.ChooseParameter(_exposureIndex);
			SetCurrentParameter(EffectsCategory.CurrentInfo, EffectsCategory);
			CameraOptionsCategory.SetToggleState(isOn: false);
			if (!FlagsStorage.IsMultiFree)
			{
				CameraPanningCategory.SetToggleState(isOn: false);
			}
		}

		private void OpenPauseWindow()
		{
			FlagsStorage.IsInputEnabledInThisWindow = false;
			_managerUI.ShowPhotoModePauseWindow();
			if ((bool)_levelScript)
			{
				_levelScript.CleanScreen();
			}
			ShowPhotoModeControlPanel(show: false);
		}

		private void OnUpButton(bool isPressed)
		{
			SetCameraHeight(isPressed, VerticalSpeed * 0.06f);
		}

		private void OnDownButton(bool isPressed)
		{
			SetCameraHeight(isPressed, (0f - VerticalSpeed) * 0.06f);
		}

		private void SetCameraHeight(bool isPressed, float speed)
		{
			if (isPressed)
			{
				_setHeightDisposable = Observable.EveryUpdate().Subscribe((long _) =>
				{
					PhotoMode.RaiseCamera(speed);
				}).AddTo(this);
			}
			else
			{
				_setHeightDisposable.Dispose();
				PhotoMode.RaiseCamera(0f);
			}
		}

		public void SetAperture(float val)
		{
			int num = Mathf.RoundToInt(val);
			PhotoMode.Aperture = _config.Apertures[num];
		}

		public void SetShutterSpeed(float val)
		{
			int num = Mathf.RoundToInt(val);
			PhotoMode.m_ShutterSpeed = _config.ShutterSpeeds[num];
		}

		public void Save()
		{
			PhotoMode.SaveScreenshot();
			ShowPhotoModePanel();
			BackToPhotoMode();
		}

		public void Exit()
		{
			PhotoMode.ExitPhotoMode();
		}

		public void BackToPhotoMode()
		{
			PhotoMode.ResetCameraSettings();
			InputMapSetEvent.TrySetGroup(WindowInputGroup);
			UiInputSetup.DisableUiNavigation(CapturePanel);
			UiInputSetup.DisableUiNavigation(DeleteConfirmPanel);
		}

		public void OnImageClick(Vector2 pos)
		{
			FocusImage.transform.position = pos;
			FocusImageAnimation();
			PhotoMode.CastFocusPoint(pos);
		}

		private void FocusImageAnimation()
		{
			FocusImage.transform.DOKill();
			FocusImage.transform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true)
				.OnComplete(() =>
				{
					FocusImage.transform.DOScale(1f, 0.1f).SetUpdate(isIndependentUpdate: true);
				});
		}

		private void Capture()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeShutterSwitch, base.gameObject);
			TutorPlayer.StopVideo();
			PhotoModePanel.SetActive(value: false);
			_managerUI.WaitingWindowText = new LocalizedString("PhotoMode.Capturing");
			_managerUI.ShowWaitingWindow(set: true).Forget();
			PhotoMode.OnCaptureCompleted += OnCaptureComplete;
			PhotoMode.Capture();
			Utils.Vibrate(0.3f, 0.1f, unscaledTime: true);
		}

		private void OnCaptureComplete()
		{
			PhotoMode.OnCaptureCompleted -= OnCaptureComplete;
			_managerUI.CloseWaitingWindow();
			ShowCapturePanel();
		}

		public void SetDisplayTexture(Texture tex)
		{
			ResultDisplay.texture = tex;
		}

		public void SetDisplayColor(Color col)
		{
			ResultDisplay.material.SetColor("_Color", col);
		}

		private void ShowConfirmPanel()
		{
			CapturePanel.SetActive(value: false);
			DeleteConfirmPanel.SetActive(value: true);
			InitBackButton();
		}

		private void ShowCapturePanel()
		{
			CapturePanel.SetActive(value: true);
			DeleteConfirmPanel.SetActive(value: false);
			PhotoModePanel.SetActive(value: false);
			InputMapSetEvent.TrySetGroup(InputGroupsEnum.Ui);
			UiInputSetup.EnableUiNavigation(CapturePanel);
			UiInputSetup.EnableUiNavigation(DeleteConfirmPanel);
			InitBackButton();
		}

		private void ShowPhotoModePanel()
		{
			CapturePanel.SetActive(value: false);
			DeleteConfirmPanel.SetActive(value: false);
			PhotoModePanel.SetActive(value: true);
			TouchDetector.ResetTouchCount();
			if (_isInfoShowed)
			{
				SetCurrentCategoryTutor(forceShow: true);
			}
		}

		private void DeleteCapture()
		{
			ShowPhotoModePanel();
			BackToPhotoMode();
		}

		private void ActivateParktronik(bool isActive)
		{
			if ((bool)V.Car)
			{
				VehicleData component = V.Car.GetComponent<VehicleData>();
				if ((bool)component && component.ParkTronikComposition != null)
				{
					component.ParkTronikComposition.TrySetActiveParktronikParent(isActive);
				}
			}
		}

		public void ShowPhotoModeControlPanel(bool show)
		{
			ControlPanelCanvasGroup.blocksRaycasts = show;
			ControlPanelCanvasGroup.alpha = (show ? 1 : 0);
			if (show)
			{
				if (_isInfoShowed)
				{
					SetCurrentCategoryTutor(forceShow: true);
				}
				InputMapSetEvent.TrySetRootGroup();
			}
			else if (_currentCategory.CategoryType != PhotoModeCategoryType.Panning)
			{
				TutorPlayer.StopVideo();
			}
		}

		private void CheckFirstOpen()
		{
			if (PlayerPrefs.GetInt("PhotoModeFirstOpen", 1) == 1)
			{
				PlayerPrefs.SetInt("PhotoModeFirstOpen", 0);
				OpenInfoMini();
			}
		}

		private void InitInfoPanel()
		{
			MiniInfoPanel.localScale = new Vector3(1f, 0f, 1f);
			MiniInfoPanelCanvasGroup.alpha = 0f;
			MiniInfoPanel.gameObject.SetActive(value: false);
			OpenInfoButton.onClick.AddListener(() =>
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeInfoSwitch, base.gameObject);
				if (!_isInfoShowed)
				{
					OpenInfoMini();
				}
				else
				{
					CloseInfoMini();
				}
			});
			OpenMoreInfoButton.onClick.AddListener(OpenMoreInfoPanel);
			CheckFirstOpen();
		}

		private void OpenMoreInfoPanel()
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.PhotoModeInfoSwitch, base.gameObject);
			InputMapSetEvent.TrySetGroup(InputGroupsEnum.Ui);
			if (!OptionsInfoPanel._managerUI)
			{
				OptionsInfoPanel._managerUI = _managerUI;
			}
			OptionsInfoPanel.Open(TutorVideoClips, _currentCategory.CategoryType);
			ShowPhotoModeControlPanel(show: false);
		}

		private void OpenInfoMini()
		{
			MiniInfoPanel.gameObject.SetActive(value: true);
			_isInfoShowed = true;
			SetCurrentCategoryTutor();
			AnimatedOpenMiniInfo(isOpening: true);
		}

		private void SetCurrentCategoryTutor(bool forceShow = false)
		{
			if (_currentCategory.CategoryType == PhotoModeCategoryType.Panning)
			{
				TutorPlayer.StopVideo();
				MiniInfoImage.SetActive(active: true);
				SetCurrentPanningImage();
			}
		}

		private void CloseInfoMini()
		{
			_isInfoShowed = false;
			TutorPlayer.StopVideo();
			AnimatedOpenMiniInfo(isOpening: false);
		}

		private void SetCurrentPanningImage()
		{
			Sprite sprite = _config.PanningTutorSprites[_currentParameter.Id];
			MiniInfoImage.sprite = sprite;
		}

		private void AnimatedOpenMiniInfo(bool isOpening)
		{
			float y = (isOpening ? InfoBackOpenedHeight : InfoBackDefaultHeight);
			InfoBackground.DOKill();
			InfoBackground.DOSizeDelta(new Vector2(0f, y), _miniInfoOpenDuration).SetUpdate(isIndependentUpdate: true);
			float endValue = (isOpening ? 1 : 0);
			float endValue2 = (isOpening ? 1 : 0);
			MiniInfoPanel.DOKill();
			MiniInfoPanel.DOScaleY(endValue, _miniInfoOpenDuration).SetUpdate(isIndependentUpdate: true);
			MiniInfoPanelCanvasGroup.DOKill();
			MiniInfoPanelCanvasGroup.DOFade(endValue2, _miniInfoOpenDuration).SetUpdate(isIndependentUpdate: true);
			float z = (isOpening ? 90 : 270);
			MiniInfoArrowIcon.DOKill();
			MiniInfoArrowIcon.DORotate(new Vector3(0f, 0f, z), _miniInfoOpenDuration).SetUpdate(isIndependentUpdate: true);
		}
	}
}
