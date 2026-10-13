using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class StopInDistance : VehicleBehaviour
	{
		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			if (math.lengthsq(stopPosition - knownWaypointsList.ClosestObstaclePoint) > 1f)
			{
				requiredBrakePower = 0f;
				stopTargetReached = false;
			}
			BehaviourResult behaviourResult = new BehaviourResult();
			if (stopTargetReached)
			{
				requiredBrakePower = 10f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), 0f, knownWaypointsList.ClosestObstaclePoint, requiredBrakePower, 0.95f, base.VehicleComponent.distanceToStop);
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
