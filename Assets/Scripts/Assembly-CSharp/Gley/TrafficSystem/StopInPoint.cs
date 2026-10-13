using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class StopInPoint : VehicleBehaviour
	{
		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			if (!stopPosition.Equals(knownWaypointsList.GetFirstStopPosition()))
			{
				requiredBrakePower = 0f;
				stopTargetReached = false;
			}
			BehaviourResult behaviourResult = new BehaviourResult();
			if (stopTargetReached)
			{
				requiredBrakePower = 10f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), 0f, knownWaypointsList.GetFirstStopPosition(), requiredBrakePower, 0.8f, 0f);
			return behaviourResult;
		}

		public override void OnDestroy()
		{
		}
	}
}
