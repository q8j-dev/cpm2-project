using System;
using CPM.Transmission;
using CPMEngine;
using UnityEngine;
using UnityEngine.Serialization;
using _Car_Parking.Scripts.Drag.Engine;

[CreateAssetMenu(fileName = "MainCarValue", menuName = "Scriptable/MainCarValues", order = 4)]
public class MainCarValues : ScriptableObject
{
	[NonSerialized]
	public bool RuntimeInstance;

	[NonSerialized]
	public GameObject car;

	public GameObject driver;

	public int gender;

	public int timeClass;

	[NonSerialized]
	public string NickName = string.Empty;

	public int CarID = 152;

	[Tooltip("Cosmetic inward pull (m per side) for THIS car's bot wheels in SimpleMode. 0 = no change. Raise if this model's wheels poke out beyond the body. Adds to BotTuning.BotWheelInwardOffset (global base). Bot-only; player/traffic unaffected.")]
	public float WheelInwardOffset;

	public VehicleClass Class;

	public int PerformanceScore;

	public float torque;

	public float torqueRPM;

	public float redlineRPM;

	public float IdleRPM = 850f;

	public float maxRPM = 8000f;

	public float EngineInertiaRatio = 0.29f;

	[Range(0f, 90f)]
	public float GearBoxLoss = 10f;

	public GearboxType drivingWheel;

	public float ShiftTime = 0.4f;

	public float LCrpm = 4000f;

	public float bodyMass;

	public TurboType Induction;

	public float turboPressure;

	public float minTurboRPM = 3000f;

	public float maxTurboRPM = 5000f;

	public LSDType FrontLsd;

	public float FrontLsdLockPercent = 20f;

	public LSDType RearLsd = LSDType.Locked;

	public float RearLsdLockPercent = 20f;

	public float[] gears;

	[Range(2.4f, 6.5f)]
	public float finalGearRatio;

	[Range(0f, 1f)]
	public float TorqueSplit4Wd = 0.5f;

	public CarOptionsValues Options;

	[Header("Front Wheels")]
	public bool UseStockFrontData;

	public FrontWheelData FrontWheelData;

	[Header("Rear Wheels")]
	public bool UseStockRearData;

	public RearWheelData RearWheelData;

	public float MaxSpeedTurnCoefficient = 1.5f;

	public float MinSpeedTurnCoefficient = 1.5f;

	public int[] BodyKitTypeInstall = Array.Empty<int>();

	[FormerlySerializedAs("Transmission")]
	public GearboxKind Gearbox = GearboxKind.AT;

	private SystemLanguage _language = SystemLanguage.Unknown;

	public AnimationCurve steeringToSpeedCurve;

	public float speedAdjustmentCoefficient;

	public float GripForwardPressureMultiplier = 0.3f;

	public float GripSidewaysPressureMultiplier = 0.1f;

	public bool IsUser;

	public bool RallyTransmission;

	public SystemLanguage language
	{
		get
		{
			if (_language == SystemLanguage.Unknown)
			{
				_language = (SystemLanguage)UnityEngine.Random.Range(0, 42);
			}
			return _language;
		}
		set
		{
			_language = value;
		}
	}
}
