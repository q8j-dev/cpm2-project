using System;
using System.Collections.Generic;
using CPM.Database;
using DebugPagesUi;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.DebugNamespace
{
	public class DebugEngineKit
	{
		private const string EngineKitGroup = "Engine(health)/";

		private const string EngineGroup = "Engine(health)/Engine/";

		private const string GearboxGroup = "Engine(health)/Gearbox/";

		private const string BlockGroup = "Engine(health)/Engine/Block/";

		private const string HeadGroup = "Engine(health)/Engine/Head/";

		private const string AirSystemGroup = "Engine(health)/Engine/AirSystem/";

		private const string FuelSystemGroup = "Engine(health)/Engine/FuelSystem/";

		private const string CoolingGroup = "Engine(health)/Engine/Cooling/";

		[DebugIntHandler("Engine(health)/Engine/Block/BlockVee")]
		public static void UpdateBlockVee(int value)
		{
			UpdateEnginePart(PartType.Block_Vee, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Block/ConnectionRod")]
		public static void UpdateConnectionRod(int value)
		{
			UpdateEnginePart(PartType.ConnectionRod, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Block/Crankshaft")]
		public static void UpdateCrankshaft(int value)
		{
			UpdateEnginePart(PartType.Crankshaft, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Block/Piston")]
		public static void UpdatePiston(int value)
		{
			UpdateEnginePart(PartType.Pistons, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Block/Flywheel")]
		public static void UpdateFlywheel(int value)
		{
			UpdateEnginePart(PartType.Flywheel, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Head/OHV_CylinderHead")]
		public static void UpdateOHV_CylinderHead(int value)
		{
			UpdateEnginePart(PartType.OHV_CylinderHead, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Head/DOHC_CylinderHead")]
		public static void UpdateDOHC_CylinderHead(int value)
		{
			UpdateEnginePart(PartType.DOHC_CylinderHead, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Head/DOHC_Camshaft_Intake")]
		public static void UpdateDOHC_Camshaft_Intake(int value)
		{
			UpdateEnginePart(PartType.DOHC_Camshaft_Intake, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Head/DOHC_Camshaft_Exhaust")]
		public static void UpdateDOHC_Camshaft_Exhaust(int value)
		{
			UpdateEnginePart(PartType.DOHC_Camshaft_Exhaust, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Head/SOHC_Camshaft")]
		public static void UpdateSOHC_Camshaft(int value)
		{
			UpdateEnginePart(PartType.SOHC_Camshaft, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Head/OHV_Camshaft")]
		public static void UpdateOHV_Camshaft(int value)
		{
			UpdateEnginePart(PartType.OHV_Camshaft, value);
		}

		[DebugIntHandler("Engine(health)/Engine/AirSystem/IntakeManifold")]
		public static void UpdateIntakeManifold(int value)
		{
			UpdateEnginePart(PartType.IntakeManifold, value);
		}

		[DebugIntHandler("Engine(health)/Engine/AirSystem/AirFilter")]
		public static void UpdateAirFilter(int value)
		{
			UpdateEnginePart(PartType.AirFilter, value);
		}

		[DebugIntHandler("Engine(health)/Engine/AirSystem/ExhaustHeader")]
		public static void UpdateExhaustHeader(int value)
		{
			UpdateEnginePart(PartType.ExhaustHeader, value);
		}

		[DebugIntHandler("Engine(health)/Engine/AirSystem/ExhaustPipe")]
		public static void UpdateExhaustPipe(int value)
		{
			UpdateEnginePart(PartType.ExhaustPipe, value);
		}

		[DebugIntHandler("Engine(health)/Engine/FuelSystem/FuelRail")]
		public static void UpdateFuelRail(int value)
		{
			UpdateEnginePart(PartType.FuelRail, value);
		}

		[DebugIntHandler("Engine(health)/Engine/FuelSystem/ThrottleValve")]
		public static void UpdateThrottleValve(int value)
		{
			UpdateEnginePart(PartType.ThrottleValve, value);
		}

		[DebugIntHandler("Engine(health)/Engine/FuelSystem/FuelPump")]
		public static void UpdateFuelPump(int value)
		{
			UpdateEnginePart(PartType.FuelPump, value);
		}

		[DebugIntHandler("Engine(health)/Engine/FuelSystem/FuelInjectors")]
		public static void UpdateFuelInjectors(int value)
		{
			UpdateEnginePart(PartType.FuelInjectors, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Cooling/Radiator")]
		public static void UpdateRadiator(int value)
		{
			UpdateEnginePart(PartType.Radiator, value);
		}

		[DebugIntHandler("Engine(health)/Engine/Cooling/Intercooler")]
		public static void UpdateIntercooler(int value)
		{
			UpdateEnginePart(PartType.Intercooler, value);
		}

		[DebugIntHandler("Engine(health)/Engine/SuperCharger")]
		public static void UpdateSuperCharger(int value)
		{
			UpdateEnginePart(PartType.SuperCharger, value);
		}

		[DebugIntHandler("Engine(health)/Engine/TurboCharger")]
		public static void UpdateTurboCharger(int value)
		{
			UpdateEnginePart(PartType.TurboCharger, value);
		}

		[DebugIntHandler("Engine(health)/Engine/ECU")]
		public static void UpdateECU(int value)
		{
			UpdateEnginePart(PartType.ECU, value);
		}

		[DebugIntHandler("Engine(health)/Gearbox/GearBox")]
		public static void UpdateGearBox(int value)
		{
			UpdateEnginePart(PartType.GearBox, value);
		}

		[DebugIntHandler("Engine(health)/Gearbox/Clutch")]
		public static void UpdateClutch(int value)
		{
			UpdateEnginePart(PartType.Clutch, value);
		}

		[DebugIntHandler("Engine(health)/Gearbox/RearLSD")]
		public static void UpdateRearLSD(int value)
		{
			UpdateEnginePart(PartType.RearLSD, value);
		}

		[DebugIntHandler("Engine(health)/Gearbox/FrontLSD")]
		public static void UpdateFrontLSD(int value)
		{
			UpdateEnginePart(PartType.FrontLSD, value);
		}

		[DebugIntHandler("Engine(health)/All")]
		public static void UpdateHealthAll(int value)
		{
			foreach (PartType value2 in Enum.GetValues(typeof(PartType)))
			{
				UpdateEnginePart(value2, value);
			}
		}

		private static void UpdateEnginePart(PartType partType, int value)
		{
			if (!V.Car || !V.Car.TryGetComponent<VehicleData>(out var component) || !V.Car.TryGetComponent<VehicleEntityProvider>(out var component2))
			{
				return;
			}
			List<PartData> partDataList = component.carInfo.TuningData.EngineKitData.GetPartDataList();
			partDataList.Add(component.carInfo.TuningData.ECU);
			partDataList.Add(component.carInfo.TuningData.GearBox);
			partDataList.Add(component.carInfo.TuningData.FrontLSD);
			partDataList.Add(component.carInfo.TuningData.RearLSD);
			for (int i = 0; i < partDataList.Count; i++)
			{
				if (partDataList[i].PartType == partType)
				{
					partDataList[i].Wear = 1f - (float)value / 100f;
				}
			}
			SaveCarInfoToMillage(component.carInfo, component2.CarEntity);
		}

		private static void SaveCarInfoToMillage(CarInfo carInfo, CarEntity carEntity)
		{
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.EngineId, carEntity.Engine);
			if ((bool)carEntity.Engine.RotorHousing?.Part)
			{
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.RotorHousing, carEntity.Engine.Block);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Rotors, carEntity.Engine.RotorHousing.Rotors);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.RotorTrackingGear, carEntity.Engine.RotorHousing.RotorTrackingGears);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.EccentricShaft, carEntity.Engine.RotorHousing.EccentricShaft);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Flywheel, carEntity.Engine.RotorHousing.Flywheel);
			}
			else
			{
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Block, carEntity.Engine.Block);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Crankshaft, carEntity.Engine.Block.Crankshaft);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.ConnectionRod, carEntity.Engine.Block.ConnectionRod);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Pistons, carEntity.Engine.Block.Pistons);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Flywheel, carEntity.Engine.Block.Flywheel);
				SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.CylinderHead, carEntity.Engine.CylinderHead);
				CylinderHeadInstance cylinderHead = carEntity.Engine.CylinderHead;
				if (!(cylinderHead is DohcCylinderHeadInstance dohcCylinderHeadInstance))
				{
					if (!(cylinderHead is SohcCylinderHeadInstance sohcCylinderHeadInstance))
					{
						if (cylinderHead is OhvCylinderHeadInstance ohvCylinderHeadInstance)
						{
							SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Camshaft1, ohvCylinderHeadInstance.Camshaft);
						}
					}
					else
					{
						SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Camshaft1, sohcCylinderHeadInstance.Camshaft);
					}
				}
				else
				{
					SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Camshaft1, dohcCylinderHeadInstance.IntakeCamshaft);
					SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Camshaft2, dohcCylinderHeadInstance.ExhaustCamshaft);
				}
			}
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.IntakeManifold, carEntity.Engine.IntakeManifold);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.ThrottleValve, carEntity.Engine.ThrottleValve);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.AirFilter, carEntity.Engine.IntakeManifold.AirFilter);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Charger, carEntity.Engine.Charger);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.FuelPump, carEntity.Engine.FuelPump);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.FuelRail, carEntity.Engine.FuelRail);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.FuelInjectors, carEntity.Engine.FuelInjectors);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.ExhaustHeader, carEntity.Engine.ExhaustHeader);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.ExhaustPipe, carEntity.Engine.ExhaustPipe);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Radiator, carEntity.Engine.Radiator);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Intercooler, carEntity.Engine.Intercooler);
			SaveCarInfoToMillage(carInfo.TuningData.ECU, carEntity.Engine.ECU);
			SaveCarInfoToMillage(carInfo.TuningData.GearBox, carEntity.GearBox);
			SaveCarInfoToMillage(carInfo.TuningData.FrontLSD, carEntity.FrontLsd);
			SaveCarInfoToMillage(carInfo.TuningData.RearLSD, carEntity.RearLsd);
			SaveCarInfoToMillage(carInfo.TuningData.EngineKitData.Clutch, carEntity.Engine.Clutch);
			carInfo.Save();
		}

		private static void SaveCarInfoToMillage<T>(PartData partData, PartInstance<T> partInstance) where T : PartItem
		{
			if (partData != null && partInstance != null && !(partInstance.Part == null))
			{
				partInstance.Mileage = partData.Wear * partInstance.Part.MaxMileage;
			}
		}
	}
}
