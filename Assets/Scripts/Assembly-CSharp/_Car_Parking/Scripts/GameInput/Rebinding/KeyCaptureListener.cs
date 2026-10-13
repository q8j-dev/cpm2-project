using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	internal sealed class KeyCaptureListener : IDisposable
	{
		private const string KeyboardLayout = "Keyboard";

		private const string MouseLayout = "Mouse";

		private const string GamepadLayout = "Gamepad";

		private const InputControlExtensions.Enumerate ChangedButtons = InputControlExtensions.Enumerate.IgnoreControlsInCurrentState | InputControlExtensions.Enumerate.IncludeSyntheticControls;

		private const InputControlExtensions.Enumerate ChangedRealButtons = InputControlExtensions.Enumerate.IgnoreControlsInCurrentState;

		private const float ScrollKeyDistance = 0.01f;

		private const double DeltaKeyReleaseSeconds = 0.25;

		private const string MouseScroll = "scroll";

		private readonly RebindDeviceProfile _profile;

		private readonly Action<InputEventPtr, InputDevice> _onEvent;

		private readonly List<(ButtonControl control, string path)> _held = new List<(ButtonControl, string)>();

		private readonly List<(ButtonControl control, string path)> _pressedInEvent = new List<(ButtonControl, string)>();

		private readonly Dictionary<string, HashSet<string>> _layoutControls = new Dictionary<string, HashSet<string>>();

		private readonly AxisControlPoller _analog;

		private readonly DeltaDirectionTracker _scroll;

		private readonly string[] _scrollPaths = new string[4];

		private bool _hooked;

		public ChordTracker Tracker { get; } = new ChordTracker();

		public bool Cancelled { get; private set; }

		public KeyCaptureListener(RebindDeviceProfile profile, AxisControlPoller analog = null)
		{
			_profile = profile;
			_analog = analog;
			_onEvent = OnEvent;
			if (analog == null && profile.Kind == RebindDeviceKind.KeyboardMouse)
			{
				_scroll = new DeltaDirectionTracker(0.01f, 0.25);
				DeltaDirection[] array = new DeltaDirection[4]
				{
					DeltaDirection.Up,
					DeltaDirection.Down,
					DeltaDirection.Left,
					DeltaDirection.Right
				};
				foreach (DeltaDirection deltaDirection in array)
				{
					_scrollPaths[(int)deltaDirection] = "<Mouse>/scroll/" + DeltaDirectionTracker.NameOf(deltaDirection);
				}
			}
		}

		public void Start()
		{
			Restart();
			if (!_hooked)
			{
				InputSystem.onEvent += _onEvent;
				Application.focusChanged += OnFocusChanged;
				_hooked = true;
			}
		}

		public void Restart()
		{
			_held.Clear();
			List<string> list = new List<string>();
			foreach (InputDevice device in InputSystem.devices)
			{
				foreach (InputControl allControl in device.allControls)
				{
					if (allControl is ButtonControl { isPressed: not false } buttonControl && IsKeyCandidate(buttonControl) && TryGetOwnedPath(buttonControl, out var path))
					{
						_held.Add((buttonControl, path));
						list.Add(path);
					}
				}
			}
			_scroll?.Reset();
			Tracker.Reset(list);
		}

		public void Tick(double time)
		{
			if (_scroll != null && _scroll.Tick(time, out var released))
			{
				Tracker.Release(_scrollPaths[(int)released]);
			}
		}

		public void Dispose()
		{
			if (_hooked)
			{
				InputSystem.onEvent -= _onEvent;
				Application.focusChanged -= OnFocusChanged;
				_hooked = false;
			}
		}

		private void OnFocusChanged(bool focused)
		{
			if (!focused)
			{
				Restart();
			}
			else
			{
				_analog?.UseEvents();
			}
		}

		private void OnEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (Cancelled || (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()))
			{
				return;
			}
			_analog?.Sample(eventPtr, device);
			bool flag = ReleaseHeld(eventPtr, device);
			_pressedInEvent.Clear();
			bool flag2 = false;
			foreach (InputControl item in eventPtr.EnumerateControls((_analog == null) ? (InputControlExtensions.Enumerate.IgnoreControlsInCurrentState | InputControlExtensions.Enumerate.IncludeSyntheticControls) : InputControlExtensions.Enumerate.IgnoreControlsInCurrentState, device))
			{
				if (item is ButtonControl buttonControl && !IsHeld(buttonControl) && IsKeyCandidate(buttonControl) && buttonControl.ReadValueFromEvent(eventPtr, out var value) && buttonControl.IsValueConsideredPressed(value) && TryGetPath(buttonControl, out var path))
				{
					if (_profile.IsForeignCancel(path))
					{
						Cancelled = true;
						return;
					}
					if (_profile.OwnsCaptured(path) && !BindingPaths.OverlapsAny(path, _profile.ExcludedPaths))
					{
						_pressedInEvent.Add((buttonControl, path));
						flag2 |= !buttonControl.synthetic;
					}
				}
			}
			foreach (var (buttonControl2, text) in _pressedInEvent)
			{
				if (!flag2 || !buttonControl2.synthetic)
				{
					_held.Add((buttonControl2, text));
					flag |= Tracker.Press(text);
				}
			}
			if (_scroll != null && device is Mouse mouse)
			{
				flag |= FeedScroll(mouse, eventPtr);
			}
			if (flag || HoldsTrackedKeyOn(device))
			{
				eventPtr.handled = true;
			}
		}

		private bool IsKeyCandidate(ButtonControl button)
		{
			if (_analog != null)
			{
				if (!button.synthetic && !_analog.Contains(button))
				{
					return !_analog.Contains(button.parent);
				}
				return false;
			}
			return true;
		}

		private bool FeedScroll(Mouse mouse, InputEventPtr eventPtr)
		{
			if (mouse.scroll.ReadUnprocessedValueFromEvent(eventPtr, out var value) && _scroll.Feed(0f, value.y, Time.realtimeSinceStartupAsDouble))
			{
				return Tracker.Press(_scrollPaths[(int)_scroll.Pressed.GetValueOrDefault()]);
			}
			return false;
		}

		private bool ReleaseHeld(InputEventPtr eventPtr, InputDevice device)
		{
			bool flag = false;
			for (int num = _held.Count - 1; num >= 0; num--)
			{
				var (buttonControl, path) = _held[num];
				if (buttonControl.device == device && buttonControl.ReadValueFromEvent(eventPtr, out var value) && !(value >= buttonControl.pressPointOrDefault * InputSystem.settings.buttonReleaseThreshold))
				{
					flag |= Tracker.IsTracking(path);
					Tracker.Release(path);
					_held.RemoveAt(num);
				}
			}
			return flag;
		}

		private bool HoldsTrackedKeyOn(InputDevice device)
		{
			foreach (var (buttonControl, path) in _held)
			{
				if (buttonControl.device == device && Tracker.IsTracking(path))
				{
					return true;
				}
			}
			if (device is Mouse)
			{
				DeltaDirection? deltaDirection = _scroll?.Pressed;
				if (deltaDirection.HasValue)
				{
					DeltaDirection valueOrDefault = deltaDirection.GetValueOrDefault();
					return Tracker.IsTracking(_scrollPaths[(int)valueOrDefault]);
				}
			}
			return false;
		}

		private bool IsHeld(ButtonControl button)
		{
			foreach (var item in _held)
			{
				if (item.control == button)
				{
					return true;
				}
			}
			return false;
		}

		private bool TryGetOwnedPath(ButtonControl button, out string path)
		{
			if (TryGetPath(button, out path) && _profile.OwnsCaptured(path))
			{
				return !BindingPaths.OverlapsAny(path, _profile.ExcludedPaths);
			}
			return false;
		}

		private bool TryGetPath(InputControl control, out string path)
		{
			path = null;
			InputDevice device = control.device;
			if (device == null || control == device)
			{
				return false;
			}
			string text = control.path.Substring(device.path.Length);
			string text2 = ((device is Keyboard) ? "Keyboard" : ((device is Mouse) ? "Mouse" : ((!(device is Gamepad)) ? null : "Gamepad")));
			string text3 = text2;
			if (text3 == null)
			{
				path = "<" + device.layout + ">" + text;
				return true;
			}
			if (!LayoutHasControl(text3, text))
			{
				return false;
			}
			path = _profile.NormalizeCapturedPath("<" + text3 + ">" + text);
			return true;
		}

		private bool LayoutHasControl(string layout, string relative)
		{
			if (!_layoutControls.TryGetValue(layout, out var value))
			{
				value = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				InputControlLayout inputControlLayout = InputSystem.LoadLayout(layout);
				if (inputControlLayout != null)
				{
					foreach (InputControlLayout.ControlItem control in inputControlLayout.controls)
					{
						string text = control.name.ToString();
						int num = text.IndexOf('/');
						value.Add((num >= 0) ? text.Substring(0, num) : text);
					}
				}
				_layoutControls[layout] = value;
			}
			string text2 = relative.TrimStart('/');
			int num2 = text2.IndexOf('/');
			return value.Contains((num2 >= 0) ? text2.Substring(0, num2) : text2);
		}
	}
}
