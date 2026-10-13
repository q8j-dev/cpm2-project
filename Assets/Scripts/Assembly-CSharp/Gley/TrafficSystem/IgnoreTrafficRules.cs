using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class IgnoreTrafficRules : VehicleBehaviour
	{
		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			MovementInfo.OnKnownListUpdated += KnownListUpdatedHandler;
			base.VehicleComponent.MovementInfo.SetIgnoreRules(ignoreRules: true);
		}

		protected override void OnBecameInactive()
		{
			base.OnBecameInactive();
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
			base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f);
			base.VehicleComponent.MovementInfo.SetIgnoreRules(ignoreRules: false);
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			if (base.VehicleComponent.MovementInfo.ClosestObstacle.Collider != null)
			{
				if (base.VehicleComponent.MovementInfo.ClosestObstacle.ObstacleType == ObstacleTypes.StaticObject)
				{
					return new BehaviourResult();
				}
				if (base.VehicleComponent.MovementInfo.ClosestObstacle.ObstacleType == ObstacleTypes.TrafficVehicle)
				{
					FollowVehicle followVehicle = (FollowVehicle)API.GetVehicleBehaviourOfType<FollowVehicle>(base.VehicleIndex);
					if (followVehicle != null)
					{
						followVehicle.SetMinFollowSpeed(0f);
						followVehicle.SetOvertakeTime(0f);
					}
					((Overtake)API.GetVehicleBehaviourOfType<Overtake>(base.VehicleIndex))?.SetIgnoreTime(ignoreTime: true);
					return new BehaviourResult();
				}
			}
			BehaviourResult behaviourResult = new BehaviourResult(100);
			behaviourResult.MaxAllowedSpeed = base.VehicleComponent.MaxSpeed;
			PerformForwardMovement(ref behaviourResult, base.VehicleComponent.MaxSpeed, base.VehicleComponent.MaxSpeed, knownWaypointsList.GetFirstStopPosition(), 2f, 1f, base.VehicleComponent.distanceToStop);
			return behaviourResult;
		}

		private void KnownListUpdatedHandler(int vehicleIndex)
		{
			if (vehicleIndex == base.VehicleIndex)
			{
				int num = math.clamp(base.VehicleComponent.MovementInfo.GetAngle(1), 0, 20);
				if (num > 2 && base.VehicleComponent.GetCurrentSpeedMS().ToKMH() / (float)base.VehicleComponent.maxPossibleSpeed > 0.6f)
				{
					base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f - (float)num / 45f);
				}
				else
				{
					base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f);
				}
			}
		}

		public override void OnDestroy()
		{
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
		}
	}
}
