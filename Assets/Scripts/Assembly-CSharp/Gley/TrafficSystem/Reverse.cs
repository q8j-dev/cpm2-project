using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class Reverse : VehicleBehaviour
	{
		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			return new BehaviourResult
			{
				TargetGear = -1
			};
		}

		public override void OnDestroy()
		{
		}
	}
}
