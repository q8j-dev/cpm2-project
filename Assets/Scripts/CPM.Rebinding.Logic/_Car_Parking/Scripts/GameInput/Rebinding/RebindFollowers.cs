using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	internal static class RebindFollowers
	{
		public static bool TryCollect(in KeyUnit leader, List<string> oldKeys, string authoredKey, bool combo, RebindDeviceProfile profile, IRebindRules rules, List<(KeyUnit from, KeyUnit to)> results)
		{
			results.Clear();
			IReadOnlyList<InputAction> readOnlyList = rules.FollowersOf(leader.Action);
			if (readOnlyList == null)
			{
				return true;
			}
			List<KeyUnit> list = new List<KeyUnit>();
			List<string> keys = new List<string>(2);
			foreach (InputAction item in readOnlyList)
			{
				if (item == null || item == leader.Action)
				{
					continue;
				}
				CollectFollowerUnits(item, oldKeys.Count > 1, profile, list);
				foreach (KeyUnit item2 in list)
				{
					KeyUnit unit = item2;
					if (StandsOn(in unit, oldKeys, authoredKey, keys))
					{
						if (!TryDestination(in unit, combo, profile, out var to))
						{
							return false;
						}
						results.Add((unit, to));
					}
				}
			}
			return true;
		}

		public static void AddConflictFollowerChanges(List<KeyUnit> conflicts, List<KeyUnit> moving, RebindDeviceProfile profile, IRebindRules rules, List<BindingChange> changes)
		{
			List<KeyUnit> list = new List<KeyUnit>();
			List<string> keys = new List<string>(2);
			List<string> list2 = new List<string>(2);
			List<KeyUnit> list3 = new List<KeyUnit>();
			foreach (KeyUnit conflict in conflicts)
			{
				KeyUnit unit = conflict;
				IReadOnlyList<InputAction> readOnlyList = rules.FollowersOf(unit.Action);
				if (readOnlyList == null || !KeyUnits.TryGetKeys(in unit, list2))
				{
					continue;
				}
				foreach (InputAction item in readOnlyList)
				{
					if (item == null || item == unit.Action || rules.PolicyFor(item) == ConflictPolicy.Lock)
					{
						continue;
					}
					CollectFollowerUnits(item, list2.Count > 1, profile, list);
					foreach (KeyUnit item2 in list)
					{
						KeyUnit unit2 = item2;
						if (!moving.Contains(unit2) && !conflicts.Contains(unit2) && !list3.Contains(unit2) && StandsOn(in unit2, list2, null, keys))
						{
							list3.Add(unit2);
							RebindUnitChanges.Clear(in unit2, changes);
						}
					}
				}
			}
		}

		private static void CollectFollowerUnits(InputAction follower, bool combo, RebindDeviceProfile profile, List<KeyUnit> units)
		{
			KeyUnits.Collect(follower, profile, units);
			if (combo)
			{
				return;
			}
			for (int num = units.Count - 1; num >= 0; num--)
			{
				KeyUnit keyUnit = units[num];
				if (keyUnit.Kind == KeyUnitKind.Combo)
				{
					units[num] = new KeyUnit(follower, KeyUnitKind.Part, keyUnit.Index);
					units.Insert(num, new KeyUnit(follower, KeyUnitKind.Part, keyUnit.Modifier));
				}
			}
		}

		private static bool StandsOn(in KeyUnit unit, List<string> leaderKeys, string authoredKey, List<string> keys)
		{
			if (leaderKeys.Count == 0)
			{
				if (unit.Kind == KeyUnitKind.Combo || string.IsNullOrEmpty(authoredKey))
				{
					return false;
				}
				InputBinding inputBinding = unit.Action.bindings[unit.Index];
				if (string.IsNullOrEmpty(inputBinding.effectivePath))
				{
					return BindingPaths.Overlap(inputBinding.path, authoredKey);
				}
				return false;
			}
			if (!KeyUnits.TryGetKeys(in unit, keys))
			{
				return false;
			}
			if (leaderKeys.Count != 1)
			{
				if (unit.Kind == KeyUnitKind.Combo)
				{
					return KeyUnits.SameKeys(keys, leaderKeys);
				}
				return false;
			}
			if (unit.Kind != KeyUnitKind.Combo)
			{
				return BindingPaths.Overlap(keys[0], leaderKeys[0]);
			}
			return false;
		}

		private static bool TryDestination(in KeyUnit unit, bool combo, RebindDeviceProfile profile, out KeyUnit to)
		{
			to = unit;
			if (combo)
			{
				if (unit.Kind == KeyUnitKind.Combo)
				{
					return true;
				}
				if (unit.Kind != KeyUnitKind.Plain || !ChordSlots.TryGetPair(unit.Action, unit.Index, profile, out var pair))
				{
					return false;
				}
				to = KeyUnits.ComboOf(in pair);
				return true;
			}
			if (unit.Kind == KeyUnitKind.Combo && ChordSlots.TryGetPair(unit.Action, unit.Index, profile, out var pair2))
			{
				to = KeyUnits.PlainOf(in pair2);
			}
			return true;
		}
	}
}
