using System;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Serializable]
	public class RallyWeatherData
	{
		public int LocationId;

		public int TrajectoryId;

		public V.placeToSpawn PlaceToSpawn;

		public WeatherType WeatherType;

		public DayTimeType DayTimeType;
	}
}
