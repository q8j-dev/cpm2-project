using System;
using _Car_Parking.Scripts.ArtistTools.Enums;

namespace _Car_Parking.Scripts.ArtistTools.BakedData
{
	[Serializable]
	public struct ImpulseBySpeedTypeModel
	{
		public BakedAnimSpeedType SpeedType;

		public float ImpulseValue;

		public ImpulseBySpeedTypeModel(BakedAnimSpeedType speedType, float impulseValue)
		{
			SpeedType = speedType;
			ImpulseValue = impulseValue;
		}
	}
}
