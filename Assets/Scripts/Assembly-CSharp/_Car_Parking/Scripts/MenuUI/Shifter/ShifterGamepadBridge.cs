using CPM.Shifter.Contracts;
using CPM.Shifter.Logic;
using CPM.Transmission;
using CPMEngine;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.MenuUI.Shifter
{
	[DefaultExecutionOrder(100)]
	public class ShifterGamepadBridge : MonoBehaviour
	{
		[Header("Wiring (same prefab as ShifterController)")]
		[SerializeField]
		private ShifterController _shifterController;

		[SerializeField]
		private ManualHShifterController _manualController;

		[SerializeField]
		private ManualHShifterInput _manualInput;

		[Header("Stick → knob conversion")]
		[Tooltip("Magnitude required to start a gamepad grab. Hysteresis vs exitDeadzone.")]
		[SerializeField]
		private float _stickEnterDeadzone = 0.35f;

		[SerializeField]
		[Tooltip("Magnitude below which an active grab is released.")]
		private float _stickExitDeadzone = 0.15f;

		[Tooltip("Fully-deflected stick maps to ±this many px of virtual-finger displacement.")]
		[SerializeField]
		private float _stickPxRange = 200f;

		private EventRegistry _eventRegistry;

		private InputDeviceDetector _detector;

		private RegistryHandle<BoolEvent> _shifterMoveStartedHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ShifterMoveStarted);

		private RegistryHandle<Vector2Variable> _shifterMoveHandle = new RegistryHandle<Vector2Variable>(EventRegistryIDs.ShifterMove);

		private RegistryHandle<BoolEvent> _shifterUpDownHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ShifterUpDown);

		private RegistryHandle<BoolEvent> _shifterMAHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.ShifterMA);

		private RegistryHandle<ScriptableEvent> _shiftToggleMAHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ShiftToggleMA);

		private RegistryHandle<GearVariable> _gearHandle = new RegistryHandle<GearVariable>(EventRegistryIDs.Gear);

		private bool _isKnobMoving;

		private readonly Vector2 StickSpeed = new Vector2(0.042f, 0.021f);

		private InputItemsProvider _inputItemsProvider;

		private CarInputManager _carInputManager;

		private SteeringWheelValueEvent _steeringWheelValueEvent;

		private bool? _requestedActionsManual;

		private int ForwardExitSelector
		{
			get
			{
				if (!IsSequential)
				{
					return 4;
				}
				return 3;
			}
		}

		private bool IsSequential
		{
			get
			{
				Powertrain powertrain = _carInputManager?.GetPowertrain;
				if (powertrain != null)
				{
					return powertrain.GearboxKind == GearboxKind.Sequential;
				}
				return false;
			}
		}

		[Inject]
		public void Construct(InputItemsProvider inputItemsProvider, CarInputManager carInputManager, EventRegistry eventRegistry, SteeringWheelValueEvent steeringWheelValueEvent)
		{
			_inputItemsProvider = inputItemsProvider;
			_carInputManager = carInputManager;
			_eventRegistry = eventRegistry;
			_steeringWheelValueEvent = steeringWheelValueEvent;
		}

		private void Awake()
		{
			if (_shifterController == null)
			{
				_shifterController = GetComponent<ShifterController>();
			}
			if (_manualController == null)
			{
				_manualController = GetComponentInChildren<ManualHShifterController>(includeInactive: true);
			}
			if (_manualInput == null)
			{
				_manualInput = GetComponentInChildren<ManualHShifterInput>(includeInactive: true);
			}
		}

		private void OnEnable()
		{
			_detector = InputDeviceDetector.Instance;
			if (_manualInput != null)
			{
				_manualInput.OnInputDisabled += EndGrabIfActive;
			}
			ShifterController.OnShifterSet += HandleModeFlip;
			V.OnCarExit += HandleCarExit;
			if (_requestedActionsManual.HasValue)
			{
				AddActionSubscriptions(_requestedActionsManual.Value);
			}
		}

		private void OnDisable()
		{
			RemoveActionSubscriptions();
			if (_manualInput != null)
			{
				_manualInput.OnInputDisabled -= EndGrabIfActive;
			}
			ShifterController.OnShifterSet -= HandleModeFlip;
			V.OnCarExit -= HandleCarExit;
			EndGrabIfActive();
		}

		private void HandleGearInput(GearEnum e)
		{
			if (FlagsStorage.OutOfCar || FlagsStorage.IsPassenger)
			{
				return;
			}
			IShifterControl shifterControl = _inputItemsProvider?.ShifterControl;
			if (shifterControl == null)
			{
				return;
			}
			if (IsAutomaticCar())
			{
				int? num = BridgeMapping.MapGearEnumToAutoSelector((byte)e, GetCurrentSelector());
				if (num.HasValue && ((num.Value != 5 && num.Value != 6 && num.Value != 7) || HasManualMode()))
				{
					shifterControl.SetAutoMode(num.Value);
					shifterControl.PlayDriverShiftAnimation();
				}
			}
			else
			{
				int? num2 = BridgeMapping.MapGearEnumToManualGear((byte)e);
				if (num2.HasValue && !(_manualController == null))
				{
					_manualController.RequestGearChange(num2.Value);
				}
			}
		}

		private void HandleShiftMD(bool m)
		{
			if (!FlagsStorage.OutOfCar && !FlagsStorage.IsPassenger && IsAutomaticCar() && (!m || HasManualMode()))
			{
				IShifterControl shifterControl = _inputItemsProvider?.ShifterControl;
				if (shifterControl != null)
				{
					shifterControl.SetAutoMode(m ? 5 : ForwardExitSelector);
					shifterControl.PlayDriverShiftAnimation();
				}
			}
		}

		private void HandleShiftUpDown(bool up)
		{
			if (!FlagsStorage.OutOfCar && !FlagsStorage.IsPassenger && IsAutomaticCar())
			{
				IShifterControl shifterControl = _inputItemsProvider?.ShifterControl;
				if (shifterControl != null)
				{
					shifterControl.SetAutoMode(GetSelectorState(up));
					shifterControl.PlayDriverShiftAnimation();
				}
			}
		}

		private int GetSelectorState(bool isUp)
		{
			int effectiveSelector = GetEffectiveSelector();
			switch (effectiveSelector)
			{
			case 5:
				if (isUp)
				{
					return IsSequential ? 7 : 6;
				}
				if (!isUp)
				{
					return IsSequential ? 6 : 7;
				}
				break;
			case 4:
				if (isUp)
				{
					return 3;
				}
				if (!isUp)
				{
					return HasAutoMode(AutoShifterModes.Sport) ? 8 : (HasAutoMode(AutoShifterModes.Low) ? 9 : effectiveSelector);
				}
				break;
			case 8:
				if (isUp)
				{
					return 4;
				}
				if (!isUp)
				{
					return HasAutoMode(AutoShifterModes.Low) ? 9 : effectiveSelector;
				}
				break;
			case 9:
				if (isUp)
				{
					return HasAutoMode(AutoShifterModes.Sport) ? 8 : 4;
				}
				break;
			case 3:
				if (isUp)
				{
					return 2;
				}
				if (!isUp)
				{
					return IsSequential ? 5 : 4;
				}
				break;
			case 2:
				if (isUp)
				{
					return 1;
				}
				if (!isUp)
				{
					return 3;
				}
				break;
			case 1:
				if (!isUp)
				{
					return 2;
				}
				break;
			}
			return effectiveSelector;
		}

		private int GetEffectiveSelector()
		{
			int currentSelector = GetCurrentSelector();
			if (currentSelector != 4)
			{
				return currentSelector;
			}
			Powertrain powertrain = _carInputManager?.GetPowertrain;
			if (powertrain == null)
			{
				return currentSelector;
			}
			if (IsSequential)
			{
				return 5;
			}
			if (powertrain.LowMode)
			{
				return 9;
			}
			if (powertrain.SportMode)
			{
				return 8;
			}
			return currentSelector;
		}

		private bool HasAutoMode(AutoShifterModes flag)
		{
			Powertrain powertrain = _carInputManager?.GetPowertrain;
			if (powertrain != null)
			{
				return powertrain.AutoModes.HasFlag(flag);
			}
			return false;
		}

		private bool HasManualMode()
		{
			Powertrain powertrain = _carInputManager?.GetPowertrain;
			if (powertrain != null)
			{
				if (powertrain.GearboxKind != GearboxKind.Sequential)
				{
					return powertrain.AutoModes.HasFlag(AutoShifterModes.Manual);
				}
				return true;
			}
			return false;
		}

		private void ToggleMD()
		{
			if (FlagsStorage.OutOfCar || FlagsStorage.IsPassenger || !IsAutomaticCar() || !HasManualMode())
			{
				return;
			}
			IShifterControl shifterControl = _inputItemsProvider?.ShifterControl;
			if (shifterControl != null)
			{
				int effectiveSelector = GetEffectiveSelector();
				int forwardExitSelector = ForwardExitSelector;
				if (effectiveSelector == 5)
				{
					shifterControl.SetAutoMode(forwardExitSelector);
				}
				else if (effectiveSelector == forwardExitSelector)
				{
					shifterControl.SetAutoMode(5);
				}
				shifterControl.PlayDriverShiftAnimation();
			}
		}

		private void Update()
		{
			if (FlagsStorage.OutOfCar || FlagsStorage.IsPassenger)
			{
				return;
			}
			if (IsAutomaticCar())
			{
				EndGrabIfActive();
				return;
			}
			Vector2Variable vector2Variable = _shifterMoveHandle.Get(_eventRegistry);
			if ((bool)vector2Variable && (double)vector2Variable.Value.sqrMagnitude > 0.001)
			{
				_manualInput.GamepadDrag(Vector2.Scale(vector2Variable.Value, StickSpeed));
			}
		}

		private void StartStopKnobMove(bool started)
		{
			if ((bool)_steeringWheelValueEvent && _steeringWheelValueEvent.StoredValue && _detector.CurrentDeviceType == InputDeviceType.Wheel)
			{
				return;
			}
			_ = _isKnobMoving;
			if (started && !_manualInput.BeginGamepadGrab())
			{
				_isKnobMoving = false;
				return;
			}
			if (_isKnobMoving && !started)
			{
				_manualInput?.EndGamepadGrab();
			}
			_isKnobMoving = started;
		}

		private void EndGrabIfActive()
		{
			StartStopKnobMove(started: false);
		}

		public void SubscribeToActions(bool isManual)
		{
			_requestedActionsManual = isManual;
			RemoveActionSubscriptions();
			if (base.isActiveAndEnabled)
			{
				AddActionSubscriptions(isManual);
			}
		}

		public void UnsubscribeFromActions()
		{
			_requestedActionsManual = null;
			RemoveActionSubscriptions();
		}

		private void OnDestroy()
		{
			RemoveActionSubscriptions();
		}

		private void AddActionSubscriptions(bool isManual)
		{
			if (isManual)
			{
				_gearHandle.Get(_eventRegistry).Subscribe(HandleGearInput);
				_shifterMoveStartedHandle.Get(_eventRegistry).Subscribe(StartStopKnobMove);
			}
			else
			{
				_shifterMAHandle.Get(_eventRegistry).Subscribe(HandleShiftMD);
				_shifterUpDownHandle.Get(_eventRegistry).Subscribe(HandleShiftUpDown);
				_shiftToggleMAHandle.Get(_eventRegistry).Subscribe(ToggleMD);
			}
		}

		private void RemoveActionSubscriptions()
		{
			_gearHandle.Get(_eventRegistry)?.Unsubscribe(HandleGearInput);
			_shifterMoveStartedHandle.Get(_eventRegistry)?.Unsubscribe(StartStopKnobMove);
			_shifterMAHandle.Get(_eventRegistry)?.Unsubscribe(HandleShiftMD);
			_shifterUpDownHandle.Get(_eventRegistry)?.Unsubscribe(HandleShiftUpDown);
			_shiftToggleMAHandle.Get(_eventRegistry)?.Unsubscribe(ToggleMD);
		}

		private void HandleModeFlip(bool isManual)
		{
			EndGrabIfActive();
		}

		private void HandleCarExit(GameObject person, VehicleData vehicleData)
		{
			EndGrabIfActive();
		}

		private bool IsAutomaticCar()
		{
			Powertrain powertrain = _carInputManager?.GetPowertrain;
			if ((bool)powertrain)
			{
				return powertrain.IsAutomaticGearbox;
			}
			return false;
		}

		private int GetCurrentSelector()
		{
			if (!(_carInputManager != null))
			{
				return 4;
			}
			return _carInputManager.SelectorState;
		}
	}
}
