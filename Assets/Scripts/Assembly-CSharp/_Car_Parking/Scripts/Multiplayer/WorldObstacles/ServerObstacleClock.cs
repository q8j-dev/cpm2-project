using System;
using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.WorldObstacles
{
	public static class ServerObstacleClock
	{
		private static double _elapsed;

		private static double _localSampledAt;

		private static bool _sampled;

		public static double Elapsed()
		{
			double timeAsDouble = Time.timeAsDouble;
			double time = PhotonNetwork.Time;
			if (!_sampled)
			{
				_localSampledAt = timeAsDouble;
				_sampled = true;
			}
			if (time > 0.0)
			{
				_elapsed = time;
			}
			else
			{
				_elapsed += Math.Max(0.0, timeAsDouble - _localSampledAt);
			}
			_localSampledAt = timeAsDouble;
			return _elapsed;
		}
	}
}
