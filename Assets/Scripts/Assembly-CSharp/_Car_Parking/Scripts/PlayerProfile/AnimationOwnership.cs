using System.Collections.Generic;

namespace _Car_Parking.Scripts.PlayerProfile
{
	public static class AnimationOwnership
	{
		private static IPlayerProfileService _service;

		public static void Initialize(IPlayerProfileService service)
		{
			_service = service;
		}

		public static bool Owns(int y)
		{
			if (_service != null)
			{
				return _service.OwnsAnimation(y);
			}
			return false;
		}

		public static void Add(IReadOnlyCollection<int> ys)
		{
			_service?.AddOwnedAnimations(ys);
		}

		public static void Remove(IReadOnlyCollection<int> ys)
		{
			_service?.RemoveOwnedAnimations(ys);
		}
	}
}
