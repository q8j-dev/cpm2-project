using System;
using System.Collections.Generic;
using System.Reflection;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.DataEntities.Car
{
	public static class CarTuningExtensions
	{
		public static EngineItem GetBaseEngine(this CarDefaultTuningDataConfig config)
		{
			if (!(config.EngineKitOffer != null))
			{
				return null;
			}
			return config.EngineKitOffer.Base;
		}

		public static FrontWheelData DeepClone(this FrontWheelData source)
		{
			FrontWheelData obj = source.Clone() as FrontWheelData;
			obj.BrakesData = source.BrakesData?.Clone() as BrakesData;
			obj.SuspensionData = source.SuspensionData?.Clone() as SteerSuspensionData;
			return obj;
		}

		public static RearWheelData DeepClone(this RearWheelData source)
		{
			RearWheelData obj = source.Clone() as RearWheelData;
			obj.BrakesData = source.BrakesData?.Clone() as BrakesData;
			obj.SuspensionData = source.SuspensionData?.Clone() as SuspensionData;
			return obj;
		}

		public static CarTuningData GetTuningData(this CarDefaultTuningDataConfig config)
		{
			if (config.EngineKitOffer == null)
			{
				throw new InvalidOperationException("CarDefaultTuningDataConfig.EngineKitOffer is null — assign an EngineKitOffer asset in the inspector. Without it the default engine kit cannot be built.");
			}
			if (config.EngineKitOffer.Base == null)
			{
				throw new InvalidOperationException("EngineKitOffer '" + config.EngineKitOffer.name + "' has no Base EngineItem — open the offer asset and set Base.");
			}
			CarTuningData carTuningData = new CarTuningData();
			EngineItem baseEngine = config.GetBaseEngine();
			carTuningData.EngineKitData = ApplyEngineByDefault(carTuningData, baseEngine);
			carTuningData.EngineKitData.ApplyStockExtras(config.EngineKitOffer.Extras, config.EngineKitOffer.name);
			if ((bool)config.ECU)
			{
				carTuningData.ECU = config.ECU.CreateNewPartDataFromPart();
			}
			if ((bool)config.GearBox)
			{
				carTuningData.GearBox = config.GearBox.CreateNewPartDataFromPart();
			}
			if ((bool)config.FrontDifferential)
			{
				carTuningData.FrontLSD = config.FrontDifferential.CreateNewPartDataFromPart();
			}
			if ((bool)config.RearDifferential)
			{
				carTuningData.RearLSD = config.RearDifferential.CreateNewPartDataFromPart();
			}
			carTuningData.DynamicTuningSettings.MinFrontSuspensionHeight = -1f;
			carTuningData.DynamicTuningSettings.MaxFrontSuspensionHeight = -1f;
			carTuningData.DynamicTuningSettings.MinRearSuspensionHeight = -1f;
			carTuningData.DynamicTuningSettings.MaxRearSuspensionHeight = -1f;
			carTuningData.DynamicTuningSettings.HeightAdjustSpeed = 0.5f;
			carTuningData.DynamicTuningSettings.CurrentFrontSuspensionHeight = -1f;
			carTuningData.DynamicTuningSettings.CurrentRearSuspensionHeight = -1f;
			carTuningData.FrontWheelData = config.FrontWheelData.Clone() as FrontWheelData;
			carTuningData.RearWheelData = config.RearWheelData.Clone() as RearWheelData;
			carTuningData.FrontWheelData.BrakesData = config.FrontWheelData.BrakesData.Clone() as BrakesData;
			carTuningData.RearWheelData.BrakesData = config.RearWheelData.BrakesData.Clone() as BrakesData;
			carTuningData.FrontWheelData.SuspensionData = config.FrontWheelData.SuspensionData.Clone() as SteerSuspensionData;
			carTuningData.RearWheelData.SuspensionData = config.RearWheelData.SuspensionData.Clone() as SuspensionData;
			carTuningData.Maintenance.FuelInTank = config.ChassisData.FuelTankCapacity;
			if (baseEngine.GetTypeEngine == EngineType.Wankel)
			{
				if (baseEngine.RotorHousing == null)
				{
					throw new InvalidOperationException("EngineItem '" + baseEngine.name + "' is configured as Wankel but RotorHousing is null — Wankel OilCapacity reads from RotorHousing.");
				}
				carTuningData.Maintenance.OilCapacity = baseEngine.RotorHousing.OilCapacity;
			}
			else
			{
				if (baseEngine.Block == null)
				{
					throw new InvalidOperationException("EngineItem '" + baseEngine.name + "' has no Block — piston OilCapacity reads from Block.OilCapacity.");
				}
				carTuningData.Maintenance.OilCapacity = baseEngine.Block.OilCapacity;
			}
			carTuningData.Maintenance.OilLife = 100f;
			carTuningData.Maintenance.FrontTyreHealth = new float[2] { 1f, 1f };
			carTuningData.Maintenance.RearTyreHealth = new float[2] { 1f, 1f };
			carTuningData.Maintenance.Mileage = 0f;
			return carTuningData;
		}

		private static EngineKitData ApplyEngineByDefault(CarTuningData ctd, EngineItem engineDefaultConfig)
		{
			EngineKitData obj = new EngineKitData();
			FieldInfo[] engineItemFields = EngineKitOfferExtensions.EngineItemFields;
			foreach (FieldInfo fieldInfo in engineItemFields)
			{
				object value = fieldInfo.GetValue(engineDefaultConfig);
				if (value == null)
				{
					continue;
				}
				string name = fieldInfo.Name;
				FieldInfo field = typeof(CarTuningData).GetField(name);
				if (!(field == null) && value is PartItem part)
				{
					object value2 = field.GetValue(ctd);
					if (EngineKitOfferExtensions.EngineKitDataFieldByName.TryGetValue(name, out var value3))
					{
						value3.SetValue(obj, value2);
					}
					PartData value4 = part.CreateNewPartDataFromPart();
					field.SetValue(ctd, value4);
				}
			}
			return engineDefaultConfig.GetEngineKit();
		}

		public static void ApplyStockExtras(this EngineKitData kit, PartItem[] extras, string offerContext = null)
		{
			if (kit == null || extras == null)
			{
				return;
			}
			foreach (PartItem partItem in extras)
			{
				if ((bool)partItem)
				{
					FieldInfo value2;
					if (!EngineKitOfferExtensions.PartTypeToSlot.TryGetValue(partItem.TypeId, out var value))
					{
						Debug.LogError(string.Format("[ApplyStockExtras] Offer '{0}' extra '{1}' has unmapped PartType {2} — dropped silently. Player paid for a kit that is missing this part.", offerContext ?? "<unknown>", partItem.name, partItem.TypeId));
					}
					else if (!EngineKitOfferExtensions.EngineKitDataFieldBySlot.TryGetValue(value, out value2))
					{
						Debug.LogError("[ApplyStockExtras] Offer '" + (offerContext ?? "<unknown>") + "' extra '" + partItem.name + "' maps to slot '" + value + "' which is missing on EngineKitData — dropped silently. PartTypeToSlot is out of sync with EngineKitData fields.");
					}
					else
					{
						value2.SetValue(kit, partItem.CreateNewPartDataFromPart());
					}
				}
			}
		}

		public static bool GetCheckEngineStatus(this EngineKitData engineKitData)
		{
			return false;
		}

		public static bool GetBrokenEngineStatus(this EngineKitData engineKitData)
		{
			return false;
		}

		public static void GetPrice(this EngineKitData engineKitData, AllParts allParts, out AdvancedInt total)
		{
			FieldInfo[] engineKitDataFields = EngineKitOfferExtensions.EngineKitDataFields;
			int num = 0;
			FieldInfo[] array = engineKitDataFields;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetValue(engineKitData) is PartData partData)
				{
					PartItem part = partData.GetPart(allParts);
					if ((bool)part)
					{
						float condition = partData.GetCondition();
						num += (int)((float)(int)part.GetPrice * condition);
					}
				}
			}
			total = new AdvancedInt(Mathf.RoundToInt((float)num * 0.5f));
		}

		public static AdvancedInt GetPrize(this EngineKitData engineKitData, AllParts allParts)
		{
			FieldInfo[] engineKitDataFields = EngineKitOfferExtensions.EngineKitDataFields;
			int num = 0;
			FieldInfo[] array = engineKitDataFields;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetValue(engineKitData) is PartData partData)
				{
					PartItem part = partData.GetPart(allParts);
					if ((bool)part)
					{
						float condition = partData.GetCondition();
						num += (int)((float)(int)part.GetPrice * condition);
					}
				}
			}
			return new AdvancedInt((int)((float)num * 0.5f));
		}

		public static float GetMass(this EngineKitData engineKitData, AllParts allParts)
		{
			float num = 0f;
			FieldInfo[] engineKitDataFields = EngineKitOfferExtensions.EngineKitDataFields;
			for (int i = 0; i < engineKitDataFields.Length; i++)
			{
				if (engineKitDataFields[i].GetValue(engineKitData) is PartData partData)
				{
					PartItem part = partData.GetPart(allParts);
					if ((bool)part)
					{
						num += part.Mass;
					}
				}
			}
			return num;
		}

		public static List<PartData> GetPartDataList(this EngineKitData engineKitData)
		{
			FieldInfo[] engineKitDataFields = EngineKitOfferExtensions.EngineKitDataFields;
			List<PartData> list = new List<PartData>();
			FieldInfo[] array = engineKitDataFields;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetValue(engineKitData) is PartData item)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static void RemoveEmptyPartSlots(this EngineKitData engineKitData)
		{
			if (engineKitData == null)
			{
				return;
			}
			FieldInfo[] engineKitDataFields = EngineKitOfferExtensions.EngineKitDataFields;
			foreach (FieldInfo fieldInfo in engineKitDataFields)
			{
				if (fieldInfo.GetValue(engineKitData) is PartData partData && partData.CheckPartData() == null)
				{
					fieldInfo.SetValue(engineKitData, null);
				}
			}
		}

		public static PartData GetInstalledPartDataByPartType(this EngineKitData engineKitData, PartType partType)
		{
			switch (partType)
			{
			case PartType.EngineId:
				return engineKitData.EngineId;
			case PartType.Block_Inline:
			case PartType.Block_Vee:
				return engineKitData.Block;
			case PartType.Crankshaft:
				return engineKitData.Crankshaft;
			case PartType.ConnectionRod:
				return engineKitData.ConnectionRod;
			case PartType.Pistons:
				return engineKitData.Pistons;
			case PartType.Flywheel:
				return engineKitData.Flywheel;
			case PartType.SOHC_CylinderHead:
			case PartType.DOHC_CylinderHead:
			case PartType.OHV_CylinderHead:
				return engineKitData.CylinderHead;
			case PartType.SOHC_Camshaft:
			case PartType.DOHC_Camshaft_Intake:
				return engineKitData.Camshaft1;
			case PartType.DOHC_Camshaft_Exhaust:
				return engineKitData.Camshaft2;
			case PartType.OHV_Camshaft:
				return engineKitData.Camshaft1;
			case PartType.IntakeManifold:
				return engineKitData.IntakeManifold;
			case PartType.AirFilter:
				return engineKitData.AirFilter;
			case PartType.FuelRail:
				return engineKitData.FuelRail;
			case PartType.FuelPump:
				return engineKitData.FuelPump;
			case PartType.FuelInjectors:
				return engineKitData.FuelInjectors;
			case PartType.ExhaustHeader:
				return engineKitData.ExhaustHeader;
			case PartType.ExhaustPipe:
				return engineKitData.ExhaustPipe;
			case PartType.ThrottleValve:
				return engineKitData.ThrottleValve;
			case PartType.Clutch:
				return engineKitData.Clutch;
			case PartType.TurboCharger:
			case PartType.SuperCharger:
				return engineKitData.Charger;
			case PartType.Radiator:
				return engineKitData.Radiator;
			case PartType.Intercooler:
				return engineKitData.Intercooler;
			case PartType.RotorHousing:
				return engineKitData.RotorHousing;
			case PartType.EccentricShaft:
				return engineKitData.EccentricShaft;
			case PartType.RotorTrackingGear:
				return engineKitData.RotorTrackingGear;
			case PartType.Rotor:
				return engineKitData.Rotors;
			default:
				return null;
			}
		}

		public static EngineItem GetEngine(this EngineKitData engineKitData)
		{
			EngineItem result = null;
			foreach (EngineItem model in AllParts.Instance.EngineKits.Models)
			{
				if (engineKitData.EngineId.ModelId == model.ModelId)
				{
					result = model;
					break;
				}
			}
			return result;
		}

		public static EngineKitData GetEngineKit(this EngineItem engineDefaultConfig)
		{
			EngineKitData engineKitData = new EngineKitData();
			FieldInfo[] engineItemFields = EngineKitOfferExtensions.EngineItemFields;
			foreach (FieldInfo fieldInfo in engineItemFields)
			{
				if (fieldInfo.GetValue(engineDefaultConfig) is PartItem part && EngineKitOfferExtensions.EngineKitDataFieldByName.TryGetValue(fieldInfo.Name, out var value))
				{
					PartData value2 = part.CreateNewPartDataFromPart();
					value.SetValue(engineKitData, value2);
				}
			}
			return engineKitData;
		}

		private static void ApplyEngine(CarTuningData ctd, EngineItem engineDefaultConfig)
		{
			ctd.EngineKitData.EngineId = engineDefaultConfig.EngineId.CreateNewPartDataFromPart();
			ctd.EngineKitData.Block = engineDefaultConfig.Block.CreateNewPartDataFromPart();
			ctd.EngineKitData.IntakeManifold = engineDefaultConfig.IntakeManifold.CreateNewPartDataFromPart();
			ctd.EngineKitData.AirFilter = engineDefaultConfig.AirFilter.CreateNewPartDataFromPart();
			ctd.EngineKitData.ExhaustHeader = engineDefaultConfig.ExhaustHeader.CreateNewPartDataFromPart();
			ctd.EngineKitData.ExhaustPipe = engineDefaultConfig.ExhaustPipe.CreateNewPartDataFromPart();
			ctd.EngineKitData.Crankshaft = engineDefaultConfig.Crankshaft.CreateNewPartDataFromPart();
			ctd.EngineKitData.ConnectionRod = engineDefaultConfig.ConnectionRod.CreateNewPartDataFromPart();
			ctd.EngineKitData.Pistons = engineDefaultConfig.Pistons.CreateNewPartDataFromPart();
			ctd.EngineKitData.Flywheel = engineDefaultConfig.Flywheel.CreateNewPartDataFromPart();
			ctd.EngineKitData.CylinderHead = engineDefaultConfig.CylinderHead.CreateNewPartDataFromPart();
			ctd.EngineKitData.Camshaft1 = engineDefaultConfig.Camshaft1.CreateNewPartDataFromPart();
			ctd.EngineKitData.Camshaft2 = engineDefaultConfig.Camshaft2.CreateNewPartDataFromPart();
			ctd.EngineKitData.FuelRail = engineDefaultConfig.FuelRail.CreateNewPartDataFromPart();
			ctd.EngineKitData.FuelInjectors = engineDefaultConfig.FuelInjectors.CreateNewPartDataFromPart();
			ctd.EngineKitData.ThrottleValve = engineDefaultConfig.ThrottleValve.CreateNewPartDataFromPart();
			ctd.EngineKitData.FuelPump = engineDefaultConfig.FuelPump.CreateNewPartDataFromPart();
			ctd.EngineKitData.Radiator = engineDefaultConfig.Radiator.CreateNewPartDataFromPart();
			ctd.EngineKitData.Intercooler = engineDefaultConfig.Intercooler.CreateNewPartDataFromPart();
			ctd.EngineKitData.Clutch = engineDefaultConfig.Clutch.CreateNewPartDataFromPart();
			ctd.EngineKitData.RotorHousing = engineDefaultConfig.RotorHousing.CreateNewPartDataFromPart();
			ctd.EngineKitData.EccentricShaft = engineDefaultConfig.EccentricShaft.CreateNewPartDataFromPart();
			ctd.EngineKitData.RotorTrackingGear = engineDefaultConfig.RotorTrackingGear.CreateNewPartDataFromPart();
			ctd.EngineKitData.Rotors = engineDefaultConfig.Rotors.CreateNewPartDataFromPart();
			ctd.EngineKitData.Charger = engineDefaultConfig.Charger.CreateNewPartDataFromPart();
		}
	}
}
