using System;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Serializable]
	public class GeoData
	{
		public string ip;

		public string country;

		public string countryCode;

		public long serverUnixTime;

		public long lastUpdateTime;
	}
}
