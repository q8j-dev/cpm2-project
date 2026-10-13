using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public abstract class ReciprocatingEnginePartInstance<TPart> : PartInstance<TPart> where TPart : ReciprocatingEnginePart
	{
		[Header("Output parameters")]
		internal float inertia;

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / Part.maxRPM > 1f) ? (value.R / Part.maxRPM) : 0f);
			float num2 = ((Part.GetReliability <= 0f) ? 0f : (value.I / Part.GetReliability));
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
	}
}
