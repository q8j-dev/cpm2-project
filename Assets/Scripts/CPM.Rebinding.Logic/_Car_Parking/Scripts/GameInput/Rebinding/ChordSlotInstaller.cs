using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class ChordSlotInstaller
	{
		private const string PlainRole = "plain";

		private const string HeaderRole = "header";

		public static int Install(InputActionAsset asset)
		{
			if (asset == null)
			{
				return 0;
			}
			RebindDeviceProfile[] array = new RebindDeviceProfile[2]
			{
				RebindDeviceProfile.CreateKeyboardMouse(),
				RebindDeviceProfile.CreateGamepad()
			};
			List<InputAction> list = new List<InputAction>();
			foreach (InputActionMap actionMap in asset.actionMaps)
			{
				list.AddRange(actionMap.actions);
			}
			int num = 0;
			foreach (InputAction item in list)
			{
				RebindDeviceProfile[] array2 = array;
				foreach (RebindDeviceProfile profile in array2)
				{
					if (TryComplete(item, profile))
					{
						num++;
					}
				}
			}
			return num;
		}

		private static bool TryComplete(InputAction action, RebindDeviceProfile profile)
		{
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			int index = -1;
			int num = 0;
			int num2 = -1;
			int num3 = 0;
			for (int i = 0; i < bindings.Count; i++)
			{
				InputBinding binding = bindings[i];
				if (ChordSlots.IsRuntime(in binding))
				{
					if (ChordSlots.TryGetDevice(in binding, out var device) && device == profile.Kind)
					{
						return false;
					}
				}
				else if (binding.isComposite)
				{
					if (IsModifierComposite(in binding) && AllPartsOwned(bindings, i, profile))
					{
						num3++;
						num2 = i;
					}
				}
				else if (!binding.isPartOfComposite && profile.Owns(binding.path))
				{
					num++;
					index = i;
				}
			}
			if (num == 1 && num3 == 0)
			{
				AddCombo(action, profile.Kind, bindings[index].interactions, bindings[index].processors);
				return true;
			}
			if (num == 0 && num3 == 1 && ChordSlots.IsOneModifier(bindings[num2]) && TryFindMain(bindings, num2, out var main) && SlotKinds.KindOf(action, main) == CaptureKind.Button)
			{
				AddPlain(action, profile.Kind, bindings[num2].interactions, bindings[main].processors);
				return true;
			}
			return false;
		}

		private static void AddCombo(InputAction action, RebindDeviceKind device, string interactions, string processors)
		{
			string groups = ChordSlots.GroupsFor(device);
			action.AddBinding(new InputBinding
			{
				id = ChordSlots.IdFor(action.id, device, "header"),
				name = "RebindCombo",
				path = "OneModifier",
				interactions = interactions,
				groups = groups,
				isComposite = true
			});
			action.AddBinding(new InputBinding
			{
				id = ChordSlots.IdFor(action.id, device, "modifier"),
				name = "modifier",
				path = string.Empty,
				groups = groups,
				isPartOfComposite = true
			});
			action.AddBinding(new InputBinding
			{
				id = ChordSlots.IdFor(action.id, device, "binding"),
				name = "binding",
				path = string.Empty,
				processors = processors,
				groups = groups,
				isPartOfComposite = true
			});
		}

		private static void AddPlain(InputAction action, RebindDeviceKind device, string interactions, string processors)
		{
			action.AddBinding(new InputBinding
			{
				id = ChordSlots.IdFor(action.id, device, "plain"),
				path = string.Empty,
				interactions = interactions,
				processors = processors,
				groups = ChordSlots.GroupsFor(device)
			});
		}

		private static bool IsModifierComposite(in InputBinding header)
		{
			if (header.path != null)
			{
				return header.path.IndexOf("Modifier", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return false;
		}

		private static bool AllPartsOwned(ReadOnlyArray<InputBinding> bindings, int header, RebindDeviceProfile profile)
		{
			bool result = false;
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				if (!profile.Owns(bindings[i].path))
				{
					return false;
				}
				result = true;
			}
			return result;
		}

		private static bool TryFindMain(ReadOnlyArray<InputBinding> bindings, int header, out int main)
		{
			int num = 0;
			main = -1;
			for (int i = header + 1; i < bindings.Count && bindings[i].isPartOfComposite; i++)
			{
				if (string.Equals(bindings[i].name, "modifier", StringComparison.OrdinalIgnoreCase))
				{
					num++;
					continue;
				}
				if (string.Equals(bindings[i].name, "binding", StringComparison.OrdinalIgnoreCase))
				{
					main = i;
					continue;
				}
				return false;
			}
			if (num == 1)
			{
				return main >= 0;
			}
			return false;
		}
	}
}
