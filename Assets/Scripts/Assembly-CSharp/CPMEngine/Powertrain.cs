using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Database;
using CPM.Rating.Entities;
using CPM.Transmission;
using CPMEngine.Protected;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.CPMRating.DailyTasks.Scripts;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

namespace CPMEngine
{
	[RequireComponent(typeof(Rigidbody))]
	public class Powertrain : MonoBehaviour
	{
		public const int Neutral = 0;

		public const int First = 1;

		public const int Reverse = -1;

		private const float SpeedMpsToDriftLimit = 2.77f;

		private const int DriftedDistanceKoef = 1000;

		private const float MinDriftDistanceInMetersToSaveSaveAndShowLabel = 9.6f;

		public bool Shifter;

		[SerializeField]
		private Axles Axles;

		private ChassisEventDelegate _onPreSimulationInvoker;

		private ChassisEventDelegate _onPostSimulationInvoker;

		private ChassisEventDelegate _onUpdateChassisInvoker;

		private ChassisEventDelegate _onUpdateWheelsInvoker;

		private ChassisEventDelegate _onApplyDynoDataInvoker;

		private ChassisEventDelegate _onMassChangedInvoker;

		private bool _cachedGearboxState;

		[SerializeField]
		protected bool SimpleSimulation;

		public bool EnableForceFeedback;

		[SerializeField]
		private DynoData DynoData = new DynoData();

		[SerializeField]
		[SerializeReference]
		private Collider CachedCollider;

		public Transform centerOfMass;

		public Transform TrailerJoint;

		public Transform Loads;

		private TrailerEntity _trailerEntity;

		private SoundController _soundController;

		private Rigidbody _rigidbody;

		private PhysEngine _physEngine;

		private SteerAssist _steerAssist;

		private VehicleData _vehicleData;

		private VehicleEntityProvider _vehicleEntityProvider;

		private bool _fuelInitialized;

		private CPMVehicleCore _cpmVehicleCore;

		private FreeDriveProvider _freeDriveProvider;

		private DailyTasksInfoContainer _dailyTasksInfoContainer;

		private EngineSound _engineSound;

		private IRay _wheelRayTracer;

		private bool _cachedSpeedMpsDirty = true;

		private float _cachedSpeedMps;

		private IDisposable _disposeStarter;

		private IDisposable _disposeEngine;

		private readonly ReactiveProperty<float> _speedMps = new ReactiveProperty<float>(0f);

		private bool _isRaceCar;

		private bool _isEngineBroken;

		private float _lastBurnOutTime;

		private bool _isAllreadyStopped;

		private bool _isDestroyed;

		public ObscuredFloat DistanceSession;

		public ObscuredFloat DistanceDrifted;

		public ObscuredFloat DistanceOffroadTravelled;

		public ObscuredFloat DistanceCurrentDrifted;

		public ObscuredFloat MaxDriftDistance;

		public ObscuredFloat MaxDriftDistanceForDailyTask = 0.01f;

		public float DriftTimeCount;

		private Bounds _weightBounds;

		private float _wheelBase;

		private float _frontTrack;

		private Vector3 _dimensions;

		private bool _starter;

		private float _starterWorks;

		private Vector2 _lastPosition;

		private float _driftTimeCount;

		private int _skipDistanceTravel;

		private bool _driftingNow;

		private const float SmokeDriftAngle = 10f;

		private float _brakePressure;

		private bool _isAutomaticGearbox;

		private bool _raceBotMode;

		private double _pendingMileageKm;

		private Vector3 _lastGoodPos;

		private Quaternion _lastGoodRot = Quaternion.identity;

		private const float AvgFuelConsumptionTauSec = 60f;

		private const float AvgFuelConsumptionMinSpeedMps = 1.39f;

		private const float AvgFuelConsumptionSampleIntervalSec = 0.2f;

		private const string AvgFuelConsumptionMigratedPrefPrefix = "CPM2.AvgFuelConsumption.Migrated.";

		private float _avgFuelConsumptionLpHk;

		private bool _avgFuelConsumptionLoaded;

		private float _avgFuelConsumptionLastSampleTime;

		private bool _engineThermalStarted;

		private SoundBankLoadService _soundBankService;

		private bool _airSuspensionBankHeld;

		private bool _compressorBankHeld;

		private const int PhysWarmupSteps = 3;

		private int _physWarmupStepsDone;

		public ObscuredBool AlwaysDriftForDebug => false;

		public bool ForceOnlineStarter
		{
			set
			{
				_physEngine.Ignition = value;
			}
		}

		public bool IsParkingGear
		{
			get
			{
				return _physEngine.ParkingGear;
			}
			set
			{
				_physEngine.ParkingGear = value;
			}
		}

		public bool SportMode
		{
			get
			{
				return _physEngine.SportMode;
			}
			set
			{
				_physEngine.SportMode = value;
			}
		}

		public bool LowMode
		{
			get
			{
				return _physEngine.LowMode;
			}
			set
			{
				_physEngine.LowMode = value;
			}
		}

		public GearboxKind GearboxKind => DynoData.Transmission.Kind;

		public AutoShifterModes AutoModes => DynoData.Transmission.AutoModes;

		public int LowMaxGear => DynoData.Transmission.LowMaxGear;

		public bool IsCruiseControlActive { get; set; }

		public Action OnCheckEngineChanged { get; set; }

		public GearboxState GearboxState
		{
			get
			{
				return _physEngine.GearboxState;
			}
			set
			{
				_physEngine.GearboxState = value;
			}
		}

		public float MaxSideAccel => _physEngine.MaxSideAccel;

		public float MaxBrakeAccel => _physEngine.MaxBrakeAccel;

		public float SpeedMpsOnline { get; set; }

		public bool ActivateLaunchControl
		{
			set
			{
				_physEngine.ActivateLaunchControl = value;
			}
		}

		public bool IsActiveLaunchControlNow => _physEngine.IsActiveLaunchControlNow;

		public bool IsTrailer
		{
			get
			{
				return _physEngine.Trailer;
			}
			set
			{
				_physEngine.Trailer = value;
			}
		}

		public bool OnLineCar
		{
			get
			{
				return _physEngine.OnLineCar;
			}
			set
			{
				_physEngine.OnLineCar = value;
			}
		}

		public IReadOnlyReactiveProperty<float> SpeedMpsReactive => _speedMps;

		public IReadOnlyReactiveProperty<float> SpeedKmhReactive { get; set; }

		public IReadOnlyReactiveProperty<float> SpeedMphReactive { get; private set; }

		public bool ManualHelper
		{
			get
			{
				return _physEngine.ManualHelper;
			}
			set
			{
				_physEngine.ManualHelper = value;
			}
		}

		public bool IsDriftingNow => _driftingNow;

		public bool IsSlidingForSmoke
		{
			get
			{
				if (SpeedMps > 2.77f)
				{
					return Mathf.Abs(_physEngine.ChassisAngle) >= 10f;
				}
				return false;
			}
		}

		private bool Fuel => CurrentFuel > 0f;

		public Bounds GetWeightBounds => _weightBounds;

		public Vector3 GetDimensions => _dimensions;

		public float GetWheelBase => _wheelBase;

		public float MaxSteeringAngle
		{
			get
			{
				return _physEngine.MaxSteerAngle;
			}
			set
			{
				DynoData.FrontSuspension.MaxSteerAngle = value;
				_physEngine.MaxSteerAngle = value * ((float)Math.PI / 180f);
			}
		}

		public Axles GetAxles
		{
			get
			{
				if (!Axles)
				{
					TryGetComponent<Axles>(out Axles);
				}
				return Axles;
			}
		}

		private VehicleData GetVehicleData
		{
			get
			{
				if (!_vehicleData && (bool)this)
				{
					TryGetComponent<VehicleData>(out _vehicleData);
				}
				return _vehicleData;
			}
		}

		public bool AbsTrigger => _physEngine.AbsTrigger;

		public bool RevLimiterTriggered => _physEngine.RevLimiterTriggered;

		public float LimiterFlutterHz => _physEngine.LimiterFlutterHz;

		public float LimiterCutDuty => 0.2f;

		public float LimiterBounceDepthRpm => _physEngine.LimiterBounceDepthRpm;

		public bool OnGround
		{
			get
			{
				bool flag = false;
				foreach (WheelProvider allWheel in GetAxles.AllWheels)
				{
					flag = allWheel.OnGroundDown;
					if (flag)
					{
						break;
					}
				}
				return flag;
			}
		}

		public bool Automatic
		{
			get
			{
				return _physEngine.Automatic;
			}
			set
			{
				_physEngine.Automatic = value;
			}
		}

		public Collider Collider
		{
			get
			{
				if (!CachedCollider)
				{
					CachedCollider = GetComponentInChildren<Collider>();
				}
				return CachedCollider;
			}
		}

		public bool IsAutomaticGearbox
		{
			get
			{
				return _isAutomaticGearbox;
			}
			set
			{
				_isAutomaticGearbox = value;
				_physEngine.At = value;
				_physEngine.AutoClutch = value;
			}
		}

		public bool CheckEngineStatus { get; private set; }

		public bool IsRaceCar
		{
			get
			{
				return _isRaceCar;
			}
			set
			{
				_isRaceCar = value;
				_physEngine.AiMode = value;
				PhysSteps = 8;
				DynoData.FrontTyre.FrictionMultiplier = 4.2f;
				DynoData.RearTyre.FrictionMultiplier = 4.4f;
				Axles getAxles = GetAxles;
				foreach (WheelProvider wheel in getAxles.FrontAxle.Wheels)
				{
					wheel.Wheel.FrictionMultiplier = DynoData.FrontTyre.FrictionMultiplier;
					wheel.Wheel.TyreFriction = 1f;
					wheel.Wheel.TyreTemperature = 100f;
					wheel.Wheel.GripTemperature = 100f;
				}
				foreach (WheelProvider wheel2 in getAxles.RearAxle.Wheels)
				{
					wheel2.Wheel.FrictionMultiplier = DynoData.RearTyre.FrictionMultiplier;
					wheel2.Wheel.TyreFriction = 1f;
					wheel2.Wheel.TyreTemperature = 100f;
					wheel2.Wheel.GripTemperature = 100f;
				}
				if (getAxles.Rear1Axle.Wheels.Count > 0)
				{
					foreach (WheelProvider wheel3 in getAxles.Rear1Axle.Wheels)
					{
						wheel3.Wheel.FrictionMultiplier = DynoData.RearTyre.FrictionMultiplier;
						wheel3.Wheel.TyreFriction = 1f;
						wheel3.Wheel.TyreTemperature = 100f;
						wheel3.Wheel.GripTemperature = 100f;
					}
				}
				if (getAxles.Rear2Axle.Wheels.Count <= 0)
				{
					return;
				}
				foreach (WheelProvider wheel4 in getAxles.Rear2Axle.Wheels)
				{
					wheel4.Wheel.FrictionMultiplier = DynoData.RearTyre.FrictionMultiplier;
					wheel4.Wheel.TyreFriction = 1f;
					wheel4.Wheel.TyreTemperature = 100f;
					wheel4.Wheel.GripTemperature = 100f;
				}
			}
		}

		public int PhysSteps
		{
			get
			{
				return _physEngine.PhysSteps;
			}
			set
			{
				_physEngine.PhysSteps = Mathf.Clamp(value, 1, 20);
			}
		}

		public bool SimpleSimulationMode
		{
			get
			{
				return SimpleSimulation;
			}
			set
			{
				SimpleSimulation = value;
				_physEngine.SimpleMode = value;
			}
		}

		public bool TrafficMode
		{
			get
			{
				return _physEngine.TrafficMode;
			}
			set
			{
				_physEngine.SimpleMode = value;
				_physEngine.TrafficMode = value;
			}
		}

		public bool TowCar
		{
			get
			{
				return _physEngine.TowingMode;
			}
			set
			{
				_physEngine.SimpleMode = value;
				_physEngine.TowingMode = value;
			}
		}

		public bool RaceBotMode
		{
			get
			{
				return _raceBotMode;
			}
			set
			{
				_raceBotMode = value;
				_physEngine.RaceBotMode = value;
				if (value)
				{
					_physEngine.SimpleMode = true;
				}
			}
		}

		public PhysEngine Phys => _physEngine;

		public float TotalMass => Mathf.Round(DynoData.Chassis.Mass);

		public float GetMileage
		{
			get
			{
				if (!_vehicleEntityProvider)
				{
					return 0f;
				}
				return _vehicleEntityProvider.CarEntity.MaintenanceInfo.Mileage;
			}
		}

		public float AvgFuelConsumption => _avgFuelConsumptionLpHk;

		public GearboxType GearType => DynoData.Transmission.GearType;

		public string GearTypeDescript => DynoData.Transmission.GearType switch
		{
			GearboxType.GearFwd => "FWD", 
			GearboxType.GearRwd => "RWD", 
			GearboxType.GearAwd => "AWD", 
			GearboxType.Gear4Wd => "4WD", 
			GearboxType.Gear4Wdr1 => "4WD", 
			GearboxType.Gear4Wdr2 => "4WD", 
			GearboxType.Gear6Wd => "6WD", 
			GearboxType.Gear6Wdr2 => "6WD", 
			GearboxType.Gear8Wd => "8WD", 
			GearboxType.Gear0Wd => "Non Geared", 
			_ => throw new ArgumentOutOfRangeException(), 
		};

		public TransferCase TransferCase { get; private set; }

		public EngineThermal EngineThermal { get; } = new EngineThermal();

		public bool HasActiveTransferCase
		{
			get
			{
				if (TransferCase != null)
				{
					return TransferCase.SupportsBase(DynoData.Transmission.GearType);
				}
				return false;
			}
		}

		public bool PositionIntegration
		{
			get
			{
				return _physEngine.PositionIntegration;
			}
			set
			{
				_physEngine.PositionIntegration = value;
			}
		}

		private GearboxType EffectiveDriveType => _physEngine.GearType;

		public float IstantConsumption => _physEngine.IstantConsumption;

		public float CurrentConsumption => _physEngine.CurrentConsumption;

		public float CurrentFuel
		{
			get
			{
				return _physEngine.CurrentFuel;
			}
			set
			{
				_physEngine.CurrentFuel = value;
			}
		}

		public bool IsEngineBroken
		{
			get
			{
				return _isEngineBroken;
			}
			set
			{
				_isEngineBroken = value;
				if (_isEngineBroken && IsEngineWorks)
				{
					ShutdownEngine();
				}
			}
		}

		public float Throttle
		{
			get
			{
				return _physEngine.Throttle;
			}
			set
			{
				_physEngine.Throttle = value;
			}
		}

		public float ThrottleIntent
		{
			set
			{
				_physEngine.ThrottleIntent = value;
			}
		}

		public float ThrottleRev => _physEngine.Accel;

		public float Brake
		{
			get
			{
				return _physEngine.Brake;
			}
			set
			{
				if (Handbrake <= 0f)
				{
					AirBrakesSimulation(value);
				}
				_physEngine.Brake = value;
				GetAxles.IsBrakePressed.Value = value > 0.005f;
			}
		}

		public float Handbrake
		{
			get
			{
				return _physEngine.HandBrake;
			}
			set
			{
				_physEngine.HandBrake = value;
			}
		}

		public bool Ignition
		{
			get
			{
				return _physEngine.Ignition;
			}
			set
			{
				_physEngine.Ignition = value;
			}
		}

		public float Clutch
		{
			get
			{
				return _physEngine.Clutch;
			}
			set
			{
				_physEngine.Clutch = value;
			}
		}

		public float ClutchPedalInput
		{
			get
			{
				return _physEngine.ClutchPedalInput;
			}
			set
			{
				_physEngine.ClutchPedalInput = value;
			}
		}

		public float WaterTemp => _physEngine.WaterTemp;

		public bool AutoClutch
		{
			get
			{
				return _physEngine.AutoClutch;
			}
			set
			{
				_physEngine.AutoClutch = value;
			}
		}

		public bool IsShifting => _physEngine.IsShifting;

		public int Gear
		{
			get
			{
				return _physEngine.Gear;
			}
			set
			{
				_physEngine.Gear = value;
			}
		}

		public float ClutchPedalOverride
		{
			get
			{
				return _physEngine.ClutchPedalOverride;
			}
			set
			{
				_physEngine.ClutchPedalOverride = value;
			}
		}

		public float CurrentRpm
		{
			get
			{
				return _physEngine.CurrentRpm;
			}
			set
			{
				_physEngine.CurrentRpm = value;
			}
		}

		public float EngineBrakeTorqueRatio
		{
			get
			{
				return _physEngine.EngineBrakeTorqueRatio;
			}
			set
			{
				DynoData.Engine.BrakeTorqueRatio = value;
				_physEngine.EngineBrakeTorqueRatio = value;
			}
		}

		public float EngineDisplacementCc => _physEngine.DisplacementCc;

		public bool IsRotaryEngine => _physEngine.IsRotaryEngine;

		public float EngineInertiaRatio
		{
			get
			{
				return _physEngine.EngineInertiaRatio;
			}
			set
			{
				DynoData.Engine.InertiaRatio = value;
				_physEngine.EngineInertiaRatio = value;
			}
		}

		public Vector3 Velocity
		{
			get
			{
				return Rigidbody.velocity;
			}
			set
			{
				Rigidbody.velocity = value;
			}
		}

		public Vector3 AngularVelocity
		{
			get
			{
				return Rigidbody.angularVelocity;
			}
			set
			{
				Rigidbody.angularVelocity = value;
			}
		}

		public float FinalDrive
		{
			get
			{
				return _physEngine.FinalDrive;
			}
			set
			{
				_physEngine.FinalDrive = value;
				DynoData.GearBox.FinalDrive = value;
				ApplyTransferCaseState();
			}
		}

		public int NumGears
		{
			get
			{
				return _physEngine.NumGears;
			}
			set
			{
				_physEngine.NumGears = value;
			}
		}

		public float Load => _physEngine.Load;

		public float SideAccel => _physEngine.SideAccel;

		public float VertAccel => _physEngine.VertAccel;

		public float AngAccel => _physEngine.AngAccel;

		public float SteerAlignMom => _physEngine.SteerAlignMom;

		public float Caster
		{
			get
			{
				return DynoData.FrontSuspension.Caster;
			}
			set
			{
				DynoData.FrontSuspension.Caster = value;
				ApplySuspDesc2Axle(AxleType.Front, DynoData.FrontSuspension);
			}
		}

		public float GearDelay => _physEngine.GearShiftingDelay;

		public float BrakeFrontDiskTorque
		{
			get
			{
				return _physEngine.BrakeFrontTorque;
			}
			set
			{
				_physEngine.BrakeFrontTorque = value;
			}
		}

		public float BrakeRearDiskTorque
		{
			get
			{
				return _physEngine.BrakeRearTorque;
			}
			set
			{
				_physEngine.BrakeRearTorque = value;
			}
		}

		public float EngineMaxTorque => _physEngine.EngineMaxTorque;

		public float EngineMaxTorqueRpm => _physEngine.EngineMaxTorqueRpm;

		public float ClutchLimit => _physEngine.ClutchLimit;

		public float RedlineRpm => _physEngine.RpmController.y;

		public float MaxRpm => _physEngine.RpmController.z;

		public float EngineIdleRpm => _physEngine.RpmController.x;

		public float EngineCurTorque => _physEngine.EngineCurTorque;

		public float EngineCurPower => _physEngine.EngineCurPower;

		public float BrakeFrontBalance
		{
			get
			{
				return _physEngine.BrakeFrontBalance;
			}
			set
			{
				_physEngine.BrakeFrontBalance = value * 0.01f;
				DynoData.Chassis.BrakeBalance = (int)value;
			}
		}

		public float Ackerman
		{
			get
			{
				return _physEngine.Ackerman;
			}
			set
			{
				DynoData.FrontSuspension.Ackerman = value;
				_physEngine.Ackerman = value * 0.01f;
				ApplySuspDesc2Axle(AxleType.Front, DynoData.FrontSuspension);
			}
		}

		public float Starter
		{
			get
			{
				return _physEngine.Starter;
			}
			set
			{
				_physEngine.Starter = value;
			}
		}

		public bool IsEngineWorks => _physEngine.IsEngineWorks;

		public float GetFrontBrakeTorque => _physEngine.FrontBrakeTorque;

		public float GetRearBrakeTorque => _physEngine.RearBrakeTorque;

		public float GetFrontBrakeMass => DynoData.FrontSuspension.BrakeDiskMass;

		public float GetRearBrakeMass => DynoData.RearSuspension.BrakeDiskMass;

		public Rigidbody Rigidbody
		{
			get
			{
				if (!_rigidbody)
				{
					TryGetComponent<Rigidbody>(out _rigidbody);
				}
				return _rigidbody;
			}
		}

		public RB GetRigidbody => _physEngine.RigidBody;

		public bool PhysicsAsleep { get; private set; }

		public string GetSpeedometerGearName
		{
			get
			{
				int gear = Gear;
				if (gear <= 0)
				{
					return gear switch
					{
						-1 => "R", 
						0 => IsParkingGear ? "P" : "N", 
						_ => string.Empty, 
					};
				}
				return (!IsAutomaticGearbox || !Automatic || GearboxKind == GearboxKind.Sequential) ? $"{Gear}" : (LowMode ? "L" : (SportMode ? "S" : "D"));
			}
		}

		public string GetKnobGearName
		{
			get
			{
				if (IsAutomaticGearbox && !Automatic)
				{
					return "M";
				}
				int gear = Gear;
				if (gear <= 0)
				{
					return gear switch
					{
						-1 => "R", 
						0 => IsParkingGear ? "P" : "N", 
						_ => string.Empty, 
					};
				}
				return (!IsAutomaticGearbox) ? $"{Gear}" : (LowMode ? "L" : (SportMode ? "S" : "D"));
			}
		}

		public float Mileage { get; private set; }

		public float SpeedKmh => SpeedMps * 3.6f;

		public float SpeedMph => SpeedMps * 2.237f;

		public float SpeedMps
		{
			get
			{
				if (!_rigidbody)
				{
					return 0f;
				}
				if (!OnLineCar && _cachedSpeedMpsDirty)
				{
					_cachedSpeedMps = Velocity.magnitude;
					_cachedSpeedMpsDirty = false;
					_speedMps.Value = Velocity.magnitude;
				}
				return _cachedSpeedMps;
			}
		}

		public event Action OnGearChanged
		{
			add
			{
				_physEngine.OnGearChanged += value;
			}
			remove
			{
				_physEngine.OnGearChanged -= value;
			}
		}

		public event Action OnBlowOff
		{
			add
			{
				_physEngine.OnBlowOff += value;
			}
			remove
			{
				_physEngine.OnBlowOff -= value;
			}
		}

		public event Action OnEngineStarted
		{
			add
			{
				_physEngine.OnEngineStarted += value;
			}
			remove
			{
				_physEngine.OnEngineStarted -= value;
			}
		}

		public event Action OnEngineShutdown
		{
			add
			{
				_physEngine.OnEngineShutdown += value;
			}
			remove
			{
				_physEngine.OnEngineShutdown -= value;
			}
		}

		public event Action<bool> OnTrcStateChanged
		{
			add
			{
				_physEngine.OnTrcStateChanged += value;
			}
			remove
			{
				_physEngine.OnTrcStateChanged -= value;
			}
		}

		public event Action<bool> OnEspStateChanged
		{
			add
			{
				_physEngine.OnEspStateChanged += value;
			}
			remove
			{
				_physEngine.OnEspStateChanged -= value;
			}
		}

		public event Action<bool> OnAbsStateChanged
		{
			add
			{
				_physEngine.OnAbsStateChanged += value;
			}
			remove
			{
				_physEngine.OnAbsStateChanged -= value;
			}
		}

		public event ChassisEventDelegate OnMassChangedEvent
		{
			add
			{
				_onMassChangedInvoker = (ChassisEventDelegate)Delegate.Combine(_onMassChangedInvoker, value);
			}
			remove
			{
				_onMassChangedInvoker = (ChassisEventDelegate)Delegate.Remove(_onMassChangedInvoker, value);
			}
		}

		public event ChassisEventDelegate OnApplyDynoDataEvent
		{
			add
			{
				_onApplyDynoDataInvoker = (ChassisEventDelegate)Delegate.Combine(_onApplyDynoDataInvoker, value);
			}
			remove
			{
				_onApplyDynoDataInvoker = (ChassisEventDelegate)Delegate.Remove(_onApplyDynoDataInvoker, value);
			}
		}

		public event ChassisEventDelegate OnPostSimulationEvent
		{
			add
			{
				_onPostSimulationInvoker = (ChassisEventDelegate)Delegate.Combine(_onPostSimulationInvoker, value);
			}
			remove
			{
				_onPostSimulationInvoker = (ChassisEventDelegate)Delegate.Remove(_onPostSimulationInvoker, value);
			}
		}

		public event ChassisEventDelegate OnPreSimulationEvent
		{
			add
			{
				_onPreSimulationInvoker = (ChassisEventDelegate)Delegate.Combine(_onPreSimulationInvoker, value);
			}
			remove
			{
				_onPreSimulationInvoker = (ChassisEventDelegate)Delegate.Remove(_onPreSimulationInvoker, value);
			}
		}

		public event ChassisEventDelegate OnUpdateChassisEvent
		{
			add
			{
				_onUpdateChassisInvoker = (ChassisEventDelegate)Delegate.Combine(_onUpdateChassisInvoker, value);
			}
			remove
			{
				_onUpdateChassisInvoker = (ChassisEventDelegate)Delegate.Remove(_onUpdateChassisInvoker, value);
			}
		}

		public event ChassisEventDelegate OnUpdateWheelsEvent
		{
			add
			{
				_onUpdateWheelsInvoker = (ChassisEventDelegate)Delegate.Combine(_onUpdateWheelsInvoker, value);
			}
			remove
			{
				_onUpdateWheelsInvoker = (ChassisEventDelegate)Delegate.Remove(_onUpdateWheelsInvoker, value);
			}
		}

		public float[] GetGearRatios()
		{
			return DynoData.GearBox.Gears;
		}

		public void RaiseGearChanged()
		{
			_physEngine.RaiseGearChanged();
		}

		public IObservable<float> OnSpeedChangedStream()
		{
			return SpeedKmhReactive;
		}

		public void SetOnlineRevLimiterTriggered(bool triggered)
		{
			_physEngine.SetOnlineRevLimiterTriggered(triggered);
		}

		public void SetOnlineEngineRunning(bool running)
		{
			_physEngine.SetOnlineEngineRunning(running);
		}

		public void SetOnlineAccel(float accel)
		{
			_physEngine.SetOnlineAccel(accel);
		}

		public void SetAutoPilot(bool status)
		{
			if (status)
			{
				if (!IsEngineWorks)
				{
					Gear = 0;
					StartEngine(starterSound: false);
				}
				AutoClutch = true;
				Automatic = true;
				if (Gear < 1)
				{
					Gear = 1;
				}
			}
			else if (!IsAutomaticGearbox)
			{
				AutoClutch = false;
				Automatic = false;
			}
		}

		public void SetCollider(Collider carCollider)
		{
			CachedCollider = carCollider;
		}

		public void SetAssits(CarOptionsValues carOptions)
		{
			DynoData.Engine.Options.AbsLevel = carOptions.AbsLevel;
			DynoData.Engine.Options.TrcLevel = carOptions.TrcLevel;
			DynoData.Engine.Options.EspLevel = carOptions.EspLevel;
			DynoData.Engine.Options.MTAssist = carOptions.MTAssist;
			DynoData.Engine.Options.ArcadeMode = carOptions.ArcadeMode;
			DynoData.Engine.Options.AnalogThrottle = carOptions.AnalogThrottle;
			ApplyAssist();
		}

		private void ApplyAssist()
		{
			_physEngine.AbsLevel = DynoData.Engine.Options.AbsLevel;
			_physEngine.TrcLevel = DynoData.Engine.Options.TrcLevel;
			_physEngine.EspLevel = DynoData.Engine.Options.EspLevel;
			_physEngine.AnalogThrottle = DynoData.Engine.Options.AnalogThrottle;
			bool num = _physEngine.ArcadeMode != DynoData.Engine.Options.ArcadeMode;
			_physEngine.ArcadeMode = DynoData.Engine.Options.ArcadeMode;
			ResetFrictionMultiplierMode();
			if (num)
			{
				SetRigidbody();
			}
			if (!IsAutomaticGearbox)
			{
				ManualHelper = !FlagsStorage.RacingGameMode && DynoData.Engine.Options.MTAssist;
				AutoClutch = !FlagsStorage.RacingGameMode && DynoData.Engine.Options.MTAssist;
			}
		}

		public void SetCheckEngine(bool value, bool sendRpc)
		{
			if (CheckEngineStatus != value)
			{
				CheckEngineStatus = value;
				if ((bool)GetVehicleData)
				{
					GetVehicleData.CarHealthComposition.DamageEngine(value, sendRpc);
				}
				OnCheckEngineChanged?.Invoke();
			}
		}

		public void SetRaceBotProfile(float topSpeedMps, float maxAccel, float maxBrake, float maxLateral, float steerGain, float maxSteerTorque, float verticalDamp, float angularDamp)
		{
			_physEngine.SetRaceBotProfile(topSpeedMps, maxAccel, maxBrake, maxLateral, steerGain, maxSteerTorque, verticalDamp, angularDamp);
		}

		public void SetRaceBotAutoVerticalDamp(bool on)
		{
			_physEngine.SetRaceBotAutoVerticalDamp(on);
		}

		public void SetRaceBotWheelInwardOffset(float offset)
		{
			_physEngine.SetRaceBotWheelInwardOffset(offset);
		}

		public void SetRaceBotLaunchCurve(float launchTraction, float launchBandFrac)
		{
			_physEngine.SetRaceBotLaunchCurve(launchTraction, launchBandFrac);
		}

		public void SetRaceBotCollisionGrip(float strength)
		{
			_physEngine.SetRaceBotCollisionGrip(strength);
		}

		public TyreType GetTyreType(AxleType axleType)
		{
			if (axleType != AxleType.Front)
			{
				return DynoData.RearTyre.TyreType;
			}
			return DynoData.FrontTyre.TyreType;
		}

		public void UpdateMileage(float m)
		{
			if ((bool)_vehicleEntityProvider && float.IsFinite(m) && !(Mathf.Abs(m) > 0.01f))
			{
				_pendingMileageKm += m;
				float mileage = _vehicleEntityProvider.CarEntity.MaintenanceInfo.Mileage;
				float num = (float)((double)mileage + _pendingMileageKm);
				if (num != mileage)
				{
					_vehicleEntityProvider.CarEntity.MaintenanceInfo.Mileage = num;
					_pendingMileageKm -= (double)num - (double)mileage;
				}
			}
		}

		public void SetMileage(float m)
		{
			if ((bool)_vehicleEntityProvider)
			{
				_vehicleEntityProvider.CarEntity.MaintenanceInfo.Mileage = m;
				_pendingMileageKm = 0.0;
			}
		}

		public void ResetAvgFuelConsumption()
		{
			_avgFuelConsumptionLpHk = 0f;
			_avgFuelConsumptionLoaded = true;
			if ((bool)_vehicleEntityProvider && (bool)_vehicleData)
			{
				WriteAvgFuelConsumptionToTuning(_vehicleData.carInfo?.TuningData?.DynamicTuningSettings);
			}
		}

		private static string AvgFuelConsumptionMigratedPrefKey(CarInfo carInfo)
		{
			string text = ((!string.IsNullOrEmpty(carInfo.CarInstanceId)) ? carInfo.CarInstanceId : carInfo.CarID.ToString());
			return "CPM2.AvgFuelConsumption.Migrated." + text;
		}

		private void UpdateAvgFuelConsumption()
		{
			if (!_vehicleEntityProvider || !_vehicleData)
			{
				return;
			}
			CarInfo carInfo = _vehicleData.carInfo;
			if (carInfo == null)
			{
				return;
			}
			DynamicTuningSettings dynamicTuningSettings = carInfo.TuningData?.DynamicTuningSettings;
			if (dynamicTuningSettings == null)
			{
				return;
			}
			if (!_avgFuelConsumptionLoaded)
			{
				string key = AvgFuelConsumptionMigratedPrefKey(carInfo);
				if (!PlayerPrefs.HasKey(key))
				{
					dynamicTuningSettings.MaxSteeringAngle = 0;
					PlayerPrefs.SetInt(key, 1);
					_avgFuelConsumptionLpHk = 0f;
				}
				else
				{
					int maxSteeringAngle = dynamicTuningSettings.MaxSteeringAngle;
					_avgFuelConsumptionLpHk = ((maxSteeringAngle > 0) ? ((float)maxSteeringAngle / 10f) : 0f);
				}
				_avgFuelConsumptionLoaded = true;
			}
			if (!(SpeedMps < 1.39f) && !(Time.time - _avgFuelConsumptionLastSampleTime < 0.2f))
			{
				_avgFuelConsumptionLastSampleTime = Time.time;
				float currentConsumption = _physEngine.CurrentConsumption;
				if (!(currentConsumption <= 0f) && !float.IsNaN(currentConsumption) && !float.IsInfinity(currentConsumption))
				{
					float num = Mathf.Clamp01(0.0033333334f);
					_avgFuelConsumptionLpHk = ((_avgFuelConsumptionLpHk <= 0f) ? currentConsumption : (_avgFuelConsumptionLpHk * (1f - num) + currentConsumption * num));
					WriteAvgFuelConsumptionToTuning(dynamicTuningSettings);
				}
			}
		}

		private void WriteAvgFuelConsumptionToTuning(DynamicTuningSettings dts)
		{
			if (dts != null)
			{
				int num = Mathf.RoundToInt(_avgFuelConsumptionLpHk * 10f);
				if (dts.MaxSteeringAngle != num)
				{
					dts.MaxSteeringAngle = num;
				}
			}
		}

		public void InitTransferCase(TransferCaseConfig config)
		{
			if (TransferCase == null && config != null && config.Enabled)
			{
				GearboxType gearType = DynoData.Transmission.GearType;
				if (!TransferCase.SupportsBase(gearType))
				{
					Debug.LogWarning($"[TransferCase] Enabled on unsupported base {gearType} — ignored (car {base.name})");
					return;
				}
				TransferCase = new TransferCase(config);
				ApplyTransferCaseState();
			}
		}

		private void ApplyTransferCaseState()
		{
			TransferCase transferCase = TransferCase;
			if (transferCase != null && _physEngine.GearType != GearboxType.Gear0Wd)
			{
				GearboxType gearType = DynoData.Transmission.GearType;
				if (!TransferCase.SupportsBase(gearType))
				{
					_physEngine.CenterDiffOpen = false;
					return;
				}
				bool flag = gearType == GearboxType.Gear4Wd;
				SetGearType(transferCase.EffectiveGearType(gearType));
				_physEngine.CenterDiffOpen = flag && !transferCase.CenterLockedEffective;
				_physEngine.FinalDrive = DynoData.GearBox.FinalDrive * transferCase.FinalDriveMultiplier;
				_physEngine.FrontLsd = (transferCase.FrontLockOn ? TransferCase.CreateLockedCopy(DynoData.Lsd.FrontLSD) : DynoData.Lsd.FrontLSD);
				_physEngine.RearLsd = (transferCase.RearLockOn ? TransferCase.CreateLockedCopy(DynoData.Lsd.RearLSD) : DynoData.Lsd.RearLSD);
			}
		}

		public bool TrySetTransferCaseMode(TransferCaseMode mode, out string denyReason)
		{
			denyReason = null;
			if (TransferCase == null)
			{
				return false;
			}
			if (!TransferCase.TrySetMode(mode, SpeedKmh, out denyReason))
			{
				return false;
			}
			ApplyTransferCaseState();
			return true;
		}

		public bool TrySetTransferCaseFrontLock(bool on, out string denyReason)
		{
			denyReason = null;
			if (TransferCase == null)
			{
				return false;
			}
			if (!TransferCase.TrySetFrontLock(on, SpeedKmh, out denyReason))
			{
				return false;
			}
			ApplyTransferCaseState();
			return true;
		}

		public bool TrySetTransferCaseRearLock(bool on, out string denyReason)
		{
			denyReason = null;
			if (TransferCase == null)
			{
				return false;
			}
			if (!TransferCase.TrySetRearLock(on, SpeedKmh, out denyReason))
			{
				return false;
			}
			ApplyTransferCaseState();
			return true;
		}

		public bool TrySetTransferCaseCenterLock(bool on, out string denyReason)
		{
			denyReason = null;
			if (TransferCase == null)
			{
				return false;
			}
			if (!TransferCase.TrySetCenterLock(on, SpeedKmh, out denyReason))
			{
				return false;
			}
			ApplyTransferCaseState();
			return true;
		}

		public void SetRealisticES(EngineSound res)
		{
			_engineSound = res;
			OnBlowOff -= Blowoff;
			OnBlowOff += Blowoff;
		}

		private void Blowoff()
		{
			_physEngine.GetEngineTurboParams(out var _, out var _, out var blowOff, out var _);
			if (blowOff > 0.1f)
			{
				_engineSound.BlowOff(blowOff);
			}
			_physEngine.BlowOff();
		}

		public void GetDesc(out DynoData desc)
		{
			desc = new DynoData();
			desc.CopyFrom(DynoData);
		}

		public void SetGearboxState(bool value, bool manual = false)
		{
			int num;
			GearboxType effectiveDriveType;
			if ((manual & value) && GearboxState != GearboxState.Burnout)
			{
				effectiveDriveType = EffectiveDriveType;
				num = ((effectiveDriveType == GearboxType.GearFwd || effectiveDriveType == GearboxType.GearRwd) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			bool flag = (byte)num != 0;
			int num2;
			if ((!manual & value) && (GearboxState == GearboxState.Launch || GearboxState == GearboxState.Burnout))
			{
				effectiveDriveType = EffectiveDriveType;
				num2 = ((effectiveDriveType == GearboxType.GearFwd || effectiveDriveType == GearboxType.GearRwd) ? 1 : 0);
			}
			else
			{
				num2 = 0;
			}
			bool flag2 = (byte)num2 != 0;
			if (_cachedGearboxState == value && !flag && !flag2)
			{
				return;
			}
			_cachedGearboxState = value;
			effectiveDriveType = EffectiveDriveType;
			if (effectiveDriveType != GearboxType.GearFwd && effectiveDriveType != GearboxType.GearRwd)
			{
				GearboxState = (_cachedGearboxState ? GearboxState.Launch : GearboxState.None);
				ActivateLaunchControl = _cachedGearboxState;
				return;
			}
			if (_cachedGearboxState)
			{
				if (manual)
				{
					GearboxState = GearboxState.Burnout;
					Clutch = 0f;
					ActivateLaunchControl = false;
					SetAlignCar(state: true);
				}
				else
				{
					float num3 = Mathf.Abs(SpeedKmh);
					if (num3 > 1f)
					{
						if (!(num3 <= 10f))
						{
							goto IL_012e;
						}
						GearboxState = GearboxState.Burnout;
						if (IsActiveLaunchControlNow)
						{
							ActivateLaunchControl = false;
						}
					}
					else
					{
						if (!(num3 <= 1f))
						{
							goto IL_012e;
						}
						if (GearboxState != GearboxState.Burnout)
						{
							ActivateLaunchControl = true;
							GearboxState = GearboxState.Launch;
						}
					}
				}
				goto IL_013c;
			}
			SetAlignCar(state: false);
			ActivateLaunchControl = false;
			_physEngine.GearboxState = GearboxState.None;
			return;
			IL_013c:
			if (DynoData.Transmission.GearType == GearboxType.GearFwd)
			{
				Handbrake = 1f;
			}
			return;
			IL_012e:
			GearboxState = GearboxState.None;
			_cachedGearboxState = false;
			goto IL_013c;
		}

		public void SetAlignPlane(Plane rightPlane, bool usePlane)
		{
			_physEngine.SetAlignPlane(rightPlane, usePlane);
		}

		public void FixVehicle(bool stop)
		{
			_physEngine.SetStopCar(stop);
		}

		public void SetKinematic(bool value)
		{
			Rigidbody.collisionDetectionMode = (value ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete);
			Rigidbody.isKinematic = value;
			foreach (WheelProvider allWheel in GetAxles.AllWheels)
			{
				allWheel.Wheel.Enabled = !value;
			}
			if (!value)
			{
				_physEngine.ResetTransmission(resetTransmission: false);
			}
		}

		public void SetDynorunMode(bool value)
		{
			_rigidbody.constraints = (value ? RigidbodyConstraints.FreezePositionX : RigidbodyConstraints.None);
			FlagsStorage.DynorunMode = value;
		}

		public float GetSteer()
		{
			if (MaxSteeringAngle == 0f)
			{
				return 0f;
			}
			return _physEngine.SteerAngle / MaxSteeringAngle;
		}

		public void SetSteer(float steer, SteerMode mode, bool driftMode = false)
		{
			if (mode == SteerMode.Stabilized)
			{
				_steerAssist.UpdateSteering(0f - steer, Time.deltaTime, driftMode);
			}
			else
			{
				_steerAssist.SetNormalizedSteering(0f - steer);
			}
		}

		public void SetSteer(float steer, bool useAssistent, bool driftMode = false)
		{
			SetSteer(steer, useAssistent ? SteerMode.Stabilized : SteerMode.Raw, driftMode);
		}

		public void GetLaunchControlSettings(out bool isHave, out float lcRpm, out float lcTimer, out float lcHoldTime)
		{
			_physEngine.GetLaunchControlSettings(out isHave, out lcRpm, out lcTimer, out lcHoldTime);
		}

		public async UniTaskVoid BurnOutNow(float time)
		{
			GearboxType effectiveDriveType = EffectiveDriveType;
			if ((effectiveDriveType != GearboxType.GearFwd && effectiveDriveType != GearboxType.GearRwd) || Time.time - _lastBurnOutTime < 60f)
			{
				return;
			}
			bool lastStateTc = _physEngine.TractionControl;
			_physEngine.TractionControl = false;
			if (!_isAllreadyStopped)
			{
				ResetTransmission();
				_isAllreadyStopped = true;
			}
			time += UnityEngine.Random.Range(-2f, 2f);
			await UniTask.Delay(1000 * UnityEngine.Random.Range(0, 2));
			if (_isDestroyed)
			{
				return;
			}
			float endTime = Time.time + time;
			Brake = 1f;
			Throttle = 1f;
			Clutch = 0f;
			GearboxState = GearboxState.Burnout;
			Vector3 posStart = base.transform.position;
			_rigidbody.velocity = Vector3.zero;
			_lastBurnOutTime = Time.time;
			while (Time.time < endTime)
			{
				await UniTask.Delay(100);
				if (_isDestroyed)
				{
					return;
				}
				if (Vector3.Distance(posStart, base.transform.position) > 10f)
				{
					break;
				}
			}
			GearboxState = GearboxState.None;
			if (GetVehicleData.IsRacingCar)
			{
				foreach (WheelProvider allWheel in GetAxles.AllWheels)
				{
					allWheel.Temperature = 125f;
				}
			}
			_isAllreadyStopped = false;
			_physEngine.TractionControl = lastStateTc;
		}

		public float CalcRpmAtSpeedInLastGear(float speed)
		{
			return _physEngine.CalcRpmAtSpeedInLastGear(speed);
		}

		private void OpenBrakePads(float pressure)
		{
			if (!(_brakePressure <= 0f))
			{
				if ((bool)_soundController)
				{
					_soundController.AirBrakes(pressure);
				}
				_brakePressure = 0f;
			}
		}

		private void AirBrakesSimulation(float val)
		{
			if (_vehicleData.isTruck)
			{
				if (IsParkingGear)
				{
					_brakePressure = 1f;
				}
				else if (val > 0.001f)
				{
					_brakePressure += Time.deltaTime;
					_brakePressure = Mathf.Clamp01(_brakePressure);
				}
				else
				{
					OpenBrakePads(_brakePressure);
				}
			}
		}

		public void GearUp()
		{
			_physEngine.GearUp();
		}

		public void GearDown()
		{
			_physEngine.GearDown();
		}

		public int GetOptimalGearForDriveEngagement()
		{
			return _physEngine.GetOptimalGearForDriveEngagement(SpeedMps);
		}

		public void SetToeIn(AxleType axleType, float value)
		{
			switch (axleType)
			{
			case AxleType.Front:
				DynoData.FrontSuspension.ToeIn = value;
				break;
			case AxleType.Rear:
			case AxleType.Rear1:
			case AxleType.Rear2:
				DynoData.RearSuspension.ToeIn = value;
				break;
			}
			ApplySuspDesc2Axle(axleType, (axleType == AxleType.Front) ? DynoData.FrontSuspension : DynoData.RearSuspension);
		}

		public void SetStabSpringCoef(AxleType axleType, float value)
		{
			switch (axleType)
			{
			case AxleType.Front:
				DynoData.FrontSuspension.AntirollBar = value;
				_physEngine.FrontStabSpringCoef = value;
				break;
			case AxleType.Rear:
			case AxleType.Rear1:
			case AxleType.Rear2:
				DynoData.RearSuspension.AntirollBar = value;
				_physEngine.RearStabSpringCoef = value;
				break;
			}
		}

		public void SetCamber(AxleType axleType, float value)
		{
			switch (axleType)
			{
			case AxleType.Front:
				DynoData.FrontSuspension.Camber = value;
				break;
			case AxleType.Rear:
			case AxleType.Rear1:
			case AxleType.Rear2:
				DynoData.RearSuspension.Camber = value;
				break;
			}
			ApplySuspDesc2Axle(axleType, (axleType == AxleType.Front) ? DynoData.FrontSuspension : DynoData.RearSuspension);
		}

		private void CalcCarAngle()
		{
			Vector3 vector = base.transform.InverseTransformDirection(Velocity);
			float num = ((vector.magnitude < 6.95f) ? 0f : (Mathf.Atan2(vector.x, Mathf.Abs(vector.z)) * 57.29578f));
			if (Mathf.Abs(num) < 10f)
			{
				num = 0f;
			}
			_physEngine.ChassisAngle = num;
		}

		public void SetEngineMaxTorque(float tor, float rpm)
		{
			DynoData.Engine.MaxTorque = tor;
			DynoData.Engine.MaxTorqueRpm = rpm;
			_physEngine.SetEngineMaxTorque(tor, rpm, generate: true);
		}

		public void GetTurboSpec(out TurboType type, out float maxTurboPressure, out float2 range)
		{
			maxTurboPressure = Utils.Pa2Bar(DynoData.Engine.TurboPressure);
			range = DynoData.Engine.TurboRange;
			type = DynoData.Engine.TurboType;
		}

		public void SetTrailer(Powertrain trailer, Vector3 loc1, Vector3 loc2, float spring, float damper, float horAngLimit, float screwAngLimit, float hillAngLimit)
		{
			if ((bool)trailer)
			{
				TrailerJoint trailerJoint = new TrailerJoint
				{
					Trailer = trailer.Phys,
					Loc1 = loc1,
					Loc2 = loc2,
					Spring = spring,
					Damper = damper,
					HorAngLimit = horAngLimit,
					ScrewAngLimit = screwAngLimit,
					HillAngLimit = hillAngLimit
				};
				Phys.SetTrailerJoint(trailerJoint);
			}
		}

		public void ResetTransmission(bool resetGear = true)
		{
			_physEngine.ResetTransmission(resetGear);
			if (!_rigidbody.isKinematic)
			{
				_rigidbody.velocity = Vector3.zero;
				_rigidbody.angularVelocity = Vector3.zero;
			}
			_rigidbody.Sleep();
			_rigidbody.WakeUp();
		}

		public void SetGearRatio(int num, float val)
		{
			if (num >= 0 && num < DynoData.GearBox.Gears.Length)
			{
				_physEngine.SetGearRatio(num, val);
				DynoData.GearBox.Gears[num] = val;
			}
		}

		public float GetGearRatio(int num)
		{
			return _physEngine.GetGearRatio(num);
		}

		public void ShutdownEngine(bool showMessage = true)
		{
			_disposeStarter?.Dispose();
			_starter = false;
			Starter = 0f;
			_starterWorks = 0f;
			_physEngine.ShutdownEngine();
			GetVehicleData.EnableCarDisplay(isEngineOn: false);
			GetVehicleData.CarHealthComposition.DamageEngine(state: false);
			ForceSendCarRpm();
		}

		public IWheel GetWheel(WheelIndex w)
		{
			return _physEngine.GetWheel((int)w);
		}

		public void GetEngineTurboParams(out float maxTurboPressure, out float curTurboPressure, out float blowOff, out TurboType turboType)
		{
			_physEngine.GetEngineTurboParams(out maxTurboPressure, out curTurboPressure, out blowOff, out turboType);
		}

		public void RecalculateAirSag()
		{
			float mass = DynoData.Chassis.Mass;
			float frontPercent = DynoData.Chassis.FrontPercent;
			DynoData.FrontSuspension.PneumaticBagSystem.Sag = AirSuspensionMath.Sag(AirSuspensionMath.CornerLoad(mass, frontPercent, front: true), DynoData.FrontSuspension.Stiffness);
			DynoData.RearSuspension.PneumaticBagSystem.Sag = AirSuspensionMath.Sag(AirSuspensionMath.CornerLoad(mass, frontPercent, front: false), DynoData.RearSuspension.Stiffness);
		}

		public float GetFrontAirSag()
		{
			return DynoData.FrontSuspension.PneumaticBagSystem.Sag;
		}

		public float GetRearAirSag()
		{
			return DynoData.RearSuspension.PneumaticBagSystem.Sag;
		}

		public float2 GetSpringSpecs(AxleType axleType)
		{
			DynoData.Suspension suspension = ((axleType == AxleType.Front) ? DynoData.FrontSuspension : DynoData.RearSuspension);
			return new float2(suspension.SpringLength, suspension.Stiffness);
		}

		public float GetFrontStockClearance()
		{
			return AirSuspensionMath.SpringLenToClearance(DynoData.FrontSuspension.SpringLength, DynoData.FrontSuspension.PneumaticBagSystem.Sag);
		}

		public float GetRearStockClearance()
		{
			return AirSuspensionMath.SpringLenToClearance(DynoData.RearSuspension.SpringLength, DynoData.RearSuspension.PneumaticBagSystem.Sag);
		}

		public void SetFrontAirBagLimit(float2 preference)
		{
			DynoData.FrontSuspension.PneumaticBagSystem.SuspensionHeight = preference;
		}

		public void SetRearAirBagLimit(float2 preference)
		{
			DynoData.RearSuspension.PneumaticBagSystem.SuspensionHeight = preference;
		}

		public void ApplyAirSuspension()
		{
			if (!_vehicleData || _vehicleData.carInfo == null)
			{
				GetAxles.HasPneumaticSuspension = false;
				UpdateAirSuspensionBank(hasAirSuspension: false);
				return;
			}
			AdditionalData additionalData = _vehicleData.carInfo.AdditionalData;
			int num;
			if (additionalData != null)
			{
				PsData airSuspension = additionalData.AirSuspension;
				if (airSuspension != null && airSuspension.Bought)
				{
					num = (airSuspension.Installed ? 1 : 0);
					goto IL_005d;
				}
			}
			num = 0;
			goto IL_005d;
			IL_005d:
			bool flag = (byte)num != 0;
			GetAxles.HasPneumaticSuspension = flag;
			UpdateAirSuspensionBank(flag);
			RecalculateAirSag();
			ApplyAirClearance(DynoData.FrontSuspension, flag);
			ApplyAirClearance(DynoData.RearSuspension, flag);
		}

		private static float2 SpringLenLimits(DynoData.Suspension suspension)
		{
			DynoData.PneumaticBagSystem pneumaticBagSystem = suspension.PneumaticBagSystem;
			return AirSuspensionMath.ResolveSpringLenLimits(pneumaticBagSystem.MinSpringLen, pneumaticBagSystem.MaxSpringLen, suspension.SpringLength);
		}

		private static void ApplyAirClearance(DynoData.Suspension suspension, bool hasAirSuspension)
		{
			DynoData.PneumaticBagSystem pneumaticBagSystem = suspension.PneumaticBagSystem;
			if (!hasAirSuspension)
			{
				pneumaticBagSystem.TargetHeight = AirSuspensionMath.SpringLenToClearance(suspension.SpringLength, pneumaticBagSystem.Sag);
				pneumaticBagSystem.CurrentSpringLength = suspension.SpringLength;
				return;
			}
			float2 float5 = SpringLenLimits(suspension);
			float2 float6 = AirSuspensionMath.EffectiveLimits(float5.x, float5.y, pneumaticBagSystem.Sag, pneumaticBagSystem.SuspensionHeight);
			float value = AirSuspensionMath.SpringLenToClearance(suspension.SpringLength, pneumaticBagSystem.Sag);
			pneumaticBagSystem.TargetHeight = ((pneumaticBagSystem.TargetHeight < 0f) ? Mathf.Clamp(value, float6.x, float6.y) : Mathf.Clamp(pneumaticBagSystem.TargetHeight, float6.x, float6.y));
			pneumaticBagSystem.CurrentSpringLength = AirSuspensionMath.ClearanceToSpringLen(pneumaticBagSystem.TargetHeight, pneumaticBagSystem.Sag, float5.y);
		}

		private void UpdateAirSuspensionBank(bool hasAirSuspension)
		{
			if (hasAirSuspension != _airSuspensionBankHeld && _soundBankService != null)
			{
				if (hasAirSuspension)
				{
					_ = _soundBankService.TryLoadBank(SoundBankType.CarAirSuspension);
				}
				else
				{
					_ = _soundBankService.TryUnloadBank(SoundBankType.CarAirSuspension);
				}
				_airSuspensionBankHeld = hasAirSuspension;
			}
		}

		private void UpdateCompressorBank(bool hasCompressor)
		{
			if (hasCompressor != _compressorBankHeld && _soundBankService != null)
			{
				if (hasCompressor)
				{
					_ = _soundBankService.TryLoadBank(SoundBankType.CarCompressor);
				}
				else
				{
					_ = _soundBankService.TryUnloadBank(SoundBankType.CarCompressor);
				}
				_compressorBankHeld = hasCompressor;
			}
		}

		public void UpdateCompressorPower(float value)
		{
			DynoData.FrontSuspension.PneumaticBagSystem.HeightAdjustSpeed = value;
			DynoData.RearSuspension.PneumaticBagSystem.HeightAdjustSpeed = value;
		}

		public float GetHeightAdjustSpeed()
		{
			return DynoData.RearSuspension.PneumaticBagSystem.HeightAdjustSpeed;
		}

		public float GetFrontTargetHeight()
		{
			return DynoData.FrontSuspension.PneumaticBagSystem.TargetHeight;
		}

		public float2 GetFrontAirBagLimit()
		{
			float2 float5 = SpringLenLimits(DynoData.FrontSuspension);
			return AirSuspensionMath.EffectiveLimits(float5.x, float5.y, DynoData.FrontSuspension.PneumaticBagSystem.Sag, DynoData.FrontSuspension.PneumaticBagSystem.SuspensionHeight);
		}

		public void SetFrontTargetHeight(float clearance)
		{
			DynoData.PneumaticBagSystem pneumaticBagSystem = DynoData.FrontSuspension.PneumaticBagSystem;
			float2 frontAirBagLimit = GetFrontAirBagLimit();
			clearance = Mathf.Clamp(clearance, frontAirBagLimit.x, frontAirBagLimit.y);
			pneumaticBagSystem.TargetHeight = clearance;
			Axles getAxles = GetAxles;
			if (getAxles.FrontAxle.Wheels.Count > 0)
			{
				getAxles.FrontAxle.TargetClearance = clearance;
			}
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.CurrentFrontSuspensionHeight = Mathf.Round(clearance * 1000f);
		}

		public float2 GetRearAirBagLimit()
		{
			float2 float5 = SpringLenLimits(DynoData.RearSuspension);
			return AirSuspensionMath.EffectiveLimits(float5.x, float5.y, DynoData.RearSuspension.PneumaticBagSystem.Sag, DynoData.RearSuspension.PneumaticBagSystem.SuspensionHeight);
		}

		public float GetRearTargetHeight()
		{
			return DynoData.RearSuspension.PneumaticBagSystem.TargetHeight;
		}

		public void SetRearTargetHeight(float clearance)
		{
			DynoData.PneumaticBagSystem pneumaticBagSystem = DynoData.RearSuspension.PneumaticBagSystem;
			float2 rearAirBagLimit = GetRearAirBagLimit();
			clearance = Mathf.Clamp(clearance, rearAirBagLimit.x, rearAirBagLimit.y);
			pneumaticBagSystem.TargetHeight = clearance;
			Axles getAxles = GetAxles;
			if (getAxles.RearAxle.Wheels.Count > 0)
			{
				getAxles.RearAxle.TargetClearance = clearance;
			}
			if (getAxles.Rear1Axle.Wheels.Count > 0)
			{
				getAxles.Rear1Axle.TargetClearance = clearance;
			}
			if (getAxles.Rear2Axle.Wheels.Count > 0)
			{
				getAxles.Rear2Axle.TargetClearance = clearance;
			}
			_vehicleEntityProvider.CarEntity.Engine.DynamicTuningSettings.CurrentRearSuspensionHeight = Mathf.Round(clearance * 1000f);
		}

		public void SetSpacers(AxleType axleType, float value)
		{
			if (axleType == AxleType.Front)
			{
				DynoData.FrontSuspension.Spacers = value;
				ApplySuspDesc2Axle(AxleType.Front, DynoData.FrontSuspension);
			}
			if (axleType == AxleType.Rear)
			{
				DynoData.RearSuspension.Spacers = value;
				ApplySuspDesc2Axle(AxleType.Rear, DynoData.RearSuspension);
			}
		}

		public void UpdateSpacers(AxleType axle)
		{
			if (!GetAxles)
			{
				return;
			}
			foreach (WheelProvider wheel in ((axle == AxleType.Front) ? GetAxles.FrontAxle : GetAxles.RearAxle).Wheels)
			{
				if ((bool)wheel.WheelModel)
				{
					wheel.WheelModel.SpacerWidth = ((axle == AxleType.Front) ? DynoData.FrontSuspension.Spacers : DynoData.RearSuspension.Spacers);
				}
			}
			Axles getAxles = GetAxles;
			if (axle != AxleType.Rear)
			{
				return;
			}
			if (getAxles.Rear1Axle.Wheels.Count > 1)
			{
				foreach (WheelProvider wheel2 in getAxles.Rear1Axle.Wheels)
				{
					if ((bool)wheel2.WheelModel)
					{
						wheel2.WheelModel.SpacerWidth = DynoData.RearSuspension.Spacers;
					}
				}
			}
			if (getAxles.Rear2Axle.Wheels.Count <= 1)
			{
				return;
			}
			foreach (WheelProvider wheel3 in getAxles.Rear2Axle.Wheels)
			{
				wheel3.WheelModel.SpacerWidth = DynoData.RearSuspension.Spacers;
			}
		}

		public void SetSprings(AxleType axleType, float3 specs)
		{
			bool num = axleType == AxleType.Front;
			Axles getAxles = GetAxles;
			bool flag = false;
			int num2;
			if ((bool)_vehicleData && _vehicleData.carInfo != null)
			{
				AdditionalData additionalData = _vehicleData.carInfo.AdditionalData;
				if (additionalData != null)
				{
					PsData airSuspension = additionalData.AirSuspension;
					if (airSuspension != null && airSuspension.Bought)
					{
						num2 = (airSuspension.Installed ? 1 : 0);
						goto IL_0056;
					}
				}
				num2 = 0;
				goto IL_0056;
			}
			goto IL_0057;
			IL_0057:
			if (num)
			{
				DynoData.FrontSuspension.SpringLength = specs.x;
				DynoData.FrontSuspension.Stiffness = specs.y;
				RecalculateAirSag();
				DynoData.FrontSuspension.PneumaticBagSystem.TargetHeight = (flag ? specs.z : (-1f));
				ApplyAirClearance(DynoData.FrontSuspension, flag);
				ApplySuspDesc2Axle(AxleType.Front, DynoData.FrontSuspension);
				return;
			}
			DynoData.RearSuspension.SpringLength = specs.x;
			DynoData.RearSuspension.Stiffness = specs.y;
			RecalculateAirSag();
			DynoData.RearSuspension.PneumaticBagSystem.TargetHeight = (flag ? specs.z : (-1f));
			ApplyAirClearance(DynoData.RearSuspension, flag);
			ApplySuspDesc2Axle(AxleType.Rear, DynoData.RearSuspension);
			if (getAxles.Rear1Axle.Wheels.Count > 0)
			{
				ApplySuspDesc2Axle(AxleType.Rear1, DynoData.RearSuspension);
			}
			if (getAxles.Rear2Axle.Wheels.Count > 0)
			{
				ApplySuspDesc2Axle(AxleType.Rear2, DynoData.RearSuspension);
			}
			return;
			IL_0056:
			flag = (byte)num2 != 0;
			goto IL_0057;
		}

		public void SetShockAbsorbers(AxleType axleType, float2 specs)
		{
			bool num = axleType == AxleType.Front;
			Axles getAxles = GetAxles;
			if (num)
			{
				DynoData.FrontSuspension.Bump = specs.x;
				DynoData.FrontSuspension.Rebound = specs.y;
				ApplySuspDesc2Axle(AxleType.Front, DynoData.FrontSuspension);
				return;
			}
			DynoData.RearSuspension.Bump = specs.x;
			DynoData.RearSuspension.Rebound = specs.y;
			ApplySuspDesc2Axle(AxleType.Rear, DynoData.RearSuspension);
			if (getAxles.Rear1Axle.Wheels.Count > 0)
			{
				ApplySuspDesc2Axle(AxleType.Rear1, DynoData.RearSuspension);
			}
			if (getAxles.Rear2Axle.Wheels.Count > 0)
			{
				ApplySuspDesc2Axle(AxleType.Rear2, DynoData.RearSuspension);
			}
		}

		private void SetBrakeParams(Axle axle)
		{
			if (axle == GetAxles.FrontAxle)
			{
				axle.SetBrakeDisk(DynoData.FrontSuspension.BrakeDiscType, DynoData.FrontSuspension.BrakeDiskMass, DynoData.FrontSuspension.BrakeDiskTorque);
			}
			else
			{
				axle.SetBrakeDisk(DynoData.RearSuspension.BrakeDiscType, DynoData.RearSuspension.BrakeDiskMass, DynoData.RearSuspension.BrakeDiskTorque);
			}
		}

		public void UpdatePhysicsSleep()
		{
			Rigidbody rigidbody = Rigidbody;
			if (!rigidbody || !rigidbody.isKinematic)
			{
				_physWarmupStepsDone = 0;
				PhysicsAsleep = false;
			}
			else if (_physWarmupStepsDone < 3)
			{
				_physWarmupStepsDone++;
				PhysicsAsleep = false;
			}
			else
			{
				PhysicsAsleep = true;
			}
		}

		public void UpdateWheels()
		{
			if (PhysicsAsleep)
			{
				_physEngine.AdvanceWheelRollAnglesWhileAsleep(Time.deltaTime);
			}
			_physEngine.UpdateWheels(base.transform.localToWorldMatrix);
			Axles getAxles = GetAxles;
			UpdateWheelTransform(WheelIndex.FrontLeft, getAxles.FrontAxle.LeftWheel);
			UpdateWheelTransform(WheelIndex.FrontRight, getAxles.FrontAxle.RightWheel);
			UpdateWheelTransform(WheelIndex.RearLeft, getAxles.RearAxle.LeftWheel);
			UpdateWheelTransform(WheelIndex.RearRight, getAxles.RearAxle.RightWheel);
			if ((bool)getAxles.Rear1Axle.LeftWheel)
			{
				UpdateWheelTransform(WheelIndex.Rear1Left, getAxles.Rear1Axle.LeftWheel);
			}
			if ((bool)getAxles.Rear1Axle.RightWheel)
			{
				UpdateWheelTransform(WheelIndex.Rear1Right, getAxles.Rear1Axle.RightWheel);
			}
			if ((bool)getAxles.Rear2Axle.LeftWheel)
			{
				UpdateWheelTransform(WheelIndex.Rear2Left, getAxles.Rear2Axle.LeftWheel);
			}
			if ((bool)getAxles.Rear2Axle.RightWheel)
			{
				UpdateWheelTransform(WheelIndex.Rear2Right, getAxles.Rear2Axle.RightWheel);
			}
			_onUpdateWheelsInvoker?.Invoke(this);
		}

		public void UpdateChassis()
		{
			if (TrafficMode || IsTrailer || TowCar)
			{
				return;
			}
			if (!OnLineCar)
			{
				float speedMps = SpeedMps;
				if (speedMps > 135f)
				{
					float num = speedMps - 135f;
					float drag = Mathf.Lerp(0f, 2f, num / 135f);
					_rigidbody.drag = drag;
					_rigidbody.angularDrag += 0.02f;
				}
				else
				{
					_rigidbody.drag = 0f;
					_rigidbody.angularDrag = 0f;
				}
				if (_starter)
				{
					StarterSimulation(Time.deltaTime);
				}
				CalcCarAngle();
				if (!_isRaceCar && (bool)_vehicleEntityProvider && !_fuelInitialized)
				{
					_physEngine.CurrentFuel = _vehicleEntityProvider.GetFuelInTank;
					_fuelInitialized = true;
				}
				CalcDrift();
				UpdateOdometer();
				UpdateAvgFuelConsumption();
				UpdateEngineThermal();
			}
			_onUpdateChassisInvoker?.Invoke(this);
		}

		private bool OnOffRoad()
		{
			List<WheelProvider> allWheels = GetAxles.AllWheels;
			for (int i = 0; i < allWheels.Count; i++)
			{
				PhysicMaterial currentPhysMat = allWheels[i].CurrentPhysMat;
				if (currentPhysMat == null || !currentPhysMat.SandSmoke)
				{
					return false;
				}
			}
			return true;
		}

		public void CancelDrift()
		{
			DistanceCurrentDrifted = 0f;
			_driftTimeCount = -0.1f;
		}

		private void CalcDrift()
		{
			if (base.gameObject != V.Car || IsCruiseControlActive || Gear < 1 || !V.CoreGameManager || !V.CoreGameManager.MenuLevelParent || !V.CoreGameManager.MenuLevelParent.driftSign)
			{
				return;
			}
			GameObject driftSign = V.CoreGameManager.MenuLevelParent.driftSign;
			if (!FlagsStorage.OutOfCar)
			{
				if (_skipDistanceTravel > 5)
				{
					_skipDistanceTravel = 0;
					float2 float5 = new float2(base.transform.localPosition.x, base.transform.localPosition.z);
					float num = math.distance(float5, _lastPosition) * 0.001f;
					if (num < 40f && !TowCar)
					{
						DistanceSession = (float)DistanceSession + num;
						if (OnOffRoad())
						{
							DistanceOffroadTravelled = (float)DistanceOffroadTravelled + num;
						}
					}
					_lastPosition = float5;
					if (_driftTimeCount >= 0f && SpeedMps > 0.5f)
					{
						_driftTimeCount -= Time.deltaTime;
					}
					else
					{
						_driftingNow = false;
						SetDriftUiActive(driftSign, active: false);
					}
					if ((bool)AlwaysDriftForDebug || SpeedMps > 2.77f)
					{
						float chassisAngle = _physEngine.ChassisAngle;
						if ((bool)AlwaysDriftForDebug || Mathf.Abs(chassisAngle) > 15f)
						{
							DistanceCurrentDrifted = (float)DistanceCurrentDrifted + num;
							_driftingNow = true;
							if ((bool)driftSign && (float)DistanceCurrentDrifted * 1000f > 9.6f)
							{
								SetDriftUiActive(V.CoreGameManager.MenuLevelParent.driftSign, active: true);
								int num2 = Mathf.CeilToInt((float)DistanceCurrentDrifted * 1000f);
								if ((bool)V.CoreGameManager.MenuLevelParent.driftDistanceLabel)
								{
									V.CoreGameManager.MenuLevelParent.driftDistanceLabel.text = $"{Utils.MetersToFeet(num2):F0}{Utils.CurrentDriftMetric}";
								}
								_driftTimeCount = 0.5f;
								DistanceDrifted = (float)DistanceDrifted + num;
							}
						}
					}
					if (_driftingNow || !((float)DistanceCurrentDrifted > 0f))
					{
						return;
					}
					bool flag = false;
					if ((float)DistanceCurrentDrifted > (float)MaxDriftDistance)
					{
						MaxDriftDistance = DistanceCurrentDrifted;
						ObscuredPrefs.SetFloat("maxDriftDistance", MaxDriftDistance);
						int num3 = Mathf.CeilToInt((float)MaxDriftDistance * 1000f);
						if ((bool)_freeDriveProvider.FreeDriveDB)
						{
							FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
							float? drift_max = num3;
							freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, drift_max)).Forget();
						}
						flag = true;
					}
					if (_dailyTasksInfoContainer.HasDriftMaxTask && (float)DistanceCurrentDrifted > (float)MaxDriftDistanceForDailyTask)
					{
						MaxDriftDistanceForDailyTask = DistanceCurrentDrifted;
						int num4 = Mathf.CeilToInt((float)MaxDriftDistanceForDailyTask * 1000f);
						float? drift_max = num4;
						RatingData rating = new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, drift_max);
						if (!flag && (bool)_freeDriveProvider.FreeDriveDB)
						{
							_freeDriveProvider.FreeDriveDB.SendRatingData(rating).Forget();
						}
					}
					DistanceCurrentDrifted = 0f;
				}
				else
				{
					_skipDistanceTravel++;
				}
			}
			else
			{
				SetDriftUiActive(driftSign, active: false);
			}
		}

		private static void SetDriftUiActive(GameObject driftUI, bool active)
		{
			if ((bool)driftUI && driftUI.activeSelf != active)
			{
				driftUI.SetActive(active);
			}
		}

		private void OnEnable()
		{
			if ((bool)_cpmVehicleCore)
			{
				_cpmVehicleCore.AddCar(this);
			}
			OnEngineStarted += StopStarter;
		}

		private void OnDisable()
		{
			if ((bool)_cpmVehicleCore)
			{
				_cpmVehicleCore.RemoveCar(this);
				OnEngineStarted -= StopStarter;
			}
		}

		private void OnDestroy()
		{
			UpdateAirSuspensionBank(hasAirSuspension: false);
			UpdateCompressorBank(hasCompressor: false);
			_isDestroyed = true;
			_wheelRayTracer?.Destroy();
			_onPreSimulationInvoker = null;
			_onPostSimulationInvoker = null;
			_onUpdateWheelsInvoker = null;
			if (Axles != null)
			{
				Axles.AllWheels?.Clear();
			}
		}

		private void OnDrawGizmos()
		{
			Bounds weightBounds = _weightBounds;
			Gizmos.matrix = base.transform.localToWorldMatrix;
			float z = weightBounds.min.z;
			float z2 = weightBounds.max.z;
			if (Axles != null && Axles.FrontAxle != null && (bool)Axles.FrontAxle.LeftWheel && Axles.RearAxle != null && (bool)Axles.RearAxle.LeftWheel)
			{
				z = Axles.RearAxle.LeftWheel.transform.localPosition.z;
				z2 = Axles.FrontAxle.LeftWheel.transform.localPosition.z;
			}
			Gizmos.DrawWireSphere(new Vector3(0f, weightBounds.min.y + weightBounds.size.y * DynoData.Chassis.UpPercent * 0.01f, z + (z2 - z) * DynoData.Chassis.FrontPercent * 0.01f), 0.1f);
			Gizmos.DrawWireCube(weightBounds.center, weightBounds.size);
		}

		[Inject]
		public void Construct(CPMVehicleCore cpmVehicleCore, FreeDriveProvider freeDriveProvider, DailyTasksInfoContainer dailyTasksInfoContainer, SoundBankLoadService soundBankService)
		{
			_cpmVehicleCore = cpmVehicleCore;
			_freeDriveProvider = freeDriveProvider;
			_dailyTasksInfoContainer = dailyTasksInfoContainer;
			_soundBankService = soundBankService;
		}

		public void SetTrafficToWorld(CPMVehicleCore cpmVehicleCore)
		{
			_cpmVehicleCore = cpmVehicleCore;
			_cpmVehicleCore.AddCar(this);
		}

		private void Awake()
		{
			InitializationChassis();
			SpeedKmhReactive = _speedMps.Select((float v) => v * 3.6f).ToReactiveProperty();
			SpeedMphReactive = _speedMps.Select((float v) => v * 2.237f).ToReactiveProperty();
			if ((bool)_cpmVehicleCore)
			{
				_cpmVehicleCore.AddCar(this);
			}
		}

		private void Start()
		{
			MaxDriftDistance = ObscuredPrefs.GetFloat("maxDriftDistance", 0.01f);
			if (!ScenesHelper.IsMenuSceneActive() && !ScenesHelper.IsParkingLevelActive() && V.Car == base.gameObject)
			{
				StartEngine(starterSound: false);
			}
		}

		private void FindWeightBounds()
		{
			if ((bool)Axles && Axles.FrontAxle != null && (bool)Axles.FrontAxle.LeftWheel && Axles.RearAxle != null && (bool)Axles.RearAxle.LeftWheel)
			{
				_wheelBase = Vector3.Distance(Axles.FrontAxle.LeftWheel.transform.localPosition, Axles.RearAxle.LeftWheel.transform.localPosition);
				_frontTrack = Axles.FrontAxle.LeftWheel.transform.localPosition.z - _wheelBase * (1f - DynoData.Chassis.FrontPercent * 0.01f);
			}
			if (_weightBounds.size.z > 1f)
			{
				return;
			}
			Vector3 size = _dimensions;
			Vector3 center = Vector3.zero;
			if ((bool)Collider)
			{
				Vector3 position = Vector3.zero;
				Collider collider = Collider;
				if (!(collider is MeshCollider meshCollider))
				{
					if (collider is BoxCollider boxCollider)
					{
						size = boxCollider.size;
						Vector3 center2 = boxCollider.center;
						position = boxCollider.transform.TransformPoint(center2);
					}
				}
				else
				{
					Vector3 size2 = meshCollider.sharedMesh.bounds.size;
					float y = Mathf.Min(size2.y, size2.z);
					float z = Mathf.Max(size2.y, size2.z);
					size = new Vector3(size2.x, y, z);
					Vector3 center2 = meshCollider.sharedMesh.bounds.center;
					position = meshCollider.transform.TransformPoint(center2);
				}
				center = base.transform.InverseTransformPoint(position);
			}
			_weightBounds = new Bounds(center, size);
		}

		public void ManualInit()
		{
			InitializationChassis();
		}

		private void InitializationChassis()
		{
			_steerAssist = new SteerAssist();
			_physEngine = new PhysEngine
			{
				PhysSteps = (IsRaceCar ? 8 : 10)
			};
			_wheelRayTracer = new WheelTracer(WheelSurfaceConfigContainer.rideableMeshesGraph, Collider);
			TryGetComponent<VehicleEntityProvider>(out _vehicleEntityProvider);
			TryGetComponent<SoundController>(out _soundController);
			_physEngine.SetFront(0, 1);
			_physEngine.SetRear(2, 3);
			_physEngine.SetRear1(4, 5);
			_physEngine.SetRear2(6, 7);
			_physEngine.ManualHelper = PlayerPrefs.GetInt("ManualGearboxAssist") > 0;
			FindWeightBounds();
			SetWheelTransform();
		}

		private void SetWheelTransform()
		{
			InitWheel(WheelIndex.FrontLeft, Axles.FrontAxle.LeftWheel);
			InitWheel(WheelIndex.FrontRight, Axles.FrontAxle.RightWheel);
			InitWheel(WheelIndex.RearLeft, Axles.RearAxle.LeftWheel);
			InitWheel(WheelIndex.RearRight, Axles.RearAxle.RightWheel);
			InitWheel(WheelIndex.Rear1Left, Axles.Rear1Axle.LeftWheel);
			InitWheel(WheelIndex.Rear1Right, Axles.Rear1Axle.RightWheel);
			InitWheel(WheelIndex.Rear2Left, Axles.Rear2Axle.LeftWheel);
			InitWheel(WheelIndex.Rear2Right, Axles.Rear2Axle.RightWheel);
			SetRigidbody();
		}

		public float CalcTimeForDistance(float targetDistance, Surface surface)
		{
			return _physEngine.CalcTimeForDistance(targetDistance, surface);
		}

		public float CalcTopSpeed()
		{
			return _physEngine.CalcTopSpeed();
		}

		public float CalcAccelTime(float sp1, float sp2, Surface surface)
		{
			return _physEngine.CalcAccelTime(sp1, sp2, surface);
		}

		public float CalcBrakeDistance(float speedFrom, Surface surface)
		{
			return _physEngine.CalcBrakeDistance(speedFrom, surface);
		}

		public void CalcMaxAccel(int ngear, float vel, Surface surface, out float dragAccelEng, out float dragAccelTyre, out float brakeAccelBR, out float brakeAccelTyre, out float brakeAccelAir, out float sideAccel)
		{
			_physEngine.CalcMaxAccel(ngear, vel, surface, out dragAccelEng, out dragAccelTyre, out brakeAccelBR, out brakeAccelTyre, out brakeAccelAir, out sideAccel);
		}

		public void CalcMaxPowerMaxTorque(out float maxHp, out float maxHpRpm, out float maxTq, out float maxTqRpm, out float maxWhp)
		{
			_physEngine.CalcMaxPowerMaxTorque(out maxHp, out maxHpRpm, out maxTq, out maxTqRpm, out maxWhp);
		}

		public void GetDynoGraphAtRpm(float rpm, out float torq, out float wtorq, out float hp, out float whp)
		{
			_physEngine.GetDynoGraphAtRpm(rpm, out torq, out wtorq, out hp, out whp);
		}

		public void SetEngineTorqueCurveSample(TorqueCurveSample[] samples)
		{
			_physEngine.SetEngineTorqueSamples(samples);
		}

		public void SetDynoData(DynoData desc, bool apply)
		{
			DynoData.CopyFrom(desc);
			UpdateCompressorBank(DynoData.Engine.TurboType != TurboType.NA);
			if (apply)
			{
				ApplyDynoData();
			}
		}

		public void GetDynoData(out DynoData desc)
		{
			desc = new DynoData();
			desc.CopyFrom(DynoData);
		}

		public void StartEngine(bool starterSound)
		{
			if (TrafficMode || IsRaceCar)
			{
				_physEngine.StartEngine();
			}
			else
			{
				if (_physEngine.IsEngineWorks || _physEngine.Starter > 0f)
				{
					return;
				}
				_starter = true;
				float num = 0.5f;
				if (starterSound)
				{
					_disposeStarter?.Dispose();
					if ((bool)GetVehicleData && (!GetVehicleData.IsRacingCar || !TrafficMode) && (bool)_engineSound)
					{
						_disposeStarter = Observable.Timer(TimeSpan.FromSeconds(num)).Subscribe((long _) =>
						{
							_engineSound.PlayEngineStartSound();
						}).AddTo(this);
					}
				}
				else if (Fuel)
				{
					_physEngine.StartEngine();
				}
				if ((bool)GetVehicleData)
				{
					GetVehicleData.EnableCarDisplay(isEngineOn: true);
				}
				ForceSendCarRpm();
			}
		}

		private void ForceSendCarRpm()
		{
			if (FlagsStorage.IsMultiFree && PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null && (bool)_freeDriveProvider.MultiPlayer && GetVehicleData.photonView.IsMine)
			{
				GetVehicleData.mppc.ForceSendNextStream(resetIgnore: true);
			}
		}

		public void ApplySuspensionData()
		{
			_physEngine.Ackerman = DynoData.FrontSuspension.Ackerman * 0.01f;
			_physEngine.FrontStabSpringCoef = DynoData.FrontSuspension.AntirollBar;
			_physEngine.RearStabSpringCoef = DynoData.RearSuspension.AntirollBar;
			BrakeFrontDiskTorque = DynoData.FrontSuspension.BrakeDiskTorque;
			BrakeRearDiskTorque = DynoData.RearSuspension.BrakeDiskTorque;
			_physEngine.FrontSuspensionType = DynoData.FrontSuspension.SuspensionType;
			_physEngine.RearSuspensionType = DynoData.RearSuspension.SuspensionType;
			_physEngine.Rear1SuspensionType = DynoData.RearSuspension.SuspensionType;
			_physEngine.Rear2SuspensionType = DynoData.RearSuspension.SuspensionType;
			_physEngine.BrakeFrontBalance = (float)DynoData.Chassis.BrakeBalance * 0.01f;
			_physEngine.MaxSteerAngle = DynoData.FrontSuspension.MaxSteerAngle * ((float)Math.PI / 180f);
			ApplyAirSuspension();
			ApplySuspDesc2Axle(AxleType.Front, DynoData.FrontSuspension);
			ApplySuspDesc2Axle(AxleType.Rear, DynoData.RearSuspension);
			Axles getAxles = GetAxles;
			if (getAxles.Rear1Axle.Wheels.Count == 2)
			{
				ApplySuspDesc2Axle(AxleType.Rear1, DynoData.RearSuspension);
			}
			if (getAxles.Rear2Axle.Wheels.Count == 2)
			{
				ApplySuspDesc2Axle(AxleType.Rear2, DynoData.RearSuspension);
			}
			ApplyTyres2Axle(AxleType.Front, DynoData.FrontTyre);
			ApplyTyres2Axle(AxleType.Rear, DynoData.RearTyre);
			SetBrakeParams(getAxles.FrontAxle);
			SetBrakeParams(getAxles.RearAxle);
			if (getAxles.Rear1Axle.Wheels.Count == 2)
			{
				SetBrakeParams(Axles.Rear1Axle);
			}
			if (getAxles.Rear2Axle.Wheels.Count == 2)
			{
				SetBrakeParams(Axles.Rear2Axle);
			}
			_steerAssist.AttachCar(_physEngine);
			Phys.SetEngineOrientation(DynoData.Engine.Orientation);
		}

		public void ApplyEngineData()
		{
			SetRigidbody();
			ApplyAeroDynamic();
			_physEngine.EngineInertiaRatio = DynoData.Engine.InertiaRatio;
			_physEngine.DisplacementCc = DynoData.Engine.Displacement;
			_physEngine.IsRotaryEngine = DynoData.Engine.IsRotary;
			_physEngine.SetRpmController(DynoData.Engine.IdleRpm, DynoData.Engine.RedlineRpm, DynoData.Engine.MaxRpm);
			_physEngine.SetEngineTurboParams(Utils.Pa2Bar(DynoData.Engine.TurboPressure), DynoData.Engine.TurboRange, DynoData.Engine.TurboType);
			_physEngine.EngineStarterRpm(Mathf.Max(DynoData.Engine.IdleRpm - 500f, 300f), DynoData.Engine.IdleRpm + 500f);
			_physEngine.SetLaunchControl(DynoData.Engine.LcRpm, DynoData.Engine.LcTime, DynoData.Engine.Options.Lc);
			_physEngine.FuelConsumptionSpeed = DynoData.Engine.FuelConsumptionSpeed;
			_physEngine.FuelConsumptionAtCostantSpeed = DynoData.Engine.FuelConsumptionAtCostantSpeed;
			SetGearType(DynoData.Transmission.GearType);
			_physEngine.TorqueSplit = DynoData.Transmission.TorqueSplit;
			_physEngine.GearShiftingDelay = DynoData.GearBox.GearShiftingDelay;
			_physEngine.GearBoxInertiaRatio = DynoData.GearBox.InertiaRatio;
			_physEngine.FrontLsd = DynoData.Lsd.FrontLSD;
			_physEngine.RearLsd = DynoData.Lsd.RearLSD;
			_physEngine.DiffLossCoef = DynoData.Transmission.DiffLossCoef;
			_physEngine.TransmissionLossCoef = DynoData.Transmission.TransmissionLossCoef;
			if (!FlagsStorage.DynorunMode)
			{
				GearboxProfile profile = GearboxProfile.For(DynoData.Transmission.Kind);
				_physEngine.At = profile.HasTorqueConverter;
				_physEngine.AutoClutch = profile.DefaultAutoClutch;
				_isAutomaticGearbox = profile.DefaultAutoClutch;
				_physEngine.ResetThrottleOverride();
				_physEngine.ResetDualClutch();
				_physEngine.SetTransmissionBehavior(TransmissionBehaviorFactory.Create(DynoData.Transmission.Kind, in profile), profile);
				_physEngine.LowMaxGear = ((DynoData.Transmission.LowMaxGear > 0) ? DynoData.Transmission.LowMaxGear : 2);
			}
			_physEngine.FinalDrive = DynoData.GearBox.FinalDrive;
			_physEngine.NumGears = DynoData.GearBox.Gears.Length - 2;
			for (int i = 0; i < DynoData.GearBox.Gears.Length; i++)
			{
				_physEngine.SetGearRatio(i, DynoData.GearBox.Gears[i]);
			}
			if (DynoData.Engine.TorqueCurveSamples.Length != 0)
			{
				_physEngine.ClutchLimit = DynoData.Transmission.ClutchMaxTorque;
				_physEngine.SetEngineTorqueSamples(DynoData.Engine.TorqueCurveSamples);
				_physEngine.SetEngineMaxTorque(DynoData.Engine.MaxTorque, DynoData.Engine.MaxTorqueRpm);
			}
			else
			{
				_physEngine.SetEngineMaxTorque(DynoData.Engine.MaxTorque, DynoData.Engine.MaxTorqueRpm, generate: true);
			}
			ApplyAssist();
			ApplyTransferCaseState();
		}

		public void ApplyDynoData()
		{
			ApplySuspensionData();
			ApplyEngineData();
			_onApplyDynoDataInvoker?.Invoke(this);
		}

		private void ApplyAeroDynamic()
		{
			_physEngine.AeroCx = DynoData.Aero.Cx;
			_physEngine.AeroSx = DynoData.Aero.Sx;
			_physEngine.AeroFrontDownforce = DynoData.Aero.FrontDownforce;
			_physEngine.AeroRearDownforce = DynoData.Aero.RearDownforce;
		}

		private void ApplyGearTypeToWheel(Axle axle, bool reset = true, bool powered = true)
		{
			if (reset)
			{
				foreach (WheelProvider allWheel in GetAxles.AllWheels)
				{
					allWheel.Wheel.IsPowered = false;
				}
			}
			if (!powered)
			{
				return;
			}
			foreach (WheelProvider wheel in axle.Wheels)
			{
				wheel.Wheel.IsPowered = true;
			}
		}

		private void SetGearType(GearboxType type)
		{
			_physEngine.GearType = type;
			Axles getAxles = GetAxles;
			switch (type)
			{
			case GearboxType.GearRwd:
				ApplyGearTypeToWheel(getAxles.RearAxle);
				break;
			case GearboxType.GearFwd:
				ApplyGearTypeToWheel(getAxles.FrontAxle);
				break;
			case GearboxType.GearAwd:
			case GearboxType.Gear4Wd:
				ApplyGearTypeToWheel(getAxles.FrontAxle);
				ApplyGearTypeToWheel(getAxles.RearAxle, reset: false);
				break;
			case GearboxType.Gear4Wdr1:
				ApplyGearTypeToWheel(getAxles.RearAxle);
				ApplyGearTypeToWheel(getAxles.Rear1Axle, reset: false);
				break;
			case GearboxType.Gear4Wdr2:
				ApplyGearTypeToWheel(getAxles.Rear1Axle);
				ApplyGearTypeToWheel(getAxles.Rear2Axle, reset: false);
				break;
			case GearboxType.Gear6Wd:
				ApplyGearTypeToWheel(getAxles.FrontAxle);
				ApplyGearTypeToWheel(getAxles.RearAxle, reset: false);
				ApplyGearTypeToWheel(getAxles.Rear1Axle, reset: false);
				break;
			case GearboxType.Gear6Wdr2:
				ApplyGearTypeToWheel(getAxles.RearAxle);
				ApplyGearTypeToWheel(getAxles.Rear1Axle, reset: false);
				ApplyGearTypeToWheel(getAxles.Rear2Axle, reset: false);
				break;
			case GearboxType.Gear8Wd:
				ApplyGearTypeToWheel(getAxles.FrontAxle);
				ApplyGearTypeToWheel(getAxles.RearAxle, reset: false);
				ApplyGearTypeToWheel(getAxles.Rear1Axle, reset: false);
				ApplyGearTypeToWheel(getAxles.Rear2Axle, reset: false);
				break;
			case GearboxType.Gear0Wd:
				ApplyGearTypeToWheel(getAxles.FrontAxle, reset: true, powered: false);
				break;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
		}

		public void SetAlignCar(bool state)
		{
			SetAlignPlane(default, state);
		}

		public void ApplySuspensionTuning(AxleType axleType)
		{
			switch (axleType)
			{
			case AxleType.Front:
				ApplySuspDesc2Axle(AxleType.Front, DynoData.FrontSuspension);
				break;
			case AxleType.Rear:
				ApplySuspDesc2Axle(AxleType.Rear, DynoData.RearSuspension);
				break;
			case AxleType.Rear1:
				ApplySuspDesc2Axle(AxleType.Rear1, DynoData.RearSuspension);
				break;
			case AxleType.Rear2:
				ApplySuspDesc2Axle(AxleType.Rear2, DynoData.RearSuspension);
				break;
			default:
				throw new ArgumentOutOfRangeException("axleType", axleType, null);
			}
		}

		private void ApplySuspDesc2Axle(AxleType axleType, DynoData.Suspension suspDesc)
		{
			AdditionalData additionalData = GetVehicleData.carInfo?.AdditionalData;
			bool flag = additionalData != null;
			List<WheelProvider> list = new List<WheelProvider>();
			Axles getAxles = GetAxles;
			float3 specs = new float3(suspDesc.SpringLength, suspDesc.Stiffness, suspDesc.PneumaticBagSystem.TargetHeight);
			float sag = suspDesc.PneumaticBagSystem.Sag;
			float2 float5 = SpringLenLimits(suspDesc);
			switch (axleType)
			{
			case AxleType.Front:
				list.AddRange(getAxles.FrontAxle.Wheels);
				UpdateSpacers(AxleType.Front);
				_physEngine.SetFront(0, 1, Mathf.Min(Utils.RoundTo(suspDesc.SpringLength * 0.5f, 1), 0.2f));
				getAxles.FrontAxle.SetSpring(specs, sag, float5.x, float5.y);
				break;
			case AxleType.Rear:
				list.AddRange(getAxles.RearAxle.Wheels);
				_physEngine.SetRear(2, 3, Mathf.Min(Utils.RoundTo(suspDesc.SpringLength * 0.5f, 1), 0.2f));
				getAxles.RearAxle.SetSpring(specs, sag, float5.x, float5.y);
				if (getAxles.Rear1Axle.Wheels.Count > 0)
				{
					list.AddRange(getAxles.Rear1Axle.Wheels);
					getAxles.Rear1Axle.SetSpring(specs, sag, float5.x, float5.y);
				}
				if (getAxles.Rear2Axle.Wheels.Count > 0)
				{
					list.AddRange(getAxles.Rear2Axle.Wheels);
					getAxles.Rear2Axle.SetSpring(specs, sag, float5.x, float5.y);
				}
				UpdateSpacers(AxleType.Rear);
				break;
			}
			foreach (WheelProvider item in list)
			{
				IWheel wheel = item.Wheel;
				if (wheel == null)
				{
					continue;
				}
				float maxSpringLen;
				if (flag)
				{
					if (additionalData != null)
					{
						PsData airSuspension = additionalData.AirSuspension;
						if (airSuspension != null && airSuspension.Bought && airSuspension.Installed)
						{
							maxSpringLen = suspDesc.PneumaticBagSystem.CurrentSpringLength;
							goto IL_0207;
						}
					}
					maxSpringLen = suspDesc.SpringLength;
					goto IL_0207;
				}
				wheel.MaxSpringLen = suspDesc.SpringLength;
				goto IL_021b;
				IL_0207:
				wheel.MaxSpringLen = maxSpringLen;
				goto IL_021b;
				IL_021b:
				wheel.SpringCoef = suspDesc.Stiffness;
				float fastBumpRebound = Utils.GetFastBumpRebound(suspDesc.Bump);
				wheel.BumpRate = new float2(fastBumpRebound, Utils.Round50(suspDesc.Bump));
				float fastBumpRebound2 = Utils.GetFastBumpRebound(suspDesc.Rebound);
				wheel.ReboundRate = new float2(fastBumpRebound2, Utils.Round50(suspDesc.Rebound));
				wheel.Camber = suspDesc.Camber;
				wheel.ToeIn = suspDesc.ToeIn * ((float)Math.PI / 180f);
				wheel.AddSteerAngle = suspDesc.AddSteerAngle;
				if (suspDesc is DynoData.SteerSuspension steerSuspension)
				{
					wheel.Caster = steerSuspension.Caster;
				}
			}
		}

		public void ApplyTrailerParams(ChassisData chassisInfo, RearWheelData wheelInfo)
		{
			DynoData.Aero.Cx = chassisInfo.Cx;
			DynoData.Aero.FrontDownforce = chassisInfo.FrontDownforce;
			DynoData.Aero.RearDownforce = chassisInfo.RearDownforce;
			DynoData.Chassis.Mass = chassisInfo.Weight;
			DynoData.Chassis.Length = chassisInfo.Length;
			DynoData.Chassis.Width = chassisInfo.Width;
			DynoData.Chassis.Height = chassisInfo.Height;
			DynoData.Chassis.FrontPercent = chassisInfo.WeightDistribution;
			DynoData.Chassis.UpPercent = chassisInfo.WeightDistributionVertical;
			ApplyAeroDynamic();
			SetRigidbody();
			IsTrailer = true;
			PhysSteps = 8;
			_physEngine.GearType = GearboxType.Gear0Wd;
			MaxSteeringAngle = 0f;
			DynoData.Chassis.BrakeBalance = (int)chassisInfo.BrakeBalance;
			wheelInfo.SuspensionData.GetShockAbsorberParams(out var rate);
			wheelInfo.SuspensionData.GetSpringParams(out var specs);
			DynoData.RearSuspension.Bump = rate.x;
			DynoData.RearSuspension.Rebound = rate.y;
			DynoData.RearSuspension.SpringLength = specs.x;
			DynoData.RearSuspension.Stiffness = specs.y;
			DynoData.RearSuspension.BrakeDiscType = wheelInfo.BrakesData.GetBrakeDiscType;
			DynoData.RearSuspension.BrakeDiskTorque = wheelInfo.BrakesData.GetFullBrakeTorque;
			DynoData.RearSuspension.AntirollBar = wheelInfo.SuspensionData.AntiSwayBarForce;
			_physEngine.RearStabSpringCoef = wheelInfo.SuspensionData.AntiSwayBarForce;
			DynoData.RearSuspension.Camber = wheelInfo.SuspensionData.Camber;
			DynoData.RearSuspension.ToeIn = wheelInfo.SuspensionData.ToeIn;
			DynoData.RearSuspension.SuspensionType = wheelInfo.SuspensionData.SuspensionType;
			DynoData.RearSuspension.Spacers = wheelInfo.SpacerWidth;
			if (Axles.RearAxle.Wheels.Count == 2)
			{
				SetWheelParams(AxleType.Rear, wheelInfo);
				ApplySuspDesc2AxleTrailer(AxleType.Rear, DynoData.RearSuspension);
				SetBrakeParams(Axles.RearAxle);
			}
			if (Axles.Rear1Axle.Wheels.Count == 2)
			{
				SetWheelParams(AxleType.Rear1, wheelInfo);
				ApplySuspDesc2AxleTrailer(AxleType.Rear1, DynoData.RearSuspension);
				SetBrakeParams(Axles.Rear1Axle);
			}
			if (Axles.Rear2Axle.Wheels.Count == 2)
			{
				SetWheelParams(AxleType.Rear2, wheelInfo);
				ApplySuspDesc2AxleTrailer(AxleType.Rear2, DynoData.RearSuspension);
				SetBrakeParams(Axles.Rear2Axle);
			}
		}

		private void ApplySuspDesc2AxleTrailer(AxleType axleType, DynoData.Suspension suspDesc)
		{
			List<WheelProvider> list = new List<WheelProvider>();
			Axles getAxles = GetAxles;
			switch (axleType)
			{
			case AxleType.Rear:
				list.AddRange(getAxles.RearAxle.Wheels);
				UpdateSpacers(AxleType.Rear);
				break;
			case AxleType.Rear1:
				list.AddRange(getAxles.Rear1Axle.Wheels);
				UpdateSpacers(AxleType.Rear1);
				break;
			case AxleType.Rear2:
				list.AddRange(getAxles.Rear2Axle.Wheels);
				UpdateSpacers(AxleType.Rear1);
				break;
			}
			foreach (WheelProvider item in list)
			{
				if (item.Wheel != null)
				{
					item.Wheel.MaxSpringLen = suspDesc.SpringLength;
					item.Wheel.SpringCoef = suspDesc.Stiffness;
					float fastBumpRebound = Utils.GetFastBumpRebound(suspDesc.Bump);
					item.Wheel.BumpRate = new float2(fastBumpRebound, Utils.Round50(suspDesc.Bump));
					float fastBumpRebound2 = Utils.GetFastBumpRebound(suspDesc.Rebound);
					item.Wheel.ReboundRate = new float2(fastBumpRebound2, Utils.Round50(suspDesc.Rebound));
					item.Wheel.Camber = suspDesc.Camber;
					item.Wheel.ToeIn = suspDesc.ToeIn * ((float)Math.PI / 180f);
				}
			}
		}

		public float CalculateWheelMass(WheelData wheelData)
		{
			float num = 250f;
			float num2 = 2700f;
			float num3 = 0.02f;
			float num4 = (float)wheelData.tyreWidth * ((float)wheelData.tyreProfile / 100f);
			float num5 = ((float)wheelData.wheelSize * 25.4f / 2f + num4) / 1000f;
			float num6 = (float)wheelData.wheelSize * 0.0127f;
			float num7 = (float)wheelData.rimWidth * 0.00254f;
			float num8 = 3.14159f * (num5 * num5 - num6 * num6) * num6 * num;
			float num9 = num6 - num3;
			float num10 = 3.14159f * (num6 * num6 - num9 * num9) * num7 * num2;
			num10 *= 1.6f;
			return Mathf.Clamp(num8 + num10, 15f, 70f);
		}

		private void ResetFrictionMultiplierMode()
		{
			Axles getAxles = GetAxles;
			DynoData.FrontTyre.FrictionMultiplier = 1.2f;
			DynoData.RearTyre.FrictionMultiplier = 1.2f;
			foreach (WheelProvider wheel in getAxles.FrontAxle.Wheels)
			{
				wheel.Wheel.FrictionMultiplier = DynoData.FrontTyre.FrictionMultiplier;
			}
			foreach (WheelProvider wheel2 in getAxles.RearAxle.Wheels)
			{
				wheel2.Wheel.FrictionMultiplier = DynoData.RearTyre.FrictionMultiplier;
			}
			if (getAxles.Rear1Axle.Wheels.Count > 0)
			{
				foreach (WheelProvider wheel3 in getAxles.Rear1Axle.Wheels)
				{
					wheel3.Wheel.FrictionMultiplier = DynoData.RearTyre.FrictionMultiplier;
				}
			}
			if (getAxles.Rear2Axle.Wheels.Count > 0)
			{
				foreach (WheelProvider wheel4 in getAxles.Rear2Axle.Wheels)
				{
					wheel4.Wheel.FrictionMultiplier = DynoData.RearTyre.FrictionMultiplier;
				}
			}
			SetRigidbody();
		}

		public void SetWheelParams(AxleType axleType, WheelData wheelData)
		{
			TyreType tyreType = (TryGetComponent<VehicleData>(out var component) ? TyreClassRules.Sanitize(wheelData.tyreType, component.CarId) : wheelData.tyreType);
			switch (axleType)
			{
			case AxleType.Front:
				DynoData.FrontTyre.DiscDiam = wheelData.wheelSize;
				DynoData.FrontTyre.DiscWidth = wheelData.rimWidth;
				DynoData.FrontTyre.Width = wheelData.tyreWidth;
				DynoData.FrontTyre.Profile = wheelData.tyreProfile;
				DynoData.FrontTyre.TyreType = tyreType;
				DynoData.FrontTyre.Mass = CalculateWheelMass(wheelData);
				ApplyTyres2Axle(AxleType.Front, DynoData.FrontTyre);
				break;
			case AxleType.Rear:
				DynoData.RearTyre.DiscDiam = wheelData.wheelSize;
				DynoData.RearTyre.DiscWidth = wheelData.rimWidth;
				DynoData.RearTyre.Width = wheelData.tyreWidth;
				DynoData.RearTyre.Profile = wheelData.tyreProfile;
				DynoData.RearTyre.TyreType = tyreType;
				DynoData.RearTyre.Mass = CalculateWheelMass(wheelData);
				ApplyTyres2Axle(AxleType.Rear, DynoData.RearTyre);
				break;
			}
		}

		private void ApplyTyres2Axle(AxleType axleType, DynoData.Tyre tyre)
		{
			if (TryGetComponent<VehicleData>(out var component))
			{
				tyre.TyreType = TyreClassRules.Sanitize(tyre.TyreType, component.CarId);
			}
			float radius = Utils.CalculateWheelRadius(tyre.Width, tyre.Profile, tyre.DiscDiam);
			List<WheelProvider> list = new List<WheelProvider>();
			Axles getAxles = GetAxles;
			switch (axleType)
			{
			case AxleType.Front:
				list.AddRange(getAxles.FrontAxle.Wheels);
				_physEngine.SetFrontTyres(DynoData.FrontTyre.TyreType, DynoData.FrontTyre.Width, DynoData.FrontTyre.Profile, DynoData.FrontTyre.DiscDiam);
				break;
			case AxleType.Rear:
				list.AddRange(getAxles.RearAxle.Wheels);
				_physEngine.SetRearTyres(DynoData.RearTyre.TyreType, DynoData.RearTyre.Width, DynoData.RearTyre.Profile, DynoData.RearTyre.DiscDiam);
				if (Axles.Rear1Axle.Wheels.Count == 2)
				{
					list.AddRange(getAxles.Rear1Axle.Wheels);
					_physEngine.SetRear1Tyres(DynoData.RearTyre.TyreType, DynoData.RearTyre.Width, DynoData.RearTyre.Profile, DynoData.RearTyre.DiscDiam);
				}
				if (Axles.Rear2Axle.Wheels.Count == 2)
				{
					list.AddRange(getAxles.Rear2Axle.Wheels);
					_physEngine.SetRear2Tyres(DynoData.RearTyre.TyreType, DynoData.RearTyre.Width, DynoData.RearTyre.Profile, DynoData.RearTyre.DiscDiam);
				}
				break;
			}
			foreach (WheelProvider item in list)
			{
				item.SetRimSize(tyre.DiscDiam, tyre.DiscWidth);
				item.SetTyre(tyre.TyreType, tyre.Width, tyre.Profile, tyre.DiscDiam);
				item.Wheel.Radius = radius;
				item.Wheel.TyrePressure = tyre.Pressure;
				item.Wheel.FrictionMultiplier = tyre.FrictionMultiplier;
				item.Wheel.Mass = tyre.Mass;
			}
		}

		public void GetTyrePressure(AxleType axle, out int pressure)
		{
			pressure = ((axle == AxleType.Front) ? ((int)DynoData.FrontTyre.Pressure) : ((int)DynoData.RearTyre.Pressure));
		}

		public void ChangeTyre(AxleType axleType, TyreType type)
		{
			if (TryGetComponent<VehicleData>(out var component))
			{
				type = TyreClassRules.Sanitize(type, component.CarId);
			}
			if (axleType == AxleType.Front)
			{
				DynoData.FrontTyre.TyreType = type;
			}
			else
			{
				DynoData.RearTyre.TyreType = type;
			}
			ApplyTyres2Axle(axleType, (axleType == AxleType.Front) ? DynoData.FrontTyre : DynoData.RearTyre);
		}

		public void SetTyrePressure(AxleType axle, float pressure)
		{
			if (axle == AxleType.Front)
			{
				DynoData.FrontTyre.Pressure = Utils.Bar2Pa(pressure);
				GetWheel(WheelIndex.FrontLeft).TyrePressure = DynoData.FrontTyre.Pressure;
				GetWheel(WheelIndex.FrontRight).TyrePressure = DynoData.FrontTyre.Pressure;
				return;
			}
			DynoData.RearTyre.Pressure = Utils.Bar2Pa(pressure);
			GetWheel(WheelIndex.RearLeft).TyrePressure = DynoData.RearTyre.Pressure;
			GetWheel(WheelIndex.RearRight).TyrePressure = DynoData.RearTyre.Pressure;
			GetWheel(WheelIndex.Rear1Left).TyrePressure = DynoData.RearTyre.Pressure;
			GetWheel(WheelIndex.Rear1Right).TyrePressure = DynoData.RearTyre.Pressure;
			GetWheel(WheelIndex.Rear2Left).TyrePressure = DynoData.RearTyre.Pressure;
			GetWheel(WheelIndex.Rear2Right).TyrePressure = DynoData.RearTyre.Pressure;
		}

		public void PostSimulation()
		{
			if (PositionIntegration)
			{
				RbEntity desc = default;
				_physEngine.GetRigidBody(ref desc);
				Rigidbody.mass = desc.Mass;
				Rigidbody.centerOfMass = desc.CenterOfMass;
				Rigidbody.inertiaTensor = desc.Inertia;
				Rigidbody.velocity = desc.Velocity;
				Rigidbody.angularVelocity = desc.AngularVelocity;
				Matrix4x4 matrix = desc.Matrix;
				Rigidbody.MovePosition(new Vector3(matrix.m03, matrix.m13, matrix.m23));
				Rigidbody.MoveRotation(Quaternion.LookRotation(new Vector3(matrix.m02, matrix.m12, matrix.m22), new Vector3(matrix.m01, matrix.m11, matrix.m21)));
			}
			else
			{
				_physEngine.GetTotals(out var fdt, out var mFdt);
				Rigidbody.AddTorque(mFdt, ForceMode.Impulse);
				Rigidbody.AddForce(fdt, ForceMode.Impulse);
			}
			_cachedSpeedMpsDirty = true;
			_onPostSimulationInvoker?.Invoke(this);
		}

		private void UpdateEngineThermal()
		{
			if (PhysRuntimeFlags.EngineThermalEnabled)
			{
				float ambientC = 25f;
				if (!_engineThermalStarted)
				{
					EngineThermal.Reset(ambientC);
					_engineThermalStarted = true;
				}
				float coolingCapacity = 0f;
				RadiatorPart radiatorPart = ((!_vehicleEntityProvider) ? null : _vehicleEntityProvider.CarEntity?.Engine?.Radiator?.Part);
				if ((bool)radiatorPart)
				{
					coolingCapacity = radiatorPart.GetCoolingCapacity;
				}
				EngineThermalInputs inputs = new EngineThermalInputs(ambientC, _physEngine.EngineCurPower, CurrentRpm, DynoData.Engine.Displacement, SpeedMps, coolingCapacity, IsEngineWorks);
				EngineThermal.Tick(in inputs, Time.deltaTime);
			}
		}

		public bool TryGetPoweredWheelsTireVelocity(out float velocity)
		{
			velocity = 0f;
			int num = 0;
			foreach (WheelProvider allWheel in GetAxles.AllWheels)
			{
				if (allWheel.Wheel.IsPowered)
				{
					velocity += allWheel.Wheel.WheelTireVelocity;
					num++;
				}
			}
			if (num == 0)
			{
				return false;
			}
			velocity /= num;
			return true;
		}

		private void UpdateOdometer()
		{
			if (TryGetPoweredWheelsTireVelocity(out var velocity))
			{
				Mileage = Utils.Meter2Km(Mathf.Abs(velocity));
				UpdateMileage(Mileage * Time.deltaTime);
			}
		}

		public void PreSimulation(float dt)
		{
			if (!OnLineCar)
			{
				if (!PositionIntegration)
				{
					Vector3 position = base.transform.position;
					Quaternion rotation = base.transform.rotation;
					Vector3 position2 = Rigidbody.position;
					Quaternion rotation2 = Rigidbody.rotation;
					Vector3 velocity = Rigidbody.velocity;
					Vector3 angularVelocity = Rigidbody.angularVelocity;
					bool num = !float.IsFinite(position.x) || !float.IsFinite(position.y) || !float.IsFinite(position.z);
					bool flag = !float.IsFinite(rotation.x) || !float.IsFinite(rotation.y) || !float.IsFinite(rotation.z) || !float.IsFinite(rotation.w);
					bool flag2 = !float.IsFinite(position2.x) || !float.IsFinite(position2.y) || !float.IsFinite(position2.z);
					bool flag3 = !float.IsFinite(rotation2.x) || !float.IsFinite(rotation2.y) || !float.IsFinite(rotation2.z) || !float.IsFinite(rotation2.w);
					bool flag4 = !float.IsFinite(velocity.x) || !float.IsFinite(velocity.y) || !float.IsFinite(velocity.z);
					bool flag5 = !float.IsFinite(angularVelocity.x) || !float.IsFinite(angularVelocity.y) || !float.IsFinite(angularVelocity.z);
					bool flag6 = num | flag2;
					bool flag7 = flag | flag3;
					Vector3 vector = position;
					Quaternion quaternion2 = rotation;
					Matrix4x4 matrix;
					if (flag6 | flag7 | flag4 | flag5)
					{
						Debug.LogError("[Powertrain recovery] " + base.name + " rigidbody Inf/NaN — snapping back. " + $"pos={vector} rot={quaternion2} vel={velocity} angVel={angularVelocity}");
						Vector3 vector2 = (flag6 ? _lastGoodPos : vector);
						Quaternion quaternion3 = (flag7 ? _lastGoodRot : quaternion2);
						base.transform.SetPositionAndRotation(vector2, quaternion3);
						Rigidbody.position = vector2;
						Rigidbody.rotation = quaternion3;
						Rigidbody.velocity = Vector3.zero;
						Rigidbody.angularVelocity = Vector3.zero;
						matrix = Matrix4x4.TRS(vector2, quaternion3, base.transform.lossyScale);
					}
					else
					{
						_lastGoodPos = vector;
						_lastGoodRot = quaternion2;
						matrix = base.transform.localToWorldMatrix;
					}
					RbEntity desc = new RbEntity
					{
						Mass = Rigidbody.mass,
						CenterOfMass = Rigidbody.centerOfMass,
						Inertia = Rigidbody.inertiaTensor,
						Velocity = Rigidbody.velocity,
						AngularVelocity = Rigidbody.angularVelocity,
						Matrix = matrix
					};
					_physEngine.SetRigidBody(ref desc);
				}
				for (int i = 0; i < 8; i++)
				{
					IWheel wheel = GetWheel((WheelIndex)i);
					if (!wheel.Enabled)
					{
						continue;
					}
					int rayCount = wheel.GetRayCount();
					for (int j = 0; j < rayCount; j++)
					{
						if (wheel.GetRayTracePoint(j, out var p, out var p2))
						{
							wheel.SetRayTraceResult(j, _wheelRayTracer.GetRay(p, p2, out var rayInfo), rayInfo);
						}
					}
					wheel.SelectContactRay();
				}
				_physEngine.UpdateHillHold();
			}
			_onPreSimulationInvoker?.Invoke(this);
		}

		private void StarterSimulation(float dt)
		{
			_physEngine.Starter += dt;
			_physEngine.Ignition = true;
			if (!Fuel || IsEngineBroken)
			{
				ShutdownEngine();
				if ((bool)V.Car && V.Car == base.gameObject && !FlagsStorage.OutOfCar && !FlagsStorage.IsPassenger)
				{
					string message = ((!Fuel) ? new LocalizedString("ThrottleSliderControll.NoFuel") : new LocalizedString("Powertrain.EngineIsBroken"));
					V.CoreGameManager.SetMessage(message, ScenesHelper.IsParkingLevelActive() ? 2 : 3);
				}
			}
			else
			{
				_starterWorks += Time.deltaTime;
				if (_starterWorks > 5f)
				{
					ShutdownEngine();
				}
			}
		}

		private void StopStarter()
		{
			_starter = false;
			Starter = 0f;
			if ((bool)_engineSound)
			{
				_engineSound.StopEngineStartSound();
			}
		}

		private Transform GetWheelTransform(WheelIndex wheelIndex)
		{
			WheelProvider wheelProvider = wheelIndex switch
			{
				WheelIndex.FrontLeft => Axles.FrontAxle.LeftWheel, 
				WheelIndex.FrontRight => Axles.FrontAxle.RightWheel, 
				WheelIndex.RearLeft => Axles.RearAxle.LeftWheel, 
				WheelIndex.RearRight => Axles.RearAxle.RightWheel, 
				WheelIndex.Rear1Left => Axles.Rear1Axle.LeftWheel, 
				WheelIndex.Rear1Right => Axles.Rear1Axle.RightWheel, 
				WheelIndex.Rear2Left => Axles.Rear2Axle.LeftWheel, 
				WheelIndex.Rear2Right => Axles.Rear2Axle.RightWheel, 
				_ => null, 
			};
			if ((bool)wheelProvider && (bool)wheelProvider.ModelTransform)
			{
				return wheelProvider.ModelTransform;
			}
			return null;
		}

		public void InitWheel(WheelIndex wheelIndex, WheelProvider wheelProvider)
		{
			IWheel wheel = GetWheel(wheelIndex);
			wheel.Enabled = false;
			if ((bool)wheelProvider)
			{
				wheel.Enabled = true;
				wheelProvider.TopPos = wheelProvider.transform.localPosition;
				wheel.TopPos = wheelProvider.TopPos;
				wheelProvider.Init(this, wheelIndex, GetVehicleData);
			}
		}

		private void UpdateWheelTransform(WheelIndex wheelIndex, WheelProvider wheelProvider)
		{
			if (!wheelProvider)
			{
				return;
			}
			IWheel wheel = GetWheel(wheelIndex);
			if (!wheel.Enabled || !wheelProvider.ModelTransform)
			{
				return;
			}
			wheel.GetPositionAndRotation(out var pos, out var rot);
			if (!float.IsFinite(pos.x) || !float.IsFinite(pos.y) || !float.IsFinite(pos.z) || !float.IsFinite(rot.x) || !float.IsFinite(rot.y) || !float.IsFinite(rot.z) || !float.IsFinite(rot.w))
			{
				return;
			}
			if ((bool)wheelProvider.ModelTransform)
			{
				wheelProvider.ModelTransform.SetPositionAndRotation(pos, rot);
			}
			if (wheelProvider.OpenWheels && (bool)wheelProvider.RimColliderTransform)
			{
				wheel.GetRimColliderPositionAndRotation(out var pos2, out var rot2);
				if (float.IsFinite(pos2.x) && float.IsFinite(pos2.y) && float.IsFinite(pos2.z) && float.IsFinite(rot2.x) && float.IsFinite(rot2.y) && float.IsFinite(rot2.z) && float.IsFinite(rot2.w))
				{
					wheelProvider.RimColliderTransform.SetPositionAndRotation(pos2, rot2);
				}
			}
		}

		public void RestoreRigidbodyPhysics()
		{
			SetRigidbody();
		}

		private void SetRigidbody()
		{
			Rigidbody.useGravity = false;
			Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			int num = 0;
			if ((bool)GetVehicleData)
			{
				num = GetVehicleData.VehicleMembersController.Members.Count((GameObject passenger) => passenger) * 70;
			}
			float mass = DynoData.Chassis.Mass + (float)num;
			Rigidbody.mass = mass;
			_dimensions = new Vector3(DynoData.Chassis.Width, DynoData.Chassis.Height, DynoData.Chassis.Length);
			centerOfMass.localPosition = _physEngine.CalcCenterOfMass(DynoData.Chassis.FrontPercent, calcFrontCmFromWheels: true, DynoData.Chassis.UpPercent, _weightBounds);
			Rigidbody.centerOfMass = centerOfMass.localPosition;
			Rigidbody.inertiaTensor = _physEngine.CalcInertia(mass, _dimensions.x, PhysEngine.ResolveInertiaHeight(_weightBounds.size.y, _dimensions.y), _dimensions.z);
			RbEntity desc = new RbEntity
			{
				Mass = Rigidbody.mass,
				CenterOfMass = Rigidbody.centerOfMass,
				Inertia = Rigidbody.inertiaTensor,
				Velocity = Rigidbody.velocity,
				AngularVelocity = Rigidbody.angularVelocity,
				Matrix = base.transform.localToWorldMatrix
			};
			_physEngine.SetRigidBody(ref desc);
			_onMassChangedInvoker?.Invoke(this);
		}
	}
}
