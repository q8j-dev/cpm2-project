using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	[Serializable]
	public class RangeFloat
	{
		public float Min;

		public float Max;

		public float GetRandom()
		{
			return UnityEngine.Random.Range(Min, Max);
		}
	}
}
