using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class ArraysExtensions
	{
		public static T Random<T>(this T[] array)
		{
			return array[UnityEngine.Random.Range(0, array.Length)];
		}

		public static int RandomIndex<T>(this T[] array)
		{
			return UnityEngine.Random.Range(0, array.Length);
		}

		public static T Random<T>(this List<T> array)
		{
			return array[UnityEngine.Random.Range(0, array.Count)];
		}

		public static int RandomIndex<T>(this List<T> array)
		{
			return UnityEngine.Random.Range(0, array.Count);
		}
	}
}
