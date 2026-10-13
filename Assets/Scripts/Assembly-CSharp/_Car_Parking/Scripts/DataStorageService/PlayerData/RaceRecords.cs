using System;
using System.Collections.Generic;
using CPMEngine;

namespace _Car_Parking.Scripts.DataStorageService.PlayerData
{
	public class RaceRecords
	{
		public static Dictionary<int, List<CircuitRacingRecord>> GetTrackRecords(IPlayerStorageService playerStorageService, int mapCount)
		{
			PlayerStorage data = playerStorageService.Data;
			if (data.CircuitRacingProgress == null)
			{
				data.CircuitRacingProgress = new CircuitRacingProgress();
			}
			Dictionary<int, List<CircuitRacingRecord>> dictionary = playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords;
			if (dictionary == null || dictionary.Count == 0)
			{
				dictionary = new Dictionary<int, List<CircuitRacingRecord>>();
				for (int i = 0; i < mapCount; i++)
				{
					dictionary.Add(i, GetRaceRecords());
				}
				playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords = dictionary;
			}
			if (dictionary.Count < mapCount)
			{
				for (int j = dictionary.Count - 1; j < mapCount; j++)
				{
					dictionary.Add(j, GetRaceRecords());
				}
				playerStorageService.Data.CircuitRacingProgress.RacingTrackRecords = dictionary;
			}
			return dictionary;
		}

		private static List<CircuitRacingRecord> GetRaceRecords()
		{
			List<CircuitRacingRecord> list = new List<CircuitRacingRecord>();
			foreach (VehicleClass value in Enum.GetValues(typeof(VehicleClass)))
			{
				CircuitRacingRecord item = new CircuitRacingRecord
				{
					ClassIndex = (int)value,
					FirstPlace = 0,
					SecondPlace = 0,
					ThirdPlace = 0,
					BestTime = 0f
				};
				list.Add(item);
			}
			return list;
		}
	}
}
