using System;
using System.Collections.Generic;
using System.Linq;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class BindingPaths
	{
		public const string KeyboardPrefix = "<Keyboard>";

		private static readonly (string Umbrella, string Left, string Right)[] KeyboardModifiers = new (string, string, string)[3]
		{
			("shift", "leftShift", "rightShift"),
			("ctrl", "leftCtrl", "rightCtrl"),
			("alt", "leftAlt", "rightAlt")
		};

		public static bool HasDevicePrefix(string path, string devicePrefix)
		{
			if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(devicePrefix))
			{
				return false;
			}
			if (path.Length > devicePrefix.Length && path[devicePrefix.Length] == '/')
			{
				return path.StartsWith(devicePrefix, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public static string Normalize(string path)
		{
			if (!TrySplit(path, out var device, out var control) || !Same(device, "<Keyboard>"))
			{
				return path;
			}
			(string, string, string)[] keyboardModifiers = KeyboardModifiers;
			for (int i = 0; i < keyboardModifiers.Length; i++)
			{
				var (text, b, b2) = keyboardModifiers[i];
				if (Same(control, text) || Same(control, b) || Same(control, b2))
				{
					return "<Keyboard>/" + text;
				}
			}
			return path;
		}

		public static bool Overlap(string a, string b)
		{
			if (!TrySplit(a, out var device, out var control) || !TrySplit(b, out var device2, out var control2) || !Same(device, device2))
			{
				return false;
			}
			if (Same(control, control2))
			{
				return true;
			}
			if (Same(device, "<Keyboard>"))
			{
				return KeyboardModifiersOverlap(control, control2);
			}
			if (!IsChildOf(control, control2) && !IsChildOf(control2, control) && !DirectionOnAxis(control, control2))
			{
				return DirectionOnAxis(control2, control);
			}
			return true;
		}

		private static bool KeyboardModifiersOverlap(string controlA, string controlB)
		{
			(string, string, string)[] keyboardModifiers = KeyboardModifiers;
			for (int i = 0; i < keyboardModifiers.Length; i++)
			{
				var (b, b2, b3) = keyboardModifiers[i];
				if (Same(controlA, b) && (Same(controlB, b2) || Same(controlB, b3)))
				{
					return true;
				}
				if (Same(controlB, b) && (Same(controlA, b2) || Same(controlA, b3)))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsChildOf(string child, string parent)
		{
			if (child.Length > parent.Length + 1 && child[parent.Length] == '/')
			{
				return child.StartsWith(parent, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private static bool DirectionOnAxis(string direction, string axis)
		{
			int num = direction.LastIndexOf('/');
			int num2 = axis.LastIndexOf('/');
			if (num < 0 || num2 < 0 || num != num2 || string.Compare(direction, 0, axis, 0, num, StringComparison.OrdinalIgnoreCase) != 0)
			{
				return false;
			}
			string a = direction.Substring(num + 1);
			string a2 = axis.Substring(num2 + 1);
			if (Same(a2, "x"))
			{
				if (!Same(a, "left"))
				{
					return Same(a, "right");
				}
				return true;
			}
			if (Same(a2, "y"))
			{
				if (!Same(a, "up"))
				{
					return Same(a, "down");
				}
				return true;
			}
			return false;
		}

		public static bool OverlapsAny(string path, IReadOnlyList<string> candidates)
		{
			return candidates?.Any((string t) => Overlap(path, t)) ?? false;
		}

		private static bool TrySplit(string path, out string device, out string control)
		{
			device = null;
			control = null;
			if (string.IsNullOrEmpty(path) || path[0] != '<')
			{
				return false;
			}
			int num = path.IndexOf('>');
			if (num < 0 || num + 2 >= path.Length || path[num + 1] != '/')
			{
				return false;
			}
			device = path.Substring(0, num + 1);
			control = path.Substring(num + 2);
			return true;
		}

		private static bool Same(string a, string b)
		{
			return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
