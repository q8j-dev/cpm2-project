using System;
using CPMEngine;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

[DisallowMultipleComponent]
public class CarAIController : MonoBehaviour
{
	public AiType AiType;

	public bool SteerAssistent;

	public bool CarOnFront;

	public bool StopCar;

	public bool WillTurn;

	public bool KeepHighRpm;

	public bool JustEntered;

	public bool AutoReverse;

	private bool _onStopCarOnStaged;

	public float AccelSmooth = 0.8f;

	public float BrakeSmooth = 2f;

	public float SteerSmooth = 10f;

	public float TargetThrottle;

	public float TargetBrake;

	public float TargetSteering;

	private float _throttleOutput;

	private float _brakeOutput;

	private float _steeringOutput;

	public float TargetSpeed;

	private float _maxSpeed;

	public Vector3 randomOffset;

	public float angleBetween1;

	public Transform target_1;

	public float Timer;

	public Vector3[] targets;

	public int CurrentTarIndex;

	public int Sign;

	public int Row;

	public int CurrentWay;

	private Vector3 _localTarget;

	private const float DragSteerReferenceDeltaTime = 1f / 60f;

	private Sort2 _sort2;

	private Powertrain _powertrain;

	private VehicleData _vehicleData;

	private IDisposable _resetStopCarDisposable;

	private IDisposable _disposableResetJustEntered;

	private IDisposable _disposableResetAutoReverse;

	private OptionalDependency<TrafficCarControl> _trafficCarControl;

	private CarInputManager _carInputManager;

	private LevelsProvider _levelProvider;

	public CircuitCarStearing circuitCarSteering;

	private bool _lc;

	private float _dontShiftTimer;

	private float _speedIntervalTimer;

	private Bounds _intervalBound;

	public bool onStopCarOnStaged
	{
		get
		{
			return _onStopCarOnStaged;
		}
		set
		{
			_onStopCarOnStaged = value;
			if (value)
			{
				_resetStopCarDisposable?.Dispose();
				_resetStopCarDisposable = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					resetStopCarOnStaged();
				}).AddTo(this);
			}
		}
	}

	public Powertrain Powertrain
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

	private VehicleData GetVehicleData
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

	[Inject]
	public void Construct(OptionalDependency<Sort2> sort2, OptionalDependency<TrafficCarControl> trafficCarControl, CarInputManager carInputManager, LevelsProvider levelProvider)
	{
		_trafficCarControl = trafficCarControl;
		_sort2 = sort2.Value;
		_carInputManager = carInputManager;
		_levelProvider = levelProvider;
	}

	private void resetStopCarOnStaged()
	{
		onStopCarOnStaged = false;
	}

	private void Awake()
	{
		TryGetComponent<Powertrain>(out _powertrain);
		TryGetComponent<VehicleData>(out _vehicleData);
	}

	private void Start()
	{
		_maxSpeed = Powertrain.CalcTopSpeed();
	}

	private void OnEnable()
	{
		TargetThrottle = (_throttleOutput = Powertrain.Throttle);
		TargetBrake = (_brakeOutput = Powertrain.Brake);
		_steeringOutput = (TargetSteering = 0f - Powertrain.GetSteer());
	}

	private void Update()
	{
		if (AiType == AiType.None)
		{
			if (!_carInputManager || !_carInputManager.IsAutopilot())
			{
				return;
			}
			if ((bool)circuitCarSteering)
			{
				circuitCarSteering.UpdateState();
				if (circuitCarSteering.Reverse)
				{
					TargetBrake = 1f;
				}
			}
			if (Mathf.Abs(TargetSteering) < 0.01f)
			{
				TargetSteering = 0f;
			}
		}
		else
		{
			CalculateControl();
		}
	}

	private void FixedUpdate()
	{
		if (AiType != AiType.None)
		{
			Powertrain powertrain = Powertrain;
			SmoothValue(AccelSmooth, TargetThrottle, ref _throttleOutput);
			SmoothValue(BrakeSmooth, TargetBrake, ref _brakeOutput);
			SmoothValue(SteerSmooth, TargetSteering, ref _steeringOutput);
			powertrain.SetSteer(_steeringOutput, SteerAssistent, driftMode: true);
			if (powertrain.GearboxState != GearboxState.Burnout)
			{
				powertrain.Throttle = Mathf.Clamp01(_throttleOutput);
				powertrain.Brake = _brakeOutput;
			}
		}
	}

	private void SmoothValue(float vel, float target, ref float current)
	{
		if (current < target)
		{
			current += vel * Time.deltaTime;
			if (current > target)
			{
				current = target;
			}
		}
		else if (current > target)
		{
			current -= vel * Time.deltaTime;
			if (current < target)
			{
				current = target;
			}
		}
	}

	private static float SqrDistXZ(float ax, float az, float bx, float bz)
	{
		float num = ax - bx;
		float num2 = az - bz;
		return num * num + num2 * num2;
	}

	private void CalculateControl()
	{
		switch (AiType)
		{
		case AiType.DragRacingCar:
			DragRacingControl();
			break;
		case AiType.CircuitRacingCar:
			circuitCarSteering.UpdateState();
			if (circuitCarSteering.Reverse)
			{
				TargetBrake = 1f;
			}
			break;
		case AiType.LevelRacingCar:
		case AiType.TrafficCar:
			TrafficControl();
			break;
		case AiType.None:
		case AiType.DragRacingCar | AiType.CircuitRacingCar:
		case AiType.DragRacingCar | AiType.LevelRacingCar:
		case AiType.CircuitRacingCar | AiType.LevelRacingCar:
		case AiType.DragRacingCar | AiType.CircuitRacingCar | AiType.LevelRacingCar:
			break;
		}
	}

	private void TrafficControl()
	{
		TargetThrottle = 0f;
		TargetBrake = 0f;
		TargetSteering = 0f;
		if (!StopCar)
		{
			TargetThrottle = Mathf.Clamp01(1f - Powertrain.SpeedMps / TargetSpeed);
			if (Powertrain.SpeedMps - 3f > TargetSpeed)
			{
				TargetBrake = 0.2f;
			}
			if (Powertrain.SpeedMps < 1f && !AutoReverse && (AiType == AiType.TrafficCar || (_trafficCarControl.HasValue && base.gameObject == _trafficCarControl.Value.CarToFollow)))
			{
				Timer += Time.deltaTime;
				if (Timer > 4f)
				{
					AutoReverse = true;
					StopCar = true;
					_disposableResetAutoReverse?.Dispose();
					_disposableResetAutoReverse = Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
					{
						ResetAutoReverse();
					}).AddTo(this);
				}
			}
			else if (Timer > 0f)
			{
				Timer = 0f;
			}
		}
		if (StopCar)
		{
			TargetBrake = 1f;
			TargetThrottle = 0f;
		}
		if (AiType == AiType.LevelRacingCar && KeepHighRpm)
		{
			TargetThrottle = 1f;
		}
		Vector3 vector = Vector3.zero;
		if (targets == null)
		{
			return;
		}
		CurrentTarIndex = Mathf.Clamp(CurrentTarIndex, 2, targets.Length - 2);
		if (CurrentTarIndex < targets.Length - 1 && CurrentTarIndex >= 1)
		{
			float x = base.transform.localPosition.x;
			float z = base.transform.localPosition.z;
			float num = SqrDistXZ(x, z, targets[CurrentTarIndex].x, targets[CurrentTarIndex].z);
			float num2 = SqrDistXZ(x, z, targets[CurrentTarIndex - 1].x, targets[CurrentTarIndex - 1].z);
			float num3 = SqrDistXZ(x, z, targets[CurrentTarIndex + 1].x, targets[CurrentTarIndex + 1].z);
			float a = Mathf.Min(num, num2, num3);
			if (Mathf.Abs(targets[CurrentTarIndex + Sign].y) >= 100f && !JustEntered && FlagsStorage.IsFreeDriveMode && (bool)_sort2)
			{
				int num4 = (int)targets[CurrentTarIndex + Sign].y - 100;
				SceneControl.WaypointData waypointData = V.SceneControl.waypointsData[num4];
				int num5 = 0;
				if (waypointData.currentTarget.Length > 1)
				{
					num5 = UnityEngine.Random.Range(0, waypointData.sign.Length + 1);
				}
				if (waypointData.currentTarget[num5] != 0)
				{
					Sign = waypointData.sign[num5];
					CurrentTarIndex = waypointData.currentTarget[num5];
					targets = _sort2.rowTrafficArrays[waypointData.directions[num5]];
				}
				JustEntered = true;
				_disposableResetJustEntered?.Dispose();
				_disposableResetJustEntered = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					ResetJustEntered();
				}).AddTo(this);
			}
			if (Mathf.Approximately(a, num2))
			{
				CurrentTarIndex--;
			}
			if (Mathf.Approximately(a, num3))
			{
				CurrentTarIndex++;
			}
			int num6 = 1;
			if (Powertrain.SpeedMps > 20f)
			{
				num6 = 2;
			}
			if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				num6 = 1;
			}
			if (AiType == AiType.LevelRacingCar)
			{
				float speedMps = Powertrain.SpeedMps;
				int num7 = ((speedMps > 60f) ? 3 : ((!(speedMps > 30f)) ? 1 : 2));
				num6 = num7;
			}
			if (Sign > 0)
			{
				if (targets.Length - 1 > CurrentTarIndex)
				{
					vector = base.transform.InverseTransformPoint(targets[CurrentTarIndex + Sign * num6] + V.GlobalOffset);
					angleBetween1 = Mathf.Atan2(vector.x, vector.z);
				}
				else
				{
					angleBetween1 = 0f;
				}
			}
			else
			{
				vector = base.transform.InverseTransformPoint(targets[CurrentTarIndex + Sign * num6] + V.GlobalOffset);
				angleBetween1 = Mathf.Atan2(vector.x, vector.z);
			}
			if (WillTurn)
			{
				if (Powertrain.SpeedMps < 20f)
				{
					WillTurn = false;
				}
				else
				{
					TargetThrottle = 0f;
					TargetBrake = 1f;
				}
			}
			if (Mathf.Abs(angleBetween1) > 20f && Powertrain.SpeedMps > 10f)
			{
				TargetThrottle = 0f;
				TargetBrake = 1f;
			}
			if (AiType == AiType.LevelRacingCar)
			{
				float b = Mathf.Clamp(Mathf.Atan2(vector.x, vector.z) / Powertrain.MaxSteeringAngle, -1f, 1f);
				TargetSteering = Mathf.Lerp(TargetSteering, b, Time.deltaTime * (_maxSpeed - Powertrain.SpeedMps));
			}
			else
			{
				TargetSteering = Mathf.Clamp(angleBetween1 / Powertrain.MaxSteeringAngle, -1f, 1f);
			}
		}
		if (AutoReverse)
		{
			TargetSteering = 0f - TargetSteering;
		}
	}

	private void DragRacingControl()
	{
		TargetThrottle = 0f;
		TargetBrake = 0f;
		TargetSteering = 0f;
		Vector3 vector = randomOffset;
		switch (GetVehicleData.racingState)
		{
		case RacingState.staged:
			Powertrain.ResetTransmission();
			Powertrain.PositionIntegration = true;
			Powertrain.FixVehicle(stop: true);
			Powertrain.ActivateLaunchControl = true;
			TargetBrake = (_brakeOutput = 1f);
			TargetThrottle = (_throttleOutput = (GetVehicleData.RacingProvider.DragRacingController.dragRacingTree.GreenLightWillOn ? 1 : 0));
			break;
		case RacingState.rollingToStart:
			Powertrain.ActivateLaunchControl = false;
			Powertrain.FixVehicle(stop: false);
			Powertrain.PositionIntegration = false;
			if (TargetSpeed > 0f)
			{
				if (Powertrain.SpeedMps < TargetSpeed)
				{
					TargetThrottle = 1f - Powertrain.SpeedMps / TargetSpeed;
				}
				else
				{
					TargetBrake = 0.3f;
				}
			}
			else
			{
				TargetBrake = 1f;
				TargetThrottle = 0f;
			}
			break;
		case RacingState.preLaunch:
			TargetThrottle = 1f;
			TargetSpeed = 0f;
			TargetBrake = (_brakeOutput = 1f);
			break;
		case RacingState.OnRacing:
			Powertrain.FixVehicle(stop: false);
			Powertrain.ActivateLaunchControl = false;
			TargetThrottle = 1f;
			TargetBrake = (_brakeOutput = 0f);
			break;
		case RacingState.Finished:
		case RacingState.toRaceAgain:
		case RacingState.toGarage:
			Powertrain.PositionIntegration = false;
			Powertrain.ActivateLaunchControl = false;
			Powertrain.FixVehicle(stop: false);
			if (Powertrain.SpeedMps < TargetSpeed)
			{
				TargetThrottle = Mathf.Clamp01(1f - Powertrain.SpeedMps / TargetSpeed);
			}
			else
			{
				TargetBrake = 1f;
			}
			TargetThrottle = Mathf.Min(TargetThrottle, 0.7f);
			break;
		case RacingState.onGarage:
			Powertrain.FixVehicle(stop: false);
			Powertrain.PositionIntegration = false;
			Powertrain.ActivateLaunchControl = false;
			TargetThrottle = 0f;
			TargetBrake = 1f;
			break;
		}
		if (CarOnFront && TargetThrottle < 1f)
		{
			TargetThrottle = 0f;
			TargetBrake = 1f;
		}
		if (targets != null)
		{
			CurrentTarIndex = Mathf.Clamp(CurrentTarIndex, 1, targets.Length - 2);
			if (CurrentTarIndex < targets.Length - 1 && CurrentTarIndex >= 1)
			{
				float x = base.transform.localPosition.x;
				float z = base.transform.localPosition.z;
				float num = SqrDistXZ(x, z, targets[CurrentTarIndex].x, targets[CurrentTarIndex].z);
				float num2 = SqrDistXZ(x, z, targets[CurrentTarIndex - 1].x, targets[CurrentTarIndex - 1].z);
				float num3 = SqrDistXZ(x, z, targets[CurrentTarIndex + 1].x, targets[CurrentTarIndex + 1].z);
				float a = Mathf.Min(num, num2, num3);
				if (Mathf.Approximately(a, num2))
				{
					CurrentTarIndex--;
				}
				if (Mathf.Approximately(a, num3))
				{
					CurrentTarIndex++;
				}
				if (Sign > 0)
				{
					if (targets.Length - 1 > CurrentTarIndex)
					{
						_localTarget = base.transform.InverseTransformPoint(targets[CurrentTarIndex + Sign] + V.GlobalOffset + vector);
					}
					else
					{
						angleBetween1 = 0f;
					}
				}
				else
				{
					_localTarget = base.transform.InverseTransformPoint(targets[CurrentTarIndex + Sign] + V.GlobalOffset + vector);
				}
			}
			float b = Mathf.Clamp(Mathf.Atan2(_localTarget.x, _localTarget.z) / Powertrain.MaxSteeringAngle, -1f, 1f);
			TargetSteering = ((GetVehicleData.racingState == RacingState.onGarage) ? 0f : Mathf.Lerp(TargetSteering, b, 1f / 60f * (_maxSpeed - Powertrain.SpeedMps)));
			if (Mathf.Abs(TargetSteering) < 0.005f)
			{
				TargetSteering = 0f;
			}
		}
		if (_onStopCarOnStaged)
		{
			TargetThrottle = 0f;
			TargetBrake = (_brakeOutput = 1f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (AiType != AiType.LevelRacingCar || FlagsStorage.IsFreeDriveMode || base.gameObject != GetVehicleData.LevelsProvider.RacingCarLevel)
		{
			return;
		}
		string text = other.name;
		if (!(text == "stop"))
		{
			if (text == "Finish")
			{
				StopCar = true;
				FlagsStorage.Car2Finished = true;
			}
			return;
		}
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		StopCar = true;
		FlagsStorage.IsReadyMultRaceCar2 = true;
		if ((bool)V.Car && _carInputManager.Ready)
		{
			_levelProvider.TreeControl.OnBothCarOnStartLine();
		}
	}

	private void ResetJustEntered()
	{
		JustEntered = false;
	}

	private void ResetAutoReverse()
	{
		StopCar = false;
		AutoReverse = false;
		Timer = 0f;
		Powertrain.Gear = 1;
	}
}
