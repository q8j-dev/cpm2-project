using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class ChordSlots
	{
		public const string CompositeName = "OneModifier";

		public const string HeaderName = "RebindCombo";

		public const string ModifierPart = "modifier";

		public const string MainPart = "binding";

		private const string MarkerPrefix = "RebindCombo.";

		private const string DefaultGroup = "Default";

		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		public static Guid IdFor(Guid actionId, RebindDeviceKind device, string role)
		{
			using MD5 mD = MD5.Create();
			return new Guid(mD.ComputeHash(Encoding.UTF8.GetBytes($"{actionId:N}|{device}|{role}")));
		}

		public static string GroupsFor(RebindDeviceKind device)
		{
			return ";Default;RebindCombo." + device;
		}

		public static bool IsRuntime(in InputBinding binding)
		{
			RebindDeviceKind device;
			return TryGetDevice(in binding, out device);
		}

		public static bool TryGetDevice(in InputBinding binding, out RebindDeviceKind device)
		{
			device = RebindDeviceKind.KeyboardMouse;
			string groups = binding.groups;
			if (string.IsNullOrEmpty(groups) || groups.IndexOf("RebindCombo.", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return false;
			}
			string[] array = groups.Split(';');
			foreach (string text in array)
			{
				if (text.StartsWith("RebindCombo.", StringComparison.OrdinalIgnoreCase) && Enum.TryParse<RebindDeviceKind>(text.Substring("RebindCombo.".Length), ignoreCase: true, out device))
				{
					return true;
				}
			}
			return false;
		}

		public static bool BelongsTo(in InputBinding binding, RebindDeviceProfile profile)
		{
			if (TryGetDevice(in binding, out var device))
			{
				return device == profile.Kind;
			}
			return profile.Owns(binding.path);
		}

		public static bool TryGetPair(InputAction action, int index, RebindDeviceProfile profile, out ChordPair pair)
		{
			if (TryGetPair(action, profile, out pair))
			{
				return pair.Contains(index);
			}
			return false;
		}

		public static bool TryGetPair(InputAction action, RebindDeviceProfile profile, out ChordPair pair)
		{
			pair = default;
			if (action == null || profile == null || profile.Kind == RebindDeviceKind.Wheel)
			{
				return false;
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			int num = -1;
			int num2 = 0;
			int num3 = -1;
			int num4 = 0;
			for (int i = 0; i < bindings.Count; i++)
			{
				InputBinding binding = bindings[i];
				if (binding.isComposite)
				{
					if (IsModifierHeader(in binding) && AnyPartBelongs(bindings, i, profile))
					{
						if (!IsOneModifier(in binding) || !TryGetParts(bindings, i, out var _, out var _) || !AllPartsBelong(bindings, i, profile))
						{
							return false;
						}
						num4++;
						num3 = i;
					}
				}
				else if (!binding.isPartOfComposite && BelongsTo(in binding, profile))
				{
					num2++;
					num = i;
				}
			}
			if (num2 != 1 || num4 != 1 || !TryGetParts(bindings, num3, out var modifier2, out var main2))
			{
				return false;
			}
			bool num5 = !IsRuntime(bindings[num]);
			bool flag = !IsRuntime(bindings[num3]);
			CaptureKind captureKind = (num5 ? SlotKinds.KindOf(action, num) : SlotKinds.KindOf(action, main2));
			if ((num5 & flag) && SlotKinds.KindOf(action, main2) != captureKind)
			{
				return false;
			}
			pair = new ChordPair(action, num, num3, modifier2, main2, captureKind);
			return true;
		}

		public static bool TryGetComboOnly(InputAction action, int index, RebindDeviceProfile profile, out ComboOnlySlots slots)
		{
			slots = default;
			if (action == null || profile == null || profile.Kind == RebindDeviceKind.Wheel)
			{
				return false;
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			if (index < 0 || index >= bindings.Count || !bindings[index].isPartOfComposite)
			{
				return false;
			}
			int num = index - 1;
			while (num >= 0 && !bindings[num].isComposite)
			{
				num--;
			}
			if (num < 0 || !IsOneModifier(bindings[num]) || IsRuntime(bindings[num]) || !TryGetParts(bindings, num, out var modifier, out var main) || index != main || !AllPartsBelong(bindings, num, profile) || (TryGetPair(action, profile, out var pair) && pair.Contains(index)))
			{
				return false;
			}
			slots = new ComboOnlySlots(num, modifier, main);
			return true;
		}

		public static bool IsOneModifier(in InputBinding header)
		{
			if (header.isComposite && header.path != null)
			{
				return header.path.StartsWith("OneModifier", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		private static bool IsModifierHeader(in InputBinding header)
		{
			if (header.path != null)
			{
				return header.path.IndexOf("Modifier", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return false;
		}

		private static bool TryGetParts(ReadOnlyArray<InputBinding> bindings, int header, out int modifier, out int main)
		{
			modifier = -1;
			main = -1;
			int num = 0;
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				num++;
				if (string.Equals(bindings[i].name, "modifier", StringComparison.OrdinalIgnoreCase))
				{
					modifier = i;
				}
				else if (string.Equals(bindings[i].name, "binding", StringComparison.OrdinalIgnoreCase))
				{
					main = i;
				}
			}
			if (num == 2 && modifier >= 0)
			{
				return main >= 0;
			}
			return false;
		}

		private static bool AnyPartBelongs(ReadOnlyArray<InputBinding> bindings, int header, RebindDeviceProfile profile)
		{
			if (TryGetDevice(bindings[header], out var device))
			{
				return device == profile.Kind;
			}
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				if (BelongsTo(bindings[i], profile))
				{
					return true;
				}
			}
			return false;
		}

		private static bool AllPartsBelong(ReadOnlyArray<InputBinding> bindings, int header, RebindDeviceProfile profile)
		{
			if (TryGetDevice(bindings[header], out var device))
			{
				return device == profile.Kind;
			}
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				if (!BelongsTo(bindings[i], profile))
				{
					return false;
				}
			}
			return true;
		}
	}
}
