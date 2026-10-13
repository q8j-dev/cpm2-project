using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class NoWaypoints : VehicleBehaviour
	{
		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			if (stopTargetReached)
			{
				requiredBrakePower = 10f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), 0f, knownWaypointsList.GetLastWaypointPosition(), requiredBrakePower, 1f, 0f);
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
