using System;
using System.Collections.Generic;
using System.Linq;
using Persistence.Systems;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[CreateAssetMenu(menuName = "Rally/RallyTrajectoryWeatherDataSO")]
	public class RallyTrajectoryWeatherDataSO : ScriptableObject
	{
		private const string SaveFileKey = "RallyWeatherData";

		private const string WeatherListKey = "WeatherDataList";

		public RallyWeatherData[] RallyWeatherDataList;

		private bool _loaded;

		public void Init()
		{
			EnsureLoaded();
		}

		public RallyWeatherData GetWeatherData(int locationId, int trajectoryId)
		{
			EnsureLoaded();
			RallyWeatherData rallyWeatherData = null;
			if (RallyWeatherDataList != null)
			{
				rallyWeatherData = RallyWeatherDataList.FirstOrDefault((RallyWeatherData x) => x != null && x.LocationId == locationId && x.TrajectoryId == trajectoryId);
				if (rallyWeatherData == null)
				{
					rallyWeatherData = RallyWeatherDataList.FirstOrDefault((RallyWeatherData x) => x != null);
				}
			}
			if (rallyWeatherData == null)
			{
				rallyWeatherData = new RallyWeatherData
				{
					LocationId = locationId,
					TrajectoryId = trajectoryId,
					WeatherType = WeatherType.Clear,
					DayTimeType = DayTimeType.Day
				};
			}
			return rallyWeatherData;
		}

		public void ShuffleData()
		{
			if (RallyWeatherDataList == null)
			{
				return;
			}
			List<WeatherType> list = new List<WeatherType>
			{
				WeatherType.Clear,
				WeatherType.Rain
			};
			List<WeatherType> list2 = new List<WeatherType>
			{
				WeatherType.Clear,
				WeatherType.Snow
			};
			RallyWeatherData[] rallyWeatherDataList = RallyWeatherDataList;
			foreach (RallyWeatherData rallyWeatherData in rallyWeatherDataList)
			{
				if (rallyWeatherData != null)
				{
					List<WeatherType> list3 = ((rallyWeatherData.PlaceToSpawn == V.placeToSpawn.Island) ? list2 : list);
					rallyWeatherData.WeatherType = list3[UnityEngine.Random.Range(0, list3.Count)];
				}
			}
			SaveToStorage();
		}

		private void SaveToStorage()
		{
			EasySaveDataHandler.SaveValues(new Dictionary<string, object> { { "WeatherDataList", RallyWeatherDataList } }, "RallyWeatherData");
		}

		private void EnsureLoaded()
		{
			if (!_loaded)
			{
				_loaded = true;
				LoadFromStorage();
			}
		}

		private void LoadFromStorage()
		{
			if (RallyWeatherDataList == null || !EasySaveDataHandler.TryGetFile("RallyWeatherData", out var file))
			{
				return;
			}
			RallyWeatherData[] value = EasySaveDataHandler.GetValue<RallyWeatherData[]>(file, "WeatherDataList");
			if (value == null)
			{
				return;
			}
			RallyWeatherData[] array = value;
			foreach (RallyWeatherData s in array)
			{
				if (s != null)
				{
					RallyWeatherData rallyWeatherData = Array.Find(RallyWeatherDataList, (RallyWeatherData x) => x != null && x.LocationId == s.LocationId && x.TrajectoryId == s.TrajectoryId);
					if (rallyWeatherData != null)
					{
						rallyWeatherData.WeatherType = s.WeatherType;
					}
				}
			}
		}
	}
}
