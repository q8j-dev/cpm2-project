using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class KeyUnits
	{
		public static KeyUnit PlainOf(in ChordPair pair)
		{
			return new KeyUnit(pair.Action, KeyUnitKind.Plain, pair.Plain, -1, -1, inPair: true, pair.Kind);
		}

		public static KeyUnit ComboOf(in ChordPair pair)
		{
			return new KeyUnit(pair.Action, KeyUnitKind.Combo, pair.Main, pair.Header, pair.Modifier, inPair: true, pair.Kind);
		}

		public static KeyUnit ComboOf(InputAction action, in ComboOnlySlots slots)
		{
			return new KeyUnit(action, KeyUnitKind.Combo, slots.Main, slots.Header, slots.Modifier);
		}

		public static KeyUnit ActiveOf(in ChordPair pair)
		{
			if (!pair.ComboActive)
			{
				return PlainOf(in pair);
			}
			return ComboOf(in pair);
		}

		public static KeyUnit Single(BindingSlot slot)
		{
			return new KeyUnit(slot.Action, slot.Binding.isPartOfComposite ? KeyUnitKind.Part : KeyUnitKind.Plain, slot.Index);
		}

		public static void Collect(InputAction action, RebindDeviceProfile profile, List<KeyUnit> results)
		{
			results.Clear();
			if (action == null || profile == null)
			{
				return;
			}
			bool flag = ChordSlots.TryGetPair(action, profile, out var pair);
			if (flag)
			{
				results.Add(PlainOf(in pair));
				results.Add(ComboOf(in pair));
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				InputBinding binding = bindings[i];
				if (!binding.isComposite && (!flag || !pair.Contains(i)) && profile.OwnsSlot(in binding))
				{
					results.Add(Single(new BindingSlot(action, i)));
				}
			}
		}

		public static bool TryGetKeys(in KeyUnit unit, List<string> keys)
		{
			keys.Clear();
			ReadOnlyArray<InputBinding> bindings = unit.Action.bindings;
			if (unit.Kind == KeyUnitKind.Combo && !Add(bindings[unit.Modifier].effectivePath, keys))
			{
				return false;
			}
			if (Add(bindings[unit.Index].effectivePath, keys))
			{
				return true;
			}
			keys.Clear();
			return false;
		}

		public static void AuthoredKeys(in KeyUnit unit, List<string> keys)
		{
			keys.Clear();
			ReadOnlyArray<InputBinding> bindings = unit.Action.bindings;
			if (unit.Kind == KeyUnitKind.Combo)
			{
				Add(bindings[unit.Modifier].path, keys);
			}
			Add(bindings[unit.Index].path, keys);
		}

		public static bool Overlaps(IReadOnlyList<string> a, IReadOnlyList<string> b)
		{
			if (a.Count > 0 && b.Count > 0)
			{
				if (!IsSubset(a, b))
				{
					return IsSubset(b, a);
				}
				return true;
			}
			return false;
		}

		public static bool SameKeys(IReadOnlyList<string> a, IReadOnlyList<string> b)
		{
			if (a.Count > 0 && a.Count == b.Count && IsSubset(a, b))
			{
				return IsSubset(b, a);
			}
			return false;
		}

		public static CaptureKind KindOf(in KeyUnit unit)
		{
			if (!unit.InPair)
			{
				return SlotKinds.KindOf(unit.Action, unit.Index);
			}
			return unit.PairKind;
		}

		private static bool IsSubset(IReadOnlyList<string> small, IReadOnlyList<string> big)
		{
			foreach (string item in small)
			{
				bool flag = false;
				foreach (string item2 in big)
				{
					if (BindingPaths.Overlap(item, item2))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private static bool Add(string path, List<string> keys)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			keys.Add(path);
			return true;
		}
	}
}
