using System.Collections.Generic;

namespace _Car_Parking.Scripts.PlayerProfile
{
	public static class PoseOwnership
	{
		private static IPlayerProfileService _service;

		public static void Initialize(IPlayerProfileService service)
		{
			_service = service;
		}

		public static bool Owns(int ikPoseId)
		{
			if (_service != null)
			{
				return _service.OwnsPose(ikPoseId);
			}
			return false;
		}

		public static void Add(IReadOnlyCollection<int> ikPoseIds)
		{
			_service?.AddOwnedPoses(ikPoseIds);
		}

		public static void Remove(IReadOnlyCollection<int> ikPoseIds)
		{
			_service?.RemoveOwnedPoses(ikPoseIds);
		}
	}
}
