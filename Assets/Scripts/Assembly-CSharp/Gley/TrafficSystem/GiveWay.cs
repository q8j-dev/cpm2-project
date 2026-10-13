using Gley.UrbanSystem.Internal;
using Unity.Mathematics;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class GiveWay : VehicleBehaviour
	{
		public delegate void PassageGranted(int vehicleIndex, int waypointIndex);

		public static PassageGranted OnPassageGranted;

		public static void TriggerPassageGrantedEvent(int vehicleIndex, int waypointIndex)
		{
			OnPassageGranted?.Invoke(vehicleIndex, waypointIndex);
		}

		public override BehaviourResult Execute(MovementInfo knownWaypointsList, float requiredBrakePower, bool stopTargetReached, float3 stopPosition, int currentGear)
		{
			if (!stopPosition.Equals(knownWaypointsList.GetFirstGiveWayPosition()))
			{
				Vector3 vector = knownWaypointsList.GetFirstGiveWayPosition() - base.VehicleComponent.FrontPosition.position;
				if (Vector3.Dot(base.VehicleComponent.GetForwardVector(), vector.normalized) < 0f)
				{
					stopTargetReached = true;
				}
				else
				{
					requiredBrakePower = 0f;
					stopTargetReached = false;
				}
			}
			BehaviourResult behaviourResult = new BehaviourResult();
			if (stopTargetReached)
			{
				requiredBrakePower = 1f;
			}
			PerformForwardMovement(ref behaviourResult, knownWaypointsList.GetFirstWaypointSpeed(), 0f, knownWaypointsList.GetFirstGiveWayPosition(), requiredBrakePower, 0.8f, 0f);
			if (stopTargetReached)
			{
				behaviourResult.BrakePercent = 6f;
				behaviourResult.MaxAllowedSpeed = 0f;
			}
			if (knownWaypointsList.IsFirstWaypointGiveWay())
			{
				base.VehicleComponent.BlinkersController.UpdateBlinkers(BlinkType.Start);
				GiveWayType giveWayType = knownWaypointsList.GetGiveWayType(0);
				switch (giveWayType)
				{
				case GiveWayType.Standard:
					if (knownWaypointsList.GetGiveWayType(1) == GiveWayType.Zipper)
					{
						giveWayType = GiveWayType.Zipper;
					}
					break;
				case GiveWayType.Zipper:
					giveWayType = GiveWayType.None;
					break;
				}
				switch (giveWayType)
				{
				case GiveWayType.Intersection:
					if (CanEnterIntersection(knownWaypointsList.GetWaypointIndex(0)))
					{
						TriggerPassageGrantedEvent(base.VehicleIndex, knownWaypointsList.GetWaypointIndex(0));
					}
					break;
				case GiveWayType.Complex:
					if (API.AllRequiredWaypointsAreFree(knownWaypointsList.GetWaypointIndex(0), base.VehicleIndex))
					{
						TriggerPassageGrantedEvent(base.VehicleIndex, knownWaypointsList.GetWaypointIndex(0));
					}
					break;
				case GiveWayType.Zipper:
					if (!API.IsThisWaypointADestination(knownWaypointsList.GetWaypointIndex(1), base.VehicleIndex))
					{
						TriggerPassageGrantedEvent(base.VehicleIndex, knownWaypointsList.GetWaypointIndex(0));
						TriggerPassageGrantedEvent(base.VehicleIndex, knownWaypointsList.GetWaypointIndex(0));
					}
					break;
				case GiveWayType.Standard:
					if (API.AllPreviousWaypointsAreFree(knownWaypointsList.GetWaypointIndex(1), base.VehicleIndex))
					{
						TriggerPassageGrantedEvent(base.VehicleIndex, knownWaypointsList.GetWaypointIndex(0));
					}
					break;
				case GiveWayType.None:
					TriggerPassageGrantedEvent(base.VehicleIndex, knownWaypointsList.GetWaypointIndex(0));
					break;
				}
			}
			return behaviourResult;
		}

		private bool CanEnterIntersection(int waypointIndex)
		{
			foreach (IIntersection associatedIntersection in base.TrafficWaypointsData.AllTrafficWaypoints[waypointIndex].AssociatedIntersections)
			{
				if (!associatedIntersection.IsPathFree(waypointIndex))
				{
					return false;
				}
			}
			return true;
		}

		public override void OnDestroy()
		{
		}
	}
}
