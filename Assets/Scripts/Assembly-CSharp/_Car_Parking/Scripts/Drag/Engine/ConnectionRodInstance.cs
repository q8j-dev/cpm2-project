using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class ConnectionRodInstance : ReciprocatingEnginePartInstance<ConnectionRodsPart>
	{
		private readonly float _length;

		public float GetLength => _length;

		public ConnectionRodInstance(ConnectionRodsPart part)
		{
			Part = part;
			_length = Part.GetLength;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / Part.maxRPM > 1f) ? (value.R / Part.maxRPM) : 0f);
			float num2 = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			float wearMultiplier = OilStarvation.GetWearMultiplier(value.O);
			float num3 = (value.P + (0.01f * num + 0.6f * num2)) * wearMultiplier;
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
