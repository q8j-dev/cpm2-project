using System;
using System.Collections.Concurrent;
using CPM.Transmission;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;

namespace CPMEngine.Protected
{
	public class PhysEngine : ITransmissionHost
	{
		private enum PhysEventId : byte
		{
			EngineShutdown = 0,
			EngineStarted = 1,
			GearChanged = 2,
			BlowOff = 3,
			TrcStateChanged = 4,
			EspStateChanged = 5,
			AbsStateChanged = 6
		}

		private readonly struct PhysEvent
		{
			public readonly PhysEventId Id;

			public readonly bool BoolArg;

			public PhysEvent(PhysEventId id, bool boolArg)
			{
				Id = id;
				BoolArg = boolArg;
			}
		}

		public const int NumWheels = 8;

		private GearboxProfile _transmissionProfile = GearboxProfile.For(GearboxKind.AT);

		private ITransmissionBehavior _behavior = new AtBehavior();

		private float _idleGovernorScale = 1f;

		private float _throttleOverride = 1f;

		private readonly ConcurrentQueue<PhysEvent> _pendingEvents = new ConcurrentQueue<PhysEvent>();

		private static readonly int MaxGears = 14;

		private static readonly float EspStrength = 2f;

		private readonly bool[] Touch = new bool[8];

		private bool _cutOut = true;

		private bool _ignition = true;

		private bool _useAlignPlane;

		private bool _isThermostatOpen;

		private bool _isFanWorking;

		private bool _hasLaunchControl;

		private bool _lcCooldown;

		private float _ratio;

		private float _fuelConsumptionAtCostantSpeed = 4.3f;

		private float _fuelConsumptionSpeed = 100f;

		private float _secondsToCover100Km;

		private float _starterTime;

		private float _maxStarterTorque = 50f;

		private float _maxStarterRpm = 1400f;

		private float _minStarterRpm = 500f;

		private const float StallGuardMarginRpm = 100f;

		private int _idFrontLeft = -1;

		private int _idFrontRight = -1;

		private int _idRearLeft = -1;

		private int _idRearRight = -1;

		private int _idRear1Left = -1;

		private int _idRear1Right = -1;

		private int _idRear2Left = -1;

		private int _idRear2Right = -1;

		private readonly int[,] _layout6Wd = new int[3, 2];

		private readonly int[,] _layout6Wdr2 = new int[3, 2];

		private readonly int[,] _layout8Wd = new int[4, 2];

		private float _diffDragImpulse;

		private float _clutchDragImpulse;

		private float _clutchB = 1f;

		private int _dormantGear = 1;

		private float _ratioB;

		private float _ratioOverride = float.NaN;

		private bool _sportMode;

		private bool _lowMode;

		private int _lowMaxGear = 1;

		private float _tcMult = 1f;

		public bool TccLocked;

		private const float TccLockEnterRatio = 0.8f;

		private const float TccLockExitRatio = 0.7f;

		private const float TccLockMinVeloEnter = 8f;

		private const float TccLockMinVeloExit = 5f;

		private bool _tcSlipLatched = true;

		public float tcStallRpmFraction = 0.6f;

		public float tcStallTorqueRatio = 1.2f;

		public float tcStallIdleMultiplier = 2.5f;

		public float tcTallGearStallFloor = 0.5f;

		private float _gearBoxInertia = 0.05f;

		private float _diffInertia = 0.05f;

		private float _wDiff;

		private float _wDiffF;

		private float _wDiffR;

		private float _brakeVal;

		private float _handbrakeVal;

		private float _accel;

		private float _accelWant;

		private float _accelIdle;

		private float _revMatchBlip;

		private float _frontBrake;

		private float _rearBrake;

		private const float HillHoldFraction = 0.6f;

		private bool _hillHoldActive;

		private int _hillHoldGear;

		private float _wEngine;

		private float _engineInertia = 0.15f;

		private float _displacementCc;

		private bool _isRotaryEngine;

		private TorqueCurveSample[] _torqueSamplesRaw = Array.Empty<TorqueCurveSample>();

		private float _effectiveDisplacementCc = 800f;

		private float _engineFuelTimer;

		private float _engineFuelFlow = 1f;

		private float _trcFueltimer;

		private int _toqsanTogyz;

		private const float BurnoutDrivenBrakeMult = 0.05f;

		private const float BurnoutSpinRate = 200f;

		private float3 _davlenieUlitki = float3.zero;

		private float2 _davlenieUlitki2 = float2.zero;

		private float _curBlowOff;

		private float _blowOffStrength;

		private bool _wasOnThrottle = true;

		private bool _wasShiftDump;

		private const float ShiftFlutterMult = 0.4f;

		private float[] _gearRatio = new float[14]
		{
			-3f, 0f, 4f, 3f, 2f, 1f, 0.7f, 0.5f, 0.5f, 0.5f,
			0.5f, 0.5f, 0.5f, 0.5f
		};

		private int _numGears = 6;

		private int _curGear = 2;

		private int _targetGear = 2;

		private float _gearTimer = 0.25f;

		private float _gearTimerClutch = 0.25f;

		private float2 _rpmStandingStart = new float2(2000f, 2700f);

		private readonly float _espMinVelocity = 15f;

		private readonly float[] _wheelSteerAng = new float[8];

		private float _steerAng;

		private float _steerAngWant;

		private float _wOld;

		private readonly int[] NumSum = new int[8];

		private readonly float[] _clutchCurveX = new float[32];

		private readonly float[] _clutchCurveY = new float[32];

		private TorqueCurveSample[] _torqueSamples = Array.Empty<TorqueCurveSample>();

		private int _clutchCurveNumSamples = 1;

		private float _waitShift;

		private const float CoastDownshiftCooldown = 0.3f;

		private float _prevAccelWant;

		private float _throttleDropTimer;

		private float _throttleRecentMax;

		private float _throttleRecentMaxAge;

		private float _engineRevLimitedSustained;

		private bool _lastShiftWasUp;

		private Plane _rightPlane = new Plane(Vector3.right, Vector3.zero);

		private float _vcLen;

		private Vector3 _airSumdt;

		private Vector3 _airSumTdt;

		private Vector3 _mgdt;

		private Vector3 _engineOrientation;

		private readonly float _fanTempOn = 95f;

		private readonly float _fanTempOff = 90f;

		private readonly float _thermostatOpenTemp = 88f;

		private readonly float _thermostatCloseTemp = 85f;

		private readonly float _radiatorCooling = 0.39f;

		private readonly float _engineSpeedHeat = 0.0005f;

		private float _engineGearUpRpm = 6000f;

		private float _engineGearDnRpm = 3500f;

		private float3 _lcController;

		private readonly RB _body;

		private TrailerJoint _trailerJoint;

		private readonly Wheel[] _wheels;

		private const float ParkRevLimitFrac = 0.6f;

		private const float LimiterBounceTargetHz = 4.5f;

		private float _limiterBouncePhase;

		private const float LimiterDisarmBandRpm = 600f;

		private const float ThrottleLiftDisarmThreshold = 0.95f;

		private bool _limiterArmed;

		private float _limiterPhase;

		public const float LimiterCutDuty = 0.2f;

		private bool _autoClutch = true;

		private bool _isAutomatic = true;

		private const float TrcIndicatorHoldSec = 0.3f;

		private bool _trcTrigger;

		private float _trcIndicatorHold;

		private bool _espTriggered;

		private bool _absTriggered;

		private bool _fixVehicle;

		private float _maxBrakeAccel;

		private float _maxDragAccel;

		private float _maxSideAccel;

		private float _maxSteerAngle;

		private float _chassisAngle;

		private float _baseWheel;

		private float _halfTrack;

		private readonly Vector3[] _baseBoundsScratch = new Vector3[8];

		private float _throttleIntent;

		private float _gearShiftingDelay = 0.25f;

		private float _engineMaxTorque = 370f;

		private float _engineMaxTorqueRpm = 6000f;

		private float _engineCurTorqueRpm;

		private const float EspDeadZoneLate = 0.21f;

		private const float EspDeadZoneEarly = 0.07f;

		private const float EspFullAuthority = 0.44f;

		private const float EspMaxSideslipForUndersteer = 15f;

		private float _espPrevSideslip;

		private const float EspUndersteerGripFraction = 0.8f;

		private const float EspOversteerBrakeMin = 0.15f;

		private const float EspOversteerBrakeMax = 0.85f;

		private const float EspUndersteerBrakeMin = 0.04f;

		private const float EspUndersteerBrakeMax = 0.12f;

		private float _espFuelCut;

		private float _rbTopSpeedMps = 80f;

		private float _rbMaxAccel = 12f;

		private float _rbMaxBrake = 14f;

		private float _rbMaxLateral = 15f;

		private float _rbSteerGain = 60f;

		private float _rbMaxSteerTq = 30f;

		private float _rbVerticalDamp = 12f;

		private float _rbAngularDamp = 8f;

		private bool _rbAutoVerticalDamp;

		private const float RaceBotVertDampCoef = 2.2f;

		private float _rbCollisionGrip;

		private const float RaceBotCollisionGripRecoverRate = 1.25f;

		private const float RaceBotCollisionExtraDownforceAccel = 30f;

		private const float RaceBotCollisionLaunchVel = 0.25f;

		private const float RaceBotCollisionAirborneRise = 0.25f;

		private const float RaceBotMaxLaunchVel = 1f;

		private const float RaceBotMaxAirborneRise = 1f;

		private const float RaceBotDownforceCoef = 0.001925f;

		private const float RaceBotMaxDownforceAccel = 9.24f;

		private const float RaceBotWheelReboundDamp = 4f;

		private const float RaceBotWheelReboundMaxVel = 1.5f;

		private const float RaceBotUprightGain = 60f;

		private const float RaceBotMaxYawRate = 2.5f;

		private float _rbLaunchTraction = 0.256f;

		private float _rbLaunchBandFrac = 0.22f;

		float ITransmissionHost.CurrentRpm => CurrentRpm;

		float ITransmissionHost.WEngine => _wEngine;

		float ITransmissionHost.WDiff => _wDiff;

		float ITransmissionHost.Ratio => _ratio;

		float ITransmissionHost.IdleRpm => RpmController.x;

		float ITransmissionHost.Redline => RpmController.y;

		float ITransmissionHost.Velocity => _vcLen;

		float ITransmissionHost.EngineTorque => EngineCurTorque;

		float ITransmissionHost.LongAccel => LongAccel;

		float ITransmissionHost.RevMatchBlip
		{
			get
			{
				return _revMatchBlip;
			}
			set
			{
				_revMatchBlip = value;
			}
		}

		float ITransmissionHost.ClutchCapacity => ClutchLimit;

		bool ITransmissionHost.AutoClutch => AutoClutch;

		GearboxType ITransmissionHost.GearType => GearType;

		bool ITransmissionHost.IsActiveLaunchControlNow => IsActiveLaunchControlNow;

		bool ITransmissionHost.DriveDecoupled => _fixVehicle;

		bool ITransmissionHost.OverheatTransmission => OverheatTransmission;

		float ITransmissionHost.TcLockupRpm => TcLockupRpm;

		float ITransmissionHost.TargetGearTotalRatio => _gearRatio[_targetGear] * FinalDrive;

		float ITransmissionHost.Starter => Starter;

		float ITransmissionHost.Clutch
		{
			get
			{
				return Clutch;
			}
			set
			{
				Clutch = value;
			}
		}

		float ITransmissionHost.ClutchPedalInput => ClutchPedalInput;

		float ITransmissionHost.TcMult
		{
			get
			{
				return _tcMult;
			}
			set
			{
				_tcMult = value;
			}
		}

		bool ITransmissionHost.TccLocked
		{
			get
			{
				return TccLocked;
			}
			set
			{
				TccLocked = value;
			}
		}

		bool ITransmissionHost.TcSlipLatched
		{
			get
			{
				return _tcSlipLatched;
			}
			set
			{
				_tcSlipLatched = value;
			}
		}

		float ITransmissionHost.ClutchB
		{
			get
			{
				return _clutchB;
			}
			set
			{
				_clutchB = value;
			}
		}

		int ITransmissionHost.DormantGear
		{
			get
			{
				return _dormantGear;
			}
			set
			{
				_dormantGear = Mathf.Clamp(value, 0, NumGears + 1);
				_ratioB = _gearRatio[_dormantGear] * FinalDrive;
			}
		}

		float ITransmissionHost.RatioB => _ratioB;

		float ITransmissionHost.RatioOverride
		{
			get
			{
				return _ratioOverride;
			}
			set
			{
				_ratioOverride = value;
			}
		}

		bool ITransmissionHost.SportMode => _sportMode;

		public bool SportMode
		{
			get
			{
				return _sportMode;
			}
			set
			{
				_sportMode = value;
			}
		}

		bool ITransmissionHost.LowMode => _lowMode;

		public bool LowMode
		{
			get
			{
				return _lowMode;
			}
			set
			{
				_lowMode = value;
			}
		}

		int ITransmissionHost.LowMaxGear => _lowMaxGear;

		public int LowMaxGear
		{
			get
			{
				return _lowMaxGear;
			}
			set
			{
				_lowMaxGear = Mathf.Max(1, value);
			}
		}

		int ITransmissionHost.CurrentGear
		{
			get
			{
				return CurrentGear;
			}
			set
			{
				CurrentGear = value;
			}
		}

		int ITransmissionHost.TargetGear
		{
			get
			{
				return _targetGear;
			}
			set
			{
				_targetGear = value;
			}
		}

		int ITransmissionHost.NumGears => NumGears;

		float ITransmissionHost.GearTimer => _gearTimer;

		float ITransmissionHost.GearShiftingDelay => GearShiftingDelay;

		float ITransmissionHost.AccelWant
		{
			get
			{
				return _accelWant;
			}
			set
			{
				_accelWant = value;
			}
		}

		float ITransmissionHost.ThrottleIntent => _throttleIntent;

		float ITransmissionHost.Throttle => _accelWant;

		float ITransmissionHost.Brake => _brakeVal;

		float ITransmissionHost.Handbrake => _handbrakeVal;

		GearboxState ITransmissionHost.GearboxState => GearboxState;

		bool ITransmissionHost.DynorunMode => PhysRuntimeFlags.DynorunMode;

		GearboxProfile ITransmissionHost.Profile => _transmissionProfile;

		float ITransmissionHost.FinalDrive => FinalDrive;

		float ITransmissionHost.ThrottleDropTimer => _throttleDropTimer;

		float ITransmissionHost.ThrottleRecentMax => _throttleRecentMax;

		float ITransmissionHost.EngineRevLimitedSustained => _engineRevLimitedSustained;

		float ITransmissionHost.DrivenWheelRadius => ((GearType == GearboxType.GearRwd) ? _wheels[_idRearLeft].Radius : ((GearType != GearboxType.GearFwd) ? _wheels[_idRearLeft].Radius : _wheels[_idFrontLeft].Radius)) * 0.95f;

		bool ITransmissionHost.EngineWorks => IsEngineWorks;

		float ITransmissionHost.StallFloorRpm => StallFloorRpm;

		float ITransmissionHost.ClutchEngagement => GetClutchMap(Clutch) * (1f - Mathf.Clamp01(ClutchPedalOverride));

		float ITransmissionHost.IdleGovernorScale
		{
			get
			{
				return _idleGovernorScale;
			}
			set
			{
				_idleGovernorScale = value;
			}
		}

		float ITransmissionHost.ThrottleOverride
		{
			get
			{
				return _throttleOverride;
			}
			set
			{
				_throttleOverride = value;
			}
		}

		private bool OverheatTransmission => _lcController.z >= _lcController.y;

		public bool IsActiveLaunchControlNow { get; private set; }

		public bool ActivateLaunchControl
		{
			set
			{
				IsActiveLaunchControlNow = (CurrentGear == 2) & value;
			}
		}

		private float StallFloorRpm => IdleGovernor.StallGuardFloorRpm(_minStarterRpm, RpmController.x, _transmissionProfile.StallFraction, 100f);

		private float TcLockupRpm => Mathf.Max(RpmController.x + 600f, EngineMaxTorqueRpm * 0.6f);

		private float TcStallOmega => PhysMath.TorqueConverterStallRpm(EngineMaxTorqueRpm, RpmController.y, RpmController.x, tcStallRpmFraction, tcStallIdleMultiplier) * 0.104719f;

		public bool HillHoldActive => _hillHoldActive;

		public float MaxBrakeAccel
		{
			get
			{
				if (_maxBrakeAccel == 0f)
				{
					OnSetEngineMaxTorq();
				}
				return _maxBrakeAccel / 9.80665f;
			}
		}

		public float MaxDragAccel
		{
			get
			{
				if (_maxDragAccel == 0f)
				{
					OnSetEngineMaxTorq();
				}
				return _maxDragAccel / 9.80665f;
			}
		}

		public float MaxSideAccel
		{
			get
			{
				if (_maxSideAccel == 0f)
				{
					OnSetEngineMaxTorq();
				}
				return _maxSideAccel / 9.80665f;
			}
		}

		public float WaterTemp { get; private set; }

		public bool OnLineCar { get; set; }

		public bool ParkingGear { get; set; }

		public float LimiterFlutterHz { get; set; } = 6f;

		public float LimiterBounceDepthRpm { get; private set; } = 250f;

		public bool At { get; set; }

		public bool RevLimiterTriggered { get; private set; }

		public bool AutoClutch
		{
			get
			{
				if (!_autoClutch)
				{
					return PhysRuntimeFlags.IsCasualControl;
				}
				return true;
			}
			set
			{
				_autoClutch = value;
			}
		}

		public bool Automatic
		{
			get
			{
				if (!_isAutomatic)
				{
					return PhysRuntimeFlags.IsCasualControl;
				}
				return true;
			}
			set
			{
				_isAutomatic = value;
			}
		}

		public bool AnalogThrottle { get; set; }

		public bool AiMode { get; set; }

		public bool ManualHelper { get; set; }

		public bool Trailer { get; set; }

		public bool PositionIntegration { get; set; }

		public float CurrentFuel { get; set; } = 1f;

		public float EngineBrakeTorqueRatio { get; set; } = 0.07f;

		public float IstantConsumption { get; private set; }

		public float FuelConsumptionAtCostantSpeed
		{
			get
			{
				return _fuelConsumptionAtCostantSpeed;
			}
			set
			{
				_fuelConsumptionAtCostantSpeed = ((value < 0f) ? 0f : value);
			}
		}

		private float RpmAtSpeedInLastGear { get; set; }

		public float FuelConsumptionSpeed
		{
			get
			{
				return _fuelConsumptionSpeed;
			}
			set
			{
				_fuelConsumptionSpeed = ((value < 1f) ? 1f : value);
			}
		}

		public float CurrentConsumption { get; private set; }

		public float EngineInertiaRatio { get; set; } = 1f;

		public float DisplacementCc
		{
			get
			{
				return _displacementCc;
			}
			set
			{
				_displacementCc = Mathf.Max(value, 0f);
			}
		}

		public bool IsRotaryEngine
		{
			get
			{
				return _isRotaryEngine;
			}
			set
			{
				_isRotaryEngine = value;
			}
		}

		public float GearBoxInertiaRatio { get; set; } = 1f;

		public float DiffInertiaRatio { get; set; } = 1f;

		public int PhysSteps { get; set; } = 10;

		public bool TrafficMode { get; set; }

		public bool ArcadeMode { get; set; }

		public bool TowingMode { get; set; }

		public bool SimpleMode { get; set; }

		public bool RaceBotMode { get; set; }

		public float Starter { get; set; }

		public bool IsEngineWorks => !_cutOut;

		public bool Ignition
		{
			get
			{
				return _ignition;
			}
			set
			{
				_ignition = value;
				if (!_ignition)
				{
					_cutOut = true;
				}
			}
		}

		public float MaxSteerAngle
		{
			get
			{
				return _maxSteerAngle;
			}
			set
			{
				_maxSteerAngle = value;
			}
		}

		public float SteerAngle
		{
			get
			{
				return _steerAng;
			}
			set
			{
				_steerAng = value;
				_steerAngWant = value;
				UpdateSteeringGeometry();
			}
		}

		public bool TrcTrigger
		{
			set
			{
				if (_trcTrigger != value)
				{
					_trcTrigger = value;
					RaiseEvent(PhysEventId.TrcStateChanged, value);
				}
			}
		}

		public float ChassisAngle
		{
			get
			{
				return _chassisAngle;
			}
			set
			{
				_chassisAngle = value;
			}
		}

		public bool ESPtrigger
		{
			set
			{
				if (_espTriggered != value)
				{
					_espTriggered = value;
					RaiseEvent(PhysEventId.EspStateChanged, value);
				}
			}
		}

		public int EspLevel { get; set; }

		public bool Esp
		{
			get
			{
				return EspLevel > 0;
			}
			set
			{
				EspLevel = (value ? 5 : 0);
			}
		}

		public bool AbsTrigger
		{
			get
			{
				return _absTriggered;
			}
			set
			{
				if (_absTriggered != value)
				{
					_absTriggered = value;
					RaiseEvent(PhysEventId.AbsStateChanged, value);
				}
			}
		}

		public float Throttle
		{
			get
			{
				return _accelWant;
			}
			set
			{
				_accelWant = value;
			}
		}

		public float ThrottleIntent
		{
			set
			{
				_throttleIntent = value;
			}
		}

		public float Accel => _accel;

		public float Clutch { get; set; } = 0.5f;

		public float ClutchPedalInput { get; set; }

		public float ClutchPedalOverride { get; set; }

		private int CurrentGear
		{
			get
			{
				return _curGear;
			}
			set
			{
				if (_curGear != value)
				{
					_curGear = value;
					RaiseEvent(PhysEventId.GearChanged);
				}
			}
		}

		public int Gear
		{
			get
			{
				return _targetGear - 1;
			}
			set
			{
				int num = value + 1;
				if (_targetGear != num && num >= 0 && num <= _numGears + 1)
				{
					_targetGear = num;
					_gearTimer = GearShiftingDelay;
					_gearTimerClutch = GearShiftingDelay;
				}
			}
		}

		public bool IsShifting => _gearTimer != 0f;

		public float FrontBrakeTorque => BrakeFrontTorque * BrakeFrontBalance;

		public float RearBrakeTorque => BrakeRearTorque * (1f - BrakeFrontBalance);

		public float Brake
		{
			get
			{
				return _brakeVal;
			}
			set
			{
				_brakeVal = value;
				_frontBrake = _brakeVal * BrakeFrontBalance;
				_rearBrake = _brakeVal * (1f - BrakeFrontBalance);
				float inputBrakeTorque = _frontBrake * BrakeFrontTorque;
				float inputBrakeTorque2 = _rearBrake * BrakeRearTorque;
				_wheels[_idFrontLeft].InputBrakeTorque = inputBrakeTorque;
				_wheels[_idFrontRight].InputBrakeTorque = inputBrakeTorque;
				_wheels[_idRearLeft].InputBrakeTorque = inputBrakeTorque2;
				_wheels[_idRearRight].InputBrakeTorque = inputBrakeTorque2;
				if (_idRear1Left != -1)
				{
					_wheels[_idRear1Left].InputBrakeTorque = inputBrakeTorque2;
				}
				if (_idRear1Right != -1)
				{
					_wheels[_idRear1Right].InputBrakeTorque = inputBrakeTorque2;
				}
				if (_idRear2Left != -1)
				{
					_wheels[_idRear2Left].InputBrakeTorque = inputBrakeTorque2;
				}
				if (_idRear2Right != -1)
				{
					_wheels[_idRear2Right].InputBrakeTorque = inputBrakeTorque2;
				}
				if (_trailerJoint.Trailer != null)
				{
					_trailerJoint.Trailer.Brake = value;
				}
			}
		}

		public float HandBrake
		{
			get
			{
				return _handbrakeVal;
			}
			set
			{
				_handbrakeVal = value;
				float handBrakeTorque = BrakeRearTorque * _handbrakeVal * 2f;
				_wheels[_idRearLeft].HandBrakeTorque = handBrakeTorque;
				_wheels[_idRearRight].HandBrakeTorque = handBrakeTorque;
				if ((At || AiMode) && GearType != GearboxType.GearFwd)
				{
					Clutch = Mathf.Max(value, Clutch);
				}
				if (_idRear1Left != -1)
				{
					_wheels[_idRear1Left].HandBrakeTorque = handBrakeTorque;
				}
				if (_idRear1Right != -1)
				{
					_wheels[_idRear1Right].HandBrakeTorque = handBrakeTorque;
				}
				if (_idRear2Left != -1)
				{
					_wheels[_idRear2Left].HandBrakeTorque = handBrakeTorque;
				}
				if (_idRear2Right != -1)
				{
					_wheels[_idRear2Right].HandBrakeTorque = handBrakeTorque;
				}
				if (_trailerJoint.Trailer != null)
				{
					_trailerJoint.Trailer.HandBrake = value;
				}
			}
		}

		public float CurrentRpm
		{
			get
			{
				return _wEngine * (30f / (float)Math.PI);
			}
			set
			{
				_wEngine = value / (30f / (float)Math.PI);
			}
		}

		public float FinalDrive { get; set; } = 4.44f;

		public float TorqueSplit { get; set; } = 0.5f;

		public bool CenterDiffOpen { get; set; }

		public int NumGears
		{
			get
			{
				return _numGears;
			}
			set
			{
				_numGears = Mathf.Clamp(value, 1, 14);
			}
		}

		public float Load { get; private set; }

		public float SideAccel => Vector3.Dot(PhysMath.GetRight(_body.Matrix), _body.Acceleration);

		public float LongAccel => Vector3.Dot(PhysMath.GetForward(_body.Matrix), _body.Acceleration);

		public float VertAccel => Vector3.Dot(PhysMath.GetUp(_body.Matrix), _body.Acceleration);

		public float AngAccel { get; private set; }

		public float SteerAlignMom => (_wheels[_idFrontLeft].Enabled ? _wheels[_idFrontLeft].AlignMom : 0f) + (_wheels[_idFrontRight].Enabled ? _wheels[_idFrontRight].AlignMom : 0f);

		public float FrontStabSpringCoef { get; set; } = 20000f;

		public float RearStabSpringCoef { get; set; } = 20000f;

		public GearboxType GearType { get; set; } = GearboxType.GearRwd;

		public GearboxState GearboxState { get; set; }

		public float GearShiftingDelay
		{
			get
			{
				return _gearShiftingDelay;
			}
			set
			{
				_gearShiftingDelay = Mathf.Max(value, 0.05f);
			}
		}

		public float TransmissionLossCoef { get; set; }

		public SuspensionType FrontSuspensionType { get; set; }

		public SuspensionType RearSuspensionType { get; set; }

		public SuspensionType Rear1SuspensionType { get; set; }

		public SuspensionType Rear2SuspensionType { get; set; }

		public float BrakeFrontTorque { get; set; }

		public float BrakeRearTorque { get; set; }

		public int AbsLevel { get; set; }

		public bool Abs
		{
			get
			{
				return AbsLevel > 0;
			}
			set
			{
				AbsLevel = (value ? 5 : 0);
			}
		}

		public int TrcLevel { get; set; }

		public bool TractionControl
		{
			get
			{
				return TrcLevel > 0;
			}
			set
			{
				TrcLevel = (value ? 5 : 0);
			}
		}

		public float EngineMaxTorque
		{
			get
			{
				return _engineMaxTorque;
			}
			private set
			{
				_engineMaxTorque = value;
			}
		}

		public float EngineMaxTorqueRpm
		{
			get
			{
				return _engineMaxTorqueRpm;
			}
			private set
			{
				_engineMaxTorqueRpm = Mathf.Max(value, 1f);
			}
		}

		public float3 RpmController { get; private set; } = new float3(800f, 8000f, 20000f);

		public float EngineCurTorque { get; private set; }

		public float EngineCurPower => EngineCurTorque * _wEngine * 0.0013596f;

		public float EngineCurWhp
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < 8; i++)
				{
					if (_wheels[i].Enabled && _wheels[i].IsPowered)
					{
						num += _wheels[i].LongtitudeForce * _wheels[i].RadiusLoaded * _wheels[i].AngularVelocity;
					}
				}
				return num * 0.0013596f;
			}
		}

		public float BrakeFrontBalance { get; set; } = 0.6f;

		public LSDValues FrontLsd { get; set; } = new LSDValues();

		public LSDValues RearLsd { get; set; } = new LSDValues();

		public float Ackerman { get; set; }

		public float AeroCx { get; set; } = 0.35f;

		public float AeroSx { get; set; } = 2.12f;

		public float AeroFrontDownforce { get; set; } = 18f;

		public float AeroRearDownforce { get; set; } = 45f;

		public RB RigidBody => _body;

		public float MaxStarterTorque
		{
			set
			{
				_maxStarterTorque = value;
			}
		}

		private float PowerCalibration
		{
			get
			{
				GearboxType gearType = GearType;
				if (gearType == GearboxType.GearFwd || gearType == GearboxType.GearRwd || _toqsanTogyz == 0)
				{
					return 0.85f;
				}
				return 0.65f;
			}
		}

		public float ClutchLimit { get; set; }

		public float DiffLossCoef { get; set; } = 0.05f;

		private float GetClutchTorque => GetClutchTorqueFor(Clutch);

		private float GetClutchTorqueB => GetClutchTorqueFor(_clutchB);

		public float RaceBotWheelInwardOffset { get; private set; }

		public event Action<bool> OnTrcStateChanged;

		public event Action OnGearChanged;

		public event Action OnEngineStarted;

		public event Action OnEngineShutdown;

		public event Action OnBlowOff;

		public event Action<bool> OnEspStateChanged;

		public event Action<bool> OnAbsStateChanged;

		public void ResetThrottleOverride()
		{
			_throttleOverride = 1f;
		}

		public void ResetDualClutch()
		{
			_clutchB = 1f;
			_dormantGear = 1;
			_ratioB = 0f;
			_ratioOverride = float.NaN;
		}

		public void SetTransmissionBehavior(ITransmissionBehavior behavior, GearboxProfile profile)
		{
			_behavior = behavior ?? new AtBehavior();
			_transmissionProfile = profile;
		}

		float ITransmissionHost.GearRatio(int index)
		{
			return _gearRatio[index];
		}

		void ITransmissionHost.StallEngine()
		{
			if (IsEngineWorks)
			{
				ShutdownEngine();
			}
		}

		private void RaiseEvent(PhysEventId id, bool boolArg = false)
		{
			_pendingEvents.Enqueue(new PhysEvent(id, boolArg));
		}

		public void FlushPendingEvents()
		{
			PhysEvent result;
			while (_pendingEvents.TryDequeue(out result))
			{
				switch (result.Id)
				{
				case PhysEventId.EngineShutdown:
					OnEngineShutdown?.Invoke();
					break;
				case PhysEventId.EngineStarted:
					OnEngineStarted?.Invoke();
					break;
				case PhysEventId.GearChanged:
					OnGearChanged?.Invoke();
					break;
				case PhysEventId.BlowOff:
					OnBlowOff?.Invoke();
					break;
				case PhysEventId.TrcStateChanged:
					OnTrcStateChanged?.Invoke(result.BoolArg);
					break;
				case PhysEventId.EspStateChanged:
					OnEspStateChanged?.Invoke(result.BoolArg);
					break;
				case PhysEventId.AbsStateChanged:
					OnAbsStateChanged?.Invoke(result.BoolArg);
					break;
				}
			}
		}

		public PhysEngine()
		{
			_body = new RB();
			_trailerJoint = new TrailerJoint
			{
				Trailer = null,
				Loc1 = Vector3.zero,
				Loc2 = Vector3.zero,
				Spring = 0f,
				Damper = 0f,
				HorAngLimit = 0f
			};
			_maxSteerAngle = 0.576f;
			_wheels = new Wheel[8];
			for (int i = 0; i < MaxGears; i++)
			{
				_gearRatio[i] *= 1.3f;
			}
			for (int j = 0; j < 8; j++)
			{
				_wheels[j] = new Wheel(this);
				_wheelSteerAng[j] = 0f;
			}
			SetClutchDefaultCurve();
			if (AiMode)
			{
				Abs = true;
			}
		}

		public void SetEngineOrientation(EngineOrientation ori)
		{
			_engineOrientation = ori switch
			{
				EngineOrientation.Transversely => Vector3.right, 
				EngineOrientation.Longitudinally => Vector3.forward, 
				_ => _engineOrientation, 
			};
		}

		public void SetOnlineRevLimiterTriggered(bool triggered)
		{
			if (OnLineCar)
			{
				RevLimiterTriggered = triggered;
			}
		}

		public void SetOnlineAccel(float accel)
		{
			if (OnLineCar)
			{
				_accel = Mathf.Clamp01(accel);
			}
		}

		public void SetOnlineEngineRunning(bool running)
		{
			if (OnLineCar && IsEngineWorks != running)
			{
				if (running)
				{
					_cutOut = false;
					RaiseEvent(PhysEventId.EngineStarted);
				}
				else
				{
					ShutdownEngine();
				}
			}
		}

		public void ShutdownEngine()
		{
			Ignition = false;
			_cutOut = true;
			CurrentRpm = 0f;
			RaiseEvent(PhysEventId.EngineShutdown);
		}

		public void SetTrailerJoint(TrailerJoint joint)
		{
			_trailerJoint = joint;
		}

		private void EmptyFuelTank()
		{
			if (IsEngineWorks)
			{
				Ignition = false;
				_wEngine = 0f;
				_accelIdle = 0f;
				_engineFuelTimer = 1f;
				_engineFuelFlow = 0f;
				RaiseEvent(PhysEventId.EngineShutdown);
			}
		}

		public void SetRpmController(float x, float y, float z)
		{
			RpmController = new float3(x, y, z);
		}

		public void StartEngine()
		{
			if (!(CurrentFuel <= 0f))
			{
				_cutOut = false;
				CurrentRpm = RpmController.x;
				RaiseEvent(PhysEventId.EngineStarted);
			}
		}

		private void UpdateSteeringGeometry()
		{
			float steerAng = _steerAng;
			float num = Mathf.Abs(steerAng);
			float num2 = Mathf.Sign(steerAng);
			float num3;
			float num4;
			if (num < 0.0005f)
			{
				num3 = steerAng;
				num4 = steerAng;
			}
			else
			{
				float num5 = _baseWheel / Mathf.Tan(num);
				float num6 = _halfTrack + 0.001f;
				if (num5 < num6)
				{
					num5 = num6;
				}
				float num7 = Mathf.Atan(_baseWheel / (num5 - _halfTrack));
				float num8 = Mathf.Atan(_baseWheel / (num5 + _halfTrack));
				num7 *= num2;
				num8 *= num2;
				float b = Mathf.Atan(_baseWheel / num5) * num2;
				num7 = Mathf.Lerp(num7, b, Ackerman);
				num8 = Mathf.Lerp(num8, b, Ackerman);
				if (num2 > 0f)
				{
					num3 = num7;
					num4 = num8;
				}
				else
				{
					num3 = num8;
					num4 = num7;
				}
			}
			_wheels[_idFrontLeft].SteerAngle = num3 - _wheels[_idFrontLeft].ToeIn;
			_wheels[_idFrontRight].SteerAngle = num4 + _wheels[_idFrontRight].ToeIn;
			_wheels[_idRearLeft].SteerAngle = 0f - _wheels[_idRearLeft].ToeIn - _wheels[_idRearLeft].AddSteerAngle * _steerAng * ((float)Math.PI / 180f);
			_wheels[_idRearRight].SteerAngle = _wheels[_idRearRight].ToeIn - _wheels[_idRearRight].AddSteerAngle * _steerAng * ((float)Math.PI / 180f);
		}

		public void RaiseGearChanged()
		{
			RaiseEvent(PhysEventId.GearChanged);
		}

		public void UpdateHillHold()
		{
			float num = 0.5f * (_wheels[_idFrontLeft].ForwardVelocity + _wheels[_idFrontRight].ForwardVelocity);
			float num2 = ((Gear == -1) ? (0f - num) : num);
			bool num3 = AutoClutch && IsEngineWorks && Gear != 0 && _brakeVal < 0.3f && _handbrakeVal <= 0f && _accelWant < 0.05f;
			if (_hillHoldActive && Gear != _hillHoldGear)
			{
				_hillHoldActive = false;
			}
			if (!num3)
			{
				_hillHoldActive = false;
			}
			else if (!_hillHoldActive && num2 < -0.1f && _vcLen < 1f)
			{
				_hillHoldActive = true;
				_hillHoldGear = Gear;
			}
			float holdBrakeTorque = (_hillHoldActive ? (0.6f * BrakeFrontTorque) : 0f);
			float holdBrakeTorque2 = (_hillHoldActive ? (0.6f * BrakeRearTorque) : 0f);
			_wheels[_idFrontLeft].HoldBrakeTorque = holdBrakeTorque;
			_wheels[_idFrontRight].HoldBrakeTorque = holdBrakeTorque;
			_wheels[_idRearLeft].HoldBrakeTorque = holdBrakeTorque2;
			_wheels[_idRearRight].HoldBrakeTorque = holdBrakeTorque2;
			if (_idRear1Left != -1)
			{
				_wheels[_idRear1Left].HoldBrakeTorque = holdBrakeTorque2;
			}
			if (_idRear1Right != -1)
			{
				_wheels[_idRear1Right].HoldBrakeTorque = holdBrakeTorque2;
			}
			if (_idRear2Left != -1)
			{
				_wheels[_idRear2Left].HoldBrakeTorque = holdBrakeTorque2;
			}
			if (_idRear2Right != -1)
			{
				_wheels[_idRear2Right].HoldBrakeTorque = holdBrakeTorque2;
			}
		}

		private void SetClutchDefaultCurve()
		{
			SetClutchMapCurveNumSamples(5);
			SetClutchMapCurveSample(4, 1f, 1f);
			SetClutchMapCurveSample(3, 0.7f, 0.9f);
			SetClutchMapCurveSample(2, 0.5f, 0.7f);
			SetClutchMapCurveSample(1, 0.3f, 0.15f);
			SetClutchMapCurveSample(0, 0f, 0f);
		}

		private void SetClutchMapCurveNumSamples(int value)
		{
			_clutchCurveNumSamples = Mathf.Min(value, 10);
		}

		private void SetClutchMapCurveSample(int i, float clutchPedal, float clutchPedalRemap)
		{
			if (i >= 0 && i < _clutchCurveNumSamples)
			{
				_clutchCurveX[i] = clutchPedal;
				_clutchCurveY[i] = clutchPedalRemap;
			}
		}

		public float GetClutchMap(float val)
		{
			float num = Mathf.Clamp01(val);
			if (_clutchCurveNumSamples < 2)
			{
				return (1f - num) * (1f - num);
			}
			int i;
			for (i = 0; i < _clutchCurveNumSamples - 1 && _clutchCurveX[i + 1] <= num; i++)
			{
			}
			float num2 = _clutchCurveX[i];
			float num3 = _clutchCurveX[i + 1];
			float num4 = _clutchCurveY[i];
			float num5 = _clutchCurveY[i + 1];
			if (num < num2)
			{
				return num4;
			}
			return Mathf.Max(1f - (num4 + (num - num2) * (num5 - num4) / (num3 - num2)), 0f);
		}

		public void SetFrontTyres(TyreType type, TyreWidth width, TyreProfile prof, WheelDiameter rim)
		{
			Wheel wheel = _wheels[_idFrontLeft];
			Wheel obj = _wheels[_idFrontRight];
			float r = PhysMath.CalculateWheelRadius(width, prof, rim);
			wheel.TyreSimulation.SetType(type, (int)width, (int)prof, r);
			obj.TyreSimulation.SetType(type, (int)width, (int)prof, r);
			wheel.Width = PhysMath.Mm2Meter(width);
			obj.Width = PhysMath.Mm2Meter(width);
			wheel.IsFront = true;
			obj.IsFront = true;
		}

		public void SetRearTyres(TyreType type, TyreWidth width, TyreProfile prof, WheelDiameter rim)
		{
			Wheel wheel = _wheels[_idRearLeft];
			Wheel obj = _wheels[_idRearRight];
			float r = PhysMath.CalculateWheelRadius(width, prof, rim);
			wheel.TyreSimulation.SetType(type, (int)width, (int)prof, r);
			obj.TyreSimulation.SetType(type, (int)width, (int)prof, r);
			wheel.Width = PhysMath.Mm2Meter(width);
			obj.Width = PhysMath.Mm2Meter(width);
			wheel.IsFront = false;
			obj.IsFront = false;
		}

		public void SetRear1Tyres(TyreType type, TyreWidth width, TyreProfile prof, WheelDiameter rim)
		{
			Wheel wheel = _wheels[_idRear1Left];
			Wheel obj = _wheels[_idRear1Right];
			float r = PhysMath.CalculateWheelRadius(width, prof, rim);
			wheel.TyreSimulation.SetType(type, (int)width, (int)prof, r);
			obj.TyreSimulation.SetType(type, (int)width, (int)prof, r);
			wheel.Width = PhysMath.Mm2Meter(width);
			obj.Width = PhysMath.Mm2Meter(width);
			wheel.IsFront = false;
			obj.IsFront = false;
		}

		public void SetRear2Tyres(TyreType type, TyreWidth width, TyreProfile prof, WheelDiameter rim)
		{
			Wheel wheel = _wheels[_idRear2Left];
			Wheel wheel2 = _wheels[_idRear2Right];
			wheel.TyreSimulation.SetType(type, (int)width, (int)prof, wheel.Radius);
			wheel2.TyreSimulation.SetType(type, (int)width, (int)prof, wheel2.Radius);
			wheel.Width = PhysMath.Mm2Meter(width);
			wheel2.Width = PhysMath.Mm2Meter(width);
			wheel.IsFront = false;
			wheel2.IsFront = false;
		}

		private void CalcValues()
		{
			_rpmStandingStart.y = CalculateUniversalEngageRpm();
			_rpmStandingStart.x = CalculateUniversalDisengageRpm();
		}

		private float CalculateUniversalEngageRpm()
		{
			float num = RpmController.y - RpmController.x;
			return RpmController.x + num * 0.3f;
		}

		private float CalculateUniversalDisengageRpm()
		{
			return Mathf.Max(RpmController.x + 500f, _rpmStandingStart.y * 0.7f);
		}

		private void OnSetEngineMaxTorq()
		{
			_effectiveDisplacementCc = ResolveEffectiveDisplacementCc();
			NormalizeTorqueCurveForPumpingModel();
			_maxStarterTorque = Mathf.Min(_engineMaxTorque * 0.2f, 150f);
			_fuelConsumptionAtCostantSpeed = 15f + _engineMaxTorque / 100f;
			if (ClutchLimit <= 0f)
			{
				ClutchLimit = _engineMaxTorque * 1.1f;
			}
			float num = _engineMaxTorque / 200f;
			_diffInertia = Mathf.Max(0.05f * num, 0.1f);
			_engineInertia = Mathf.Max(PhysMath.RoundTo(EngineInertiaRatio, 3), PhysMath.MinEngineInertia(_effectiveDisplacementCc));
			_gearBoxInertia = Mathf.Clamp(_diffInertia, 0.05f, _diffInertia * 1.2f);
			_engineGearUpRpm = RpmController.y - 100f;
			float value = _engineGearUpRpm * 0.4f;
			_engineGearDnRpm = Mathf.Clamp(value, RpmController.x + 200f, _engineGearUpRpm);
			CalcMaxAccel(1, 0f, Surface.Dryasphalt, out var _, out _maxDragAccel, out var _, out _maxBrakeAccel, out var _, out _maxSideAccel);
			float num2 = 30f / (float)Math.PI / Mathf.Max(_engineInertia, 0.05f);
			float num3 = Mathf.Max(PhysMath.CalcPumpingBrakeTorque(RpmController.y, _engineMaxTorqueRpm, _effectiveDisplacementCc, _isRotaryEngine) * num2, 1f);
			float num4 = Mathf.Max(0.5f * _engineMaxTorque * num2, 1f);
			float num5 = 2f / 9f;
			LimiterBounceDepthRpm = Mathf.Clamp(num5 / (1f / num3 + 1f / num4), 150f, 400f);
			LimiterFlutterHz = Mathf.Clamp(RpmController.y / 1200f, 2.5f, 7f);
		}

		public void SetEngineMaxTorque(float torque, float rpm, bool generate = false)
		{
			EngineMaxTorque = torque;
			EngineMaxTorqueRpm = rpm;
			if (generate)
			{
				SetEngineDefaultCurve();
			}
			else
			{
				SetEngineTorqueCurveFinalize();
			}
		}

		private void SetEngineDefaultCurve()
		{
			_torqueSamplesRaw = PhysMath.GenerateTorqueCurve(_engineMaxTorque, _engineMaxTorqueRpm, _toqsanTogyz, RpmController.y, RpmController.z, _davlenieUlitki.y, _davlenieUlitki.z, RpmController.x);
			ClutchLimit = Mathf.Round(_engineMaxTorque * (1f + PhysMath.TurboEfficiency((TurboType)_toqsanTogyz)) * 2f);
			SetEngineTorqueCurveFinalize();
		}

		public void SetEngineTurboParams(float maxTurboPressure, float2 range, TurboType type)
		{
			_davlenieUlitki = Mathf.Clamp(maxTurboPressure, 0f, 3f);
			_davlenieUlitki.y = range.x;
			_davlenieUlitki.z = range.y;
			_davlenieUlitki2.y = PhysMath.GetDragMultiplier(type);
			_toqsanTogyz = (int)type;
		}

		public void GetEngineTurboParams(out float maxTurboPressure, out float curTurboPressure, out float blowOff, out TurboType type)
		{
			maxTurboPressure = _davlenieUlitki.x;
			curTurboPressure = _davlenieUlitki2.x;
			blowOff = _blowOffStrength;
			type = (TurboType)_toqsanTogyz;
		}

		public void GetTurboSpec(out TurboType type, out float maxTurboPressure, out float minTurboRpm, out float maxTurboRpm)
		{
			maxTurboPressure = _davlenieUlitki.x;
			minTurboRpm = _davlenieUlitki.y;
			maxTurboRpm = _davlenieUlitki.z;
			type = (TurboType)_toqsanTogyz;
		}

		public IWheel GetWheel(int i)
		{
			return _wheels[i];
		}

		public void SetRigidBody(ref RbEntity desc)
		{
			_body.SetParams(ref desc);
		}

		public void GetRigidBody(ref RbEntity desc)
		{
			_body.GetParams(ref desc);
		}

		public void GetTotals(out Vector3 fdt, out Vector3 mFdt)
		{
			_body.GetTotals(out fdt, out mFdt);
		}

		public void UpdateTraffic(float dt)
		{
			Vector3 vector = new Vector3(0f, -9.80665f, 0f);
			_body.ClearTotals();
			_body.StoreVelocity();
			_davlenieUlitki2.x = 0f;
			_curBlowOff = 0f;
			Vector3 up = PhysMath.GetUp(_body.Matrix);
			Vector3 forward = PhysMath.GetForward(_body.Matrix);
			Vector3 right = PhysMath.GetRight(_body.Matrix);
			Vector3 midNorm = Vector3.zero;
			Vector3 midTarget = Vector3.zero;
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				if (_wheels[i].Enabled && _wheels[i].UpdateRoadDist_Traffic(dt, ref midNorm, ref midTarget))
				{
					num++;
				}
			}
			if (num == 0)
			{
				_mgdt = vector * (_body.Mass * dt);
				_body.AddExtForce(_mgdt);
				_body.ProcessExternalForces();
				return;
			}
			midTarget /= (float)num;
			PhysMath.Vector3NormalizeSafe(ref midNorm);
			vector = up * (0f - vector.magnitude);
			_mgdt = vector * (_body.Mass * dt);
			_body.AddExtForce(_mgdt);
			Vector3 point = (_wheels[_idFrontLeft].TopPos + _wheels[_idFrontRight].TopPos) * 0.5f;
			Vector3 point2 = (_wheels[_idRearLeft].TopPos + _wheels[_idRearRight].TopPos) * 0.5f;
			Vector3 pos = _body.Matrix.MultiplyPoint3x4(point);
			Vector3 pos2 = _body.Matrix.MultiplyPoint3x4(point2);
			Vector3 velocity = _body.GetVelocity(pos);
			Vector3 velocity2 = _body.GetVelocity(pos2);
			velocity -= up * Vector3.Dot(up, velocity);
			velocity2 -= up * Vector3.Dot(up, velocity2);
			float magnitude = velocity.magnitude;
			float magnitude2 = velocity2.magnitude;
			Vector3 vector2 = forward * Mathf.Cos(_steerAngWant) + right * Mathf.Sin(0f - _steerAngWant);
			if (magnitude <= 1.1f)
			{
				Vector3 vector3 = Vector3.Cross(up, vector2);
				float value = (0f - Vector3.Dot(vector3, velocity)) * 10f;
				value = Mathf.Clamp(value, -10f, 10f);
				_body.AddExtForce(pos, vector3 * (value * _body.Mass * dt));
			}
			else
			{
				Vector3 vector4 = velocity / magnitude;
				float f = Vector3.Dot(vector2, vector4);
				Vector3 rhs = Vector3.Cross(vector4, vector2);
				float num2 = Mathf.Atan2(rhs.magnitude, Mathf.Abs(f));
				if (Vector3.Dot(midNorm, rhs) < 0f)
				{
					num2 = 0f - num2;
				}
				float num3 = Mathf.Clamp(num2 * 50f, -10f, 10f);
				Vector3 vector5 = Vector3.Cross(up, vector2);
				_body.AddExtForce(pos, vector5 * (num3 * _body.Mass * dt));
			}
			float num4 = 0f;
			if (magnitude2 <= 1.1f)
			{
				float value2 = (0f - Vector3.Dot(right, velocity2)) * 10f;
				value2 = Mathf.Clamp(value2, -10f, 10f);
				_body.AddExtForce(pos2, right * (value2 * _body.Mass * dt));
			}
			else
			{
				Vector3 vector6 = velocity2 / magnitude2;
				num4 = Vector3.Dot(forward, vector6);
				Vector3 rhs2 = Vector3.Cross(vector6, forward);
				float num5 = Mathf.Atan2(rhs2.magnitude, Mathf.Abs(num4));
				if (Vector3.Dot(midNorm, rhs2) < 0f)
				{
					num5 = 0f - num5;
				}
				float num6 = Mathf.Clamp(num5 * 50f, -10f, 10f);
				_body.AddExtForce(pos2, right * (num6 * _body.Mass * dt));
			}
			_accel = _accelWant;
			if (CurrentRpm > RpmController.z)
			{
				CurrentRpm = RpmController.z;
				_accel = 0f;
			}
			if (CurrentGear != 1)
			{
				if (CurrentRpm > _engineGearUpRpm && CurrentGear > 1)
				{
					GearUp();
				}
				if (CurrentRpm < _engineGearDnRpm && CurrentGear > 2)
				{
					GearDown();
				}
			}
			CurrentGear = _targetGear;
			_ratio = _gearRatio[CurrentGear] * FinalDrive;
			float num7 = 0f;
			float b = RpmController.x / (30f / (float)Math.PI);
			if (CurrentGear == 1)
			{
				_wEngine = 0f;
			}
			else
			{
				_wEngine = magnitude2 / _wheels[_idRearLeft].RadiusLoaded * Mathf.Abs(_ratio);
				num7 = _engineMaxTorque * _ratio * _accel;
			}
			float num8 = _body.Mass * 5f;
			float num9 = Mathf.Clamp(num7 / _wheels[_idRearLeft].RadiusLoaded, 0f - num8, num8);
			_wEngine = Mathf.Max(_wEngine, b);
			Vector3 vector7 = up * Vector3.Dot(_body.Velocity, up);
			_body.AddExtForce(vector7 * ((0f - _body.Mass) * 6f * dt));
			_body.AddExtForce(midTarget, forward * (num9 * dt));
			float num10 = (0f - _brakeVal) * Mathf.Clamp(_body.Mass * 18f, 0f, 25000f);
			Vector3 vector8 = forward * num10;
			Vector3 vector9 = ((num4 < 0f) ? (-vector8) : ((num4 != 0f) ? vector8 : (velocity2 * (num10 * 0.3f))));
			vector8 = vector9;
			_body.AddExtForce(midTarget, vector8 * dt);
			Vector3 angularVelocity = _body.AngularVelocity;
			angularVelocity -= up * Vector3.Dot(up, angularVelocity);
			Vector3 normalized = angularVelocity.normalized;
			Vector3 tdt = angularVelocity * ((0f - _body.GetInertiaDir(normalized)) * 5f * dt);
			_body.AddExtTorque(tdt);
			_body.ProcessExternalForces();
			if (PositionIntegration)
			{
				_body.UpdatePosition(dt);
			}
		}

		public void UpdateWheels(Matrix4x4 mat)
		{
			for (int i = 0; i < 8; i++)
			{
				Wheel wheel = _wheels[i];
				if (wheel.Enabled)
				{
					if (OnLineCar)
					{
						wheel.UpdateRenderMatrixOnline(mat, Time.deltaTime);
					}
					else if (SimpleMode)
					{
						wheel.UpdateRenderMatrix_Simple(mat, RaceBotWheelInwardOffset);
					}
					else
					{
						wheel.UpdateRenderMatrix_Advanced(mat);
					}
				}
			}
		}

		public void ResetTransmission(bool resetTransmission = true)
		{
			_wDiff = 0f;
			_wDiffF = 0f;
			_wDiffR = 0f;
			_accelWant = 0f;
			for (int i = 0; i < 8; i++)
			{
				_wheels[i].AngularVelocity = 0f;
			}
			if (resetTransmission)
			{
				ParkingGear = false;
				if (AutoClutch || AiMode || At)
				{
					CurrentGear = 2;
				}
				else
				{
					CurrentGear = 1;
				}
			}
		}

		public int GetOptimalGearForDriveEngagement(float velMps)
		{
			if (Mathf.Abs(velMps) < 0.5f)
			{
				return 1;
			}
			float x = RpmController.x;
			float y = RpmController.y;
			float num = x + (y - x) * 0.4f;
			float num2 = ((GearType == GearboxType.GearRwd) ? _wheels[_idRearLeft].Radius : ((GearType != GearboxType.GearFwd) ? _wheels[_idRearLeft].Radius : _wheels[_idFrontLeft].Radius));
			num2 *= 0.95f;
			float num3 = Mathf.Abs(velMps) / num2;
			int result = 1;
			float num4 = float.MaxValue;
			for (int i = 1; i <= _numGears; i++)
			{
				float num5 = num3 * _gearRatio[i + 1] * FinalDrive * (30f / (float)Math.PI);
				if (!(num5 < x + 200f) && !(num5 > y - 200f))
				{
					float num6 = Mathf.Abs(num5 - num);
					if (num6 < num4)
					{
						num4 = num6;
						result = i;
					}
				}
			}
			return result;
		}

		public void GearUp()
		{
			if (CurrentGear != _numGears + 1)
			{
				Gear++;
			}
		}

		public void GearDown()
		{
			if (CurrentGear != 0)
			{
				Gear--;
			}
		}

		public void SetGearRatio(int gear, float val)
		{
			gear = Mathf.Max(gear, 0);
			val = ((gear == 0) ? (0f - Mathf.Abs(val)) : Mathf.Abs(val));
			if (gear < 20)
			{
				_gearRatio[gear] = val;
			}
		}

		public float GetGearRatio(int num)
		{
			if (num < 0)
			{
				num = 0;
			}
			return _gearRatio[num];
		}

		public void SetFront(int left, int right, float k = 0.2f)
		{
			_idFrontLeft = left;
			_idFrontRight = right;
			_wheels[_idFrontLeft].SuspKLong = 0f - k;
			_wheels[_idFrontRight].SuspKLong = 0f - k;
			_wheels[_idFrontLeft].IsLeft = true;
			_wheels[_idFrontRight].IsLeft = false;
			_halfTrack = (_wheels[_idFrontRight].TopPos.x - _wheels[_idFrontLeft].TopPos.x) * 0.5f;
		}

		public void GetFront(out int left, out int right)
		{
			left = _idFrontLeft;
			right = _idFrontRight;
		}

		public void SetRear(int left, int right, float k = 0.2f)
		{
			_idRearLeft = left;
			_idRearRight = right;
			_wheels[_idRearLeft].SuspKLong = k;
			_wheels[_idRearRight].SuspKLong = k;
			_wheels[_idRearLeft].IsLeft = true;
			_wheels[_idRearRight].IsLeft = false;
			_baseWheel = _wheels[_idFrontLeft].TopPos.z - _wheels[_idRearLeft].TopPos.z;
		}

		public void SetRear1(int left, int right)
		{
			_idRear1Left = left;
			_idRear1Right = right;
			_wheels[_idRear1Left].SuspKLong = 0f;
			_wheels[_idRear1Right].SuspKLong = 0f;
			_wheels[_idRear1Left].IsLeft = true;
			_wheels[_idRear1Right].IsLeft = false;
		}

		public void SetRear2(int left, int right)
		{
			_idRear2Left = left;
			_idRear2Right = right;
			_wheels[_idRear2Left].SuspKLong = 0f;
			_wheels[_idRear2Right].SuspKLong = 0f;
			_wheels[_idRear2Left].IsLeft = true;
			_wheels[_idRear2Right].IsLeft = false;
		}

		public void GetRear(out int left, out int right)
		{
			left = _idRearLeft;
			right = _idRearRight;
		}

		public void SetEngineTorqueSamples(TorqueCurveSample[] samples)
		{
			_torqueSamplesRaw = samples ?? Array.Empty<TorqueCurveSample>();
			SetEngineTorqueCurveFinalize();
		}

		private void SetEngineTorqueCurveFinalize()
		{
			CalcValues();
			OnSetEngineMaxTorq();
		}

		private float GetEngineMaxTorqueRpm(float rpm)
		{
			switch (_torqueSamples.Length)
			{
			case 0:
				return 0f;
			case 1:
				return _torqueSamples[0].Torque;
			default:
			{
				float a = 0f;
				for (int i = 0; i < _torqueSamples.Length - 1; i++)
				{
					if (rpm < _torqueSamples[i].Rpm)
					{
						return _torqueSamples[i].Torque;
					}
					a = _torqueSamples[i].Torque + (rpm - _torqueSamples[i].Rpm) * (_torqueSamples[i + 1].Torque - _torqueSamples[i].Torque) / (_torqueSamples[i + 1].Rpm - _torqueSamples[i].Rpm);
					if (_torqueSamples[i + 1].Rpm > rpm)
					{
						break;
					}
				}
				return Mathf.Max(a, 0f);
			}
			}
		}

		private float GetEngineTorqueRpm(float rpm, float rpmMaxT)
		{
			float num = 0f;
			float num2 = CalcEngineBrakeTorqueRpm(rpm);
			rpmMaxT += num2;
			rpmMaxT *= Mathf.Clamp(PhysMath.CalcEngineBoostMultiplier(_davlenieUlitki2.x, (TurboType)_toqsanTogyz), 1f, 3f);
			if (Starter > 0.1f)
			{
				num = 5f * _maxStarterTorque * (_maxStarterRpm - rpm) / _maxStarterRpm;
				num = Mathf.Clamp(num, (0f - _maxStarterTorque) * 0.3f, _maxStarterTorque);
				num *= Starter;
			}
			if (_cutOut)
			{
				if (rpm > 10f)
				{
					num -= num2;
				}
				return num;
			}
			float num3 = Mathf.Max(_accel, _revMatchBlip);
			float num4 = rpmMaxT * (num3 * (1f - _accelIdle) + _accelIdle) * _engineFuelFlow;
			float num5 = Mathf.Min((1f + _body.Velocity.magnitude * 0.2f) * num3 * (RpmController.y - RpmController.x) + RpmController.x, RpmController.y);
			if (IsActiveLaunchControlNow && !OverheatTransmission && !At)
			{
				num5 = Mathf.Min(num5, _lcController.x);
			}
			if (rpm > num5)
			{
				float num6 = Mathf.Max(1f - (rpm - num5) * 0.002f, 0f);
				num4 *= num6;
			}
			num4 -= num2;
			return num4 + num;
		}

		private void UpdateSuspensionAngles(int left, int right, SuspensionType type)
		{
			switch (type)
			{
			case SuspensionType.Dependent:
			{
				float num = Mathf.Atan2(_wheels[left].CurSpringLen - _wheels[right].CurSpringLen, Vector3.Distance(_wheels[left].TopPos, _wheels[right].TopPos)) * 57.29578f;
				_wheels[left].CamberSusp = num;
				_wheels[right].CamberSusp = 0f - num;
				_wheels[left].CamberAddTop = 0f;
				_wheels[left].CamberAddDown = 0f;
				_wheels[left].Camber = 0f;
				_wheels[right].CamberAddTop = 0f;
				_wheels[right].CamberAddDown = 0f;
				_wheels[right].Camber = 0f;
				break;
			}
			case SuspensionType.DoubleWishbone:
				_wheels[left].CamberSusp = 0f;
				_wheels[right].CamberSusp = 0f;
				_wheels[left].CamberAddTop = -2f;
				_wheels[left].CamberAddDown = 2f;
				_wheels[right].CamberAddTop = -2f;
				_wheels[right].CamberAddDown = 2f;
				break;
			case SuspensionType.MultiLink:
				_wheels[left].CamberSusp = 0f;
				_wheels[right].CamberSusp = 0f;
				_wheels[left].CamberAddTop = -1.5f;
				_wheels[left].CamberAddDown = 1.5f;
				_wheels[right].CamberAddTop = -1.5f;
				_wheels[right].CamberAddDown = 1.5f;
				break;
			case SuspensionType.SemiDependent:
				_wheels[left].CamberSusp = -1f;
				_wheels[right].CamberSusp = -1f;
				_wheels[left].CamberAddTop = -1.5f;
				_wheels[left].CamberAddDown = 2f;
				_wheels[right].CamberAddTop = -1.5f;
				_wheels[right].CamberAddDown = 2f;
				break;
			case SuspensionType.LeafSpring:
				_wheels[left].CamberSusp = 0f;
				_wheels[right].CamberSusp = 0f;
				_wheels[left].CamberAddTop = -0.5f;
				_wheels[left].CamberAddDown = 0.5f;
				_wheels[right].CamberAddTop = -0.5f;
				_wheels[right].CamberAddDown = 0.5f;
				break;
			default:
				_wheels[left].CamberSusp = 0f;
				_wheels[right].CamberSusp = 0f;
				_wheels[left].CamberAddTop = -3f;
				_wheels[left].CamberAddDown = 3f;
				_wheels[right].CamberAddTop = -3f;
				_wheels[right].CamberAddDown = 3f;
				break;
			}
		}

		public void EngineStarterRpm(float min, float max)
		{
			_minStarterRpm = min;
			_maxStarterRpm = max;
		}

		private void UpdateStabilizers()
		{
			if (!OnLineCar)
			{
				if (_idFrontLeft != -1 && _idFrontRight != -1 && _wheels[_idFrontLeft].Enabled)
				{
					Wheel wheel = _wheels[_idFrontLeft];
					Wheel obj = _wheels[_idFrontRight];
					float num = (obj.CurSpringLen - wheel.CurSpringLen) * FrontStabSpringCoef;
					wheel.ExtStabForce = 0f - num;
					obj.ExtStabForce = num;
				}
				if (_idRearLeft != -1 && _idRearRight != -1 && _wheels[_idRearLeft].Enabled)
				{
					Wheel wheel2 = _wheels[_idRearLeft];
					Wheel obj2 = _wheels[_idRearRight];
					float num2 = (obj2.CurSpringLen - wheel2.CurSpringLen) * RearStabSpringCoef;
					wheel2.ExtStabForce = 0f - num2;
					obj2.ExtStabForce = num2;
				}
				if (_idRear1Left != -1 && _idRear1Right != -1 && _wheels[_idRear1Left].Enabled)
				{
					Wheel wheel3 = _wheels[_idRear1Left];
					Wheel obj3 = _wheels[_idRear1Right];
					float num3 = (obj3.CurSpringLen - wheel3.CurSpringLen) * RearStabSpringCoef;
					wheel3.ExtStabForce = 0f - num3;
					obj3.ExtStabForce = num3;
				}
				if (_idRear2Left != -1 && _idRear2Right != -1 && _wheels[_idRear2Left].Enabled)
				{
					Wheel wheel4 = _wheels[_idRear2Left];
					Wheel obj4 = _wheels[_idRear2Right];
					float num4 = (obj4.CurSpringLen - wheel4.CurSpringLen) * RearStabSpringCoef;
					wheel4.ExtStabForce = 0f - num4;
					obj4.ExtStabForce = num4;
				}
			}
		}

		private void CalcTurboCharger(float dt)
		{
			int toqsanTogyz = _toqsanTogyz;
			if (toqsanTogyz == 0 || toqsanTogyz == 6 || _davlenieUlitki.x == 0f || !(_davlenieUlitki.z - _davlenieUlitki.y > 0f))
			{
				return;
			}
			bool flag = _accelWant > 0.1f;
			_davlenieUlitki2.x = PhysMath.TurboSpoolStep(_davlenieUlitki2.x, CurrentRpm, Load, flag, _davlenieUlitki.x, _davlenieUlitki.y, _davlenieUlitki.z, (TurboType)_toqsanTogyz, dt);
			if (_davlenieUlitki2.x > 0f)
			{
				_curBlowOff += Mathf.Abs(_davlenieUlitki2.x / _davlenieUlitki.x * dt) * 0.5f;
			}
			_curBlowOff = Mathf.Clamp01(_curBlowOff);
			GearboxKind kind = _transmissionProfile.Kind;
			bool flag2 = kind == GearboxKind.MT || kind == GearboxKind.AMT || kind == GearboxKind.Sequential;
			bool flag3 = (kind == GearboxKind.AT || kind == GearboxKind.DCT) && !Automatic;
			bool flag4 = IsShifting && CurrentRpm >= _davlenieUlitki.y && (flag2 | flag3);
			bool flag5 = !flag && _wasOnThrottle;
			bool flag6 = flag4 && !_wasShiftDump;
			if (flag5 | flag6)
			{
				float num = Mathf.Clamp01(_davlenieUlitki2.x / _davlenieUlitki.x);
				if (flag6 && !flag5 && !flag2)
				{
					num *= 0.4f;
				}
				_blowOffStrength = num;
				RaiseEvent(PhysEventId.BlowOff);
			}
			_wasOnThrottle = flag;
			_wasShiftDump = flag4;
		}

		private void CalcSuperCharger(float dt)
		{
			if (_toqsanTogyz != 6 || _davlenieUlitki.x == 0f || PhysicsThreadCache.TimeScale == 0f)
			{
				return;
			}
			if (!IsEngineWorks)
			{
				_davlenieUlitki2.x = 0f;
				return;
			}
			float x = RpmController.x;
			float x2 = _davlenieUlitki.x;
			float y = _davlenieUlitki.y;
			float z = _davlenieUlitki.z;
			float currentRpm = CurrentRpm;
			float num = 0f;
			if (currentRpm <= x)
			{
				num = 0f;
			}
			else if (currentRpm > x && currentRpm < y)
			{
				float num2 = (currentRpm - x) / (y - x);
				num = 0.1f * x2 * num2;
			}
			else if (currentRpm >= y && currentRpm <= z)
			{
				float num3 = (currentRpm - y) / (z - y);
				num = x2 * num3;
			}
			else if (currentRpm > z)
			{
				num = x2;
			}
			float num4 = num;
			float num5 = PhysMath.CalcGasPressure(CurrentRpm, Load, x2, y, z);
			if (num5 < num4)
			{
				num4 = num5;
			}
			float num6 = 2f;
			_davlenieUlitki2.x = Mathf.Lerp(_davlenieUlitki2.x, num4, num6 * dt);
			_davlenieUlitki2.x = Mathf.Clamp(_davlenieUlitki2.x, 0f, x2 * 1.1f);
			if (Accel > 0f)
			{
				_davlenieUlitki2.x = PhysMath.RoundTo(_davlenieUlitki2.x, 2);
			}
		}

		public void BlowOff()
		{
			_davlenieUlitki2.x = 0f;
			_curBlowOff = 0f;
		}

		private float CalcEngineBrakeTorqueRpm(float rpm)
		{
			return PhysMath.CalcPumpingBrakeTorque(rpm, _engineMaxTorqueRpm, _effectiveDisplacementCc, _isRotaryEngine);
		}

		private float LegacyBrakeTorqueForNormalization(float rpm)
		{
			return PhysMath.LegacyBrakeTorque(rpm, _engineMaxTorqueRpm, _engineMaxTorque);
		}

		private float ResolveEffectiveDisplacementCc()
		{
			if (_displacementCc > 0f)
			{
				return _displacementCc;
			}
			float num = 0f;
			for (int i = 0; i < _torqueSamplesRaw.Length; i++)
			{
				if (_torqueSamplesRaw[i].Torque > num)
				{
					num = _torqueSamplesRaw[i].Torque;
				}
			}
			return PhysMath.EstimateDisplacementCc(num);
		}

		private void NormalizeTorqueCurveForPumpingModel()
		{
			int num = _torqueSamplesRaw.Length;
			if (num == 0)
			{
				_torqueSamples = Array.Empty<TorqueCurveSample>();
				return;
			}
			if (_torqueSamples == null || _torqueSamples.Length != num)
			{
				_torqueSamples = new TorqueCurveSample[num];
			}
			for (int i = 0; i < num; i++)
			{
				float rpm = _torqueSamplesRaw[i].Rpm;
				float boostMultiplier = Mathf.Clamp(PhysMath.CalcEngineBoostMultiplier(PhysMath.CalcTurboTargetPressure(rpm, 1f, _davlenieUlitki.x, _davlenieUlitki.y, _davlenieUlitki.z), (TurboType)_toqsanTogyz), 1f, 3f);
				float legacyBrake = LegacyBrakeTorqueForNormalization(rpm);
				float pumpingBrake = PhysMath.CalcPumpingBrakeTorque(rpm, _engineMaxTorqueRpm, _effectiveDisplacementCc, _isRotaryEngine);
				_torqueSamples[i].Rpm = rpm;
				_torqueSamples[i].Torque = PhysMath.NormalizeTorqueSample(_torqueSamplesRaw[i].Torque, boostMultiplier, legacyBrake, pumpingBrake);
			}
		}

		public void SetLaunchControl(float lcRpm, float lcTimer, bool enable = false)
		{
			_hasLaunchControl = enable;
			_lcController.x = Mathf.Min(lcRpm, RpmController.y);
			_lcController.y = lcTimer / 1000f;
		}

		public void GetLaunchControlSettings(out bool isHave, out float lcRpm, out float lcTimer, out float lcHoldTime)
		{
			isHave = _hasLaunchControl;
			lcRpm = _lcController.x;
			lcTimer = _lcController.z;
			lcHoldTime = _lcController.y;
		}

		public void GetDynoGraphAtRpm(float rpm, out float torq, out float wtorq, out float hp, out float whp)
		{
			float curPressure = PhysMath.CalcTurboTargetPressure(rpm, 1f, _davlenieUlitki.x, _davlenieUlitki.y, _davlenieUlitki.z);
			float pumpingBrake = CalcEngineBrakeTorqueRpm(rpm);
			float num = PhysMath.DynoTorqueAtRpm(GetEngineMaxTorqueRpm(rpm), Mathf.Clamp(PhysMath.CalcEngineBoostMultiplier(curPressure, (TurboType)_toqsanTogyz), 1f, 3f), pumpingBrake);
			float num2 = Mathf.Min(RpmController.y, RpmController.z);
			if (rpm < _minStarterRpm || rpm > num2)
			{
				num = 0f;
			}
			torq = num;
			float num3 = rpm / (30f / (float)Math.PI);
			hp = num * num3 / 735.4987f;
			num = Mathf.Min(num, ClutchLimit);
			whp = hp * (1f - TransmissionLossCoef);
			wtorq = num * (1f - TransmissionLossCoef);
		}

		public float CalcTimeForDistance(float targetDistance, Surface surface)
		{
			float num = 0.5f * AeroCx * AeroSx * 1.29f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0.005f;
			float num6 = ((GearType == GearboxType.GearRwd) ? _wheels[_idRearLeft].Radius : ((GearType != GearboxType.GearFwd) ? _wheels[_idRearLeft].Radius : _wheels[_idFrontLeft].Radius));
			num6 *= 0.95f;
			CalcMaxAccel(1, 0f, surface, out var _, out var tireDragAccel, out var _, out var tireBrakeAccel, out var airBrakeAccel, out var lateralAccel);
			float b = tireDragAccel * _body.Mass * 1.8f;
			int num7 = 1;
			for (int i = 0; i <= 20000; i++)
			{
				float num8 = _gearRatio[num7 + 1] * FinalDrive;
				if (num2 / ((float)Math.PI / 30f / num8 * num6) >= RpmController.y * 0.98f && num7 < _numGears)
				{
					num4 += GearShiftingDelay;
					num7++;
				}
				float num9 = _gearRatio[num7 + 1] * FinalDrive;
				float num10 = num2 / ((float)Math.PI / 30f / num9 * num6);
				if (num7 == 1 && num10 < _rpmStandingStart.y)
				{
					num10 = _rpmStandingStart.y;
				}
				GetDynoGraphAtRpm(num10, out lateralAccel, out airBrakeAccel, out tireBrakeAccel, out var whp);
				float num11 = Mathf.Min(whp * 735.4987f * PowerCalibration / Mathf.Max(num2, 1f), b);
				float num12 = num2 * num2 * num + 0.117679805f * _body.Mass;
				num2 += (num11 - num12) / _body.Mass * num5;
				num3 += num2 * num5;
				if (num3 >= targetDistance)
				{
					return num4;
				}
				num4 += num5;
			}
			return num4;
		}

		public float CalcTopSpeed()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0.5f * AeroCx * AeroSx * 1.29f;
			float num5 = 0.012f * (AeroFrontDownforce + AeroRearDownforce) / 77.284f;
			float num6 = 9.80665f * _body.Mass * 0.012f;
			float radius = _wheels[_idRearLeft].Radius;
			if (GearType == GearboxType.GearFwd)
			{
				radius = _wheels[_idFrontLeft].Radius;
			}
			radius *= 0.95f;
			float num7 = RpmController.y;
			if (RpmController.y < 1f)
			{
				num7 = 20000f;
			}
			for (int i = 0; i <= 20; i++)
			{
				float num8 = num7 * ((float)i / 20f);
				GetDynoGraphAtRpm(num8, out var _, out var _, out var _, out var whp);
				float num9 = whp * 735.4987f;
				if (num8 > num && num > 0f)
				{
					for (int j = 1; j <= _numGears; j++)
					{
						float num10 = _gearRatio[j + 1] * FinalDrive;
						float num11 = num * ((float)Math.PI / 30f) / num10 * radius;
						float num12 = num8 * ((float)Math.PI / 30f) / num10 * radius;
						float num13 = num2 / num11;
						float num14 = num9 / num12;
						float num15 = num4 + num5;
						float num16 = num11 * num11 * num15 + num6;
						float num17 = num12 * num12 * num15 + num6;
						if (num13 >= num16 && num14 <= num17)
						{
							float num18 = num16 - num13 - num17 + num14;
							float num19 = 0f;
							if (Mathf.Abs(num18) > 0.001f)
							{
								num19 = (num16 - num13) / num18;
							}
							float num20 = num11 * (1f - num19) + num12 * num19;
							if (num20 > num3)
							{
								num3 = num20;
							}
						}
						if (num14 > num17 && num12 > num3)
						{
							num3 = num12;
						}
					}
				}
				num = num8;
				num2 = num9;
			}
			return num3;
		}

		public float CalcAccelTime(float speedFrom, float speedTo, Surface surface)
		{
			speedFrom /= 3.6f;
			speedTo /= 3.6f;
			float num = 0f;
			float num2 = 0f;
			float num3 = speedFrom;
			float num4 = speedFrom;
			int num5 = 1;
			float num6 = 0.5f * AeroCx * AeroSx * 1.29f;
			float num7 = 9.80665f * _body.Mass;
			float num8 = 0.012f * num7;
			CalcMaxAccel(1, 0f, surface, out var _, out var tireDragAccel, out var _, out var tireBrakeAccel, out var airBrakeAccel, out var lateralAccel);
			float b = tireDragAccel * _body.Mass * 1.8f;
			float num9 = ((GearType == GearboxType.GearRwd) ? _wheels[_idRearLeft].Radius : ((GearType != GearboxType.GearFwd) ? _wheels[_idRearLeft].Radius : _wheels[_idFrontLeft].Radius));
			num9 *= 0.95f;
			for (int i = 0; i <= 500; i++)
			{
				float num10 = _gearRatio[num5 + 1] * FinalDrive;
				if (num3 / ((float)Math.PI / 30f / num10 * num9) >= RpmController.y * 0.98f && num5 < _numGears)
				{
					num2 += GearShiftingDelay;
					num5++;
				}
				float num11 = _gearRatio[num5 + 1] * FinalDrive;
				float num12 = num3 / ((float)Math.PI / 30f / num11 * num9);
				if (num5 == 1 && num12 < _rpmStandingStart.x)
				{
					num12 = _rpmStandingStart.x;
				}
				GetDynoGraphAtRpm(num12, out lateralAccel, out airBrakeAccel, out tireBrakeAccel, out var whp);
				float num13 = Mathf.Min(whp * 735.4987f * PowerCalibration / Mathf.Max(num3, 1f), b);
				float num14 = num3 * num3 * num6 + num8;
				float num15 = (num13 - num14) / _body.Mass;
				num3 += num15 * 0.05f;
				if (num3 > speedTo)
				{
					float num16 = 1f;
					if (num3 > num4 + 0.001f)
					{
						num16 = (speedTo - num4) / (num3 - num4);
					}
					return num + num2 + num16 * 0.05f;
				}
				num += 0.05f;
				num4 = num3;
			}
			return num + num2;
		}

		public float CalcBrakeDistance(float speedFrom, Surface surface)
		{
			float num = speedFrom / 3.6f;
			float num2 = 0f;
			float mass = _body.Mass;
			float num3 = 0.5f * AeroCx * AeroSx * 1.29f;
			float num4 = AeroFrontDownforce / 77.284f;
			float num5 = AeroRearDownforce / 77.284f;
			float num6 = Vector3.Dot(Vector3.forward, _wheels[_idFrontLeft].TopPos - _body.LocalCenterMass);
			float num7 = Vector3.Dot(Vector3.forward, _body.LocalCenterMass - _wheels[_idRearLeft].TopPos);
			float num8 = num6 + num7;
			float num9 = num7 / num8;
			float num10 = 1f - num9;
			float b = 2f * FrontBrakeTorque / _wheels[_idFrontLeft].Radius;
			float b2 = 2f * RearBrakeTorque / _wheels[_idRearLeft].Radius;
			for (int i = 0; i <= 500; i++)
			{
				float num11 = num * num * num3;
				float value = num * num * (num4 + num5);
				value = Mathf.Clamp(value, (0f - mass) * 5f, float.MaxValue);
				float num12 = Mathf.Max(9.80665f * mass + value, 0f);
				float num13 = num12 * 0.001f;
				float a = _wheels[_idFrontLeft].TyreSimulation.GetMuStatLong(num, num13 * num10 * 0.5f, surface) * num12 * num10;
				float a2 = _wheels[_idRearLeft].TyreSimulation.GetMuStatLong(num, num13 * num9 * 0.5f, surface) * num12 * num9;
				float num14 = Mathf.Min(a, b);
				float num15 = Mathf.Min(a2, b2);
				float num16 = 0.117679805f * mass;
				float num17 = (num14 + num15 + num11 + num16) / mass;
				if (num17 <= 0f)
				{
					break;
				}
				float num18 = num - num17 * 0.05f;
				if (num18 <= 0f)
				{
					float num19 = num / num17;
					num2 += num * num19 - 0.5f * num17 * num19 * num19;
					break;
				}
				num2 += num * 0.05f - 0.5f * num17 * 0.05f * 0.05f;
				num = num18;
			}
			return num2;
		}

		public void CalcMaxAccel(int currentGear, float velocity, Surface surfaceType, out float engineDragAccel, out float tireDragAccel, out float brakeAccel, out float tireBrakeAccel, out float airBrakeAccel, out float lateralAccel)
		{
			float mass = _body.Mass;
			float num = 0.5f * AeroCx * AeroSx * 1.29f;
			float num2 = velocity * velocity * num;
			float num3 = AeroFrontDownforce / 77.284f;
			float num4 = AeroRearDownforce / 77.284f;
			float value = velocity * velocity * (num3 + num4);
			value = Mathf.Clamp(value, (0f - mass) * 5f, float.MaxValue);
			float a = 9.80665f * mass + value;
			a = Mathf.Max(a, 0f);
			float num5 = a * 0.001f;
			float num6 = Vector3.Dot(Vector3.forward, _wheels[_idFrontLeft].TopPos - _body.LocalCenterMass);
			float num7 = Vector3.Dot(Vector3.forward, _body.LocalCenterMass - _wheels[_idRearLeft].TopPos);
			float num8 = num6 + num7;
			float num9 = num7 / num8;
			float num10 = 1f - num9;
			float mountY = 0.5f * (_wheels[_idFrontLeft].TopPos.y + _wheels[_idRearLeft].TopPos.y);
			float num11 = 9.80665f * mass;
			float num12 = ResolveRestSpringDrop(_wheels[_idFrontLeft].MaxSpringLen, _wheels[_idFrontLeft].SpringCoef, num11 * num10 * 0.5f);
			float num13 = ResolveRestSpringDrop(_wheels[_idRearLeft].MaxSpringLen, _wheels[_idRearLeft].SpringCoef, num11 * num9 * 0.5f);
			float num14 = ResolveCogHeight(_body.LocalCenterMass.y, mountY, 0.5f * (num12 + num13), _wheels[_idFrontLeft].Radius);
			float num15;
			float muStatLong;
			float muStatLong2;
			switch (GearType)
			{
			case GearboxType.GearFwd:
			{
				muStatLong = _wheels[_idFrontLeft].TyreSimulation.GetMuStatLong(velocity, num5 * num10 * 0.5f, surfaceType);
				float num17 = Mathf.Min(0.5f, muStatLong * num14 / num8);
				num15 = muStatLong * a * num10 / (1f + num17);
				break;
			}
			case GearboxType.GearRwd:
			{
				muStatLong2 = _wheels[_idRearLeft].TyreSimulation.GetMuStatLong(velocity, num5 * num9 * 0.5f, surfaceType);
				float num16 = Mathf.Min(0.5f, muStatLong2 * num14 / num8);
				num15 = muStatLong2 * a * num9 / (1f - num16);
				break;
			}
			default:
				muStatLong = _wheels[_idFrontLeft].TyreSimulation.GetMuStatLong(velocity, num5 * num10 * 0.5f, surfaceType);
				muStatLong2 = _wheels[_idRearLeft].TyreSimulation.GetMuStatLong(velocity, num5 * num9 * 0.5f, surfaceType);
				num15 = (muStatLong * num10 + muStatLong2 * num9) * a;
				break;
			}
			airBrakeAccel = num2 / mass;
			tireDragAccel = num15 / mass;
			float b = 2f * FrontBrakeTorque / _wheels[_idFrontLeft].Radius;
			float b2 = 2f * RearBrakeTorque / _wheels[_idRearLeft].Radius;
			muStatLong = _wheels[_idFrontLeft].TyreSimulation.GetMuStatLong(velocity, num5 * num10 * 0.5f, surfaceType) * a * num10;
			muStatLong2 = _wheels[_idRearLeft].TyreSimulation.GetMuStatLong(velocity, num5 * num9 * 0.5f, surfaceType);
			float num18 = Mathf.Min(muStatLong, b);
			float num19 = Mathf.Min(muStatLong2 * a * num9, b2);
			tireBrakeAccel = (num18 + num19) / mass;
			brakeAccel = tireBrakeAccel + airBrakeAccel;
			float muStatLat = _wheels[_idFrontLeft].TyreSimulation.GetMuStatLat(velocity, num5 * num10 * 0.5f, surfaceType);
			float muStatLat2 = _wheels[_idRearLeft].TyreSimulation.GetMuStatLat(velocity, num5 * num9 * 0.5f, surfaceType);
			float num20 = muStatLat * a * num10;
			float num21 = muStatLat2 * a * num9;
			lateralAccel = Mathf.Min(1000f * num20 / (mass * num10), 1000f * num21 / (mass * num9));
			float torq = GearType switch
			{
				GearboxType.GearFwd => _wheels[_idFrontLeft].Radius, 
				GearboxType.GearRwd => _wheels[_idRearLeft].Radius, 
				_ => _wheels[_idFrontLeft].Radius, 
			};
			float num22 = torq;
			num22 *= 0.95f;
			float num23 = _gearRatio[currentGear + 1] * FinalDrive;
			float num24 = velocity / ((float)Math.PI / 30f / num23 * num22);
			if (currentGear == 1 && num24 < _rpmStandingStart.x)
			{
				num24 = _rpmStandingStart.x;
			}
			GetDynoGraphAtRpm(num24, out torq, out var _, out var _, out var whp);
			float num25 = whp * 735.4987f;
			float num26 = ((velocity > 1f) ? (num25 / velocity) : num25);
			engineDragAccel = num26 / mass;
		}

		public void CalcMaxPowerMaxTorque(out float maxHp, out float maxHpRpm, out float maxTq, out float maxTqRpm, out float maxWhp)
		{
			maxHp = -1f;
			maxTq = -1f;
			maxHpRpm = 0f;
			maxTqRpm = 0f;
			maxWhp = 0f;
			float torq;
			float wtorq;
			float hp;
			float whp;
			for (int i = 1; i <= 100; i++)
			{
				float num = RpmController.y * ((float)i / 100f);
				GetDynoGraphAtRpm(num, out torq, out wtorq, out hp, out whp);
				if (torq >= maxTq)
				{
					maxTq = torq;
					maxTqRpm = num;
				}
				if (hp >= maxHp)
				{
					maxHp = hp;
					maxHpRpm = num;
					maxWhp = whp;
				}
			}
			for (int j = 0; j < _torqueSamples.Length; j++)
			{
				GetDynoGraphAtRpm(_torqueSamples[j].Rpm, out torq, out wtorq, out hp, out whp);
				if (torq >= maxTq)
				{
					maxTq = torq;
					maxTqRpm = _torqueSamples[j].Rpm;
				}
				if (hp >= maxHp)
				{
					maxHp = hp;
					maxHpRpm = _torqueSamples[j].Rpm;
					maxWhp = whp;
				}
			}
		}

		private void GetWheelImpulse(float bTdt, float extTdt, float inertia, float angV, out float wheelImpulse)
		{
			float num = Mathf.Abs(bTdt);
			float num2 = Mathf.Clamp((0f - angV) * inertia - extTdt, 0f - num, num);
			wheelImpulse = num2 + extTdt;
		}

		private void GetDragImpulse(float clutchImpulse, float torqueImpulse, float diffImpulse, float engineInertia, float diffInertia, float w1, float w2, out float resT1dt, out float resT2dt)
		{
			GetDragImpulse(clutchImpulse, torqueImpulse, diffImpulse, engineInertia, diffInertia, w1, w2, _ratio, out resT1dt, out resT2dt);
		}

		private void GetDragImpulse(float clutchImpulse, float torqueImpulse, float diffImpulse, float engineInertia, float diffInertia, float w1, float w2, float ratio, out float resT1DT, out float resT2DT)
		{
			diffImpulse /= ratio;
			clutchImpulse = Mathf.Abs(clutchImpulse);
			float value = (engineInertia * diffInertia * (w2 - w1) + engineInertia * diffImpulse - diffInertia * torqueImpulse) / (engineInertia + diffInertia);
			value = Mathf.Clamp(value, 0f - clutchImpulse, clutchImpulse);
			resT1DT = value + torqueImpulse;
			resT2DT = 0f - value + diffImpulse;
			float num = 0.03f;
			if (w2 > 0f)
			{
				num = 0f - num;
			}
			float num2 = (torqueImpulse * (num - 1f) - diffInertia * w2 - diffImpulse * (1f + num) + engineInertia * w1 * (num - 1f)) / (engineInertia * (num - 1f) - diffInertia);
			value = engineInertia * (num2 - w1) - torqueImpulse;
			if (value + diffImpulse >= 0f)
			{
				value = Mathf.Clamp(value, 0f - clutchImpulse, clutchImpulse);
				resT1DT = torqueImpulse + value;
				resT2DT = diffImpulse - value + (diffImpulse + value) * num;
				return;
			}
			num2 = (torqueImpulse * (num + 1f) + diffInertia * w2 + diffImpulse * (1f - num) + engineInertia * w1 * (num + 1f)) / (engineInertia * (num + 1f) + diffInertia);
			value = engineInertia * (num2 - w1) - torqueImpulse;
			if (!(value + diffImpulse > 0f))
			{
				value = Mathf.Clamp(value, 0f - clutchImpulse, clutchImpulse);
				resT1DT = torqueImpulse + value;
				resT2DT = diffImpulse - value - (diffImpulse + value) * num;
			}
		}

		private void GetTcDragImpulse(float clutchImpulse, float torqueImpulse, float diffImpulse, float engineInertia, float diffInertia, float w1, float w2, float dt, out float resT1dt, out float resT2dt)
		{
			GetTcDragImpulse(clutchImpulse, torqueImpulse, diffImpulse, engineInertia, diffInertia, w1, w2, dt, _ratio, out resT1dt, out resT2dt);
		}

		private void GetTcDragImpulse(float clutchImpulse, float torqueImpulse, float diffImpulse, float engineInertia, float diffInertia, float w1, float w2, float dt, float ratio, out float resT1dt, out float resT2dt)
		{
			GetDragImpulse(clutchImpulse, torqueImpulse, diffImpulse, engineInertia, diffInertia, w1, w2, ratio, out var resT1DT, out var resT2DT);
			float num = w1 - w2;
			if (!AutoClutch || !At || _accelWant < 0.05f || num <= 0f || _gearTimer > 0f || (!Automatic && _vcLen > 5f) || PhysRuntimeFlags.DynorunMode)
			{
				resT1dt = resT1DT;
				resT2dt = resT2DT;
				return;
			}
			float num2 = Mathf.Abs(w2) * (30f / (float)Math.PI);
			float num3 = TcStallOmega * PhysMath.TallGearLaunchStallScale(ratio, _gearRatio[2] * FinalDrive, num2, TcLockupRpm, tcTallGearStallFloor);
			float num4 = Mathf.Max(num3 * num3, 1f);
			float num5 = _engineMaxTorque / num4 * w1 * w1 * dt;
			float a = torqueImpulse - num5;
			float num6 = Mathf.Max(w1, RpmController.x * 0.104719f * 0.5f);
			float num7 = Mathf.Clamp01(Mathf.Abs(w2) / num6) * Mathf.Clamp01(_vcLen);
			float num8 = Mathf.Clamp01((num7 - 0.6f) / 0.2f);
			float num9 = num8 * num8 * (3f - 2f * num8);
			if (w1 > num3 * 0.7f && _vcLen > 0.5f && !_tcSlipLatched)
			{
				float num10 = Mathf.Clamp01(num7 / 0.8f);
				num9 = Mathf.Max(num9, num10 * num10 * (3f - 2f * num10));
			}
			if (_accelWant > 0.5f && (_tcSlipLatched || num2 < RpmController.x + 150f))
			{
				num9 = 0f;
			}
			float num11 = 1f - Mathf.Clamp01(ClutchPedalOverride);
			resT1dt = Mathf.Lerp(torqueImpulse, Mathf.Lerp(a, resT1DT, num9), num11);
			float num12 = Mathf.Lerp(tcStallTorqueRatio, 1f, num7);
			float num13 = torqueImpulse * num12 * num11;
			float num14 = diffImpulse / _ratio;
			resT2dt = Mathf.Lerp(num13 + num14, resT2DT, num9);
		}

		private static float AxleSlipVelocity(Wheel left, Wheel right)
		{
			return (left.WheelTireVelocity + right.WheelTireVelocity) * 0.5f - (left.ForwardVelocity + right.ForwardVelocity) * 0.5f;
		}

		private void CalcDiffTorques(float tDo, float t1O, float t2O, float di, float wi, out float sumTd, out float sumT1, out float sumT2)
		{
			float num = (di * (t1O + t2O) * 0.5f - tDo * wi) / (di + 2f * wi);
			sumT1 = t1O - num;
			sumT2 = t2O - num;
			sumTd = 2f * num + tDo;
		}

		public float CalcRpmAtSpeedInLastGear(float speed)
		{
			if (speed > 0f)
			{
				return speed / (_wheels[_idFrontLeft].Radius * 2f * 0.1885f / (Mathf.Abs(_gearRatio[_numGears]) * FinalDrive));
			}
			return 0f;
		}

		private void CalcTransmission2Wd(float dt, float invDT, int idGearedLeft, int idGearedRight)
		{
			Wheel wheel = _wheels[idGearedLeft];
			Wheel wheel2 = _wheels[idGearedRight];
			LSDValues lSDValues = ((GearType == GearboxType.GearFwd) ? FrontLsd : RearLsd);
			LsdSimulator.UpdateLSD(Load, wheel.TransTorque + wheel2.TransTorque, wheel.AngularVelocity, wheel2.AngularVelocity, lSDValues, out var leftTd, out var rightTd);
			bool num = GearboxState == GearboxState.Burnout;
			float num2 = OpposingBrakeTorque(wheel, dt, invDT);
			if (num)
			{
				num2 *= 0.05f;
			}
			float num3 = num2 - wheel.LongtitudeFricForce * wheel.RadiusLoaded;
			float num4 = (0f - wheel.LongtitudeForce) * wheel.RadiusLoaded + wheel.TransTorque;
			GetWheelImpulse(num3 * dt, (leftTd + num4) * dt, wheel.Inertia, wheel.AngularVelocity, out var wheelImpulse);
			float num5 = OpposingBrakeTorque(wheel2, dt, invDT);
			if (num)
			{
				num5 *= 0.05f;
			}
			num3 = num5 - wheel2.LongtitudeFricForce * wheel2.RadiusLoaded;
			num4 = (0f - wheel2.LongtitudeForce) * wheel2.RadiusLoaded + wheel2.TransTorque;
			GetWheelImpulse(num3 * dt, (0f - rightTd + num4) * dt, wheel2.Inertia, wheel2.AngularVelocity, out var wheelImpulse2);
			wheelImpulse = wheelImpulse * invDT - wheel.TransTorque;
			wheelImpulse2 = wheelImpulse2 * invDT - wheel2.TransTorque;
			float num6 = wheelImpulse + wheelImpulse2;
			float num7 = _diffInertia + _gearBoxInertia + 2f * wheel.Inertia;
			Load = Mathf.Max(EngineCurTorque / (_engineCurTorqueRpm * 1.1f + 5f), 0f);
			bool flag = GearboxState == GearboxState.Burnout && !At;
			float resT2dt2;
			if (((_ratio == 0f && _ratioB == 0f) || _fixVehicle || IsActiveLaunchControlNow) | flag)
			{
				_clutchDragImpulse = 0f;
				_diffDragImpulse = 0f;
				_wEngine += EngineCurTorque * dt / _engineInertia;
				_wDiff += num6 / num7 * dt;
				if (IsActiveLaunchControlNow && !OverheatTransmission)
				{
					CurrentRpm = Mathf.Min(CurrentRpm, _lcController.x);
					wheel.Eps = Mathf.Min(EngineCurTorque, 1000f) / num7;
					wheel2.Eps = Mathf.Min(EngineCurTorque, 1000f) / num7;
				}
				else if (flag)
				{
					CurrentRpm = Mathf.Min(CurrentRpm, RpmController.y * 0.9f);
				}
			}
			else
			{
				EngineCurTorque *= 1f - TransmissionLossCoef;
				float num8 = (num6 - _wDiff * 1.5f * DiffLossCoef) * dt;
				float num9 = EngineCurTorque * dt;
				float num10 = 0f;
				float num11 = 0f;
				if (_ratio != 0f)
				{
					float w = _wDiff * _ratio;
					GetTcDragImpulse(GetClutchTorque * _tcMult * dt, num9, num8, _engineInertia, num7 / (_ratio * _ratio), _wEngine, w, dt, out var resT1dt, out var _);
					float num12 = resT1dt - num9;
					num10 += num12;
					num11 += (0f - num12) * _ratio;
				}
				if (_ratioB != 0f)
				{
					float w2 = _wDiff * _ratioB;
					GetTcDragImpulse(GetClutchTorqueB * _tcMult * dt, num9, num8, _engineInertia, num7 / (_ratioB * _ratioB), _wEngine, w2, dt, _ratioB, out var resT1dt2, out resT2dt2);
					float num13 = resT1dt2 - num9;
					num10 += num13;
					num11 += (0f - num13) * _ratioB;
				}
				_clutchDragImpulse = (num9 + num10) * invDT;
				float num14 = num8 + num11;
				_diffDragImpulse = num14 * invDT;
				_wEngine += _clutchDragImpulse * dt / _engineInertia;
				_wDiff += num14 / num7;
				Load = Mathf.Clamp01(Mathf.Max(_clutchDragImpulse, EngineCurTorque) / (_engineCurTorqueRpm * 1.1f + 5f));
			}
			float num15 = wheel.SumBrakeTorque + wheel2.SumBrakeTorque;
			if (num15 > 0f && _wDiff != 0f)
			{
				float f = (wheel.AngularVelocity + wheel2.AngularVelocity) * 0.5f;
				float b = num15 * dt / (num7 * 2f);
				float num16 = Mathf.Sign(_wDiff) * Mathf.Min(Mathf.Abs(_wDiff), Mathf.Abs(f));
				float f2 = _wDiff - num16;
				float num17 = Mathf.Sign(f2) * Mathf.Min(Mathf.Abs(f2), b);
				_wDiff -= num17;
			}
			if (flag)
			{
				float target = _wEngine / (_ratio + Mathf.Sign(_ratio) * Mathf.Epsilon);
				_wDiff = Mathf.MoveTowards(_wDiff, target, 200f * dt);
			}
			float tDo = _diffDragImpulse - num6;
			CalcDiffTorques(tDo, wheelImpulse, wheelImpulse2, num7, wheel.Inertia, out resT2dt2, out var sumT, out var sumT2);
			UpdateWheel(wheel, sumT, wheelImpulse, wheel.Inertia, dt);
			UpdateWheel(wheel2, sumT2, wheelImpulse2, wheel2.Inertia, dt);
			float num18 = _wDiff - (wheel.AngularVelocity + wheel2.AngularVelocity) * 0.5f;
			wheel.AngularVelocity += num18;
			wheel2.AngularVelocity += num18;
			ApplyLsdLockImplicit(wheel, wheel2, lSDValues, dt);
			ClampOpposingWheelSpin(wheel, wheel2);
		}

		private void ClampOpposingWheelSpin(Wheel left, Wheel right)
		{
			float angularVelocity = left.AngularVelocity;
			float angularVelocity2 = right.AngularVelocity;
			if (!(angularVelocity * angularVelocity2 >= 0f))
			{
				float f = angularVelocity - angularVelocity2;
				float num = Mathf.Abs(_wDiff) * 2f + 15f;
				if (!(Mathf.Abs(f) <= num))
				{
					float num2 = (Mathf.Abs(f) - num) * 0.5f;
					left.AngularVelocity = angularVelocity - Mathf.Sign(angularVelocity) * num2;
					right.AngularVelocity = angularVelocity2 - Mathf.Sign(angularVelocity2) * num2;
				}
			}
		}

		private static float OpposingBrakeTorque(Wheel wheel, float dt, float invDt)
		{
			float angularVelocity = wheel.AngularVelocity;
			float sumBrakeTorque = wheel.SumBrakeTorque;
			float num = (0f - wheel.LongtitudeForce) * wheel.RadiusLoaded;
			float num2 = Mathf.Abs(num);
			if (angularVelocity == 0f)
			{
				float f = num + wheel.TransTorque;
				float num3 = Mathf.Abs(f);
				if (sumBrakeTorque <= 0f || num3 <= 0.001f)
				{
					return 0f;
				}
				float num4 = Mathf.Min(sumBrakeTorque, num3);
				return (0f - Mathf.Sign(f)) * num4;
			}
			float b = wheel.Inertia * Mathf.Abs(angularVelocity) * invDt + num2;
			float num5 = Mathf.Min(sumBrakeTorque, b);
			if (!(angularVelocity > 0f))
			{
				return num5;
			}
			return 0f - num5;
		}

		private void CalcTransmissionLimits(float dt)
		{
			float num = Mathf.Max(RpmController.y + 500f, RpmController.z) * 0.104719f;
			float num2 = (IsEngineWorks ? (RpmController.x * 0.104719f) : 0f);
			if (!At && _transmissionProfile.Kind == GearboxKind.MT)
			{
				num2 = 0f;
			}
			_wEngine = Mathf.Clamp(_wEngine, 0f, num);
			if (num2 > 0f)
			{
				float stallFloorW = StallFloorRpm * 0.104719f;
				float maxHoldTorque = GetEngineMaxTorqueRpm(CurrentRpm) * 0.5f;
				_wEngine = IdleGovernor.IdleHoldStep(_wEngine, num2, stallFloorW, maxHoldTorque, _engineInertia, dt);
			}
			float num3 = (ParkingGear ? (RpmController.y * 0.6f) : RpmController.y);
			if (_wEngine * (30f / (float)Math.PI) >= num3)
			{
				float num4 = RevLimiterBounce.CeilingRpm(num3, LimiterBounceDepthRpm, LimiterFlutterHz, ref _limiterBouncePhase, dt);
				_wEngine = Mathf.Min(_wEngine, num4 * 0.104719f);
			}
			else
			{
				_limiterBouncePhase = 0f;
			}
			float num5 = num / Mathf.Abs(_ratio + Mathf.Epsilon);
			for (int i = 0; i < 8 && _wheels[i].Enabled; i++)
			{
				if (_ratio == 0f)
				{
					break;
				}
				if (_wheels[i].AngularVelocity > num5)
				{
					_wheels[i].AngularVelocity = num5;
				}
				if (_wheels[i].AngularVelocity < 0f - num5)
				{
					_wheels[i].AngularVelocity = 0f - num5;
				}
			}
		}

		private void CalcTransmissionNgeared(float dt, float invDt, int idNgeared)
		{
			Wheel wheel = _wheels[idNgeared];
			float num = OpposingBrakeTorque(wheel, dt, invDt) - wheel.LongtitudeFricForce * wheel.RadiusLoaded;
			float num2 = (0f - wheel.LongtitudeForce) * wheel.RadiusLoaded;
			GetWheelImpulse(num * dt, num2 * dt, wheel.Inertia, wheel.AngularVelocity, out var wheelImpulse);
			float num3 = (wheel.Eps = wheelImpulse / (wheel.Inertia * dt));
			wheel.AngularVelocity += num3 * dt;
		}

		private void CalcTransmission_Simple(float dt, float invDT)
		{
			if (TowingMode)
			{
				return;
			}
			bool num = GearType == GearboxType.Gear0Wd;
			bool flag = _idRear1Left != -1 && _idRear1Right != -1;
			bool flag2 = _idRear2Left != -1 && _idRear2Right != -1;
			if (num)
			{
				CalcTransmissionNgeared(dt, invDT, _idRearLeft);
				CalcTransmissionNgeared(dt, invDT, _idRearRight);
				if (flag)
				{
					CalcTransmissionNgeared(dt, invDT, _idRear1Left);
					CalcTransmissionNgeared(dt, invDT, _idRear1Right);
				}
				if (flag2)
				{
					CalcTransmissionNgeared(dt, invDT, _idRear2Left);
					CalcTransmissionNgeared(dt, invDT, _idRear2Right);
				}
				return;
			}
			int num2;
			int num3;
			int num4;
			int num5;
			if (GearType != GearboxType.GearRwd)
			{
				num2 = _idFrontLeft;
				num3 = _idFrontRight;
				num4 = _idRearLeft;
				num5 = _idRearRight;
			}
			else
			{
				num2 = _idRearLeft;
				num3 = _idRearRight;
				num4 = _idFrontLeft;
				num5 = _idFrontRight;
			}
			Wheel wheel = _wheels[num2];
			Wheel wheel2 = _wheels[num3];
			Wheel wheel3 = _wheels[num4];
			Wheel wheel4 = _wheels[num5];
			LSDValues lsd = ((GearType == GearboxType.GearFwd) ? FrontLsd : RearLsd);
			float num6 = OpposingBrakeTorque(wheel, dt, invDT);
			float num7 = OpposingBrakeTorque(wheel2, dt, invDT);
			float num8 = OpposingBrakeTorque(wheel3, dt, invDT);
			float num9 = OpposingBrakeTorque(wheel4, dt, invDT);
			float num10 = (0f - wheel.LongtitudeForce) * wheel.RadiusLoaded + wheel.TransTorque;
			GetWheelImpulse((num6 - wheel.LongtitudeFricForce * wheel.RadiusLoaded) * dt, num10 * dt, wheel.Inertia, wheel.AngularVelocity, out var wheelImpulse);
			num10 = (0f - wheel2.LongtitudeForce) * wheel2.RadiusLoaded + wheel2.TransTorque;
			GetWheelImpulse((num7 - wheel2.LongtitudeFricForce * wheel2.RadiusLoaded) * dt, num10 * dt, wheel2.Inertia, wheel2.AngularVelocity, out var wheelImpulse2);
			num10 = (0f - wheel3.LongtitudeForce) * wheel3.RadiusLoaded;
			GetWheelImpulse((num8 - wheel3.LongtitudeFricForce * wheel3.RadiusLoaded) * dt, num10 * dt, wheel3.Inertia, wheel3.AngularVelocity, out var wheelImpulse3);
			num10 = (0f - wheel4.LongtitudeForce) * wheel4.RadiusLoaded;
			GetWheelImpulse((num9 - wheel4.LongtitudeFricForce * wheel4.RadiusLoaded) * dt, num10 * dt, wheel4.Inertia, wheel4.AngularVelocity, out var wheelImpulse4);
			wheelImpulse3 *= invDT;
			wheelImpulse4 *= invDT;
			wheelImpulse *= invDT;
			wheelImpulse2 *= invDT;
			wheelImpulse -= wheel.TransTorque;
			wheelImpulse2 -= wheel2.TransTorque;
			float num11 = _diffInertia + _gearBoxInertia + 2f * wheel.Inertia;
			float num12 = wheelImpulse + wheelImpulse2;
			float resT2dt;
			if ((_ratio == 0f && _ratioB == 0f) || _fixVehicle || IsActiveLaunchControlNow)
			{
				_clutchDragImpulse = 0f;
				_diffDragImpulse = 0f;
				_wDiff += num12 / num11 * dt;
				_wEngine += EngineCurTorque * dt / _engineInertia;
			}
			else
			{
				float num13 = EngineCurTorque * dt;
				float num14 = num12 * dt;
				float num15 = 0f;
				float num16 = 0f;
				if (_ratio != 0f)
				{
					float w = _wDiff * _ratio;
					GetTcDragImpulse(GetClutchTorque * _tcMult * dt, num13, num14, _engineInertia, num11 / (_ratio * _ratio), _wEngine, w, dt, out var resT1dt, out resT2dt);
					float num17 = resT1dt - num13;
					num15 += num17;
					num16 += (0f - num17) * _ratio;
				}
				if (_ratioB != 0f)
				{
					float w2 = _wDiff * _ratioB;
					GetTcDragImpulse(GetClutchTorqueB * _tcMult * dt, num13, num14, _engineInertia, num11 / (_ratioB * _ratioB), _wEngine, w2, dt, _ratioB, out var resT1dt2, out resT2dt);
					float num18 = resT1dt2 - num13;
					num15 += num18;
					num16 += (0f - num18) * _ratioB;
				}
				_clutchDragImpulse = (num13 + num15) * invDT;
				float num19 = num14 + num16;
				_diffDragImpulse = num19 * invDT;
				_wDiff += num19 / num11;
				_wEngine += _clutchDragImpulse * dt / _engineInertia;
			}
			if (IsActiveLaunchControlNow && !OverheatTransmission)
			{
				CurrentRpm = Mathf.Min(CurrentRpm, _lcController.x);
				wheel.Eps = Mathf.Min(EngineCurTorque, 1000f) / wheel.Inertia;
				wheel.AngularVelocity = 0f;
				wheel.AngularVelocity0 = 0f;
				wheel2.Eps = Mathf.Min(EngineCurTorque, 1000f) / wheel2.Inertia;
				wheel2.AngularVelocity = 0f;
				wheel2.AngularVelocity0 = 0f;
				return;
			}
			float tDo = _diffDragImpulse - (wheelImpulse + wheelImpulse2);
			CalcDiffTorques(tDo, wheelImpulse, wheelImpulse2, num11, wheel.Inertia, out resT2dt, out var sumT, out var sumT2);
			wheel.TransTorque = sumT - wheelImpulse;
			wheel.Eps = sumT / wheel.Inertia;
			wheel.AngularVelocity += sumT / wheel.Inertia * dt;
			wheel2.TransTorque = sumT2 - wheelImpulse2;
			wheel2.Eps = sumT2 / wheel2.Inertia;
			wheel2.AngularVelocity += sumT2 / wheel2.Inertia * dt;
			float num20 = (wheel.AngularVelocity + wheel2.AngularVelocity) * 0.5f;
			float num21 = _wDiff - num20;
			wheel.AngularVelocity += num21;
			wheel2.AngularVelocity += num21;
			wheel3.Eps = wheelImpulse3 / wheel3.Inertia;
			wheel3.AngularVelocity += wheelImpulse3 / wheel3.Inertia * dt;
			wheel4.Eps = wheelImpulse4 / wheel4.Inertia;
			wheel4.AngularVelocity += wheelImpulse4 / wheel4.Inertia * dt;
			ApplyLsdLockImplicit(wheel, wheel2, lsd, dt);
			CalcTransmissionLimits(dt);
		}

		private void CalcTransmission_Advanced(float dt, float invDT)
		{
			if (TowingMode)
			{
				return;
			}
			GearboxType gearType = GearType;
			bool flag = gearType == GearboxType.GearFwd;
			bool flag2 = gearType == GearboxType.GearRwd;
			bool flag3 = gearType == GearboxType.Gear4Wd;
			bool flag4 = gearType == GearboxType.GearAwd;
			bool flag5 = gearType == GearboxType.Gear4Wdr1;
			bool flag6 = gearType == GearboxType.Gear4Wdr2;
			bool flag7 = gearType == GearboxType.Gear6Wd;
			bool flag8 = gearType == GearboxType.Gear6Wdr2;
			bool flag9 = gearType == GearboxType.Gear8Wd;
			bool num = gearType == GearboxType.Gear0Wd;
			bool flag10 = true;
			bool flag11 = true;
			bool flag12 = _idRear1Left != -1 && _idRear1Right != -1;
			bool flag13 = _idRear2Left != -1 && _idRear2Right != -1;
			if (flag12 && !_wheels[_idRear1Left].Enabled)
			{
				flag12 = false;
			}
			if (flag13 && !_wheels[_idRear2Left].Enabled)
			{
				flag13 = false;
			}
			if (num)
			{
				if (flag11)
				{
					CalcTransmissionNgeared(dt, invDT, _idRearLeft);
					CalcTransmissionNgeared(dt, invDT, _idRearRight);
				}
				if (flag12)
				{
					CalcTransmissionNgeared(dt, invDT, _idRear1Left);
					CalcTransmissionNgeared(dt, invDT, _idRear1Right);
				}
				if (flag13)
				{
					CalcTransmissionNgeared(dt, invDT, _idRear2Left);
					CalcTransmissionNgeared(dt, invDT, _idRear2Right);
				}
				return;
			}
			bool flag14 = false;
			bool flag15 = false;
			bool flag16 = false;
			bool flag17 = false;
			float currentRpm = CurrentRpm;
			_engineCurTorqueRpm = GetEngineMaxTorqueRpm(currentRpm);
			EngineCurTorque = GetEngineTorqueRpm(currentRpm, _engineCurTorqueRpm);
			if (flag & flag10)
			{
				flag14 = true;
				CalcTransmission2Wd(dt, invDT, _idFrontLeft, _idFrontRight);
			}
			if (flag2 & flag11)
			{
				flag15 = true;
				CalcTransmission2Wd(dt, invDT, _idRearLeft, _idRearRight);
			}
			if ((flag3 | flag4) & flag10 & flag11)
			{
				flag14 = true;
				flag15 = true;
				if (flag4)
				{
					CalcTransmissionAwd(dt, invDT, _idFrontLeft, _idFrontRight, _idRearLeft, _idRearRight);
				}
				else
				{
					CalcTransmission4Wd3D(dt, invDT, _idFrontLeft, _idFrontRight, _idRearLeft, _idRearRight);
				}
			}
			if (flag5 & flag11 & flag12)
			{
				flag15 = true;
				flag16 = true;
				CalcTransmission4Wd3D(dt, invDT, _idRearLeft, _idRearRight, _idRear1Left, _idRear1Right);
			}
			if (flag6 & flag12 & flag13)
			{
				flag16 = true;
				flag17 = true;
				CalcTransmission4Wd3D(dt, invDT, _idRear1Left, _idRear1Right, _idRear2Left, _idRear2Right);
			}
			if (flag7 & flag10 & flag11 & flag12)
			{
				_layout6Wd[0, 0] = _idFrontLeft;
				_layout6Wd[0, 1] = _idFrontRight;
				_layout6Wd[1, 0] = _idRearLeft;
				_layout6Wd[1, 1] = _idRearRight;
				_layout6Wd[2, 0] = _idRear1Left;
				_layout6Wd[2, 1] = _idRear1Right;
				flag14 = true;
				flag15 = true;
				flag16 = true;
				CalcTransmissionXwd(dt, invDT, _layout6Wd, 3);
			}
			if (flag8 & flag11 & flag12 & flag13)
			{
				_layout6Wdr2[0, 0] = _idRearLeft;
				_layout6Wdr2[0, 1] = _idRearRight;
				_layout6Wdr2[1, 0] = _idRear1Left;
				_layout6Wdr2[1, 1] = _idRear1Right;
				_layout6Wdr2[2, 0] = _idRear2Left;
				_layout6Wdr2[2, 1] = _idRear2Right;
				flag15 = true;
				flag16 = true;
				flag17 = true;
				CalcTransmissionXwd(dt, invDT, _layout6Wdr2, 3);
			}
			if (flag9 & flag10 & flag11 & flag12 & flag13)
			{
				_layout8Wd[0, 0] = _idFrontLeft;
				_layout8Wd[0, 1] = _idFrontRight;
				_layout8Wd[1, 0] = _idRearLeft;
				_layout8Wd[1, 1] = _idRearRight;
				_layout8Wd[2, 0] = _idRear1Left;
				_layout8Wd[2, 1] = _idRear1Right;
				_layout8Wd[3, 0] = _idRear2Left;
				_layout8Wd[3, 1] = _idRear2Right;
				flag14 = true;
				flag15 = true;
				flag16 = true;
				flag17 = true;
				CalcTransmissionXwd(dt, invDT, _layout8Wd, 4);
			}
			if (flag10 && !flag14)
			{
				CalcTransmissionNgeared(dt, invDT, _idFrontLeft);
				CalcTransmissionNgeared(dt, invDT, _idFrontRight);
			}
			if (flag11 && !flag15)
			{
				CalcTransmissionNgeared(dt, invDT, _idRearLeft);
				CalcTransmissionNgeared(dt, invDT, _idRearRight);
			}
			if (flag12 && !flag16)
			{
				CalcTransmissionNgeared(dt, invDT, _idRear1Left);
				CalcTransmissionNgeared(dt, invDT, _idRear1Right);
			}
			if (flag13 && !flag17)
			{
				CalcTransmissionNgeared(dt, invDT, _idRear2Left);
				CalcTransmissionNgeared(dt, invDT, _idRear2Right);
			}
			CalcTransmissionLimits(dt);
		}

		private void UpdateWheel(Wheel w, float torque, float wheelImpulse, float inertia, float dt)
		{
			w.TransTorque = Mathf.Clamp(torque - wheelImpulse, -800f, 800f);
			float angularVelocity = w.AngularVelocity;
			if (Mathf.Abs(angularVelocity) < 0.5f && Mathf.Abs(w.AngularVelocity0) < 2f && w.SumBrakeTorque >= Mathf.Abs(torque))
			{
				w.Eps = 0f;
				w.AngularVelocity = 0f;
				return;
			}
			float num = torque / inertia;
			if (!IsActiveLaunchControlNow)
			{
				w.Eps = num;
				w.AngularVelocity = angularVelocity + num * dt;
			}
		}

		private void CalcTransmissionAwd(float dt, float invDT, int idFrontLeft, int idFrontRight, int idRearLeft, int idRearRight)
		{
			Wheel wheel = _wheels[idFrontLeft];
			Wheel wheel2 = _wheels[idFrontRight];
			Wheel wheel3 = _wheels[idRearLeft];
			Wheel wheel4 = _wheels[idRearRight];
			bool flag = TorqueSplit > 0.5f;
			if (flag)
			{
				wheel = _wheels[idRearLeft];
				wheel2 = _wheels[idRearRight];
				wheel3 = _wheels[idFrontLeft];
				wheel4 = _wheels[idFrontRight];
			}
			float num = OpposingBrakeTorque(wheel, dt, invDT);
			float num2 = OpposingBrakeTorque(wheel2, dt, invDT);
			float num3 = OpposingBrakeTorque(wheel3, dt, invDT);
			float num4 = OpposingBrakeTorque(wheel4, dt, invDT);
			LSDValues lSDValues = (flag ? RearLsd : FrontLsd);
			LSDValues lSDValues2 = (flag ? FrontLsd : RearLsd);
			LsdSimulator.UpdateLSD(Load, wheel.TransTorque + wheel2.TransTorque, wheel.AngularVelocity, wheel2.AngularVelocity, lSDValues, out var leftTd, out var rightTd);
			LsdSimulator.UpdateLSD(Load, wheel3.TransTorque + wheel4.TransTorque, wheel3.AngularVelocity, wheel4.AngularVelocity, lSDValues2, out var leftTd2, out rightTd);
			float num5 = num - wheel.LongtitudeFricForce * wheel.RadiusLoaded;
			float num6 = num2 - wheel2.LongtitudeFricForce * wheel2.RadiusLoaded;
			float num7 = num3 - wheel3.LongtitudeFricForce * wheel3.RadiusLoaded;
			float num8 = num4 - wheel4.LongtitudeFricForce * wheel4.RadiusLoaded;
			float num9 = (0f - wheel.LongtitudeForce) * wheel.RadiusLoaded + wheel.TransTorque;
			float num10 = (0f - wheel2.LongtitudeForce) * wheel2.RadiusLoaded + wheel2.TransTorque;
			float num11 = (0f - wheel3.LongtitudeForce) * wheel3.RadiusLoaded + wheel3.TransTorque;
			float num12 = (0f - wheel4.LongtitudeForce) * wheel4.RadiusLoaded + wheel4.TransTorque;
			GetWheelImpulse(num5 * dt, (leftTd + num9) * dt, wheel.Inertia, wheel.AngularVelocity, out var wheelImpulse);
			GetWheelImpulse(num6 * dt, (0f - leftTd + num10) * dt, wheel2.Inertia, wheel2.AngularVelocity, out var wheelImpulse2);
			GetWheelImpulse(num7 * dt, (leftTd2 + num11) * dt, wheel3.Inertia, wheel3.AngularVelocity, out var wheelImpulse3);
			GetWheelImpulse(num8 * dt, (0f - leftTd2 + num12) * dt, wheel4.Inertia, wheel4.AngularVelocity, out var wheelImpulse4);
			wheelImpulse = wheelImpulse * invDT - wheel.TransTorque;
			wheelImpulse2 = wheelImpulse2 * invDT - wheel2.TransTorque;
			wheelImpulse3 = wheelImpulse3 * invDT - wheel3.TransTorque;
			wheelImpulse4 = wheelImpulse4 * invDT - wheel4.TransTorque;
			Load = Mathf.Max(EngineCurTorque / (_engineCurTorqueRpm * 1.1f + 5f), 0f);
			float inertia = wheel3.Inertia;
			float num13 = _diffInertia + _gearBoxInertia + 2f * inertia;
			float num14 = CenterDiffSimulator.FrontTorqueShare(flag ? (1f - TorqueSplit) : TorqueSplit, AxleSlipVelocity(wheel, wheel2), AxleSlipVelocity(wheel3, wheel4));
			float num15 = 0f;
			float num16 = (_wDiff - _wDiffF) * 200f;
			float num17 = ((!(_handbrakeVal > 0.05f)) ? num16 : (flag ? Mathf.Min(0f, num16) : Mathf.Max(0f, num16)));
			if ((_ratio == 0f && _ratioB == 0f) || _fixVehicle || IsActiveLaunchControlNow)
			{
				_clutchDragImpulse = 0f;
				_diffDragImpulse = 0f;
				_wDiff += (wheelImpulse3 + wheelImpulse4 - num17 - _wDiff * 1.5f * DiffLossCoef) / num13 * dt;
				_wDiffF += (wheelImpulse + wheelImpulse2 + num17 - _wDiffF * 1.5f * DiffLossCoef) / num13 * dt;
				_wEngine += EngineCurTorque * dt / _engineInertia;
				if (IsActiveLaunchControlNow && !OverheatTransmission)
				{
					CurrentRpm = Mathf.Min(CurrentRpm, _lcController.x);
					float eps = (wheel3.Eps = Mathf.Min(EngineCurTorque, 1000f) / num13);
					wheel4.Eps = eps;
				}
			}
			else
			{
				float num19 = (wheelImpulse3 + wheelImpulse4 - num17 - _wDiff * 1.5f * DiffLossCoef) * dt;
				float num20 = EngineCurTorque * (1f - TransmissionLossCoef) * dt;
				float num21 = 0f;
				float num22 = 0f;
				if (_ratio != 0f)
				{
					float w = _wDiff * _ratio;
					GetTcDragImpulse(GetClutchTorque * _tcMult * dt, num20, num19, _engineInertia, num13 / (_ratio * _ratio), _wEngine, w, dt, out var resT1dt, out rightTd);
					float num23 = resT1dt - num20;
					num21 += num23;
					num22 += (0f - num23) * _ratio;
				}
				if (_ratioB != 0f)
				{
					float w2 = _wDiff * _ratioB;
					GetTcDragImpulse(GetClutchTorqueB * _tcMult * dt, num20, num19, _engineInertia, num13 / (_ratioB * _ratioB), _wEngine, w2, dt, _ratioB, out var resT1dt2, out rightTd);
					float num24 = resT1dt2 - num20;
					num21 += num24;
					num22 += (0f - num24) * _ratioB;
				}
				_clutchDragImpulse = (num20 + num21) * invDT;
				float num25 = num19 + num22;
				_diffDragImpulse = num25 * invDT;
				_wEngine += _clutchDragImpulse * dt / _engineInertia;
				num15 = num22 * invDT;
				float num26 = num15 * num14;
				_wDiff += (_diffDragImpulse - num26) / num13 * dt;
				_wDiffF += (wheelImpulse + wheelImpulse2 + num17 + num26 - _wDiffF * 1.5f * DiffLossCoef) / num13 * dt;
				Load = Mathf.Clamp01(Mathf.Max(_clutchDragImpulse, EngineCurTorque) / (_engineCurTorqueRpm * 1.1f + 5f));
			}
			float num27 = wheel3.SumBrakeTorque + wheel4.SumBrakeTorque;
			if (num27 > 0f && _wDiff != 0f)
			{
				float f = (wheel3.AngularVelocity + wheel4.AngularVelocity) * 0.5f;
				float b = num27 * dt / (num13 * 2f);
				float num28 = Mathf.Sign(_wDiff) * Mathf.Min(Mathf.Abs(_wDiff), Mathf.Abs(f));
				float f2 = _wDiff - num28;
				float num29 = Mathf.Sign(f2) * Mathf.Min(Mathf.Abs(f2), b);
				_wDiff -= num29;
			}
			float num30 = wheel.SumBrakeTorque + wheel2.SumBrakeTorque;
			if (num30 > 0f && _wDiffF != 0f)
			{
				float f3 = (wheel.AngularVelocity + wheel2.AngularVelocity) * 0.5f;
				float b2 = num30 * dt / (num13 * 2f);
				float num31 = Mathf.Sign(_wDiffF) * Mathf.Min(Mathf.Abs(_wDiffF), Mathf.Abs(f3));
				float f4 = _wDiffF - num31;
				float num32 = Mathf.Sign(f4) * Mathf.Min(Mathf.Abs(f4), b2);
				_wDiffF -= num32;
			}
			CenterDiffSimulator.SplitDriveTorque(num15, num14, num17, out var frontAxleTorque, out var rearAxleTorque);
			if (_ratio == 0f && _ratioB == 0f)
			{
				rearAxleTorque = 0f;
			}
			CalcDiffTorques(frontAxleTorque, wheelImpulse, wheelImpulse2, num13, inertia, out rightTd, out var sumT, out var sumT2);
			CalcDiffTorques(rearAxleTorque, wheelImpulse3, wheelImpulse4, num13, inertia, out rightTd, out var sumT3, out var sumT4);
			UpdateWheel(wheel, sumT, wheelImpulse, wheel.Inertia, dt);
			UpdateWheel(wheel2, sumT2, wheelImpulse2, wheel2.Inertia, dt);
			UpdateWheel(wheel3, sumT3, wheelImpulse3, wheel3.Inertia, dt);
			UpdateWheel(wheel4, sumT4, wheelImpulse4, wheel4.Inertia, dt);
			if (_handbrakeVal <= 0.05f)
			{
				float num33 = _wDiff - _wDiffF;
				float num34 = CenterDiffSimulator.ContractedAxleDelta(num33, 1f, num13, dt);
				float num35 = (num33 - num34) * 0.5f;
				_wDiff -= num35;
				_wDiffF += num35;
			}
			float num36 = (wheel.AngularVelocity + wheel2.AngularVelocity) * 0.5f;
			float num37 = _wDiffF - num36;
			wheel.AngularVelocity += num37;
			wheel2.AngularVelocity += num37;
			float num38 = (wheel3.AngularVelocity + wheel4.AngularVelocity) * 0.5f;
			float num39 = _wDiff - num38;
			wheel3.AngularVelocity += num39;
			wheel4.AngularVelocity += num39;
			ApplyLsdLockImplicit(wheel, wheel2, lSDValues, dt);
			ApplyLsdLockImplicit(wheel3, wheel4, lSDValues2, dt);
		}

		private void CalcTransmission4Wd3D(float dt, float invDt, int idFrontLeft, int idFrontRight, int idRearLeft, int idRearRight)
		{
			Wheel wheel = _wheels[idFrontLeft];
			Wheel wheel2 = _wheels[idFrontRight];
			Wheel wheel3 = _wheels[idRearLeft];
			Wheel wheel4 = _wheels[idRearRight];
			LsdSimulator.UpdateLSD(Load, wheel.TransTorque + wheel2.TransTorque, wheel.AngularVelocity, wheel2.AngularVelocity, FrontLsd, out var leftTd, out var rightTd);
			LsdSimulator.UpdateLSD(Load, wheel3.TransTorque + wheel4.TransTorque, wheel3.AngularVelocity, wheel4.AngularVelocity, RearLsd, out var leftTd2, out rightTd);
			float num = OpposingBrakeTorque(wheel, dt, invDt);
			float num2 = OpposingBrakeTorque(wheel2, dt, invDt);
			float num3 = OpposingBrakeTorque(wheel3, dt, invDt);
			float num4 = OpposingBrakeTorque(wheel4, dt, invDt);
			float num5 = (0f - wheel.LongtitudeForce) * wheel.RadiusLoaded + wheel.TransTorque;
			float num6 = (0f - wheel2.LongtitudeForce) * wheel2.RadiusLoaded + wheel2.TransTorque;
			float num7 = (0f - wheel3.LongtitudeForce) * wheel3.RadiusLoaded + wheel3.TransTorque;
			float num8 = (0f - wheel4.LongtitudeForce) * wheel4.RadiusLoaded + wheel4.TransTorque;
			GetWheelImpulse((num - wheel.LongtitudeFricForce * wheel.RadiusLoaded) * dt, (leftTd + num5) * dt, wheel.Inertia, wheel.AngularVelocity, out var wheelImpulse);
			GetWheelImpulse((num2 - wheel2.LongtitudeFricForce * wheel2.RadiusLoaded) * dt, (0f - leftTd + num6) * dt, wheel2.Inertia, wheel2.AngularVelocity, out var wheelImpulse2);
			GetWheelImpulse((num3 - wheel3.LongtitudeFricForce * wheel3.RadiusLoaded) * dt, (leftTd2 + num7) * dt, wheel3.Inertia, wheel3.AngularVelocity, out var wheelImpulse3);
			GetWheelImpulse((num4 - wheel4.LongtitudeFricForce * wheel4.RadiusLoaded) * dt, (0f - leftTd2 + num8) * dt, wheel4.Inertia, wheel4.AngularVelocity, out var wheelImpulse4);
			wheelImpulse = wheelImpulse * invDt - wheel.TransTorque;
			wheelImpulse2 = wheelImpulse2 * invDt - wheel2.TransTorque;
			wheelImpulse3 = wheelImpulse3 * invDt - wheel3.TransTorque;
			wheelImpulse4 = wheelImpulse4 * invDt - wheel4.TransTorque;
			Load = Mathf.Max(EngineCurTorque / (_engineCurTorqueRpm * 1.1f + 5f), 0f);
			float num9 = (wheel.Inertia + wheel2.Inertia + wheel3.Inertia + wheel4.Inertia) * 0.25f;
			float num10 = _diffInertia + _gearBoxInertia + 2f * num9;
			float num11 = CenterDiffSimulator.CouplingTorque(CenterDiffOpen, _wDiff, _wDiffF);
			float num12 = ((_handbrakeVal > 0.05f) ? Mathf.Max(0f, num11) : num11);
			float num13 = CenterDiffSimulator.FrontTorqueShare(TorqueSplit, AxleSlipVelocity(wheel, wheel2), AxleSlipVelocity(wheel3, wheel4));
			float num14 = 0f;
			if ((_ratio == 0f && _ratioB == 0f) || _fixVehicle || IsActiveLaunchControlNow)
			{
				_clutchDragImpulse = 0f;
				_diffDragImpulse = 0f;
				_wEngine += EngineCurTorque * dt / _engineInertia;
				_wDiff += (wheelImpulse3 + wheelImpulse4 - num12 - _wDiff * 1.5f * DiffLossCoef) / num10 * dt;
				_wDiffF += (wheelImpulse + wheelImpulse2 + num12 - _wDiffF * 1.5f * DiffLossCoef) / num10 * dt;
				if (IsActiveLaunchControlNow && !OverheatTransmission)
				{
					CurrentRpm = Mathf.Min(CurrentRpm, _lcController.x);
					float eps = (wheel3.Eps = Mathf.Min(EngineCurTorque, 1000f) / num10);
					wheel4.Eps = eps;
				}
			}
			else
			{
				float num16 = (wheelImpulse3 + wheelImpulse4 - num12 - _wDiff * 1.5f * DiffLossCoef) * dt;
				float num17 = EngineCurTorque * (1f - TransmissionLossCoef) * dt;
				float num18 = 0f;
				float num19 = 0f;
				if (_ratio != 0f)
				{
					float w = _wDiff * _ratio;
					GetTcDragImpulse(GetClutchTorque * _tcMult * dt, num17, num16, _engineInertia, num10 / (_ratio * _ratio), _wEngine, w, dt, out var resT1dt, out rightTd);
					float num20 = resT1dt - num17;
					num18 += num20;
					num19 += (0f - num20) * _ratio;
				}
				if (_ratioB != 0f)
				{
					float w2 = _wDiff * _ratioB;
					GetTcDragImpulse(GetClutchTorqueB * _tcMult * dt, num17, num16, _engineInertia, num10 / (_ratioB * _ratioB), _wEngine, w2, dt, _ratioB, out var resT1dt2, out rightTd);
					float num21 = resT1dt2 - num17;
					num18 += num21;
					num19 += (0f - num21) * _ratioB;
				}
				_clutchDragImpulse = (num17 + num18) * invDt;
				float num22 = num16 + num19;
				_diffDragImpulse = num22 * invDt;
				_wEngine += _clutchDragImpulse * dt / _engineInertia;
				num14 = num19 * invDt;
				float num23 = num14 * num13;
				_wDiff += (_diffDragImpulse - num23) / num10 * dt;
				_wDiffF += (wheelImpulse + wheelImpulse2 + num12 + num23 - _wDiffF * 1.5f * DiffLossCoef) / num10 * dt;
				Load = Mathf.Clamp01(Mathf.Max(_clutchDragImpulse, EngineCurTorque) / (_engineCurTorqueRpm * 1.1f + 5f));
			}
			float num24 = wheel3.SumBrakeTorque + wheel4.SumBrakeTorque;
			if (num24 > 0f && _wDiff != 0f)
			{
				float f = (wheel3.AngularVelocity + wheel4.AngularVelocity) * 0.5f;
				float b = num24 * dt / (num10 * 2f);
				float num25 = Mathf.Sign(_wDiff) * Mathf.Min(Mathf.Abs(_wDiff), Mathf.Abs(f));
				float f2 = _wDiff - num25;
				float num26 = Mathf.Sign(f2) * Mathf.Min(Mathf.Abs(f2), b);
				_wDiff -= num26;
			}
			float num27 = wheel.SumBrakeTorque + wheel2.SumBrakeTorque;
			if (num27 > 0f && _wDiffF != 0f)
			{
				float f3 = (wheel.AngularVelocity + wheel2.AngularVelocity) * 0.5f;
				float b2 = num27 * dt / (num10 * 2f);
				float num28 = Mathf.Sign(_wDiffF) * Mathf.Min(Mathf.Abs(_wDiffF), Mathf.Abs(f3));
				float f4 = _wDiffF - num28;
				float num29 = Mathf.Sign(f4) * Mathf.Min(Mathf.Abs(f4), b2);
				_wDiffF -= num29;
			}
			CenterDiffSimulator.SplitDriveTorque(num14, num13, num12, out var frontAxleTorque, out var rearAxleTorque);
			if (_ratio == 0f && _ratioB == 0f)
			{
				rearAxleTorque = 0f;
				frontAxleTorque = num12;
			}
			CalcDiffTorques(frontAxleTorque, wheelImpulse, wheelImpulse2, num10, num9, out rightTd, out var sumT, out var sumT2);
			CalcDiffTorques(rearAxleTorque, wheelImpulse3, wheelImpulse4, num10, num9, out rightTd, out var sumT3, out var sumT4);
			UpdateWheel(wheel, sumT, wheelImpulse, wheel.Inertia, dt);
			UpdateWheel(wheel2, sumT2, wheelImpulse2, wheel2.Inertia, dt);
			UpdateWheel(wheel3, sumT3, wheelImpulse3, wheel3.Inertia, dt);
			UpdateWheel(wheel4, sumT4, wheelImpulse4, wheel4.Inertia, dt);
			if (!CenterDiffOpen && _handbrakeVal <= 0.05f)
			{
				float num30 = _wDiff - _wDiffF;
				float deadZone = CenterDiffSimulator.LockedCenterDeadZone(steerAngleRad: (wheel.SteerAngle + wheel2.SteerAngle) * 0.5f, rearAxleSpeed: _wDiff, frontAxleSpeed: _wDiffF);
				float num31 = CenterDiffSimulator.ContractedAxleDelta(num30, deadZone, num10, dt);
				float num32 = (num30 - num31) * 0.5f;
				_wDiff -= num32;
				_wDiffF += num32;
			}
			float num33 = (wheel.AngularVelocity + wheel2.AngularVelocity) * 0.5f;
			float num34 = _wDiffF - num33;
			wheel.AngularVelocity += num34;
			wheel2.AngularVelocity += num34;
			float num35 = (wheel3.AngularVelocity + wheel4.AngularVelocity) * 0.5f;
			float num36 = _wDiff - num35;
			wheel3.AngularVelocity += num36;
			wheel4.AngularVelocity += num36;
			ApplyLsdLockImplicit(wheel, wheel2, FrontLsd, dt);
			ApplyLsdLockImplicit(wheel3, wheel4, RearLsd, dt);
		}

		private void ApplyLsdLockImplicit(Wheel a, Wheel b, LSDValues lsd, float dt)
		{
			float num = a.AngularVelocity - b.AngularVelocity;
			float effInertia = (a.Inertia + b.Inertia) * 0.5f;
			float num2 = LsdSimulator.ContractedDiff(num, lsd, effInertia, dt);
			if (!Mathf.Approximately(num2, num))
			{
				float num3 = (num - num2) * 0.5f;
				a.AngularVelocity -= num3;
				b.AngularVelocity += num3;
			}
		}

		private void CalcTransmissionXwd(float dt, float invDT, int[,] parId, int nAxis)
		{
			float num = 0f;
			float num2 = 0f;
			float[][] array = new float[8][];
			for (int i = 0; i < 8; i++)
			{
				array[i] = new float[2];
			}
			float rightTd;
			for (int j = 0; j < nAxis; j++)
			{
				int num3 = parId[j, 0];
				int num4 = parId[j, 1];
				Wheel wheel = _wheels[num3];
				Wheel wheel2 = _wheels[num4];
				LSDValues lsdValues = AxleLsd(num3);
				LsdSimulator.UpdateLSD(Load, wheel.TransTorque + wheel2.TransTorque, wheel.AngularVelocity, wheel2.AngularVelocity, lsdValues, out var leftTd, out rightTd);
				float num5 = OpposingBrakeTorque(wheel, dt, invDT) - wheel.LongtitudeFricForce * wheel.RadiusLoaded;
				float num6 = (0f - wheel.LongtitudeForce) * wheel.RadiusLoaded + wheel.TransTorque + leftTd;
				GetWheelImpulse(num5 * dt, num6 * dt, wheel.Inertia, wheel.AngularVelocity, out var wheelImpulse);
				num5 = OpposingBrakeTorque(wheel2, dt, invDT) - wheel2.LongtitudeFricForce * wheel2.RadiusLoaded;
				float num7 = (0f - wheel2.LongtitudeForce) * wheel2.RadiusLoaded + wheel2.TransTorque - leftTd;
				GetWheelImpulse(num5 * dt, num7 * dt, wheel2.Inertia, wheel2.AngularVelocity, out var wheelImpulse2);
				wheelImpulse = wheelImpulse * invDT - wheel.TransTorque;
				wheelImpulse2 = wheelImpulse2 * invDT - wheel2.TransTorque;
				array[j][0] = wheelImpulse;
				array[j][1] = wheelImpulse2;
				num2 += wheelImpulse + wheelImpulse2;
				num += wheel.Inertia + wheel2.Inertia;
			}
			Load = Mathf.Max(EngineCurTorque / (_engineCurTorqueRpm * 1.1f + 5f), 0f);
			float num8 = _diffInertia + _gearBoxInertia + num;
			if ((_ratio == 0f && _ratioB == 0f) || _fixVehicle || IsActiveLaunchControlNow)
			{
				_wEngine += EngineCurTorque * dt / _engineInertia;
				_diffDragImpulse = 0f;
				_wDiff += num2 / num8 * dt;
				if (IsActiveLaunchControlNow && !OverheatTransmission)
				{
					CurrentRpm = Mathf.Min(CurrentRpm, _lcController.x);
				}
			}
			else
			{
				float num9 = EngineCurTorque * (1f - TransmissionLossCoef);
				float num10 = (num2 - _wDiff * 1.5f * DiffLossCoef) * dt;
				float num11 = num9 * dt;
				float num12 = 0f;
				float num13 = 0f;
				if (_ratio != 0f)
				{
					float w = _wDiff * _ratio;
					GetTcDragImpulse(GetClutchTorque * _tcMult * dt, num11, num10, _engineInertia, num8 / (_ratio * _ratio), _wEngine, w, dt, out var resT1dt, out rightTd);
					float num14 = resT1dt - num11;
					num12 += num14;
					num13 += (0f - num14) * _ratio;
				}
				if (_ratioB != 0f)
				{
					float w2 = _wDiff * _ratioB;
					GetTcDragImpulse(GetClutchTorqueB * _tcMult * dt, num11, num10, _engineInertia, num8 / (_ratioB * _ratioB), _wEngine, w2, dt, _ratioB, out var resT1dt2, out rightTd);
					float num15 = resT1dt2 - num11;
					num12 += num15;
					num13 += (0f - num15) * _ratioB;
				}
				_clutchDragImpulse = (num11 + num12) * invDT;
				float num16 = num10 + num13;
				_diffDragImpulse = num16 * invDT;
				_wDiff += num16 / num8;
				_wEngine += _clutchDragImpulse * dt / _engineInertia;
				Load = Mathf.Clamp01(Mathf.Max(_clutchDragImpulse, EngineCurTorque) / (_engineCurTorqueRpm * 1.1f + 5f));
			}
			float num17 = _diffDragImpulse - num2;
			float num18 = 0f;
			for (int k = 0; k < nAxis; k++)
			{
				int num19 = parId[k, 0];
				int num20 = parId[k, 1];
				Wheel wheel3 = _wheels[num19];
				Wheel wheel4 = _wheels[num20];
				float num21 = array[k][0];
				float num22 = array[k][1];
				CalcDiffTorques(num17 / (float)nAxis, num21, num22, num8, wheel3.Inertia, out rightTd, out var sumT, out var sumT2);
				UpdateWheel(wheel3, sumT, num21, wheel3.Inertia, dt);
				UpdateWheel(wheel4, sumT2, num22, wheel3.Inertia, dt);
				num18 += wheel3.AngularVelocity + wheel4.AngularVelocity;
			}
			float num23 = num18 / ((float)nAxis * 2f);
			float num24 = _wDiff - num23;
			for (int l = 0; l < nAxis; l++)
			{
				int num25 = parId[l, 0];
				int num26 = parId[l, 1];
				_wheels[num25].AngularVelocity += num24;
				_wheels[num26].AngularVelocity += num24;
			}
			for (int m = 0; m < nAxis; m++)
			{
				ApplyLsdLockImplicit(_wheels[parId[m, 0]], _wheels[parId[m, 1]], AxleLsd(parId[m, 0]), dt);
			}
		}

		private LSDValues AxleLsd(int leftWheelId)
		{
			if (leftWheelId != _idFrontLeft)
			{
				return RearLsd;
			}
			return FrontLsd;
		}

		private float GetClutchTorqueFor(float clutchValue)
		{
			return ClutchLimit * GetClutchMap(clutchValue) * (1f - Mathf.Clamp01(ClutchPedalOverride));
		}

		private void WaterTempSimulation(float hz)
		{
			if (AiMode)
			{
				return;
			}
			float num = _radiatorCooling * (1f + _vcLen * 0.015f);
			if (IsEngineWorks)
			{
				if (WaterTemp > _thermostatOpenTemp)
				{
					_isThermostatOpen = true;
				}
				if (WaterTemp > _fanTempOn)
				{
					_isFanWorking = true;
				}
				WaterTemp += _engineSpeedHeat * CurrentRpm * hz;
				WaterTemp -= num * hz;
				if (_isThermostatOpen)
				{
					WaterTemp -= 3.2f * _radiatorCooling * hz;
				}
				if (_isFanWorking)
				{
					WaterTemp -= 4.2f * _radiatorCooling * hz;
				}
				if (WaterTemp < _fanTempOff && _isFanWorking)
				{
					_isFanWorking = false;
				}
				if (WaterTemp < _thermostatCloseTemp && _isThermostatOpen)
				{
					_isThermostatOpen = false;
				}
			}
			else
			{
				WaterTemp -= 0.025f * hz;
			}
			WaterTemp = Mathf.Clamp(WaterTemp, 60f, 150f);
		}

		private void LaunchControlSimulation(float dt)
		{
			if (!_hasLaunchControl || !AutoClutch)
			{
				return;
			}
			if (!_lcCooldown && _lcController.z >= _lcController.y && GearboxState == GearboxState.Launch)
			{
				_lcCooldown = true;
				IsActiveLaunchControlNow = false;
			}
			if (_lcCooldown)
			{
				IsActiveLaunchControlNow = false;
				_lcController.z -= dt;
				if (_lcController.z <= 0f)
				{
					_lcController.z = 0f;
					_lcCooldown = false;
				}
				_lcController.z = Mathf.Clamp(_lcController.z, 0f, _lcController.y + 1f);
				return;
			}
			if (!IsActiveLaunchControlNow && GearboxState == GearboxState.Launch && CurrentGear == 2 && !OverheatTransmission)
			{
				IsActiveLaunchControlNow = true;
			}
			if (GearboxState != GearboxState.Launch && !IsEngineWorks)
			{
				_lcController.z -= dt * 0.1f;
				_lcController.z = Mathf.Clamp(_lcController.z, 0f, _lcController.y + 1f);
			}
			if (IsActiveLaunchControlNow)
			{
				_lcController.z += dt;
			}
			else
			{
				_lcController.z -= dt * 0.1f;
			}
			_lcController.z = Mathf.Clamp(_lcController.z, 0f, _lcController.y + 1f);
		}

		private void FuelSimulation(float dt)
		{
			if (AiMode)
			{
				return;
			}
			RpmAtSpeedInLastGear = CalcRpmAtSpeedInLastGear(_fuelConsumptionSpeed);
			_secondsToCover100Km = 100f / _fuelConsumptionSpeed * 3600f;
			float num = Mathf.Abs(_vcLen);
			float a = num * 3.6f;
			float b = ((_ratio == 0f) ? 0f : 50f);
			float num2 = 0.1f + Mathf.Max(a, b) / _fuelConsumptionSpeed;
			if (CurrentFuel > 0f)
			{
				if (CurrentRpm >= _minStarterRpm)
				{
					if (RpmAtSpeedInLastGear != 0f)
					{
						IstantConsumption = CurrentRpm * (0.1f + Load * 0.9f) * FuelConsumptionAtCostantSpeed / (RpmAtSpeedInLastGear * _secondsToCover100Km) * num2;
					}
					CurrentFuel -= IstantConsumption * dt;
					CurrentFuel = Mathf.Max(CurrentFuel, 0f);
					CurrentConsumption = ((num > 0.5f) ? (IstantConsumption / num * 100000f) : 0f);
				}
			}
			else
			{
				EmptyFuelTank();
			}
		}

		private void EspSimulation(bool enable, float dt)
		{
			_espFuelCut = 0f;
			for (int i = 0; i < 8; i++)
			{
				if (_wheels[i] != null)
				{
					_wheels[i].EspBraking = false;
				}
			}
			float num = Mathf.Abs(_vcLen);
			Vector3 velocity = _body.Velocity;
			float forwardSpeed = Vector3.Dot(PhysMath.GetForward(_body.Matrix), velocity);
			float lateralSpeed = Vector3.Dot(PhysMath.GetRight(_body.Matrix), velocity);
			float num2 = PhysMath.EspSideslipDegrees(forwardSpeed, lateralSpeed);
			float sideslipRateDegPerSec = ((dt > 0f) ? ((num2 - _espPrevSideslip) / dt) : 0f);
			_espPrevSideslip = num2;
			if (!enable || num * 3.6f <= _espMinVelocity)
			{
				return;
			}
			float t = (float)(EspLevel - 1) / 9f;
			float y = _body.AngularVelocity.y;
			float num3 = ((_maxSideAccel > 0f) ? (_maxSideAccel / 1000f) : 9.80665f);
			float referenceYawRate = PhysMath.EspReferenceYawRate(num, 0f - _steerAng, _baseWheel, num3);
			float deadZone = Mathf.Lerp(0.21f, 0.07f, t);
			PhysMath.EspCorrection espCorrection = PhysMath.EspCorrectionFor(y, referenceYawRate, deadZone, 0.44f);
			if (!espCorrection.Active)
			{
				return;
			}
			float num4 = Mathf.Abs(SideAccel);
			bool flag = PhysMath.EspBodyIsSliding(num2, sideslipRateDegPerSec);
			bool flag2 = num4 >= num3 * 0.8f;
			if (espCorrection.RearAxle)
			{
				if (!flag2 || Mathf.Abs(num2) > 15f)
				{
					return;
				}
			}
			else if (!flag)
			{
				return;
			}
			int num5 = ((!espCorrection.RearAxle) ? (espCorrection.RightSide ? _idFrontRight : _idFrontLeft) : (espCorrection.RightSide ? _idRearRight : _idRearLeft));
			if (num5 == -1 || _wheels[num5] == null)
			{
				return;
			}
			ESPtrigger = true;
			float num6 = (espCorrection.RearAxle ? Mathf.Lerp(0.4f, 0.85f, t) : Mathf.Lerp(0.25f, 0.95f, t));
			_espFuelCut = num6 * espCorrection.Intensity01;
			float num7 = PhysMath.EspBrakeTorqueLimit(_wheels[num5].NormalForce, _wheels[num5].RadiusLoaded, num3);
			float num9;
			if (espCorrection.RearAxle)
			{
				float num8 = Mathf.Clamp01(1f - num4 / Mathf.Max(num3, 0.1f));
				num9 = num7 * Mathf.Lerp(0.04f, 0.12f, t) * espCorrection.Intensity01 * num8;
			}
			else
			{
				num9 = num7 * Mathf.Lerp(0.15f, 0.85f, t) * espCorrection.Intensity01;
			}
			float num10 = (espCorrection.RearAxle ? (_rearBrake * BrakeRearTorque) : (_frontBrake * BrakeFrontTorque));
			_wheels[num5].InputBrakeTorque = Mathf.Max(num10, num9);
			_wheels[num5].EspBraking = num9 > num10;
			float num11 = 0f;
			if (!espCorrection.RearAxle)
			{
				int num12 = (espCorrection.RightSide ? _idRearRight : _idRearLeft);
				float b = ((num12 != -1 && _wheels[num12] != null) ? PhysMath.EspSpinBrakeShare(WheelOverspeed(_wheels[num12])) : 0f);
				num11 = Mathf.Max(PhysMath.EspSecondWheelShare(num2) * espCorrection.Intensity01, b);
			}
			if (num11 > 0f)
			{
				int num13 = (espCorrection.RightSide ? _idRearRight : _idRearLeft);
				if (num13 != -1 && _wheels[num13] != null)
				{
					float num14 = PhysMath.EspBrakeTorqueLimit(_wheels[num13].NormalForce, _wheels[num13].RadiusLoaded, num3) * Mathf.Lerp(0.15f, 0.85f, t) * num11;
					float num15 = _rearBrake * BrakeRearTorque;
					_wheels[num13].InputBrakeTorque = Mathf.Max(num15, num14);
					_wheels[num13].EspBraking = num14 > num15;
				}
			}
		}

		private void UpdateStaticSag()
		{
			float num = Vector3.Dot(Vector3.forward, _wheels[_idFrontLeft].TopPos - _body.LocalCenterMass);
			float num2 = Vector3.Dot(Vector3.forward, _body.LocalCenterMass - _wheels[_idRearLeft].TopPos);
			float num3 = num + num2;
			if (num3 <= 1E-06f)
			{
				return;
			}
			float num4 = Mathf.Clamp01(num2 / num3);
			float num5 = 9.80665f * _body.Mass;
			for (int i = 0; i < 8; i++)
			{
				Wheel wheel = _wheels[i];
				if (wheel != null && wheel.Enabled && !(wheel.SpringCoef <= 0f))
				{
					float num6 = (wheel.IsFront ? (1f - num4) : num4);
					wheel.StaticSag = num5 * num6 * 0.5f / wheel.SpringCoef;
				}
			}
		}

		public void AdvanceWheelRollAnglesWhileAsleep(float dt)
		{
			for (int i = 0; i < 8; i++)
			{
				Wheel wheel = _wheels[i];
				if (wheel != null && wheel.Enabled)
				{
					wheel.UpdateRollAngle(dt);
				}
			}
		}

		public void ApplyWheels(float dt, int steps)
		{
			float num = dt / (float)steps;
			float invDT = Mathf.Max(1f / num, 1E-06f);
			ESPtrigger = false;
			EspSimulation(Esp, dt);
			_trcIndicatorHold -= dt;
			if (_trcIndicatorHold <= 0f)
			{
				TrcTrigger = false;
			}
			_trcFueltimer = 0f;
			bool flag = false;
			UpdateStaticSag();
			_ = CurrentRpm;
			for (int i = 0; i < steps; i++)
			{
				UpdateStabilizers();
				for (int j = 0; j < 8; j++)
				{
					if (!_wheels[j].Enabled)
					{
						continue;
					}
					if (!OnLineCar)
					{
						_wheels[j].IsTouch = Touch[j];
						flag |= _wheels[j].UpdateAbs();
						_wheels[j].CalculateSuspensionForce(num);
						if (_wheels[j].IsTouch)
						{
							NumSum[j]++;
							if (SimpleMode || TowingMode || ArcadeMode)
							{
								_wheels[j].CalcFrictionArcade(num);
							}
							else
							{
								_wheels[j].UpdateTyreWear(num);
								_wheels[j].CalculateFriction(num);
								_wheels[j].CalcFrictionDigger(num);
							}
						}
						if (!SimpleMode)
						{
							_wheels[j].CalcSound(num);
						}
					}
					_wheels[j].UpdateRollAngle(num);
				}
				if (!OnLineCar)
				{
					_ratio = (float.IsNaN(_ratioOverride) ? (_gearRatio[CurrentGear] * FinalDrive) : _ratioOverride);
					if (SimpleMode)
					{
						CalcTransmission_Simple(num, invDT);
					}
					else
					{
						CalcTransmission_Advanced(num, invDT);
					}
					_body.AddExtForce(_mgdt);
					_body.AddExtForce(_airSumdt);
					_body.AddExtTorque(_airSumTdt);
					if ((_accelWant > 0.5f && CurrentGear == 1 && CurrentRpm < RpmController.y - 200f) || (!AutoClutch && (double)Clutch > 0.5 && CurrentRpm < RpmController.y - 200f && !TowingMode) || (ClutchPedalOverride > 0.5f && _accelWant > 0.5f && CurrentRpm < RpmController.y - 200f))
					{
						Vector3 vector = _body.Matrix.MultiplyVector(_engineOrientation);
						float num2 = Mathf.Min(Mathf.Abs(EngineCurTorque * 0.003f), 5f) * Mathf.Sign(EngineCurTorque);
						_body.AddExtTorque(-vector * num2);
					}
					if (IsActiveLaunchControlNow && _accelWant > 0.5f && CurrentRpm < RpmController.y - 200f)
					{
						float num3 = Mathf.Min(Mathf.Abs(EngineCurTorque * 0.003f), 5f) * Mathf.Sign(EngineCurTorque);
						Vector3 vector2 = _body.Matrix.MultiplyVector(Vector3.right);
						_body.AddExtTorque(-vector2 * num3);
					}
				}
				else
				{
					float engineMaxTorqueRpm = GetEngineMaxTorqueRpm(CurrentRpm);
					EngineCurTorque = GetEngineTorqueRpm(CurrentRpm, engineMaxTorqueRpm);
					Load = Mathf.Max(EngineCurTorque / (engineMaxTorqueRpm * 1.1f + 5f), 0f);
					_body.AddExtForce(_mgdt);
				}
				_body.ProcessExternalForces();
				if (!OnLineCar && !TowingMode)
				{
					FuelSimulation(num);
					WaterTempSimulation(num);
					TractionControlSimulator();
					AtClutchSimulation(num);
					FuelFlowSimulation(num);
				}
			}
			if (!OnLineCar)
			{
				AbsTrigger = flag;
			}
		}

		private void FuelFlowSimulation(float subDelta)
		{
			float num = (ParkingGear ? (RpmController.y * 0.6f) : RpmController.y);
			bool flag = _accelWant < 0.95f;
			if (CurrentRpm >= num)
			{
				_limiterArmed = true;
			}
			else if ((CurrentRpm < num - 600f) | flag)
			{
				_limiterArmed = false;
			}
			float b = 0f;
			if (_limiterArmed && LimiterFlutterHz > 0f)
			{
				_limiterPhase += subDelta * LimiterFlutterHz;
				_limiterPhase -= Mathf.Floor(_limiterPhase);
				bool flag2 = _limiterPhase < 0.2f;
				b = (flag2 ? 1f : 0f);
				RevLimiterTriggered = flag2;
			}
			else
			{
				_limiterPhase = 0f;
				RevLimiterTriggered = false;
			}
			_engineFuelFlow = 1f;
			float num2 = _accelWant * _throttleOverride;
			if (_lcCooldown && GearboxState == GearboxState.Launch)
			{
				num2 = 0f;
			}
			float num3 = num2;
			float num4 = Mathf.Max(Mathf.Max(Mathf.Min(_trcFueltimer, 1f), b), _espFuelCut);
			if (num4 > 0f)
			{
				_engineFuelFlow = 1f - num4;
				num3 = num2 * _engineFuelFlow;
			}
			if (_accel >= num3)
			{
				_accel = num3;
				return;
			}
			_accel += 25f * subDelta;
			if (_accel > num3)
			{
				_accel = num3;
			}
		}

		private static float WheelOverspeed(Wheel wheel)
		{
			return Mathf.Abs(wheel.AngularVelocity) - Mathf.Abs(wheel.AngularVelocity0);
		}

		private static float DriveSlipOnly(float slip)
		{
			return Mathf.Max(slip, 0f);
		}

		private void TractionControlSimulator()
		{
			if (!TractionControl || !(CurrentRpm > _rpmStandingStart.y + 150f) || !(Throttle > 0f))
			{
				return;
			}
			float slip = 0f;
			float slip2 = 0f;
			bool flag = GearboxType.GearAwd == GearType;
			if (((GearType == GearboxType.GearRwd) | flag) || GearType == GearboxType.Gear4Wd)
			{
				slip = _wheels[_idRearLeft].LongitudinalSlip;
				slip2 = _wheels[_idRearRight].LongitudinalSlip;
				if (_vcLen < 10f)
				{
					slip = WheelOverspeed(_wheels[_idRearLeft]);
					slip2 = WheelOverspeed(_wheels[_idRearRight]);
				}
				if (!_wheels[_idRearLeft].IsTouch)
				{
					slip = 0f;
				}
				if (!_wheels[_idRearRight].IsTouch)
				{
					slip2 = 0f;
				}
			}
			else if (GearType == GearboxType.GearFwd)
			{
				slip = _wheels[_idFrontLeft].LongitudinalSlip;
				slip2 = _wheels[_idFrontRight].LongitudinalSlip;
				if (_vcLen < 10f)
				{
					slip = WheelOverspeed(_wheels[_idFrontLeft]);
					slip2 = WheelOverspeed(_wheels[_idFrontRight]);
				}
				if (!_wheels[_idFrontLeft].IsTouch)
				{
					slip = 0f;
				}
				if (!_wheels[_idFrontRight].IsTouch)
				{
					slip2 = 0f;
				}
			}
			slip = DriveSlipOnly(slip);
			slip2 = DriveSlipOnly(slip2);
			float num = Mathf.Max(slip, slip2);
			num = ((_vcLen >= 10f) ? (num - 10f / _vcLen) : (num - 1f));
			float t = (float)(TrcLevel - 1) / 9f;
			float num2 = Mathf.Lerp(3.5f, 1.2f, t);
			float b = Mathf.Lerp(0.2f, 0.8f, t);
			if (num > num2)
			{
				TrcTrigger = true;
				_trcIndicatorHold = 0.3f;
				_trcFueltimer = Mathf.Min(num - num2, 2f);
				_trcFueltimer = Mathf.Min(_trcFueltimer * 0.5f, b);
			}
		}

		private void AtClutchSimulation(float subDelta)
		{
			_revMatchBlip = 0f;
			_behavior.Tick(this, subDelta);
		}

		public void PostUpdate(float dt)
		{
			if (OnLineCar)
			{
				return;
			}
			float num = 1f / dt;
			if (PositionIntegration)
			{
				if (_useAlignPlane)
				{
					_body.AlignForces(_rightPlane);
					_body.AlignVelocities(_rightPlane);
				}
				_body.UpdatePosition(dt);
				if (_useAlignPlane)
				{
					_body.AlignMatrix(_rightPlane);
				}
			}
			_body.CalcAccel(num);
			for (int i = 0; i < 8; i++)
			{
				if (_wheels[i].Enabled && NumSum[i] != 0)
				{
					float num2 = 1f / (float)NumSum[i];
					_wheels[i].LatForce *= num2;
					_wheels[i].LongForce *= num2;
					_wheels[i].VertForce *= num2;
					_wheels[i].AlignMom *= num2;
					_wheels[i].CurrentCamber *= num2;
				}
			}
			if (Trailer)
			{
				float num3 = Vector3.Dot(_body.AngularVelocity, Vector3.up);
				AngAccel = (num3 - _wOld) * num;
				_wOld = num3;
				return;
			}
			_gearTimer -= dt;
			_gearTimerClutch -= dt;
			_gearTimer = Mathf.Max(_gearTimer, 0f);
			_gearTimerClutch = Mathf.Max(_gearTimerClutch, 0f);
			if (PhysRuntimeFlags.DynorunMode && _accelWant < 0.05f)
			{
				float num4 = 1f - 0.8f * dt;
				_wDiff *= num4;
				_wDiffF *= num4;
				_wDiffR *= num4;
				if (Mathf.Abs(_wDiff) < 2f)
				{
					_wDiff = 0f;
				}
				if (Mathf.Abs(_wDiffF) < 2f)
				{
					_wDiffF = 0f;
				}
				if (Mathf.Abs(_wDiffR) < 2f)
				{
					_wDiffR = 0f;
				}
			}
			if ((Automatic || AiMode) && AtTransmission.AppliesStandstillGoverning(_transmissionProfile.Kind) && CurrentGear != 1 && !PhysRuntimeFlags.DynorunMode)
			{
				int num5 = AtTransmission.StandstillGear(_lowMode, _lowMaxGear);
				if (Mathf.Abs(_vcLen) < 0.5f && CurrentGear > num5 + 1 && _gearTimer == 0f && _gearTimerClutch == 0f)
				{
					_waitShift = 0f;
					Gear = num5;
				}
				_waitShift -= dt;
				_waitShift = Mathf.Max(0f, _waitShift);
				bool flag = AtTransmission.LimiterCancelsShiftCooldown(CurrentRpm, RpmController.x, RpmController.y, _accelWant);
				if ((_gearTimer == 0f && _gearTimerClutch == 0f) & flag)
				{
					_waitShift = 0f;
				}
				if (_prevAccelWant > _accelWant + 0.05f)
				{
					_throttleDropTimer = 0.7f;
				}
				_throttleDropTimer = Mathf.Max(0f, _throttleDropTimer - dt);
				float num6 = RpmController.y - RpmController.x;
				if (CurrentRpm >= RpmController.y - num6 * 0.1f)
				{
					_engineRevLimitedSustained += dt;
				}
				else
				{
					_engineRevLimitedSustained = Mathf.Max(0f, _engineRevLimitedSustained - dt * 2f);
				}
				if (_accelWant >= _throttleRecentMax - 0.05f)
				{
					if (_accelWant > _throttleRecentMax)
					{
						_throttleRecentMax = _accelWant;
					}
					_throttleRecentMaxAge = 0f;
				}
				else
				{
					_throttleRecentMaxAge += dt;
					if (_throttleRecentMaxAge > 3f)
					{
						_throttleRecentMax = _accelWant;
						_throttleRecentMaxAge = 0f;
					}
				}
				if (_accelWant >= 0.85f && _prevAccelWant < 0.85f && _gearTimer == 0f)
				{
					_waitShift = 0f;
				}
				_prevAccelWant = _accelWant;
				if (_waitShift == 0f)
				{
					if (SimpleMode)
					{
						if (CurrentRpm > _engineGearUpRpm && CurrentGear > 1 && Clutch < 0.7f && _gearTimer == 0f && _gearTimerClutch == 0f)
						{
							_lastShiftWasUp = true;
							GearUp();
							_waitShift = 1f;
						}
						else if (CurrentRpm < _engineGearDnRpm && CurrentGear > 2 && _gearTimer == 0f && _gearTimerClutch == 0f)
						{
							_lastShiftWasUp = false;
							GearDown();
							_waitShift = 1f;
						}
					}
					else
					{
						_behavior.WantsShift(this, out var gear);
						bool flag2 = gear < Gear;
						if (Gear != gear && Gear > 0 && _gearTimer == 0f && _gearTimerClutch == 0f && ((Clutch < 0.7f) | flag2))
						{
							bool flag3 = (_lastShiftWasUp = gear > Gear);
							bool flag4 = !flag3 && _accelWant > 0.5f;
							bool flag5 = !flag3 && _accelWant < 0.05f;
							_waitShift = ((_transmissionProfile.Kind == GearboxKind.AMT) ? 0f : ((!flag3) ? (flag4 ? 2.5f : (flag5 ? 0.3f : 1.2f)) : ((_accelWant >= 0.85f) ? 1f : 1.2f)));
							Gear = gear;
						}
					}
				}
			}
			IdleSimulation();
			if (!AiMode)
			{
				if (!ManualHelper && CurrentRpm < _minStarterRpm && IsEngineWorks && Starter < 0.1f && !PhysRuntimeFlags.IsCasualControl)
				{
					ShutdownEngine();
				}
				if (_cutOut)
				{
					if (Starter <= 0.1f)
					{
						_starterTime = 0f;
					}
					else
					{
						_starterTime += dt;
					}
					if (CurrentRpm > _maxStarterRpm * 0.9f)
					{
						if (Starter == 0f)
						{
							if (_ignition)
							{
								_cutOut = false;
								RaiseEvent(PhysEventId.EngineStarted);
							}
						}
						else if (_starterTime > 1f)
						{
							if (_ignition)
							{
								_cutOut = false;
								RaiseEvent(PhysEventId.EngineStarted);
							}
							_starterTime = 0f;
						}
					}
				}
			}
			float num7 = Vector3.Dot(_body.AngularVelocity, Vector3.up);
			AngAccel = (num7 - _wOld) * num;
			_wOld = num7;
		}

		private void IdleSimulation()
		{
			_accelIdle = IdleGovernor.ThrottleFloor(CurrentRpm, RpmController.x, _idleGovernorScale);
		}

		public void PreUpdate(float dt, int steps)
		{
			float num = 1f / dt;
			float num2 = dt / (float)steps;
			Vector3 a = new Vector3(0f, -9.80665f, 0f);
			if (OnLineCar)
			{
				_mgdt = PhysMath.Vector3Mul(a, _body.Mass * num2);
				_cutOut = CurrentRpm < _minStarterRpm;
				CalcTurboCharger(dt);
				CalcSuperCharger(dt);
				return;
			}
			if (ParkingGear || _fixVehicle)
			{
				Brake = 1f;
			}
			_body.ClearTotals();
			_body.StoreVelocity();
			for (int i = 0; i < 8; i++)
			{
				if (_wheels[i].Enabled)
				{
					_wheels[i].SteerSpeed = (_wheels[i].SteerAngle - _wheelSteerAng[i]) * num;
					_wheelSteerAng[i] = _wheels[i].SteerAngle;
				}
			}
			Vector3 velocity = _body.Velocity;
			Vector3 up = PhysMath.GetUp(_body.Matrix);
			Vector3 right = PhysMath.GetRight(_body.Matrix);
			for (int j = 0; j < 8; j++)
			{
				_wheels[j].LatForce = Vector3.zero;
				_wheels[j].LongForce = Vector3.zero;
				_wheels[j].VertForce = Vector3.zero;
				_wheels[j].CurrentCamber = 0f;
				_wheels[j].AlignMom = 0f;
				NumSum[j] = 0;
			}
			float num3 = 0.5f * AeroCx * AeroSx * 1.29f;
			_vcLen = velocity.magnitude;
			Vector3 vector = PhysMath.Vector3Mul(-velocity, _vcLen * num3);
			float num4 = 10f * AeroFrontDownforce / 772.83997f;
			float num5 = 10f * AeroRearDownforce / 772.83997f;
			float num6 = PhysMath.Sqr(_vcLen) * num4;
			float num7 = PhysMath.Sqr(_vcLen) * num5;
			_airSumdt = PhysMath.Vector3Mul(vector - PhysMath.Vector3Mul(up, num6 + num7), num2);
			float num8 = Vector3.Dot(Vector3.forward, _wheels[_idFrontLeft].TopPos - _body.LocalCenterMass);
			float num9 = Vector3.Dot(Vector3.forward, _body.LocalCenterMass - _wheels[_idRearLeft].TopPos);
			_airSumTdt = PhysMath.Vector3Mul(-right, (num7 * num9 - num6 * num8) * num2);
			_mgdt = PhysMath.Vector3Mul(a, _body.Mass * num2);
			UpdateSuspensionAngles(_idFrontLeft, _idFrontRight, FrontSuspensionType);
			UpdateSuspensionAngles(_idRearLeft, _idRearRight, RearSuspensionType);
			if (_idRear1Left != -1 && _idRear1Right != -1)
			{
				UpdateSuspensionAngles(_idRear1Left, _idRear1Right, Rear1SuspensionType);
			}
			if (_idRear2Left != -1 && _idRear2Right != -1)
			{
				UpdateSuspensionAngles(_idRear2Left, _idRear2Right, Rear2SuspensionType);
			}
			for (int k = 0; k < 8; k++)
			{
				if (_wheels[k].Enabled)
				{
					if (SimpleMode || ArcadeMode)
					{
						_wheels[k].UpdateRoadDistanceArcade();
						_wheels[k].UpdatePhysGroundMatrix_Simple();
					}
					else
					{
						_wheels[k].UpdateRoadDistance();
						_wheels[k].UpdatePhysGroundMatrix_Advanced();
					}
					Touch[k] = _wheels[k].IsTouch;
				}
			}
			if (!Trailer && !TrafficMode)
			{
				CalcTurboCharger(dt);
				CalcSuperCharger(dt);
				LaunchControlSimulation(dt);
			}
		}

		public void SetAlignPlane(Plane rightPlane, bool usePlane)
		{
			_rightPlane = rightPlane;
			_useAlignPlane = usePlane;
		}

		public void SetStopCar(bool stop)
		{
			_fixVehicle = stop;
		}

		public Vector3 ArcadeInertia(float mass, float carWidth, float carHeight, float carLength)
		{
			Vector3 vector = Vector3.right * carWidth + Vector3.up * (carHeight * 0.55f) + Vector3.forward * carLength;
			float x = mass / 12f * (vector.y * vector.y + vector.z * vector.z);
			float y = mass / 12f * (vector.x * vector.x + vector.z * vector.z);
			float z = mass / 12f * (vector.x * vector.x + vector.y * vector.y);
			return new Vector3(x, y, z);
		}

		public static float ResolveInertiaLength(float carLength, float wheelbase)
		{
			if (wheelbase < carLength * 0.5f)
			{
				return carLength;
			}
			return (carLength + wheelbase) / 1.6f;
		}

		public static float ResolveRestSpringDrop(float maxSpringLen, float springCoef, float cornerLoad)
		{
			if (springCoef <= 0f || maxSpringLen <= 0f)
			{
				return 0f;
			}
			return Mathf.Clamp(maxSpringLen - cornerLoad / springCoef, 0f, maxSpringLen);
		}

		public static float ResolveCogHeight(float comY, float mountY, float restSpringDrop, float wheelRadius)
		{
			return Mathf.Max(0.15f, comY - (mountY - restSpringDrop - wheelRadius));
		}

		public static float ResolveInertiaHeight(float colliderHeight, float configHeight)
		{
			if (colliderHeight <= 0f)
			{
				return configHeight;
			}
			if (configHeight <= 0f)
			{
				return colliderHeight;
			}
			float num = colliderHeight / configHeight;
			if (!(num < 0.5f) && !(num > 2f))
			{
				return colliderHeight;
			}
			return configHeight;
		}

		private bool TryGetWheelbase(out float wheelbase)
		{
			int count = 0;
			for (int i = 0; i < 8; i++)
			{
				if (_wheels[i].Enabled)
				{
					_baseBoundsScratch[count++] = _wheels[i].TopPos;
				}
			}
			if (!TryProjectSpan(_baseBoundsScratch, count, Vector3.forward, out var min, out var max) || max <= min)
			{
				wheelbase = 0f;
				return false;
			}
			wheelbase = max - min;
			return true;
		}

		public Vector3 CalcInertia(float mass, float carWidth, float carHeight, float carLength)
		{
			if (ArcadeMode)
			{
				return ArcadeInertia(mass, carWidth, carHeight, carLength);
			}
			float num = (TryGetWheelbase(out var wheelbase) ? ResolveInertiaLength(carLength, wheelbase) : carLength);
			Vector3 vector = Vector3.right * (carWidth * 0.9f) + Vector3.up * (carHeight * 0.8f) + Vector3.forward * (num * 0.9f);
			float x = mass / 12f * (vector.y * vector.y + vector.z * vector.z);
			float y = mass / 12f * (vector.x * vector.x + vector.z * vector.z);
			float z = mass / 12f * (vector.x * vector.x + vector.y * vector.y);
			return new Vector3(x, y, z);
		}

		public static bool TryProjectSpan(Vector3[] points, int count, Vector3 dir, out float min, out float max)
		{
			min = float.MaxValue;
			max = float.MinValue;
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				float num = Vector3.Dot(points[i], dir);
				if (num < min)
				{
					min = num;
				}
				if (num > max)
				{
					max = num;
				}
				flag = true;
			}
			if (!flag)
			{
				min = 0f;
				max = 0f;
			}
			return flag;
		}

		private void GetBaseBounds(Vector3 dir, out float bMin, out float bMax)
		{
			int count = 0;
			for (int i = 0; i < 8; i++)
			{
				if (_wheels[i].Enabled)
				{
					_baseBoundsScratch[count++] = _wheels[i].TopPos;
				}
			}
			if (!TryProjectSpan(_baseBoundsScratch, count, dir, out bMin, out bMax) || bMin > bMax)
			{
				bMin = -2f;
				bMax = 2f;
			}
		}

		public static Vector3 ResolveCenterOfMass(float frontPercent, float topPercent, float forwardMin, float forwardMax, float upMin, float upMax)
		{
			if (forwardMin > forwardMax)
			{
				float num = forwardMax;
				float num2 = forwardMin;
				forwardMin = num;
				forwardMax = num2;
			}
			if (upMin > upMax)
			{
				float num3 = upMax;
				float num2 = upMin;
				upMin = num3;
				upMax = num2;
			}
			Vector3 vector = Vector3.up * (upMin * (1f - topPercent * 0.01f) + upMax * (topPercent * 0.01f));
			Vector3 vector2 = Vector3.forward * (forwardMin * (1f - frontPercent * 0.01f) + forwardMax * (frontPercent * 0.01f));
			return vector + vector2;
		}

		public static float ResolveLiftLimitHeight(float trackWidth, float lateralLimitG, float margin)
		{
			if (trackWidth <= 0f || lateralLimitG <= 0f)
			{
				return 0f;
			}
			return trackWidth / (2f * lateralLimitG * Mathf.Max(margin, 0.001f));
		}

		public static float LateralLimitToG(float maxSideAccel)
		{
			return maxSideAccel / 9806.65f;
		}

		public static float ResolveUpPercent(float targetHeightRatio, float bodyHeight, float groundY, float colliderMinY, float colliderMaxY)
		{
			if (colliderMinY > colliderMaxY)
			{
				float num = colliderMaxY;
				float num2 = colliderMinY;
				colliderMinY = num;
				colliderMaxY = num2;
			}
			float num3 = colliderMaxY - colliderMinY;
			if (num3 <= 0.0001f)
			{
				return 0f;
			}
			return (groundY + targetHeightRatio * bodyHeight - colliderMinY) / num3 * 100f;
		}

		public Vector3 CalcCenterOfMass(float frontPercent, bool calcFrontCmFromWheels, float topPercent, Bounds bounds)
		{
			float bMin = Vector3.Dot(bounds.min, Vector3.forward);
			float bMax = Vector3.Dot(bounds.max, Vector3.forward);
			if (calcFrontCmFromWheels)
			{
				GetBaseBounds(Vector3.forward, out bMin, out bMax);
			}
			float upMin = Vector3.Dot(bounds.min, Vector3.up);
			float upMax = Vector3.Dot(bounds.max, Vector3.up);
			return ResolveCenterOfMass(frontPercent, topPercent, bMin, bMax, upMin, upMax);
		}

		public void SetRaceBotWheelInwardOffset(float offset)
		{
			RaceBotWheelInwardOffset = offset;
		}

		public void SetRaceBotLaunchCurve(float launchTraction, float launchBandFrac)
		{
			_rbLaunchTraction = Mathf.Clamp(launchTraction, 0.05f, 1f);
			_rbLaunchBandFrac = Mathf.Clamp(launchBandFrac, 0.02f, 0.6f);
		}

		public void SetRaceBotProfile(float topSpeedMps, float maxAccel, float maxBrake, float maxLateral, float steerGain, float maxSteerTorque, float verticalDamp, float angularDamp)
		{
			_rbTopSpeedMps = Mathf.Max(1f, topSpeedMps);
			_rbMaxAccel = Mathf.Max(0.1f, maxAccel);
			_rbMaxBrake = Mathf.Max(0.1f, maxBrake);
			_rbMaxLateral = Mathf.Max(0.1f, maxLateral);
			_rbSteerGain = Mathf.Max(1f, steerGain);
			_rbMaxSteerTq = Mathf.Max(1f, maxSteerTorque);
			_rbVerticalDamp = Mathf.Max(0f, verticalDamp);
			_rbAngularDamp = Mathf.Max(0f, angularDamp);
		}

		public void SetRaceBotAutoVerticalDamp(bool on)
		{
			_rbAutoVerticalDamp = on;
		}

		public void SetRaceBotCollisionGrip(float strength)
		{
			_rbCollisionGrip = Mathf.Clamp01(Mathf.Max(_rbCollisionGrip, strength));
		}

		public void UpdateRaceBot(float dt)
		{
			Vector3 vector = new Vector3(0f, -9.80665f, 0f);
			_body.ClearTotals();
			_body.StoreVelocity();
			float rbCollisionGrip = _rbCollisionGrip;
			if (_rbCollisionGrip > 0f)
			{
				_rbCollisionGrip = Mathf.Max(0f, _rbCollisionGrip - 1.25f * dt);
			}
			_davlenieUlitki2.x = 0f;
			_curBlowOff = 0f;
			Vector3 up = PhysMath.GetUp(_body.Matrix);
			Vector3 forward = PhysMath.GetForward(_body.Matrix);
			Vector3 right = PhysMath.GetRight(_body.Matrix);
			Vector3 midNorm = Vector3.zero;
			Vector3 midTarget = Vector3.zero;
			int num = 0;
			float num2 = 0f;
			for (int i = 0; i < 8; i++)
			{
				if (_wheels[i].Enabled)
				{
					num2 += _wheels[i].SpringCoef;
					if (_wheels[i].UpdateRoadDist_Traffic(dt, ref midNorm, ref midTarget))
					{
						num++;
					}
				}
			}
			if (num == 0)
			{
				_mgdt = vector * (_body.Mass * dt);
				_body.AddExtForce(_mgdt);
				_body.ProcessExternalForces();
				float num3 = Mathf.Lerp(1f, 0.25f, rbCollisionGrip);
				float y = _body.Velocity.y;
				if (y > num3)
				{
					_body.AddExtForce(new Vector3(0f, (0f - (y - num3)) * _body.Mass, 0f));
					_body.ProcessExternalForces();
				}
				if (PositionIntegration)
				{
					_body.UpdatePosition(dt);
				}
				return;
			}
			midTarget /= (float)num;
			PhysMath.Vector3NormalizeSafe(ref midNorm);
			if (_rbAutoVerticalDamp && num2 > 0f)
			{
				_rbVerticalDamp = Mathf.Clamp(2.2f * Mathf.Sqrt(num2 / Mathf.Max(1f, _body.Mass)), 6f, 30f);
			}
			vector = up * (0f - vector.magnitude);
			_mgdt = vector * (_body.Mass * dt);
			_body.AddExtForce(_mgdt);
			Vector3 point = (_wheels[_idFrontLeft].TopPos + _wheels[_idFrontRight].TopPos) * 0.5f;
			Vector3 point2 = (_wheels[_idRearLeft].TopPos + _wheels[_idRearRight].TopPos) * 0.5f;
			point2.y = (point.y = _body.LocalCenterMass.y);
			Vector3 pos = _body.Matrix.MultiplyPoint3x4(point);
			Vector3 pos2 = _body.Matrix.MultiplyPoint3x4(point2);
			Vector3 velocity = _body.GetVelocity(pos);
			Vector3 velocity2 = _body.GetVelocity(pos2);
			velocity -= up * Vector3.Dot(up, velocity);
			velocity2 -= up * Vector3.Dot(up, velocity2);
			float magnitude = velocity.magnitude;
			float magnitude2 = velocity2.magnitude;
			Vector3 vector2 = forward * Mathf.Cos(_steerAngWant) + right * Mathf.Sin(0f - _steerAngWant);
			if (magnitude <= 1.1f)
			{
				Vector3 v = Vector3.Cross(up, vector2);
				float value = (0f - Vector3.Dot(v, velocity)) * 10f;
				value = Mathf.Clamp(value, 0f - _rbMaxLateral, _rbMaxLateral);
				v -= midNorm * Vector3.Dot(v, midNorm);
				PhysMath.Vector3NormalizeSafe(ref v);
				_body.AddExtForce(pos, v * (value * _body.Mass * dt));
			}
			else
			{
				Vector3 vector3 = velocity / magnitude;
				float f = Vector3.Dot(vector2, vector3);
				Vector3 rhs = Vector3.Cross(vector3, vector2);
				float num4 = Mathf.Atan2(rhs.magnitude, Mathf.Abs(f));
				if (Vector3.Dot(midNorm, rhs) < 0f)
				{
					num4 = 0f - num4;
				}
				float num5 = Mathf.Clamp(num4 * _rbSteerGain, 0f - _rbMaxLateral, _rbMaxLateral);
				Vector3 v2 = Vector3.Cross(up, vector2);
				v2 -= midNorm * Vector3.Dot(v2, midNorm);
				PhysMath.Vector3NormalizeSafe(ref v2);
				_body.AddExtForce(pos, v2 * (num5 * _body.Mass * dt));
			}
			float num6 = 0f;
			if (magnitude2 <= 1.1f)
			{
				float value2 = (0f - Vector3.Dot(right, velocity2)) * 10f;
				value2 = Mathf.Clamp(value2, 0f - _rbMaxLateral, _rbMaxLateral);
				Vector3 v3 = right - midNorm * Vector3.Dot(right, midNorm);
				PhysMath.Vector3NormalizeSafe(ref v3);
				_body.AddExtForce(pos2, v3 * (value2 * _body.Mass * dt));
			}
			else
			{
				Vector3 vector4 = velocity2 / magnitude2;
				num6 = Vector3.Dot(forward, vector4);
				Vector3 rhs2 = Vector3.Cross(vector4, forward);
				float num7 = Mathf.Atan2(rhs2.magnitude, Mathf.Abs(num6));
				if (Vector3.Dot(midNorm, rhs2) < 0f)
				{
					num7 = 0f - num7;
				}
				float num8 = Mathf.Clamp(num7 * _rbSteerGain, 0f - _rbMaxLateral, _rbMaxLateral);
				Vector3 v4 = right - midNorm * Vector3.Dot(right, midNorm);
				PhysMath.Vector3NormalizeSafe(ref v4);
				_body.AddExtForce(pos2, v4 * (num8 * _body.Mass * dt));
			}
			float num9 = Vector3.Dot(_body.Velocity, forward);
			float num10 = Mathf.Clamp01(_accelWant);
			if (num9 >= _rbTopSpeedMps)
			{
				num10 = 0f;
			}
			float num11 = Mathf.Clamp01(num9 / Mathf.Max(1f, _rbTopSpeedMps));
			float num12 = ((num11 < _rbLaunchBandFrac) ? Mathf.Lerp(_rbLaunchTraction, 1f, num11 / _rbLaunchBandFrac) : Mathf.Clamp01((1.02f - num11) / 0.62f));
			float num13 = num11 % (1f / 6f) / (1f / 6f);
			if (num11 > 1f / 12f && num13 < 0.07f)
			{
				num12 *= 0.35f;
			}
			float num14 = num10 * _rbMaxAccel * num12 * _body.Mass;
			_accel = Mathf.Clamp01(_accelWant);
			SetRaceBotEngineSoundRpm(num9);
			Vector3 v5 = forward - midNorm * Vector3.Dot(forward, midNorm);
			PhysMath.Vector3NormalizeSafe(ref v5);
			Vector3 vector5 = up * Vector3.Dot(_body.Velocity, up);
			_body.AddExtForce(vector5 * ((0f - _body.Mass) * _rbVerticalDamp * dt));
			float num15 = 4f * _body.Mass * 0.25f;
			for (int j = 0; j < 8; j++)
			{
				if (_wheels[j].Enabled)
				{
					Vector3 pos3 = _body.Matrix.MultiplyPoint3x4(_wheels[j].TopPos);
					float num16 = Vector3.Dot(_body.GetVelocity(pos3), midNorm);
					if (num16 > 0f)
					{
						num16 = Mathf.Min(num16, 1.5f);
						_body.AddExtForce(pos3, midNorm * ((0f - num16) * num15 * dt));
					}
				}
			}
			float sqrMagnitude = _body.Velocity.sqrMagnitude;
			float num17 = Mathf.Min(0.001925f * sqrMagnitude, 9.24f);
			num17 += rbCollisionGrip * 30f;
			_body.AddExtForce(midNorm * ((0f - num17) * _body.Mass * dt));
			_body.AddExtForce(v5 * (num14 * dt));
			float num18 = Mathf.Clamp01(_brakeVal) * (_rbMaxBrake * _body.Mass);
			Vector3 vector6 = v5 * (0f - num18);
			Vector3 vector7 = ((num6 < 0f) ? (-vector6) : ((num6 != 0f) ? vector6 : (velocity2 * ((0f - num18) * 0.3f))));
			vector6 = vector7;
			_body.AddExtForce(vector6 * dt);
			Vector3 angularVelocity = _body.AngularVelocity;
			angularVelocity -= up * Vector3.Dot(up, angularVelocity);
			Vector3 normalized = angularVelocity.normalized;
			Vector3 tdt = angularVelocity * ((0f - _body.GetInertiaDir(normalized)) * _rbAngularDamp * dt);
			_body.AddExtTorque(tdt);
			Vector3 vector8 = Vector3.Cross(up, midNorm);
			float magnitude3 = vector8.magnitude;
			if (magnitude3 > 0.001f)
			{
				Vector3 vector9 = vector8 / magnitude3;
				_body.AddExtTorque(vector9 * (_body.GetInertiaDir(vector9) * 60f * magnitude3 * dt));
			}
			_body.ProcessExternalForces();
			if (num > 0)
			{
				bool flag = false;
				float num19 = Mathf.Lerp(1f, 0.25f, rbCollisionGrip);
				float num20 = Vector3.Dot(_body.Velocity, midNorm);
				if (num20 > num19)
				{
					_body.AddExtForce(midNorm * ((0f - (num20 - num19)) * _body.Mass));
					flag = true;
				}
				float num21 = Vector3.Dot(_body.AngularVelocity, up);
				if (Mathf.Abs(num21) > 2.5f)
				{
					float num22 = num21 - Mathf.Sign(num21) * 2.5f;
					_body.AddExtTorque(up * ((0f - num22) * _body.GetInertiaDir(up)));
					flag = true;
				}
				if (flag)
				{
					_body.ProcessExternalForces();
				}
			}
			if (PositionIntegration)
			{
				_body.UpdatePosition(dt);
			}
		}

		private void SetRaceBotEngineSoundRpm(float fwdSpeed)
		{
			float num = ((RpmController.x > 1f) ? RpmController.x : 900f);
			float b = ((RpmController.y > num) ? RpmController.y : (num + 6000f));
			float num2 = Mathf.Max(1f, _rbTopSpeedMps);
			float num3 = Mathf.Clamp01(Mathf.Abs(fwdSpeed) / num2);
			float num4 = 1f / 6f;
			int num5 = Mathf.Min((int)(num3 / num4), 5);
			float t = (num3 - (float)num5 * num4) / num4;
			float num6 = Mathf.Lerp(Mathf.Lerp(num, b, 0.45f), b, t);
			CurrentRpm = ((num3 < 0.01f) ? num : num6);
		}
	}
}
