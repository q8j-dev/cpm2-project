using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Database
{
	public class HomesController
	{
		public const int MaxHomeIndex = 3;

		public static void SetHomes(long homesBitmask)
		{
			List<int> listFromBitMask = BitMaskToListConverter.GetListFromBitMask(homesBitmask);
			for (int i = 0; i < listFromBitMask.Count; i++)
			{
				if (listFromBitMask[i] == 1)
				{
					ObscuredPrefs.SetInt("homeOpen" + i, 1);
				}
			}
		}

		public static bool IsHomeBought(int index)
		{
			return ObscuredPrefs.GetInt("homeOpen" + index) == 1;
		}

		public static long GenerateOpenHomes()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 4; i++)
			{
				int item = ObscuredPrefs.GetInt("homeOpen" + i);
				list.Add(item);
			}
			return BitMaskToListConverter.GetBitMaskFromList(list);
		}
	}
}
