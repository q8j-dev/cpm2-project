using CPMEngine;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public class AirFuelDeliverySystem : PartItem
	{
		internal float max_air_consumption = 1E-05f;

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix;
	}
}
