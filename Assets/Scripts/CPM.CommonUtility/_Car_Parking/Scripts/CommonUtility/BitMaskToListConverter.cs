using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility
{
	public static class BitMaskToListConverter
	{
		public const int ElementCount = 64;

		public static List<int> GetListFromBitMask(long bitmask, int elementsCount = -1)
		{
			int num = ((elementsCount == -1) ? 64 : elementsCount);
			List<int> list = Enumerable.Repeat(0, num).ToList();
			for (int i = 0; i < num; i++)
			{
				list[i] = (((bitmask & (1L << i)) != 0L) ? 1 : 0);
			}
			return list;
		}

		public static long GetBitMaskFromList(List<int> array)
		{
			long num = 0L;
			if (array == null)
			{
				return num;
			}
			array.Resize2(64, 0);
			for (int i = 0; i < array.Count; i++)
			{
				if (array[i] == 1)
				{
					num |= 1L << i;
				}
			}
			return num;
		}

		private static void TryLogError(string s)
		{
			try
			{
				Debug.LogError(s);
			}
			catch (Exception)
			{
			}
		}
	}
}
