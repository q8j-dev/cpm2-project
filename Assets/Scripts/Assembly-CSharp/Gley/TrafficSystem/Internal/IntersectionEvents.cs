using System.Collections.Generic;

namespace Gley.TrafficSystem.Internal
{
	public class IntersectionEvents
	{
		public delegate void ActiveIntersectionsChanged(List<GenericIntersection> activeIntersections);

		public static event ActiveIntersectionsChanged OnActiveIntersectionsChanged;

		public static void TriggerActiveIntersectionsChangedEvent(List<GenericIntersection> activeIntersections)
		{
			OnActiveIntersectionsChanged?.Invoke(activeIntersections);
		}
	}
}
