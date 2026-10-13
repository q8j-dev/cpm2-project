using System;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct KeyUnit : IEquatable<KeyUnit>
	{
		public readonly InputAction Action;

		public readonly KeyUnitKind Kind;

		public readonly int Index;

		public readonly int Header;

		public readonly int Modifier;

		public readonly bool InPair;

		public readonly CaptureKind PairKind;

		public BindingSlot Slot => new BindingSlot(Action, Index);

		public int TriggerIndex
		{
			get
			{
				if (Kind != KeyUnitKind.Combo)
				{
					return Index;
				}
				return Header;
			}
		}

		public KeyUnit(InputAction action, KeyUnitKind kind, int index, int header = -1, int modifier = -1, bool inPair = false, CaptureKind pairKind = CaptureKind.Button)
		{
			Action = action;
			Kind = kind;
			Index = index;
			Header = header;
			Modifier = modifier;
			InPair = inPair;
			PairKind = pairKind;
		}

		public bool Equals(KeyUnit other)
		{
			if (Action == other.Action && Kind == other.Kind)
			{
				return Index == other.Index;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is KeyUnit other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((Action?.GetHashCode() ?? 0) * 397) ^ (Index * 31) ^ (int)Kind;
		}

		public override string ToString()
		{
			return $"{Kind} {Slot}";
		}
	}
}
