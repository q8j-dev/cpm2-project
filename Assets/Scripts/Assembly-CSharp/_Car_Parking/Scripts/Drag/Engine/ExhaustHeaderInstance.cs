using System;
using CPMEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class ExhaustHeaderInstance : PartInstance<ExhaustHeaderPart>
	{
		private readonly float _efficiency;

		public float GetBlowing => _efficiency * (1f + GetInvEfficiency(base.Condition, 0.1f));

		public ExhaustHeaderInstance(ExhaustHeaderPart part)
		{
			Part = part;
			_efficiency = Part.efficiency;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((((Part.GetReliability > 0f) ? (value.I / Part.GetReliability) : 0f) < 1f) ? 0f : 1f);
			Mileage += value.P + 0.1f * num;
		}
	}
}
