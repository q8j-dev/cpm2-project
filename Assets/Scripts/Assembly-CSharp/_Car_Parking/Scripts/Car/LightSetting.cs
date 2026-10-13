using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	[Serializable]
	public class LightSetting
	{
		public LightSideType Side;

		public float LightDistance = 200f;

		public float LightIntensity = 200f;

		public float InnerSpotAngle;

		public float OuterSpotAngle;

		public Texture2D LowBeamCookie;

		public Texture2D HighBeamCookie;
	}
}
