using System;
using CPM.Ads.Policy;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Scopes
{
	public sealed class ServerTimeRawUtc : IRawUtcTime
	{
		private readonly ServerTimeService _serverTime;

		public DateTime UtcNow => DateTimeOffset.FromUnixTimeSeconds(_serverTime.CurrentServerUnixTimeSec).UtcDateTime;

		public ServerTimeRawUtc(ServerTimeService serverTime)
		{
			_serverTime = serverTime;
		}
	}
}
