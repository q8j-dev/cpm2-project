using System;
using CPMEngine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class IntercoolerInstance<TPart> : PartInstance<TPart> where TPart : IntercoolerPart
	{
		private float _intercoolerPressure => Part.IntercoolingPressure;

		public override void UpdateWear(Pv value)
		{
			float num = ((value.I > Part.GetReliability) ? (value.I / Part.GetReliability) : 0f);
			float num2 = (((float)value.D / _intercoolerPressure > 1f) ? ((float)value.D / _intercoolerPressure) : 0f);
			Mileage += value.P + (0.25f * num + num2);
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 4004.6438f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
