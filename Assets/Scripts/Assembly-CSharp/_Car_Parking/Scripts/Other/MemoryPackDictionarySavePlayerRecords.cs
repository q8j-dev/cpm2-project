using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace _Car_Parking.Scripts.Other
{
	public class MemoryPackDictionarySavePlayerRecords : MemoryPackDictionaryBase
	{
		private const string LocalID = "LocalID";

		private const string Name = "Name";

		private const string Wheels = "Wheels";

		private const string MatTypes = "MatTypes";

		private const string Calipers = "Calipers";

		private const string Brakes = "Brakes";

		private const string Gender = "Gender";

		private const string RacingProgressRaceCount = "RacingProgress.RaceCount";

		private const string RacingProgressRaceWin = "RacingProgress.RaceWin";

		private const string RacingProgressBestResult = "RacingProgress.BestResult";

		private const string RacingProgressRacingClassRecords = "RacingProgress.RacingClassRecords";

		private const string RacingProgressManualRacingClassRecords = "RacingProgress.ManualRacingClassRecords";

		private const string CircuitRacingProgressRaceCount = "CircuitRacingProgress.RaceCount";

		private const string CircuitRacingProgressRaceWin = "CircuitRacingProgress.RaceWin";

		private const string CircuitRacingProgressMoneyEarned = "CircuitRacingProgress.MoneyEarned";

		private const string CircuitRacingProgressLastMoneyEarnedDay = "CircuitRacingProgress.LastMoneyEarnedDay";

		private const string CircuitRacingProgressRacingTrackRecords = "CircuitRacingProgress.RacingTrackRecords";

		private const string VisualTuningFavouriteDataFavouriteVinyls = "VisualTuningFavouriteData.FavouriteVinyls";

		private const string VisualTuningFavouriteDataFavouriteWheels = "VisualTuningFavouriteData.FavouriteWheels";

		private const string EquipmentsMaleBag = "EquipmentsMale.bag";

		private const string EquipmentsMaleBeard = "EquipmentsMale.beard";

		private const string EquipmentsMaleCap = "EquipmentsMale.cap";

		private const string EquipmentsMaleFace = "EquipmentsMale.face";

		private const string EquipmentsMaleGlasses = "EquipmentsMale.glasses";

		private const string EquipmentsMaleGloves = "EquipmentsMale.gloves";

		private const string EquipmentsMaleHair = "EquipmentsMale.hair";

		private const string EquipmentsMaleMask = "EquipmentsMale.mask";

		private const string EquipmentsMalePants = "EquipmentsMale.pants";

		private const string EquipmentsMaleShoes = "EquipmentsMale.shoes";

		private const string EquipmentsMaleTop = "EquipmentsMale.top";

		private const string EquipmentsMaleSelectedEquipments = "EquipmentsMale.SelectedEquipments";

		private const string EquipmentsFemaleBag = "EquipmentsFemale.bag";

		private const string EquipmentsFemaleBeard = "EquipmentsFemale.beard";

		private const string EquipmentsFemaleCap = "EquipmentsFemale.cap";

		private const string EquipmentsFemaleFace = "EquipmentsFemale.face";

		private const string EquipmentsFemaleGlasses = "EquipmentsFemale.glasses";

		private const string EquipmentsFemaleGloves = "EquipmentsFemale.gloves";

		private const string EquipmentsFemaleHair = "EquipmentsFemale.hair";

		private const string EquipmentsFemaleMask = "EquipmentsFemale.mask";

		private const string EquipmentsFemalePants = "EquipmentsFemale.pants";

		private const string EquipmentsFemaleShoes = "EquipmentsFemale.shoes";

		private const string EquipmentsFemaleTop = "EquipmentsFemale.top";

		private const string EquipmentsFemaleSelectedEquipments = "EquipmentsFemale.SelectedEquipments";

		private const string OtherFlags = "Other.Flags";

		private const string OtherAnimations = "Other.Animations";

		private const string OtherSelectedAnimations = "Other.SelectedAnimations";

		private const string OtherEggs = "Other.Eggs";

		private const string OtherLevelsDoneTime = "Other.LevelsDoneTime";

		private const string OtherHomes = "Other.Homes";

		private const string OtherMain = "Other.Main";

		private const string OtherPurchased = "Other.Purchased";

		private const string OtherCamera = "Other.Camera";

		private const string OtherPoliceBodyKits = "Other.PoliceBodyKits";

		private const string OtherPoliceSounds = "Other.PoliceSounds";

		protected override Dictionary<string, ushort> GetKeyIndexMap()
		{
			return new Dictionary<string, ushort>
			{
				["LocalID"] = 0,
				["Name"] = 1,
				["Wheels"] = 2,
				["MatTypes"] = 3,
				["Calipers"] = 4,
				["Brakes"] = 5,
				["Gender"] = 6,
				["RacingProgress.RaceCount"] = 7,
				["RacingProgress.RaceWin"] = 8,
				["RacingProgress.BestResult"] = 9,
				["RacingProgress.RacingClassRecords"] = 10,
				["RacingProgress.ManualRacingClassRecords"] = 11,
				["CircuitRacingProgress.RaceCount"] = 12,
				["CircuitRacingProgress.RaceWin"] = 13,
				["CircuitRacingProgress.MoneyEarned"] = 14,
				["CircuitRacingProgress.LastMoneyEarnedDay"] = 15,
				["CircuitRacingProgress.RacingTrackRecords"] = 16,
				["VisualTuningFavouriteData.FavouriteVinyls"] = 17,
				["VisualTuningFavouriteData.FavouriteWheels"] = 18,
				["EquipmentsMale.bag"] = 19,
				["EquipmentsMale.beard"] = 20,
				["EquipmentsMale.cap"] = 21,
				["EquipmentsMale.face"] = 22,
				["EquipmentsMale.glasses"] = 23,
				["EquipmentsMale.gloves"] = 24,
				["EquipmentsMale.hair"] = 25,
				["EquipmentsMale.mask"] = 26,
				["EquipmentsMale.pants"] = 27,
				["EquipmentsMale.shoes"] = 28,
				["EquipmentsMale.top"] = 29,
				["EquipmentsMale.SelectedEquipments"] = 30,
				["EquipmentsFemale.bag"] = 31,
				["EquipmentsFemale.beard"] = 32,
				["EquipmentsFemale.cap"] = 33,
				["EquipmentsFemale.face"] = 34,
				["EquipmentsFemale.glasses"] = 35,
				["EquipmentsFemale.gloves"] = 36,
				["EquipmentsFemale.hair"] = 37,
				["EquipmentsFemale.mask"] = 38,
				["EquipmentsFemale.pants"] = 39,
				["EquipmentsFemale.shoes"] = 40,
				["EquipmentsFemale.top"] = 41,
				["EquipmentsFemale.SelectedEquipments"] = 42,
				["Other.Flags"] = 43,
				["Other.Animations"] = 44,
				["Other.SelectedAnimations"] = 45,
				["Other.Eggs"] = 46,
				["Other.LevelsDoneTime"] = 47,
				["Other.Homes"] = 48,
				["Other.Main"] = 49,
				["Other.Purchased"] = 50,
				["Other.Camera"] = 51,
				["Other.PoliceBodyKits"] = 52,
				["Other.PoliceSounds"] = 53
			};
		}

		protected override Dictionary<string, Type> GetTypesMap()
		{
			return new Dictionary<string, Type>
			{
				["LocalID"] = typeof(string),
				["Name"] = typeof(string),
				["Wheels"] = typeof(List<int>),
				["MatTypes"] = typeof(int),
				["Calipers"] = typeof(long),
				["Brakes"] = typeof(long),
				["Gender"] = typeof(int),
				["RacingProgress.RaceCount"] = typeof(int),
				["RacingProgress.RaceWin"] = typeof(int),
				["RacingProgress.BestResult"] = typeof(OneRaceResult),
				["RacingProgress.RacingClassRecords"] = typeof(List<RacingClassRecord>),
				["RacingProgress.ManualRacingClassRecords"] = typeof(List<RacingClassRecord>),
				["CircuitRacingProgress.RaceCount"] = typeof(int),
				["CircuitRacingProgress.RaceWin"] = typeof(int),
				["CircuitRacingProgress.MoneyEarned"] = typeof(int),
				["CircuitRacingProgress.LastMoneyEarnedDay"] = typeof(string),
				["CircuitRacingProgress.RacingTrackRecords"] = typeof(Dictionary<int, List<CircuitRacingRecord>>),
				["VisualTuningFavouriteData.FavouriteVinyls"] = typeof(List<int>),
				["VisualTuningFavouriteData.FavouriteWheels"] = typeof(List<int>),
				["EquipmentsMale.bag"] = typeof(List<int>),
				["EquipmentsMale.beard"] = typeof(List<int>),
				["EquipmentsMale.cap"] = typeof(List<int>),
				["EquipmentsMale.face"] = typeof(List<int>),
				["EquipmentsMale.glasses"] = typeof(List<int>),
				["EquipmentsMale.gloves"] = typeof(List<int>),
				["EquipmentsMale.hair"] = typeof(List<int>),
				["EquipmentsMale.mask"] = typeof(List<int>),
				["EquipmentsMale.pants"] = typeof(List<int>),
				["EquipmentsMale.shoes"] = typeof(List<int>),
				["EquipmentsMale.top"] = typeof(List<int>),
				["EquipmentsMale.SelectedEquipments"] = typeof(List<int>),
				["EquipmentsFemale.bag"] = typeof(List<int>),
				["EquipmentsFemale.beard"] = typeof(List<int>),
				["EquipmentsFemale.cap"] = typeof(List<int>),
				["EquipmentsFemale.face"] = typeof(List<int>),
				["EquipmentsFemale.glasses"] = typeof(List<int>),
				["EquipmentsFemale.gloves"] = typeof(List<int>),
				["EquipmentsFemale.hair"] = typeof(List<int>),
				["EquipmentsFemale.mask"] = typeof(List<int>),
				["EquipmentsFemale.pants"] = typeof(List<int>),
				["EquipmentsFemale.shoes"] = typeof(List<int>),
				["EquipmentsFemale.top"] = typeof(List<int>),
				["EquipmentsFemale.SelectedEquipments"] = typeof(List<int>),
				["Other.Flags"] = typeof(Dictionary<int, int>),
				["Other.Animations"] = typeof(long),
				["Other.SelectedAnimations"] = typeof(List<int>),
				["Other.Eggs"] = typeof(List<long>),
				["Other.LevelsDoneTime"] = typeof(List<float>),
				["Other.Homes"] = typeof(long),
				["Other.Main"] = typeof(float[]),
				["Other.Purchased"] = typeof(int[]),
				["Other.Camera"] = typeof(float[]),
				["Other.PoliceBodyKits"] = typeof(Dictionary<int, string>),
				["Other.PoliceSounds"] = typeof(List<int>)
			};
		}
	}
}
