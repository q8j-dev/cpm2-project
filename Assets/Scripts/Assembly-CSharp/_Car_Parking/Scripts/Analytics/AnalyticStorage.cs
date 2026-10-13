using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Analytics
{
	public static class AnalyticStorage
	{
		public static string LastSessionId;

		public static int TallRewardInSession;

		private static float _sessionStartTime;

		private static float _totalPausedTime;

		private static float _pauseStartTime;

		private static bool _isPaused;

		public static void StartSession()
		{
			LastSessionId = Guid.NewGuid().ToString();
			_sessionStartTime = Time.realtimeSinceStartup;
			_totalPausedTime = 0f;
			_isPaused = false;
			TallRewardInSession = 0;
		}

		public static void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				_pauseStartTime = Time.realtimeSinceStartup;
				_isPaused = true;
			}
			else if (_isPaused)
			{
				_totalPausedTime += Time.realtimeSinceStartup - _pauseStartTime;
				_isPaused = false;
			}
		}

		public static long GetSessionDurationMs()
		{
			float val = Time.realtimeSinceStartup - _sessionStartTime - _totalPausedTime;
			return (long)(Math.Max(0f, val) * 1000f);
		}
	}
}
