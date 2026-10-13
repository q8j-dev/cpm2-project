using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class AxisControlPoller
	{
		private const float ScrollNotchWeight = 150f;

		private readonly RebindDeviceProfile _profile;

		private readonly CaptureKind _kind;

		private readonly List<InputControl> _controls = new List<InputControl>();

		private float[] _values = new float[0];

		private float[] _eventValues = new float[0];

		private bool[] _isDelta = new bool[0];

		private bool _fromEvents;

		public int Count => _controls.Count;

		public AxisControlPoller(RebindDeviceProfile profile, CaptureKind kind)
		{
			_profile = profile;
			_kind = kind;
			foreach (InputDevice device in InputSystem.devices)
			{
				if (!IsPageDevice(device))
				{
					continue;
				}
				foreach (InputControl allControl in device.allControls)
				{
					if (IsCandidate(allControl))
					{
						_controls.Add(allControl);
					}
				}
			}
			_values = new float[_controls.Count];
			_eventValues = new float[_controls.Count];
			_isDelta = new bool[_controls.Count];
			for (int i = 0; i < _controls.Count; i++)
			{
				_isDelta[i] = IsRelative(_controls[i]);
			}
		}

		public IReadOnlyList<float> Read()
		{
			for (int i = 0; i < _controls.Count; i++)
			{
				if (!_fromEvents)
				{
					_values[i] = ValueOf(_controls[i]);
					continue;
				}
				_values[i] = _eventValues[i];
				if (_isDelta[i])
				{
					_eventValues[i] = 0f;
				}
			}
			return _values;
		}

		public void UseEvents()
		{
			for (int i = 0; i < _controls.Count; i++)
			{
				_eventValues[i] = (_isDelta[i] ? 0f : ValueOf(_controls[i]));
			}
			_fromEvents = true;
		}

		public void Sample(InputEventPtr eventPtr, InputDevice device)
		{
			if (!_fromEvents)
			{
				return;
			}
			for (int i = 0; i < _controls.Count; i++)
			{
				InputControl inputControl = _controls[i];
				if (inputControl.device == device && TryReadFromEvent(inputControl, eventPtr, out var value))
				{
					_eventValues[i] = (_isDelta[i] ? (_eventValues[i] + value) : value);
				}
			}
		}

		public bool Contains(InputControl control)
		{
			return _controls.Contains(control);
		}

		public string PathOf(int index)
		{
			InputControl inputControl = _controls[index];
			string relativeControlPath = inputControl.path.Substring(inputControl.device.path.Length);
			return _profile.PathForAxisControl(inputControl.device.layout, relativeControlPath);
		}

		private bool IsPageDevice(InputDevice device)
		{
			return _profile.Kind switch
			{
				RebindDeviceKind.KeyboardMouse => device is Mouse, 
				RebindDeviceKind.Gamepad => device is Gamepad, 
				_ => _profile.OwnsCaptured("<" + device.layout + ">/x"), 
			};
		}

		private bool IsCandidate(InputControl control)
		{
			if (control.synthetic || IsExcluded(control))
			{
				return false;
			}
			if (_kind == CaptureKind.Vector2)
			{
				return control is Vector2Control;
			}
			if (!(control is AxisControl) || control.children.Count > 0)
			{
				return false;
			}
			if (control is ButtonControl button)
			{
				return IsAnalog(button);
			}
			return true;
		}

		private bool IsExcluded(InputControl control)
		{
			foreach (string excludedPath in _profile.ExcludedPaths)
			{
				if (InputControlPath.MatchesPrefix(excludedPath, control))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsAnalog(ButtonControl button)
		{
			if (button.stateBlock.sizeInBits <= 1)
			{
				return button.stateBlock.format == InputStateBlock.FormatFloat;
			}
			return true;
		}

		private static float ValueOf(InputControl control)
		{
			float num = ((control is Vector2Control vector2Control) ? vector2Control.ReadUnprocessedValue().magnitude : ((!(control is AxisControl axisControl)) ? 0f : axisControl.ReadUnprocessedValue()));
			float num2 = num;
			if (!IsScroll(control))
			{
				return num2;
			}
			return num2 * 150f;
		}

		private static bool TryReadFromEvent(InputControl control, InputEventPtr eventPtr, out float value)
		{
			value = 0f;
			if (!(control is Vector2Control control2))
			{
				if (!(control is AxisControl control3) || !control3.ReadUnprocessedValueFromEvent(eventPtr, out var value2))
				{
					goto IL_0048;
				}
				value = value2;
			}
			else
			{
				if (!control2.ReadUnprocessedValueFromEvent(eventPtr, out var value3))
				{
					goto IL_0048;
				}
				value = value3.magnitude;
			}
			if (IsScroll(control))
			{
				value *= 150f;
			}
			return true;
			IL_0048:
			return false;
		}

		private static bool IsScroll(InputControl control)
		{
			return control.path.IndexOf("/scroll", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		private static bool IsRelative(InputControl control)
		{
			if (!IsScroll(control))
			{
				return control.path.IndexOf("/delta", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return true;
		}
	}
}
