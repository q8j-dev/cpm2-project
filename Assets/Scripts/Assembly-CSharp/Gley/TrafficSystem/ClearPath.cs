using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class ClearPath : VehicleBehaviour
	{
		private RoadSide _roadSide;

		private float _accelerationPercent;

		private bool _checkWaypoint;

		protected override void OnBecomeActive()
		{
			_accelerationPercent = 1f;
			base.OnBecomeActive();
			MovementInfo.OnKnownListUpdated += KnownListUpdatedHandler;
			if (_roadSide == RoadSide.Left)
			{
				base.VehicleComponent.MovementInfo.SetOffset(-1f);
			}
			else
			{
				base.VehicleComponent.MovementInfo.SetOffset(1f);
			}
			((FollowVehicle)API.GetVehicleBehaviourOfType<FollowVehicle>(base.VehicleIndex)).DisableOvertake(active: true);
		}

		protected override void OnBecameInactive()
		{
			base.OnBecameInactive();
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
			base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(1f);
			base.VehicleComponent.MovementInfo.ResetOffset();
			((FollowVehicle)API.GetVehicleBehaviourOfType<FollowVehicle>(base.VehicleIndex)).DisableOvertake(active: false);
		}

		public override void SetParams(object[] parameters)
		{
			_roadSide = (RoadSide)parameters[0];
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			if (_checkWaypoint)
			{
				_checkWaypoint = false;
				int otherLaneWaypointIndex = base.TrafficWaypointsData.GetOtherLaneWaypointIndex(knownWaypointsList.GetWaypointIndex(0), base.VehicleComponent.VehicleType, _roadSide, base.VehicleComponent.GetForwardVector());
				if (otherLaneWaypointIndex != -1)
				{
					if (API.AllPreviousWaypointsAreFree(otherLaneWaypointIndex, base.VehicleIndex))
					{
						API.AddWaypointAndClear(otherLaneWaypointIndex, base.VehicleIndex);
					}
				}
				else
				{
					if (base.TrafficWaypointsData.GetWaypointFromIndex(knownWaypointsList.GetWaypointIndex(0)).IsConnector)
					{
						_accelerationPercent = 1f;
					}
					_accelerationPercent *= 0.9f;
					if (_accelerationPercent < 0.1f)
					{
						_accelerationPercent = 0.1f;
					}
					base.VehicleComponent.MovementInfo.SetMaxSpeedCorrectionPercent(_accelerationPercent);
				}
			}
			return new BehaviourResult();
		}

		private void KnownListUpdatedHandler(int vehicleIndex)
		{
			if (vehicleIndex == base.VehicleIndex)
			{
				_checkWaypoint = true;
			}
		}

		public override void OnDestroy()
		{
		}
	}
}
