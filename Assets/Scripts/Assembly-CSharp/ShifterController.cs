using System;
using System.Threading;
using CPM.Shifter.Contracts;
using CPM.Transmission;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.GameInput.Settings;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.MenuUI.TransferCaseUI;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[RequireComponent(typeof(RectTransform))]
public class ShifterController : MonoBehaviour, IShifterControl
{
	[Header("Sub-mode containers")]
	[Tooltip("GameObject holding manual H-pattern visuals. Toggled active by SetManual / inactive by SetAutoMatic.")]
	[SerializeField]
	private GameObject _manualVisuals = default;

	[Tooltip("GameObject holding automatic PRNDM visuals. Toggled active by SetAutoMatic / inactive by SetManual.")]
	[SerializeField]
	private GameObject _autoVisuals = default;

	[Header("Sub-mode controllers")]
	[SerializeField]
	private ManualHShifterController _manualMode;

	[SerializeField]
	private AutoShifterController _autoMode;

	[Header("Shared widgets")]
	[SerializeField]
	private ClutchSliderControl _clutchSlider = default;

	[Tooltip("Optional. Wire here so ShowObjects/HideObjects toggle the throttle pedal together with the shifter visuals (e.g. hide it on car-exit).")]
	[SerializeField]
	private ThrottleSliderControl _throttleSlider = default;

	[Tooltip("Optional. Same as _throttleSlider but for the brake pedal.")]
	[SerializeField]
	private BrakeSliderControl _brakeSlider = default;

	[SerializeField]
	[Tooltip("Wire driving widgets that don't have dedicated fields here (Horn, Key/Starter, Handbrake, gear-trace canvas, etc.). They get toggled together with the pedals on car-exit/entry. Replaces the legacy ShifterControl behavior where these siblings lived on the shifter prefab and were toggled implicitly by SetActiveObjects.")]
	private GameObject[] _extraDriveWidgets = default;

	[SerializeField]
	[Tooltip("Optional. HUD button whose visibility/state depends on the current car's Powertrain. Spawn paths (SetupCarDriver -> CarInputManager.AttachVehicle -> InitHUDAnalog) never raise V.OnCarEntered, so UpdateCar() cascades to it here — same pattern as the speedometer below.")]
	[Header("Cross-widget cascades")]
	private TransferCaseHudButton _transferCaseButton = default;

	[SerializeField]
	[Tooltip("Empty anchor the InputHintsSpawner spawns the D↔M rocker hint into. Enabled only when the current car actually supports a manual (M) mode, so the hint hides on cars where the M toggle is blocked. Hint gate only — the action block itself lives in ShifterGamepadBridge.")]
	[Header("Desktop input hint")]
	private GameObject _manualHintAnchor = default;

	[SerializeField]
	[Tooltip("Widgets hidden while casual driving assist is on (clutch, H-shifter, etc.). Just drop the GameObjects here — a CanvasGroup is added automatically so the layout keeps their slot.")]
	private GameObject[] _casualHiddenWidgets = default;

	private CanvasGroup[] _casualGroups;

	private CarInputManager _carInputManager;

	private InputItemsProvider _inputItemsProvider;

	private CancellationTokenSource _initCts;

	private bool _isAutoMode;

	private Powertrain _subscribedPowertrain;

	private bool _isAutomaticCarCached;

	private DesktopControlsSettingsSO _controlsSettings;

	private IShifterMode ActiveMode
	{
		get
		{
			if (!_isAutoMode)
			{
				return _manualMode;
			}
			return _autoMode;
		}
	}

	private bool IsAutomaticCar => _isAutomaticCarCached;

	public bool IsTouching => ActiveMode?.IsTouching ?? false;

	public static event Action<bool> OnShifterSet;

	public static event Action OnReleaseGearEvent;

	public static void FireReleaseGearEvent()
	{
		OnReleaseGearEvent?.Invoke();
	}

	[Inject]
	public void ConstructControlsSettings(OptionalDependency<DesktopControlsSettingsSO> settings)
	{
		if (settings != null && settings.HasValue)
		{
			_controlsSettings = settings.Value;
		}
	}

	[Inject]
	public void Construct(InputItemsProvider inputItemsProvider, CarInputManager carInputManager)
	{
		_inputItemsProvider = inputItemsProvider;
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		if (_inputItemsProvider != null)
		{
			_inputItemsProvider.ShifterControl = this;
		}
		if (_manualMode == null)
		{
			_manualMode = GetComponentInChildren<ManualHShifterController>(includeInactive: true);
		}
		if (_autoMode == null)
		{
			_autoMode = GetComponentInChildren<AutoShifterController>(includeInactive: true);
		}
	}

	private void OnEnable()
	{
		Subscribe();
	}

	private void OnDisable()
	{
		Unsubscribe();
	}

	private void Subscribe()
	{
		_initCts = new CancellationTokenSource();
		InitAsync(_initCts.Token).Forget();
		V.OnCarExit -= OnCarExitHandler;
		V.OnCarExit += OnCarExitHandler;
		if ((bool)_controlsSettings)
		{
			_controlsSettings.CasualModeChanged -= OnCasualModeChanged;
			_controlsSettings.CasualModeChanged += OnCasualModeChanged;
		}
		ApplyCasualVisibility();
	}

	private void Unsubscribe()
	{
		CommonExtensions.CancelAndDisposeToken(ref _initCts);
		if ((bool)_subscribedPowertrain)
		{
			_subscribedPowertrain.OnGearChanged -= OnPowertrainGearChanged;
			_subscribedPowertrain = null;
		}
		_isAutomaticCarCached = false;
		V.OnCarExit -= OnCarExitHandler;
		if ((bool)_controlsSettings)
		{
			_controlsSettings.CasualModeChanged -= OnCasualModeChanged;
		}
	}

	private async UniTaskVoid InitAsync(CancellationToken ct)
	{
		while (!ct.IsCancellationRequested && (!_carInputManager || !_carInputManager.GetPowertrain))
		{
			await UniTask.Yield(ct);
		}
		if (!ct.IsCancellationRequested)
		{
			_subscribedPowertrain = _carInputManager.GetPowertrain;
			_isAutomaticCarCached = _subscribedPowertrain.IsAutomaticGearbox;
			_subscribedPowertrain.OnGearChanged -= OnPowertrainGearChanged;
			_subscribedPowertrain.OnGearChanged += OnPowertrainGearChanged;
			SetGearbox(_isAutomaticCarCached);
		}
	}

	private void OnCarExitHandler(GameObject person, VehicleData vehicleData)
	{
		if (base.isActiveAndEnabled)
		{
			HideObjects();
		}
	}

	private void OnDestroy()
	{
		V.OnCarExit -= OnCarExitHandler;
		if ((bool)_subscribedPowertrain)
		{
			_subscribedPowertrain.OnGearChanged -= OnPowertrainGearChanged;
			_subscribedPowertrain = null;
		}
		if ((bool)_controlsSettings)
		{
			_controlsSettings.CasualModeChanged -= OnCasualModeChanged;
		}
		if (_inputItemsProvider != null && _inputItemsProvider.ShifterControl == this)
		{
			_inputItemsProvider.ShifterControl = null;
		}
	}

	private void OnPowertrainGearChanged()
	{
		if (base.isActiveAndEnabled)
		{
			ActiveMode?.SyncToCurrentGear();
		}
	}

	public void SetGearbox(bool isAutomatic)
	{
		if (isAutomatic)
		{
			SetAutoMatic();
		}
		else
		{
			SetManual();
		}
		SetActiveObjects(state: true);
		SetDriveWidgetsActive(active: true);
		ApplyManualHintVisibility();
	}

	private void ApplyManualHintVisibility()
	{
		if ((bool)_manualHintAnchor)
		{
			bool active = _isAutomaticCarCached && (bool)_subscribedPowertrain && (_subscribedPowertrain.GearboxKind == GearboxKind.Sequential || _subscribedPowertrain.AutoModes.HasFlag(AutoShifterModes.Manual));
			_manualHintAnchor.SetActive(active);
		}
	}

	public void SetManual()
	{
		FlagsStorage.PRND = false;
		_isAutoMode = false;
		if ((bool)_autoVisuals)
		{
			_autoVisuals.SetActive(value: false);
		}
		_autoMode?.Deactivate();
		if ((bool)_clutchSlider)
		{
			_clutchSlider.gameObject.SetActive(value: true);
		}
		if ((bool)_manualVisuals)
		{
			_manualVisuals.SetActive(value: true);
		}
		_manualMode?.Activate();
		OnShifterSet?.Invoke(obj: true);
	}

	public void SetAutoMatic()
	{
		FlagsStorage.PRND = true;
		_isAutoMode = true;
		if ((bool)_manualVisuals)
		{
			_manualVisuals.SetActive(value: false);
		}
		_manualMode?.Deactivate();
		if ((bool)_clutchSlider)
		{
			_clutchSlider.gameObject.SetActive(value: false);
		}
		if ((bool)_autoVisuals)
		{
			_autoVisuals.SetActive(value: true);
		}
		_autoMode?.Activate();
		OnShifterSet?.Invoke(obj: false);
	}

	public void UpdateCar()
	{
		Unsubscribe();
		Subscribe();
		SpeedometerUI speedometerUI = _inputItemsProvider?.SpeedometerUI;
		if ((bool)speedometerUI)
		{
			speedometerUI.UpdateCar();
		}
		if ((bool)_transferCaseButton)
		{
			_transferCaseButton.RefreshFromCar();
		}
	}

	public void ShowObjects(bool fromControlUi)
	{
		bool flag = (_isAutoMode = (bool)_carInputManager && (bool)_carInputManager.GetPowertrain && _carInputManager.GetPowertrain.IsAutomaticGearbox);
		if ((bool)_manualVisuals)
		{
			_manualVisuals.SetActive(!flag);
		}
		if ((bool)_autoVisuals)
		{
			_autoVisuals.SetActive(flag);
		}
		if ((bool)_clutchSlider)
		{
			_clutchSlider.gameObject.SetActive(!flag);
		}
		SetDriveWidgetsActive(active: true);
		ActiveMode?.SetGearTraceAlpha(fromControlUi ? 1f : 0.3f);
	}

	public void HideObjects()
	{
		if ((bool)_manualVisuals)
		{
			_manualVisuals.SetActive(value: false);
		}
		if ((bool)_autoVisuals)
		{
			_autoVisuals.SetActive(value: false);
		}
		if ((bool)_clutchSlider)
		{
			_clutchSlider.gameObject.SetActive(value: false);
		}
		SetDriveWidgetsActive(active: false);
	}

	private void SetDriveWidgetsActive(bool active)
	{
		if ((bool)_throttleSlider)
		{
			_throttleSlider.gameObject.SetActive(active);
		}
		if ((bool)_brakeSlider)
		{
			_brakeSlider.gameObject.SetActive(active);
		}
		if (_extraDriveWidgets != null)
		{
			for (int i = 0; i < _extraDriveWidgets.Length; i++)
			{
				if ((bool)_extraDriveWidgets[i])
				{
					_extraDriveWidgets[i].SetActive(active);
				}
			}
		}
		ApplyCasualVisibility();
	}

	private void OnCasualModeChanged(bool _)
	{
		ApplyCasualVisibility();
		ActiveMode?.SyncToCurrentGear();
	}

	private void ApplyCasualVisibility()
	{
		if (_casualHiddenWidgets == null)
		{
			return;
		}
		if (_casualGroups == null || _casualGroups.Length != _casualHiddenWidgets.Length)
		{
			_casualGroups = new CanvasGroup[_casualHiddenWidgets.Length];
			for (int i = 0; i < _casualHiddenWidgets.Length; i++)
			{
				GameObject gameObject = _casualHiddenWidgets[i];
				if ((bool)gameObject)
				{
					_casualGroups[i] = (gameObject.TryGetComponent<CanvasGroup>(out var component) ? component : gameObject.AddComponent<CanvasGroup>());
				}
			}
		}
		bool isCasualControl = FlagsStorage.IsCasualControl;
		for (int j = 0; j < _casualGroups.Length; j++)
		{
			CanvasGroup canvasGroup = _casualGroups[j];
			if ((bool)canvasGroup)
			{
				canvasGroup.alpha = (isCasualControl ? 0f : 1f);
				canvasGroup.interactable = !isCasualControl;
				canvasGroup.blocksRaycasts = !isCasualControl;
			}
		}
	}

	public void SetActiveObjects(bool state)
	{
		SetDriveWidgetsActive(state);
	}

	public void ResetPos()
	{
		if ((bool)_clutchSlider)
		{
			_clutchSlider.ResetClutch();
		}
		if (!IsAutomaticCar)
		{
			if (_carInputManager?.GetPowertrain != null)
			{
				_carInputManager.GetPowertrain.Gear = 0;
			}
			ActiveMode?.ResetToNeutral();
		}
	}

	public void OnClutchReleased()
	{
	}

	public void SetNeutral()
	{
		if (!IsAutomaticCar)
		{
			if (_carInputManager?.GetPowertrain != null)
			{
				_carInputManager.GetPowertrain.Gear = 0;
			}
			ActiveMode?.ResetToNeutral();
		}
	}

	public void SetPosByGearJoyStick(int gear)
	{
		ActiveMode?.SetPosByGear(gear);
	}

	public void SetAutoMode(int selectorState)
	{
		if (IsAutomaticCar && (bool)_autoMode)
		{
			_autoMode.SetAutoMode(selectorState);
		}
	}

	public void PlayDriverShiftAnimation()
	{
		ActiveMode?.PlayDriverShiftAnimation();
	}

	public void SetGearTraceAlpha(float a)
	{
		ActiveMode?.SetGearTraceAlpha(a);
	}

	public void SetGearLabelText(string text)
	{
		ActiveMode?.SetGearLabelText(text);
	}
}
