using System;
using Services.Drone;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drone;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam.InputHints;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class DroneControlWindow : WindowScript
	{
		[Header("Elements to hide")]
		[SerializeField]
		private CanvasGroup MainCanvasGroup;

		[Header("Joysticks")]
		[SerializeField]
		private FixedJoystick CircleJoystick;

		[SerializeField]
		private FixedJoystick ZoomJoystick;

		[SerializeField]
		private FixedJoystick VerticalJoystick;

		[SerializeField]
		private FixedJoystick DiagonalJoystick;

		[Header("Buttons")]
		[SerializeField]
		private Button SettingsButton;

		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private Button ScreenshotButton;

		[SerializeField]
		private Button HideUIButton;

		[SerializeField]
		private Button FocusButton;

		[SerializeField]
		private Button SportModeButton;

		[SerializeField]
		private Button ShowBigMapButton;

		[SerializeField]
		private Button ResetRotationButton;

		[Header("Joystick click detectors")]
		[SerializeField]
		private ClickDetector CircleClickDetector;

		[SerializeField]
		private ClickDetector VerticalClickDetector;

		[SerializeField]
		private ClickDetector ZoomClickDetector;

		[SerializeField]
		private ClickDetector DiagonalClickDetector;

		[Header("Sprites and images to change")]
		[SerializeField]
		private Image FocusImage;

		[SerializeField]
		private Image SportModeImage;

		[SerializeField]
		private Sprite FocusOnSprite;

		[SerializeField]
		private Sprite FocusOffSprite;

		[SerializeField]
		private Sprite SportModeOnSprite;

		[SerializeField]
		private Sprite SportModeOffSprite;

		[Header("Battery")]
		[SerializeField]
		private Slider Battery;

		[SerializeField]
		private Image BatteryFillImage;

		[SerializeField]
		private TextMeshProUGUI BatteryText;

		[SerializeField]
		private Gradient BatteryGradient;

		[Header("Map")]
		[SerializeField]
		private RawImage SmallMapTexture;

		[SerializeField]
		private GameObject[] ToHideObjects;

		private bool _isSportModeOn = true;

		private bool _isFocusOn;

		private bool _isHidden;

		private FreeCameraController _freeCameraController;

		private MenuLevelParent _menuLevelParent;

		private CoreGameManager _coreGameManager;

		private FreeDriveControl _freeDriveControl;

		private MapCamera _mapCamera;

		private DroneProvider _droneProvider;

		private LevelScript _levelScript;

		private ChatControl _chatControl;

		private EventRegistry _eventRegistry;

		private const float ZoomSpeed = 1f;

		private const float DroneZoomMouseScale = 1.5f;

		private static readonly Vector2 Multiplier = new Vector2(2.5f, 2.5f);

		private const float DesktopMoveSpeed = 6f;

		private const float DroneLookSmoothLerp = 15f;

		private Vector2 _smoothedDroneLook;

		private const float GamepadLookScale = 0.32f;

		private const float GamepadLookYScale = 0.6f;

		private const float DesktopTiltSpeed = 10f;

		private const float DroneTiltSpeed = 2.5f;

		private const float DesktopHeightSpeed = 20f;

		private const float MinZoomFov = 10f;

		private const float MaxZoomFov = 75f;

		private const float MinLookFactor = 0.15f;

		private bool _isCameraHeightStarted;

		private bool _isCameraTiltStarted;

		private bool _isCameraMoveStarted;

		private bool _isCameraLookStarted;

		private Camera _desktopDroneCamera;

		private DesktopControlsSettingsSO _controlsSettings;

		private RegistryHandle<FloatVariable> _droneCameraHeightHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.DroneCameraHeight);

		private RegistryHandle<Vector2Variable> _droneCameraMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.DroneCameraMove);

		private RegistryHandle<FloatVariable> _droneCameraTiltHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.DroneCameraTilt);

		private RegistryHandle<ScriptableEvent> _onSnapshotHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnSnapshot);

		private RegistryHandle<ScriptableEvent> _onResetDroneCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnResetDroneCamera);

		private RegistryHandle<ScriptableEvent> _toggleDroneSpeedHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleDroneSpeed);

		private RegistryHandle<ScriptableEvent> _onCenterDroneCameraHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnCenterDroneCamera);

		private RegistryHandle<ScriptableEvent> _closeDroneHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.CloseDrone);

		private RegistryHandle<BoolEvent> _onLookHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnLook);

		private RegistryHandle<Vector2Variable> _lookHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.Look);

		private RegistryHandle<FloatVariable> _zoomHandle = new RegistryHandle<FloatVariable>(EventRegistryIDs.Zoom);

		private RegistryHandle<BoolEvent> _onCameraMoveHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraMove);

		private RegistryHandle<BoolEvent> _onCameraHeightHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraHeight);

		private RegistryHandle<BoolEvent> _onCameraTiltHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnCameraTilt);

		private RegistryHandle<ScriptableEvent> _toggleUiHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToggleUi);

		private RegistryHandle<BoolEvent> _toggleMapHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ToggleMap);

		private RegistryHandle<ScriptableEvent> _onMenuHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.OnMenu);

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Drone;

		protected override bool ChangeRootGameInputGroup => true;

		[Inject]
		public void Construct(MenuLevelParent menuLevelParent, CoreGameManager coreGameManager, FreeDriveControl freeDriveControl, MapCamera mapCamera, ChatControl chatControl, DroneProvider droneProvider, LevelScript levelScript, FreeDriveProvider freeDriveProvider, EventRegistry eventRegistry, OptionalDependency<DesktopControlsSettingsSO> settingsDep, FreeCameraController freeCameraController)
		{
			_freeCameraController = freeCameraController;
			_chatControl = chatControl;
			_levelScript = levelScript;
			_droneProvider = droneProvider;
			_mapCamera = mapCamera;
			_freeDriveControl = freeDriveControl;
			_coreGameManager = coreGameManager;
			_eventRegistry = eventRegistry;
			if (settingsDep.HasValue)
			{
				_controlsSettings = settingsDep.Value;
			}
			_menuLevelParent = menuLevelParent;
		}

		protected override void Start()
		{
			_freeCameraController.CircleJoystick = CircleJoystick;
			_freeCameraController.ZoomJoystick = ZoomJoystick;
			_freeCameraController.VerticalJoystick = VerticalJoystick;
			_freeCameraController.DiagonalJoystick = DiagonalJoystick;
			_droneProvider.SetDroneWindow(this);
			_menuLevelParent.isDroneEnabled = true;
			SmallMapTexture.texture = _mapCamera.rtSmall;
			VerticalJoystick.SnapY = false;
			FixedJoystick zoomJoystick = ZoomJoystick;
			bool snapY = (ZoomJoystick.SnapX = false);
			zoomJoystick.SnapY = snapY;
			CircleClickDetector.OnPressed += (bool pressed) =>
			{
				_freeCameraController.IsCirclePressed = pressed;
			};
			VerticalClickDetector.OnPressed += (bool pressed) =>
			{
				_freeCameraController.IsVerticalPressed = pressed;
			};
			ZoomClickDetector.OnPressed += (bool pressed) =>
			{
				_freeCameraController.IsZoomPressed = pressed;
			};
			DiagonalClickDetector.OnPressed += (bool pressed) =>
			{
				_freeCameraController.IsDiagonalPressed = pressed;
			};
			SettingsButton.onClick.AddListener(OpenSettings);
			CloseButton.onClick.AddListener(CloseWindow);
			ScreenshotButton.onClick.AddListener(MakeScreenshot);
			HideUIButton.onClick.AddListener(HideUI);
			FocusButton.onClick.AddListener(Focus);
			SportModeButton.onClick.AddListener(SportMode);
			ShowBigMapButton.onClick.AddListener(ShowBigMap);
			ResetRotationButton.onClick.AddListener(ResetRotation);
			SetBattery();
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Repeat().Subscribe((long _) =>
			{
				SetBattery();
			})
				.AddTo(this);
			base.Start();
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

		protected override void OnEnable()
		{
			SubscribeToActions();
			base.OnEnable();
		}

		private void OnDisable()
		{
			UnsubscribeToActions();
			_isCameraLookStarted = false;
			_isCameraHeightStarted = false;
			_isCameraTiltStarted = false;
			_isCameraMoveStarted = false;
		}

		private void SubscribeToActions()
		{
			_onSnapshotHandle.Get(_eventRegistry).Subscribe(MakeScreenshot);
			_onResetDroneCameraHandle.Get(_eventRegistry).Subscribe(ResetRotation);
			_toggleDroneSpeedHandle.Get(_eventRegistry).Subscribe(SportMode);
			_onCenterDroneCameraHandle.Get(_eventRegistry).Subscribe(Focus);
			_closeDroneHandle.Get(_eventRegistry).Subscribe(CloseWindow);
			_onLookHandle.Get(_eventRegistry).Subscribe(OnDesktopLookToggle);
			_onCameraMoveHandle.Get(_eventRegistry).Subscribe(StartStopCameraMove);
			_onCameraHeightHandle.Get(_eventRegistry).Subscribe(StartStopCameraHeight);
			_onCameraTiltHandle.Get(_eventRegistry).Subscribe(StartStopCameraTilt);
			_toggleUiHandle.Get(_eventRegistry).Subscribe(HideUI);
			_toggleMapHandle.Get(_eventRegistry).Subscribe(ShowBigMapFromInput);
			_onMenuHandle.Get(_eventRegistry).Subscribe(OpenSettings);
		}

		private void UnsubscribeToActions()
		{
			_onSnapshotHandle.Get(_eventRegistry)?.Unsubscribe(MakeScreenshot);
			_onResetDroneCameraHandle.Get(_eventRegistry)?.Unsubscribe(ResetRotation);
			_toggleDroneSpeedHandle.Get(_eventRegistry)?.Unsubscribe(SportMode);
			_onCenterDroneCameraHandle.Get(_eventRegistry)?.Unsubscribe(Focus);
			_closeDroneHandle.Get(_eventRegistry)?.Unsubscribe(CloseWindow);
			_onLookHandle.Get(_eventRegistry)?.Unsubscribe(OnDesktopLookToggle);
			_onCameraMoveHandle.Get(_eventRegistry)?.Unsubscribe(StartStopCameraMove);
			_onCameraHeightHandle.Get(_eventRegistry)?.Unsubscribe(StartStopCameraHeight);
			_onCameraTiltHandle.Get(_eventRegistry)?.Unsubscribe(StartStopCameraTilt);
			_toggleUiHandle.Get(_eventRegistry)?.Unsubscribe(HideUI);
			_toggleMapHandle.Get(_eventRegistry)?.Unsubscribe(ShowBigMapFromInput);
			_onMenuHandle.Get(_eventRegistry)?.Unsubscribe(OpenSettings);
		}

		private void ShowBigMapFromInput(bool show)
		{
			if (show)
			{
				GameObject[] toHideObjects = ToHideObjects;
				for (int i = 0; i < toHideObjects.Length; i++)
				{
					toHideObjects[i].SetActive(value: false);
				}
				_freeDriveControl.OnBigMapHide += ShowHiddenElements;
			}
		}

		private void StartStopCameraMove(bool isStarted)
		{
			_isCameraMoveStarted = isStarted;
		}

		private void StartStopCameraHeight(bool isStarted)
		{
			if (isStarted)
			{
				_freeCameraController.VerticalJoystick = null;
			}
			else if (_isCameraHeightStarted)
			{
				_freeCameraController.VerticalInput = 0f;
				_freeCameraController.VerticalJoystick = VerticalJoystick;
			}
			_freeCameraController.IsVerticalPressed = isStarted;
			_isCameraHeightStarted = isStarted;
		}

		private void StartStopCameraTilt(bool isStarted)
		{
			_freeCameraController.DiagonalJoystick = ((_isCameraTiltStarted && !isStarted) ? DiagonalJoystick : null);
			_freeCameraController.IsDiagonalPressed = isStarted;
			_isCameraTiltStarted = isStarted;
		}

		private void Update()
		{
			if (!_freeCameraController)
			{
				return;
			}
			Vector2Variable vector2Variable = _droneCameraMoveHandle.Get(_eventRegistry);
			FloatVariable floatVariable = _droneCameraHeightHandle.Get(_eventRegistry);
			FloatVariable floatVariable2 = _droneCameraTiltHandle.Get(_eventRegistry);
			Vector2 vector = (vector2Variable ? vector2Variable.Value : Vector2.zero);
			float num = (floatVariable ? floatVariable.Value : 0f);
			float num2 = (floatVariable2 ? floatVariable2.Value : 0f);
			if (_isCameraHeightStarted)
			{
				if (_isSportModeOn)
				{
					num *= 1.5f;
				}
				_freeCameraController.VerticalInput = num;
			}
			if (_isCameraTiltStarted)
			{
				_freeCameraController.CameraTilt += (0f - num2) * 2.5f;
			}
			if (_isCameraMoveStarted && vector.sqrMagnitude > 0f)
			{
				if (_isSportModeOn)
				{
					vector *= 1.5f;
				}
				Transform transform = _freeCameraController.transform;
				Vector3 normalized = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
				Vector3 normalized2 = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
				_freeCameraController.MoveExternalClamped(normalized * vector.y + normalized2 * vector.x);
			}
			ApplyDesktopLookWrapper();
			ApplyDesktopZoomWrapper();
		}

		private void OnDesktopLookToggle(bool isLooking)
		{
			_isCameraLookStarted = isLooking;
			if (!isLooking)
			{
				_smoothedDroneLook = Vector2.zero;
			}
		}

		private void ApplyDesktopLookWrapper()
		{
			if (!_isCameraLookStarted || !_freeCameraController)
			{
				return;
			}
			Vector2Variable vector2Variable = _lookHandle.Get(_eventRegistry);
			if (!vector2Variable)
			{
				return;
			}
			_smoothedDroneLook = Vector2.Lerp(_smoothedDroneLook, vector2Variable.Value, 15f);
			if (!(_smoothedDroneLook.sqrMagnitude < 0.0001f))
			{
				_smoothedDroneLook = ScaleDroneLook(_smoothedDroneLook);
				if (!_desktopDroneCamera && (bool)_freeCameraController)
				{
					_freeCameraController.TryGetComponent<Camera>(out _desktopDroneCamera);
				}
				float value = ((_desktopDroneCamera ? _desktopDroneCamera.fieldOfView : 75f) - 10f) / 65f;
				float num = Mathf.Lerp(0.15f, 1f, Mathf.Clamp01(value));
				bool flag = InputDeviceDetector.Instance != null && InputDeviceDetector.Instance.CurrentDeviceType != InputDeviceType.KeyboardMouse;
				float num2 = (flag ? 0.32f : 1f);
				float angle = _smoothedDroneLook.x * Multiplier.x * num * num2;
				float angle2 = (0f - _smoothedDroneLook.y) * Multiplier.y * num * num2 * (flag ? 0.6f : 1f);
				Transform transform = _freeCameraController.transform;
				transform.rotation = Quaternion.AngleAxis(angle, Vector3.up) * transform.rotation;
				transform.rotation *= Quaternion.AngleAxis(angle2, Vector3.right);
			}
		}

		private void ApplyDesktopZoomWrapper()
		{
			FloatVariable floatVariable = _zoomHandle.Get(_eventRegistry);
			if (!floatVariable)
			{
				return;
			}
			float value = floatVariable.Value;
			if (!(Mathf.Abs(value) < 0.01f))
			{
				if (!_desktopDroneCamera && (bool)_freeCameraController)
				{
					_freeCameraController.TryGetComponent<Camera>(out _desktopDroneCamera);
				}
				if ((bool)_desktopDroneCamera)
				{
					float num = ((InputDeviceDetector.Instance != null && InputDeviceDetector.Instance.CurrentDeviceType != InputDeviceType.KeyboardMouse) ? 1f : 1.5f);
					_desktopDroneCamera.fieldOfView = Mathf.Clamp(_desktopDroneCamera.fieldOfView - value * num, 10f, 75f);
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_freeDriveControl.OnBigMapHide -= ShowHiddenElements;
			_droneProvider.ResetDroneWindow();
			_menuLevelParent.isDroneEnabled = false;
			_freeCameraController.CircleJoystick = null;
			_freeCameraController.ZoomJoystick = null;
			_freeCameraController.VerticalJoystick = null;
			_freeCameraController.DiagonalJoystick = null;
		}

		private void ResetRotation()
		{
			_freeCameraController.ResetDiagonalRot();
		}

		private void OpenSettings()
		{
			_levelScript.CleanScreen();
			_managerUI.ShowSettingsWindow(null, cleanScreen: false);
		}

		private void CloseWindow()
		{
			Close();
			_menuLevelParent.DeactiveDrone();
		}

		private void MakeScreenshot()
		{
			_coreGameManager.MakeScreenShot();
		}

		private void HideUI()
		{
			_isHidden = !_isHidden;
			MainCanvasGroup.alpha = ((!_isHidden) ? 1 : 0);
			if ((bool)_chatControl)
			{
				if (_isHidden)
				{
					_chatControl.HideAllChat();
				}
				else
				{
					_chatControl.ShowPartiallyChat();
				}
			}
		}

		private void Focus()
		{
			_isFocusOn = !_isFocusOn;
			FocusImage.sprite = (_isFocusOn ? FocusOnSprite : FocusOffSprite);
			_freeCameraController.LookAt();
		}

		private void SportMode()
		{
			_isSportModeOn = !_isSportModeOn;
			SportModeImage.sprite = (_isSportModeOn ? SportModeOnSprite : SportModeOffSprite);
			_freeCameraController.SetSport();
		}

		private void SetBattery()
		{
			float num = SystemInfo.batteryLevel;
			if (num < 0f)
			{
				num = 1f;
			}
			Battery.value = num;
			BatteryText.text = $"{num * 100f}%";
			BatteryFillImage.color = BatteryGradient.Evaluate(1f - num);
		}

		private void ShowBigMap()
		{
			GameObject[] toHideObjects = ToHideObjects;
			for (int i = 0; i < toHideObjects.Length; i++)
			{
				toHideObjects[i].SetActive(value: false);
			}
			_freeDriveControl.ShowBigMap();
			_freeDriveControl.OnBigMapHide += ShowHiddenElements;
		}

		private void ShowHiddenElements()
		{
			_freeDriveControl.OnBigMapHide -= ShowHiddenElements;
			GameObject[] toHideObjects = ToHideObjects;
			for (int i = 0; i < toHideObjects.Length; i++)
			{
				toHideObjects[i].SetActive(value: true);
			}
		}
	}
}
