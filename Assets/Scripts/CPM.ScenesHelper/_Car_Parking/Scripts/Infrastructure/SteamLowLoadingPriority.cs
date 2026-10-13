using UnityEngine;

namespace _Car_Parking.Scripts.Infrastructure
{
	public static class SteamLowLoadingPriority
	{
		private const ThreadPriority Baseline = ThreadPriority.BelowNormal;

		private static int _count;

		public static void Acquire()
		{
			_count++;
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}

		public static void Release()
		{
			if (_count != 0 && --_count == 0 && Application.backgroundLoadingPriority == ThreadPriority.Low)
			{
				Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
			}
		}

		public static void HealOrphaned()
		{
			if (_count == 0 && Application.backgroundLoadingPriority == ThreadPriority.Low)
			{
				Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
			}
		}
	}
}
