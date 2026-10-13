using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	[Serializable]
	public class RemapVar
	{
		public RangeFloat Source;

		public RangeFloat Destination;

		public float GetValue(float value)
		{
			return Mathf.Lerp(Destination.Min, Destination.Max, Mathf.InverseLerp(Source.Min, Source.Max, value));
		}
	}
}
