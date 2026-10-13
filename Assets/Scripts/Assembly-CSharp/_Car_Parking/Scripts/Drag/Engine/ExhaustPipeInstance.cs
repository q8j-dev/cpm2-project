using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class ExhaustPipeInstance : PartInstance<ExhaustPipePart>
	{
		private readonly float _diameterCm;

		public float GetDiameter => Mathf.Clamp(_diameterCm, 3f, 10f) * GetEfficiency(base.Condition, 0.8f);

		public ExhaustPipeInstance(ExhaustPipePart part)
		{
			Part = part;
			_diameterCm = Part.diameter * 0.1f;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((((Part.GetReliability > 0f) ? (value.I / Part.GetReliability) : 0f) < 1f) ? 0f : 1f);
			Mileage += value.P + 0.1f * num;
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 3603.4324f || _diameterCm > 10f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
