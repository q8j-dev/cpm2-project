using System.Collections.Generic;
using Gley.UrbanSystem.Internal;

namespace Gley.TrafficSystem.Internal
{
	public class DisabledWaypointsManager : IDestroyable
	{
		private readonly TrafficWaypointsData _trafficWaypointsData;

		private readonly WaypointSelector _waypointSelector;

		private List<int> _disabledWaypoints;

		public List<int> DisabledWaypoints => _disabledWaypoints;

		public DisabledWaypointsManager(TrafficWaypointsData trafficWaypointsData, WaypointSelector waypointSelector, Area area)
		{
			Assign();
			_disabledWaypoints = new List<int>();
			_trafficWaypointsData = trafficWaypointsData;
			_waypointSelector = waypointSelector;
			if (area.radius > 0f)
			{
				DisableAreaWaypoints(area);
			}
		}

		public void Assign()
		{
			DestroyableManager.Instance.Register(this);
		}

		public void EnableAllWaypoints()
		{
			foreach (int disabledWaypoint in _disabledWaypoints)
			{
				_trafficWaypointsData.AllTrafficWaypoints[disabledWaypoint].TemporaryDisabled = false;
			}
			_disabledWaypoints = new List<int>();
		}

		public void AddDisabledWaypoint(int waypointIndex)
		{
			_disabledWaypoints.Add(waypointIndex);
			_trafficWaypointsData.AllTrafficWaypoints[waypointIndex].TemporaryDisabled = true;
		}

		public void DisableAreaWaypoints(Area area)
		{
			foreach (int areaWaypoint in _waypointSelector.GetAreaWaypoints(area))
			{
				AddDisabledWaypoint(areaWaypoint);
			}
		}

		public void OnDestroy()
		{
		}
	}
}
