using System;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct BindingSlot : IEquatable<BindingSlot>
	{
		public readonly InputAction Action;

		public readonly int Index;

		public InputBinding Binding => Action.bindings[Index];

		public BindingSlot(InputAction action, int index)
		{
			Action = action;
			Index = index;
		}

		public bool Equals(BindingSlot other)
		{
			if (Action == other.Action)
			{
				return Index == other.Index;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is BindingSlot other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((Action?.GetHashCode() ?? 0) * 397) ^ Index;
		}

		public override string ToString()
		{
			return $"{Action?.actionMap?.name}/{Action?.name}[{Index}]";
		}
	}
}
