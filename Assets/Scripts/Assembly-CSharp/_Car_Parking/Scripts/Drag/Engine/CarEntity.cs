using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class CarEntity : PartInstanceBase, IReturnablePart<VehicleDataResult>
	{
		[Header("Maintenance")]
		public MaintenanceInfo MaintenanceInfo;

		private List<PartInstanceBase> _subParts;

		public string CarInstanceId;

		[field: SerializeField]
		public ObscuredInt PerformancePoints { get; private set; }

		[field: SerializeField]
		[field: Header("Engine Components")]
		public EngineInstance Engine { get; private set; }

		[field: SerializeField]
		public GearBoxInstance GearBox { get; private set; }

		[field: SerializeField]
		public FrontLSDInstance FrontLsd { get; private set; }

		[field: SerializeField]
		public RearLSDInstance RearLsd { get; private set; }

		[field: Header("Front Suspension")]
		[field: SerializeField]
		public FrontWheelData FrontWheel { get; private set; }

		[field: Header("Rear Suspension")]
		[field: SerializeField]
		public RearWheelData RearWheel { get; private set; }

		[field: Header("AeroDynamics")]
		[field: SerializeField]
		public ChassisData ChassisData { get; private set; }

		public override List<PartInstanceBase> SubParts
		{
			get
			{
				if (_subParts == null || _subParts.Count == 0)
				{
					_subParts = new List<PartInstanceBase> { Engine, GearBox, FrontLsd, RearLsd };
				}
				return _subParts;
			}
		}

		public GearBoxPart GetGearBox => GearBox.Part;

		public TransmissionValues GetGearboxValues
		{
			get
			{
				TransmissionValues value = GearBox.GetValue();
				int num = 0;
				if (Engine.DynamicTuningSettings.Gears.Length < value.GearRatio.Length - 2)
				{
					Engine.DynamicTuningSettings.Gears = Enumerable.Repeat(-1f, 14).ToArray();
				}
				for (int i = 2; i < value.GearRatio.Length; i++)
				{
					if (Engine.DynamicTuningSettings.Gears[num] > 0f)
					{
						value.GearRatio[i] = Engine.DynamicTuningSettings.Gears[num];
					}
					num++;
				}
				if (Engine.DynamicTuningSettings.TorqueSplit > 0f)
				{
					value.TorqueSplit = Engine.DynamicTuningSettings.TorqueSplit;
				}
				if (Engine.DynamicTuningSettings.FinalDrive > 0f)
				{
					value.FinalDrive = Engine.DynamicTuningSettings.FinalDrive;
				}
				if (Engine.DynamicTuningSettings.LaunchTime > 0f)
				{
					value.LaunchTime = Engine.DynamicTuningSettings.LaunchTime;
				}
				if (Engine.DynamicTuningSettings.LaunchRpm > 0f)
				{
					value.LaunchRpm = Engine.DynamicTuningSettings.LaunchRpm;
				}
				return value;
			}
		}

		public float GetMaxOilCapacity
		{
			get
			{
				if (Engine?.RotorHousing == null || !Engine.RotorHousing.Part)
				{
					if (!(Engine?.Block?.Part != null))
					{
						return 0f;
					}
					return Engine.Block.Part.OilCapacity;
				}
				return Engine.RotorHousing.GetParams.OilCapacity;
			}
		}

		public CarEntity(EngineInstance engine, GearBoxInstance gearBox, FrontLSDInstance frontLsd, RearLSDInstance rearLsd, FrontWheelData frontWheel, RearWheelData rearWheel, ChassisData chassisData)
		{
			Engine = engine;
			GearBox = gearBox;
			FrontLsd = frontLsd;
			RearLsd = rearLsd;
			FrontWheel = frontWheel;
			RearWheel = rearWheel;
			ChassisData = chassisData;
		}

		public void SetPerformance(AdvancedInt score)
		{
			score.Get(out int value);
			PerformancePoints = value;
		}

		public void GetOilData(out float capacity, out float oilLife)
		{
			capacity = MaintenanceInfo.OilCapacity;
			oilLife = Mathf.Clamp(MaintenanceInfo.OilLife, 0f, 100f);
		}

		public void ChangeOil(float oil)
		{
			MaintenanceInfo.OilCapacity = oil;
			MaintenanceInfo.OilLife = 100f;
		}

		public void SetNewFrontTyres()
		{
			MaintenanceInfo.FrontTyreHealth[0] = 1f;
			MaintenanceInfo.FrontTyreHealth[1] = 1f;
		}

		public void SetNewRearTyres()
		{
			MaintenanceInfo.RearTyreHealth[0] = 1f;
			MaintenanceInfo.RearTyreHealth[1] = 1f;
		}

		public void ResetGearBox()
		{
			Engine.DynamicTuningSettings.FinalDrive = -1f;
			Engine.DynamicTuningSettings.Gears = Enumerable.Repeat(-1f, 14).ToArray();
		}

		public VehicleDataResult GetValue()
		{
			EngineOutput value = Engine.GetValue();
			TransmissionValues getGearboxValues = GetGearboxValues;
			LSDValues value2 = FrontLsd.GetValue();
			LSDValues value3 = RearLsd.GetValue();
			VehicleDataResult result = new VehicleDataResult
			{
				EngineId = value.SoundId,
				CheckEngine = value.CheckEngine,
				IsBroken = value.IsBroken,
				Torque = value.Torque,
				Displacement = value.Displacement,
				IdleRpm = value.IdleRpm,
				RedlineRpm = value.RedlineRpm,
				MaxRpm = value.MaxRpm,
				EngineInertia = value.Inertia,
				EngineBrakeInertia = value.BrakeInertia,
				Weight = GetWeight(),
				TorqueSplit = getGearboxValues.TorqueSplit,
				Kind = getGearboxValues.Kind,
				AutoModes = getGearboxValues.AutoModes,
				LowMaxGear = getGearboxValues.LowMaxGear,
				GearRatios = getGearboxValues.GearRatio,
				EndRatio = getGearboxValues.FinalDrive,
				GearShiftDelay = value.ShiftDelay,
				FrontLsd = value2,
				RearLsd = value3,
				GearboxInertia = getGearboxValues.Inertia,
				MaxClutchF = value.MaxClutchF,
				HaveLc = getGearboxValues.LaunchControl,
				LcRpm = getGearboxValues.LaunchRpm,
				LcTime = getGearboxValues.LaunchTime,
				GearBoxType = (GearboxType)getGearboxValues.Type,
				TurboPressure = value.TurboPressure,
				MinTurboRpm = value.MinTurboRpm,
				MaxTurboRpm = value.MaxTurboRpm,
				TurboType = value.TurboType,
				ExhaustType = (((bool)Engine.ExhaustHeader.Part && Engine.ExhaustHeader.Part.Rarity > 1) ? 1 : 0),
				BurbleType = (Engine.ExhaustPipe.Part ? Engine.ExhaustPipe.Part.VendorId : 0)
			};
			if (Engine.Charger != null)
			{
				result.BlowOffType = (Engine.Charger.Part ? Engine.Charger.Part.VendorId : 0);
				result.SuperChargerType = (Engine.Charger.Part ? Engine.Charger.Part.Rarity : 0);
			}
			result.FuelConsumptionSpeed = value.FuelConsumptionSpeed;
			result.FuelConsumptionAtCostantSpeed = value.FuelConsumption;
			result.FrontWheel = FrontWheel;
			result.RearWheel = RearWheel;
			result.MaintenanceData = MaintenanceInfo;
			result.MaxSteeringAngle = Engine.DynamicTuningSettings.MaxSteeringAngle;
			result.CurrentFrontSuspensionHeight = Engine.DynamicTuningSettings.CurrentFrontSuspensionHeight;
			result.CurrentRearSuspensionHeight = Engine.DynamicTuningSettings.CurrentRearSuspensionHeight;
			result.MinFrontSuspensionHeight = Engine.DynamicTuningSettings.MinFrontSuspensionHeight;
			result.MaxFrontSuspensionHeight = Engine.DynamicTuningSettings.MaxFrontSuspensionHeight;
			result.MinRearSuspensionHeight = Engine.DynamicTuningSettings.MinRearSuspensionHeight;
			result.MaxRearSuspensionHeight = Engine.DynamicTuningSettings.MaxRearSuspensionHeight;
			result.HeightAdjustSpeed = Engine.DynamicTuningSettings.HeightAdjustSpeed;
			result.EngineType = Engine.GetEngineType;
			result.Points = PerformancePoints;
			return result;
		}

		public void RecalculateEngine(VehicleEntityProvider vehicleEntityProvider, ICarTuningFactory carTuningFactory, CarInfo carInfo)
		{
			VehicleDataResult value = (vehicleEntityProvider.CarEntity = carTuningFactory.AssemblyCar(carInfo)).GetValue();
			vehicleEntityProvider.ApplyEngineValue(value);
			carInfo.TuningData.PerformancePoints = PerformancePoints;
			CarInfoDeferredSaver.RequestSaveDebounced(carInfo);
		}

		public static VehicleDataResult RecalculateSuspension(VehicleEntityProvider vehicleEntityProvider, ICarTuningFactory carTuningFactory, CarInfo carInfo)
		{
			CarEntity carEntity = carTuningFactory.AssemblyCar(carInfo);
			vehicleEntityProvider.CarEntity = carEntity;
			VehicleDataResult value = vehicleEntityProvider.CarEntity.GetValue();
			vehicleEntityProvider.ApplySuspensionValue(value);
			carInfo.Save();
			return value;
		}

		public static VehicleDataResult RecalculateSuspensionWithoutSave(VehicleEntityProvider vehicleEntityProvider, ICarTuningFactory carTuningFactory, CarInfo carInfo)
		{
			CarEntity carEntity = carTuningFactory.AssemblyCar(carInfo);
			vehicleEntityProvider.CarEntity = carEntity;
			VehicleDataResult value = vehicleEntityProvider.CarEntity.GetValue();
			vehicleEntityProvider.ApplySuspensionValue(value);
			return value;
		}
	}
}
