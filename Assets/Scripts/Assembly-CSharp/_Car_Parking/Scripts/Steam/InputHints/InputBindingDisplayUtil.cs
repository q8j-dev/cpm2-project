using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.Steam.InputHints
{
	public static class InputBindingDisplayUtil
	{
		private const string KeyboardPrefix = "<Keyboard>";

		private const string MousePrefix = "<Mouse>";

		private const string GamepadPrefix = "<Gamepad>";

		private const string WheelPrefix = "<HID::Gudsen MOZA R5 Base>";

		public static void GetDisplayParts(InputAction action, ForcedDeviceType device, List<string> results)
		{
			results.Clear();
			if (action == null)
			{
				return;
			}
			string value;
			string text;
			switch (device)
			{
			case ForcedDeviceType.KeyboardMouse:
				value = "<Keyboard>";
				text = "<Mouse>";
				break;
			case ForcedDeviceType.Wheel:
				value = "<HID::Gudsen MOZA R5 Base>";
				text = null;
				break;
			default:
				value = "<Gamepad>";
				text = null;
				break;
			}
			bool flag = false;
			for (int i = 0; i < action.bindings.Count; i++)
			{
				InputBinding inputBinding = action.bindings[i];
				if (inputBinding.isComposite)
				{
					continue;
				}
				string effectivePath = inputBinding.effectivePath;
				if (string.IsNullOrEmpty(effectivePath) || (!effectivePath.StartsWith(value, StringComparison.OrdinalIgnoreCase) && (text == null || !effectivePath.StartsWith(text, StringComparison.OrdinalIgnoreCase))))
				{
					continue;
				}
				if (inputBinding.isPartOfComposite)
				{
					if (!IsModifierPartName(inputBinding.name) && !string.IsNullOrEmpty(inputBinding.name) && !ContainsIgnoreCase(results, inputBinding.name))
					{
						results.Add(inputBinding.name);
					}
				}
				else if (!flag)
				{
					flag = true;
					results.Insert(0, string.Empty);
				}
			}
		}

		public static void GetDisplaySlots(InputAction action, Predicate<InputBinding> ownsSlot, List<(string part, int index)> results)
		{
			results.Clear();
			if (action == null || ownsSlot == null)
			{
				return;
			}
			for (int i = 0; i < action.bindings.Count; i++)
			{
				InputBinding obj = action.bindings[i];
				if (obj.isComposite || !ownsSlot(obj))
				{
					continue;
				}
				string text = (obj.isPartOfComposite ? (obj.name ?? string.Empty) : string.Empty);
				if (obj.isPartOfComposite && (IsModifierPartName(text) || text.Length == 0))
				{
					continue;
				}
				bool flag = false;
				foreach (var result in results)
				{
					if (string.Equals(result.part, text, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					results.Add((text, i));
				}
			}
		}

		public static bool BindingBelongsToDevice(in InputBinding binding, ForcedDeviceType device)
		{
			string path = binding.path;
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			switch (device)
			{
			case ForcedDeviceType.KeyboardMouse:
				if (!path.StartsWith("<Keyboard>", StringComparison.OrdinalIgnoreCase))
				{
					return path.StartsWith("<Mouse>", StringComparison.OrdinalIgnoreCase);
				}
				return true;
			case ForcedDeviceType.Wheel:
				return path.StartsWith("<HID::Gudsen MOZA R5 Base>", StringComparison.OrdinalIgnoreCase);
			default:
				return path.StartsWith("<Gamepad>", StringComparison.OrdinalIgnoreCase);
			}
		}

		public static bool ActionHasHold(InputAction action)
		{
			if (action == null)
			{
				return false;
			}
			if (ContainsHold(action.interactions))
			{
				return true;
			}
			for (int i = 0; i < action.bindings.Count; i++)
			{
				if (ContainsHold(action.bindings[i].interactions))
				{
					return true;
				}
			}
			return false;
		}

		private static bool ContainsHold(string interactions)
		{
			if (!string.IsNullOrEmpty(interactions))
			{
				return interactions.IndexOf("hold", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return false;
		}

		public static bool IsModifierPartName(string partName)
		{
			return partName?.StartsWith("modifier", StringComparison.OrdinalIgnoreCase) ?? false;
		}

		public static bool IsPlainBindingPartName(string partName)
		{
			if (!string.Equals(partName, "binding", StringComparison.OrdinalIgnoreCase))
			{
				return string.Equals(partName, "button", StringComparison.OrdinalIgnoreCase);
			}
			return true;
		}

		private static bool ContainsIgnoreCase(List<string> list, string value)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (string.Equals(list[i], value, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}
	}
}
