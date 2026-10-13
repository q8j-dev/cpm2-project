using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class PhysicsHelpers
	{
		public static int GetIgnoreLayerMaskCollision(int testLayer)
		{
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				if (!Physics.GetIgnoreLayerCollision(testLayer, i))
				{
					num |= 1 << i;
				}
			}
			return num;
		}

		public static void IgnoreLayerMaskCollision(int myLayer, int mask, bool ignore)
		{
			for (int i = 0; i < 32; i++)
			{
				if (mask.IsBitSetTo1(i))
				{
					Physics.IgnoreLayerCollision(myLayer, i, ignore);
				}
			}
		}
	}
}
