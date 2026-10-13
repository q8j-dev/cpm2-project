using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class CrankshaftInstance : ReciprocatingEnginePartInstance<CrankshaftPart>
	{
		private readonly float _stroke;

		private readonly float _inertia;

		private readonly float _maxRpm;

		private readonly float _reliability;

		public float Stroke => _stroke;

		public float GetInertia => _inertia;

		public CrankshaftInstance(CrankshaftPart part)
		{
			Part = part;
			_stroke = part.GetStroke;
			_inertia = part.GetInertia();
			_maxRpm = part.maxRPM;
			_reliability = part.GetReliability;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / _maxRpm > 1f) ? (value.R / _maxRpm) : 0f);
			float num2 = ((value.I > _reliability) ? (value.I / _reliability) : 0f);
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
