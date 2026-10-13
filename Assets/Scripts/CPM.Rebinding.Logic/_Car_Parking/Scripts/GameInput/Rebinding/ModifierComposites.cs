using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class ModifierComposites
	{
		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		public static bool IsModifierPart(in InputBinding binding)
		{
			if (binding.isPartOfComposite && binding.name != null)
			{
				return binding.name.StartsWith("modifier", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public static bool IsInModifierComposite(ReadOnlyArray<InputBinding> bindings, int partIndex)
		{
			if (TryFindHeader(bindings, partIndex, out var header))
			{
				return IsModifierHeader(bindings[header]);
			}
			return false;
		}

		public static bool HasUnboundModifier(InputAction action, int index, RebindDeviceProfile profile)
		{
			if (profile != null)
			{
				return HasUnboundModifierWhere(action, index, (InputBinding part) => profile.OwnsSlot(in part));
			}
			return false;
		}

		public static bool HasUnboundModifier(InputAction action, int index)
		{
			return HasUnboundModifierWhere(action, index, null);
		}

		public static int RebindSlotOf(InputAction action, int index, RebindDeviceProfile profile)
		{
			if (action == null || profile == null)
			{
				return index;
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			if (index < 0 || index >= bindings.Count || !bindings[index].isPartOfComposite || IsModifierPart(bindings[index]) || string.IsNullOrEmpty(bindings[index].effectivePath))
			{
				return index;
			}
			if (!TryFindHeader(bindings, index, out var header) || !IsModifierHeader(bindings[header]))
			{
				return index;
			}
			int num = -1;
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				InputBinding binding = bindings[i];
				if (IsModifierPart(in binding) && profile.OwnsSlot(in binding))
				{
					if (string.IsNullOrEmpty(binding.effectivePath))
					{
						return i;
					}
					if (num < 0)
					{
						num = i;
					}
				}
			}
			if (num < 0)
			{
				return index;
			}
			return num;
		}

		public static int MainPartOf(ReadOnlyArray<InputBinding> bindings, int index)
		{
			if (index < 0 || index >= bindings.Count || !IsModifierPart(bindings[index]) || !TryFindHeader(bindings, index, out var header))
			{
				return index;
			}
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				if (!IsModifierPart(bindings[i]))
				{
					return i;
				}
			}
			return index;
		}

		private static bool HasUnboundModifierWhere(InputAction action, int index, Predicate<InputBinding> onDevice)
		{
			if (action == null)
			{
				return false;
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			if (index < 0 || index >= bindings.Count || !bindings[index].isPartOfComposite || IsModifierPart(bindings[index]))
			{
				return false;
			}
			if (!TryFindHeader(bindings, index, out var header) || !IsModifierHeader(bindings[header]))
			{
				return false;
			}
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				InputBinding binding = bindings[i];
				if (IsModifierPart(in binding) && string.IsNullOrEmpty(binding.effectivePath) && (onDevice == null || onDevice(binding)))
				{
					return true;
				}
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

		private static bool TryFindHeader(ReadOnlyArray<InputBinding> bindings, int partIndex, out int header)
		{
			for (header = partIndex - 1; header >= 0; header--)
			{
				if (bindings[header].isComposite)
				{
					return true;
				}
			}
			return false;
		}
	}
}
