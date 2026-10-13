using System;
using CPMEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class EccentricShaftInstance : ReciprocatingEnginePartInstance<EccentricShaftPart>
	{
		private readonly float _eccentricity;

		private readonly float _inertia;

		private readonly float _maxRpm;

		private readonly float _reliability;

		public float Eccentricity => _eccentricity;

		public float GetInertia => _inertia;

		public EccentricShaftInstance(EccentricShaftPart part)
		{
			Part = part;
			_eccentricity = part.Eccentricity;
			_inertia = part.GetInertia();
			_maxRpm = part.maxRPM;
			_reliability = part.GetReliability;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / _maxRpm > 1f) ? (value.R / _maxRpm) : 0f);
			float num2 = ((value.I > _reliability) ? (value.I / _reliability) : 0f);
			float wearMultiplier = OilStarvation.GetWearMultiplier(value.O);
			Mileage += (value.P + (0.01f * num + 0.4f * num2)) * wearMultiplier;
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 3659.9998f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
