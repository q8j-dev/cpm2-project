using System;
using System.Collections.Generic;
using System.Linq;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Scripting;
using UnityEngine.Splines;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

public class LapRaceCar : MonoBehaviour
{
	private const int LPlaceCoeficent = 100;

	private const int WaitLimit = 12;

	private const int LemanWaitLimit = 600;

	private const float DistanceMagnitudeToRoad = 100f;

	private const float MinDistanceToUserCarToRoad = 100f;

	private const float DelayToStartCheckToRoad = 5f;

	private const int MissCountsWrongWay = 3;

	private const float EmptyFuelTankLiters = 0.001f;

	private const string UI_MAP_ICON = "UI_mapIconCircuitCar";

	public bool IsWrongWay;

	public bool IsHasBonusLap;

	public bool IsBroken;

	public bool IsLemanFinished;

	public bool IsDisqualified;

	public bool IsCrossWrongFinishSide;

	public int place;

	public int LastTriggerId;

	public int PitStopCount;

	public float amplCrash = 0.03f;

	public float gainCrash = 0.4f;

	private float _timeCrash;

	private int _id;

	private int _allLapsCount;

	private int _currentLap = -1;

	private bool _isUser;

	private bool _isManual;

	private bool _isDestroyed;

	private bool _isOutOfRoad;

	private bool _isRaceStarted;

	private bool _isRaceFinish;

	private float _startTime;

	private float _currentLapStartTime;

	private float _totalProgress;

	private float _penaltyDistance;

	private float _allTime;

	private float _localProgress;

	private float _penaltyCrashSecounds;

	private float _penaltyTime;

	private float _finalDistance;

	private float _trackLength;

	private float _allRaceTime;

	private List<float> _lapTime;

	private Rigidbody _rigidbody;

	private Axles _axles;

	private Powertrain _powertrain;

	private VehicleEntityProvider _vehicleEntityProvider;

	private CircuitRaceControlller _circuitRaceControlller;

	private CircuitCarStearing _circuitCarStearing;

	private LapCheckpointTrigger[] _lapCheckpointTriggers;

	private CircuitTopUserMeshLabel _circuitTopUserMeshLabel;

	private CarAIController _carAiController;

	private GlobalCollectionsProvider _collectionsProvider;

	private RacingCameraController _racingCameraController;

	private string _name;

	private Texture _rankIcon;

	private Texture _carIcon;

	[HideInInspector]
	public MaintenanceStatus CurrentWheelStatus;

	[HideInInspector]
	public MaintenanceStatus CurrentFuelStatus;

	private CarDefaultTuningDataConfig _carDefaultTuningDataConfig;

	private Vector3 _lastSplinePosition;

	private Quaternion _lastSplineRotation;

	private Vector3 _nextSplinePosition;

	private Quaternion _nextSplineRotation;

	private int _nearestSplinePointIndex;

	private Transform _miniMapTransform;

	private int _passedLaps;

	private float _startTimeCheckToRoad;

	private int _missCountsWrongWay;

	private float _bestLapTime = -1f;

	private float _performancePoints;

	private float _stuckTimer;

	private float _recoveryCooldown;

	private bool _skipSplineWidth;

	private int lastKnotIndex;

	private bool _canToRoad = true;

	private float _delayToRoad = 2f;

	public VehicleData VehicleData { get; private set; }

	public DisqualificationType CurrentDisqualificationType { get; set; }

	[UnityEngine.Scripting.Preserve]
	[Inject]
	private void Construct(GlobalCollectionsProvider collectionsProvider, RacingCameraController racingCameraController)
	{
		_collectionsProvider = collectionsProvider;
		_racingCameraController = racingCameraController;
	}

	private void OnDestroy()
	{
		CircuitRaceControlller circuitRaceControlller = _circuitRaceControlller;
		circuitRaceControlller.PrepareToRaceAction = (Action)Delegate.Remove(circuitRaceControlller.PrepareToRaceAction, new Action(PrepareToRace));
		_isDestroyed = true;
	}

	private void Awake()
	{
		TryGetComponent<Powertrain>(out _powertrain);
	}

	private void Start()
	{
		_powertrain = GetComponent<Powertrain>();
		_isManual = !_powertrain.IsAutomaticGearbox;
		SubscribeController();
	}

	public void AddPenaltyDistance(float distance)
	{
		_penaltyDistance += distance;
	}

	public void AddPenaltyTime(float time)
	{
		_penaltyTime += time;
	}

	private void SubscribeController()
	{
		TryGetComponent<CarAIController>(out _carAiController);
	}

	public void ResetTransmission()
	{
		_powertrain.ResetTransmission();
	}

	public void InitRaceCar(CircuitRaceControlller circuitRaceController, int id, int lapCount, int carId, GlobalCollectionsProvider collectionsProvider, bool isUser = false)
	{
		_circuitRaceControlller = circuitRaceController;
		_collectionsProvider = collectionsProvider;
		circuitRaceController.PrepareToRaceAction = (Action)Delegate.Remove(circuitRaceController.PrepareToRaceAction, new Action(PrepareToRace));
		circuitRaceController.PrepareToRaceAction = (Action)Delegate.Combine(circuitRaceController.PrepareToRaceAction, new Action(PrepareToRace));
		ResetVariables();
		IsLemanFinished = false;
		PitStopCount = 0;
		IsHasBonusLap = false;
		CurrentWheelStatus = MaintenanceStatus.Good;
		CurrentFuelStatus = MaintenanceStatus.Good;
		VehicleData = GetComponent<VehicleData>();
		_axles = VehicleData.Axles;
		_vehicleEntityProvider = VehicleData.GetVehicleEntityMono;
		IsCrossWrongFinishSide = false;
		if (TryGetComponent<CircuitCarStearing>(out _circuitCarStearing))
		{
			_circuitCarStearing.SetMinSpeed();
		}
		if (!isUser)
		{
			_circuitTopUserMeshLabel = _circuitCarStearing.CircuitTopUserMeshLabel;
		}
		_carDefaultTuningDataConfig = CarTuningFactory.GetDefaultCarTuningDataConfig(carId).Tuning;
		_id = id;
		_isUser = isUser;
		_trackLength = _circuitRaceControlller.TotalTrackDistance;
		_lapCheckpointTriggers = _circuitRaceControlller.GetCheckPoints();
		_powertrain = VehicleData.GetPowertrain;
		_rigidbody = _powertrain.Rigidbody;
		_powertrain.FixVehicle(stop: false);
		if (!isUser)
		{
			CreateMapIcon();
		}
		UtilsRank.RankType rankType = UtilsRank.RankType.Beginner;
		if (isUser)
		{
			rankType = UtilsRank.GetRank();
		}
		else
		{
			_circuitTopUserMeshLabel.SetCamera(circuitRaceController.GetCameraTransform());
		}
		_rankIcon = UtilsRank.LoadRankTextureFromResources(rankType);
		_carIcon = Resources.Load($"UI/CarImages/b{carId}", typeof(Texture)) as Texture;
		if (!_carAiController)
		{
			SubscribeController();
		}
	}

	public void SetDirection(int laneIndex)
	{
		_circuitCarStearing.Direction = CircuitRaceControlller.Direction;
		_circuitCarStearing.LaneIndex = laneIndex;
	}

	public void SetLaps(int lapCount)
	{
		_allLapsCount = lapCount;
		_lapTime = new List<float>();
		for (int i = 0; i < lapCount; i++)
		{
			_lapTime.Add(0f);
		}
	}

	private void PrepareToRace()
	{
		_powertrain.FixVehicle(stop: true);
		_circuitCarStearing.ChangeSteeringState(CarSteeringState.RaseStartPos);
	}

	public void Restart()
	{
		_circuitRaceControlller.circuitCarsControl.SetIgnoreCollisionWithCollider(this, ignore: false);
		if (_isUser)
		{
			SetAutopilot(state: false);
		}
		else
		{
			_vehicleEntityProvider.Refuel(_carDefaultTuningDataConfig.ChassisData.FuelTankCapacity);
		}
		FreezeCar(state: true);
		if ((bool)_rigidbody)
		{
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;
		}
		_circuitCarStearing.ChangeSteeringState(CarSteeringState.Stop);
		_powertrain.ResetTransmission(resetGear: false);
		_circuitCarStearing.SetWaypoints(6);
		_circuitCarStearing.SetMinSpeed();
		ResetVariables();
	}

	public float GetPerformancePoints()
	{
		return (int)_vehicleEntityProvider.CarEntity.PerformancePoints;
	}

	private void ResetVariables()
	{
		_stuckTimer = 0f;
		IsLemanFinished = false;
		_isRaceFinish = false;
		LastTriggerId = -1;
		PitStopCount = 0;
		_penaltyTime = 0f;
		_currentLapStartTime = 0f;
		_passedLaps = 0;
		_penaltyCrashSecounds = 0f;
		_penaltyDistance = 0f;
		_localProgress = 0f;
		_currentLap = -1;
		_totalProgress = 0f;
		_allTime = 0f;
		_startTime = 0f;
		_allRaceTime = 0f;
		_bestLapTime = 0f;
	}

	public void SetName(string name)
	{
		_name = name;
		if (!_isUser)
		{
			_circuitTopUserMeshLabel.InitTexts(_name, place.ToString());
		}
	}

	public VehicleEntityProvider GetVehicleEntityProvider()
	{
		return _vehicleEntityProvider;
	}

	public CarDefaultTuningDataConfig GetCarDefaultTuningDataConfig()
	{
		return _carDefaultTuningDataConfig;
	}

	public Rigidbody GetRigidbody()
	{
		return _rigidbody;
	}

	public Powertrain GetPowerTrain()
	{
		return _powertrain;
	}

	public int GetNearestSplinePointIndex()
	{
		return _nearestSplinePointIndex;
	}

	public void ChangeCarState(CarSteeringState state, bool notResetWaypoint = false)
	{
		if (!_isUser)
		{
			_circuitCarStearing.ChangeSteeringState(state, IsBroken, notResetWaypoint);
		}
	}

	public void ChangeSplineWidthState()
	{
		_skipSplineWidth = !_skipSplineWidth;
	}

	public void SetPitStopWaypoints(BakedTrajectory path)
	{
		_circuitCarStearing.SetPitStopPath(path);
	}

	public float GetSpeed()
	{
		return _powertrain.SpeedMps;
	}

	public void SetPlaceLabel(string place)
	{
		_circuitTopUserMeshLabel.SetPlace(place);
	}

	public void ShowLabel(bool state)
	{
		_circuitTopUserMeshLabel.gameObject.SetActive(state);
	}

	public void SetTopMeshAlpha(float alpha)
	{
		_circuitTopUserMeshLabel.SetAlpha(alpha);
	}

	public Texture GetRankIcon()
	{
		return _rankIcon;
	}

	public Texture GetCarIcon()
	{
		return _carIcon;
	}

	public bool IsCheatFinish()
	{
		return Time.time - _currentLapStartTime < 10f;
	}

	public string GetUserName()
	{
		return _name;
	}

	private async UniTaskVoid CreateMapIcon()
	{
		GameObject gameObject = await Addressables.InstantiateAsync("UI_mapIconCircuitCar");
		if ((bool)gameObject)
		{
			_collectionsProvider.RegisterMapIcon(gameObject.transform);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = base.transform.localPosition;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localEulerAngles = new Vector3(90f, -90f, 0f);
			gameObject.transform.localScale = new Vector3(20f, 20f, 5f);
			if (!FlagsStorage.IsMultiFree)
			{
				gameObject.transform.parent = base.transform;
			}
		}
	}

	public void EnableWheels(bool state)
	{
		if (!_axles)
		{
			return;
		}
		foreach (WheelProvider allWheel in _axles.AllWheels)
		{
			allWheel.enabled = false;
		}
	}

	public void FreezeCar(bool state)
	{
		if ((bool)_circuitCarStearing)
		{
			_circuitCarStearing.StopCar(state);
		}
		if (_isUser)
		{
			_powertrain.FixVehicle(state);
		}
	}

	public bool IsUser()
	{
		return _isUser;
	}

	public int GetCurrentLap()
	{
		return _currentLap;
	}

	public int GetTotalLaps()
	{
		return _lapTime.Count;
	}

	public int GetRaceCarId()
	{
		return _id;
	}

	public void SetAllTime(float time)
	{
		_allTime = time;
	}

	public float GetTotalProgress()
	{
		return _totalProgress;
	}

	public float GetTotalDistance()
	{
		float num = 0f;
		if (_currentLap > 0)
		{
			num = (float)_currentLap * _circuitRaceControlller.TotalTrackDistance;
		}
		return num + _totalProgress;
	}

	public float GetLemanTotalDistance()
	{
		float num = 0f;
		num += (float)_passedLaps * _circuitRaceControlller.TotalTrackDistance;
		if (num == 0f)
		{
			num = _totalProgress;
		}
		return num;
	}

	public float GetTotalDistanceForLeman()
	{
		return GetLemanTotalDistance() - _penaltyDistance - (float)(place * 100);
	}

	public float GetTotalClearDistance()
	{
		return GetLemanTotalDistance() + _penaltyDistance;
	}

	public float GetPenaltyDistance()
	{
		return _penaltyDistance;
	}

	public void SetFinalDistance(float distance)
	{
		_finalDistance = distance;
	}

	public float GetFinalDistance()
	{
		return _finalDistance;
	}

	public void UpdateDistance(float checkpointDistance, float distanceToNext)
	{
	}

	private bool CheckBestTime(float currentTime)
	{
		bool flag = _bestLapTime <= 0f || _bestLapTime > currentTime;
		_bestLapTime = (flag ? currentTime : _bestLapTime);
		return flag;
	}

	public float OnLapFinish()
	{
		int currentLap = _currentLap;
		if (_circuitRaceControlller.IsCurrentMapLeman())
		{
			float num = Time.time - _currentLapStartTime;
			if (_lapTime.Count <= currentLap)
			{
				_lapTime.Add(num);
			}
			else
			{
				_lapTime[currentLap] = num;
			}
			_currentLapStartTime = _lapTime[currentLap];
			if (!_circuitRaceControlller.IsLemanTimeExpired())
			{
				_currentLap++;
			}
			_passedLaps++;
			_currentLapStartTime = Time.time;
		}
		else
		{
			_lapTime[currentLap] = Time.time - _currentLapStartTime;
			_currentLapStartTime = _lapTime[currentLap];
			_currentLap++;
			_currentLapStartTime = Time.time;
		}
		if (_isUser)
		{
			_missCountsWrongWay = 0;
		}
		if (CheckBestTime(_lapTime[currentLap]) && _isUser)
		{
			_circuitRaceControlller.CircuitRacingUIControll.BestTimeChanged();
		}
		return _lapTime[currentLap];
	}

	public float GetCurrentLapTime()
	{
		return _lapTime.Last();
	}

	public float GetBestLap()
	{
		return _bestLapTime;
	}

	public float GetFinishTime()
	{
		float num = 0f;
		for (int i = 0; i < _lapTime.Count; i++)
		{
			num += _lapTime[i];
		}
		return num;
	}

	public float GetLastFinishTime()
	{
		return _lapTime.Last();
	}

	public float GetCrashPenaltySecs()
	{
		return _penaltyCrashSecounds;
	}

	public float GetFirstFinishTime()
	{
		return _lapTime.First();
	}

	public float GetLapTimeCalcValue()
	{
		float num = 0f;
		for (int i = 0; i < _lapTime.Count; i++)
		{
			num += _lapTime[i];
		}
		return num / (float)_lapTime.Count;
	}

	public bool IsFinalLap()
	{
		return _currentLap == _allLapsCount;
	}

	public bool IsFinishRace()
	{
		if (_circuitRaceControlller.GetRaceMapModel().RaceConfig.IsLeman)
		{
			return false;
		}
		return _isRaceFinish;
	}

	public void StartRace()
	{
		if (_isUser)
		{
			VehicleClass classVehicle = Utils.GetClassVehicle(_vehicleEntityProvider.CarEntity.PerformancePoints, _vehicleEntityProvider.GetVehicleData.CarId);
			CircuitRaceControlller.CurrentClassId.Get(out int value);
			if ((int)classVehicle > value)
			{
				CurrentDisqualificationType = DisqualificationType.AnotherClass;
				_circuitRaceControlller.RemoveUser();
				return;
			}
		}
		_currentLapStartTime = Time.time;
		_currentLap = 0;
		_localProgress = 0f;
		_startTime = Time.time;
		_missCountsWrongWay = 0;
	}

	public float GetStartTime()
	{
		return _startTime;
	}

	public float GetCurrentTime()
	{
		return Time.time - _startTime;
	}

	public float GetTotalTime()
	{
		float num = 0f;
		if (_allTime > 0f)
		{
			return _allTime + _penaltyTime + _penaltyCrashSecounds + (float)place;
		}
		return _allTime + GetFinishTime() + _penaltyTime + _penaltyCrashSecounds + (float)place;
	}

	public float GetClearTime()
	{
		return _allTime + GetFinishTime();
	}

	public float GetBestLapTime()
	{
		float num = _lapTime.OrderBy((float x) => x).ToArray().First();
		if (num == 0f)
		{
			num = GetTotalTime() / (float)_allLapsCount;
		}
		return num;
	}

	private void SetAutopilot(bool state)
	{
		if (_isUser && (bool)_circuitCarStearing)
		{
			if (state)
			{
				_circuitCarStearing.ChangeSteeringState(CarSteeringState.Race);
			}
			_circuitCarStearing.enabled = state;
			_circuitRaceControlller.EnableAutoPilot(state);
		}
	}

	public void FinishRace(bool stopCar = true)
	{
		IsLemanFinished = true;
		_isRaceFinish = true;
		if (_isUser)
		{
			if (stopCar)
			{
				_circuitRaceControlller.circuitCarsControl.StopCar(base.gameObject);
			}
			SetAutopilot(state: true);
		}
	}

	public void Disqualified()
	{
	}

	public void CalculatePos()
	{
		if (_isDestroyed)
		{
			return;
		}
		CheckToRoad();
		int nearestBezierKnotIndex = GetNearestBezierKnotIndex(base.transform.position);
		float num = CalculateLengthFromIndexToEnd(nearestBezierKnotIndex);
		if (IsLemanFinished)
		{
			return;
		}
		if (IsCrossWrongFinishSide)
		{
			_totalProgress = 0f;
			return;
		}
		if (_isUser)
		{
			float num2 = _circuitRaceControlller.TotalTrackDistance * (float)_currentLap + num;
			bool flag = num2 < _localProgress;
			IsWrongWay = IsUserRideWrongWay(flag);
			if (_isUser)
			{
				_circuitRaceControlller.RaceCrewTalkSoundController.PlayWrongWaySoundEvent(flag);
			}
			_localProgress = num2;
			_circuitRaceControlller.UserRideWrongWay(IsWrongWay);
			if (IsWrongWay)
			{
				return;
			}
		}
		CheckPenaltyTime();
		_totalProgress = num;
		if (_circuitRaceControlller.IsCurrentMapLeman())
		{
			CheckMaintenance();
		}
		if (!_isUser)
		{
			DetectStuck();
		}
	}

	private bool IsUserRideWrongWay(bool startWrongWay)
	{
		if (!startWrongWay)
		{
			return false;
		}
		_missCountsWrongWay++;
		if (_missCountsWrongWay >= 3)
		{
			_missCountsWrongWay = 0;
			return true;
		}
		return false;
	}

	private void CheckPenaltyTime()
	{
		bool flag = IsOutOfTrackBySpline() || IsOutOfRoadBySurface();
		if (flag && _powertrain.SpeedKmh > 1f)
		{
			AddPenaltyDistance(_circuitRaceControlller.GetOutOfRoadPenaltyVar());
			AddPenaltyTime(_circuitRaceControlller.GetPenaltyOutOfRoadVar());
		}
		_isOutOfRoad = flag;
		_circuitCarStearing.IsCarOutOfRoad = flag;
		if (_isUser)
		{
			_circuitRaceControlller.UserOutOfRoad(flag);
		}
	}

	private bool IsOutOfTrackBySpline()
	{
		if (_skipSplineWidth)
		{
			return false;
		}
		Spline spline = _circuitRaceControlller.GetSpline();
		if (spline == null || spline.Count < 2)
		{
			return false;
		}
		RaceConfigSO currentRaceConfig = _circuitRaceControlller.CurrentRaceConfig;
		if (currentRaceConfig == null)
		{
			return false;
		}
		float halfTrackWidth = currentRaceConfig.HalfTrackWidth;
		int index = Mathf.Clamp(_nearestSplinePointIndex, 0, spline.Count - 1);
		float3 position = spline[index].Position;
		Vector3 position2 = base.transform.position;
		Vector3 b = new Vector3(position.x, position.y, position.z);
		position2.y = 0f;
		b.y = 0f;
		return Vector3.Distance(position2, b) > halfTrackWidth;
	}

	private bool IsOutOfRoadBySurface()
	{
		if (_axles == null)
		{
			return false;
		}
		int num = 0;
		for (int i = 0; i < _axles.AllWheels.Count; i++)
		{
			CPMEngine.PhysicMaterial currentPhysMat = _axles.AllWheels[i].CurrentPhysMat;
			if (currentPhysMat != null)
			{
				Surface surfaceType = currentPhysMat.SurfaceType;
				if (surfaceType != Surface.Dryasphalt && surfaceType != Surface.Wetasphalt && surfaceType != Surface.Kerb)
				{
					num++;
				}
			}
		}
		return num > 2;
	}

	public float GetPenaltyTime()
	{
		return _penaltyTime + _penaltyCrashSecounds;
	}

	private float CalculateLengthFromIndexToEnd(int startIndex)
	{
		Spline spline = _circuitRaceControlller.GetSpline();
		int count = spline.Count;
		if (startIndex < 0 || startIndex >= count - 1)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = startIndex; i < count - 1; i++)
		{
			float3 position = spline[i].Position;
			float3 position2 = spline[i + 1].Position;
			num += Vector3.Distance(position, position2);
		}
		return num;
	}

	public int GetNearestBezierKnotIndex(Vector3 position)
	{
		int num = 0;
		float num2 = float.MaxValue;
		Spline spline = _circuitRaceControlller.GetSpline();
		for (int i = 0; i < spline.Count; i++)
		{
			BezierKnot bezierKnot = spline[i];
			Vector3 vector = new Vector3(bezierKnot.Position.x, bezierKnot.Position.y, bezierKnot.Position.z);
			float num3 = Vector3.SqrMagnitude(position - vector);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
				if (!_isOutOfRoad)
				{
					_lastSplinePosition = vector;
					_lastSplineRotation = bezierKnot.Rotation;
					int index = ((i == 0) ? (spline.Count - 1) : (i - 1));
					_nextSplinePosition = new Vector3(spline[index].Position.x, spline[index].Position.y, spline[index].Position.z);
					_nextSplineRotation = spline[index].Rotation;
				}
			}
		}
		_nearestSplinePointIndex = num;
		return num;
	}

	public void ToRoad()
	{
		if (!_canToRoad)
		{
			return;
		}
		_canToRoad = false;
		Observable.Timer(TimeSpan.FromSeconds(_delayToRoad)).Subscribe((long _) =>
		{
			_canToRoad = true;
		}).AddTo(this);
		if (_isUser || !V.Car || !((V.Car.transform.position - base.transform.position).magnitude < 100f))
		{
			if ((bool)_circuitCarStearing)
			{
				_circuitCarStearing.ResetWaypoint();
			}
			SetCarToRoad();
			DelayedEnableColliderOurCar(3f);
		}
	}

	private void SetCarToRoad()
	{
		(Vector3, Quaternion) posAndRotToRoad = GetPosAndRotToRoad();
		Vector3 position = posAndRotToRoad.Item1 + new Vector3(0f, 0.3f, 0f);
		Quaternion rotation = ((CircuitRaceControlller.Direction > 0) ? posAndRotToRoad.Item2 : GetFlippedRotation(posAndRotToRoad.Item2));
		_circuitRaceControlller.circuitCarsControl.SetCarToRoad(this, position, rotation);
		if (_isUser)
		{
			_circuitRaceControlller.ResetUserCameraInertia();
		}
	}

	private (Vector3, Quaternion) GetPosAndRotToRoad()
	{
		if (Physics.OverlapSphere(_lastSplinePosition, 1f, LayerViaName.CarLayer).Length != 0)
		{
			return (_nextSplinePosition, _nextSplineRotation);
		}
		return (_lastSplinePosition, _lastSplineRotation);
	}

	private Quaternion GetFlippedRotation(Quaternion originalRotation)
	{
		return originalRotation * Quaternion.Euler(0f, 180f, 0f);
	}

	private void DelayedEnableColliderOurCar(float delay)
	{
		_circuitRaceControlller.circuitCarsControl.SetIgnoreCollisionWithCollider(this, ignore: true);
		Observable.Timer(TimeSpan.FromSeconds(delay)).Subscribe((long _) =>
		{
			_circuitRaceControlller.circuitCarsControl.SetIgnoreCollisionWithCollider(this, ignore: false);
		}).AddTo(this);
	}

	private void CheckToRoad()
	{
		if (_isUser && !IsLemanFinished)
		{
			return;
		}
		if (_startTimeCheckToRoad == 0f)
		{
			_startTimeCheckToRoad = Time.time;
		}
		if (Time.time - _startTimeCheckToRoad < 5f || !_powertrain || !(_powertrain.SpeedKmh < 5f))
		{
			return;
		}
		float magnitude = (_lastSplinePosition - base.transform.position).magnitude;
		if (_circuitCarStearing.GetCurrentState() != CarSteeringState.OnPitStop && _circuitCarStearing.GetCurrentState() != CarSteeringState.Stop)
		{
			bool flag = magnitude >= 100f || _isOutOfRoad;
			bool flag2 = _circuitRaceControlller.GetRaceState() == RaceState.RaceFinished;
			bool isLemanFinished = IsLemanFinished;
			if (((flag2 & isLemanFinished) || (!flag2 && !isLemanFinished)) && flag && !IsBroken)
			{
				ToRoad();
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 22 && !_circuitRaceControlller.IsRaceFinished() && !IsLemanFinished)
		{
			CheckCollisionSide(collision);
		}
		if (!_isUser || Time.time - _timeCrash < 3f)
		{
			return;
		}
		if (collision.contactCount > 0)
		{
			for (int i = 0; i < collision.contactCount; i++)
			{
				ContactPoint contact = collision.GetContact(i);
				bool num = base.transform.InverseTransformPoint(contact.point).y < 0f;
				bool flag = Vector3.Dot(contact.normal, base.transform.up) > 0f;
				if (num & flag)
				{
					return;
				}
			}
		}
		_timeCrash = Time.time;
		float num2 = collision.relativeVelocity.magnitude;
		if (!(num2 < 5f) && collision.gameObject.layer != LayerViaName.RoadLayer)
		{
			if ((bool)collision.gameObject.GetComponent<Powertrain>())
			{
				num2 *= 1.5f;
			}
			float value = num2 * 0.015f;
			value = Mathf.Clamp(value, 0.2f, 1f);
			_racingCameraController.StopCameraShake();
			_racingCameraController.SetCameraShake(amplCrash * num2, num2 * gainCrash, value);
		}
	}

	private void CheckCollisionSide(Collision col)
	{
		float num = Vector3.Angle(col.GetContact(0).normal, Vector3.up);
		float magnitude = col.relativeVelocity.magnitude;
		if (num > 90f && magnitude > _circuitRaceControlller.GetMinCrashValue() && _isUser)
		{
			if (_circuitRaceControlller.GetRaceMapModel().RaceConfig.IsLeman)
			{
				AddPenaltyDistance(_circuitRaceControlller.GetPenaltyCrashMetersVar());
				_circuitRaceControlller.CircuitRacingUIControll.ShowCrashWithDistanceMessage(_circuitRaceControlller.GetPenaltyCrashMetersVar());
			}
			else
			{
				_penaltyCrashSecounds += _circuitRaceControlller.GetPenaltyCrashSecsVar();
				_circuitRaceControlller.CircuitRacingUIControll.ShowCrashWithTimeMessage(_circuitRaceControlller.GetPenaltyCrashSecsVar());
			}
		}
	}

	public void EndPitStop()
	{
		_circuitCarStearing.ChangeSteeringState(CarSteeringState.GoToPitStop, IsCarBroken: true);
	}

	public void SetLemanAchievment()
	{
		if (_circuitRaceControlller.IsCurrentMapLeman() && place <= 2)
		{
			if (PitStopCount == 1)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQMw", "le_mans_race");
				_circuitRaceControlller.UnlockSplashAndDash();
			}
			if (PitStopCount == 0 && IsFuelTankEmpty())
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQOQ", "le_mans_race");
				_circuitRaceControlller.UnlockRunningOnFumes();
			}
		}
	}

	private bool IsFuelTankEmpty()
	{
		if ((bool)_powertrain)
		{
			return _powertrain.CurrentFuel < 0.001f;
		}
		return false;
	}

	public void StartPitStop()
	{
		_circuitRaceControlller.circuitCarsControl.StopCar(base.gameObject);
		PitStopCount++;
		if (_isUser)
		{
			_circuitRaceControlller.ShowPitStopMenu();
			return;
		}
		_circuitCarStearing.ChangeSteeringState(CarSteeringState.Stop);
		DelayedPitStopEnd().Forget();
	}

	public Transform GetMiniMapTransform()
	{
		if (!_miniMapTransform)
		{
			GameObject gameObject = new GameObject("MiniMapTransform");
			_miniMapTransform = gameObject.transform;
			_miniMapTransform.SetParent(base.transform);
			_miniMapTransform.localPosition = Vector3.zero;
			_miniMapTransform.localRotation = Quaternion.identity;
			_miniMapTransform.localScale = Vector3.one;
			gameObject.AddComponent<CarMiniMapTransformHelper>();
		}
		return _miniMapTransform;
	}

	private async UniTaskVoid DelayedPitStopEnd()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(2.0));
		if (!_isDestroyed)
		{
			_vehicleEntityProvider.Refuel(_carDefaultTuningDataConfig.ChassisData.FuelTankCapacity);
			if (!_powertrain.IsEngineWorks)
			{
				_powertrain.StartEngine(starterSound: false);
			}
			_circuitCarStearing.HardChangeSteeringState(CarSteeringState.GoToPitStop, resetWaypoint: true);
		}
	}

	public void StopPitStop()
	{
		if (_isUser)
		{
			_circuitRaceControlller.RaceCrewTalkSoundController.PlayPitStopSoundEvent(MaintenanceType.None);
			_circuitRaceControlller.StopPitStopMenu();
		}
		else
		{
			IsBroken = false;
		}
	}

	private void CheckMaintenance()
	{
		float num = (float)_carDefaultTuningDataConfig.ChassisData.FuelTankCapacity / 100f;
		float num2 = _vehicleEntityProvider.CarEntity.MaintenanceInfo.FuelInTank / num;
		if (_isUser)
		{
			float left = 0f;
			float right = 0f;
			float left2 = 0f;
			float right2 = 0f;
			_axles.GetTyreHealth(_axles.FrontAxle, out left, out right);
			_axles.GetTyreHealth(_axles.RearAxle, out left2, out right2);
			int wheelHealthPercent = (int)((left + right + left2 + right2) / 4f * 100f);
			_circuitRaceControlller.CheckMaintenance(this, wheelHealthPercent, (int)num2);
		}
		else if ((int)num2 < 30)
		{
			IsBroken = true;
		}
	}

	private void DetectStuck()
	{
		if (_recoveryCooldown > Time.time || IsBroken || IsLemanFinished || _isRaceFinish)
		{
			return;
		}
		CarSteeringState currentState = _circuitCarStearing.GetCurrentState();
		if (currentState == CarSteeringState.Race || currentState == CarSteeringState.GoToPitStop)
		{
			if (_rigidbody.velocity.magnitude < 0.5f)
			{
				_stuckTimer += Time.fixedDeltaTime;
			}
			else
			{
				_stuckTimer = 0f;
			}
			if (_stuckTimer > 1.5f)
			{
				_stuckTimer = 0f;
				ToRoad();
			}
		}
	}

	public void SetWheelsTemperature()
	{
		if (!_axles)
		{
			return;
		}
		foreach (WheelProvider allWheel in _axles.AllWheels)
		{
			allWheel.Temperature = 125f;
		}
	}
}
