using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class ThrottleValveInstance : PartInstance<ThrottleValvePart>
	{
		private readonly float _diameter;

		public float GetDiameter => _diameter * GetEfficiency(base.Condition, 0.8f);

		public ThrottleValveInstance(ThrottleValvePart part)
		{
			Part = part;
			_diameter = Mathf.Clamp(part.GetDiameter * 0.1f, 3f, 9.5f);
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((((Part.GetReliability > 0f) ? (value.I / Part.GetReliability) : 0f) < 1f) ? 0f : 1f);
			Mileage += value.P + 0.1f * num;
		}
	}
}
