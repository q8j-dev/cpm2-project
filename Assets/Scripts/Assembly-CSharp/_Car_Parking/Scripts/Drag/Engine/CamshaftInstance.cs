using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class CamshaftInstance<TPart> : ReciprocatingEnginePartInstance<TPart> where TPart : CamshaftPart
	{
		internal float time_in_open;

		internal float time_in_close;

		internal float time_out_open;

		internal float time_out_close;

		public override List<PartInstanceBase> SubParts { get; }

		public new virtual void UpdateVariables()
		{
			if (Part.static_input_time_in_duration > 0f)
			{
				time_in_open = 360f - Part.static_input_lobe_separation - Part.static_input_time_in_duration / 2f;
				time_in_close = time_in_open + Part.static_input_time_in_duration;
			}
			else
			{
				time_in_open = 0f;
				time_in_close = 0f;
			}
			if (Part.static_input_time_out_duration > 0f)
			{
				time_out_open = 360f + Part.static_input_lobe_separation - Part.static_input_time_out_duration / 2f;
				time_out_close = time_out_open + Part.static_input_time_out_duration;
			}
			else
			{
				time_out_open = 0f;
				time_out_close = 0f;
			}
			time_in_open = Utils.RollTo(time_in_open, 0f, 720f);
			time_in_close = Utils.RollTo(time_in_close, 0f, 720f);
			time_out_open = Utils.RollTo(time_out_open, 0f, 720f);
			time_out_close = Utils.RollTo(time_out_close, 0f, 720f);
			time_in_open = Utils.RoundTo(time_in_open / 720f, 4);
			time_in_close = Utils.RoundTo(time_in_close / 720f, 4);
			time_out_open = Utils.RoundTo(time_out_open / 720f, 4);
			time_out_close = Utils.RoundTo(time_out_close / 720f, 4);
		}

		public override void UpdateWear(Pv value)
		{
			float num = Mathf.Clamp01(value.R / Part.maxRPM);
			float num2 = ((Part.MaxMileage > 0f) ? Mathf.Clamp01(value.P / Part.MaxMileage) : 0f);
			float num3 = Mathf.Clamp01(value.I / Part.GetReliability);
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			float wearMultiplier = OilStarvation.GetWearMultiplier(value.O);
			float num4 = (num2 * 0.3f + 0.2f * num + 0.5f * num3) * wearMultiplier * Time.deltaTime;
			if (!float.IsFinite(num4) || Mathf.Abs(num4) > 5f)
			{
				num4 = 0f;
			}
			Mileage += num4;
			if (!float.IsFinite(Mileage) && Part != null && Part.MaxMileage > 0f)
			{
				Mileage = Part.MaxMileage;
			}
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.P > 3659.9998f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
