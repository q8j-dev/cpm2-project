using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class FlywheelInstance : ReciprocatingEnginePartInstance<FlywheelPart>
	{
		private float _wear = 1f;

		private readonly float _maxRpm;

		private readonly float _inertia;

		public float GetInertia => inertia;

		public FlywheelInstance(FlywheelPart part)
		{
			Part = part;
			_maxRpm = Mathf.Clamp(Part.maxRPM, 0f, 20000f);
			inertia = part.GetInertia();
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / _maxRpm > 1f) ? (value.R / _maxRpm) : 0f);
			float wearMultiplier = OilStarvation.GetWearMultiplier(value.O);
			float num2 = (value.P + 0.06f * num) * wearMultiplier;
			if (!float.IsFinite(num2) || Mathf.Abs(num2) > 5f)
			{
				num2 = 0f;
			}
			Mileage += num2;
			if (!float.IsFinite(Mileage) && Part != null && Part.MaxMileage > 0f)
			{
				Mileage = Part.MaxMileage;
			}
		}

		public override void UpdateLoad(Pv value)
		{
			bool num = float.IsFinite(value.I) && value.I < 50000f && value.I > 3602.2842f;
			bool flag = float.IsFinite(value.R) && value.R < 50000f && value.R > 20000f;
			if (num | flag)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
