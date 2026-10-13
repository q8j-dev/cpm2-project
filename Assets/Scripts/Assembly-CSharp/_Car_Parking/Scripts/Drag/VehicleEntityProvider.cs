using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CPM.Database;
using CPM.Transmission;
using CPMEngine;
using Car_Parking.Scripts.Drag.CarCompositions;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataEntities.Car;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.Instances;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag
{
	public class VehicleEntityProvider : MonoBehaviour
	{
		private const int W16 = 64;

		private const int IDChiron = 178;

		private const int IDVeyron = 27;

		private const float OilIdleBurnPerSec = 0.001f;

		private const float OilLoadBurnPerSec = 0.00833f;

		private const float OilRpmBurnPerSec = 0.004f;

		private const float OilOverrevBurnPerSec = 0.05f;

		private const float OilBurnBasePerSec = 6E-05f;

		private const float OilBurnLoadFactor = 0.8f;

		private const float OilBurnRpmFactor = 0.4f;

		private const float OilBurnWearMultiplier = 1.5f;

		private const float DryEngineSeizeSeconds = 60f;

		private const float OilBurnWearStart = 0.3f;

		private const float OilBurnOilLifeStart = 30f;

		private const float OilBurnIdleFactor = 0.05f;

		private const float MileageUpdateTimer = 5f;

		public bool IsMultiplayerCar;

		public CarEntity CarEntity;

		public InstallPartHandler OnPartChanged;

		private bool _isLvlCar;

		private ICarInfoService _carInfoService;

		private VehicleStockTuningData _getDefaultCarTuningDataConfig;

		private Powertrain _powertrain;

		private VehicleData _vehicleData;

		private Axles _axles;

		private SoundController _soundController;

		private CancellationTokenSource _cancellationTokenSourceApplySuspensionValue;

		private CancellationTokenSource _engineLoopsCts;

		private bool _wearDirtySinceRecalc;

		public bool IsAiCar { get; set; }

		public bool IsRallyCar { get; set; }

		public float AllWeight { get; private set; }

		public VehicleDataResult VehicleEngineResult { get; private set; }

		private Powertrain GetDriveTrain
		{
			get
			{
				if (!_powertrain)
				{
					TryGetComponent<Powertrain>(out _powertrain);
				}
				return _powertrain;
			}
		}

		public VehicleData GetVehicleData
		{
			get
			{
				if (!_vehicleData)
				{
					TryGetComponent<VehicleData>(out _vehicleData);
				}
				return _vehicleData;
			}
		}

		private SoundController GetSoundController
		{
			get
			{
				if (!_soundController)
				{
					TryGetComponent<SoundController>(out _soundController);
				}
				return _soundController;
			}
		}

		private Axles GetAxles
		{
			get
			{
				if (!_axles)
				{
					TryGetComponent<Axles>(out _axles);
				}
				return _axles;
			}
		}

		public bool IsLvlCar
		{
			set
			{
				_isLvlCar = value;
				if (_isLvlCar)
				{
					CommonExtensions.CancelAndDisposeToken(ref _engineLoopsCts);
				}
			}
		}

		public VehicleStockTuningData GetStockTuningData => _getDefaultCarTuningDataConfig;

		public float GetFuelInTank => CarEntity.MaintenanceInfo.FuelInTank;

		[Inject]
		public void Construct(ICarInfoService carInfoService)
		{
			_carInfoService = carInfoService;
		}

		public async UniTask SwapEngine(int id = -1, bool isOnlineCar = false)
		{
			TryFindDefaultConfig();
			if (!GetSoundController)
			{
				Debug.LogError($"SoundController is null:{base.gameObject}", base.gameObject);
				return;
			}
			if (!_getDefaultCarTuningDataConfig)
			{
				Debug.LogError($"DefaultCarTuningDataConfig is null:{base.gameObject}", base.gameObject);
				return;
			}
			EngineItem baseEngine = _getDefaultCarTuningDataConfig.Tuning.GetBaseEngine();
			int engineSound = baseEngine.EngineId.EngineSound;
			int carId = _getDefaultCarTuningDataConfig.Serial;
			int starterId = baseEngine.EngineId.StarterSound;
			if (isOnlineCar)
			{
				int id2 = ((id == engineSound) ? carId : id);
				GetSoundController.StarterId = starterId;
				await GetSoundController.SwapEngineSound(id2);
				return;
			}
			if (CarEntity?.Engine?.Part == null)
			{
				Debug.LogError($"CarEntity.Engine.Part is null:{base.gameObject}", base.gameObject);
				GetSoundController.StarterId = starterId;
				await GetSoundController.SwapEngineSound(carId);
				return;
			}
			try
			{
				int id2;
				if (id == -1)
				{
					int soundId = CarEntity.Engine.GetValue().SoundId;
					id2 = ((soundId == engineSound) ? carId : soundId);
				}
				else
				{
					id2 = id;
				}
				GetSoundController.StarterId = starterId;
				await GetSoundController.SwapEngineSound(id2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Set sound engine swap:" + ex);
				GetSoundController.StarterId = starterId;
				await GetSoundController.SwapEngineSound(carId);
			}
		}

		private void Start()
		{
			TryFindDefaultConfig();
			GetVehicleData.CarChildsReferences.FuelTank?.Setup(_getDefaultCarTuningDataConfig.Tuning.ChassisData.FuelTankCapacity);
			GetDriveTrain.InitTransferCase(_getDefaultCarTuningDataConfig.Tuning.TransferCase);
		}

		public void Refuel(float amount)
		{
			CarEntity.MaintenanceInfo.FuelInTank = amount;
			GetDriveTrain.CurrentFuel = amount;
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceApplySuspensionValue);
			CommonExtensions.CancelAndDisposeToken(ref _engineLoopsCts);
		}

		private void OnDisable()
		{
			TrySaveMileageOnDisable();
		}

		private void TrySaveMileageOnDisable()
		{
			if (!_vehicleData || !_vehicleData.mppc || !_vehicleData.mppc.isOnlineCar)
			{
				TrySaveMileage();
			}
		}

		public void UpdatePerformance()
		{
			TryFindDefaultConfig();
			Utils.CalculateUserRating(_getDefaultCarTuningDataConfig.Serial, base.gameObject, out var _, out var ps);
			AdvancedInt performance = new AdvancedInt(ps);
			CarEntity.SetPerformance(performance);
		}

		public void Init(bool initSuspension)
		{
			float num = 20f;
			SwapEngine().Forget();
			CommonExtensions.CancelAndDisposeToken(ref _engineLoopsCts);
			_engineLoopsCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			InitEngineLoop(num, fixedUpdate: false, _engineLoopsCts.Token).Forget();
			if (!_isLvlCar || !IsAiCar)
			{
				RecalculateLoop(num, _engineLoopsCts.Token).Forget();
			}
			AllWeight = CarEntity.GetWeight();
			VehicleEngineResult = CarEntity.GetValue();
			if (initSuspension)
			{
				ApplySuspensionValue(VehicleEngineResult);
			}
			ApplyEngineValue(VehicleEngineResult);
			GetDriveTrain.Gear = (GetDriveTrain.IsAutomaticGearbox ? 1 : 0);
			OnPartChanged?.Invoke(this);
		}

		public void CheckerEngineCheating(out int2 res)
		{
			TryFindDefaultConfig();
			EngineType getEngineType = _getDefaultCarTuningDataConfig.Tuning.ChassisData.GetEngineType;
			int num = VehicleEngineResult.EngineType;
			int num2 = _getDefaultCarTuningDataConfig.Serial;
			if (!_getDefaultCarTuningDataConfig.Tuning.ChassisData.MatchesRef(num2))
			{
				res = Utils.NotValid;
				return;
			}
			if (num == 64 && num2 != 27 && num2 != 178)
			{
				res = Utils.NotValid;
				return;
			}
			res = Utils.NotValid;
			if (getEngineType.HasFlag((EngineType)num))
			{
				res = Utils.Valid;
			}
		}

		public void InitOtherMultiplayerCar(VehicleDataResult vehicleDataEngineResult)
		{
			TryFindDefaultConfig();
			IsMultiplayerCar = true;
			ChassisData chassisData = _getDefaultCarTuningDataConfig.Tuning.ChassisData;
			EngineInstance engineInstance = new EngineInstance();
			engineInstance.SetEt(vehicleDataEngineResult.EngineType);
			CarEntity = new CarEntity(engineInstance, null, null, null, null, null, chassisData);
			CarEntity.MaintenanceInfo = vehicleDataEngineResult.MaintenanceData;
			ApplySuspensionValue(vehicleDataEngineResult);
			ApplyEngineValue(vehicleDataEngineResult);
			SwapEngine(vehicleDataEngineResult.EngineId, isOnlineCar: true).Forget();
		}

		public void InitMultiplayerCar(VehicleDataResult vehicleDataEngineResult)
		{
			VehicleEngineResult = vehicleDataEngineResult;
			SwapEngine().Forget();
			float num = 20f;
			CommonExtensions.CancelAndDisposeToken(ref _engineLoopsCts);
			_engineLoopsCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
			InitEngineLoop(num, fixedUpdate: true, _engineLoopsCts.Token).Forget();
			RecalculateLoop(num, _engineLoopsCts.Token).Forget();
			AllWeight = CarEntity.GetWeight();
			ApplySuspensionValue(vehicleDataEngineResult);
			ApplyEngineValue(vehicleDataEngineResult);
			GetDriveTrain.Gear = (GetDriveTrain.IsAutomaticGearbox ? 1 : 0);
			OnPartChanged?.Invoke(this);
		}

		private void TryFindDefaultConfig()
		{
			int carId = GetVehicleData.CarId;
			if (!_getDefaultCarTuningDataConfig || (int)_getDefaultCarTuningDataConfig.Serial != carId)
			{
				_getDefaultCarTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(carId);
			}
		}

		private void UpdateOilLife()
		{
			Powertrain getDriveTrain = GetDriveTrain;
			if ((bool)getDriveTrain && getDriveTrain.IsEngineWorks)
			{
				float num = Mathf.Max(1f, getDriveTrain.RedlineRpm);
				float num2 = Mathf.Clamp01(getDriveTrain.CurrentRpm / num);
				float num3 = Mathf.Clamp01(getDriveTrain.Load);
				float deltaTime = Time.deltaTime;
				float num4 = 0.00833f * num3 * num3;
				float num5 = 0.004f * num2 * num2;
				float num6 = Mathf.Max(0f, (getDriveTrain.CurrentRpm - num) / num);
				float num7 = 0.05f * num6;
				float num8 = (0.001f + num4 + num5 + num7) * deltaTime;
				if (!float.IsFinite(num8))
				{
					num8 = 0f;
				}
				float num9 = CarEntity.MaintenanceInfo.OilLife - num8;
				if (!float.IsFinite(num9))
				{
					num9 = 0f;
				}
				CarEntity.MaintenanceInfo.OilLife = Mathf.Max(0f, num9);
				float num10 = CarEntity.Engine?.Block?.Wear ?? (CarEntity.Engine?.RotorHousing?.Wear).GetValueOrDefault();
				float a = Mathf.InverseLerp(0.3f, 1f, num10);
				float value = Mathf.Clamp01(CarEntity.MaintenanceInfo.OilLife / 100f);
				float b = Mathf.InverseLerp(0.3f, 0f, value);
				float num11 = Mathf.Max(a, b);
				float num12 = 0.05f + 0.95f * num11;
				float num13 = 1f + num10 * 1.5f;
				float num14 = 1f + 0.8f * num3 * num3 + 0.4f * num2 * num2;
				float num15 = 6E-05f * num14 * num12 * num13 * deltaTime;
				if (!float.IsFinite(num15))
				{
					num15 = 0f;
				}
				float num16 = CarEntity.MaintenanceInfo.OilCapacity - num15;
				if (!float.IsFinite(num16))
				{
					num16 = 0f;
				}
				CarEntity.MaintenanceInfo.OilCapacity = Mathf.Max(0f, num16);
				if (CarEntity.MaintenanceInfo.OilCapacity <= 0f)
				{
					SeizeDryEngine(deltaTime);
				}
			}
		}

		private void SeizeDryEngine(float deltaTime)
		{
			EngineInstance engineInstance = CarEntity?.Engine;
			if (engineInstance != null)
			{
				float rate = deltaTime / 60f;
				if (engineInstance.Block != null)
				{
					AddDryWear(engineInstance.Block, rate);
					AddDryWear(engineInstance.Block.Crankshaft, rate);
					AddDryWear(engineInstance.Block.Pistons, rate);
					AddDryWear(engineInstance.Block.ConnectionRod, rate);
				}
				if (engineInstance.RotorHousing != null)
				{
					AddDryWear(engineInstance.RotorHousing, rate);
					AddDryWear(engineInstance.RotorHousing.Rotors, rate);
					AddDryWear(engineInstance.RotorHousing.EccentricShaft, rate);
					AddDryWear(engineInstance.RotorHousing.RotorTrackingGears, rate);
				}
			}
		}

		private static void AddDryWear<TPart>(PartInstance<TPart> inst, float rate) where TPart : PartItem
		{
			if ((bool)((inst != null) ? inst.Part : null))
			{
				inst.Mileage += inst.Part.MaxMileage * rate;
			}
		}

		private async UniTaskVoid InitEngineLoop(float skipSeconds, bool fixedUpdate, CancellationToken ct)
		{
			_ = 1;
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(skipSeconds), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				PlayerLoopTiming timing = (fixedUpdate ? PlayerLoopTiming.FixedUpdate : PlayerLoopTiming.Update);
				while (!ct.IsCancellationRequested)
				{
					InitEngine();
					await UniTask.Yield(timing, ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private async UniTaskVoid RecalculateLoop(float intervalSeconds, CancellationToken ct)
		{
			try
			{
				TimeSpan interval = TimeSpan.FromSeconds(intervalSeconds);
				while (!ct.IsCancellationRequested)
				{
					await UniTask.Delay(interval, ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					RecalculateCar();
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void RecalculateCar()
		{
			if (CarEntity != null && !IsRallyCar && _wearDirtySinceRecalc)
			{
				_wearDirtySinceRecalc = false;
				ApplyEngineValue(CarEntity.GetValue());
			}
		}

		private void InitEngine()
		{
			if (!(base.gameObject != V.Car) && !FlagsStorage.OutOfCar)
			{
				Pv value = default;
				Powertrain getDriveTrain = GetDriveTrain;
				if ((bool)getDriveTrain)
				{
					value.P = getDriveTrain.Mileage * Time.deltaTime;
					value.I = Mathf.Abs(getDriveTrain.EngineCurTorque);
					getDriveTrain.GetEngineTurboParams(out var _, out var curTurboPressure, out var _, out var _);
					value.D = Mathf.RoundToInt(Utils.Bar2Pa(curTurboPressure));
					value.R = getDriveTrain.CurrentRpm;
				}
				if (CarEntity != null)
				{
					float getMaxOilCapacity = CarEntity.GetMaxOilCapacity;
					value.O = ((getMaxOilCapacity > 0f) ? Mathf.Clamp01(CarEntity.MaintenanceInfo.OilCapacity / getMaxOilCapacity) : 0f);
				}
				CarEntity?.UpdateWear(value);
				CarEntity?.UpdateLoad(value);
				UpdateOilLife();
				_wearDirtySinceRecalc = true;
			}
		}

		public void SetFuelInTank(float value)
		{
			CarEntity.MaintenanceInfo.FuelInTank = value;
		}

		public void ApplyEngineValue(VehicleDataResult result)
		{
			TryFindDefaultConfig();
			ChassisData chassisData = _getDefaultCarTuningDataConfig.Tuning.ChassisData;
			GetDriveTrain.GetDynoData(out var desc);
			float num = (_powertrain ? _powertrain.CurrentFuel : CarEntity.MaintenanceInfo.FuelInTank) * 0.73722f;
			float weight = result.Weight;
			float num2 = num + weight;
			desc.Chassis.Mass = chassisData.Weight + num2;
			desc.Chassis.Length = chassisData.Length;
			desc.Chassis.Width = chassisData.Width;
			desc.Chassis.Height = chassisData.Height;
			desc.Chassis.UpPercent = chassisData.WeightDistributionVertical;
			desc.Chassis.FrontPercent = chassisData.WeightDistribution;
			desc.Aero.Cx = chassisData.Cx;
			desc.Aero.Sx = chassisData.Width * chassisData.Height * 0.84f;
			desc.Aero.FrontDownforce = chassisData.FrontDownforce;
			desc.Aero.RearDownforce = chassisData.RearDownforce;
			if (result.Torque.Length != 0)
			{
				desc.Engine.TorqueCurveSamples = result.Torque;
				Utils.GetEngineParams(result.Torque, result.TurboType, Utils.Pa2Bar(result.TurboPressure), out desc.Engine.MaxTorque, out desc.Engine.MaxTorqueRpm);
			}
			desc.Engine.IdleRpm = result.IdleRpm;
			desc.Engine.RedlineRpm = result.RedlineRpm;
			desc.Engine.MaxRpm = result.MaxRpm;
			desc.Engine.InertiaRatio = result.EngineInertia;
			desc.Engine.Displacement = result.Displacement;
			desc.Engine.IsRotary = ((int)result.EngineType & 0x400) != 0;
			desc.GearBox.Gears = result.GearRatios;
			desc.Engine.IsBroken = result.IsBroken;
			desc.Engine.FuelConsumptionAtCostantSpeed = result.FuelConsumptionAtCostantSpeed;
			desc.Engine.FuelConsumptionSpeed = result.FuelConsumptionSpeed;
			desc.Engine.Options.Lc = result.HaveLc;
			desc.Engine.LcRpm = result.LcRpm;
			desc.Engine.LcTime = result.LcTime;
			desc.Engine.Orientation = chassisData.Orientation;
			desc.GearBox.FinalDrive = result.EndRatio;
			desc.GearBox.InertiaRatio = result.GearboxInertia;
			desc.Transmission.GearType = result.GearBoxType;
			GearboxKind gearboxKind = ((!GetVehicleData || !GetVehicleData.CarValues) ? GearboxKind.AT : GetVehicleData.CarValues.Gearbox);
			desc.Transmission.Kind = (IsAiCar ? GearboxKind.AT : (IsRallyCar ? gearboxKind : result.Kind));
			desc.Transmission.AutoModes = result.AutoModes;
			desc.Transmission.LowMaxGear = result.LowMaxGear;
			desc.Transmission.TorqueSplit = result.TorqueSplit;
			int2 int5 = new int2(50, 667);
			CheckerEngineCheating(out var res);
			if (math.all(res == int5))
			{
				GetDriveTrain.SetCheckEngine(value: true, sendRpc: true);
				desc.Engine.TurboPressure = 0;
				desc.Engine.TurboRange = Vector2.zero;
				desc.Engine.TurboType = TurboType.NA;
				desc.GearBox.GearShiftingDelay = 1f;
				desc.Transmission.ClutchMaxTorque = 250f;
			}
			else
			{
				GetDriveTrain.SetCheckEngine(result.CheckEngine, sendRpc: true);
				GetDriveTrain.IsEngineBroken = result.IsBroken;
				desc.Engine.TurboPressure = result.TurboPressure;
				desc.Engine.TurboRange = new float2(result.MinTurboRpm, result.MaxTurboRpm);
				desc.Engine.TurboType = result.TurboType;
				desc.GearBox.GearShiftingDelay = result.GearShiftDelay;
				desc.Transmission.ClutchMaxTorque = result.MaxClutchF;
			}
			desc.Lsd.FrontLSD.Type = result.FrontLsd.Type;
			desc.Lsd.FrontLSD.AccelBias = result.FrontLsd.AccelBias;
			desc.Lsd.FrontLSD.BrakeBias = result.FrontLsd.BrakeBias;
			desc.Lsd.FrontLSD.MaxBias = result.FrontLsd.MaxBias;
			desc.Lsd.FrontLSD.TorqueBiasRatio = result.FrontLsd.TorqueBiasRatio;
			desc.Lsd.FrontLSD.LockPercent = result.FrontLsd.LockPercent;
			desc.Lsd.RearLSD.Type = result.RearLsd.Type;
			desc.Lsd.RearLSD.AccelBias = result.RearLsd.AccelBias;
			desc.Lsd.RearLSD.BrakeBias = result.RearLsd.BrakeBias;
			desc.Lsd.RearLSD.MaxBias = result.RearLsd.MaxBias;
			desc.Lsd.RearLSD.TorqueBiasRatio = result.RearLsd.TorqueBiasRatio;
			desc.Lsd.RearLSD.LockPercent = result.RearLsd.LockPercent;
			GetDriveTrain.SetDynoData(desc, apply: false);
			GetDriveTrain.ApplyEngineData();
			if ((bool)GetSoundController)
			{
				_soundController.HasExhaustShot = result.ExhaustType > 0;
				_soundController.HasBurble = result.BurbleType > 0;
				_soundController.BurbleType = result.BurbleType;
				_soundController.BlowOffType = result.BlowOffType;
				_soundController.SuperChargerType = result.SuperChargerType;
				_soundController.RefreshExhaustBank();
				UpdatePerformance();
			}
		}

		public void ApplySuspensionValue(VehicleDataResult result)
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceApplySuspensionValue);
			_cancellationTokenSourceApplySuspensionValue = new CancellationTokenSource();
			ApplySuspensionValueAsync(result, _cancellationTokenSourceApplySuspensionValue.Token).Forget();
		}

		private async UniTaskVoid ApplySuspensionValueAsync(VehicleDataResult result, CancellationToken cancellationToken)
		{
			TryFindDefaultConfig();
			CarInfo carInfo = _vehicleData.carInfo;
			ChassisData chassisData = _getDefaultCarTuningDataConfig.Tuning.ChassisData;
			SuspensionData suspensionData = _getDefaultCarTuningDataConfig.Tuning.FrontWheelData.SuspensionData;
			SuspensionData suspensionData2 = _getDefaultCarTuningDataConfig.Tuning.RearWheelData.SuspensionData;
			TuningLimitsData frontWheelDataLimits = _getDefaultCarTuningDataConfig.Tuning.FrontWheelDataLimits;
			TuningLimitsData rearWheelDataLimits = _getDefaultCarTuningDataConfig.Tuning.RearWheelDataLimits;
			GetDriveTrain.GetDynoData(out var desc);
			float num = (_powertrain ? _powertrain.CurrentFuel : CarEntity.MaintenanceInfo.FuelInTank) * 0.73722f;
			desc.Chassis.Mass = chassisData.Weight + num + result.Weight;
			desc.Chassis.FrontPercent = chassisData.WeightDistribution;
			desc.Chassis.BrakeBalance = (int)chassisData.BrakeBalance;
			int maxSteerAngle = result.FrontWheel.SuspensionData.MaxSteerAngle;
			desc.FrontSuspension.MaxSteerAngle = Mathf.Clamp(maxSteerAngle, frontWheelDataLimits.SteerAngleMin, frontWheelDataLimits.SteerAngleMax);
			desc.FrontSuspension.Caster = Mathf.Clamp(result.FrontWheel.SuspensionData.Caster, frontWheelDataLimits.CasterMin, frontWheelDataLimits.CasterMax);
			desc.FrontSuspension.Ackerman = Mathf.Clamp(result.FrontWheel.SuspensionData.Ackerman, frontWheelDataLimits.AckermanMin, frontWheelDataLimits.AckermanMax);
			result.FrontWheel.SuspensionData.GetSpringParams(out var specs);
			specs = ClampSuspensionPair(specs, Utils.Inch2Meter(frontWheelDataLimits.SpringLengthMin), Utils.Inch2Meter(frontWheelDataLimits.SpringLengthMax), Utils.Kgfmm2NM(frontWheelDataLimits.SpringRateMin), Utils.Kgfmm2NM(frontWheelDataLimits.SpringRateMax), "front", "spring", "SpringLength", "SpringRate");
			desc.FrontSuspension.SpringLength = specs.x;
			desc.FrontSuspension.Stiffness = specs.y;
			float sag = AirSuspensionMath.Sag(AirSuspensionMath.CornerLoad(desc.Chassis.Mass, desc.Chassis.FrontPercent, front: true), specs.y);
			float z = ReadAirSuspensionRange(carInfo, specs.x, sag, isFront: true, result.MinFrontSuspensionHeight, result.MaxFrontSuspensionHeight, result.CurrentFrontSuspensionHeight, desc.FrontSuspension.PneumaticBagSystem, Utils.Inch2Meter(frontWheelDataLimits.SpringLengthMin), Utils.Inch2Meter(frontWheelDataLimits.SpringLengthMax));
			float2 float5 = SpringLenLimits(desc.FrontSuspension.PneumaticBagSystem, specs.x);
			float3 specs2 = new float3(specs.x, specs.y, z);
			desc.FrontSuspension.PneumaticBagSystem.HeightAdjustSpeed = result.HeightAdjustSpeed;
			ApplyShockAbsorbers(result, suspensionData, suspensionData2, frontWheelDataLimits, rearWheelDataLimits, desc, out desc);
			bool num2 = result.FrontWheel.BrakesData.ClampDiameterToWheelSize((int)result.FrontWheel.wheelSize) | result.FrontWheel.BrakesData.ClampToDiscLimits();
			desc.FrontSuspension.BrakeDiskTorque = result.FrontWheel.BrakesData.GetFullBrakeTorque;
			desc.FrontSuspension.BrakeDiscType = result.FrontWheel.BrakesData.GetBrakeDiscType;
			desc.FrontSuspension.BrakeDiskMass = result.FrontWheel.BrakesData.GetMass;
			float pressure = _getDefaultCarTuningDataConfig.Tuning.FrontWheelData.Pressure;
			desc.FrontTyre.Pressure = Utils.Bar2Pa(Mathf.Clamp(result.FrontWheel.Pressure, frontWheelDataLimits.GetMinWheelPressure(result.FrontWheel.tyreType), frontWheelDataLimits.GetMaxWheelPressure(pressure, result.FrontWheel.tyreType)));
			desc.FrontTyre.DiscDiam = result.FrontWheel.wheelSize;
			desc.FrontTyre.DiscWidth = result.FrontWheel.rimWidth;
			desc.FrontTyre.Width = result.FrontWheel.tyreWidth;
			desc.FrontTyre.Profile = result.FrontWheel.tyreProfile;
			desc.FrontTyre.TyreType = result.FrontWheel.tyreType;
			desc.FrontTyre.Mass = _powertrain.CalculateWheelMass(result.FrontWheel);
			desc.FrontSuspension.AntirollBar = result.FrontWheel.SuspensionData.AntiSwayBarForce;
			desc.FrontSuspension.Camber = Mathf.Clamp(result.FrontWheel.SuspensionData.Camber, frontWheelDataLimits.CamberMin, frontWheelDataLimits.CamberMax);
			desc.FrontSuspension.ToeIn = Mathf.Clamp(result.FrontWheel.SuspensionData.ToeIn, frontWheelDataLimits.ToeMin, frontWheelDataLimits.ToeMax);
			desc.FrontSuspension.SuspensionType = _getDefaultCarTuningDataConfig.Tuning.FrontWheelData.SuspensionData.SuspensionType;
			desc.FrontSuspension.Spacers = result.FrontWheel.SpacerWidth;
			bool flag = result.RearWheel.BrakesData.ClampDiameterToWheelSize((int)result.RearWheel.wheelSize);
			flag |= result.RearWheel.BrakesData.ClampToDiscLimits();
			desc.RearSuspension.BrakeDiskTorque = result.RearWheel.BrakesData.GetFullBrakeTorque;
			desc.RearSuspension.BrakeDiscType = result.RearWheel.BrakesData.GetBrakeDiscType;
			desc.RearSuspension.BrakeDiskMass = result.RearWheel.BrakesData.GetMass;
			if ((num2 | flag) && carInfo != null)
			{
				carInfo.TuningData.FrontWheelData.BrakesData = result.FrontWheel.BrakesData;
				carInfo.TuningData.RearWheelData.BrakesData = result.RearWheel.BrakesData;
				CarInfoDeferredSaver.RequestSave(carInfo);
			}
			float pressure2 = _getDefaultCarTuningDataConfig.Tuning.RearWheelData.Pressure;
			desc.RearTyre.Pressure = Utils.Bar2Pa(Mathf.Clamp(result.RearWheel.Pressure, rearWheelDataLimits.GetMinWheelPressure(result.RearWheel.tyreType), rearWheelDataLimits.GetMaxWheelPressure(pressure2, result.RearWheel.tyreType)));
			desc.RearTyre.DiscDiam = result.RearWheel.wheelSize;
			desc.RearTyre.DiscWidth = result.RearWheel.rimWidth;
			desc.RearTyre.Width = result.RearWheel.tyreWidth;
			desc.RearTyre.Profile = result.RearWheel.tyreProfile;
			desc.RearTyre.TyreType = result.RearWheel.tyreType;
			desc.RearTyre.Mass = _powertrain.CalculateWheelMass(result.RearWheel);
			desc.RearSuspension.AntirollBar = result.RearWheel.SuspensionData.AntiSwayBarForce;
			desc.RearSuspension.Camber = Mathf.Clamp(result.RearWheel.SuspensionData.Camber, rearWheelDataLimits.CamberMin, rearWheelDataLimits.CamberMax);
			desc.RearSuspension.ToeIn = Mathf.Clamp(result.RearWheel.SuspensionData.ToeIn, rearWheelDataLimits.ToeMin, rearWheelDataLimits.ToeMax);
			desc.RearSuspension.SuspensionType = _getDefaultCarTuningDataConfig.Tuning.RearWheelData.SuspensionData.SuspensionType;
			desc.RearSuspension.Spacers = result.RearWheel.SpacerWidth;
			result.RearWheel.SuspensionData.GetSpringParams(out specs);
			specs = ClampSuspensionPair(specs, Utils.Inch2Meter(rearWheelDataLimits.SpringLengthMin), Utils.Inch2Meter(rearWheelDataLimits.SpringLengthMax), Utils.Kgfmm2NM(rearWheelDataLimits.SpringRateMin), Utils.Kgfmm2NM(rearWheelDataLimits.SpringRateMax), "rear", "spring", "SpringLength", "SpringRate");
			desc.RearSuspension.SpringLength = specs.x;
			desc.RearSuspension.Stiffness = specs.y;
			float sag2 = AirSuspensionMath.Sag(AirSuspensionMath.CornerLoad(desc.Chassis.Mass, desc.Chassis.FrontPercent, front: false), specs.y);
			float z2 = ReadAirSuspensionRange(carInfo, specs.x, sag2, isFront: false, result.MinRearSuspensionHeight, result.MaxRearSuspensionHeight, result.CurrentRearSuspensionHeight, desc.RearSuspension.PneumaticBagSystem, Utils.Inch2Meter(rearWheelDataLimits.SpringLengthMin), Utils.Inch2Meter(rearWheelDataLimits.SpringLengthMax));
			float2 float6 = SpringLenLimits(desc.RearSuspension.PneumaticBagSystem, specs.x);
			float3 specs3 = new float3(specs.x, specs.y, z2);
			desc.RearSuspension.PneumaticBagSystem.HeightAdjustSpeed = result.HeightAdjustSpeed;
			desc.FrontTyre.FrictionMultiplier = 1.2f;
			desc.RearTyre.FrictionMultiplier = 1.2f;
			if (!GetAxles)
			{
				return;
			}
			GetAxles.FrontAxle.SetSpring(specs2, sag, float5.x, float5.y);
			_axles.RearAxle.SetSpring(specs3, sag2, float6.x, float6.y);
			_axles.SetTyreHealth(_axles.FrontAxle, result.MaintenanceData.FrontTyreHealth, result.FrontWheel.tyreType);
			_axles.SetTyreHealth(_axles.RearAxle, result.MaintenanceData.RearTyreHealth, result.RearWheel.tyreType);
			if (_axles.Rear1Axle.Wheels.Count > 0)
			{
				_axles.SetTyreHealth(_axles.Rear1Axle, result.MaintenanceData.RearTyreHealth, result.RearWheel.tyreType);
				_axles.Rear1Axle.SetSpring(specs3, sag2, float6.x, float6.y);
			}
			if (_axles.Rear2Axle.Wheels.Count > 0)
			{
				_axles.SetTyreHealth(_axles.Rear2Axle, result.MaintenanceData.RearTyreHealth, result.RearWheel.tyreType);
				_axles.Rear2Axle.SetSpring(specs3, sag2, float6.x, float6.y);
			}
			GetDriveTrain.SetDynoData(desc, apply: false);
			GetDriveTrain.ApplySuspensionData();
			bool mpCar = TryGetComponent<MultiPlayerPosControl>(out var component) && component.isOnlineCar;
			foreach (WheelProvider whl in _axles.FrontAxle.Wheels)
			{
				if (mpCar)
				{
					await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
				if ((bool)whl.WheelModel)
				{
					whl.WheelModel.BrakeDisk.Resize(result.FrontWheel.BrakesData.GetBrakeDiskDiameter());
				}
			}
			foreach (WheelProvider whl in _axles.RearAxle.Wheels)
			{
				if (mpCar)
				{
					await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
				if ((bool)whl.WheelModel)
				{
					whl.WheelModel.BrakeDisk.Resize(result.RearWheel.BrakesData.GetBrakeDiskDiameter());
				}
			}
			foreach (WheelProvider whl in _axles.Rear1Axle.Wheels)
			{
				if (mpCar)
				{
					await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
				if ((bool)whl.WheelModel)
				{
					whl.WheelModel.BrakeDisk.Resize(result.RearWheel.BrakesData.GetBrakeDiskDiameter());
				}
			}
			foreach (WheelProvider whl in _axles.Rear2Axle.Wheels)
			{
				if (mpCar)
				{
					await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
				if ((bool)whl.WheelModel)
				{
					whl.WheelModel.BrakeDisk.Resize(result.RearWheel.BrakesData.GetBrakeDiskDiameter());
				}
			}
		}

		private static float2 SpringLenLimits(DynoData.PneumaticBagSystem bag, float fittedSpringLen)
		{
			return AirSuspensionMath.ResolveSpringLenLimits(bag.MinSpringLen, bag.MaxSpringLen, fittedSpringLen);
		}

		private static float ReadAirSuspensionRange(CarInfo carInfo, float springLength, float sag, bool isFront, float storedMin, float storedMax, float storedCurrent, DynoData.PneumaticBagSystem bag, float minSpringLenLimit, float maxSpringLenLimit)
		{
			float num = AirSuspensionMath.ReadStoredLimitMm(storedMax, sag, springLength);
			float num2 = AirSuspensionMath.ReadStoredSetpointMm(storedCurrent, sag);
			bag.Sag = sag;
			bag.SuspensionHeight = new float2(-1f, (num < 0f) ? (-1f) : (num * 0.001f));
			bag.MinSpringLen = minSpringLenLimit;
			bag.MaxSpringLen = maxSpringLenLimit;
			float2 float5 = SpringLenLimits(bag, springLength);
			float2 float6 = AirSuspensionMath.EffectiveLimits(float5.x, float5.y, sag, bag.SuspensionHeight);
			float value = AirSuspensionMath.SpringLenToClearance(springLength, sag);
			bag.TargetHeight = ((num2 < 0f) ? Mathf.Clamp(value, float6.x, float6.y) : Mathf.Clamp(num2 * 0.001f, float6.x, float6.y));
			bag.CurrentSpringLength = AirSuspensionMath.ClearanceToSpringLen(bag.TargetHeight, sag, float5.y);
			if (carInfo != null)
			{
				DynamicTuningSettings dynamicTuningSettings = carInfo.TuningData.DynamicTuningSettings;
				bool flag = storedMin >= 0f;
				bool flag2 = IsLegacyMetres(storedMax) || (storedMax >= 0f && num < 0f);
				if (isFront)
				{
					if (flag)
					{
						dynamicTuningSettings.MinFrontSuspensionHeight = -1f;
					}
					if (flag2)
					{
						dynamicTuningSettings.MaxFrontSuspensionHeight = num;
					}
					if (IsLegacyMetres(storedCurrent))
					{
						dynamicTuningSettings.CurrentFrontSuspensionHeight = num2;
					}
				}
				else
				{
					if (flag)
					{
						dynamicTuningSettings.MinRearSuspensionHeight = -1f;
					}
					if (flag2)
					{
						dynamicTuningSettings.MaxRearSuspensionHeight = num;
					}
					if (IsLegacyMetres(storedCurrent))
					{
						dynamicTuningSettings.CurrentRearSuspensionHeight = num2;
					}
				}
			}
			return bag.TargetHeight;
		}

		private static bool IsLegacyMetres(float stored)
		{
			if (stored > 0f)
			{
				return stored < 1f;
			}
			return false;
		}

		public void CheckerPartsInEngine(out string partsLine, out int2 isIncompatible)
		{
			if (CarEntity?.Engine == null)
			{
				partsLine = new LocalizedString("VehicleEntityProvider.EngineIsNotInstalled");
				isIncompatible = Utils.NotValid;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int carId = _getDefaultCarTuningDataConfig.Serial;
			int modelId = CarEntity.Engine.Part.ModelId;
			isIncompatible = Utils.NotValid;
			List<PartType> list = new List<PartType>();
			List<PartItem> list2;
			if ((bool)CarEntity.Engine.RotorHousing?.Part)
			{
				list2 = new List<PartItem>
				{
					CarEntity.Engine.RotorHousing?.Part,
					CarEntity.Engine.RotorHousing?.Rotors?.Part,
					CarEntity.Engine.RotorHousing?.RotorTrackingGears?.Part,
					CarEntity.Engine.RotorHousing?.EccentricShaft?.Part,
					CarEntity.Engine.RotorHousing?.Flywheel?.Part
				};
			}
			else
			{
				list2 = new List<PartItem>
				{
					CarEntity.Engine.Block?.Part,
					CarEntity.Engine.Block?.Crankshaft?.Part,
					CarEntity.Engine.Block?.Flywheel?.Part,
					CarEntity.Engine.Block?.Pistons?.Part,
					CarEntity.Engine.Block?.ConnectionRod?.Part,
					CarEntity.Engine.CylinderHead?.Part
				};
				CylinderHeadInstance cylinderHead = CarEntity.Engine.CylinderHead;
				if (!(cylinderHead is DohcCylinderHeadInstance dohcCylinderHeadInstance))
				{
					if (!(cylinderHead is SohcCylinderHeadInstance sohcCylinderHeadInstance))
					{
						if (cylinderHead is OhvCylinderHeadInstance ohvCylinderHeadInstance)
						{
							list2.Add(ohvCylinderHeadInstance.Camshaft?.Part);
						}
					}
					else
					{
						list2.Add(sohcCylinderHeadInstance.Camshaft?.Part);
					}
				}
				else
				{
					list2.Add(dohcCylinderHeadInstance.IntakeCamshaft?.Part);
					list2.Add(dohcCylinderHeadInstance.ExhaustCamshaft?.Part);
				}
			}
			list2.Add(CarEntity.Engine.Charger?.Part);
			list2.Add(CarEntity.Engine.Clutch?.Part);
			list2.Add(CarEntity.Engine.Intercooler?.Part);
			list2.Add(CarEntity.Engine.Radiator?.Part);
			list2.Add(CarEntity.Engine.ExhaustHeader?.Part);
			list2.Add(CarEntity.Engine.ExhaustPipe?.Part);
			list2.Add(CarEntity.Engine.FuelInjectors?.Part);
			list2.Add(CarEntity.Engine.FuelPump?.Part);
			list2.Add(CarEntity.Engine.IntakeManifold?.Part);
			list2.Add(CarEntity.Engine.ECU?.Part);
			foreach (PartItem item in list2)
			{
				if ((bool)item)
				{
					CheckPartNotCompatible(item, carId, modelId, list, out var incompartiblePart);
					if (!string.IsNullOrEmpty(incompartiblePart))
					{
						stringBuilder.Append(incompartiblePart + ",");
					}
				}
			}
			if (CarEntity?.GearBox == null)
			{
				partsLine = new LocalizedString("VehicleEntityProvider.GearboxNotInstalled");
				isIncompatible = Utils.NotValid;
				return;
			}
			GearboxType getGearboxType = _getDefaultCarTuningDataConfig.Tuning.ChassisData.GetGearboxType;
			GearboxType type = (GearboxType)CarEntity.GearBox.GetValue().Type;
			if ((bool)CarEntity.GearBox?.Part && !getGearboxType.HasFlag(type))
			{
				if (!string.IsNullOrEmpty(CarEntity.GearBox?.Part.GetPartTypeName()))
				{
					stringBuilder.Append(CarEntity.GearBox?.Part.GetPartTypeName() + ",");
				}
				list.Add(PartType.GearBox);
			}
			if (stringBuilder.Length > 0)
			{
				if (stringBuilder[stringBuilder.Length - 1] == ',')
				{
					stringBuilder.Length--;
				}
			}
			partsLine = stringBuilder.ToString();
			isIncompatible = ((list.Count > 0) ? Utils.NotValid : Utils.Valid);
		}

		private void CheckPartNotCompatible(PartItem part, int carId, int engineMotelId, List<PartType> notCompatibleParts, out string incompartiblePart)
		{
			incompartiblePart = string.Empty;
			if ((bool)part && part.CheckNotCompatibleFull(carId, engineMotelId))
			{
				incompartiblePart = part.GetPartTypeName();
				notCompatibleParts.Add(part.TypeId);
			}
		}

		public void UpdateSuspensionState(SuspensionState state)
		{
			CarInfo carInfo = _vehicleData.carInfo;
			if (carInfo != null)
			{
				carInfo.TuningData.FrontWheelData.SuspensionData.SuspensionState = state;
				carInfo.TuningData.RearWheelData.SuspensionData.SuspensionState = state;
			}
			CarEntity.FrontWheel.SuspensionData.SuspensionState = state;
			CarEntity.RearWheel.SuspensionData.SuspensionState = state;
			TryFindDefaultConfig();
			SuspensionData suspensionData = _getDefaultCarTuningDataConfig.Tuning.FrontWheelData.SuspensionData;
			SuspensionData suspensionData2 = _getDefaultCarTuningDataConfig.Tuning.RearWheelData.SuspensionData;
			TuningLimitsData frontWheelDataLimits = _getDefaultCarTuningDataConfig.Tuning.FrontWheelDataLimits;
			TuningLimitsData rearWheelDataLimits = _getDefaultCarTuningDataConfig.Tuning.RearWheelDataLimits;
			GetDriveTrain.GetDynoData(out var desc);
			ApplyShockAbsorbers(CarEntity.GetValue(), suspensionData, suspensionData2, frontWheelDataLimits, rearWheelDataLimits, desc, out desc);
			GetDriveTrain.SetDynoData(desc, apply: false);
			GetDriveTrain.ApplySuspensionData();
		}

		private void ApplyShockAbsorbers(VehicleDataResult result, SuspensionData defaultFrontSuspensionData, SuspensionData defaultRearSuspensionData, TuningLimitsData frontTuningLimits, TuningLimitsData rearTuningLimits, DynoData dynoData, out DynoData dynoDataOut)
		{
			float4 suspensionValues = Utils.GetSuspensionValues(result.FrontWheel.SuspensionData, result.RearWheel.SuspensionData, defaultFrontSuspensionData, defaultRearSuspensionData, result.FrontWheel.SuspensionData.SuspensionState);
			float2 specs = new float2(suspensionValues.x, suspensionValues.y);
			specs = ClampSuspensionPair(specs, frontTuningLimits.BumpMin, frontTuningLimits.BumpMax, frontTuningLimits.ReboundMin, frontTuningLimits.ReboundMax, "front", "shock", "Bump", "Rebound");
			dynoData.FrontSuspension.Bump = specs.x;
			dynoData.FrontSuspension.Rebound = specs.y;
			specs = new float2(suspensionValues.z, suspensionValues.w);
			specs = ClampSuspensionPair(specs, rearTuningLimits.BumpMin, rearTuningLimits.BumpMax, rearTuningLimits.ReboundMin, rearTuningLimits.ReboundMax, "rear", "shock", "Bump", "Rebound");
			dynoData.RearSuspension.Bump = specs.x;
			dynoData.RearSuspension.Rebound = specs.y;
			dynoDataOut = dynoData;
		}

		private static float2 ClampSuspensionPair(float2 specs, float minX, float maxX, float minY, float maxY, string axleLabel, string pairLabel, string fieldXName, string fieldYName)
		{
			float x = Mathf.Clamp(specs.x, minX, maxX);
			float y = Mathf.Clamp(specs.y, minY, maxY);
			return new float2(x, y);
		}

		public void SetShockAbsorberBump(AxleType axle, float value)
		{
			if ((bool)_axles)
			{
				VehicleDataResult value2 = CarEntity.GetValue();
				bool num = axle == AxleType.Front;
				CarInfo carInfo = _vehicleData.carInfo;
				float num2 = Utils.KN2N(value);
				if (num)
				{
					carInfo.TuningData.FrontWheelData.SuspensionData.SuspensionState = SuspensionState.Custom;
					carInfo.TuningData.FrontWheelData.SuspensionData.Bump = num2;
				}
				else
				{
					carInfo.TuningData.RearWheelData.SuspensionData.SuspensionState = SuspensionState.Custom;
					carInfo.TuningData.RearWheelData.SuspensionData.Bump = num2;
				}
				(num ? value2.FrontWheel.SuspensionData : value2.RearWheel.SuspensionData).GetShockAbsorberParams(out var rate);
				float2 specs = new float2(num2, rate.y);
				_powertrain.SetShockAbsorbers(axle, specs);
			}
		}

		public void SetShockAbsorberRebound(AxleType axle, float value)
		{
			if ((bool)_axles)
			{
				VehicleDataResult value2 = CarEntity.GetValue();
				bool num = axle == AxleType.Front;
				CarInfo carInfo = _vehicleData.carInfo;
				float num2 = Utils.KN2N(value);
				if (num)
				{
					carInfo.TuningData.FrontWheelData.SuspensionData.SuspensionState = SuspensionState.Custom;
					carInfo.TuningData.FrontWheelData.SuspensionData.Rebound = num2;
				}
				else
				{
					carInfo.TuningData.RearWheelData.SuspensionData.SuspensionState = SuspensionState.Custom;
					carInfo.TuningData.RearWheelData.SuspensionData.Rebound = num2;
				}
				(num ? value2.FrontWheel.SuspensionData : value2.RearWheel.SuspensionData).GetShockAbsorberParams(out var rate);
				float2 specs = new float2(rate.x, num2);
				_powertrain.SetShockAbsorbers(axle, specs);
			}
		}

		public void UpdateAntirollBars(AxleType axle, float value)
		{
			float num = Utils.KN2N(value);
			if (axle == AxleType.Front)
			{
				_vehicleData.carInfo.TuningData.FrontWheelData.SuspensionData.AntiSwayBarForce = num;
			}
			else
			{
				_vehicleData.carInfo.TuningData.RearWheelData.SuspensionData.AntiSwayBarForce = num;
			}
			if ((bool)GetDriveTrain)
			{
				GetDriveTrain.SetStabSpringCoef(axle, num);
			}
		}

		public void UpdateCambers(AxleType axle, float value)
		{
			if (axle == AxleType.Front)
			{
				_vehicleData.carInfo.TuningData.FrontWheelData.SuspensionData.Camber = value;
			}
			else
			{
				_vehicleData.carInfo.TuningData.RearWheelData.SuspensionData.Camber = value;
			}
			if ((bool)GetDriveTrain)
			{
				GetDriveTrain.SetCamber(axle, value);
			}
		}

		public void UpdateToe(AxleType axle, float value)
		{
			if (axle == AxleType.Front)
			{
				_vehicleData.carInfo.TuningData.FrontWheelData.SuspensionData.ToeIn = value;
			}
			else
			{
				_vehicleData.carInfo.TuningData.RearWheelData.SuspensionData.ToeIn = value;
			}
			if ((bool)GetDriveTrain)
			{
				GetDriveTrain.SetToeIn(axle, value);
			}
		}

		public void UpdateMaxSteeringAngle(int value)
		{
			_vehicleData.carInfo.TuningData.FrontWheelData.SuspensionData.MaxSteerAngle = value;
			if ((bool)GetDriveTrain)
			{
				GetDriveTrain.MaxSteeringAngle = value;
			}
		}

		public void UpdateAckerman(int value)
		{
			_vehicleData.carInfo.TuningData.FrontWheelData.SuspensionData.Ackerman = value;
			if ((bool)GetDriveTrain)
			{
				GetDriveTrain.Ackerman = value;
			}
		}

		public void UpdateCaster(float value)
		{
			_vehicleData.carInfo.TuningData.FrontWheelData.SuspensionData.Caster = value;
			if ((bool)GetDriveTrain)
			{
				GetDriveTrain.Caster = value;
			}
		}

		public void UpdateSpacers(AxleType axle)
		{
			if (!_axles)
			{
				return;
			}
			VehicleDataResult value = CarEntity.GetValue();
			foreach (WheelProvider wheel in ((axle == AxleType.Front) ? _axles.FrontAxle : _axles.RearAxle).Wheels)
			{
				if ((bool)wheel.WheelModel)
				{
					wheel.WheelModel.SpacerWidth = ((axle == AxleType.Front) ? value.FrontWheel.SpacerWidth : value.RearWheel.SpacerWidth);
				}
			}
			if (axle != AxleType.Rear || !_axles.DualAxles)
			{
				return;
			}
			foreach (WheelProvider wheel2 in _axles.Rear1Axle.Wheels)
			{
				wheel2.WheelModel.SpacerWidth = value.RearWheel.SpacerWidth;
			}
			foreach (WheelProvider wheel3 in _axles.Rear2Axle.Wheels)
			{
				wheel3.WheelModel.SpacerWidth = value.RearWheel.SpacerWidth;
			}
		}

		public void UpdateCompressorPower(float value)
		{
			CarEntity.Engine.DynamicTuningSettings.HeightAdjustSpeed = value;
			_vehicleData.carInfo.TuningData.DynamicTuningSettings.HeightAdjustSpeed = value;
			if ((bool)_powertrain)
			{
				_powertrain.UpdateCompressorPower(value);
			}
		}

		public void UpdateSprings(AxleType axle)
		{
			if (!GetAxles)
			{
				return;
			}
			VehicleDataResult value = CarEntity.GetValue();
			bool flag = axle == AxleType.Front;
			float2 specs;
			if (flag)
			{
				float springLengthMin = _getDefaultCarTuningDataConfig.Tuning.FrontWheelDataLimits.SpringLengthMin;
				float springLengthMax = _getDefaultCarTuningDataConfig.Tuning.FrontWheelDataLimits.SpringLengthMax;
				value.FrontWheel.SuspensionData.GetSpringParams(out specs);
				specs.x = Utils.RoundTo(Mathf.Clamp(specs.x, Utils.Inch2Meter(springLengthMin), Utils.Inch2Meter(springLengthMax)), 4);
			}
			else
			{
				float springLengthMin2 = _getDefaultCarTuningDataConfig.Tuning.RearWheelDataLimits.SpringLengthMin;
				float springLengthMax2 = _getDefaultCarTuningDataConfig.Tuning.RearWheelDataLimits.SpringLengthMax;
				value.RearWheel.SuspensionData.GetSpringParams(out specs);
				specs.x = Utils.RoundTo(Mathf.Clamp(specs.x, Utils.Inch2Meter(springLengthMin2), Utils.Inch2Meter(springLengthMax2)), 4);
			}
			int num;
			float2 obj;
			if ((bool)_vehicleData && _vehicleData.carInfo != null)
			{
				AdditionalData additionalData = _vehicleData.carInfo.AdditionalData;
				if (additionalData != null)
				{
					PsData airSuspension = additionalData.AirSuspension;
					if (airSuspension != null && airSuspension.Bought)
					{
						num = (airSuspension.Installed ? 1 : 0);
						if (num == 0)
						{
							goto IL_0146;
						}
						obj = _powertrain.GetSpringSpecs(axle);
						goto IL_015e;
					}
				}
				num = 0;
			}
			else
			{
				num = 0;
			}
			goto IL_0146;
			IL_0146:
			obj = default;
			goto IL_015e;
			IL_015e:
			float2 float5 = obj;
			bool flag2 = num != 0 && (!Mathf.Approximately(float5.x, specs.x) || !Mathf.Approximately(float5.y, specs.y));
			DynamicTuningSettings dynamicTuningSettings = _vehicleData.carInfo.TuningData.DynamicTuningSettings;
			float num2 = (flag ? dynamicTuningSettings.CurrentFrontSuspensionHeight : dynamicTuningSettings.CurrentRearSuspensionHeight);
			float z = ((num == 0 || flag2) ? (-1f) : ((num2 < 0f) ? (-1f) : (num2 * 0.001f)));
			float3 specs2 = new float3(specs.x, specs.y, z);
			DynamicTuningSettings dynamicTuningSettings2 = _vehicleData.carInfo.TuningData.DynamicTuningSettings;
			float num3 = (flag ? dynamicTuningSettings2.MinFrontSuspensionHeight : dynamicTuningSettings2.MinRearSuspensionHeight);
			float num4 = (flag ? dynamicTuningSettings2.MaxFrontSuspensionHeight : dynamicTuningSettings2.MaxRearSuspensionHeight);
			float2 float6 = new float2((num3 < 0f) ? (-1f) : (num3 * 0.001f), (num4 < 0f) ? (-1f) : (num4 * 0.001f));
			if (flag)
			{
				_powertrain.SetFrontAirBagLimit(float6);
			}
			else
			{
				_powertrain.SetRearAirBagLimit(float6);
			}
			_powertrain.SetSprings(axle, specs2);
			if (((uint)num & (flag2 ? 1u : 0u)) != 0)
			{
				float num5 = (flag ? _powertrain.GetFrontTargetHeight() : _powertrain.GetRearTargetHeight());
				DynamicTuningSettings dynamicTuningSettings3 = _vehicleData.carInfo.TuningData.DynamicTuningSettings;
				if (flag)
				{
					dynamicTuningSettings3.CurrentFrontSuspensionHeight = Mathf.Round(num5 * 1000f);
				}
				else
				{
					dynamicTuningSettings3.CurrentRearSuspensionHeight = Mathf.Round(num5 * 1000f);
				}
			}
		}

		public void UpdateShockAbsorbers(AxleType axle)
		{
			if ((bool)_axles)
			{
				bool num = axle == AxleType.Front;
				VehicleDataResult value = CarEntity.GetValue();
				float2 rate;
				if (num)
				{
					value.FrontWheel.SuspensionData.GetShockAbsorberParams(out rate);
				}
				else
				{
					value.RearWheel.SuspensionData.GetShockAbsorberParams(out rate);
				}
				_powertrain.SetShockAbsorbers(axle, rate);
			}
		}

		public void TrySaveMileage(bool useFullSave = true)
		{
			if (V.Car != base.gameObject)
			{
				return;
			}
			int slotId = _vehicleData.slotId;
			if (_carInfoService.TryGetFromSlot(slotId, out var carInfo) && _vehicleData.carInfo != null)
			{
				if (CarEntity.CarInstanceId != carInfo.CarInstanceId)
				{
					Debug.LogWarning("Car instance id is not equal.");
					return;
				}
				SavePlayerTotalDistance(carInfo);
				SaveMileage(carInfo);
				SaveFluids(carInfo);
				SaveDirt(carInfo, useFullSave);
				carInfo.SaveCarMillage(CarEntity, useFullSave);
			}
		}

		private void SaveDirt(CarInfo carInfo, bool useFullSave)
		{
			DirtComposition dirtComposition = _vehicleData.DirtComposition;
			if (dirtComposition != null && !dirtComposition.SkipWrite && carInfo.Colors != null)
			{
				carInfo.Colors.DirtAmount = dirtComposition.DirtIntensity;
				carInfo.Colors.DirtColor = dirtComposition.DirtColor;
				dirtComposition.SaveDirtToPrefs();
				if (!useFullSave)
				{
					SaveColorsPartial(carInfo);
				}
			}
		}

		private static void SaveColorsPartial(CarInfo carInfo)
		{
			string carInfoPath = DirectoryHelper.GetCarInfoPath(carInfo.CarInstanceId);
			if (EasySaveDataHandler.TryGetFile(carInfoPath, out var file))
			{
				EasySaveDataHandler.SaveValue(carInfo.Colors, file, "Colors");
			}
			EasySaveDataHandler.SaveValue(carInfo.Colors, carInfoPath, "Colors");
		}

		private void SaveFluids(CarInfo carInfo)
		{
			CarEntity.MaintenanceInfo.FuelInTank = (_powertrain ? _powertrain.CurrentFuel : CarEntity.MaintenanceInfo.FuelInTank);
			carInfo.TuningData.Maintenance.FuelInTank = CarEntity.MaintenanceInfo.FuelInTank;
			carInfo.TuningData.Maintenance.OilCapacity = CarEntity.MaintenanceInfo.OilCapacity;
			carInfo.TuningData.Maintenance.OilLife = CarEntity.MaintenanceInfo.OilLife;
		}

		private void SaveMileage(CarInfo carInfo)
		{
			if (CarEntity.MaintenanceInfo.Mileage > 0f)
			{
				carInfo.TuningData.Maintenance.Mileage = CarEntity.MaintenanceInfo.Mileage;
			}
		}

		private void SavePlayerTotalDistance(CarInfo carInfo)
		{
			float num = Mathf.Max(CarEntity.MaintenanceInfo.Mileage - carInfo.TuningData.Maintenance.Mileage, 0f);
			PlayerTotalDistance.Set((float)PlayerTotalDistance.Get() + num);
		}

		public void UpdateEngine()
		{
			throw new NotImplementedException();
		}
	}
}
