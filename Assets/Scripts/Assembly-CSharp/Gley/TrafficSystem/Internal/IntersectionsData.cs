using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	public class IntersectionsData : MonoBehaviour
	{
		[SerializeField]
		private IntersectionDataType[] _allIntersections;

		[SerializeField]
		private PriorityIntersectionData[] _allPriorityIntersections;

		[SerializeField]
		private TrafficLightsIntersectionData[] _allLightsIntersections;

		[SerializeField]
		private TrafficLightsCrossingData[] _allLightsCrossings;

		[SerializeField]
		private PriorityCrossingData[] _allPriorityCrossings;

		public IntersectionDataType[] AllIntersections => _allIntersections;

		public PriorityIntersectionData[] AllPriorityIntersections => _allPriorityIntersections;

		public TrafficLightsIntersectionData[] AllLightsIntersections => _allLightsIntersections;

		public TrafficLightsCrossingData[] AllLightsCrossings => _allLightsCrossings;

		public PriorityCrossingData[] AllPriorityCrossings => _allPriorityCrossings;

		public void SetTrafficIntersectionData(IntersectionDataType[] allIntersections, TrafficLightsIntersectionData[] allLightsIntersections, PriorityIntersectionData[] allPriorityIntersections, TrafficLightsCrossingData[] allLightsCrossings, PriorityCrossingData[] allPriorityCrossings)
		{
			_allIntersections = allIntersections;
			_allLightsCrossings = allLightsCrossings;
			_allLightsIntersections = allLightsIntersections;
			_allPriorityIntersections = allPriorityIntersections;
			_allPriorityCrossings = allPriorityCrossings;
		}

		public bool IsValid(out string error)
		{
			error = string.Empty;
			if (_allIntersections == null || _allLightsCrossings == null || _allLightsIntersections == null || _allPriorityCrossings == null || _allPriorityIntersections == null)
			{
				error = TrafficSystemErrors.NullIntersectionData;
				return false;
			}
			return true;
		}
	}
}
