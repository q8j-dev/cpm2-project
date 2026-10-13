using CPMEngine;
using UniRx;
using UnityEngine;

public class Dragy : MonoBehaviour
{
	public enum LostReason
	{
		none = 0,
		slow = 1,
		falseStart = 2,
		fasterClass = 3,
		carAccident = 4,
		timeOut = 5,
		outOfRoad = 6
	}

	public float StartTime;

	public float lapTime;

	public float StartTimeOnStationary;

	public float reactionTime;

	public float kmh100Result;

	public float kmh100200Result;

	public float mph60Result;

	public float quarterMileResult;

	public float halfMileResult;

	public bool mph60measured;

	public float ft60;

	public float ft330;

	public float et1_8;

	public bool kmh100measured;

	public bool m402measured;

	public bool kmh100200measured;

	public bool ResetDone;

	public Vector3 startPoint;

	public int falseStartCount;

	public bool lost;

	public LostReason reason;

	public float exitSpeed;

	public float timeWhen100Reached;

	public bool lostButFor3place;

	public float kmh100slope;

	public float mph60Slope;

	public float m402Result;

	public float m402Slope;

	public float kmh100200Slope;

	private bool _checkMaxSpeed;

	public float BrakingDistance;

	private Powertrain _powertrain;

	private Axles _axles;

	private Vector3 _startBrakingPosition;

	public float maxSpeed;

	public bool CheckMaxSpeed
	{
		get
		{
			return _checkMaxSpeed;
		}
		set
		{
			_checkMaxSpeed = value;
			if (_checkMaxSpeed)
			{
				maxSpeed = 0f;
			}
		}
	}

	public bool MeasureSplits { get; set; }

	private void Awake()
	{
		_powertrain = GetComponent<Powertrain>();
		_axles = GetComponent<Axles>();
	}

	private void OnEnable()
	{
		if ((bool)_axles)
		{
			_axles.IsBrakePressed.Subscribe(CalculateBrakeDistance).AddTo(this);
		}
	}

	public float GetResult(bool isQuaterMile)
	{
		if (!isQuaterMile)
		{
			return halfMileResult;
		}
		return quarterMileResult;
	}

	public float on402Reached()
	{
		float result = (quarterMileResult = Time.time - StartTime);
		exitSpeed = _powertrain.SpeedKmh;
		m402Result = result;
		return result;
	}

	public float on800mReached()
	{
		float result = (halfMileResult = Time.time - StartTime);
		exitSpeed = _powertrain.SpeedKmh;
		return result;
	}

	public void SetReactionTime(float time)
	{
		reactionTime = time;
		if (time < 0f)
		{
			falseStartCount++;
		}
	}

	public float on60mhpReached()
	{
		mph60Result = Time.time - StartTimeOnStationary;
		return mph60Result;
	}

	public float on100kmhReached()
	{
		kmh100Result = Time.time - StartTimeOnStationary;
		return kmh100Result;
	}

	public float on100200kmhTriggerd()
	{
		kmh100200Result = Time.time - timeWhen100Reached;
		return kmh100200Result;
	}

	public void onCarStop()
	{
		StartTimeOnStationary = Time.time;
		startPoint = base.transform.position;
		mph60measured = false;
		kmh100measured = false;
		kmh100200measured = false;
	}

	private void Update()
	{
		if (_powertrain.IsRaceCar && !MeasureSplits)
		{
			return;
		}
		float speedMps = _powertrain.SpeedMps;
		float speedKmh = _powertrain.SpeedKmh;
		float speedMph = _powertrain.SpeedMph;
		if (speedMps < 0.02f)
		{
			onCarStop();
		}
		if (_checkMaxSpeed && speedKmh > maxSpeed)
		{
			maxSpeed = speedKmh;
		}
		if (!mph60measured && speedMph > 60f)
		{
			mph60measured = true;
			on60mhpReached();
		}
		if (!kmh100measured && speedKmh > 100f)
		{
			kmh100measured = true;
			on100kmhReached();
		}
		if (speedKmh > 100f)
		{
			if (timeWhen100Reached == 0f)
			{
				timeWhen100Reached = Time.time;
			}
		}
		else
		{
			timeWhen100Reached = 0f;
		}
		if (speedKmh > 200f && timeWhen100Reached != -1f)
		{
			on100200kmhTriggerd();
			timeWhen100Reached = -1f;
		}
		float num = Vector3.Distance(base.transform.position, startPoint);
		if (!m402measured && num > 402f)
		{
			m402measured = true;
			on402Reached();
		}
		if (ft60 == 0f && num > 18.1f)
		{
			ft60 = Time.time - StartTimeOnStationary;
		}
		if (ft330 == 0f && num > 99.8f)
		{
			ft330 = Time.time - StartTimeOnStationary;
		}
		if (et1_8 == 0f && num > 201f)
		{
			et1_8 = Time.time - StartTimeOnStationary;
		}
	}

	private void CalculateBrakeDistance(bool brakePressed)
	{
		if (brakePressed)
		{
			_startBrakingPosition = base.transform.position;
		}
		else if (_powertrain.SpeedMps < 0.1f)
		{
			BrakingDistance = Vector3.Distance(_startBrakingPosition, base.transform.position);
		}
	}

	public void Reset()
	{
		reactionTime = 0f;
		kmh100Result = 0f;
		mph60Result = 0f;
		quarterMileResult = 0f;
		halfMileResult = 0f;
		ft60 = 0f;
		ft330 = 0f;
		et1_8 = 0f;
		lostButFor3place = false;
		onCarStop();
	}
}
