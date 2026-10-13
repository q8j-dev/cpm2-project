using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class InteractionRules
	{
		public const string PressInteraction = "Press";

		public const string HoldInteraction = "Hold";

		private const string SliderPressInteraction = "SliderPress";

		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		private const int ReleaseBehavior = 1;

		public static InteractionClass Classify(string interactions)
		{
			if (string.IsNullOrEmpty(interactions))
			{
				return InteractionClass.Press;
			}
			using (IEnumerator<NameAndParameters> enumerator = NameAndParameters.ParseMultiple(interactions).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return ClassifyEntry(enumerator.Current);
				}
			}
			return InteractionClass.Press;
		}

		public static string EffectiveInteractions(InputAction action, int index)
		{
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			InputBinding inputBinding = bindings[index];
			return Join((!inputBinding.isPartOfComposite) ? inputBinding.effectiveInteractions : HeaderOf(bindings, index)?.effectiveInteractions, action.interactions);
		}

		public static InteractionClass ClassOf(InputAction action, int index)
		{
			return Classify(EffectiveInteractions(action, index));
		}

		public static int TriggerIndexOf(ReadOnlyArray<InputBinding> bindings, int index)
		{
			if (index < 0 || index >= bindings.Count || !bindings[index].isPartOfComposite)
			{
				return index;
			}
			for (int num = index - 1; num >= 0; num--)
			{
				if (bindings[num].isComposite)
				{
					return num;
				}
			}
			return index;
		}

		public static bool IsContinuous(InputAction action, InteractionClass cls)
		{
			if (action != null && action.type != InputActionType.Button)
			{
				return cls == InteractionClass.Press;
			}
			return false;
		}

		public static bool CanShareKey(InteractionClass a, bool aContinuous, InteractionClass b, bool bContinuous)
		{
			if (!IsTapLike(a) || !((b == InteractionClass.Hold) | bContinuous))
			{
				if (IsTapLike(b))
				{
					return (a == InteractionClass.Hold) | aContinuous;
				}
				return false;
			}
			return true;
		}

		private static bool IsTapLike(InteractionClass cls)
		{
			if (cls != InteractionClass.Tap)
			{
				return cls == InteractionClass.MultiTap;
			}
			return true;
		}

		public static bool CanTogglePressHold(InputAction action, int index, IRebindRules rules)
		{
			if (action == null || rules == null || index < 0 || index >= action.bindings.Count)
			{
				return false;
			}
			InputBinding header = action.bindings[index];
			if ((header.isComposite && !ChordSlots.IsOneModifier(in header)) || header.isPartOfComposite || IsAnalogAction(action))
			{
				return false;
			}
			string interactions = EffectiveInteractions(action, index);
			if (EntryCount(interactions) > 1)
			{
				return false;
			}
			InteractionClass interactionClass = Classify(interactions);
			if (interactionClass == InteractionClass.Press || interactionClass == InteractionClass.Hold)
			{
				return rules.AllowsPressHold(action);
			}
			return false;
		}

		public static bool IsAnalogAction(InputAction action)
		{
			if (action == null || action.type != InputActionType.Button)
			{
				return true;
			}
			ReadOnlyArray<InputBinding> bindings = action.bindings;
			for (int i = 0; i < bindings.Count; i++)
			{
				InputBinding inputBinding = bindings[i];
				if (!inputBinding.isComposite && (HasInteraction(inputBinding.interactions, "SliderPress") || (!string.IsNullOrEmpty(inputBinding.path) && SlotKinds.KindOf(action, i) != CaptureKind.Button)))
				{
					return true;
				}
			}
			return false;
		}

		private static bool HasInteraction(string interactions, string name)
		{
			if (!string.IsNullOrEmpty(interactions))
			{
				return interactions.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return false;
		}

		public static string InteractionsFor(InputAction action, int index, InteractionClass target)
		{
			string text = action.bindings[index].interactions ?? string.Empty;
			if (Classify(Join(text, action.interactions)) == target)
			{
				return text;
			}
			if (target != InteractionClass.Hold)
			{
				return "Press";
			}
			return "Hold";
		}

		public static string Join(string first, string second)
		{
			if (string.IsNullOrEmpty(first))
			{
				return second ?? string.Empty;
			}
			if (!string.IsNullOrEmpty(second))
			{
				return first + "," + second;
			}
			return first;
		}

		private static InteractionClass ClassifyEntry(NameAndParameters entry)
		{
			string name = entry.name;
			if (string.Equals(name, "Hold", StringComparison.OrdinalIgnoreCase))
			{
				return InteractionClass.Hold;
			}
			if (string.Equals(name, "Tap", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "SlowTap", StringComparison.OrdinalIgnoreCase))
			{
				return InteractionClass.Tap;
			}
			if (string.Equals(name, "MultiTap", StringComparison.OrdinalIgnoreCase))
			{
				return InteractionClass.MultiTap;
			}
			if (string.Equals(name, "Press", StringComparison.OrdinalIgnoreCase) && BehaviorOf(entry) == 1)
			{
				return InteractionClass.Release;
			}
			return InteractionClass.Press;
		}

		private static int BehaviorOf(NameAndParameters entry)
		{
			foreach (NamedValue parameter in entry.parameters)
			{
				if (string.Equals(parameter.name, "behavior", StringComparison.OrdinalIgnoreCase))
				{
					return parameter.value.ToInt32();
				}
			}
			return 0;
		}

		private static int EntryCount(string interactions)
		{
			if (string.IsNullOrEmpty(interactions))
			{
				return 0;
			}
			int num = 0;
			foreach (NameAndParameters item in NameAndParameters.ParseMultiple(interactions))
			{
				_ = item;
				num++;
			}
			return num;
		}

		private static InputBinding? HeaderOf(ReadOnlyArray<InputBinding> bindings, int partIndex)
		{
			for (int num = partIndex - 1; num >= 0; num--)
			{
				if (bindings[num].isComposite)
				{
					return bindings[num];
				}
			}
			return null;
		}
	}
}
