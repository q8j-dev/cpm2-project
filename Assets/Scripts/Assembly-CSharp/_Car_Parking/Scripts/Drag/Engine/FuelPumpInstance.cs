using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class FuelPumpInstance : PartInstance<FuelPumpPart>
	{
		private readonly float _inputEfficiency;

		private readonly float _inputPressure;

		public float GetPressure => Mathf.Clamp(_inputPressure, 1f, 9f);

		public float GetOutputEfficiency => _inputEfficiency * GetEfficiency(base.Condition, 0.5f) * 0.74f / 3.6f;

		public FuelPumpInstance(FuelPumpPart part)
		{
			Part = part;
			_inputEfficiency = Part.GetEfficiency;
			_inputPressure = Part.GetPressure;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((Part.GetReliability > 0f && value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			Mileage += value.P + 0.3f * num;
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 3607.0583f || _inputPressure > 9f || _inputEfficiency > 400f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
