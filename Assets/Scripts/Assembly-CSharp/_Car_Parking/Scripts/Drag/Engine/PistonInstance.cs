using System;
using CPMEngine;
using Unity.Mathematics;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class PistonInstance : ReciprocatingEnginePartInstance<PistonsPart>
	{
		private float _wear = 1f;

		private readonly float _bore;

		private readonly float _height;

		public float Bore => _bore;

		public float Height => _height;

		public float GetCrownVolume => (float)Math.PI / 4f * math.pow(_bore, 2f) * _height / 1000f;

		public PistonInstance(PistonsPart part)
		{
			Part = part;
			_bore = Part.GetBore;
			_height = Part.GetHeight;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / Part.maxRPM > 1f) ? (value.R / Part.maxRPM) : 0f);
			float num2 = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			float wearMultiplier = OilStarvation.GetWearMultiplier(value.O);
			float num3 = (value.P + (0.01f * num + 0.4f * num2)) * wearMultiplier;
			if (!float.IsFinite(num3) || Mathf.Abs(num3) > 5f)
			{
				num3 = 0f;
			}
			Mileage += num3;
			if (!float.IsFinite(Mileage) && Part != null && Part.MaxMileage > 0f)
			{
				Mileage = Part.MaxMileage;
			}
		}

		public override void UpdateLoad(Pv value)
		{
			if (float.IsFinite(value.I) && value.I < 50000f && value.I > 3659.9998f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
