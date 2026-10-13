using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPMEngine;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EngineTuning
{
	public class CarTuningFactory : ICarTuningFactory
	{
		private readonly AllParts _allParts;

		private IInventoryPartsService _inventoryPartsService;

		private IEngineKitsInventoryService _engineKitsInventoryService;

		private IDatabaseCarsService _databaseCarsService;

		private bool _allPartsWasUpdated;

		private bool _engineMissingLogged;

		private static readonly Dictionary<int, VehicleStockTuningData> _defaultConfigCache = new Dictionary<int, VehicleStockTuningData>();

		public event InstallPartHandler OnUpdateCar;

		public event Action<string, float> MessageNotification;

		public event Action<string, float, uint> MessageNotificationWithOtherSound;

		[Inject]
		public CarTuningFactory(AllParts allParts, IInventoryPartsService inventoryPartsService, IEngineKitsInventoryService engineKitsInventoryService, IDatabaseCarsService databaseCarsService)
		{
			_engineKitsInventoryService = engineKitsInventoryService;
			_inventoryPartsService = inventoryPartsService;
			_allParts = allParts;
			_databaseCarsService = databaseCarsService;
		}

		public CarEntity AssemblyCarWithTuningData(CarTuningData tuningDataInstance, int carId, out bool needUpdate)
		{
			VehicleStockTuningData defaultCarTuningDataConfig = GetDefaultCarTuningDataConfig(carId);
			CarEntity result = Create(tuningDataInstance, defaultCarTuningDataConfig, carId, out needUpdate);
			InstallPartHandler installPartHandler = OnUpdateCar;
			if (installPartHandler != null)
			{
				installPartHandler(this);
				return result;
			}
			return result;
		}

		public CarEntity AssemblyNewCar(CarTuningData tuningDataInstance, VehicleStockTuningData vehicleStock, int carId)
		{
			VehiclePresetTuningData[] presets = vehicleStock.Presets;
			if (presets != null && presets.Length > 0 && (bool)vehicleStock.Presets[0])
			{
				tuningDataInstance.FrontWheelData = vehicleStock.Presets[0].FrontWheelData.DeepClone();
				tuningDataInstance.RearWheelData = vehicleStock.Presets[0].RearWheelData.DeepClone();
				if ((bool)vehicleStock.Presets[0].GearBox)
				{
					tuningDataInstance.GearBox = vehicleStock.Presets[0].GearBox.CreateNewPartDataFromPart();
				}
			}
			CarEntity result = Create(tuningDataInstance, vehicleStock, carId, out var _);
			InstallPartHandler installPartHandler = OnUpdateCar;
			if (installPartHandler != null)
			{
				installPartHandler(this);
				return result;
			}
			return result;
		}

		public CarEntity AssemblyCar(CarInfo carInfo, bool checkNulls = false)
		{
			if (checkNulls)
			{
				VehicleStockTuningData defaultCarTuningDataConfig = GetDefaultCarTuningDataConfig(carInfo.CarID);
				carInfo.CheckForNulls(defaultCarTuningDataConfig);
				carInfo.TuningData.CheckForNaNs(defaultCarTuningDataConfig);
			}
			CarEntity carEntity = AssemblyCarWithTuningData(carInfo.TuningData, carInfo.CarID, out var needUpdate);
			if (needUpdate)
			{
				Debug.Log("Car updated");
				CarInfoDeferredSaver.RequestSave(carInfo);
				_databaseCarsService.SaveCarInCacheToSend(carInfo);
			}
			carEntity.CarInstanceId = carInfo.CarInstanceId;
			return carEntity;
		}

		public VehicleStockTuningData CreateDefault(int carId)
		{
			return GetDefaultCarTuningDataConfig(carId);
		}

		public static VehicleStockTuningData GetDefaultCarTuningDataConfig(int carId)
		{
			if (_defaultConfigCache.TryGetValue(carId, out var value) && (bool)value)
			{
				return value;
			}
			VehicleStockTuningData vehicleStockTuningData = Resources.Load<VehicleStockTuningData>(string.Format("{0}car{1}", "Configs/CarTuningData/", carId));
			if ((bool)vehicleStockTuningData)
			{
				_defaultConfigCache[carId] = vehicleStockTuningData;
			}
			return vehicleStockTuningData;
		}

		private void SaveCar(CarTuningData carTuningDataInstance)
		{
			string text = JsonUtility.ToJson(carTuningDataInstance);
			Debug.Log("Save car:" + text);
			PlayerPrefs.SetString($"carTuning{V.CarId}", text);
		}

		private CarEntity Create(CarTuningData carTuningData, VehicleStockTuningData vehicleStock, int carId, out bool needUpdate)
		{
			ChassisData chassisData = vehicleStock.Tuning.ChassisData;
			EngineInstance engineInstance = CreateEngine(carTuningData, carId, out needUpdate);
			if (engineInstance == null)
			{
				return null;
			}
			GearBoxInstance gearBox = CreateGearBox(carTuningData.GearBox);
			FrontLSDInstance frontLsd = CreateFrontLSD(carTuningData);
			RearLSDInstance rearLsd = CreateRearLSD(carTuningData);
			FrontWheelData frontWheelData = carTuningData.FrontWheelData;
			RearWheelData rearWheelData = carTuningData.RearWheelData;
			return new CarEntity(engineInstance, gearBox, frontLsd, rearLsd, frontWheelData, rearWheelData, chassisData)
			{
				MaintenanceInfo = ((carTuningData.Maintenance.Clone() is MaintenanceInfo maintenanceInfo) ? maintenanceInfo : default(MaintenanceInfo))
			};
		}

		private GearBoxInstance CreateGearBox(PartData gearbox)
		{
			GearBoxPart part = _allParts.GetPart<GearBoxPart>(gearbox.CheckPartData());
			return new GearBoxInstance(part)
			{
				Part = part,
				Mileage = gearbox.GetMileage(part)
			};
		}

		private FrontLSDInstance CreateFrontLSD(CarTuningData carTuningData)
		{
			FrontLSDInstance frontLSDInstance = new FrontLSDInstance();
			frontLSDInstance.DynamicTuningSettings = carTuningData.DynamicTuningSettings;
			frontLSDInstance.Mileage = PartDataExtensions.GetMileage(part: frontLSDInstance.Part = _allParts.GetPart<FrontLSDPart>(carTuningData.FrontLSD.CheckPartData()), partData: carTuningData.FrontLSD);
			return frontLSDInstance;
		}

		private RearLSDInstance CreateRearLSD(CarTuningData carTuningData)
		{
			RearLSDInstance rearLSDInstance = new RearLSDInstance();
			rearLSDInstance.DynamicTuningSettings = carTuningData.DynamicTuningSettings;
			rearLSDInstance.Mileage = PartDataExtensions.GetMileage(part: rearLSDInstance.Part = _allParts.GetPart<RearLSDPart>(carTuningData.RearLSD.CheckPartData()), partData: carTuningData.RearLSD);
			return rearLSDInstance;
		}

		private EngineInstance CreateEngine(CarTuningData carTuningData, int carId, out bool needUpdate)
		{
			needUpdate = false;
			EngineId part = _allParts.GetPart<EngineId>(carTuningData.EngineKitData.EngineId.CheckPartData());
			EngineItem engineItem = (part ? _allParts.GetPart<EngineItem>(PartType.Engine, part.ModelId) : null);
			if (!part || !engineItem)
			{
				if (!_engineMissingLogged)
				{
					_engineMissingLogged = true;
					Debug.LogError($"CreateEngine: engine unresolved for car {carId} " + $"(modelId {carTuningData.EngineKitData.EngineId?.ModelId}). Assembly skipped.");
				}
				return null;
			}
			EngineInstance engineInstance = new EngineInstance
			{
				Part = part,
				InstanceId = carTuningData.EngineKitData.EngineId.InstanceID,
				Mileage = carTuningData.EngineKitData.EngineId.GetMileage(part)
			};
			int getTypeEngine = (int)engineItem.GetTypeEngine;
			engineInstance.SetEt(getTypeEngine);
			int modelId = carTuningData.EngineKitData.EngineId.ModelId;
			FlywheelPart flywheelPart = _allParts.GetPart<FlywheelPart>(carTuningData.EngineKitData.Flywheel.CheckPartData());
			if (!flywheelPart || flywheelPart.CheckNotCompatibleEngine(modelId))
			{
				flywheelPart = _allParts.GetCompatiblePartFromEngine<FlywheelPart>(modelId, PartType.Flywheel);
				carTuningData.EngineKitData.Flywheel = flywheelPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			RotorHousingPart rotorHousingPart = null;
			if (carTuningData.EngineKitData.RotorHousing != null)
			{
				rotorHousingPart = _allParts.GetPart<RotorHousingPart>(carTuningData.EngineKitData.RotorHousing.CheckPartData());
				if (!rotorHousingPart || rotorHousingPart.CheckNotCompatibleEngine(modelId))
				{
					rotorHousingPart = _allParts.GetCompatiblePartFromEngine<RotorHousingPart>(modelId, PartType.RotorHousing);
					carTuningData.EngineKitData.RotorHousing = rotorHousingPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
			}
			if ((bool)rotorHousingPart)
			{
				RotorPart rotorPart = _allParts.GetPart<RotorPart>(carTuningData.EngineKitData.Rotors.CheckPartData());
				if (!rotorPart || rotorPart.CheckNotCompatibleEngine(modelId))
				{
					rotorPart = _allParts.GetCompatiblePartFromEngine<RotorPart>(modelId, PartType.Rotor);
					carTuningData.EngineKitData.Rotors = rotorPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
				EccentricShaftPart eccentricShaftPart = _allParts.GetPart<EccentricShaftPart>(carTuningData.EngineKitData.EccentricShaft.CheckPartData());
				if (!eccentricShaftPart || eccentricShaftPart.CheckNotCompatibleEngine(modelId))
				{
					eccentricShaftPart = _allParts.GetCompatiblePartFromEngine<EccentricShaftPart>(modelId, PartType.EccentricShaft);
					carTuningData.EngineKitData.EccentricShaft = eccentricShaftPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
				RotorTrackingGearPart rotorTrackingGearPart = _allParts.GetPart<RotorTrackingGearPart>(carTuningData.EngineKitData.RotorTrackingGear.CheckPartData());
				if (!rotorTrackingGearPart || rotorTrackingGearPart.CheckNotCompatibleEngine(modelId))
				{
					rotorTrackingGearPart = _allParts.GetCompatiblePartFromEngine<RotorTrackingGearPart>(modelId, PartType.RotorTrackingGear);
					carTuningData.EngineKitData.RotorTrackingGear = rotorTrackingGearPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
				engineInstance.RotorHousing = new RotorHousingInstance(rotorHousingPart)
				{
					InstanceId = carTuningData.EngineKitData.RotorHousing.InstanceID,
					Mileage = carTuningData.EngineKitData.RotorHousing.GetMileage(rotorHousingPart),
					Rotors = new RotorInstance(rotorPart)
					{
						InstanceId = carTuningData.EngineKitData.Rotors.InstanceID,
						Mileage = carTuningData.EngineKitData.Rotors.GetMileage(rotorPart)
					},
					EccentricShaft = new EccentricShaftInstance(eccentricShaftPart)
					{
						InstanceId = carTuningData.EngineKitData.EccentricShaft.InstanceID,
						Mileage = carTuningData.EngineKitData.EccentricShaft.GetMileage(eccentricShaftPart)
					},
					RotorTrackingGears = new RotorTrackingGearInstance(rotorTrackingGearPart)
					{
						InstanceId = carTuningData.EngineKitData.RotorTrackingGear.InstanceID,
						Mileage = carTuningData.EngineKitData.RotorTrackingGear.GetMileage(rotorTrackingGearPart)
					},
					Flywheel = new FlywheelInstance(flywheelPart)
					{
						InstanceId = carTuningData.EngineKitData.Flywheel.InstanceID,
						Mileage = carTuningData.EngineKitData.Flywheel.GetMileage(flywheelPart)
					}
				};
			}
			else
			{
				BlockPart blockPart = _allParts.GetPart<BlockPart>(carTuningData.EngineKitData.Block.CheckPartData());
				if (!blockPart || blockPart.CheckNotCompatibleEngine(modelId))
				{
					blockPart = _allParts.GetCompatibleBlockFromEngine(carTuningData.EngineKitData.EngineId.ModelId);
					carTuningData.EngineKitData.Block = blockPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
				CrankshaftPart crankshaftPart = _allParts.GetPart<CrankshaftPart>(carTuningData.EngineKitData.Crankshaft.CheckPartData());
				if (!crankshaftPart || crankshaftPart.CheckNotCompatibleEngine(modelId))
				{
					crankshaftPart = _allParts.GetCompatiblePartFromEngine<CrankshaftPart>(modelId, PartType.Crankshaft);
					carTuningData.EngineKitData.Crankshaft = crankshaftPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
				ConnectionRodsPart connectionRodsPart = _allParts.GetPart<ConnectionRodsPart>(carTuningData.EngineKitData.ConnectionRod.CheckPartData());
				if (!connectionRodsPart || connectionRodsPart.CheckNotCompatibleEngine(modelId))
				{
					connectionRodsPart = _allParts.GetCompatiblePartFromEngine<ConnectionRodsPart>(modelId, PartType.ConnectionRod);
					carTuningData.EngineKitData.ConnectionRod = connectionRodsPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
				PistonsPart pistonsPart = _allParts.GetPart<PistonsPart>(carTuningData.EngineKitData.Pistons.CheckPartData());
				if (!pistonsPart || pistonsPart.CheckNotCompatibleEngine(modelId))
				{
					pistonsPart = _allParts.GetCompatiblePartFromEngine<PistonsPart>(modelId, PartType.Pistons);
					carTuningData.EngineKitData.Pistons = pistonsPart.CreateNewPartDataFromPart();
					needUpdate = true;
				}
				engineInstance.Block = new BlockInstance(blockPart)
				{
					InstanceId = carTuningData.EngineKitData.Block.InstanceID,
					Mileage = carTuningData.EngineKitData.Block.GetMileage(blockPart),
					Crankshaft = new CrankshaftInstance(crankshaftPart)
					{
						InstanceId = carTuningData.EngineKitData.Crankshaft.InstanceID,
						Mileage = carTuningData.EngineKitData.Crankshaft.GetMileage(crankshaftPart)
					},
					ConnectionRod = new ConnectionRodInstance(connectionRodsPart)
					{
						InstanceId = carTuningData.EngineKitData.ConnectionRod.InstanceID,
						Mileage = carTuningData.EngineKitData.ConnectionRod.GetMileage(connectionRodsPart)
					},
					Pistons = new PistonInstance(pistonsPart)
					{
						InstanceId = carTuningData.EngineKitData.Pistons.InstanceID,
						Mileage = carTuningData.EngineKitData.Pistons.GetMileage(pistonsPart)
					},
					Flywheel = new FlywheelInstance(flywheelPart)
					{
						InstanceId = carTuningData.EngineKitData.Flywheel.InstanceID,
						Mileage = carTuningData.EngineKitData.Flywheel.GetMileage(flywheelPart)
					}
				};
				SetCylinderHead(carTuningData, engineInstance, needUpdate, out var update);
				needUpdate = update;
			}
			IntakeManifoldPart intakeManifoldPart = _allParts.GetPart<IntakeManifoldPart>(carTuningData.EngineKitData.IntakeManifold.CheckPartData());
			if (!intakeManifoldPart || intakeManifoldPart.CheckNotCompatibleEngine(modelId))
			{
				intakeManifoldPart = _allParts.GetCompatiblePartFromEngine<IntakeManifoldPart>(modelId, PartType.IntakeManifold);
				carTuningData.EngineKitData.IntakeManifold = intakeManifoldPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			AirFilterPart airFilterPart = _allParts.GetPart<AirFilterPart>(carTuningData.EngineKitData.AirFilter.CheckPartData());
			if (!airFilterPart || airFilterPart.CheckNotCompatibleEngine(modelId))
			{
				airFilterPart = _allParts.GetCompatiblePartFromEngine<AirFilterPart>(modelId, PartType.AirFilter);
				carTuningData.EngineKitData.AirFilter = airFilterPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			engineInstance.IntakeManifold = new IntakeManifoldInstance(intakeManifoldPart)
			{
				InstanceId = carTuningData.EngineKitData.IntakeManifold.InstanceID,
				Mileage = carTuningData.EngineKitData.IntakeManifold.GetMileage(intakeManifoldPart),
				AirFilter = new AirFilterInstance(airFilterPart)
				{
					InstanceId = carTuningData.EngineKitData.AirFilter.InstanceID,
					Mileage = carTuningData.EngineKitData.AirFilter.GetMileage(airFilterPart)
				}
			};
			FuelRailPart fuelRailPart = _allParts.GetPart<FuelRailPart>(carTuningData.EngineKitData.FuelRail.CheckPartData());
			if (!fuelRailPart || fuelRailPart.CheckNotCompatibleEngine(modelId))
			{
				fuelRailPart = _allParts.GetCompatiblePartFromEngine<FuelRailPart>(modelId, PartType.FuelRail);
				carTuningData.EngineKitData.FuelRail = fuelRailPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			FuelInjectorsPart fuelInjectorsPart = _allParts.GetPart<FuelInjectorsPart>(carTuningData.EngineKitData.FuelInjectors.CheckPartData());
			if (!fuelInjectorsPart || fuelInjectorsPart.CheckNotCompatibleEngine(modelId))
			{
				fuelInjectorsPart = _allParts.GetCompatiblePartFromEngine<FuelInjectorsPart>(modelId, PartType.FuelInjectors);
				carTuningData.EngineKitData.FuelInjectors = fuelInjectorsPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			FuelPumpPart fuelPumpPart = _allParts.GetPart<FuelPumpPart>(carTuningData.EngineKitData.FuelPump.CheckPartData());
			if (!fuelPumpPart || fuelPumpPart.CheckNotCompatibleEngine(modelId))
			{
				fuelPumpPart = _allParts.GetCompatiblePartFromEngine<FuelPumpPart>(modelId, PartType.FuelPump);
				carTuningData.EngineKitData.FuelPump = fuelPumpPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			ThrottleValvePart throttleValvePart = _allParts.GetPart<ThrottleValvePart>(carTuningData.EngineKitData.ThrottleValve.CheckPartData());
			if (!throttleValvePart || throttleValvePart.CheckNotCompatibleEngine(modelId))
			{
				throttleValvePart = _allParts.GetCompatiblePartFromEngine<ThrottleValvePart>(modelId, PartType.ThrottleValve);
				carTuningData.EngineKitData.ThrottleValve = throttleValvePart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			ExhaustHeaderPart exhaustHeaderPart = _allParts.GetPart<ExhaustHeaderPart>(carTuningData.EngineKitData.ExhaustHeader.CheckPartData());
			if (!exhaustHeaderPart || exhaustHeaderPart.CheckNotCompatibleEngine(modelId))
			{
				exhaustHeaderPart = _allParts.GetCompatiblePartFromEngine<ExhaustHeaderPart>(modelId, PartType.ExhaustHeader);
				carTuningData.EngineKitData.ExhaustHeader = exhaustHeaderPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			ExhaustPipePart exhaustPipePart = _allParts.GetPart<ExhaustPipePart>(carTuningData.EngineKitData.ExhaustPipe.CheckPartData());
			if (!exhaustPipePart || exhaustPipePart.CheckNotCompatibleEngine(modelId))
			{
				exhaustPipePart = _allParts.GetCompatiblePartFromEngine<ExhaustPipePart>(modelId, PartType.ExhaustPipe);
				carTuningData.EngineKitData.ExhaustPipe = exhaustPipePart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			ClutchPart clutchPart = _allParts.GetPart<ClutchPart>(carTuningData.EngineKitData.Clutch.CheckPartData());
			if (!clutchPart || clutchPart.CheckNotCompatibleEngine(modelId))
			{
				clutchPart = _allParts.GetCompatiblePartFromEngine<ClutchPart>(modelId, PartType.Clutch);
				carTuningData.EngineKitData.Clutch = clutchPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			ECUPart eCUPart = _allParts.GetPart<ECUPart>(carTuningData.ECU.CheckPartData());
			if (!eCUPart || eCUPart.CheckNotCompatibleEngine(modelId))
			{
				eCUPart = _allParts.GetCompatibleEcuFromEngine(carTuningData.EngineKitData.EngineId.ModelId, carId);
				carTuningData.ECU = eCUPart.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			engineInstance.ECU = new EcuInstance(eCUPart)
			{
				InstanceId = carTuningData.ECU.InstanceID,
				Mileage = carTuningData.ECU.GetMileage(eCUPart)
			};
			engineInstance.FuelRail = new FuelRailInstance(fuelRailPart)
			{
				InstanceId = carTuningData.EngineKitData.FuelRail.InstanceID,
				Mileage = carTuningData.EngineKitData.FuelRail.GetMileage(fuelRailPart)
			};
			engineInstance.FuelInjectors = new FuelInjectorsInstance(fuelInjectorsPart)
			{
				InstanceId = carTuningData.EngineKitData.FuelInjectors.InstanceID,
				Mileage = carTuningData.EngineKitData.FuelInjectors.GetMileage(fuelInjectorsPart)
			};
			engineInstance.FuelPump = new FuelPumpInstance(fuelPumpPart)
			{
				InstanceId = carTuningData.EngineKitData.FuelPump.InstanceID,
				Mileage = carTuningData.EngineKitData.FuelPump.GetMileage(fuelPumpPart)
			};
			engineInstance.ThrottleValve = new ThrottleValveInstance(throttleValvePart)
			{
				InstanceId = carTuningData.EngineKitData.ThrottleValve.InstanceID,
				Mileage = carTuningData.EngineKitData.ThrottleValve.GetMileage(throttleValvePart)
			};
			engineInstance.ExhaustHeader = new ExhaustHeaderInstance(exhaustHeaderPart)
			{
				InstanceId = carTuningData.EngineKitData.ExhaustHeader.InstanceID,
				Mileage = carTuningData.EngineKitData.ExhaustHeader.GetMileage(exhaustHeaderPart)
			};
			engineInstance.ExhaustPipe = new ExhaustPipeInstance(exhaustPipePart)
			{
				InstanceId = carTuningData.EngineKitData.ExhaustPipe.InstanceID,
				Mileage = carTuningData.EngineKitData.ExhaustPipe.GetMileage(exhaustPipePart)
			};
			engineInstance.Clutch = new ClutchInstance(clutchPart)
			{
				InstanceId = carTuningData.EngineKitData.Clutch.InstanceID,
				Mileage = carTuningData.EngineKitData.Clutch.GetMileage(clutchPart)
			};
			if (carTuningData.EngineKitData.Charger != null)
			{
				ChargerPart part2 = _allParts.GetPart<ChargerPart>(carTuningData.EngineKitData.Charger);
				if ((bool)part2 && !part2.CheckNotCompatibleEngine(modelId))
				{
					engineInstance.Charger = new ChargerInstance(part2)
					{
						InstanceId = carTuningData.EngineKitData.Charger.InstanceID,
						Mileage = carTuningData.EngineKitData.Charger.GetMileage(part2)
					};
				}
				else
				{
					carTuningData.EngineKitData.Charger = null;
					needUpdate = true;
				}
			}
			RadiatorPart part3 = _allParts.GetPart<RadiatorPart>(carTuningData.EngineKitData.Radiator.CheckPartData());
			if (!part3 || part3.CheckNotCompatibleEngine(modelId))
			{
				part3 = _allParts.GetCompatiblePartFromEngine<RadiatorPart>(modelId, PartType.Radiator);
				carTuningData.EngineKitData.Radiator = part3.CreateNewPartDataFromPart();
				needUpdate = true;
			}
			engineInstance.Radiator = _allParts.GetPartInstance<RadiatorPart>(carTuningData.EngineKitData.Radiator);
			if (carTuningData.EngineKitData.Intercooler != null)
			{
				IntercoolerPart part4 = _allParts.GetPart<IntercoolerPart>(carTuningData.EngineKitData.Intercooler);
				if (part4 == null || part4.CheckNotCompatibleEngine(modelId))
				{
					carTuningData.EngineKitData.Intercooler = null;
					needUpdate = true;
				}
			}
			engineInstance.Intercooler = _allParts.CreateCustomInstance<IntercoolerInstance<IntercoolerPart>, IntercoolerPart>(carTuningData.EngineKitData.Intercooler);
			engineInstance.DynamicTuningSettings = carTuningData.DynamicTuningSettings;
			return engineInstance;
		}

		public void CheckStatus(EngineInstance engineInstance)
		{
			if (engineInstance.ECU == null)
			{
				MessageNotification?.Invoke(new LocalizedString("CarTuning.InstallECU"), 4f);
			}
		}

		private void SetCylinderHead(CarTuningData carTuningData, EngineInstance carEntityEngine, bool needUpdate, out bool update)
		{
			carTuningData.EngineKitData.CylinderHead.CheckPartData();
			update = needUpdate;
			PartType partType = carTuningData.EngineKitData.CylinderHead.PartType;
			CylinderHeadPart cylinderHeadPart = _allParts.GetPart<CylinderHeadPart>(carTuningData.EngineKitData.CylinderHead);
			int modelId = carTuningData.EngineKitData.EngineId.ModelId;
			if (!cylinderHeadPart || cylinderHeadPart.CheckNotCompatibleEngine(modelId))
			{
				cylinderHeadPart = _allParts.GetCompatibleCylinderHeadFromEngine(modelId);
				carTuningData.EngineKitData.CylinderHead = cylinderHeadPart.CreateNewPartDataFromPart();
				partType = carTuningData.EngineKitData.CylinderHead.PartType;
				update = true;
			}
			if ((partType == PartType.OHV_CylinderHead || partType == PartType.SOHC_CylinderHead) && carTuningData.EngineKitData.Camshaft2 != null)
			{
				carTuningData.EngineKitData.Camshaft2 = null;
				update = true;
			}
			switch (partType)
			{
			case PartType.SOHC_CylinderHead:
			{
				SohcCylinderHeadInstance sohcCylinderHeadInstance = new SohcCylinderHeadInstance
				{
					Part = cylinderHeadPart,
					Mileage = carTuningData.EngineKitData.CylinderHead.GetMileage(cylinderHeadPart),
					InstanceId = carTuningData.EngineKitData.CylinderHead.InstanceID
				};
				PartData partData2 = carTuningData.EngineKitData.Camshaft1.CheckPartData();
				SOHC_Camshaft sOHC_Camshaft = _allParts.GetPart<SOHC_Camshaft>(partData2);
				if (!sOHC_Camshaft || sOHC_Camshaft.CheckNotCompatibleEngine(modelId))
				{
					sOHC_Camshaft = _allParts.GetCompatibleCamshaft(modelId, PartType.SOHC_Camshaft) as SOHC_Camshaft;
					carTuningData.EngineKitData.Camshaft1 = sOHC_Camshaft.CreateNewPartDataFromPart();
					partData2 = carTuningData.EngineKitData.Camshaft1;
					update = true;
				}
				sohcCylinderHeadInstance.Camshaft = new CamshaftInstance<SOHC_Camshaft>
				{
					Part = sOHC_Camshaft,
					InstanceId = partData2.InstanceID,
					Mileage = carTuningData.EngineKitData.Camshaft1.GetMileage(sOHC_Camshaft)
				};
				carEntityEngine.CylinderHead = sohcCylinderHeadInstance;
				break;
			}
			case PartType.DOHC_CylinderHead:
			{
				DohcCylinderHeadInstance dohcCylinderHeadInstance = new DohcCylinderHeadInstance
				{
					Part = cylinderHeadPart,
					Mileage = carTuningData.EngineKitData.CylinderHead.GetMileage(cylinderHeadPart),
					InstanceId = carTuningData.EngineKitData.CylinderHead.InstanceID
				};
				PartData partData3 = carTuningData.EngineKitData.Camshaft1.CheckPartData();
				DOHC_Camshaft_Intake dOHC_Camshaft_Intake = _allParts.GetPart<DOHC_Camshaft_Intake>(partData3);
				if (!dOHC_Camshaft_Intake || dOHC_Camshaft_Intake.CheckNotCompatibleEngine(modelId))
				{
					dOHC_Camshaft_Intake = _allParts.GetCompatibleCamshaft(modelId, PartType.DOHC_Camshaft_Intake) as DOHC_Camshaft_Intake;
					carTuningData.EngineKitData.Camshaft1 = dOHC_Camshaft_Intake.CreateNewPartDataFromPart();
					partData3 = carTuningData.EngineKitData.Camshaft1;
					update = true;
				}
				dohcCylinderHeadInstance.IntakeCamshaft = new CamshaftInstance<DOHC_Camshaft_Intake>
				{
					Part = dOHC_Camshaft_Intake,
					InstanceId = partData3.InstanceID,
					Mileage = carTuningData.EngineKitData.Camshaft1.GetMileage(dOHC_Camshaft_Intake)
				};
				PartData partData4 = null;
				DOHC_Camshaft_Exhaust dOHC_Camshaft_Exhaust = null;
				if (carTuningData.EngineKitData.Camshaft2 != null)
				{
					partData4 = carTuningData.EngineKitData.Camshaft2.CheckPartData();
					dOHC_Camshaft_Exhaust = _allParts.GetPart<DOHC_Camshaft_Exhaust>(partData4);
				}
				if (!dOHC_Camshaft_Exhaust || dOHC_Camshaft_Exhaust.CheckNotCompatibleEngine(modelId))
				{
					dOHC_Camshaft_Exhaust = _allParts.GetCompatibleCamshaft(modelId, PartType.DOHC_Camshaft_Exhaust) as DOHC_Camshaft_Exhaust;
					carTuningData.EngineKitData.Camshaft2 = dOHC_Camshaft_Exhaust.CreateNewPartDataFromPart();
					partData4 = carTuningData.EngineKitData.Camshaft2;
					update = true;
				}
				dohcCylinderHeadInstance.ExhaustCamshaft = new CamshaftInstance<DOHC_Camshaft_Exhaust>
				{
					Part = dOHC_Camshaft_Exhaust,
					InstanceId = partData4.InstanceID,
					Mileage = carTuningData.EngineKitData.Camshaft2.GetMileage(dOHC_Camshaft_Exhaust)
				};
				carEntityEngine.CylinderHead = dohcCylinderHeadInstance;
				break;
			}
			case PartType.OHV_CylinderHead:
			{
				OhvCylinderHeadInstance ohvCylinderHeadInstance = new OhvCylinderHeadInstance
				{
					Part = cylinderHeadPart,
					Mileage = carTuningData.EngineKitData.CylinderHead.GetMileage(cylinderHeadPart),
					InstanceId = carTuningData.EngineKitData.CylinderHead.InstanceID
				};
				PartData partData = carTuningData.EngineKitData.Camshaft1.CheckPartData();
				OHV_Camshaft oHV_Camshaft = _allParts.GetPart<OHV_Camshaft>(partData);
				if (!oHV_Camshaft)
				{
					oHV_Camshaft = _allParts.GetCompatibleCamshaft(modelId, PartType.OHV_Camshaft) as OHV_Camshaft;
					carTuningData.EngineKitData.Camshaft1 = oHV_Camshaft.CreateNewPartDataFromPart();
					partData = carTuningData.EngineKitData.Camshaft1;
					update = true;
				}
				ohvCylinderHeadInstance.Camshaft = new CamshaftInstance<OHV_Camshaft>
				{
					Part = oHV_Camshaft,
					InstanceId = partData.InstanceID,
					Mileage = carTuningData.EngineKitData.Camshaft1.GetMileage(oHV_Camshaft)
				};
				carEntityEngine.CylinderHead = ohvCylinderHeadInstance;
				break;
			}
			}
		}

		public void SetPartsInCar(CarInfo carInfo, params PartData[] partsData)
		{
			if (V.Car.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				carInfo.SaveCarMillage(component.CarEntity);
				CarTuningData tuningData = carInfo.TuningData;
				foreach (PartData partData in partsData)
				{
					tuningData = SetOnePart(partData, 0, carInfo.TuningData);
				}
				tuningData = CheckIncompatibleParts(tuningData);
				string text = partsData.Select((PartData x) => x.GetPartTypeName()).ToList().Beautify();
				MessageNotificationWithOtherSound?.Invoke(string.Concat(new LocalizedString("CarTuning.Partwasset:"), " ", text), 2f, SoundEventsDataHelper.InstallEngineDetailProcessSwitch);
				CarEntity carEntity = AssemblyCar(carInfo, checkNulls: true);
				if (carEntity != null)
				{
					component.CarEntity = carEntity;
					component.Init(initSuspension: false);
				}
				carInfo.Save();
				_databaseCarsService.SaveCurrentCar(saveNow: true);
			}
		}

		private CarTuningData CheckIncompatibleParts(CarTuningData tuningData)
		{
			int modelId = tuningData.EngineKitData.EngineId.ModelId;
			if (tuningData.ECU == null)
			{
				return tuningData;
			}
			PartItem part = tuningData.ECU.GetPart(_allParts);
			if (part == null)
			{
				return tuningData;
			}
			if (!part.IsEngineLinked(modelId))
			{
				_inventoryPartsService.Add(tuningData.ECU.GetCopy());
				tuningData.ECU = null;
			}
			return tuningData;
		}

		public void SetPartInCar(PartData partData, CarInfo carInfo, int index = 0)
		{
			if (V.Car.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				carInfo.SaveCarMillage(component.CarEntity);
				MessageNotification?.Invoke(string.Format("{0} {1}", new LocalizedString("CarTuning.Partwasset:"), partData.PartType), 2f);
				CarEntity carEntity = AssemblyCar(carInfo, checkNulls: true);
				if (carEntity != null)
				{
					component.CarEntity = carEntity;
					component.Init(initSuspension: false);
				}
				carInfo.Save();
				_databaseCarsService.SaveCurrentCar(saveNow: true);
			}
		}

		public void BuyPart(PartData partData)
		{
			_inventoryPartsService.Add(partData);
		}

		public List<PartType> GetEnginePartsNotCopmatibleToCar(EngineKitData engineKitData, int carId)
		{
			List<PartType> list = new List<PartType>();
			CheckPartNotCompatible(engineKitData.Camshaft1, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Camshaft2, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Block, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Crankshaft, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Charger, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Clutch, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Flywheel, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Intercooler, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Pistons, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.Radiator, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.AirFilter, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.ConnectionRod, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.CylinderHead, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.ExhaustHeader, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.ExhaustPipe, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.FuelInjectors, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.FuelPump, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.FuelRail, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.IntakeManifold, carId, engineKitData.EngineId.ModelId, list);
			CheckPartNotCompatible(engineKitData.ThrottleValve, carId, engineKitData.EngineId.ModelId, list);
			return list;
		}

		private void CheckPartNotCompatible(PartData partData, int carId, int engineMotelId, List<PartType> notCompatibleParts)
		{
			if (partData != null && _allParts.GetPart(partData).CheckNotCompatibleFull(carId, engineMotelId))
			{
				notCompatibleParts.Add(partData.PartType);
			}
		}

		public void SetNewEngineInCar(CarInfo carInfo, EngineKitData engineKitData, PartData ecu)
		{
			if (!V.Car.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				return;
			}
			if (carInfo.TuningData.ECU != null)
			{
				_inventoryPartsService.Add(carInfo.TuningData.ECU);
				_inventoryPartsService.Remove(carInfo.TuningData.ECU);
			}
			_inventoryPartsService.Add(carInfo.TuningData.ECU);
			carInfo.TuningData.ECU = ecu;
			_engineKitsInventoryService.Remove(engineKitData);
			EngineKitData engineKitData2 = carInfo.TuningData.EngineKitData;
			_engineKitsInventoryService.Add(engineKitData2);
			carInfo.TuningData.EngineKitData = engineKitData;
			CarEntity carEntity = AssemblyCar(carInfo, checkNulls: true);
			if (carEntity != null)
			{
				component.CarEntity = carEntity;
				FuelType getFuelType = component.CarEntity.Engine.FuelRail.GetFuelType;
				if (getFuelType != FuelType.None)
				{
					carInfo.TuningData.DynamicTuningSettings.FuelType = getFuelType;
				}
				SetMaintenanceData(carInfo, component);
				component.Init(initSuspension: false);
				ResetEngineValues(component.CarEntity.Engine);
			}
			carInfo.Save();
		}

		private void ResetEngineValues(EngineInstance engine)
		{
			engine.DynamicTuningSettings.IdleRpm = -1f;
			engine.DynamicTuningSettings.RedlineRpm = -1f;
			engine.DynamicTuningSettings.Boost = -1f;
			engine.DynamicTuningSettings.MixtureRatio = -1f;
		}

		private void SetMaintenanceData(CarInfo carInfo, VehicleEntityProvider vehicleEntityProvider)
		{
			float oilCapacity = ((carInfo.TuningData.EngineKitData.RotorHousing == null) ? new BlockResult(_allParts.GetPart<BlockPart>(carInfo.TuningData.EngineKitData.Block)).OilCapacity : new RotorHousingResult(_allParts.GetPart<RotorHousingPart>(carInfo.TuningData.EngineKitData.RotorHousing)).OilCapacity);
			carInfo.TuningData.Maintenance.OilCapacity = oilCapacity;
			carInfo.TuningData.Maintenance.OilLife = 100f;
			vehicleEntityProvider.CarEntity.MaintenanceInfo.OilCapacity = oilCapacity;
			vehicleEntityProvider.CarEntity.MaintenanceInfo.OilLife = 100f;
		}

		public EngineKitData SetOnePart(PartData partData, EngineKitData engineKitData, bool placeOldPartInInventory = true)
		{
			PartData item = null;
			switch (partData.PartType)
			{
			case PartType.EngineId:
				item = engineKitData.EngineId.GetCopy();
				engineKitData.EngineId = partData;
				break;
			case PartType.Block_Inline:
			case PartType.Block_Vee:
				item = engineKitData.Block.GetCopy();
				engineKitData.Block = partData;
				break;
			case PartType.Crankshaft:
				item = engineKitData.Crankshaft.GetCopy();
				engineKitData.Crankshaft = partData;
				break;
			case PartType.ConnectionRod:
				item = engineKitData.ConnectionRod.GetCopy();
				engineKitData.ConnectionRod = partData;
				break;
			case PartType.Pistons:
				item = engineKitData.Pistons.GetCopy();
				engineKitData.Pistons = partData;
				break;
			case PartType.Flywheel:
				item = engineKitData.Flywheel.GetCopy();
				engineKitData.Flywheel = partData;
				break;
			case PartType.SOHC_CylinderHead:
				item = engineKitData.CylinderHead.GetCopy();
				engineKitData.CylinderHead = partData;
				break;
			case PartType.DOHC_CylinderHead:
				item = engineKitData.CylinderHead.GetCopy();
				engineKitData.CylinderHead = partData;
				break;
			case PartType.OHV_CylinderHead:
				item = engineKitData.CylinderHead.GetCopy();
				engineKitData.CylinderHead = partData;
				break;
			case PartType.SOHC_Camshaft:
				item = engineKitData.Camshaft1.GetCopy();
				engineKitData.Camshaft1 = partData;
				break;
			case PartType.DOHC_Camshaft_Intake:
				item = engineKitData.Camshaft1.GetCopy();
				engineKitData.Camshaft1 = partData;
				break;
			case PartType.DOHC_Camshaft_Exhaust:
				item = engineKitData.Camshaft2.GetCopy();
				engineKitData.Camshaft2 = partData;
				break;
			case PartType.OHV_Camshaft:
				item = engineKitData.Camshaft1.GetCopy();
				engineKitData.Camshaft1 = partData;
				break;
			case PartType.IntakeManifold:
				item = engineKitData.IntakeManifold.GetCopy();
				engineKitData.IntakeManifold = partData;
				break;
			case PartType.AirFilter:
				item = engineKitData.AirFilter.GetCopy();
				engineKitData.AirFilter = partData;
				break;
			case PartType.FuelPump:
				item = engineKitData.FuelPump.GetCopy();
				engineKitData.FuelPump = partData;
				break;
			case PartType.FuelRail:
				item = engineKitData.FuelRail.GetCopy();
				engineKitData.FuelRail = partData;
				break;
			case PartType.FuelInjectors:
				item = engineKitData.FuelInjectors.GetCopy();
				engineKitData.FuelInjectors = partData;
				break;
			case PartType.ExhaustHeader:
				item = engineKitData.ExhaustHeader.GetCopy();
				engineKitData.ExhaustHeader = partData;
				break;
			case PartType.ExhaustPipe:
				item = engineKitData.ExhaustPipe.GetCopy();
				engineKitData.ExhaustPipe = partData;
				break;
			case PartType.ThrottleValve:
				item = engineKitData.ThrottleValve.GetCopy();
				engineKitData.ThrottleValve = partData;
				break;
			case PartType.Radiator:
				item = engineKitData.Radiator.GetCopy();
				engineKitData.Radiator = partData;
				break;
			case PartType.Intercooler:
				item = engineKitData.Intercooler.GetCopy();
				engineKitData.Intercooler = partData;
				break;
			case PartType.Clutch:
				item = engineKitData.Clutch.GetCopy();
				engineKitData.Clutch = partData;
				break;
			case PartType.TurboCharger:
				item = engineKitData.Charger.GetCopy();
				engineKitData.Charger = partData;
				break;
			case PartType.SuperCharger:
				item = engineKitData.Charger.GetCopy();
				engineKitData.Charger = partData;
				break;
			default:
				Debug.LogError("Cant place unknown part: " + partData.PartType);
				break;
			case PartType.None:
				break;
			}
			if (placeOldPartInInventory)
			{
				_inventoryPartsService.Add(item);
				_inventoryPartsService.Remove(partData);
			}
			return engineKitData;
		}

		public CarTuningData SetOnePart(PartData partData, int index, CarTuningData tuningData, bool placeOldPartInInventory = true)
		{
			PartData item = null;
			switch (partData.PartType)
			{
			case PartType.EngineId:
				item = tuningData.EngineKitData.EngineId.GetCopy();
				tuningData.EngineKitData.EngineId = partData;
				break;
			case PartType.Block_Inline:
			case PartType.Block_Vee:
				item = tuningData.EngineKitData.Block.GetCopy();
				tuningData.EngineKitData.Block = partData;
				break;
			case PartType.Crankshaft:
				item = tuningData.EngineKitData.Crankshaft.GetCopy();
				tuningData.EngineKitData.Crankshaft = partData;
				break;
			case PartType.ConnectionRod:
				item = tuningData.EngineKitData.ConnectionRod.GetCopy();
				tuningData.EngineKitData.ConnectionRod = partData;
				break;
			case PartType.Pistons:
				item = tuningData.EngineKitData.Pistons.GetCopy();
				tuningData.EngineKitData.Pistons = partData;
				break;
			case PartType.Flywheel:
				item = tuningData.EngineKitData.Flywheel.GetCopy();
				tuningData.EngineKitData.Flywheel = partData;
				break;
			case PartType.SOHC_CylinderHead:
				item = tuningData.EngineKitData.CylinderHead.GetCopy();
				tuningData.EngineKitData.CylinderHead = partData;
				break;
			case PartType.DOHC_CylinderHead:
				item = tuningData.EngineKitData.CylinderHead.GetCopy();
				tuningData.EngineKitData.CylinderHead = partData;
				break;
			case PartType.OHV_CylinderHead:
				item = tuningData.EngineKitData.CylinderHead.GetCopy();
				tuningData.EngineKitData.CylinderHead = partData;
				break;
			case PartType.SOHC_Camshaft:
				item = tuningData.EngineKitData.Camshaft1.GetCopy();
				tuningData.EngineKitData.Camshaft1 = partData;
				break;
			case PartType.DOHC_Camshaft_Intake:
				item = tuningData.EngineKitData.Camshaft1.GetCopy();
				tuningData.EngineKitData.Camshaft1 = partData;
				break;
			case PartType.DOHC_Camshaft_Exhaust:
				item = tuningData.EngineKitData.Camshaft2.GetCopy();
				tuningData.EngineKitData.Camshaft2 = partData;
				break;
			case PartType.OHV_Camshaft:
				item = tuningData.EngineKitData.Camshaft1.GetCopy();
				tuningData.EngineKitData.Camshaft1 = partData;
				break;
			case PartType.IntakeManifold:
				item = tuningData.EngineKitData.IntakeManifold.GetCopy();
				tuningData.EngineKitData.IntakeManifold = partData;
				break;
			case PartType.AirFilter:
				item = tuningData.EngineKitData.AirFilter.GetCopy();
				tuningData.EngineKitData.AirFilter = partData;
				break;
			case PartType.FuelPump:
				item = tuningData.EngineKitData.FuelPump.GetCopy();
				tuningData.EngineKitData.FuelPump = partData;
				break;
			case PartType.FuelRail:
				item = tuningData.EngineKitData.FuelRail.GetCopy();
				tuningData.EngineKitData.FuelRail = partData;
				break;
			case PartType.FuelInjectors:
				item = tuningData.EngineKitData.FuelInjectors.GetCopy();
				tuningData.EngineKitData.FuelInjectors = partData;
				break;
			case PartType.ExhaustHeader:
				item = tuningData.EngineKitData.ExhaustHeader.GetCopy();
				tuningData.EngineKitData.ExhaustHeader = partData;
				break;
			case PartType.ExhaustPipe:
				item = tuningData.EngineKitData.ExhaustPipe.GetCopy();
				tuningData.EngineKitData.ExhaustPipe = partData;
				break;
			case PartType.ThrottleValve:
				item = tuningData.EngineKitData.ThrottleValve.GetCopy();
				tuningData.EngineKitData.ThrottleValve = partData;
				break;
			case PartType.Radiator:
				item = tuningData.EngineKitData.Radiator.GetCopy();
				tuningData.EngineKitData.Radiator = partData;
				break;
			case PartType.Intercooler:
				item = tuningData.EngineKitData.Intercooler.GetCopy();
				tuningData.EngineKitData.Intercooler = ((partData.InstanceID == -1) ? null : partData);
				break;
			case PartType.GearBox:
				item = tuningData.GearBox.GetCopy();
				tuningData.GearBox = partData;
				break;
			case PartType.Clutch:
				item = tuningData.EngineKitData.Clutch.GetCopy();
				tuningData.EngineKitData.Clutch = partData;
				break;
			case PartType.ECU:
				item = tuningData.ECU.GetCopy();
				tuningData.ECU = partData;
				break;
			case PartType.TurboCharger:
				item = tuningData.EngineKitData.Charger.GetCopy();
				tuningData.EngineKitData.Charger = ((partData.InstanceID == -1) ? null : partData);
				break;
			case PartType.SuperCharger:
				item = tuningData.EngineKitData.Charger.GetCopy();
				tuningData.EngineKitData.Charger = ((partData.InstanceID == -1) ? null : partData);
				break;
			case PartType.FrontLSD:
				item = tuningData.FrontLSD.GetCopy();
				tuningData.FrontLSD = ((partData.InstanceID == -1) ? null : partData);
				break;
			case PartType.RearLSD:
				item = tuningData.RearLSD.GetCopy();
				tuningData.RearLSD = ((partData.InstanceID == -1) ? null : partData);
				break;
			case PartType.RotorHousing:
				item = tuningData.EngineKitData.RotorHousing.GetCopy();
				tuningData.EngineKitData.RotorHousing = partData;
				break;
			case PartType.Rotor:
				item = tuningData.EngineKitData.Rotors.GetCopy();
				tuningData.EngineKitData.Rotors = partData;
				break;
			case PartType.EccentricShaft:
				item = tuningData.EngineKitData.EccentricShaft.GetCopy();
				tuningData.EngineKitData.EccentricShaft = partData;
				break;
			case PartType.RotorTrackingGear:
				item = tuningData.EngineKitData.RotorTrackingGear.GetCopy();
				tuningData.EngineKitData.RotorTrackingGear = partData;
				break;
			default:
				Debug.LogError("Cant place unknown part: " + partData.PartType);
				break;
			case PartType.None:
				break;
			}
			if (placeOldPartInInventory)
			{
				_inventoryPartsService.Add(item);
				_inventoryPartsService.Remove(partData);
			}
			return tuningData;
		}
	}
}
