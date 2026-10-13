using Unity.Mathematics;

namespace Gley.TrafficSystem
{
	public class ChangeLane : VehicleBehaviour
	{
		private RoadSide _roadSide;

		private bool _checkWaypoint;

		protected override void OnBecomeActive()
		{
			base.OnBecomeActive();
			MovementInfo.OnKnownListUpdated += KnownListUpdatedHandler;
		}

		protected override void OnBecameInactive()
		{
			base.OnBecameInactive();
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
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
				if (otherLaneWaypointIndex != -1 && API.AllPreviousWaypointsAreFree(otherLaneWaypointIndex, base.VehicleIndex))
				{
					API.AddWaypointAndClear(otherLaneWaypointIndex, base.VehicleIndex);
					Stop();
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
			MovementInfo.OnKnownListUpdated -= KnownListUpdatedHandler;
		}
	}
}
