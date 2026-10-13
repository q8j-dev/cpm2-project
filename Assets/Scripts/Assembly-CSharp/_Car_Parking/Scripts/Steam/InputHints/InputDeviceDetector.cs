using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.GameInput;
using _Car_Parking.Scripts.GameInput.Rebinding;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	public class InputDeviceDetector : IInitializable, IDisposable, ITickable
	{
		private const float MOUSE_DELTA_THRESHOLD = 0.5f;

		private const float TRIGGER_THRESHOLD = 0.2f;

		private const float SWITCH_DEBOUNCE = 0.3f;

		private const float STICK_THRESHOLD_SWITCH = 0.6f;

		private const float STICK_THRESHOLD_KEEP = 0.3f;

		private readonly Dictionary<int, bool> _simRacingByDeviceId = new Dictionary<int, bool>();

		private GamepadMirrorTracker _mirrors;

		private float lastSwitchTime;

		private static InputDevice _lastWheelDevice;

		private const float WHEEL_STEER_DELTA = 0.04f;

		private float _lastWheelSteer;

		private bool _wheelSteerBaseline;

		public static InputDeviceDetector Instance { get; private set; }

		public InputDeviceType CurrentDeviceType { get; private set; }

		public InputActionAsset BindingSource { get; set; }

		public static string CurrentWheelProduct
		{
			get
			{
				if (_lastWheelDevice != null && _lastWheelDevice.added)
				{
					return WheelBindingRoot(_lastWheelDevice);
				}
				foreach (InputDevice device in InputSystem.devices)
				{
					string product = device.description.product;
					if (!string.IsNullOrEmpty(product) && (product.ToLowerInvariant().Contains("moza") || product.ToLowerInvariant().Contains("wheel")))
					{
						return WheelBindingRoot(device);
					}
				}
				return null;
			}
		}

		public static event Action OnInstanceReady;

		public event Action<InputDeviceType> OnDeviceTypeChanged;

		public event Action OnBindingsChanged;

		public void NotifyBindingsChanged()
		{
			Action action = OnBindingsChanged;
			if (action == null)
			{
				return;
			}
			Delegate[] invocationList = action.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				try
				{
					((Action)obj)();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		[Inject]
		public void ConstructMirrors(GamepadMirrorTracker mirrors)
		{
			_mirrors = mirrors;
		}

		private static string WheelBindingRoot(InputDevice device)
		{
			string layout = device.layout;
			if (!string.IsNullOrEmpty(layout) && layout.StartsWith("HID::", StringComparison.Ordinal))
			{
				return layout.Substring("HID::".Length);
			}
			return device.description.product;
		}

		public void Initialize()
		{
			Instance = this;
			InputSystem.onActionChange += OnActionChange;
			OnInstanceReady?.Invoke();
		}

		public void Dispose()
		{
			InputSystem.onActionChange -= OnActionChange;
			if (Instance == this)
			{
				Instance = null;
			}
		}

		private bool IsMeaningfulInput(InputControl control)
		{
			if (control is StickControl stickControl)
			{
				float num = stickControl.x.ReadUnprocessedValue();
				float num2 = stickControl.y.ReadUnprocessedValue();
				float num3 = ((CurrentDeviceType == InputDeviceType.KeyboardMouse) ? 0.6f : 0.3f);
				return num * num + num2 * num2 > num3 * num3;
			}
			if (control is AxisControl axisControl && control.parent is StickControl)
			{
				float num4 = ((CurrentDeviceType == InputDeviceType.KeyboardMouse) ? 0.6f : 0.3f);
				return Mathf.Abs(axisControl.ReadUnprocessedValue()) > num4;
			}
			if (control is Vector2Control vector2Control && control.name == "delta")
			{
				float num5 = ((CurrentDeviceType != InputDeviceType.KeyboardMouse) ? 2f : 0.5f);
				return vector2Control.ReadValue().sqrMagnitude > num5 * num5;
			}
			if (control is AxisControl axisControl2 && (control.name == "leftTrigger" || control.name == "rightTrigger"))
			{
				float num6 = ((CurrentDeviceType == InputDeviceType.KeyboardMouse) ? 0.5f : 0.2f);
				return axisControl2.ReadValue() > num6;
			}
			if (control is Vector2Control vector2Control2 && control.name == "scroll")
			{
				return vector2Control2.ReadValue().sqrMagnitude > 0.01f;
			}
			if (control.device is Mouse && !(control is ButtonControl))
			{
				return false;
			}
			return true;
		}

		private void OnActionChange(object obj, InputActionChange change)
		{
			if (change == InputActionChange.ActionPerformed && obj is InputAction { activeControl: { } activeControl } && IsMeaningfulInput(activeControl) && (_mirrors == null || !_mirrors.IsMirrorPad(activeControl.device)))
			{
				TrySwitchDevice(ResolveDeviceType(activeControl.device));
			}
		}

		private InputDeviceType ResolveDeviceType(InputDevice device)
		{
			if (IsSimRacingDevice(device))
			{
				_lastWheelDevice = device;
				return InputDeviceType.Wheel;
			}
			if (!(device is Keyboard) && !(device is Mouse))
			{
				if (!(device is DualShockGamepad))
				{
					if (!(device is XInputController))
					{
						if (device is Gamepad)
						{
							return InputDeviceType.GenericGamepad;
						}
						return InputDeviceType.KeyboardMouse;
					}
					return InputDeviceType.Xbox;
				}
				return InputDeviceType.PlayStation;
			}
			return InputDeviceType.KeyboardMouse;
		}

		private bool IsSimRacingDevice(InputDevice device)
		{
			if (!(device is UnityEngine.InputSystem.Joystick) || device is Gamepad)
			{
				return false;
			}
			if (!_simRacingByDeviceId.TryGetValue(device.deviceId, out var value))
			{
				value = WheelClassifier.Classify(WheelDeviceInfo.From(device)) != WheelKind.None;
				_simRacingByDeviceId[device.deviceId] = value;
			}
			return value;
		}

		public static InputDeviceType GetConnectedGamepadType()
		{
			Gamepad gamepad = Instance?._mirrors?.GetPreferred() ?? ((Gamepad.all.Count > 0) ? Gamepad.all[0] : null);
			if (!(gamepad is DualShockGamepad))
			{
				if (gamepad is XInputController)
				{
					return InputDeviceType.Xbox;
				}
				return InputDeviceType.GenericGamepad;
			}
			return InputDeviceType.PlayStation;
		}

		private void TrySwitchDevice(InputDeviceType newType)
		{
			if (newType != CurrentDeviceType && !(Time.unscaledTime - lastSwitchTime < 0.3f))
			{
				lastSwitchTime = Time.unscaledTime;
				CurrentDeviceType = newType;
				OnDeviceTypeChanged?.Invoke(CurrentDeviceType);
			}
		}

		public void Tick()
		{
			PollWheelSteering();
			Gamepad gamepad = ((_mirrors != null) ? _mirrors.GetPreferred() : Gamepad.current);
			if (gamepad != null)
			{
				float num = ((CurrentDeviceType == InputDeviceType.KeyboardMouse) ? 0.6f : 0.3f);
				float num2 = num * num;
				if (!(gamepad.rightStick.ReadUnprocessedValue().sqrMagnitude < num2) || !(gamepad.leftStick.ReadUnprocessedValue().sqrMagnitude < num2))
				{
					TrySwitchDevice(ResolveDeviceType(gamepad));
				}
			}
		}

		private void PollWheelSteering()
		{
			if (!IsWheelConnected())
			{
				_wheelSteerBaseline = false;
				return;
			}
			try
			{
				if (MozaWrapper.AnyPedalEngaged())
				{
					TrySwitchDevice(InputDeviceType.Wheel);
				}
				float mozaSteerAngle = MozaWrapper.GetMozaSteerAngle();
				if (_wheelSteerBaseline && Mathf.Abs(mozaSteerAngle - _lastWheelSteer) > 0.04f)
				{
					TrySwitchDevice(InputDeviceType.Wheel);
				}
				_lastWheelSteer = mozaSteerAngle;
				_wheelSteerBaseline = true;
			}
			catch
			{
			}
		}

		private static bool IsWheelConnected()
		{
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			for (int i = 0; i < devices.Count; i++)
			{
				string text = devices[i].description.product?.ToLowerInvariant() ?? string.Empty;
				if (text.Contains("moza") || text.Contains("wheel"))
				{
					return true;
				}
			}
			return false;
		}
	}
}
