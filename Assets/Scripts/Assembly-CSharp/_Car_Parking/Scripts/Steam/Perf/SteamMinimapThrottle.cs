using UnityEngine;

namespace _Car_Parking.Scripts.Steam.Perf
{
	public static class SteamMinimapThrottle
	{
		private const int RenderIntervalMs = 33;

		private const int IconRefreshFrames = 10;

		private static float _lastOrtSize = -1f;

		public static int RenderInterval(int defaultMs)
		{
			if (!SteamPerfFlags.IsOn("minimap"))
			{
				return defaultMs;
			}
			return 33;
		}

		public static bool ShouldZoomIcons(float ortSize)
		{
			if (!SteamPerfFlags.IsOn("minimap"))
			{
				return true;
			}
			if (Mathf.Approximately(ortSize, _lastOrtSize) && Time.frameCount % 10 != 0)
			{
				return false;
			}
			_lastOrtSize = ortSize;
			return true;
		}
	}
}
