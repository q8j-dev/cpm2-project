using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	public class ServerTimeService
	{
		private long _serverUnixTimeSec = -1L;

		private double _localTimeWhenSynced;

		public long CurrentServerUnixTimeSec
		{
			get
			{
				double num = Time.realtimeSinceStartupAsDouble - _localTimeWhenSynced;
				if (_serverUnixTimeSec == -1)
				{
					return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				}
				return _serverUnixTimeSec + (long)num;
			}
		}

		public long CurrentServerUnixTimeMs
		{
			get
			{
				if (_serverUnixTimeSec == -1)
				{
					return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
				}
				double num = (Time.realtimeSinceStartupAsDouble - _localTimeWhenSynced) * 1000.0;
				return _serverUnixTimeSec * 1000 + (long)num;
			}
		}

		public void SyncTimeFromServer(long serverUnixTime)
		{
			_serverUnixTimeSec = serverUnixTime;
			_localTimeWhenSynced = Time.realtimeSinceStartupAsDouble;
		}
	}
}
