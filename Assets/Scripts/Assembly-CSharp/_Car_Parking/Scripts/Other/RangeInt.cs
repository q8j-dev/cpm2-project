using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	[Serializable]
	public class RangeInt
	{
		public int Min;

		public int Max;

		public int GetRandom()
		{
			return UnityEngine.Random.Range(Min, Max);
		}
	}
}
