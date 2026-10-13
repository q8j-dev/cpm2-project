using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class PlayerStorageHashes : HashesData<PlayerStorage>
	{
		public override void CalculateChangedValues(PlayerStorage data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<PlayerStorage>.Data;
			data2.Clear();
			CacheData(data.LocalID, "LocalID", data2, alwaysSend: true);
			CacheData(data.Name, "Name", data2, fullData);
			CacheData(data.Wheels, "Wheels", data2, fullData);
			CacheData(data.MatTypes, "MatTypes", data2, fullData);
			CacheData(data.Calipers, "Calipers", data2, fullData);
			CacheData(data.Brakes, "Brakes", data2, fullData);
			CacheData(data.Gender, "Gender", data2, fullData);
			if (data.RacingProgress != null)
			{
				CacheData(data.RacingProgress.RaceCount, "RacingProgress.RaceCount", data2, fullData);
				CacheData(data.RacingProgress.RaceWin, "RacingProgress.RaceWin", data2, fullData);
				CacheData(data.RacingProgress.BestResult, "RacingProgress.BestResult", data2, fullData);
				CacheData(data.RacingProgress.RacingClassRecords, "RacingProgress.RacingClassRecords", data2, fullData);
				CacheData(data.RacingProgress.ManualRacingClassRecords, "RacingProgress.ManualRacingClassRecords", data2, fullData);
			}
			if (data.CircuitRacingProgress != null)
			{
				CacheData(data.CircuitRacingProgress.RaceCount, "CircuitRacingProgress.RaceCount", data2, fullData);
				CacheData(data.CircuitRacingProgress.RaceWin, "CircuitRacingProgress.RaceWin", data2, fullData);
				CacheData(data.CircuitRacingProgress.MoneyEarned, "CircuitRacingProgress.MoneyEarned", data2, fullData);
				CacheData(data.CircuitRacingProgress.LastMoneyEarnedDay, "CircuitRacingProgress.LastMoneyEarnedDay", data2, fullData);
				CacheData(data.CircuitRacingProgress.RacingTrackRecords, "CircuitRacingProgress.RacingTrackRecords", data2, fullData);
			}
			if (data.VisualTuningFavouriteData != null)
			{
				CacheData(data.VisualTuningFavouriteData.FavouriteVinyls, "VisualTuningFavouriteData.FavouriteVinyls", data2, fullData);
				CacheData(data.VisualTuningFavouriteData.FavouriteWheels, "VisualTuningFavouriteData.FavouriteWheels", data2, fullData);
			}
			if (data.EquipmentsMale != null)
			{
				CacheData(data.EquipmentsMale.bag, "EquipmentsMale.bag", data2, fullData);
				CacheData(data.EquipmentsMale.beard, "EquipmentsMale.beard", data2, fullData);
				CacheData(data.EquipmentsMale.cap, "EquipmentsMale.cap", data2, fullData);
				CacheData(data.EquipmentsMale.face, "EquipmentsMale.face", data2, fullData);
				CacheData(data.EquipmentsMale.glasses, "EquipmentsMale.glasses", data2, fullData);
				CacheData(data.EquipmentsMale.gloves, "EquipmentsMale.gloves", data2, fullData);
				CacheData(data.EquipmentsMale.hair, "EquipmentsMale.hair", data2, fullData);
				CacheData(data.EquipmentsMale.mask, "EquipmentsMale.mask", data2, fullData);
				CacheData(data.EquipmentsMale.pants, "EquipmentsMale.pants", data2, fullData);
				CacheData(data.EquipmentsMale.shoes, "EquipmentsMale.shoes", data2, fullData);
				CacheData(data.EquipmentsMale.top, "EquipmentsMale.top", data2, fullData);
				CacheData(data.EquipmentsMale.SelectedEquipments, "EquipmentsMale.SelectedEquipments", data2, fullData);
			}
			if (data.EquipmentsFemale != null)
			{
				CacheData(data.EquipmentsFemale.bag, "EquipmentsFemale.bag", data2, fullData);
				CacheData(data.EquipmentsFemale.beard, "EquipmentsFemale.beard", data2, fullData);
				CacheData(data.EquipmentsFemale.cap, "EquipmentsFemale.cap", data2, fullData);
				CacheData(data.EquipmentsFemale.face, "EquipmentsFemale.face", data2, fullData);
				CacheData(data.EquipmentsFemale.glasses, "EquipmentsFemale.glasses", data2, fullData);
				CacheData(data.EquipmentsFemale.gloves, "EquipmentsFemale.gloves", data2, fullData);
				CacheData(data.EquipmentsFemale.hair, "EquipmentsFemale.hair", data2, fullData);
				CacheData(data.EquipmentsFemale.mask, "EquipmentsFemale.mask", data2, fullData);
				CacheData(data.EquipmentsFemale.pants, "EquipmentsFemale.pants", data2, fullData);
				CacheData(data.EquipmentsFemale.shoes, "EquipmentsFemale.shoes", data2, fullData);
				CacheData(data.EquipmentsFemale.top, "EquipmentsFemale.top", data2, fullData);
				CacheData(data.EquipmentsFemale.SelectedEquipments, "EquipmentsFemale.SelectedEquipments", data2, fullData);
			}
			if (data.Other != null)
			{
				CacheData(data.Other.Flags, "Other.Flags", data2, fullData);
				CacheData(data.Other.Animations, "Other.Animations", data2, fullData);
				CacheData(data.Other.SelectedAnimations, "Other.SelectedAnimations", data2, fullData);
				CacheData(data.Other.Eggs, "Other.Eggs", data2, fullData);
				CacheData(data.Other.LevelsDoneTime, "Other.LevelsDoneTime", data2, fullData);
				CacheData(data.Other.Homes, "Other.Homes", data2, fullData);
				CacheData(data.Other.Main, "Other.Main", data2, fullData);
				CacheData(data.Other.Purchased, "Other.Purchased", data2, fullData);
				CacheData(data.Other.Camera, "Other.Camera", data2, fullData);
				CacheData(data.Other.PoliceBodyKits, "Other.PoliceBodyKits", data2, fullData);
				CacheData(data.Other.PoliceSounds, "Other.PoliceSounds", data2, fullData);
			}
			if (data2.Count == 1 && data2.ContainsKey("LocalID"))
			{
				data2.Clear();
			}
			changedData = data2;
		}
	}
}
