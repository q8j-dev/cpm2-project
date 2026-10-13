using System;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class SlotKinds
	{
		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		private static readonly string[] AxisNames = new string[10] { "x", "y", "z", "rx", "ry", "rz", "slider", "throttle", "rudder", "wheel" };

		private static readonly string[] Vector2Names = new string[7] { "stick", "hat", "dpad", "delta", "scroll", "leftStick", "rightStick" };

		public static CaptureKind KindOf(InputAction action, int index)
		{
			InputBinding inputBinding = action.bindings[index];
			if (TryKindOfPath(inputBinding.path, out var kind) || TryKindOfPath(inputBinding.effectivePath, out kind))
			{
				return kind;
			}
			if (TryKindOfControlName(ControlNameOf(inputBinding.path), out kind))
			{
				return kind;
			}
			if (!inputBinding.isPartOfComposite && TryKindOfLayout(action.expectedControlType, out kind))
			{
				return kind;
			}
			if (action.type != InputActionType.Button)
			{
				return CaptureKind.Axis;
			}
			return CaptureKind.Button;
		}

		public static AxisSemantic SemanticOf(InputAction action, int index)
		{
			InputBinding inputBinding = action.bindings[index];
			string text = (string.IsNullOrEmpty(inputBinding.effectivePath) ? (inputBinding.path ?? string.Empty) : inputBinding.effectivePath);
			if (text.StartsWith("<Mouse>/delta", StringComparison.OrdinalIgnoreCase) || text.StartsWith("<Mouse>/scroll", StringComparison.OrdinalIgnoreCase))
			{
				return AxisSemantic.Delta;
			}
			if (!Contains(inputBinding.effectiveProcessors, "SliderRemap") && !Contains(inputBinding.effectiveInteractions, "SliderPress"))
			{
				return AxisSemantic.Bipolar;
			}
			return AxisSemantic.Pedal;
		}

		private static bool Contains(string list, string name)
		{
			if (!string.IsNullOrEmpty(list))
			{
				return list.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return false;
		}

		public static bool TryKindOfPath(string path, out CaptureKind kind)
		{
			kind = CaptureKind.Button;
			if (string.IsNullOrEmpty(path) || path[0] == '*')
			{
				return false;
			}
			return TryKindOfLayout(InputControlPath.TryGetControlLayout(path), out kind);
		}

		public static CaptureKind KindOfControlName(string control)
		{
			if (!TryKindOfControlName(control, out var kind))
			{
				return CaptureKind.Button;
			}
			return kind;
		}

		private static bool TryKindOfLayout(string layout, out CaptureKind kind)
		{
			kind = CaptureKind.Button;
			if (string.IsNullOrEmpty(layout))
			{
				return false;
			}
			if (IsBasedOn(layout, "Button"))
			{
				return true;
			}
			if (IsBasedOn(layout, "Vector2"))
			{
				kind = CaptureKind.Vector2;
				return true;
			}
			if (IsBasedOn(layout, "Axis"))
			{
				kind = CaptureKind.Axis;
				return true;
			}
			return false;
		}

		private static bool TryKindOfControlName(string control, out CaptureKind kind)
		{
			kind = CaptureKind.Button;
			if (string.IsNullOrEmpty(control))
			{
				return false;
			}
			int num = control.LastIndexOf('/');
			string leaf = ((num >= 0) ? control.Substring(num + 1) : control);
			if (num >= 0 && IsDirection(leaf))
			{
				return true;
			}
			if (Array.Exists(AxisNames, (string n) => string.Equals(n, leaf, StringComparison.OrdinalIgnoreCase)))
			{
				kind = CaptureKind.Axis;
				return true;
			}
			if (num < 0 && Array.Exists(Vector2Names, (string n) => string.Equals(n, leaf, StringComparison.OrdinalIgnoreCase)))
			{
				kind = CaptureKind.Vector2;
				return true;
			}
			if (!leaf.StartsWith("button", StringComparison.OrdinalIgnoreCase) && !string.Equals(leaf, "trigger", StringComparison.OrdinalIgnoreCase) && !leaf.EndsWith("Button", StringComparison.OrdinalIgnoreCase) && !leaf.EndsWith("Press", StringComparison.OrdinalIgnoreCase) && !leaf.EndsWith("Shoulder", StringComparison.OrdinalIgnoreCase))
			{
				return leaf.EndsWith("Trigger", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		public static bool IsDirectionPath(string path)
		{
			string text = ControlNameOf(path);
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			int num = text.LastIndexOf('/');
			if (num > 0)
			{
				return IsDirection(text.Substring(num + 1));
			}
			return false;
		}

		public static bool IsRelativeDirectionPath(string path)
		{
			if (IsDirectionPath(path) && path != null)
			{
				if (path.IndexOf("/delta/", StringComparison.OrdinalIgnoreCase) < 0)
				{
					return path.IndexOf("/scroll/", StringComparison.OrdinalIgnoreCase) >= 0;
				}
				return true;
			}
			return false;
		}

		public static bool IsMouseMoveDirectionPath(string path)
		{
			if (IsDirectionPath(path))
			{
				return path.IndexOf("/delta/", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return false;
		}

		private static bool IsDirection(string leaf)
		{
			if (!string.Equals(leaf, "up", StringComparison.OrdinalIgnoreCase) && !string.Equals(leaf, "down", StringComparison.OrdinalIgnoreCase) && !string.Equals(leaf, "left", StringComparison.OrdinalIgnoreCase))
			{
				return string.Equals(leaf, "right", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		private static string ControlNameOf(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}
			int num = path.IndexOf('>');
			if (num < 0 || num + 2 >= path.Length || path[num + 1] != '/')
			{
				return null;
			}
			return path.Substring(num + 2);
		}

		private static bool IsBasedOn(string layout, string baseLayout)
		{
			if (!string.Equals(layout, baseLayout, StringComparison.OrdinalIgnoreCase))
			{
				return InputSystem.IsFirstLayoutBasedOnSecond(layout, baseLayout);
			}
			return true;
		}
	}
}
