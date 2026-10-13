using UnityEngine;

namespace _Car_Parking.Scripts.Infrastructure
{
	public static class LoadingPriorityBoost
	{
		private static int _count;

		private static ThreadPriority _previous;

		public static void Acquire()
		{
			if (_count++ == 0)
			{
				_previous = Application.backgroundLoadingPriority;
				Application.backgroundLoadingPriority = ThreadPriority.High;
			}
		}

		public static void Release()
		{
			if (_count > 0 && --_count == 0)
			{
				Application.backgroundLoadingPriority = _previous;
			}
		}
	}
}
