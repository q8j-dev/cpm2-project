using System;
using CPMEngine;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class CylinderHeadInstance : ReciprocatingEnginePartInstance<CylinderHeadPart>
	{
		[Header("Head output parameters")]
		internal float InMin;

		internal float OutMin;

		internal float InMax;

		internal float OutMax;

		internal float TimeInOpen;

		internal float TimeInClose;

		internal float TimeOutOpen;

		internal float TimeOutClose;

		private float _wear = 1f;

		internal float IntakeTunnelArea => Utils.RoundTo(GetEfficiency(base.Condition, 0.9f) * ((float)Math.PI / 4f) * math.pow(Part.intake_valve_diam * 0.001f, 2f) * Part.intake_valves_per_cylinder, 7);

		internal float ExhaustTunnelArea => Utils.RoundTo(GetEfficiency(base.Condition, 0.9f) * ((float)Math.PI / 4f) * math.pow(Part.exhaust_valve_diam * 0.001f, 2f) * Part.exhaust_valves_per_cylinder, 7);

		public virtual float GetVmin(CylinderHeadPart cylinderHead)
		{
			return cylinderHead.Vmin;
		}

		public virtual float GetInertia()
		{
			return Part.GetInertia() * 2f;
		}

		public virtual void UpdateParams()
		{
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((value.R / Part.maxRPM > 1f) ? (value.R / Part.maxRPM) : 0f);
			float num2 = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			Mileage += value.P + (0.1f * num + 0.35f * num2);
			if (SubParts == null || SubParts.Count == 0)
			{
				return;
			}
			foreach (PartInstanceBase subPart in SubParts)
			{
				subPart?.UpdateWear(value);
			}
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 4001.744f)
			{
				Mileage += Part.MaxMileage;
			}
			base.UpdateLoad(value);
		}
	}
}
