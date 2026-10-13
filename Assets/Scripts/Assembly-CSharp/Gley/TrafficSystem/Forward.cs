using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class Forward : VehicleBehaviour
	{
		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), knownWaypointsList.GetFirstWaypointSpeed(), TrafficSystemConstants.DEFAULT_POSITION, 0f, float.PositiveInfinity, 0f);
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
