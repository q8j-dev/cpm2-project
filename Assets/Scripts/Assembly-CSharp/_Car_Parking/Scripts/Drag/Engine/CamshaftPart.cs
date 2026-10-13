using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public class CamshaftPart : ReciprocatingEnginePart
	{
		public float static_input_lobe_separation;

		[Header("Intake parameters")]
		[SerializeField]
		public float static_input_time_in_duration;

		public float lift_in_open;

		public float lift_in_close;

		[Header("Exhaust parameters")]
		[SerializeField]
		public float static_input_time_out_duration;

		public float lift_out_open;

		public float lift_out_close;

		public override string GetDescription => string.Format(new LocalizedString(Description), this.GetEngineName(), GetMaxReliability, maxRPM, this.GetPartVendorName());

		public override float GetInertia()
		{
			if (inertia > 0f)
			{
				return inertia * 2f;
			}
			inertia = Utils.KgToInertia(Mass);
			return inertia * 2f;
		}
	}
}
