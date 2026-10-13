using UnityEngine;
using _Car_Parking.Scripts.ArtistTools.Enums;

namespace _Car_Parking.Scripts.ArtistTools.BakedData
{
	public class BakedAnimParameters
	{
		public Vector3 ExplosionCenter { get; }

		public float Impulse { get; }

		public BakedAnimSpeedType SpeedType { get; }

		public BakedAnimParameters(Vector3 explosionCenter, float impulse, BakedAnimSpeedType speedType)
		{
			ExplosionCenter = explosionCenter;
			Impulse = impulse;
			SpeedType = speedType;
		}
	}
}
