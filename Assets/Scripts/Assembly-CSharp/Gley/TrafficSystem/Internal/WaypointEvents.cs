namespace Gley.TrafficSystem.Internal
{
	public static class WaypointEvents
	{
		public delegate void TrafficLightChanged(int waypointIndex, bool stop);

		public delegate void StopStateChanged(int waypointIndex, bool stopState);

		public delegate void GiveWayStateChanged(int waypointIndex, GiveWayType giveWayType);

		public static event TrafficLightChanged OnTrafficLightChanged;

		public static event StopStateChanged OnStopStateChanged;

		public static event GiveWayStateChanged OnGiveWayStateChanged;

		public static void TriggerTrafficLightChangedEvent(int waypointIndex, bool stop)
		{
			OnTrafficLightChanged?.Invoke(waypointIndex, stop);
		}

		public static void TriggerStopStateChangedEvent(int waypointIndex, bool stopState)
		{
			OnStopStateChanged?.Invoke(waypointIndex, stopState);
		}

		public static void TriggerGiveWayStateChangedEvent(int waypointIndex, GiveWayType giveWayState)
		{
			OnGiveWayStateChanged?.Invoke(waypointIndex, giveWayState);
		}
	}
}
