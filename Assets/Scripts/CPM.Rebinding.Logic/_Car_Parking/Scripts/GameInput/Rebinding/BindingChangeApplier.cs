using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class BindingChangeApplier
	{
		public static void Apply(IReadOnlyList<BindingChange> changes)
		{
			if (changes == null)
			{
				return;
			}
			foreach (BindingChange change in changes)
			{
				Apply(change);
			}
		}

		public static void Apply(in BindingChange change)
		{
			InputAction inputAction = change.Action ?? throw new ArgumentNullException("change", "BindingChange.Action is null");
			if (change.Index < 0 || change.Index >= inputAction.bindings.Count)
			{
				throw new ArgumentOutOfRangeException("change", $"Binding index {change.Index} out of range for '{inputAction.name}' ({inputAction.bindings.Count} bindings)");
			}
			InputBinding inputBinding = inputAction.bindings[change.Index];
			string text = (change.Changes(ChangeFields.Path) ? change.NewPath : inputBinding.overridePath);
			string text2 = (change.Changes(ChangeFields.Interactions) ? change.NewInteractions : inputBinding.overrideInteractions);
			string text3 = (change.Changes(ChangeFields.Processors) ? change.NewProcessors : inputBinding.overrideProcessors);
			if (text != null && string.Equals(text, inputBinding.path ?? string.Empty, StringComparison.OrdinalIgnoreCase))
			{
				text = null;
			}
			if (text2 != null && SameText(text2, inputBinding.interactions))
			{
				text2 = null;
			}
			if (text3 != null && SameText(text3, inputBinding.processors))
			{
				text3 = null;
			}
			if (text == null && text2 == null && text3 == null)
			{
				inputAction.RemoveBindingOverride(change.Index);
				return;
			}
			inputAction.ApplyBindingOverride(change.Index, new InputBinding
			{
				overridePath = text,
				overrideInteractions = text2,
				overrideProcessors = text3
			});
		}

		private static bool SameText(string value, string authored)
		{
			if (!string.IsNullOrEmpty(value))
			{
				return string.Equals(value, authored, StringComparison.Ordinal);
			}
			return string.IsNullOrEmpty(authored);
		}
	}
}
