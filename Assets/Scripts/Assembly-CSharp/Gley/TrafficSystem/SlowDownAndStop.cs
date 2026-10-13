using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class SlowDownAndStop : VehicleBehaviour
	{
		private float _accelerationPercent;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			_accelerationPercent = 1f;
			MovementInfo.OnKnownListUpdated += KnownListUpdatedHandler;
		}

		protected override void OnBecameInactive()
		{
			base.OnBecameInactive();
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
			base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f);
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			return new BehaviourResult();
		}

		private void KnownListUpdatedHandler(int vehicleIndex)
		{
			if (vehicleIndex == base.VehicleIndex)
			{
				_accelerationPercent *= 0.9f;
				if (_accelerationPercent < 0.1f)
				{
					_accelerationPercent = 0.1f;
				}
				base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(_accelerationPercent);
			}
		}

		public override void OnDestroy()
		{
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
		}
	}
}
