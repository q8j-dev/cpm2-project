using System;
using CPMEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class FuelInjectorsInstance : AirFuelDeliverySystemInstance<FuelInjectorsPart>
	{
		public readonly float InjectorPerformance;

		public float GetPerformance => InjectorPerformance * GetEfficiency(base.Condition, 0.3f);

		public FuelInjectorsInstance(FuelInjectorsPart part)
		{
			Part = part;
			InjectorPerformance = part.GetPerformance;
		}

		public override void UpdateWear(Pv value)
		{
			float num = ((((Part.GetReliability > 0f) ? (value.I / Part.GetReliability) : 0f) < 1f) ? 0f : 1f);
			Mileage += value.P + 0.5f * num;
		}

		public override void UpdateLoad(Pv value)
		{
			if (value.I > 4008.4683f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
