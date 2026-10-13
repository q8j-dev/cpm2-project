using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using Newtonsoft.Json;
using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning.Instances;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EngineTuning
{
	public static class PartDataExtensions
	{
		private const string Separator = "?";

		private const string Separator2 = "%";

		public static PartItem GetPart(this PartData partData, AllParts allParts)
		{
			return allParts.GetPart(partData);
		}

		public static PartData GetPartData<TPart>(this PartInstance<TPart> partInstance) where TPart : PartItem
		{
			if (partInstance == null || !partInstance.Part)
			{
				return null;
			}
			return new PartData
			{
				InstanceID = partInstance.InstanceId,
				PartType = partInstance.Part.TypeId,
				Wear = partInstance.Wear,
				ModelId = partInstance.Part.ModelId
			};
		}

		public static string GetPartName(this PartItem part)
		{
			if ((bool)part)
			{
				return part.GetName;
			}
			return "N/A";
		}

		public static bool IsTamperedConfig(this PartItem p)
		{
			if (p == null)
			{
				return false;
			}
			if (p.TypeId == PartType.GearBox || p.TypeId == PartType.FrontLSD || p.TypeId == PartType.RearLSD)
			{
				return false;
			}
			if (!p.IsLinkProfileStale())
			{
				return p.EngineLinkCount == 0;
			}
			return true;
		}

		public static bool CheckNotCompatibleEngine(this PartItem p, int engineId)
		{
			if (p.TypeId == PartType.GearBox || p.TypeId == PartType.FrontLSD || p.TypeId == PartType.RearLSD)
			{
				return false;
			}
			if (p.IsLinkProfileStale())
			{
				return true;
			}
			if (p.EngineLinkCount == 0)
			{
				int num = p.ModelId / 100000;
				bool flag = num != engineId;
				if (flag)
				{
					Debug.LogWarning($"[AntiCheat] Empty CompatibleEngines on {p.TypeId} ModelId={p.ModelId} — rejecting install on engine {engineId} (native={num}).");
				}
				return flag;
			}
			return !p.IsEngineLinked(engineId);
		}

		public static bool CheckNotCompatibleCar(this PartItem p, int carId)
		{
			if (p.IsLinkProfileStale())
			{
				return true;
			}
			if (p.CarExcludeCount > 0 && p.IsCarExcluded(carId))
			{
				return true;
			}
			if (p.CarLinkCount > 0)
			{
				return !p.IsCarLinked(carId);
			}
			return false;
		}

		public static bool CheckNotCompatibleFull(this PartItem p, int carId, int engineId)
		{
			if (!p.CheckNotCompatibleEngine(engineId))
			{
				return p.CheckNotCompatibleCar(carId);
			}
			return true;
		}

		public static string GetPartDescriptionName(this PartItem part)
		{
			return part.TypeId switch
			{
				PartType.Block_Inline => "Block.Description", 
				PartType.Block_Vee => "Block.Description", 
				PartType.Crankshaft => "Crankshaft.Description", 
				PartType.None => string.Empty, 
				PartType.ConnectionRod => "ConnectionRods.Description", 
				PartType.Pistons => "Pistons.Description", 
				PartType.Flywheel => "Flywheel.Description", 
				PartType.SOHC_CylinderHead => "CylinderHead.Description", 
				PartType.DOHC_CylinderHead => "CylinderHead.Description", 
				PartType.OHV_CylinderHead => "CylinderHead.Description", 
				PartType.SOHC_Camshaft => "Camshaft_SOHC.Description", 
				PartType.DOHC_Camshaft_Intake => "Camshaft_Intake.Description", 
				PartType.DOHC_Camshaft_Exhaust => "Camshaft_Exhaust.Description", 
				PartType.OHV_Camshaft => "Camshaft_OHV.Description", 
				PartType.IntakeManifold => "IntakeManifold.Description", 
				PartType.AirFilter => "AirFilter.Description", 
				PartType.FuelRail => "FuelRail.Description", 
				PartType.FuelPump => "FuelPump.Description", 
				PartType.FuelInjectors => "FuelInjectors.Description", 
				PartType.ExhaustHeader => "ExhaustHeader.Description", 
				PartType.ExhaustPipe => "ExhaustPipe.Description", 
				PartType.ThrottleValve => "ThrottleBody.Description", 
				PartType.GearBox => "GearBox.Description", 
				PartType.Clutch => "Clutch.Description", 
				PartType.ECU => "ECU.Description", 
				PartType.TurboCharger => "Turbocharger.Description", 
				PartType.SuperCharger => "Supercharger.Description", 
				PartType.Radiator => "Radiator.Description", 
				PartType.Intercooler => "Intercooler.Description", 
				PartType.RotorHousing => "RotorHousing.Description", 
				PartType.Rotor => "Rotor.Description", 
				PartType.EccentricShaft => "EccentricShaft.Description", 
				PartType.RotorTrackingGear => "RotorTrackingGear.Description", 
				_ => part.TypeId.ToString(), 
			};
		}

		public static string GetPartTypeName(this PartItem part)
		{
			return part.TypeId switch
			{
				PartType.Block_Inline => new LocalizedString("PartType.Block_Inline"), 
				PartType.Block_Vee => new LocalizedString("PartType.Block_Vee"), 
				PartType.Crankshaft => new LocalizedString("PartType.Crankshaft"), 
				PartType.None => string.Empty, 
				PartType.ConnectionRod => new LocalizedString("PartType.ConnectionRod"), 
				PartType.Pistons => new LocalizedString("PartType.Pistons"), 
				PartType.Flywheel => new LocalizedString("PartType.Flywheel"), 
				PartType.SOHC_CylinderHead => new LocalizedString("PartType.SOHC_CylinderHead"), 
				PartType.DOHC_CylinderHead => new LocalizedString("PartType.DOHC_CylinderHead"), 
				PartType.OHV_CylinderHead => new LocalizedString("PartType.OHV_CylinderHead"), 
				PartType.SOHC_Camshaft => new LocalizedString("PartType.SOHC_Camshaft"), 
				PartType.DOHC_Camshaft_Intake => new LocalizedString("PartType.DOHC_Camshaft_Intake"), 
				PartType.DOHC_Camshaft_Exhaust => new LocalizedString("PartType.DOHC_Camshaft_Exhaust"), 
				PartType.OHV_Camshaft => new LocalizedString("PartType.OHV_Camshaft"), 
				PartType.IntakeManifold => new LocalizedString("PartType.IntakeManifold"), 
				PartType.AirFilter => new LocalizedString("PartType.AirFilter"), 
				PartType.FuelRail => new LocalizedString("PartType.FuelRail"), 
				PartType.FuelPump => new LocalizedString("PartType.FuelPump"), 
				PartType.FuelInjectors => new LocalizedString("PartType.FuelInjectors"), 
				PartType.ExhaustHeader => new LocalizedString("PartType.ExhaustHeader"), 
				PartType.ExhaustPipe => new LocalizedString("PartType.ExhaustPipe"), 
				PartType.ThrottleValve => new LocalizedString("PartType.ThrottleValve"), 
				PartType.GearBox => new LocalizedString("PartType.GearBox"), 
				PartType.Clutch => new LocalizedString("PartType.Clutch"), 
				PartType.ECU => new LocalizedString("PartType.ECU"), 
				PartType.TurboCharger => new LocalizedString("PartType.TurboCharger"), 
				PartType.SuperCharger => new LocalizedString("PartType.SuperCharger"), 
				PartType.Radiator => new LocalizedString("PartType.Radiator"), 
				PartType.Intercooler => new LocalizedString("PartType.Intercooler"), 
				PartType.FrontLSD => new LocalizedString("PartType.FrontLSD"), 
				PartType.RearLSD => new LocalizedString("PartType.RearLSD"), 
				PartType.EngineId => new LocalizedString("PartType.EngineId"), 
				PartType.RotorHousing => new LocalizedString("PartType.RotorHousing"), 
				PartType.Rotor => new LocalizedString("PartType.Rotor"), 
				PartType.EccentricShaft => new LocalizedString("PartType.EccentricShaft"), 
				PartType.RotorTrackingGear => new LocalizedString("PartType.RotorTrackingGear"), 
				_ => part.TypeId.ToString(), 
			};
		}

		public static string GetPartTypeName(this PartData part)
		{
			return part.PartType switch
			{
				PartType.Block_Inline => new LocalizedString("PartType.Block_Inline"), 
				PartType.Block_Vee => new LocalizedString("PartType.Block_Vee"), 
				PartType.Crankshaft => new LocalizedString("PartType.Crankshaft"), 
				PartType.None => string.Empty, 
				PartType.ConnectionRod => new LocalizedString("PartType.ConnectionRod"), 
				PartType.Pistons => new LocalizedString("PartType.Pistons"), 
				PartType.Flywheel => new LocalizedString("PartType.Flywheel"), 
				PartType.SOHC_CylinderHead => new LocalizedString("PartType.SOHC_CylinderHead"), 
				PartType.DOHC_CylinderHead => new LocalizedString("PartType.DOHC_CylinderHead"), 
				PartType.OHV_CylinderHead => new LocalizedString("PartType.OHV_CylinderHead"), 
				PartType.SOHC_Camshaft => new LocalizedString("PartType.SOHC_Camshaft"), 
				PartType.DOHC_Camshaft_Intake => new LocalizedString("PartType.DOHC_Camshaft_Intake"), 
				PartType.DOHC_Camshaft_Exhaust => new LocalizedString("PartType.DOHC_Camshaft_Exhaust"), 
				PartType.OHV_Camshaft => new LocalizedString("PartType.OHV_Camshaft"), 
				PartType.IntakeManifold => new LocalizedString("PartType.IntakeManifold"), 
				PartType.AirFilter => new LocalizedString("PartType.AirFilter"), 
				PartType.FuelRail => new LocalizedString("PartType.FuelRail"), 
				PartType.FuelPump => new LocalizedString("PartType.FuelPump"), 
				PartType.FuelInjectors => new LocalizedString("PartType.FuelInjectors"), 
				PartType.ExhaustHeader => new LocalizedString("PartType.ExhaustHeader"), 
				PartType.ExhaustPipe => new LocalizedString("PartType.ExhaustPipe"), 
				PartType.ThrottleValve => new LocalizedString("PartType.ThrottleValve"), 
				PartType.GearBox => new LocalizedString("PartType.GearBox"), 
				PartType.Clutch => new LocalizedString("PartType.Clutch"), 
				PartType.ECU => new LocalizedString("PartType.ECU"), 
				PartType.TurboCharger => new LocalizedString("PartType.TurboCharger"), 
				PartType.SuperCharger => new LocalizedString("PartType.SuperCharger"), 
				PartType.Radiator => new LocalizedString("PartType.Radiator"), 
				PartType.Intercooler => new LocalizedString("PartType.Intercooler"), 
				PartType.FrontLSD => new LocalizedString("PartType.FrontLSD"), 
				PartType.RearLSD => new LocalizedString("PartType.RearLSD"), 
				PartType.EngineId => new LocalizedString("PartType.EngineId"), 
				PartType.RotorHousing => new LocalizedString("PartType.RotorHousing"), 
				PartType.Rotor => new LocalizedString("PartType.Rotor"), 
				PartType.EccentricShaft => new LocalizedString("PartType.EccentricShaft"), 
				PartType.RotorTrackingGear => new LocalizedString("PartType.RotorTrackingGear"), 
				_ => part.PartType.ToString(), 
			};
		}

		public static IEnumerable<string> GetInstalledPartsName(CarTuningData carTuningData, AllParts allParts)
		{
			List<string> list = new List<string>();
			FieldInfo[] fields = typeof(CarTuningData).GetFields();
			FieldInfo[] fields2 = typeof(EngineKitData).GetFields();
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				object value = array[i].GetValue(carTuningData);
				if (value == null)
				{
					continue;
				}
				object obj = value;
				if (!(obj is EngineKitData))
				{
					if (obj is PartData partData && !string.IsNullOrEmpty(partData.GetName(allParts)))
					{
						list.Add(partData.GetName(allParts));
					}
					continue;
				}
				FieldInfo[] array2 = fields2;
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j].GetValue(carTuningData.EngineKitData) is PartData partData2 && !string.IsNullOrEmpty(partData2.GetName(allParts)))
					{
						list.Add(partData2.GetName(allParts));
					}
				}
			}
			return list;
		}

		public static PartData CreateNewPartDataFromPart(this PartItem part)
		{
			if (!part)
			{
				return null;
			}
			PartData obj = new PartData
			{
				InstanceID = 0,
				PartType = part.TypeId,
				Wear = 0f,
				ModelId = part.ModelId
			};
			obj.GenerateInstanceId();
			return obj;
		}

		public static PartData CheckCorrectPartTypeAndName(this PartData partData, AllParts allParts)
		{
			string text = partData.ModelId.ToString();
			if (text.Length > 3)
			{
				int partType = ((text.Length >= 5) ? int.Parse(text.Substring(3, 2)) : 0);
				partData.PartType = (PartType)partType;
			}
			else
			{
				int.Parse(text);
				partData.PartType = PartType.EngineId;
			}
			return partData;
		}

		public static void CheckPartModelId(this PartItem part)
		{
			try
			{
				if (!part)
				{
					return;
				}
				string text = part.ModelId.ToString();
				if (text != null && text.Length >= 8)
				{
					string text2 = text.Substring(0, 3);
					int num = int.Parse(text.Substring(3, 2));
					int num2 = int.Parse(text.Substring(5, 3));
					if (part.TypeId != (PartType)num)
					{
						Debug.LogError("Part model id not correct. SetCorrect.", part);
						int typeId = (int)part.TypeId;
						int modelId = int.Parse(text2 + typeId.ToString("00") + num2.ToString("000"));
						part.ModelId = modelId;
						part.RecalculateNotice();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Part model id not correct: " + ex, part);
			}
		}

		public static PartData GetCopy(this PartData partData)
		{
			if (partData == null)
			{
				return null;
			}
			return new PartData
			{
				InstanceID = partData.InstanceID,
				Wear = partData.Wear,
				ModelId = partData.ModelId,
				PartType = partData.PartType
			};
		}

		public static float GetMileage(this PartData partData)
		{
			PartItem part = partData.GetPart(AllParts.Instance);
			return partData.GetMileage(part);
		}

		public static float GetMileage(this PartData partData, PartItem part)
		{
			float maxMileage = part.MaxMileage;
			return Mathf.Clamp(partData.Wear * maxMileage, 0f, 1000000f);
		}

		public static float GetCondition(this PartData partData)
		{
			return 1f - partData.Wear;
		}

		public static string GetPartVendorName(this PartItem partItem)
		{
			if (AllParts.Instance.DetailVendorsConfigs != null)
			{
				Vendor vendorById = AllParts.Instance.DetailVendorsConfigs.GetVendorById(partItem.VendorId);
				if (vendorById != null)
				{
					return vendorById.Name;
				}
			}
			return string.Empty;
		}

		public static string GetPartTypeText(this PartData partData)
		{
			return Regex.Replace(Regex.Replace(partData.PartType.ToString(), "_", " "), "([a-z])([A-Z])", "$1 $2");
		}

		public static float GetDurability(this PartData partData)
		{
			return partData.GetCondition();
		}

		public static string GetName(this PartData partData)
		{
			if (partData == null)
			{
				return string.Empty;
			}
			PartItem part = AllParts.Instance.GetPart(partData);
			if (!part)
			{
				return string.Empty;
			}
			return part.GetName;
		}

		public static string GetEngineName(this PartItem partItem)
		{
			if (partItem == null)
			{
				return string.Empty;
			}
			int sourceRef = partItem.GetSourceRef();
			if (sourceRef > 0)
			{
				return AllParts.Instance.GetPart<EngineId>(PartType.EngineId, sourceRef).GetPartName();
			}
			return string.Empty;
		}

		private static string GetName(this PartData partData, AllParts allParts)
		{
			if (partData == null)
			{
				return string.Empty;
			}
			PartItem part = allParts.GetPart(partData);
			if (!part)
			{
				return string.Empty;
			}
			return part.GetName;
		}

		public static PartData CheckPartData(this PartData partData)
		{
			if (partData == null)
			{
				return null;
			}
			if (partData.ModelId == 0)
			{
				return null;
			}
			if (partData.PartType == PartType.None)
			{
				partData.CheckCorrectPartTypeAndName(AllParts.Instance);
			}
			int instanceID = partData.InstanceID;
			if (instanceID == 0 || instanceID == -1)
			{
				partData.GenerateInstanceId();
			}
			return partData;
		}

		public static void GenerateInstanceId(this PartData partData)
		{
			partData.InstanceID = GetRandomId(6);
		}

		private static int GetRandomId(int count = 1)
		{
			return UnityEngine.Random.Range(0, 999999);
		}

		public static CarDefaultTuningDataConfig GetDefaultTuningConfig(this CarInfo carInfo)
		{
			return CarTuningFactory.GetDefaultCarTuningDataConfig(carInfo.CarID).Tuning;
		}

		public static string GetStringTuningData(this CarInfo carInfo)
		{
			string item = JsonConvert.SerializeObject(CarTuningDataConverter.ToArray(carInfo.TuningData));
			List<string> list = new List<string>();
			list.Add(carInfo.CarInstanceId);
			list.Add(item);
			return string.Join("%", list);
		}

		public static CarTuningData GetTuningDataByString(string carTuningDataString, int carId, out string carInstanceId)
		{
			string[] array = carTuningDataString.Split("%");
			carInstanceId = "";
			CarTuningData result = new CarTuningData();
			if (array.Length > 1)
			{
				carInstanceId = array[0];
				result = CarTuningDataConverter.Get(JsonConvert.DeserializeObject<object[]>(array[1]));
			}
			else
			{
				Debug.LogError("Can't parse car tuning data string: " + carTuningDataString);
			}
			return result;
		}

		public static PartData CreateCompatibleECUFromEngine(this PartData partData, int engineId, int carId)
		{
			ECUPart eCUPart = (from x in AllParts.Instance.GetParts(PartType.ECU)
				select (ECUPart)x).FirstOrDefault((ECUPart x) => x.IsCarLinked(carId) && x.IsEngineLinked(engineId));
			if (!eCUPart)
			{
				eCUPart = (from x in AllParts.Instance.GetParts(PartType.ECU)
					select (ECUPart)x).FirstOrDefault((ECUPart x) => x.Rarity == 1 && x.IsEngineLinked(engineId));
			}
			partData.InstanceID = 0;
			if ((bool)eCUPart)
			{
				partData.PartType = eCUPart.TypeId;
				partData.Wear = 0f;
				partData.ModelId = eCUPart.ModelId;
			}
			partData.GenerateInstanceId();
			return partData;
		}

		public static PartData GetInstalledPartDataByPartType(this CarTuningData tuningData, PartType partType)
		{
			return partType switch
			{
				PartType.GearBox => tuningData.GearBox, 
				PartType.ECU => tuningData.ECU, 
				PartType.RearLSD => tuningData.RearLSD, 
				PartType.FrontLSD => tuningData.FrontLSD, 
				_ => null, 
			};
		}

		public static void CheckForNaNs(this CarTuningData tuningData, VehicleStockTuningData config)
		{
			TuningLimitsData frontWheelDataLimits = config.Tuning.FrontWheelDataLimits;
			TuningLimitsData rearWheelDataLimits = config.Tuning.RearWheelDataLimits;
			DynamicTuningSettings dynamicTuningSettings = tuningData.DynamicTuningSettings;
			float ceilingMm = SuspensionHeightCeilingMm(frontWheelDataLimits.SpringLengthMax);
			float ceilingMm2 = SuspensionHeightCeilingMm(rearWheelDataLimits.SpringLengthMax);
			dynamicTuningSettings.CurrentFrontSuspensionHeight = SanitizeSuspensionHeightMm(dynamicTuningSettings.CurrentFrontSuspensionHeight, ceilingMm);
			dynamicTuningSettings.MinFrontSuspensionHeight = SanitizeSuspensionHeightMm(dynamicTuningSettings.MinFrontSuspensionHeight, ceilingMm);
			dynamicTuningSettings.MaxFrontSuspensionHeight = SanitizeSuspensionHeightMm(dynamicTuningSettings.MaxFrontSuspensionHeight, ceilingMm);
			dynamicTuningSettings.CurrentRearSuspensionHeight = SanitizeSuspensionHeightMm(dynamicTuningSettings.CurrentRearSuspensionHeight, ceilingMm2);
			dynamicTuningSettings.MinRearSuspensionHeight = SanitizeSuspensionHeightMm(dynamicTuningSettings.MinRearSuspensionHeight, ceilingMm2);
			dynamicTuningSettings.MaxRearSuspensionHeight = SanitizeSuspensionHeightMm(dynamicTuningSettings.MaxRearSuspensionHeight, ceilingMm2);
			if (!float.IsNaN(dynamicTuningSettings.FrontLsdLock))
			{
				float frontLsdLock = dynamicTuningSettings.FrontLsdLock;
				if (!(frontLsdLock > -1f) || !(frontLsdLock < 10f))
				{
					goto IL_00d3;
				}
			}
			dynamicTuningSettings.FrontLsdLock = 10f;
			goto IL_00d3;
			IL_00d3:
			if (!float.IsNaN(dynamicTuningSettings.RearLsdLock))
			{
				float frontLsdLock = dynamicTuningSettings.RearLsdLock;
				if (!(frontLsdLock > -1f) || !(frontLsdLock < 10f))
				{
					goto IL_0105;
				}
			}
			dynamicTuningSettings.RearLsdLock = 10f;
			goto IL_0105;
			IL_0105:
			if (float.IsNaN(dynamicTuningSettings.HeightAdjustSpeed))
			{
				dynamicTuningSettings.HeightAdjustSpeed = 0.5f;
			}
			SteerSuspensionData suspensionData = tuningData.FrontWheelData.SuspensionData;
			if (float.IsNaN(suspensionData.Bump))
			{
				suspensionData.Bump = config.Tuning.FrontWheelData.SuspensionData.Bump;
			}
			if (float.IsNaN(suspensionData.Rebound))
			{
				suspensionData.Rebound = config.Tuning.FrontWheelData.SuspensionData.Rebound;
			}
			SuspensionData suspensionData2 = tuningData.RearWheelData.SuspensionData;
			if (float.IsNaN(suspensionData2.Bump))
			{
				suspensionData2.Bump = config.Tuning.RearWheelData.SuspensionData.Bump;
			}
			if (float.IsNaN(suspensionData2.Rebound))
			{
				suspensionData2.Rebound = config.Tuning.RearWheelData.SuspensionData.Rebound;
			}
		}

		private static float SuspensionHeightCeilingMm(float springLengthMaxInch)
		{
			return Mathf.Round(Utils.Inch2Meter(springLengthMaxInch) * 1000f);
		}

		private static float SanitizeSuspensionHeightMm(float value, float ceilingMm)
		{
			if (float.IsNaN(value) || value < 0f)
			{
				return -1f;
			}
			if (value > 0f && value < 1f)
			{
				return value;
			}
			return Mathf.Clamp(Mathf.Round(value), 0f, ceilingMm);
		}

		public static bool TryFindInstalledPartData(this CarTuningData tuningData, out PartData installedPartData, PartType partType, int indexToFind)
		{
			FieldInfo[] fields = typeof(CarTuningData).GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(tuningData);
				if (value != null && value is PartData partData && partData.PartType == partType)
				{
					int num = 0;
					int num2 = (fieldInfo.Name.Contains("Rear") ? 1 : 0);
					int num3 = (fieldInfo.Name.Contains("2") ? 1 : 0);
					if (num2 == 1 || num3 == 1)
					{
						num = 1;
					}
					if (num == indexToFind)
					{
						installedPartData = partData;
						return true;
					}
				}
			}
			installedPartData = null;
			return false;
		}

		public static int CalcShortTuningStates(this CarTuningData tuningData)
		{
			TuningPartsState partsState = GetPartsState(tuningData.EngineKitData.Block, tuningData.EngineKitData.ConnectionRod, tuningData.EngineKitData.Crankshaft, tuningData.EngineKitData.Pistons, tuningData.EngineKitData.Flywheel);
			TuningPartsState partsState2 = GetPartsState(tuningData.EngineKitData.RotorHousing, tuningData.EngineKitData.Rotors, tuningData.EngineKitData.RotorTrackingGear, tuningData.EngineKitData.EccentricShaft, tuningData.EngineKitData.Flywheel);
			TuningPartsState partsState3 = GetPartsState(tuningData.EngineKitData.CylinderHead, tuningData.EngineKitData.Camshaft1, tuningData.EngineKitData.Camshaft2);
			TuningPartsState partsState4 = GetPartsState(tuningData.EngineKitData.IntakeManifold, tuningData.EngineKitData.AirFilter, tuningData.EngineKitData.ExhaustHeader, tuningData.EngineKitData.ExhaustPipe);
			TuningPartsState partsState5 = GetPartsState(tuningData.EngineKitData.FuelRail, tuningData.EngineKitData.FuelInjectors, tuningData.EngineKitData.ThrottleValve, tuningData.EngineKitData.FuelPump);
			TuningPartsState partsState6 = GetPartsState(tuningData.EngineKitData.Radiator, tuningData.EngineKitData.Intercooler, tuningData.EngineKitData.Charger);
			TuningPartsState partsState7 = GetPartsState(tuningData.GearBox, tuningData.EngineKitData.Clutch, tuningData.FrontLSD, tuningData.RearLSD);
			return BitMaskPacker.PackTuningData(new int[7]
			{
				(int)partsState,
				(int)partsState3,
				(int)partsState4,
				(int)partsState5,
				(int)partsState6,
				(int)partsState7,
				(int)partsState2
			});
		}

		private static TuningPartsState GetPartsState(params PartData[] partDatas)
		{
			float num = 0f;
			int num2 = 0;
			foreach (PartData partData in partDatas)
			{
				if (partData != null)
				{
					num2++;
					num += partData.GetDurability() * 100f;
				}
			}
			if (num2 == 0)
			{
				return TuningPartsState.Perfect;
			}
			float num3 = num / (float)num2;
			if (num3 < 60f)
			{
				if (num3 < 20f)
				{
					return TuningPartsState.Bad;
				}
				return TuningPartsState.Normal;
			}
			if (num3 < 90f)
			{
				return TuningPartsState.Good;
			}
			return TuningPartsState.Perfect;
		}
	}
}
