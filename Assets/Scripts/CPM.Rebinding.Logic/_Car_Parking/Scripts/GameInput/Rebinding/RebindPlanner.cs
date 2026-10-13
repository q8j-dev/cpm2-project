using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class RebindPlanner
	{
		private readonly struct ConflictQuery
		{
			public readonly BindingSlot Target;

			public readonly KeyUnit NewUnit;

			public readonly CaptureKind TargetKind;

			public readonly InteractionClass TargetClass;

			public readonly List<string> NewKeys;

			public readonly List<BindingSlot> Followers;

			public readonly List<KeyUnit> Moving;

			public ConflictQuery(BindingSlot target, KeyUnit newUnit, CaptureKind targetKind, InteractionClass targetClass, List<string> newKeys, List<BindingSlot> followers, List<KeyUnit> moving)
			{
				Target = target;
				NewUnit = newUnit;
				TargetKind = targetKind;
				TargetClass = targetClass;
				NewKeys = newKeys;
				Followers = followers;
				Moving = moving;
			}
		}

		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		public static string EffectivePath(in InputBinding binding)
		{
			return binding.overridePath ?? binding.path;
		}

		public static bool TryFindSlot(InputAction action, string part, string displayedPath, out int index)
		{
			index = -1;
			if (action == null || string.IsNullOrEmpty(displayedPath))
			{
				return false;
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			bool flag = !string.IsNullOrEmpty(part);
			for (int i = 0; i < bindings.Count; i++)
			{
				InputBinding binding = bindings[i];
				if (binding.isComposite || !string.Equals(EffectivePath(in binding), displayedPath, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				bool num;
				if (!flag)
				{
					num = !binding.isPartOfComposite;
				}
				else
				{
					if (!binding.isPartOfComposite)
					{
						continue;
					}
					num = string.Equals(binding.name, part, StringComparison.OrdinalIgnoreCase);
				}
				if (num)
				{
					index = i;
					return true;
				}
			}
			for (int j = 0; j < bindings.Count; j++)
			{
				InputBinding binding2 = bindings[j];
				if (!binding2.isComposite && string.Equals(EffectivePath(in binding2), displayedPath, StringComparison.OrdinalIgnoreCase))
				{
					index = j;
					return true;
				}
			}
			return false;
		}

		public static bool CanRebind(InputAction action, int index, RebindDeviceProfile profile, IRebindRules rules)
		{
			if (action == null || profile == null || rules == null)
			{
				return false;
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			if (index < 0 || index >= bindings.Count)
			{
				return false;
			}
			InputBinding binding = bindings[index];
			if (binding.isComposite)
			{
				return false;
			}
			string text = EffectivePath(in binding);
			if (!profile.OwnsSlot(in binding) || !profile.SupportsKind(CaptureKindOf(action, index, profile)))
			{
				return false;
			}
			if (binding.isPartOfComposite && !string.IsNullOrEmpty(text) && !ModifierComposites.IsModifierPart(in binding) && ModifierComposites.IsInModifierComposite(bindings, index) && !AcceptsCombo(action, index, profile))
			{
				return false;
			}
			if (rules.IsReservedPath(text))
			{
				return false;
			}
			return PolicyOf(rules, action, profile) == ConflictPolicy.Unbind;
		}

		internal static ConflictPolicy PolicyOf(IRebindRules rules, InputAction action, RebindDeviceProfile profile)
		{
			if (!rules.IsLockedOn(action, profile.Kind))
			{
				return rules.PolicyFor(action);
			}
			return ConflictPolicy.Lock;
		}

		public static CaptureKind CaptureKindOf(InputAction action, int index, RebindDeviceProfile profile)
		{
			if (ChordSlots.TryGetPair(action, index, profile, out var pair))
			{
				return pair.Kind;
			}
			if (ChordSlots.TryGetComboOnly(action, index, profile, out var slots))
			{
				return SlotKinds.KindOf(action, slots.Main);
			}
			return SlotKinds.KindOf(action, index);
		}

		public static bool AcceptsCombo(InputAction action, int index, RebindDeviceProfile profile)
		{
			if (!ChordSlots.TryGetPair(action, index, profile, out var _))
			{
				return RequiresCombo(action, index, profile);
			}
			return true;
		}

		public static bool RequiresCombo(InputAction action, int index, RebindDeviceProfile profile)
		{
			ComboOnlySlots slots;
			return ChordSlots.TryGetComboOnly(action, index, profile, out slots);
		}

		public static int TriggerIndexFor(BindingSlot slot, bool combo, RebindDeviceProfile profile)
		{
			if (ChordSlots.TryGetPair(slot.Action, slot.Index, profile, out var pair))
			{
				if (!combo)
				{
					return pair.Plain;
				}
				return pair.Header;
			}
			if (ChordSlots.TryGetComboOnly(slot.Action, slot.Index, profile, out var slots))
			{
				return slots.Header;
			}
			if (!slot.Binding.isPartOfComposite)
			{
				return slot.Index;
			}
			return -1;
		}

		public static RebindPlan Plan(BindingSlot target, string capturedPath, IReadOnlyList<InputActionMap> scope, RebindDeviceProfile profile, IRebindRules rules)
		{
			return Plan(new RebindRequest(target, capturedPath), scope, profile, rules);
		}

		public static RebindPlan Plan(RebindRequest request, IReadOnlyList<InputActionMap> scope, RebindDeviceProfile profile, IRebindRules rules)
		{
			BindingSlot target = request.Target;
			InputAction action = target.Action;
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			bool flag = ChordSlots.TryGetPair(action, target.Index, profile, out var pair);
			ComboOnlySlots slots = default;
			bool flag2 = !flag && ChordSlots.TryGetComboOnly(action, target.Index, profile, out slots);
			string key = NormalizeCaptured(profile, request.CapturedPath);
			string modifier = ((request.CapturedModifierPath == null) ? null : NormalizeCaptured(profile, request.CapturedModifierPath));
			if (modifier != null && BindingPaths.Overlap(modifier, key))
			{
				modifier = null;
			}
			List<string> list = ((modifier == null) ? new List<string> { key } : new List<string> { modifier, key });
			KeyUnit unit = (flag ? KeyUnits.ActiveOf(in pair) : (flag2 ? KeyUnits.ComboOf(action, in slots) : KeyUnits.Single(target)));
			KeyUnit unit2 = ((!flag) ? unit : ((modifier == null) ? KeyUnits.PlainOf(in pair) : KeyUnits.ComboOf(in pair)));
			string oldPath = EffectivePath(bindings[unit.Index]);
			List<string> list2 = new List<string>();
			AddBoundKeys(in unit, list2);
			CaptureKind targetKind = (flag ? pair.Kind : SlotKinds.KindOf(action, unit.Index));
			InteractionClass interactionClass = InteractionRules.ClassOf(action, unit.Index);
			InteractionClass targetClass = ((request.NewInteractions == null) ? interactionClass : InteractionRules.Classify(InteractionRules.Join(request.NewInteractions, action.interactions)));
			bool flag3 = unit2.Kind != unit.Kind;
			string text = request.NewInteractions;
			if (((text == null) & flag3) && bindings[unit.TriggerIndex].overrideInteractions != null)
			{
				text = InteractionRules.InteractionsFor(action, unit2.TriggerIndex, interactionClass);
			}
			if (modifier != null && !flag && !flag2)
			{
				return Reject(RebindVerdict.ComboNotAllowed);
			}
			if ((modifier == null) & flag2)
			{
				return Reject(RebindVerdict.ComboRequired);
			}
			if (modifier != null && (!profile.OwnsCaptured(modifier) || !CapturedKindFits(modifier, CaptureKind.Button)))
			{
				return Reject(RebindVerdict.Unsupported);
			}
			if (!profile.OwnsCaptured(key) || !KeyFits(key, targetKind, action))
			{
				return Reject(RebindVerdict.Unsupported);
			}
			bool flag4 = !flag3 && KeyUnits.SameKeys(list2, list);
			bool flag5 = targetClass != interactionClass;
			bool flag6 = request.NewProcessors != null && !string.Equals(request.NewProcessors, bindings[unit2.Index].effectiveProcessors ?? string.Empty, StringComparison.OrdinalIgnoreCase);
			if (flag4 && !flag5 && !flag6)
			{
				return Reject(RebindVerdict.NoChange);
			}
			if (!flag4 && list.Any(rules.IsReservedPath))
			{
				return Reject(RebindVerdict.Reserved);
			}
			if (!flag4 && list.Any((string k) => !rules.HasGlyph(k)))
			{
				return Reject(RebindVerdict.Unsupported);
			}
			List<(KeyUnit, KeyUnit)> list3 = new List<(KeyUnit, KeyUnit)>();
			if (!flag4 && !RebindFollowers.TryCollect(in unit, list2, TargetAuthoredKey(in unit, in pair, flag), modifier != null, profile, rules, list3))
			{
				return Reject(RebindVerdict.ComboNotAllowed);
			}
			List<BindingSlot> followers = list3.Select(((KeyUnit from, KeyUnit to) f) => f.to.Slot).ToList();
			List<KeyUnit> list4 = new List<KeyUnit> { unit2 };
			if (flag)
			{
				list4.Add((unit2.Kind == KeyUnitKind.Combo) ? KeyUnits.PlainOf(in pair) : KeyUnits.ComboOf(in pair));
			}
			if (flag2)
			{
				list4.Add(new KeyUnit(action, KeyUnitKind.Part, slots.Modifier));
				list4.Add(new KeyUnit(action, KeyUnitKind.Part, slots.Main));
			}
			foreach (var (item, item2) in list3)
			{
				list4.Add(item);
				list4.Add(item2);
			}
			if (!flag4 && !ReturnsToAuthored(in unit2, list) && TryFindLockedHolder(target, list, followers, scope, profile, rules, out var holder))
			{
				return Reject(RebindVerdict.Locked, holder);
			}
			List<KeyUnit> list5 = new List<KeyUnit>();
			if ((!flag4 | flag5) && !TryCollectConflicts(new ConflictQuery(target, unit2, targetKind, targetClass, list, followers, list4), scope, profile, rules, list5, out var lockedBy))
			{
				return Reject(RebindVerdict.Locked, lockedBy);
			}
			if (flag4 && list5.Count > 0)
			{
				return Reject(RebindVerdict.ClassTaken, list5[0].Slot);
			}
			List<BindingChange> list6 = new List<BindingChange>(2 + list3.Count + list5.Count);
			AddTargetChanges(in unit, in unit2, list, flag4, flag5, flag6, text, request.NewProcessors, list6);
			foreach (var item3 in list3)
			{
				var (unit3, unit4) = item3;
				if (!unit3.Equals(unit4))
				{
					RebindUnitChanges.Clear(in unit3, list6);
				}
				RebindUnitChanges.Assign(in unit4, list, list6);
			}
			foreach (KeyUnit item4 in list5)
			{
				KeyUnit unit5 = item4;
				if (!flag && !flag2 && unit5.Kind != KeyUnitKind.Combo && IsSamePart(target, unit5.Slot))
				{
					list6.Add(new BindingChange(unit5.Action, unit5.Index, oldPath));
				}
				else
				{
					RebindUnitChanges.Clear(in unit5, list6);
				}
			}
			RebindFollowers.AddConflictFollowerChanges(list5, list4, profile, rules, list6);
			List<BindingSlot> conflicts = list5.Select((KeyUnit c) => c.Slot).ToList();
			return RebindPlan.Accepted(target, oldPath, key, conflicts, list6, NeedsConfirmation(target, conflicts), targetClass, modifier);
			RebindPlan Reject(RebindVerdict verdict, BindingSlot lockedBy2 = default(BindingSlot))
			{
				return RebindPlan.Rejected(verdict, target, oldPath, key, targetClass, lockedBy2, modifier);
			}
		}

		private static string NormalizeCaptured(RebindDeviceProfile profile, string path)
		{
			return BindingPaths.Normalize(profile.NormalizeCapturedPath(path));
		}

		private static bool CapturedKindFits(string capturedPath, CaptureKind targetKind)
		{
			if (SlotKinds.TryKindOfPath(capturedPath, out var kind))
			{
				return kind == targetKind;
			}
			return true;
		}

		private static bool KeyFits(string capturedPath, CaptureKind targetKind, InputAction action)
		{
			if (!CapturedKindFits(capturedPath, targetKind))
			{
				if (targetKind == CaptureKind.Button && SlotKinds.IsDirectionPath(capturedPath) && !SlotKinds.IsMouseMoveDirectionPath(capturedPath))
				{
					if (action.type != InputActionType.Button)
					{
						return !SlotKinds.IsRelativeDirectionPath(capturedPath);
					}
					return true;
				}
				return false;
			}
			return true;
		}

		private static void AddBoundKeys(in KeyUnit unit, List<string> keys)
		{
			ReadOnlyArray<InputBinding> bindings = unit.Action.bindings;
			keys.Clear();
			if (unit.Kind == KeyUnitKind.Combo && !string.IsNullOrEmpty(bindings[unit.Modifier].effectivePath))
			{
				keys.Add(bindings[unit.Modifier].effectivePath);
			}
			if (!string.IsNullOrEmpty(bindings[unit.Index].effectivePath))
			{
				keys.Add(bindings[unit.Index].effectivePath);
			}
		}

		private static string TargetAuthoredKey(in KeyUnit oldUnit, in ChordPair pair, bool hasPair)
		{
			string path = oldUnit.Action.bindings[oldUnit.Index].path;
			if (!string.IsNullOrEmpty(path) || !hasPair)
			{
				return path;
			}
			string path2 = pair.Action.bindings[pair.Plain].path;
			if (string.IsNullOrEmpty(path2))
			{
				return pair.Action.bindings[pair.Main].path;
			}
			return path2;
		}

		private static bool ReturnsToAuthored(in KeyUnit unit, List<string> newKeys)
		{
			List<string> list = new List<string>(2);
			KeyUnits.AuthoredKeys(in unit, list);
			return KeyUnits.SameKeys(list, newKeys);
		}

		private static void AddTargetChanges(in KeyUnit oldUnit, in KeyUnit newUnit, List<string> newKeys, bool sameKeys, bool classChanged, bool processorsChanged, string newInteractions, string newProcessors, List<BindingChange> changes)
		{
			InputAction action = newUnit.Action;
			if (!newUnit.Equals(oldUnit))
			{
				RebindUnitChanges.Clear(in oldUnit, changes);
				InputBinding inputBinding = action.bindings[oldUnit.TriggerIndex];
				if (inputBinding.overrideInteractions != null)
				{
					changes.Add(new BindingChange(action, oldUnit.TriggerIndex, ChangeFields.Interactions, null, inputBinding.interactions ?? string.Empty, null));
				}
				RebindUnitChanges.Assign(in newUnit, newKeys, changes, newInteractions, newProcessors);
			}
			else if (!sameKeys)
			{
				RebindUnitChanges.Assign(in newUnit, newKeys, changes, classChanged ? newInteractions : null, processorsChanged ? newProcessors : null);
			}
			else
			{
				RebindUnitChanges.SetTrigger(in newUnit, classChanged ? newInteractions : null, processorsChanged ? newProcessors : null, changes);
			}
		}

		private static bool TryCollectConflicts(in ConflictQuery query, IReadOnlyList<InputActionMap> scope, RebindDeviceProfile profile, IRebindRules rules, List<KeyUnit> conflicts, out BindingSlot lockedBy)
		{
			lockedBy = default;
			if (scope == null)
			{
				return true;
			}
			List<KeyUnit> list = new List<KeyUnit>();
			List<string> keys = new List<string>(2);
			foreach (InputActionMap item in scope)
			{
				foreach (InputAction action in item.actions)
				{
					KeyUnits.Collect(action, profile, list);
					if (PolicyOf(rules, action, profile) == ConflictPolicy.Lock)
					{
						CollapseComboOnly(action, profile, list);
					}
					foreach (KeyUnit item2 in list)
					{
						if (TryClassify(item2, in query, profile, rules, conflicts, keys, out var unit, out var policy))
						{
							if (policy == ConflictPolicy.Lock)
							{
								lockedBy = unit.Slot;
								return false;
							}
							conflicts.Add(unit);
						}
					}
				}
			}
			return true;
		}

		private static bool TryClassify(KeyUnit candidate, in ConflictQuery query, RebindDeviceProfile profile, IRebindRules rules, List<KeyUnit> conflicts, List<string> keys, out KeyUnit unit, out ConflictPolicy policy)
		{
			unit = candidate;
			policy = ConflictPolicy.Ignore;
			InputAction action = candidate.Action;
			if (!KeyUnits.TryGetKeys(in candidate, keys) || !KeyUnits.Overlaps(keys, query.NewKeys))
			{
				return false;
			}
			if (query.Moving.Contains(candidate) || conflicts.Contains(candidate))
			{
				return false;
			}
			if (action != query.Target.Action && rules.AreLinked(action, query.Target.Action))
			{
				return false;
			}
			if (!Clashes(candidate.Slot, query.NewUnit.Slot, query.TargetClass, query.Followers, rules))
			{
				return false;
			}
			policy = PolicyOf(rules, action, profile);
			if (policy == ConflictPolicy.Ignore)
			{
				if (!TryFindDisplayedLeaderUnit(in candidate, keys, in query, profile, rules, out unit) || conflicts.Contains(unit))
				{
					return false;
				}
				policy = PolicyOf(rules, unit.Action, profile);
			}
			if (policy == ConflictPolicy.Lock && LeavesDirectionsFree(in unit, keys, query.NewKeys))
			{
				return false;
			}
			return policy != ConflictPolicy.Ignore;
		}

		private static bool TryFindDisplayedLeaderUnit(in KeyUnit follower, List<string> keys, in ConflictQuery query, RebindDeviceProfile profile, IRebindRules rules, out KeyUnit leaderUnit)
		{
			leaderUnit = default;
			IReadOnlyList<InputAction> readOnlyList = rules.LeadersOf(follower.Action);
			if (readOnlyList == null)
			{
				return false;
			}
			List<KeyUnit> list = new List<KeyUnit>();
			List<string> list2 = new List<string>(2);
			foreach (InputAction item in readOnlyList)
			{
				if (item == null || item == query.Target.Action || PolicyOf(rules, item, profile) == ConflictPolicy.Ignore)
				{
					continue;
				}
				KeyUnits.Collect(item, profile, list);
				foreach (KeyUnit item2 in list)
				{
					KeyUnit unit = item2;
					if (!query.Moving.Contains(unit) && KeyUnits.TryGetKeys(in unit, list2) && KeyUnits.SameKeys(list2, keys))
					{
						leaderUnit = unit;
						return true;
					}
				}
			}
			return false;
		}

		private static bool Clashes(BindingSlot other, BindingSlot target, InteractionClass targetClass, List<BindingSlot> followers, IRebindRules rules)
		{
			InteractionClass interactionClass = InteractionRules.ClassOf(other.Action, other.Index);
			bool bContinuous = InteractionRules.IsContinuous(other.Action, interactionClass);
			if (rules.CanFireTogether(other, target) && !InteractionRules.CanShareKey(targetClass, InteractionRules.IsContinuous(target.Action, targetClass), interactionClass, bContinuous))
			{
				return true;
			}
			foreach (BindingSlot follower in followers)
			{
				InteractionClass interactionClass2 = InteractionRules.ClassOf(follower.Action, follower.Index);
				if (rules.CanFireTogether(other, follower) && !InteractionRules.CanShareKey(interactionClass2, InteractionRules.IsContinuous(follower.Action, interactionClass2), interactionClass, bContinuous))
				{
					return true;
				}
			}
			return false;
		}

		private static bool TryFindLockedHolder(BindingSlot target, List<string> newKeys, List<BindingSlot> followers, IReadOnlyList<InputActionMap> scope, RebindDeviceProfile profile, IRebindRules rules, out BindingSlot holder)
		{
			holder = default;
			if (scope == null)
			{
				return false;
			}
			List<KeyUnit> list = new List<KeyUnit>();
			List<string> list2 = new List<string>(2);
			foreach (InputActionMap item in scope)
			{
				foreach (InputAction action in item.actions)
				{
					if (action == target.Action || PolicyOf(rules, action, profile) != ConflictPolicy.Lock || rules.AreLinked(action, target.Action))
					{
						continue;
					}
					KeyUnits.Collect(action, profile, list);
					CollapseComboOnly(action, profile, list);
					foreach (KeyUnit item2 in list)
					{
						KeyUnit unit = item2;
						if (KeyUnits.TryGetKeys(in unit, list2) && KeyUnits.Overlaps(list2, newKeys) && !LeavesDirectionsFree(in unit, list2, newKeys) && (rules.CanFireTogether(unit.Slot, target) || followers.Any((BindingSlot f) => rules.CanFireTogether(unit.Slot, f))))
						{
							holder = unit.Slot;
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool LeavesDirectionsFree(in KeyUnit locked, List<string> lockedKeys, List<string> newKeys)
		{
			bool flag = locked.Kind != KeyUnitKind.Combo && SlotKinds.KindOf(locked.Action, locked.Index) != CaptureKind.Button;
			foreach (string newKey in newKeys)
			{
				foreach (string lockedKey in lockedKeys)
				{
					if (BindingPaths.Overlap(newKey, lockedKey) && (!((locked.Kind == KeyUnitKind.Combo) ? IsAnalogPath(lockedKey) : flag) || !SlotKinds.IsDirectionPath(newKey)))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static bool IsAnalogPath(string path)
		{
			if (SlotKinds.TryKindOfPath(path, out var kind))
			{
				return kind != CaptureKind.Button;
			}
			return false;
		}

		private static void CollapseComboOnly(InputAction action, RebindDeviceProfile profile, List<KeyUnit> units)
		{
			List<int> modifiers = null;
			for (int i = 0; i < units.Count; i++)
			{
				if (units[i].Kind == KeyUnitKind.Part && ChordSlots.TryGetComboOnly(action, units[i].Index, profile, out var slots))
				{
					units[i] = KeyUnits.ComboOf(action, in slots);
					(modifiers ?? (modifiers = new List<int>(1))).Add(slots.Modifier);
				}
			}
			if (modifiers != null)
			{
				units.RemoveAll((KeyUnit unit) => unit.Kind == KeyUnitKind.Part && modifiers.Contains(unit.Index));
			}
		}

		private static bool NeedsConfirmation(BindingSlot target, List<BindingSlot> conflicts)
		{
			return conflicts.Any((BindingSlot conflict) => !IsSamePart(target, conflict));
		}

		private static bool IsSamePart(BindingSlot target, BindingSlot conflict)
		{
			InputBinding binding = target.Binding;
			InputBinding binding2 = conflict.Binding;
			if (conflict.Action == target.Action && binding2.isPartOfComposite == binding.isPartOfComposite)
			{
				return string.Equals(binding2.name ?? string.Empty, binding.name ?? string.Empty, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
	}
}
