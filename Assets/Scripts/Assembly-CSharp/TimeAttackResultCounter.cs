using System;
using System.Collections.Generic;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.EventSystem;

public class TimeAttackResultCounter : IRaceResultCounter, IDisposable
{
	private const float PenaltyModifier = 10f;

	private const float OutOfRoadPenaltyModifier = 2f;

	private float _penaltyTime;

	private float _finalPenaltyTime;

	private float _currentTime;

	private int _ctCheck;

	private int _ptCheck;

	private int _fptCheck;

	private int _cf;

	private bool _cantMerge;

	private float _timestamp;

	private Axles _axles;

	private Powertrain _powertrain;

	private FreeDriveControl _freeDriveControl;

	private IDisposable _carSubscription;

	private IDisposable _penaltySubscription;

	private IDisposable _tick;

	private FreeRaceCar _freeRaceCar;

	private FreeTimeAttackController _freeTimeAttackController;

	private EventRestrictionService _eventRestrictionService;

	[UnityEngine.Scripting.Preserve]
	[Inject]
	public void Construct(FreeDriveControl freeDriveControl, FreeTimeAttackController timeAttackController, EventRestrictionService eventRestrictionService)
	{
		_freeDriveControl = freeDriveControl;
		_freeTimeAttackController = timeAttackController;
		_eventRestrictionService = eventRestrictionService;
	}

	public void OnStartRace()
	{
		_timestamp = Time.time;
		_cf = UnityEngine.Random.Range(3, 40);
		_ctCheck = 0;
		_ptCheck = 0;
		_fptCheck = 0;
	}

	public void Init(FreeRaceCar freeRaceCar)
	{
		_freeRaceCar = freeRaceCar;
		_powertrain = freeRaceCar.GetComponent<Powertrain>();
		_axles = freeRaceCar.GetComponent<Axles>();
		FreeDriveControl freeDriveControl = _freeDriveControl;
		freeDriveControl.OnCarToRoad = (Action<GameObject>)Delegate.Combine(freeDriveControl.OnCarToRoad, new Action<GameObject>(OnCarToRoad));
		_carSubscription = Observable.Interval(TimeSpan.FromSeconds(1.0)).DelaySubscription(TimeSpan.FromSeconds(0.5)).Subscribe((long _) =>
		{
			CheckPenaltyTime();
		});
		_penaltySubscription = Observable.Interval(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			MergePenalty();
		});
		_tick = Observable.EveryUpdate().Subscribe((long _) =>
		{
			Tick();
		});
	}

	public float GetFinalResult()
	{
		if (_cf != 0)
		{
			float num = (float)_ctCheck / (1000f * (float)_cf);
			if (num > 0f && Mathf.Abs(_currentTime - num) > num * 0.15f)
			{
				_currentTime = num;
			}
			float num2 = (float)_ptCheck / (1000f * (float)_cf);
			if (Mathf.Abs(_penaltyTime - num2) > Mathf.Max(num2 * 0.15f, 0.5f))
			{
				_penaltyTime = num2;
			}
			float num3 = (float)_fptCheck / (1000f * (float)_cf);
			if (Mathf.Abs(_finalPenaltyTime - num3) > Mathf.Max(num3 * 0.15f, 0.5f))
			{
				_finalPenaltyTime = num3;
			}
		}
		_penaltyTime = Mathf.Abs(_penaltyTime);
		_finalPenaltyTime = Mathf.Abs(_finalPenaltyTime);
		_currentTime = Mathf.Abs(_currentTime);
		_currentTime = _currentTime + _penaltyTime + _finalPenaltyTime;
		_timestamp = Time.time - _timestamp + _penaltyTime + _finalPenaltyTime;
		return _currentTime;
	}

	public float GetCurrentResult()
	{
		return _currentTime;
	}

	private void Tick()
	{
		if (_freeRaceCar.IsRaceStarted)
		{
			_currentTime += Time.deltaTime;
			_ctCheck += (int)(Time.deltaTime * 1000f * (float)_cf);
			_freeRaceCar.UserTimeAction?.Invoke(_currentTime, _penaltyTime, _finalPenaltyTime);
		}
	}

	public void Restart()
	{
		_currentTime = 0f;
		_penaltyTime = 0f;
		_finalPenaltyTime = 0f;
		_ctCheck = 0;
		_ptCheck = 0;
		_fptCheck = 0;
		_cf = UnityEngine.Random.Range(3, 40);
		_powertrain.ResetTransmission();
		_axles.SetTyreHealth(_axles.FrontAxle, 1f, _axles.FrontAxle.LeftWheel.TyreType);
		_axles.SetTyreHealth(_axles.RearAxle, 1f, _axles.RearAxle.LeftWheel.TyreType);
	}

	private void CheckPenaltyTime()
	{
		int num = 0;
		for (int i = 0; i < _axles.AllWheels.Count; i++)
		{
			WheelProvider wheelProvider = _axles.AllWheels[i];
			if (!IsSurfaceRequired(wheelProvider.WheelSurface))
			{
				num++;
			}
		}
		if (num > 2 && _powertrain.SpeedKmh > 1f && _powertrain.OnGround)
		{
			_cantMerge = true;
			AddPenalty(2f);
		}
		else
		{
			_cantMerge = false;
		}
	}

	private bool IsSurfaceRequired(Surface surface)
	{
		if (_freeTimeAttackController.TimeAttackMode == TimeAttackMode.Rally)
		{
			return true;
		}
		Surface[] requiredSurfaces = _freeTimeAttackController.CurrentTimeAttackData.RequiredSurfaces;
		for (int i = 0; i < requiredSurfaces.Length; i++)
		{
			if (requiredSurfaces[i] == surface)
			{
				return true;
			}
		}
		return false;
	}

	private void MergePenalty()
	{
		if (!_cantMerge)
		{
			_finalPenaltyTime += _penaltyTime;
			_fptCheck += _ptCheck;
			_penaltyTime = 0f;
			_ptCheck = 0;
		}
	}

	public void AddPenalty(float penalty)
	{
		if (!(penalty < 0f))
		{
			_penaltyTime += penalty;
			_ptCheck += (int)(penalty * 1000f * (float)_cf);
		}
	}

	public void OnCarToRoad(GameObject car)
	{
		_penaltyTime += 10f;
		_ptCheck += (int)(10000f * (float)_cf);
	}

	public TimeAttackAnalyticData GetAnalyticData()
	{
		TimeAttackAnalyticData timeAttackAnalyticData = new TimeAttackAnalyticData();
		List<VehicleClass> vehicleClasses = _eventRestrictionService.GetVehicleClasses(_freeTimeAttackController.CurrentTimeAttackData.EventType);
		string requiredCarClasses = ((vehicleClasses != null) ? string.Join(",", vehicleClasses) : string.Empty);
		timeAttackAnalyticData.EventVariant = _freeTimeAttackController.CurrentTimeAttackData.EventType.ToString();
		timeAttackAnalyticData.RequiredCarClasses = requiredCarClasses;
		timeAttackAnalyticData.TrackLayoutId = $"{_freeTimeAttackController.CurrentTimeAttackData.EventType}_{_freeTimeAttackController.CurrentTrackIndex}";
		timeAttackAnalyticData.TrajectoryId = _freeTimeAttackController.CurrentTrajectoryIndex.ToString();
		timeAttackAnalyticData.TtdCode = Mathf.FloorToInt(Mathf.Abs(_currentTime - _timestamp));
		return timeAttackAnalyticData;
	}

	public void Dispose()
	{
		FreeDriveControl freeDriveControl = _freeDriveControl;
		freeDriveControl.OnCarToRoad = (Action<GameObject>)Delegate.Remove(freeDriveControl.OnCarToRoad, new Action<GameObject>(OnCarToRoad));
		_carSubscription?.Dispose();
		_penaltySubscription?.Dispose();
		_tick?.Dispose();
	}
}
