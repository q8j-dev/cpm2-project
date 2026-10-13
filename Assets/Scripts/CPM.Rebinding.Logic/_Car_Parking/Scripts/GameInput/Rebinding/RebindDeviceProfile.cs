using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public sealed class RebindDeviceProfile
	{
		private readonly struct StepKeys
		{
			public readonly string[] Confirm;

			public readonly string[] Cancel;

			public readonly string[] Switch;

			public StepKeys(string[] confirm, string[] cancel, string[] @switch)
			{
				Confirm = confirm;
				Cancel = cancel;
				Switch = @switch;
			}
		}

		private const string MousePrefix = "<Mouse>";

		private const string PointerPrefix = "<Pointer>";

		private const string GamepadPrefix = "<Gamepad>";

		private const string JoystickPrefix = "<Joystick>";

		private const string Enter = "<Keyboard>/enter";

		private const string NumpadEnter = "<Keyboard>/numpadEnter";

		private const string Escape = "<Keyboard>/escape";

		private const string Tab = "<Keyboard>/tab";

		private const string PadSouth = "<Gamepad>/buttonSouth";

		private const string PadEast = "<Gamepad>/buttonEast";

		private const string PadWest = "<Gamepad>/buttonWest";

		private readonly string[] _capturePrefixes;

		private readonly StepKeys _stepKeys;

		private readonly string[] _excludedPaths;

		private readonly CaptureKind[] _kinds;

		private readonly bool _ownsTemplateSlots;

		public RebindDeviceKind Kind { get; }

		public IReadOnlyList<string> CapturePrefixes => _capturePrefixes;

		public IReadOnlyList<string> ConfirmPaths => _stepKeys.Confirm;

		public IReadOnlyList<string> CancelPaths => _stepKeys.Cancel;

		public IReadOnlyList<string> SwitchPaths => _stepKeys.Switch;

		public IReadOnlyList<string> ExcludedPaths => _excludedPaths;

		public string DeviceLayout { get; }

		private RebindDeviceProfile(RebindDeviceKind kind, string[] capturePrefixes, StepKeys stepKeys, string[] excludedPaths, CaptureKind[] kinds, string deviceLayout = null, bool ownsTemplateSlots = false)
		{
			Kind = kind;
			_capturePrefixes = capturePrefixes;
			_stepKeys = stepKeys;
			_excludedPaths = excludedPaths;
			_kinds = kinds;
			DeviceLayout = deviceLayout;
			_ownsTemplateSlots = ownsTemplateSlots;
		}

		public static RebindDeviceProfile CreateKeyboardMouse()
		{
			return new RebindDeviceProfile(RebindDeviceKind.KeyboardMouse, new string[2] { "<Keyboard>", "<Mouse>" }, new StepKeys(new string[2] { "<Keyboard>/enter", "<Keyboard>/numpadEnter" }, new string[1] { "<Keyboard>/escape" }, new string[1] { "<Keyboard>/tab" }), new string[4] { "<Keyboard>/anyKey", "<Mouse>/press", "<Mouse>/position", "<Mouse>/clickCount" }, new CaptureKind[3]
			{
				CaptureKind.Button,
				CaptureKind.Axis,
				CaptureKind.Vector2
			});
		}

		public static RebindDeviceProfile CreateGamepad()
		{
			return new RebindDeviceProfile(RebindDeviceKind.Gamepad, new string[1] { "<Gamepad>" }, new StepKeys(new string[3] { "<Gamepad>/buttonSouth", "<Keyboard>/enter", "<Keyboard>/numpadEnter" }, new string[2] { "<Gamepad>/buttonEast", "<Keyboard>/escape" }, new string[1] { "<Gamepad>/buttonWest" }), Array.Empty<string>(), new CaptureKind[3]
			{
				CaptureKind.Button,
				CaptureKind.Axis,
				CaptureKind.Vector2
			});
		}

		public static RebindDeviceProfile CreateWheel(string deviceLayout, bool ownsTemplateSlots, IReadOnlyList<string> wheelConfirm = null, IReadOnlyList<string> wheelCancel = null)
		{
			return new RebindDeviceProfile(RebindDeviceKind.Wheel, new string[1] { "<" + deviceLayout + ">" }, new StepKeys(Join(new string[3] { "<Keyboard>/enter", "<Keyboard>/numpadEnter", "<Gamepad>/buttonSouth" }, wheelConfirm), Join(new string[2] { "<Keyboard>/escape", "<Gamepad>/buttonEast" }, wheelCancel), new string[2] { "<Keyboard>/tab", "<Gamepad>/buttonWest" }), Array.Empty<string>(), new CaptureKind[3]
			{
				CaptureKind.Button,
				CaptureKind.Axis,
				CaptureKind.Vector2
			}, deviceLayout, ownsTemplateSlots);
		}

		private static string[] Join(string[] fixedKeys, IReadOnlyList<string> extra)
		{
			if (extra == null || extra.Count == 0)
			{
				return fixedKeys;
			}
			List<string> list = new List<string>(fixedKeys);
			foreach (string item in extra)
			{
				if (!string.IsNullOrEmpty(item) && !list.Contains(item))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public bool OwnsSlot(in InputBinding binding)
		{
			string effectivePath = binding.effectivePath;
			if (Owns(effectivePath) || (_ownsTemplateSlots && WheelTemplate.IsTemplateSlot(in binding)))
			{
				return true;
			}
			if (ChordSlots.TryGetDevice(in binding, out var device))
			{
				return device == Kind;
			}
			if (string.IsNullOrEmpty(effectivePath))
			{
				return Owns(binding.path);
			}
			return false;
		}

		public bool OwnsCaptured(string path)
		{
			string[] capturePrefixes = _capturePrefixes;
			foreach (string devicePrefix in capturePrefixes)
			{
				if (BindingPaths.HasDevicePrefix(path, devicePrefix))
				{
					return true;
				}
			}
			return false;
		}

		public bool Owns(string path)
		{
			return OwnsCaptured(path);
		}

		public bool IsForeignCancel(string path)
		{
			if (!OwnsCaptured(path))
			{
				return BindingPaths.OverlapsAny(path, CancelPaths);
			}
			return false;
		}

		public bool IsCancelChord(IReadOnlyList<string> keys)
		{
			if (keys != null && keys.Count == 1)
			{
				return BindingPaths.OverlapsAny(keys[0], CancelPaths);
			}
			return false;
		}

		public bool SupportsKind(CaptureKind kind)
		{
			return Array.IndexOf(_kinds, kind) >= 0;
		}

		public string PathForAxisControl(string deviceLayout, string relativeControlPath)
		{
			return Kind switch
			{
				RebindDeviceKind.Gamepad => "<Gamepad>", 
				RebindDeviceKind.KeyboardMouse => "<Mouse>", 
				_ => "<" + deviceLayout + ">", 
			} + relativeControlPath;
		}

		public string NormalizeCapturedPath(string path)
		{
			if (Kind == RebindDeviceKind.KeyboardMouse && BindingPaths.HasDevicePrefix(path, "<Pointer>"))
			{
				return "<Mouse>" + path.Substring("<Pointer>".Length);
			}
			if (Kind == RebindDeviceKind.Wheel && DeviceLayout != null && BindingPaths.HasDevicePrefix(path, "<Joystick>"))
			{
				return "<" + DeviceLayout + ">" + path.Substring("<Joystick>".Length);
			}
			return path;
		}
	}
}
