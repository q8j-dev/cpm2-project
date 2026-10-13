using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	internal static class RebindUnitChanges
	{
		public static void Assign(in KeyUnit unit, IReadOnlyList<string> keys, List<BindingChange> changes, string interactions = null, string processors = null)
		{
			InputAction action = unit.Action;
			string newPath = keys[keys.Count - 1];
			ChangeFields changeFields = ((processors != null) ? ChangeFields.Processors : ChangeFields.None);
			if (unit.Kind == KeyUnitKind.Combo)
			{
				changes.Add(new BindingChange(action, unit.Modifier, keys[0]));
				changes.Add(new BindingChange(action, unit.Index, ChangeFields.Path | changeFields, newPath, null, processors));
				if (interactions != null)
				{
					changes.Add(new BindingChange(action, unit.Header, ChangeFields.Interactions, null, interactions, null));
				}
			}
			else
			{
				ChangeFields fields = (ChangeFields)((int)(ChangeFields.Path | changeFields) | ((interactions != null) ? 2 : 0));
				changes.Add(new BindingChange(action, unit.Index, fields, newPath, interactions, processors));
			}
		}

		public static void Clear(in KeyUnit unit, List<BindingChange> changes)
		{
			if (unit.Kind == KeyUnitKind.Combo)
			{
				ClearBinding(unit.Action, unit.Modifier, changes);
			}
			ClearBinding(unit.Action, unit.Index, changes);
		}

		public static void SetTrigger(in KeyUnit unit, string interactions, string processors, List<BindingChange> changes)
		{
			if (interactions != null)
			{
				changes.Add(new BindingChange(unit.Action, unit.TriggerIndex, ChangeFields.Interactions, null, interactions, null));
			}
			if (processors != null)
			{
				changes.Add(new BindingChange(unit.Action, unit.Index, ChangeFields.Processors, null, null, processors));
			}
		}

		private static void ClearBinding(InputAction action, int index, List<BindingChange> changes)
		{
			if (!string.IsNullOrEmpty(action.bindings[index].effectivePath))
			{
				changes.Add(new BindingChange(action, index, string.Empty));
			}
		}
	}
}
