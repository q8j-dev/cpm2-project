using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public class AirFuelDeliverySystemInstance<TPart> : PartInstance<TPart> where TPart : AirFuelDeliverySystem
	{
		internal float mixture_ratio = 1f;

		internal float optimal_mixture_ratio = 1f;

		private float optimal_burn_time_base = 0.003f;

		private float optimal_burn_time_offset = 0.001f;

		internal float mixture_H = 1f;

		internal float time_burn = 0.1f;

		internal float minComp = default;

		internal float maxComp = default;

		public virtual void UpdateVariables(float condition)
		{
		}
	}
}
