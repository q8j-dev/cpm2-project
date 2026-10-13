using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class FuelRailInstance : AirFuelDeliverySystemInstance<FuelRailPart>
	{
		internal float default_mixture_ratio = 1f;

		private readonly float _defaultEfficiency;

		private readonly float _defaultMFC;

		private readonly FuelType _inputFuelType;

		public float GetOutputMFC => _defaultMFC;

		public float GetOutputEfficiency => _defaultEfficiency * GetEfficiency(base.Condition, 0.6f);

		public FuelType GetFuelType => _inputFuelType;

		public FuelRailInstance(FuelRailPart part)
		{
			Part = part;
			_defaultEfficiency = part.GetEfficiency;
			_defaultMFC = part.GetMFC;
			_inputFuelType = part.GetFT;
		}

		public virtual void RandomizeMixtureRatio(float power)
		{
			mixture_ratio = optimal_mixture_ratio;
			if (power > 0f && power <= 1f)
			{
				float num = 0.5f + power * 0.5f;
				float num2 = 1f - num;
				mixture_ratio = mixture_ratio * num + UnityEngine.Random.Range(0.01f, 1f) * mixture_ratio * num2 - 0.5f * mixture_ratio * num2;
			}
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((Part.GetReliability > 0f) ? (value.I / Part.GetReliability) : 0f);
			float num2 = value.P + 0.05f * num;
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
			if (float.IsFinite(value.I) && value.I < 50000f && value.I > 4003.847f)
			{
				Mileage += Part.MaxMileage;
			}
		}

		public override void UpdateVariables(float wear)
		{
			RandomizeMixtureRatio(1f);
		}
	}
}
