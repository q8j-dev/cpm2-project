using System;

namespace CPM.Shifter.Logic
{
	public readonly struct SlotCoord : IEquatable<SlotCoord>
	{
		public readonly int Col;

		public readonly int Row;

		public SlotCoord(int col, int row)
		{
			Col = col;
			Row = row;
		}

		public override string ToString()
		{
			return $"({Col},{Row})";
		}

		public bool Equals(SlotCoord other)
		{
			if (other.Col == Col)
			{
				return other.Row == Row;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is SlotCoord other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (Col * 397) ^ Row;
		}

		public static bool operator ==(SlotCoord a, SlotCoord b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SlotCoord a, SlotCoord b)
		{
			return !a.Equals(b);
		}
	}
}
