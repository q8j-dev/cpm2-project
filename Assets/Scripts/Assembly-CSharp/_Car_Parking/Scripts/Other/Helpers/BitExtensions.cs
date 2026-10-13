namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class BitExtensions
	{
		public static int SetBitTo1(this int value, int position)
		{
			return value |= 1 << position;
		}

		public static int SetBitTo0(this int value, int position)
		{
			return value & ~(1 << position);
		}

		public static bool IsBitSetTo1(this int value, int position)
		{
			return (value & (1 << position)) != 0;
		}

		public static bool IsBitSetTo0(this int value, int position)
		{
			return !value.IsBitSetTo1(position);
		}

		public static int SetBit(this int number, int position, bool bitValue)
		{
			if (bitValue)
			{
				return number.SetBitTo1(position);
			}
			return number.SetBitTo0(position);
		}
	}
}
