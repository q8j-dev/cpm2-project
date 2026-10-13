using System;
using System.Collections.Generic;
using CPMEngine;
using UnityEngine;

public class CircuitCarStearing : MonoBehaviour
{
	private enum DrivingState
	{
		Normal = 0,
		Slowing = 1,
		PreparingOvertake = 2,
		Overtaking = 3,
		Returning = 4
	}

	public CarAIController AiController;

	public float BaseSwitchDistance = 5f;

	public float NextTargetCoefficient = 2f;

	public float SpeedAdjustmentCoefficient = 0.8f;

	public float MinSpeedThreshold = 80f;

	public float MinAngleThreshold = 5f;

	public Transform Visual;

	public float AdjustedTargetSpeed;

	public float BrakeSpeedInPercent = 0.1f;

	public float AppliedBrakeMultiply = 1f;

	public float DetectionDistance = 2f;

	public float FollowOffset;

	public GameObject OvertakenCar;

	public bool IsCarOutOfRoad;

	public bool Reverse;

	public int Direction = 1;

	public CircuitTopUserMeshLabel CircuitTopUserMeshLabel;

	public int StateToCheck;

	public float ZLenght;

	public float MaxSpeedToleranceValue = 0.25f;

	public float MinSpeedToleranceValue = 0.75f;

	public float MinOvertakeDistance = 6f;

	[Header("Racing Line")]
	public float ApexFactor;

	public int RacingLineLookAhead = 5;

	public int StateByCollider;

	public int NumberPointsAnalyze = 5;

	public AnimationCurve SteeringToSpeedCurve = AnimationCurve.Linear(0f, 1f, 90f, 0f);

	public float AverageSteeringAngle;

	public List<float> SteeringMinSpeed = new List<float>();

	public List<float> SteeringMaxSpeed = new List<float>();

	public float CarFoundTime;

	public Vector3 lerpedTargetPosition;

	public Vector3 ReversePosStart;

	public Vector3 CachedPosition;

	[Header("New AI Architecture (Step 0 of refactor)")]
	[SerializeField]
	private bool _useNewArchitecture = true;

	public static bool GlobalUseNewArchitecture = true;

	private float _distanceToCarAhead;

	private int _laneIndex;

	private BakedTrajectory _trajectory;

	private BakedTrajectory _pitStopTrajectory;

	private RaceConfigSO _raceConfig;

	private int _currentWaypointIndex = -1;

	private bool _usingPitStopPath;

	private Collider _carCollider;

	private CarSteeringState _currentState;

	private bool _stopCar;

	private Axles _axles;

	private Powertrain _powertrain;

	private float _maxSpeed;

	private float _targetSpeed;

	private float _minTurnSpeedCoefficient;

	private float _maxTurnSpeedCoefficient;

	private CircuitRaceControlller _raceControlller;

	private float _targetFollowOffset = 10f;

	private DrivingState _driveState;

	private float _baseLaneOffset;

	private CircuitCarStearing _carAheadPersistent;

	private Rigidbody _rb;

	private float _raceStartTime;

	private BotPerception _newPerception;

	private BotRouteFollower _newRouteFollower;

	private bool _laneSeededFromGrid;

	private BotSpeedPlanner _newSpeedPlanner;

	private BotVehicleController _newVehicleController;

	private BotDriveStateMachine _newDriveStateMachine;

	private DriveBlackboard _newBlackboard;

	private bool _newSubsystemsReady;

	private bool _newProfileBaked;

	private BakedTrajectory _newActiveTrajectory;

	private float _newLogTimer;

	private BotRacePhysicsProfileSO _raceBotProfile;

	private bool _isRaceBot;

	private float _frozenTopSpeedKmh;

	private float _frozenMaxLateralMps2;

	private float _frozenMaxBrakeMps2;

	private float _desiredThrottle = 1f;

	private float _desiredBrake;

	private float _carSpeedMps;

	private float _carSpeedKmh;

	private Vector3 _targetPosition;

	private float _reversingTime;

	private float _laneChangeCooldown;

	private Rigidbody _carAheadRb;

	private float _sideAvoidanceOffset;

	private float _stuckTimer;

	private float _laneChangeGraceTimer;

	private float _carAheadPrevSpeedMps;

	private float _carAheadDeceleration;

	private CircuitCarsControl _circuitCarsControl;

	private float _carAheadSpeedMps;

	private float _cachedAdjustedSpeed;

	private float _previousSteering;

	private float _travelSinceSwitch;

	private float _brakeAccel;

	private Quaternion _cachedRotation;

	private Quaternion _cachedInvRotation;

	private Vector3 _cachedForward;

	private float _cachedAverageAngle;

	private int _cachedWaypointIndex = -1;

	public int GridColumn { get; private set; } = 2;

	public int GridRow { get; private set; }

	public bool UseNewArchitecture
	{
		get
		{
			if (!_useNewArchitecture)
			{
				return GlobalUseNewArchitecture;
			}
			return true;
		}
	}

	public Rigidbody Rigidbody => _rb;

	public Vector3 Velocity
	{
		get
		{
			if (!_rb)
			{
				return Vector3.zero;
			}
			return _rb.velocity;
		}
	}

	public float SpeedMps => _carSpeedMps;

	public Vector3 CachedForward => _cachedForward;

	public Quaternion CachedInvRotation => _cachedInvRotation;

	public CircuitCarsControl CircuitCarsControl => _circuitCarsControl;

	public BakedTrajectory Trajectory => _trajectory;

	public BakedTrajectory PitStopTrajectory => _pitStopTrajectory;

	public bool UsingPitStopPath => _usingPitStopPath;

	public Powertrain Powertrain => _powertrain;

	public float MinTurnCoefficient => _minTurnSpeedCoefficient;

	public float MaxTurnCoefficient => _maxTurnSpeedCoefficient;

	public float BrakeAccel => _brakeAccel;

	public float MaxSpeedCached => _maxSpeed;

	public int TotalLanes
	{
		get
		{
			if (_currentState != CarSteeringState.GoToPitStop)
			{
				return 5;
			}
			return 1;
		}
	}

	public float LaneWidth
	{
		get
		{
			if (_currentState != CarSteeringState.GoToPitStop)
			{
				return 2f;
			}
			return 4f;
		}
	}

	public int LaneIndex
	{
		get
		{
			if (_currentState != CarSteeringState.GoToPitStop)
			{
				return _laneIndex;
			}
			return 0;
		}
		set
		{
			_laneIndex = value;
		}
	}

	private bool CarAheadTooClose
	{
		get
		{
			if (!_carAheadPersistent)
			{
				return false;
			}
			float distanceToCarAhead = _distanceToCarAhead;
			float num = Mathf.Max(25f, _carSpeedMps * 0.55f);
			return distanceToCarAhead < num;
		}
	}

	private bool CanChangeLane
	{
		get
		{
			if (_carSpeedMps < 3f)
			{
				return true;
			}
			return Mathf.Abs(AiController.TargetSteering) * 35f < 35f;
		}
	}

	public bool IsRaceBot => _isRaceBot;

	public BotPersonality Personality { get; private set; } = BotPersonality.Neutral;

	private bool HasWaypoint => _currentWaypointIndex >= 0;

	private BakedTrajectory ActiveTrajectory
	{
		get
		{
			if (!_usingPitStopPath)
			{
				return _trajectory;
			}
			return _pitStopTrajectory;
		}
	}

	private int ActiveWaypointCount => ActiveTrajectory?.Count ?? 0;

	private float Spacing => ActiveTrajectory?.SampleSpacing ?? 2f;

	public void SetGridPosition(int column, int row)
	{
		GridColumn = column;
		GridRow = row;
	}

	private ref WaypointData GetWP(int index)
	{
		return ref ActiveTrajectory.GetWaypoint(index);
	}

	private int Wrap(int index)
	{
		return ActiveTrajectory.WrapIndex(index);
	}

	private int Advance(int index, int steps)
	{
		return ActiveTrajectory.Advance(index, steps, Direction);
	}

	private int AdvanceByDistance(int index, float meters)
	{
		int steps = Mathf.Max(1, Mathf.RoundToInt(meters / Spacing));
		return Advance(index, steps);
	}

	public void Init(CircuitRaceControlller raceCircuitRaceControlller)
	{
		_raceControlller = raceCircuitRaceControlller;
		_circuitCarsControl = _raceControlller.circuitCarsControl;
		if ((bool)AiController)
		{
			AiController.BrakeSmooth = 8f;
			AiController.AccelSmooth = 6f;
		}
	}

	public void SetMinSpeedCoefficient(float minSpeed, float maxSpeed)
	{
		float bakedCoeffScale = _circuitCarsControl.BotTuningSO.BakedCoeffScale;
		_minTurnSpeedCoefficient = minSpeed * bakedCoeffScale;
		_maxTurnSpeedCoefficient = maxSpeed * bakedCoeffScale;
		_newSpeedPlanner?.SetCoefficients(_minTurnSpeedCoefficient, _maxTurnSpeedCoefficient);
	}

	public void SetRaceConfig(RaceConfigSO config)
	{
		if (!(config == null))
		{
			_raceConfig = config;
			_trajectory = config.BakedTrajectory;
			_pitStopTrajectory = config.PitStopTrajectory;
			_newProfileBaked = false;
			_newActiveTrajectory = null;
			if (_newRouteFollower != null)
			{
				_newRouteFollower.SetActiveTrajectory(null);
			}
		}
	}

	public CarSteeringState GetCurrentState()
	{
		return _currentState;
	}

	public void SetWaypoints(int stepCount)
	{
		ResetCarState();
		_currentWaypointIndex = FindClosestWaypoint();
		float num = (float)stepCount * 10f;
		int num2 = Mathf.Max(1, Mathf.RoundToInt(num / Spacing));
		int num3 = Wrap(_currentWaypointIndex + num2);
		int num4 = Wrap(_currentWaypointIndex - num2);
		_currentWaypointIndex = ((Direction > 0) ? num3 : num4);
		lerpedTargetPosition = GetWP(_currentWaypointIndex).Position;
		InitLateralOffset();
	}

	private void Awake()
	{
		TryGetComponent<Rigidbody>(out _rb);
		TryGetComponent<Powertrain>(out _powertrain);
		TryGetComponent<Axles>(out _axles);
	}

	private void Start()
	{
		if (AiController == null)
		{
			AiController = GetComponent<CarAIController>();
			AiController.SteerAssistent = false;
		}
		VehicleData component = GetComponent<VehicleData>();
		if (component != null)
		{
			_carCollider = component.Collider;
		}
		if (_carCollider != null)
		{
			ZLenght = _carCollider.bounds.size.z * 0.5f * _carCollider.transform.localScale.z;
		}
		CircuitRaceControlller.CurrentClassId.Get(out int _);
		if (!_isRaceBot)
		{
			_maxSpeed = _powertrain.CalcTopSpeed() * 3.61f;
			_targetSpeed = _maxSpeed / (_powertrain.MaxSideAccel / 1000f);
			_brakeAccel = Mathf.Max(5f, _powertrain.MaxBrakeAccel / 1000f);
		}
	}

	public void SetMinSpeed()
	{
		if (!_axles)
		{
			_axles = GetComponent<Axles>();
		}
		SteeringMinSpeed.Clear();
		SteeringMaxSpeed.Clear();
		int count = _trajectory.Count;
		for (int i = 0; i < count; i++)
		{
			ref WaypointData waypoint = ref _trajectory.GetWaypoint(i);
			float turnRadius = waypoint.TurnRadius;
			float forwardSteeringAngle = waypoint.ForwardSteeringAngle;
			float getWheelBase = _powertrain.GetWheelBase;
			float maxSteeringAngle = _powertrain.MaxSteeringAngle;
			float t = Mathf.InverseLerp(5f, 100f, turnRadius);
			float num = Mathf.Lerp(1.2f, 3f, t);
			float item = CalculateTurnSpeed(turnRadius, _minTurnSpeedCoefficient * num, forwardSteeringAngle, getWheelBase, maxSteeringAngle);
			float item2 = CalculateTurnSpeed(turnRadius, _maxTurnSpeedCoefficient * num, forwardSteeringAngle, getWheelBase, maxSteeringAngle);
			SteeringMinSpeed.Add(item);
			SteeringMaxSpeed.Add(item2);
		}
	}

	public float CalculateTurnSpeed(float turnRadiusFromWaypoints, float coeficent, float forwardTurnSteeringAngle, float wheelBase, float maxSteerAngleDeg)
	{
		if (!_powertrain)
		{
			_powertrain = GetComponent<Powertrain>();
		}
		float b = _powertrain.MaxSideAccel / 1000f * coeficent;
		b = Mathf.Max(0.01f, b);
		float num = turnRadiusFromWaypoints;
		if (num <= 0f || float.IsInfinity(num) || float.IsNaN(num))
		{
			num = float.MaxValue;
		}
		float value = Mathf.Sqrt(b * num);
		float min = 2.777778f;
		float max = 138.88889f;
		return Mathf.Clamp(value, min, max) * 3.6f;
	}

	public void ResetWaypoint()
	{
		_currentWaypointIndex = -1;
	}

	public void GoToPitStopWaypoint()
	{
	}

	public void StopCar(bool state)
	{
		_stopCar = state;
	}

	public void SetPitStopPath(BakedTrajectory specificPath)
	{
		BaseSwitchDistance = 1f;
		_usingPitStopPath = true;
		_pitStopTrajectory = specificPath;
	}

	private void GoToWaypoint()
	{
		if (HasWaypoint)
		{
			FollowWaypointsWithOvertaking();
			AdjustCarSpeed();
			float num = Mathf.Clamp(4f + _carSpeedMps * 0.08f, 4f, 8f);
			if (HasWaypoint)
			{
				float num2 = Mathf.Abs((Direction > 0) ? GetWP(_currentWaypointIndex).ForwardSteeringAngle : GetWP(_currentWaypointIndex).ReverseSteeringAngle);
				float num3 = Mathf.Lerp(1f, 3f, Mathf.Clamp01((num2 - 0.1f) / 0.4f));
				num *= num3;
			}
			lerpedTargetPosition = Vector3.Lerp(lerpedTargetPosition, _targetPosition, Time.fixedDeltaTime * num);
			UpdateDrivingState();
		}
	}

	public void HardChangeSteeringState(CarSteeringState state, bool resetWaypoint = false)
	{
		_currentState = state;
		if (resetWaypoint)
		{
			_currentWaypointIndex = -1;
		}
	}

	public void ChangeSteeringState(CarSteeringState state, bool IsCarBroken = false, bool notResetWaypoint = false)
	{
		if ((state == CarSteeringState.GoToPitStop && !IsCarBroken) || (state == CarSteeringState.PreparePitStop && !IsCarBroken))
		{
			return;
		}
		_currentState = state;
		if (!notResetWaypoint)
		{
			if (state == CarSteeringState.GoToPitStop || state == CarSteeringState.Race || state == CarSteeringState.PreparePitStop)
			{
				_currentWaypointIndex = -1;
			}
			if (state == CarSteeringState.Race)
			{
				_currentWaypointIndex = -1;
				_usingPitStopPath = false;
				BaseSwitchDistance = 5f;
				_raceStartTime = Time.time;
			}
		}
	}

	public void UpdateState()
	{
		if (!_powertrain)
		{
			_powertrain = GetComponent<Powertrain>();
		}
		switch (_currentState)
		{
		case CarSteeringState.Garage:
			GoToWaypoint();
			SetThrottleForCruise();
			break;
		case CarSteeringState.RaseStartPos:
			_desiredThrottle = 1f;
			_powertrain.FixVehicle(stop: true);
			break;
		case CarSteeringState.Race:
			_powertrain.FixVehicle(stop: false);
			GoToWaypoint();
			break;
		case CarSteeringState.GoToPitStop:
			GoToWaypoint();
			SetThrottleForCruise();
			break;
		case CarSteeringState.PreparePitStop:
			GoToWaypoint();
			break;
		case CarSteeringState.Stop:
			_desiredThrottle = 0f;
			_powertrain.FixVehicle(stop: true);
			break;
		case CarSteeringState.OnPitStop:
			break;
		}
	}

	private void SetThrottleForCruise()
	{
		float num = AiController.TargetSpeed + MaxSpeedToleranceValue * 20f;
		float num2 = AiController.TargetSpeed - MinSpeedToleranceValue * 20f;
		if (!_powertrain)
		{
			_powertrain = GetComponent<Powertrain>();
		}
		float num3 = _carSpeedMps * 3.6f;
		if (num3 > num || _powertrain.CurrentRpm > _powertrain.RedlineRpm)
		{
			_desiredThrottle = 0f;
		}
		else if (num3 > num2)
		{
			float t = (num - num3) / 20f;
			float desiredThrottle = Mathf.Lerp(0f, 1f, t);
			_desiredThrottle = desiredThrottle;
		}
		else
		{
			_desiredThrottle = 1f;
		}
	}

	private void FollowWaypointsWithOvertaking()
	{
		ref WaypointData wP = ref GetWP(_currentWaypointIndex);
		Vector3 to = wP.Position - CachedPosition;
		if (Vector3.Angle(_cachedForward, to) > 90f && _carSpeedMps > 2f)
		{
			int num = FindClosestWaypoint();
			if (num >= 0)
			{
				_currentWaypointIndex = AdvanceByDistance(num, 10f);
				_travelSinceSwitch = 0f;
				wP = ref GetWP(_currentWaypointIndex);
			}
		}
		_targetPosition = wP.Position;
		_targetPosition += wP.Right * FollowOffset;
		if (_currentState != CarSteeringState.PreparePitStop)
		{
			_targetPosition = GetCenterWithOffset(_currentWaypointIndex);
		}
		Vector3 vector = base.transform.InverseTransformPoint(lerpedTargetPosition);
		if (!_axles)
		{
			_axles = GetComponent<Axles>();
		}
		float num2 = Mathf.Cos(_powertrain.Caster * ((float)Math.PI / 180f));
		float num3 = Mathf.Clamp(Mathf.Atan2(vector.x, vector.z) / num2 / _powertrain.MaxSteeringAngle, -1f, 1f);
		float num4 = Mathf.Abs(num3 - _previousSteering);
		float num5 = Mathf.Lerp(5f, 2.5f, Mathf.Clamp01(_carSpeedMps / 60f));
		float num6 = Mathf.Lerp(1f, 3f, Mathf.Clamp01(num4 / 0.5f));
		float maxDelta = num5 * num6 * Time.fixedDeltaTime;
		num3 = (_previousSteering = Mathf.MoveTowards(_previousSteering, num3, maxDelta));
		if (Reverse)
		{
			AiController.TargetSteering = 0f - num3;
		}
		else
		{
			AiController.TargetSteering = num3;
		}
		float num7 = AdjustSwitchDistance(_carSpeedMps * 3.61f);
		float num8 = Spacing * 3f;
		if (Vector3.Distance(CachedPosition, wP.Position) < num7 && _travelSinceSwitch >= num8)
		{
			int index = AdvanceByDistance(_currentWaypointIndex, 10f);
			ref WaypointData wP2 = ref GetWP(index);
			float a = Mathf.Abs((Direction > 0) ? wP2.ForwardSteeringAngle : wP2.ReverseSteeringAngle);
			float b = Mathf.Abs((Direction > 0) ? wP.ForwardSteeringAngle : wP.ReverseSteeringAngle);
			float num9 = Mathf.Max(a, b);
			float meters = ((num9 >= 0.05f) ? Mathf.Clamp(_carSpeedMps * 0.3f, 10f, 25f) : ((!(num9 >= 0.02f)) ? Mathf.Clamp(_carSpeedMps * 0.4f, 15f, 40f) : Mathf.Clamp(_carSpeedMps * 0.3f, 12f, 30f)));
			_ = _currentWaypointIndex;
			_currentWaypointIndex = AdvanceByDistance(_currentWaypointIndex, meters);
			_travelSinceSwitch = 0f;
		}
	}

	private Vector3 GetCenterWithOffset(int wpIndex = -1)
	{
		if (wpIndex < 0)
		{
			wpIndex = _currentWaypointIndex;
		}
		ref WaypointData wP = ref GetWP(wpIndex);
		Vector3 leftBoundary = wP.LeftBoundary;
		Vector3 rightBoundary = wP.RightBoundary;
		Vector3 normalized = (rightBoundary - leftBoundary).normalized;
		float num = Vector3.Distance(leftBoundary, rightBoundary) * 0.5f;
		float num2 = CalculateRacingLineOffset(wpIndex, num);
		float value = FollowOffset + num2;
		value = Mathf.Clamp(value, (0f - num) * 0.9f, num * 0.9f);
		return wP.Position + normalized * value;
	}

	private float CalculateRacingLineOffset(int wpIndex, float roadHalfWidth)
	{
		if (ApexFactor <= 0f)
		{
			return 0f;
		}
		if (_carAheadPersistent != null && CarAheadTooClose)
		{
			return 0f;
		}
		ref WaypointData wP = ref GetWP(wpIndex);
		float num = ((Direction > 0) ? wP.ForwardSteeringAngle : wP.ReverseSteeringAngle);
		float num2 = 0f;
		bool flag = false;
		int num3 = wpIndex;
		int num4 = 0;
		int steps = Mathf.Max(1, Mathf.RoundToInt(10f / Spacing));
		for (int i = 0; i < RacingLineLookAhead; i++)
		{
			num3 = Advance(num3, steps);
			if (num3 == wpIndex)
			{
				break;
			}
			ref WaypointData wP2 = ref GetWP(num3);
			float num5 = ((Direction > 0) ? wP2.ForwardSteeringAngle : wP2.ReverseSteeringAngle);
			num2 += num5;
			num4++;
			if (Mathf.Abs(num) > 0.02f && Mathf.Abs(num5) > 0.02f && Mathf.Sign(num) != Mathf.Sign(num5))
			{
				flag = true;
			}
		}
		if (flag)
		{
			return 0f;
		}
		float num6 = ((num4 > 0) ? (num2 / (float)num4) : 0f);
		float num7 = num * roadHalfWidth * ApexFactor;
		float num8 = (0f - num6) * roadHalfWidth * ApexFactor * 0.5f;
		float num9 = Mathf.Abs(num);
		float num10 = Mathf.Abs(num6);
		float num11 = num9 + num10;
		if (num11 < 0.01f)
		{
			return 0f;
		}
		return (num7 * num9 + num8 * num10) / num11;
	}

	private void ReversingNow()
	{
		if (!AiController.AutoReverse)
		{
			_desiredBrake = 1f;
			_desiredThrottle = 0f;
			AiController.AutoReverse = true;
			ReversePosStart = CachedPosition;
		}
		_reversingTime = Time.fixedDeltaTime + _reversingTime;
	}

	private void CancelReverse()
	{
		Reverse = false;
		if (AiController.AutoReverse || AiController.Powertrain.Gear == -1)
		{
			AiController.AutoReverse = false;
			AiController.Powertrain.Gear = 1;
		}
		_reversingTime = 0f;
		ReversePosStart = Vector3.zero;
	}

	private void SetSpeedState()
	{
		switch (_currentState)
		{
		case CarSteeringState.GoToPitStop:
		case CarSteeringState.PreparePitStop:
			AiController.TargetSpeed = 60f;
			break;
		case CarSteeringState.OnPitStop:
			MinSpeedThreshold = 0f;
			AiController.TargetSpeed = 0f;
			break;
		case CarSteeringState.Race:
			MinSpeedThreshold = 150f;
			AiController.TargetSpeed = 500f;
			break;
		case CarSteeringState.Garage:
			AiController.TargetSpeed = 120f;
			break;
		case CarSteeringState.RaseStartPos:
			break;
		}
	}

	private void AdjustCarSpeed()
	{
		if (IsCarOutOfRoad && !Reverse)
		{
			_desiredThrottle = 0.3f;
			_currentWaypointIndex = -1;
			return;
		}
		SetSpeedState();
		AdjustedTargetSpeed = AdjustSpeedBasedOnSteering(AiController.TargetSpeed);
		if (HasWaypoint && GetWP(_currentWaypointIndex).ShouldBrake)
		{
			_desiredBrake = Mathf.Max(_desiredBrake, 0.3f);
			_desiredThrottle = 0f;
		}
		float num = _carSpeedKmh - AdjustedTargetSpeed;
		if (num > 0f)
		{
			_desiredThrottle = 0f;
			if (((AdjustedTargetSpeed > 1f) ? (num / AdjustedTargetSpeed) : 1f) > 0.3f)
			{
				_desiredBrake = 1f;
			}
			else
			{
				_desiredBrake = Mathf.Clamp01(num / 10f);
			}
		}
		else if (Reverse)
		{
			StateToCheck = 2;
			ReversingNow();
			if (Vector3.Distance(CachedPosition, ReversePosStart) > ZLenght * 5f || _reversingTime > 10f)
			{
				CancelReverse();
			}
		}
		else if (_carSpeedKmh < AdjustedTargetSpeed || _carSpeedKmh < MinSpeedThreshold)
		{
			_powertrain.FixVehicle(stop: false);
			StateToCheck = 3;
			_desiredThrottle = 1f;
			if (_driveState == DrivingState.Normal || _carAheadPersistent == null)
			{
				_desiredBrake = 0f;
			}
		}
		else if (_carSpeedKmh > AdjustedTargetSpeed + AdjustedTargetSpeed * BrakeSpeedInPercent)
		{
			StateToCheck = 4;
			_desiredThrottle = 0f;
			_desiredBrake = (1f - AiController.TargetSteering) * AppliedBrakeMultiply;
		}
		else
		{
			StateToCheck = 5;
			_desiredThrottle = 0f;
			if (_driveState == DrivingState.Normal || _carAheadPersistent == null)
			{
				_desiredBrake = 0f;
			}
		}
	}

	private void FixedUpdate()
	{
		if (UseNewArchitecture && _currentState == CarSteeringState.Race)
		{
			TickNew();
			return;
		}
		if (_stopCar || _currentState == CarSteeringState.Stop)
		{
			_desiredThrottle = 0f;
			_desiredBrake = 1f;
			if (AiController != null)
			{
				ApplyThrottleAndBrake();
			}
			return;
		}
		CachedPosition = base.transform.position;
		_cachedRotation = base.transform.rotation;
		_cachedInvRotation = Quaternion.Inverse(_cachedRotation);
		_cachedForward = _cachedRotation * Vector3.forward;
		_carSpeedMps = _powertrain.SpeedMps;
		_carSpeedKmh = _carSpeedMps * 3.6f;
		_travelSinceSwitch += _carSpeedMps * Time.fixedDeltaTime;
		if (!HasWaypoint)
		{
			_currentWaypointIndex = FindClosestWaypoint();
		}
		StateToCheck = 0;
		ApplyThrottleAndBrake();
	}

	private BotContext BuildBotContext()
	{
		return new BotContext
		{
			Position = CachedPosition,
			Rotation = _cachedRotation,
			InvRotation = _cachedInvRotation,
			Forward = _cachedForward,
			SpeedMps = _carSpeedMps,
			SpeedKmh = _carSpeedKmh,
			Powertrain = _powertrain,
			MaxSpeedKmh = _maxSpeed,
			BrakeAccelMps2 = _brakeAccel,
			OuterState = _currentState,
			Direction = Direction,
			Dt = Time.fixedDeltaTime,
			TimeSinceRaceStart = Time.time - _raceStartTime,
			LaneWidth = LaneWidth,
			TotalLanes = TotalLanes,
			OwnerName = base.name
		};
	}

	private void EnsureNewSubsystems()
	{
		if (!_newSubsystemsReady && !(_circuitCarsControl == null))
		{
			_newBlackboard = new DriveBlackboard
			{
				LaneIndex = LaneIndex,
				GridColumn = GridColumn,
				GridRow = GridRow,
				RaceStartTime = _raceStartTime,
				DesiredLateralOffset = FollowOffset,
				LaneChangeCooldown = 4f
			};
			_newPerception = new BotPerception(this, _circuitCarsControl);
			_newRouteFollower = new BotRouteFollower(this);
			_newSpeedPlanner = new BotSpeedPlanner(this);
			_newSpeedPlanner.SetCoefficients(_minTurnSpeedCoefficient, _maxTurnSpeedCoefficient);
			_newVehicleController = new BotVehicleController(this);
			_newDriveStateMachine = new BotDriveStateMachine(this);
			_newSubsystemsReady = true;
		}
	}

	private void EnsureProfileBaked()
	{
		if (!_newProfileBaked && !(_powertrain == null) && _trajectory != null)
		{
			if (_isRaceBot)
			{
				_newSpeedPlanner.SetFrozenSpec(_frozenTopSpeedKmh, _frozenMaxBrakeMps2, _frozenMaxLateralMps2);
			}
			_newSpeedPlanner.BakeProfile(_trajectory, _powertrain);
			_newProfileBaked = true;
		}
	}

	public void EnableRaceBotPhysics(Powertrain pt, BotRacePhysicsProfileSO profile, int carIndex, BotPersonality? personality = null, float ppPaceScale = 1f)
	{
		if (!pt)
		{
			pt = (_powertrain ? _powertrain : GetComponent<Powertrain>());
		}
		if ((bool)pt)
		{
			_powertrain = pt;
			_raceBotProfile = profile;
			Personality = personality ?? BotPersonality.Neutral;
			BotTuningSO botTuningSO = _circuitCarsControl.BotTuningSO;
			float num = (profile ? profile.MaxAccelMps2 : botTuningSO.DefaultBotMaxAccel);
			float num2 = (profile ? profile.MaxBrakeMps2 : botTuningSO.DefaultBotMaxBrake);
			float num3 = Mathf.Max(botTuningSO.DefaultBotMaxLateral, profile ? profile.MaxLateralAccelMps2 : 0f);
			float steerGain = (profile ? profile.SteerTorqueGain : botTuningSO.DefaultBotSteerGain);
			float maxSteerTorque = (profile ? profile.MaxSteerTorque : botTuningSO.DefaultBotMaxSteerTorque);
			float verticalDamp = (profile ? profile.VerticalDamp : 14f);
			float angularDamp = (profile ? profile.AngularDamp : 11f);
			float num4 = (((bool)profile && profile.TopSpeedKmh > 0f) ? profile.TopSpeedKmh : (pt.CalcTopSpeed() * 3.6f));
			float num5 = (profile ? Mathf.Clamp01(profile.PaceSpreadFrac) : botTuningSO.DefaultBotPaceSpread);
			float num6 = Mathf.Abs(Mathf.Sin((float)(carIndex + 1) * 12.9898f) * 43758.547f);
			num6 -= Mathf.Floor(num6);
			float num7 = Personality.PaceBias * ppPaceScale * (1f + (num6 * 2f - 1f) * num5);
			num *= num7;
			num4 *= num7;
			num4 = (_frozenTopSpeedKmh = num4 * Mathf.Clamp(botTuningSO.StraightSpeedScale, 0.3f, 1.2f));
			_frozenMaxLateralMps2 = num3;
			_frozenMaxBrakeMps2 = num2;
			_isRaceBot = true;
			_maxSpeed = num4;
			_brakeAccel = Mathf.Max(5f, num2);
			_targetSpeed = _maxSpeed / Mathf.Max(0.01f, num3);
			pt.SetRaceBotProfile(num4 / 3.6f, num, num2, num3, steerGain, maxSteerTorque, verticalDamp, angularDamp);
			pt.RaceBotMode = true;
			pt.SetRaceBotLaunchCurve(profile ? profile.LaunchTraction : 0.256f, profile ? profile.LaunchBandFrac : 0.22f);
			pt.SetRaceBotAutoVerticalDamp(profile == null);
			pt.StartEngine(starterSound: false);
		}
	}

	private void SyncActiveTrajectory()
	{
		BakedTrajectory bakedTrajectory = (_usingPitStopPath ? _pitStopTrajectory : _trajectory);
		if (bakedTrajectory != _newActiveTrajectory)
		{
			_newRouteFollower.SetActiveTrajectory(bakedTrajectory);
			_newActiveTrajectory = bakedTrajectory;
		}
	}

	private void TickNew()
	{
		if (!_powertrain)
		{
			_powertrain = GetComponent<Powertrain>();
		}
		EnsureNewSubsystems();
		if (!_newSubsystemsReady)
		{
			return;
		}
		EnsureProfileBaked();
		if (_stopCar)
		{
			return;
		}
		switch (_currentState)
		{
		case CarSteeringState.Stop:
			return;
		case CarSteeringState.RaseStartPos:
			_desiredThrottle = 1f;
			if ((bool)_powertrain)
			{
				_powertrain.FixVehicle(stop: true);
			}
			ApplyThrottleAndBrake();
			return;
		case CarSteeringState.OnPitStop:
			return;
		case CarSteeringState.Race:
			if ((bool)_powertrain)
			{
				_powertrain.FixVehicle(stop: false);
			}
			break;
		}
		SyncActiveTrajectory();
		CachedPosition = base.transform.position;
		_cachedRotation = base.transform.rotation;
		_cachedInvRotation = Quaternion.Inverse(_cachedRotation);
		_cachedForward = _cachedRotation * Vector3.forward;
		if ((bool)_powertrain)
		{
			_carSpeedMps = _powertrain.SpeedMps;
		}
		_carSpeedKmh = _carSpeedMps * 3.6f;
		if (!_laneSeededFromGrid && _newActiveTrajectory != null && _newActiveTrajectory.Count > 0)
		{
			_laneSeededFromGrid = true;
			int index = 0;
			float num = float.MaxValue;
			for (int i = 0; i < _newActiveTrajectory.Count; i++)
			{
				float sqrMagnitude = (CachedPosition - _newActiveTrajectory.GetWaypoint(i).Position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					index = i;
				}
			}
			ref WaypointData waypoint = ref _newActiveTrajectory.GetWaypoint(index);
			Vector3 normalized = (waypoint.RightBoundary - waypoint.LeftBoundary).normalized;
			float num2 = Vector3.Dot(CachedPosition - waypoint.Position, normalized);
			int num3 = TotalLanes / 2;
			int num4 = (LaneIndex = Mathf.Clamp(Mathf.RoundToInt(num2 / Mathf.Max(0.5f, LaneWidth)), -num3, num3));
			FollowOffset = (float)num4 * LaneWidth;
			_newBlackboard.LaneIndex = num4;
			_newBlackboard.DesiredLateralOffset = FollowOffset;
			_newDriveStateMachine.SeedLane(FollowOffset);
			_newRouteFollower.SeedFollowOffset(FollowOffset);
		}
		_newBlackboard.IsCarOutOfRoad = IsCarOutOfRoad;
		if (_carSpeedMps < 1f && AiController != null && AiController.TargetBrake > 0.3f)
		{
			_newBlackboard.StuckTimer = Mathf.Min(5f, _newBlackboard.StuckTimer + Time.fixedDeltaTime);
		}
		else
		{
			_newBlackboard.StuckTimer = Mathf.MoveTowards(_newBlackboard.StuckTimer, 0f, Time.fixedDeltaTime * 2f);
		}
		if (_newBlackboard.LaneChangeCooldown > 0f)
		{
			_newBlackboard.LaneChangeCooldown -= Time.fixedDeltaTime;
		}
		if (_newBlackboard.OvertakeCommitTimer > 0f)
		{
			_newBlackboard.OvertakeCommitTimer -= Time.fixedDeltaTime;
		}
		BotContext ctx = BuildBotContext();
		ref PerceptionSnapshot snap = ref _newPerception.Tick(in ctx);
		RouteTarget route = _newRouteFollower.Tick(in ctx, in snap, _newBlackboard);
		SpeedTarget speed = _newSpeedPlanner.Tick(in ctx, in snap, in route);
		DriveCommand cmd = _newDriveStateMachine.Tick(in ctx, in snap, in route, in speed, _newBlackboard);
		_newBlackboard.DesiredLateralOffset = cmd.DesiredLateralOffset;
		_newBlackboard.LaneIndex = cmd.LaneIndexHint;
		_newBlackboard.SuppressApex = cmd.SuppressApex;
		if (cmd.RequestReverse)
		{
			Reverse = true;
		}
		else if (_carSpeedMps > 3f && Reverse)
		{
			Reverse = false;
		}
		_newVehicleController.Apply(AiController, in cmd, in route, Reverse);
		lerpedTargetPosition = route.LerpedTarget;
		FollowOffset = route.FollowOffset;
		LaneIndex = cmd.LaneIndexHint;
		AdjustedTargetSpeed = speed.MaxSpeedKmh;
		AverageSteeringAngle = route.LookAheadCurvature;
		OvertakenCar = _newBlackboard.OvertakenCar;
	}

	private void ApplyThrottleAndBrake()
	{
		_desiredThrottle = Mathf.Clamp01(_desiredThrottle);
		_desiredBrake = Mathf.Clamp01(_desiredBrake);
		if (_desiredBrake > 0.1f)
		{
			_desiredThrottle = 0f;
		}
		AiController.TargetThrottle = _desiredThrottle;
		AiController.TargetBrake = _desiredBrake;
	}

	private float AdjustSpeedBasedOnSteering(float baseTargetSpeed)
	{
		if (!HasWaypoint || SteeringMaxSpeed.Count == 0)
		{
			return baseTargetSpeed;
		}
		if (_currentState == CarSteeringState.GoToPitStop || _currentState == CarSteeringState.PreparePitStop || _currentState == CarSteeringState.OnPitStop)
		{
			return 30f;
		}
		int currentWaypointIndex = _currentWaypointIndex;
		if (currentWaypointIndex == _cachedWaypointIndex)
		{
			AverageSteeringAngle = _cachedAverageAngle;
			return _cachedAdjustedSpeed;
		}
		float num = Mathf.Clamp(_carSpeedMps * _carSpeedMps / (2f * _brakeAccel) + 10f, 40f, 400f);
		float num2 = _maxSpeed;
		float num3 = 0f;
		float num4 = 0f;
		int num5 = 0;
		ref WaypointData wP = ref GetWP(_currentWaypointIndex);
		float num6 = Vector3.Distance(CachedPosition, wP.Position);
		int num7 = _currentWaypointIndex;
		int activeWaypointCount = ActiveWaypointCount;
		for (int i = 0; i < 200 && i < activeWaypointCount; i++)
		{
			ref WaypointData wP2 = ref GetWP(num7);
			int num8 = num7;
			if (num8 >= 0 && num8 < SteeringMaxSpeed.Count)
			{
				float num9 = SteeringMaxSpeed[num8];
				if (num9 < num2)
				{
					num2 = num9;
					num3 = num6;
				}
			}
			num4 += Mathf.Abs((Direction > 0) ? wP2.ForwardSteeringAngle : wP2.ReverseSteeringAngle);
			num5++;
			num6 += ((wP2.DistanceToNext > 0.01f) ? wP2.DistanceToNext : 3f);
			if (num6 >= num)
			{
				break;
			}
			int num10 = Advance(num7, 1);
			if (num10 == _currentWaypointIndex)
			{
				break;
			}
			num7 = num10;
		}
		AverageSteeringAngle = ((num5 > 0) ? (num4 / (float)num5) : 0f);
		if (num2 < _maxSpeed)
		{
			float num11 = num2 / 3.6f;
			float num12 = Mathf.Max(0f, (_carSpeedMps * _carSpeedMps - num11 * num11) / (2f * _brakeAccel));
			if (num12 >= num3)
			{
				float t = Mathf.Clamp01(num12 / Mathf.Max(num3, 0.1f));
				float b = Mathf.Lerp(_carSpeedKmh, num2, t);
				num2 = Mathf.Min(num2, b);
			}
			else
			{
				float num13 = _maxSpeed;
				int currentWaypointIndex2 = _currentWaypointIndex;
				if (currentWaypointIndex2 >= 0 && currentWaypointIndex2 < SteeringMaxSpeed.Count)
				{
					num13 = Mathf.Min(num13, SteeringMaxSpeed[currentWaypointIndex2] * 1.1f);
				}
				num2 = num13;
			}
		}
		if (AverageSteeringAngle > 0.12f)
		{
			float num14 = Mathf.Lerp(1f, 0.5f, Mathf.Clamp01((AverageSteeringAngle - 0.12f) / 0.15f));
			float b2 = _maxSpeed * num14;
			num2 = Mathf.Min(num2, b2);
		}
		float num15 = ((!(num2 >= _maxSpeed)) ? Mathf.Clamp(num2, 20f, _maxSpeed) : _maxSpeed);
		_cachedWaypointIndex = currentWaypointIndex;
		_cachedAverageAngle = AverageSteeringAngle;
		_cachedAdjustedSpeed = num15;
		return num15;
	}

	private float AdjustSwitchDistance(float speedKm)
	{
		return Mathf.Lerp(BaseSwitchDistance, 25f * Mathf.Clamp(NextTargetCoefficient * (speedKm / 100f), 1f, 3f), speedKm / 150f);
	}

	private int FindClosestWaypoint()
	{
		if (ActiveTrajectory == null || ActiveTrajectory.Count == 0)
		{
			return -1;
		}
		float num = float.PositiveInfinity;
		int num2 = -1;
		int activeWaypointCount = ActiveWaypointCount;
		Vector3 cachedPosition = CachedPosition;
		for (int i = 0; i < activeWaypointCount; i++)
		{
			float sqrMagnitude = (cachedPosition - GetWP(i).Position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				num2 = i;
			}
			if (sqrMagnitude < 100f)
			{
				break;
			}
		}
		if (num2 < 0)
		{
			return -1;
		}
		if (_currentState == CarSteeringState.PreparePitStop)
		{
			return AdvanceByDistance(num2, 20f);
		}
		return AdvanceByDistance(num2, 30f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_currentState == CarSteeringState.Race)
		{
			float magnitude = collision.relativeVelocity.magnitude;
			if (!(magnitude < 5f) && _isRaceBot && (bool)_powertrain)
			{
				_powertrain.SetRaceBotCollisionGrip(Mathf.Clamp01((magnitude - 5f) / 15f));
			}
		}
	}

	private void OnDestroy()
	{
	}

	public void SetSplineLength()
	{
		_raceControlller.TotalTrackDistance = _raceConfig.LapDistance;
	}

	private void InitLateralOffset()
	{
		_baseLaneOffset = (float)LaneIndex * LaneWidth;
		if (Mathf.Abs(FollowOffset) < 0.01f)
		{
			FollowOffset = _baseLaneOffset;
		}
		else
		{
			_baseLaneOffset = FollowOffset;
		}
	}

	private void UpdateDrivingState()
	{
		float num = LaneWidth * (10f + _carSpeedMps / 6f);
		switch (_driveState)
		{
		case DrivingState.Normal:
			_desiredBrake = Mathf.MoveTowards(_desiredBrake, 0f, Time.fixedDeltaTime * 8f);
			_desiredThrottle = Mathf.MoveTowards(_desiredThrottle, 1f, Time.fixedDeltaTime * 6f);
			if (OvertakenCar == null)
			{
				_targetFollowOffset = _baseLaneOffset;
			}
			break;
		case DrivingState.Slowing:
		{
			float carAheadSpeedMps = _carAheadSpeedMps;
			float num2 = Mathf.Max(0f, _carSpeedMps - carAheadSpeedMps);
			float num3 = _carSpeedMps * _carSpeedMps / (2f * _brakeAccel);
			float num4 = Mathf.Max(8f, num3 * 0.2f + num2 * num2 / (2f * _brakeAccel) + num2 * 0.4f);
			bool num5 = (bool)_carAheadPersistent && _distanceToCarAhead < num4;
			float num6 = 0f;
			if (_carAheadDeceleration > 2f)
			{
				num6 = Mathf.Clamp01(_carAheadDeceleration / 12f);
			}
			if (num5)
			{
				_desiredBrake = 1f;
				_desiredThrottle = 0f;
			}
			else if ((bool)_carAheadPersistent && (bool)_carAheadRb)
			{
				float num7 = Mathf.Max(15f, _carSpeedMps * _carSpeedMps / (2f * _brakeAccel) * 0.5f);
				float num8 = Mathf.Clamp01(_distanceToCarAhead / num7);
				float num9 = carAheadSpeedMps * Mathf.Lerp(0.6f, 1f, num8);
				if (_carSpeedMps > num9 + 1f)
				{
					float a = Mathf.Clamp01((_carSpeedMps - num9) / 15f);
					_desiredThrottle = Mathf.Lerp(0f, 0.2f, num8);
					_desiredBrake = Mathf.Max(a, num6);
				}
				else
				{
					_desiredBrake = num6;
					_desiredThrottle = ((num6 > 0.1f) ? 0f : Mathf.Clamp01(0.3f + num8 * 0.7f));
				}
			}
			else
			{
				_desiredThrottle = 0.6f;
				_desiredBrake = 0f;
			}
			break;
		}
		case DrivingState.PreparingOvertake:
			_desiredBrake = Mathf.Lerp(_desiredBrake, 0f, Time.fixedDeltaTime * 3f);
			_desiredThrottle = Mathf.Lerp(_desiredThrottle, 1f, Time.fixedDeltaTime * 1.5f);
			if (Mathf.Abs(FollowOffset - _baseLaneOffset) < LaneWidth * 0.4f)
			{
				_driveState = DrivingState.Overtaking;
				_laneChangeGraceTimer = 0.3f;
			}
			else if (_carAheadPersistent == null)
			{
				_driveState = DrivingState.Returning;
				OvertakenCar = null;
			}
			break;
		case DrivingState.Overtaking:
		{
			_desiredThrottle = Mathf.Lerp(_desiredThrottle, 1f, Time.fixedDeltaTime * 2f);
			_desiredBrake = Mathf.Lerp(_desiredBrake, 0f, Time.fixedDeltaTime * 3f);
			if (_carAheadPersistent == null)
			{
				_driveState = DrivingState.Returning;
				break;
			}
			float num10 = Vector3.Distance(CachedPosition, _carAheadPersistent.CachedPosition);
			if (base.transform.InverseTransformPoint(_carAheadPersistent.CachedPosition).z < (0f - ZLenght) * 4f || num10 > MinOvertakeDistance * 4f)
			{
				_driveState = DrivingState.Returning;
				OvertakenCar = null;
			}
			break;
		}
		case DrivingState.Returning:
			_desiredBrake = Mathf.Lerp(_desiredBrake, 0f, Time.fixedDeltaTime * 2f);
			_desiredThrottle = Mathf.Lerp(_desiredThrottle, 1f, Time.fixedDeltaTime * 1.2f);
			if (Mathf.Abs(FollowOffset - _baseLaneOffset) < 0.2f)
			{
				_driveState = DrivingState.Normal;
				FollowOffset = _baseLaneOffset;
			}
			break;
		}
		float value = _targetFollowOffset + _sideAvoidanceOffset - FollowOffset;
		float num11 = num * Time.fixedDeltaTime;
		float num12 = Mathf.Clamp(value, 0f - num11, num11);
		FollowOffset += num12;
		float num13 = LaneWidth * ((float)TotalLanes / 2f - 0.5f);
		FollowOffset = Mathf.Clamp(FollowOffset, 0f - num13, num13);
	}

	public void ResetCarState()
	{
		_driveState = DrivingState.Normal;
		_currentState = CarSteeringState.Stop;
		_laneSeededFromGrid = false;
		CachedPosition = base.transform.position;
		_currentWaypointIndex = -1;
		_usingPitStopPath = false;
		_cachedWaypointIndex = -1;
		_cachedAverageAngle = 0f;
		_cachedAdjustedSpeed = 0f;
		_carAheadSpeedMps = 0f;
		OvertakenCar = null;
		_carAheadPersistent = null;
		_carAheadRb = null;
		_carAheadPrevSpeedMps = 0f;
		_carAheadDeceleration = 0f;
		_sideAvoidanceOffset = 0f;
		_stuckTimer = 0f;
		_laneChangeCooldown = 0f;
		_laneChangeGraceTimer = 0f;
		_targetFollowOffset = 0f;
		FollowOffset = 0f;
		_baseLaneOffset = 0f;
		lerpedTargetPosition = Vector3.zero;
		_targetPosition = Vector3.zero;
		Reverse = false;
		IsCarOutOfRoad = false;
		_stopCar = false;
		CarFoundTime = 0f;
		_reversingTime = 0f;
		_carSpeedMps = 0f;
		AdjustedTargetSpeed = 0f;
		DetectionDistance = 2f;
		AverageSteeringAngle = 0f;
		_previousSteering = 0f;
		_travelSinceSwitch = 0f;
		ReversePosStart = Vector3.zero;
		StateToCheck = 0;
		StateByCollider = 0;
		if (AiController != null)
		{
			_desiredThrottle = 0f;
			_desiredBrake = 0f;
			AiController.TargetSteering = 0f;
			AiController.AutoReverse = false;
		}
		if (_newRouteFollower != null)
		{
			_newRouteFollower.SetActiveTrajectory(null);
		}
		_newActiveTrajectory = null;
		_newBlackboard?.Reset();
	}
}
