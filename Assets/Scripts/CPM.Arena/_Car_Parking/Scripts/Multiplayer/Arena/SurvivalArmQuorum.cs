using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class SurvivalArmQuorum
	{
		public readonly struct TouchTally
		{
			public readonly int StillIn;

			public readonly int Touched;

			public bool Any => Touched > 0;

			public bool All
			{
				get
				{
					if (StillIn > 0)
					{
						return Touched == StillIn;
					}
					return false;
				}
			}

			public TouchTally(int stillIn, int touched)
			{
				StillIn = stillIn;
				Touched = touched;
			}
		}

		public static bool IsTouched(object raw)
		{
			bool flag = default;
			int num;
			if (raw is bool)
			{
				flag = (bool)raw;
				num = 1;
			}
			else
			{
				num = 0;
			}
			return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
		}

		public static TouchTally Tally(IReadOnlyList<int> participants, ICollection<int> present, Func<int, object> readTouch)
		{
			if (participants == null || present == null || readTouch == null)
			{
				return default;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < participants.Count; i++)
			{
				int num3 = participants[i];
				if (present.Contains(num3))
				{
					num++;
					if (IsTouched(readTouch(num3)))
					{
						num2++;
					}
				}
			}
			return new TouchTally(num, num2);
		}
	}
}
