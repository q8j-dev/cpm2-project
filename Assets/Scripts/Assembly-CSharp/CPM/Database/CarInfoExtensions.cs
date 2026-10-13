using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CPM.Scripts.VinylDrawer;
using DataEntities.Vinyl;
using Persistence.Systems;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace CPM.Database
{
	public static class CarInfoExtensions
	{
		private static Regex _regex;

		public static event Action<CarInfo> OnCarInfoUpdated;

		public static event Action<BoughtBodyKits, BodyKitType, int> OnBoughtBodyKitsUpdated;

		public static void InvokeCarInfoUpdated(this CarInfo carInfo)
		{
			OnCarInfoUpdated?.Invoke(carInfo);
		}

		public static void SaveMillage<T>(PartData partData, PartInstance<T> partInstance) where T : PartItem
		{
			if (partData != null && partInstance != null)
			{
				partData.Wear = partInstance.Wear;
			}
		}

		public static void CheckForNulls(this CarInfo carInfo, VehicleStockTuningData config)
		{
			if (!config)
			{
				return;
			}
			CarTuningData carInfoTuningData = null;
			if (carInfo.TuningData == null)
			{
				carInfo.TuningData = StockTuning();
				Debug.LogWarning("[NullData] TuningData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.EngineKitData == null)
			{
				carInfo.TuningData.EngineKitData = StockTuning().EngineKitData;
				Debug.LogWarning("[NullData] EngineKitData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.FrontWheelData == null)
			{
				carInfo.TuningData.FrontWheelData = StockTuning().FrontWheelData;
				Debug.LogWarning("[NullData] FrontWheelData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.FrontWheelData.BrakesData == null)
			{
				carInfo.TuningData.FrontWheelData.BrakesData = StockTuning().FrontWheelData.BrakesData.Clone() as BrakesData;
				Debug.LogWarning("[NullData] FrontWheelData BrakesData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.FrontWheelData.SuspensionData == null)
			{
				carInfo.TuningData.FrontWheelData.SuspensionData = StockTuning().FrontWheelData.SuspensionData.Clone() as SteerSuspensionData;
				Debug.LogWarning("[NullData] FrontWheelData SuspensionData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.RearWheelData == null)
			{
				carInfo.TuningData.RearWheelData = StockTuning().RearWheelData;
				Debug.LogWarning("[NullData] RearWheelData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.RearWheelData.BrakesData == null)
			{
				carInfo.TuningData.RearWheelData.BrakesData = StockTuning().RearWheelData.BrakesData.Clone() as BrakesData;
				Debug.LogWarning("[NullData] RearWheelData BrakesData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.RearWheelData.SuspensionData == null)
			{
				carInfo.TuningData.RearWheelData.SuspensionData = StockTuning().RearWheelData.SuspensionData.Clone() as SuspensionData;
				Debug.LogWarning("[NullData] RearWheelData SuspensionData was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.Maintenance.FrontTyreHealth.Length == 0)
			{
				carInfo.TuningData.Maintenance = StockTuning().Maintenance;
				Debug.LogWarning("[NullData] Maintenance was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.DynamicTuningSettings == null)
			{
				carInfo.TuningData.DynamicTuningSettings = StockTuning().DynamicTuningSettings;
				Debug.LogWarning("[NullData] DynamicTuningValues was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.ECU == null)
			{
				PartData partData = new PartData();
				carInfo.TuningData.ECU = partData.CreateCompatibleECUFromEngine(carInfo.TuningData.EngineKitData.EngineId.ModelId, carInfo.CarID);
				Debug.LogWarning("[NullData] ECU was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.GearBox == null)
			{
				carInfo.TuningData.GearBox = StockTuning().GearBox;
				Debug.LogWarning("[NullData] GearBox was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.FrontLSD == null)
			{
				carInfo.TuningData.FrontLSD = StockTuning().FrontLSD;
				Debug.LogWarning("[NullData] FrontLSD was null. car:" + carInfo.CarID);
			}
			if (carInfo.TuningData.RearLSD == null)
			{
				carInfo.TuningData.RearLSD = StockTuning().RearLSD;
				Debug.LogWarning("[NullData] RearLSD was null. car:" + carInfo.CarID);
			}
			if (carInfo.BoughtBodyKits == null)
			{
				carInfo.BoughtBodyKits = new BoughtBodyKits();
				Debug.LogWarning("[NullData] BoughtBodyKits was null car. car:" + carInfo.CarID);
			}
			BoughtBodyKits boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.SpoilerIds == null)
			{
				boughtBodyKits.SpoilerIds = new List<int>();
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.FrontBumperIds == null)
			{
				boughtBodyKits.FrontBumperIds = new List<int>();
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.RearBumperIds == null)
			{
				boughtBodyKits.RearBumperIds = new List<int>();
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.SkirtIds == null)
			{
				boughtBodyKits.SkirtIds = new List<int>();
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.HoodIds == null)
			{
				boughtBodyKits.HoodIds = new List<int>();
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.HoodAirIntakeIds == null)
			{
				boughtBodyKits.HoodAirIntakeIds = new List<int>();
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.RoofAirIntakeIds == null)
			{
				boughtBodyKits.RoofAirIntakeIds = new List<int>();
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.FenderIds == null)
			{
				boughtBodyKits.FenderIds = new List<int>();
			}
			if (carInfo.BoughtBodyKits.TrimIds == null)
			{
				carInfo.BoughtBodyKits.TrimIds = new List<int>();
				if (config.CarInfoDefault != null && config.CarInfoDefault.InstalledBodyKits != null && config.CarInfoDefault.InstalledBodyKits.Trims != null)
				{
					carInfo.BoughtBodyKits.TrimIds = new List<int>(config.CarInfoDefault.InstalledBodyKits.Trims);
				}
			}
			boughtBodyKits = carInfo.BoughtBodyKits;
			if (boughtBodyKits.Interiors == null)
			{
				boughtBodyKits.Interiors = new List<int>();
			}
			if (carInfo.InstalledBodyKits == null)
			{
				carInfo.InstalledBodyKits = new InstalledBodyKits();
				Debug.LogWarning("[NullData] InstalledBodyKits was null. car:" + carInfo.CarID);
			}
			if (carInfo.InstalledBodyKits.Trims == null)
			{
				carInfo.InstalledBodyKits.Trims = Array.Empty<int>();
			}
			if (carInfo.CarID == 49 && carInfo.InstalledBodyKits.Trims.Length == 0)
			{
				carInfo.InstalledBodyKits.Trims = new int[1] { 15 };
			}
			if (carInfo.Colors == null)
			{
				carInfo.Colors = new Colors();
				Debug.LogWarning("[NullData] Colors was null. car:" + carInfo.CarID);
			}
			if (carInfo.AdditionalData == null)
			{
				carInfo.AdditionalData = new AdditionalData();
				Debug.LogWarning("[NullData] AdditionalData was null. car:" + carInfo.CarID);
			}
			if (carInfo.AdditionalData.Police == null)
			{
				carInfo.AdditionalData.Police = new PoliceData();
				Debug.LogWarning("[NullData] AdditionalData.Police was null. car:" + carInfo.CarID);
			}
			carInfo.AdditionalData.Police.CheckForNulls();
			if (carInfo.AdditionalData.AirSuspension == null)
			{
				carInfo.AdditionalData.AirSuspension = new PsData();
				Debug.LogWarning("[NullData] AdditionalData.AirSuspension was null. car:" + carInfo.CarID);
			}
			CarTuningData StockTuning()
			{
				return carInfoTuningData ?? (carInfoTuningData = config.Tuning.GetTuningData());
			}
		}

		public static void SaveCarMillage(this CarInfo carInfo, CarEntity carEntity, bool useFullSave = true)
		{
			SaveMillage(carInfo.TuningData.EngineKitData.EngineId, carEntity.Engine);
			if (carInfo.TuningData.EngineKitData.RotorHousing != null)
			{
				SaveMillage(carInfo.TuningData.EngineKitData.RotorHousing, carEntity.Engine.RotorHousing);
				SaveMillage(carInfo.TuningData.EngineKitData.Rotors, carEntity.Engine.RotorHousing.Rotors);
				SaveMillage(carInfo.TuningData.EngineKitData.RotorTrackingGear, carEntity.Engine.RotorHousing.RotorTrackingGears);
				SaveMillage(carInfo.TuningData.EngineKitData.EccentricShaft, carEntity.Engine.RotorHousing.EccentricShaft);
				SaveMillage(carInfo.TuningData.EngineKitData.Flywheel, carEntity.Engine.RotorHousing.Flywheel);
			}
			else
			{
				SaveMillage(carInfo.TuningData.EngineKitData.Block, carEntity.Engine.Block);
				SaveMillage(carInfo.TuningData.EngineKitData.Crankshaft, carEntity.Engine.Block.Crankshaft);
				SaveMillage(carInfo.TuningData.EngineKitData.ConnectionRod, carEntity.Engine.Block.ConnectionRod);
				SaveMillage(carInfo.TuningData.EngineKitData.Pistons, carEntity.Engine.Block.Pistons);
				SaveMillage(carInfo.TuningData.EngineKitData.Flywheel, carEntity.Engine.Block.Flywheel);
			}
			SaveMillage(carInfo.TuningData.EngineKitData.IntakeManifold, carEntity.Engine.IntakeManifold);
			SaveMillage(carInfo.TuningData.EngineKitData.ThrottleValve, carEntity.Engine.ThrottleValve);
			SaveMillage(carInfo.TuningData.EngineKitData.AirFilter, carEntity.Engine.IntakeManifold.AirFilter);
			SaveMillage(carInfo.TuningData.EngineKitData.Charger, carEntity.Engine.Charger);
			SaveMillage(carInfo.TuningData.EngineKitData.FuelPump, carEntity.Engine.FuelPump);
			SaveMillage(carInfo.TuningData.EngineKitData.FuelRail, carEntity.Engine.FuelRail);
			SaveMillage(carInfo.TuningData.EngineKitData.FuelInjectors, carEntity.Engine.FuelInjectors);
			SaveMillage(carInfo.TuningData.EngineKitData.ExhaustHeader, carEntity.Engine.ExhaustHeader);
			SaveMillage(carInfo.TuningData.EngineKitData.ExhaustPipe, carEntity.Engine.ExhaustPipe);
			SaveMillage(carInfo.TuningData.EngineKitData.Radiator, carEntity.Engine.Radiator);
			SaveMillage(carInfo.TuningData.EngineKitData.Intercooler, carEntity.Engine.Intercooler);
			SaveMillage(carInfo.TuningData.ECU, carEntity.Engine.ECU);
			SaveMillage(carInfo.TuningData.GearBox, carEntity.GearBox);
			SaveMillage(carInfo.TuningData.FrontLSD, carEntity.FrontLsd);
			SaveMillage(carInfo.TuningData.RearLSD, carEntity.RearLsd);
			SaveMillage(carInfo.TuningData.EngineKitData.Clutch, carEntity.Engine.Clutch);
			if (carInfo.TuningData.EngineKitData.RotorHousing != null)
			{
				SaveMillage(carInfo.TuningData.EngineKitData.CylinderHead, carEntity.Engine.CylinderHead);
				CylinderHeadInstance cylinderHead = carEntity.Engine.CylinderHead;
				if (!(cylinderHead is DohcCylinderHeadInstance dohcCylinderHeadInstance))
				{
					if (!(cylinderHead is SohcCylinderHeadInstance sohcCylinderHeadInstance))
					{
						if (cylinderHead is OhvCylinderHeadInstance ohvCylinderHeadInstance)
						{
							SaveMillage(carInfo.TuningData.EngineKitData.Camshaft1, ohvCylinderHeadInstance.Camshaft);
						}
					}
					else
					{
						SaveMillage(carInfo.TuningData.EngineKitData.Camshaft1, sohcCylinderHeadInstance.Camshaft);
					}
				}
				else
				{
					SaveMillage(carInfo.TuningData.EngineKitData.Camshaft1, dohcCylinderHeadInstance.IntakeCamshaft);
					SaveMillage(carInfo.TuningData.EngineKitData.Camshaft2, dohcCylinderHeadInstance.ExhaustCamshaft);
				}
			}
			if (useFullSave)
			{
				carInfo.Save();
				return;
			}
			string carInfoPath = DirectoryHelper.GetCarInfoPath(carInfo.CarInstanceId);
			EasySaveDataHandler.SaveValue(carInfo.TuningData, carInfoPath, "TuningData");
			carInfo.InvokeCarInfoUpdated();
		}

		public static void Save(this CarInfo carInfo)
		{
			EasySaveDataHandler.SaveValues(new Dictionary<string, object>
			{
				{ "CarInstanceId", carInfo.CarInstanceId },
				{ "CarID", carInfo.CarID },
				{ "TuningData", carInfo.TuningData },
				{ "BoughtBodyKits", carInfo.BoughtBodyKits },
				{ "InstalledBodyKits", carInfo.InstalledBodyKits },
				{ "Colors", carInfo.Colors },
				{ "AdditionalData", carInfo.AdditionalData }
			}, DirectoryHelper.GetCarInfoPath(carInfo.CarInstanceId));
			carInfo.InvokeCarInfoUpdated();
		}

		public static void Read(this CarInfo carInfo, ES3File file)
		{
			carInfo.CarInstanceId = (string)file.Load<object>("CarInstanceId");
			carInfo.CarID = (int)file.Load<object>("CarID");
			carInfo.TuningData = file.Load<CarTuningData>("TuningData");
			carInfo.BoughtBodyKits = file.Load<BoughtBodyKits>("BoughtBodyKits");
			carInfo.InstalledBodyKits = file.Load<InstalledBodyKits>("InstalledBodyKits");
			carInfo.Colors = file.Load<Colors>("Colors");
			carInfo.AdditionalData = file.Load<AdditionalData>("AdditionalData");
		}

		public static int GetInstalledBodyKitId(this InstalledBodyKits installedBodyKits, BodyKitType bodyKitType)
		{
			return bodyKitType switch
			{
				BodyKitType.Spoiler => installedBodyKits.SpoilerId, 
				BodyKitType.FrontBumper => installedBodyKits.FrontBumperId, 
				BodyKitType.RearBumper => installedBodyKits.RearBumperId, 
				BodyKitType.Skirt => installedBodyKits.SkirtId, 
				BodyKitType.Hood => installedBodyKits.HoodId, 
				BodyKitType.AirIntakeHood => installedBodyKits.HoodAirIntakeId, 
				BodyKitType.AirIntakeRoof => installedBodyKits.RoofAirIntakeId, 
				BodyKitType.Fender => installedBodyKits.FenderId, 
				BodyKitType.Interior => installedBodyKits.InteriorId, 
				_ => -1, 
			};
		}

		public static int[] GetInstalledBodyKitIds(this InstalledBodyKits installedBodyKits, BodyKitType bodyKitType)
		{
			if (bodyKitType == BodyKitType.Trim)
			{
				if (installedBodyKits.Trims != null)
				{
					return (int[])installedBodyKits.Trims.Clone();
				}
				return Array.Empty<int>();
			}
			return new int[1] { installedBodyKits.GetInstalledBodyKitId(bodyKitType) };
		}

		public static List<int> GetBoughtBodyKits(this BoughtBodyKits boughtBodyKits, BodyKitType bodyKitType)
		{
			switch (bodyKitType)
			{
			case BodyKitType.Fender:
				return boughtBodyKits.FenderIds;
			case BodyKitType.Hood:
				return boughtBodyKits.HoodIds;
			case BodyKitType.FrontBumper:
				return boughtBodyKits.FrontBumperIds;
			case BodyKitType.RearBumper:
				return boughtBodyKits.RearBumperIds;
			case BodyKitType.Skirt:
				return boughtBodyKits.SkirtIds;
			case BodyKitType.AirIntakeHood:
				return boughtBodyKits.HoodAirIntakeIds;
			case BodyKitType.AirIntakeRoof:
				return boughtBodyKits.RoofAirIntakeIds;
			case BodyKitType.Spoiler:
				return boughtBodyKits.SpoilerIds;
			case BodyKitType.Trim:
				return boughtBodyKits.TrimIds;
			case BodyKitType.Interior:
				return boughtBodyKits.Interiors;
			default:
				Debug.LogError("Unknown type of body kits:" + bodyKitType);
				return null;
			}
		}

		public static void AddBoughtBodyKits(this BoughtBodyKits boughtBodyKits, BodyKitType bodyKitType, int index)
		{
			List<int> boughtBodyKits2 = boughtBodyKits.GetBoughtBodyKits(bodyKitType);
			if (!boughtBodyKits2.Contains(index))
			{
				boughtBodyKits2.Add(index);
				OnBoughtBodyKitsUpdated?.Invoke(boughtBodyKits, bodyKitType, index);
			}
		}

		public static ref int GetInstalledBodyKitIndexRef(this InstalledBodyKits installedBodyKits, BodyKitType bodyKitType)
		{
			if (bodyKitType == BodyKitType.None)
			{
				throw new ArgumentException("Body kit type is None");
			}
			ref int reference = ref installedBodyKits.SpoilerId;
			switch (bodyKitType)
			{
			case BodyKitType.Spoiler:
				reference = ref installedBodyKits.SpoilerId;
				break;
			case BodyKitType.FrontBumper:
				reference = ref installedBodyKits.FrontBumperId;
				break;
			case BodyKitType.RearBumper:
				reference = ref installedBodyKits.RearBumperId;
				break;
			case BodyKitType.Skirt:
				reference = ref installedBodyKits.SkirtId;
				break;
			case BodyKitType.Hood:
				reference = ref installedBodyKits.HoodId;
				break;
			case BodyKitType.AirIntakeHood:
				reference = ref installedBodyKits.HoodAirIntakeId;
				break;
			case BodyKitType.AirIntakeRoof:
				reference = ref installedBodyKits.RoofAirIntakeId;
				break;
			case BodyKitType.Fender:
				reference = ref installedBodyKits.FenderId;
				break;
			case BodyKitType.Interior:
				reference = ref installedBodyKits.InteriorId;
				break;
			default:
				reference = -1;
				break;
			}
			return ref reference;
		}

		public static bool GetBodyKitPaidStatus(this BoughtBodyKits boughtBodyKits, BodyKitType bodyKitType, int index)
		{
			return bodyKitType switch
			{
				BodyKitType.Spoiler => boughtBodyKits.SpoilerIds.Contains(index), 
				BodyKitType.Fender => boughtBodyKits.FenderIds.Contains(index), 
				BodyKitType.Hood => boughtBodyKits.HoodIds.Contains(index), 
				BodyKitType.Skirt => boughtBodyKits.SkirtIds.Contains(index), 
				BodyKitType.FrontBumper => boughtBodyKits.FrontBumperIds.Contains(index), 
				BodyKitType.RearBumper => boughtBodyKits.RearBumperIds.Contains(index), 
				BodyKitType.AirIntakeHood => boughtBodyKits.HoodAirIntakeIds.Contains(index), 
				BodyKitType.AirIntakeRoof => boughtBodyKits.RoofAirIntakeIds.Contains(index), 
				BodyKitType.Trim => boughtBodyKits.TrimIds.Contains(index), 
				BodyKitType.Interior => boughtBodyKits.Interiors.Contains(index), 
				_ => false, 
			};
		}

		public static bool IsBodyKitInstalled(this InstalledBodyKits installedBodyKits, BodyKitType bodyKitType, int index)
		{
			if (bodyKitType == BodyKitType.Trim)
			{
				return installedBodyKits.Trims.Contains(index);
			}
			return index == installedBodyKits.GetInstalledBodyKitId(bodyKitType);
		}

		public static int[] GetInstalledKitIds(this InstalledBodyKits kits)
		{
			if (kits == null)
			{
				kits = new InstalledBodyKits();
			}
			return new int[9] { kits.SpoilerId, kits.FrontBumperId, kits.RearBumperId, kits.SkirtId, kits.HoodId, kits.HoodAirIntakeId, kits.RoofAirIntakeId, kits.FenderId, kits.InteriorId };
		}

		public static int[] GetInstalledTrims(this InstalledBodyKits kits)
		{
			if (kits == null)
			{
				kits = new InstalledBodyKits();
			}
			if (kits.Trims == null)
			{
				kits.Trims = Array.Empty<int>();
			}
			return (int[])kits.Trims.Clone();
		}

		public static bool TryGetCarVinyls(this CarInfo carInfo, VinylsType vinylsType, out List<VinylData> allVinyls)
		{
			allVinyls = new List<VinylData>();
			string carInstanceId = carInfo.CarInstanceId;
			string text = "";
			switch (vinylsType)
			{
			case VinylsType.CarVinyls:
				text = DirectoryHelper.GetCarVinylsPath(carInstanceId);
				break;
			case VinylsType.WindowVinyls:
				text = DirectoryHelper.GetWindowVinylsPath(carInstanceId);
				break;
			default:
				return false;
			}
			if (!EasySaveDataHandler.TryGetFile(text, out var file))
			{
				return false;
			}
			string[] keys = file.GetKeys();
			foreach (string key in keys)
			{
				allVinyls.Add(file.Load<VinylData>(key));
			}
			return true;
		}

		public static bool IsInstanceIdCorrect(this CarInfo carInfo, bool tryFixInstanceId)
		{
			if (_regex == null)
			{
				_regex = new Regex("[^a-zA-Z0-9_]");
			}
			if (_regex.IsMatch(carInfo.CarInstanceId))
			{
				if (tryFixInstanceId)
				{
					try
					{
						string[] array = carInfo.CarInstanceId.Split("_");
						if (array.Length >= 4)
						{
							string text = UnityEngine.Random.Range(0, 9999).ToString();
							string text2 = (int)DateTime.Now.TimeOfDay.TotalSeconds + text;
							carInfo.CarInstanceId = array[0] + "_" + array[1] + "_" + array[2] + "_" + text2;
						}
					}
					catch (Exception ex)
					{
						Debug.LogError("Error while fixing car instance id. Random: " + ex.Message);
						string replacement = UnityEngine.Random.Range(0, 9999).ToString();
						carInfo.CarInstanceId = _regex.Replace(carInfo.CarInstanceId, replacement);
					}
				}
				return false;
			}
			return true;
		}
	}
}
