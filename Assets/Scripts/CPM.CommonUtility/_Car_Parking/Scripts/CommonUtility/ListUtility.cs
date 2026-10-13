using System.Collections.Generic;
using System.Linq;

namespace _Car_Parking.Scripts.CommonUtility
{
	public static class ListUtility
	{
		public static void Resize2<T>(this List<T> list, int size, T element = default(T))
		{
			int count = list.Count;
			if (size < count)
			{
				list.RemoveRange(size, count - size);
			}
			else if (size > count)
			{
				if (size > list.Capacity)
				{
					list.Capacity = size;
				}
				list.AddRange(Enumerable.Repeat(element, size - count));
			}
		}
	}
}
