using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct BindingChange
	{
		public readonly InputAction Action;

		public readonly int Index;

		public readonly ChangeFields Fields;

		public readonly string NewPath;

		public readonly string NewInteractions;

		public readonly string NewProcessors;

		public BindingChange(InputAction action, int index, string newPath)
			: this(action, index, ChangeFields.Path, newPath, null, null)
		{
		}

		public BindingChange(InputAction action, int index, ChangeFields fields, string newPath, string newInteractions, string newProcessors)
		{
			Action = action;
			Index = index;
			Fields = fields;
			NewPath = newPath;
			NewInteractions = newInteractions;
			NewProcessors = newProcessors;
		}

		public bool Changes(ChangeFields fields)
		{
			return (Fields & fields) == fields;
		}
	}
}
