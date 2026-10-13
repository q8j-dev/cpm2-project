using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class Decelerate : VehicleBehaviour
	{
		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), knownWaypointsList.GetFirstSlowDownSpeed(), knownWaypointsList.GetFirstSlowDownPosition(), requiredBrakePower, 0.5f, 0f);
			if (knownWaypointsList.GetFirstSlowDownSpeed() == TrafficSystemConstants.DEFAULT_SPEED)
			{
				Stop();
			}
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
