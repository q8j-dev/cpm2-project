using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class Stop : VehicleBehaviour
	{
		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			PerformForwardMovement(ref behaviourResult, 0f, 0f, TrafficSystemConstants.DEFAULT_POSITION, 1f, 1f, 0f);
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
