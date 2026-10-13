using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class AvoidReverse : VehicleBehaviour
	{
		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			BehaviourResult behaviourResult = new BehaviourResult();
			behaviourResult.TargetGear = -1;
			if (currentGear == behaviourResult.TargetGear)
			{
				if (base.VehicleComponent.frontTrigger.transform.localEulerAngles.y > 0f)
				{
					Steer(ref behaviourResult, -1f);
				}
				else
				{
					Steer(ref behaviourResult, 1f);
				}
			}
			if (requiredBrakePower > 0.1f && requiredBrakePower < 1f)
			{
				requiredBrakePower = 1f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), 0f, knownWaypointsList.ClosestObstaclePoint, requiredBrakePower, 0.95f, base.VehicleComponent.distanceToStop);
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
