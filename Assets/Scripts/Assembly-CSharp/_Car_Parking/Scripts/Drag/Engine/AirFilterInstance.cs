using System;
using CPMEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class AirFilterInstance : AirFuelDeliverySystemInstance<AirFilterPart>
	{
		private readonly float _dustedEfficiency;

		private readonly float _maxAirConsumption;

		public float GetAirConsumption => _maxAirConsumption * GetEfficiency(base.Condition, 0.8f);

		public float Efficiency => _dustedEfficiency * GetEfficiency(base.Condition, 0.4f);

		public AirFilterInstance(AirFilterPart part)
		{
			Part = part;
			_dustedEfficiency = Part.GetDust;
			_maxAirConsumption = Part.GetInputMaxAir;
		}

		public override void UpdateWear(Pv value)
		{
			Mileage += value.P;
		}
	}
}
