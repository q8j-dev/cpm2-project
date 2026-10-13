using System;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Analytics.Analyzers;

namespace Car_Parking.Scripts.Racing
{
	public class DriftController : MonoBehaviour
	{
		public enum DriftState
		{
			No = 0,
			Drift = 1,
			Change = 2
		}

		private const float MinComboCount = 0f;

		private const float MaxComboCount = 50f;

		private const float MinCrashStrengh = 10f;

		[SerializeField]
		protected Powertrain Car;

		[SerializeField]
		protected float ComboReadyTimeLimit = 2f;

		[SerializeField]
		protected float DriftNoDriftTimeLimit = 3f;

		[SerializeField]
		protected float DriftBeginPointsTimeLimit = 1f;

		[SerializeField]
		protected float DriftBeginVelocityLimit = 30f;

		[SerializeField]
		protected float DriftResetCooldown = 4f;

		[SerializeField]
		protected float ComboStep = 0.1f;

		[SerializeField]
		protected float ComboStepMaxValue = 1000f;

		[SerializeField]
		protected float PointsForChange = 300f;

		public float CurrentComboCount;

		private float _driftMultiplyValue = 0.8f;

		private DriftState _driftState;

		private bool _drift;

		private bool _fastDrift;

		private float _driftAng;

		private float _noDriftTime;

		private float _driftTime;

		private float _driftTimeSide;

		private int _prevSign;

		private int _currentSignForShow;

		private int _numCombo;

		private bool _combo;

		private bool _stopped;

		private float _points;

		private long _pCheck;

		private long _tCheck;

		private int _cf;

		private float _lowVelocityTime;

		private DriftAnalyticData _analyticData;

		public Action DriftFailedEvent;

		public Action<bool> EarnedPointsEvent;

		public Action<float> OnComboAdded;

		private float _totalDriftPoints;

		public DriftState CurrentState => _driftState;

		public DriftAnalyticData DriftAnalyticData => _analyticData;

		public float comboReadyTimeLimit
		{
			get
			{
				return ComboReadyTimeLimit;
			}
			set
			{
				ComboReadyTimeLimit = value;
			}
		}

		public float DriftAngle => _driftAng;

		public int CurrentSign => _currentSignForShow;

		public float driftBeginPointsTimeLimit
		{
			get
			{
				return DriftBeginPointsTimeLimit;
			}
			set
			{
				DriftBeginPointsTimeLimit = value;
			}
		}

		public float driftBeginVelocityLimit
		{
			get
			{
				return DriftBeginVelocityLimit;
			}
			set
			{
				DriftBeginVelocityLimit = value;
			}
		}

		public float driftNoDriftTimeLimit
		{
			get
			{
				return DriftNoDriftTimeLimit;
			}
			set
			{
				DriftNoDriftTimeLimit = value;
			}
		}

		public float DriftTime => _driftTime;

		public float DriftTimeSide => _driftTimeSide;

		public bool IsCombo => _combo;

		public bool IsDrift => _drift;

		public bool IsFastDrift => _fastDrift;

		public int NumCombo => _numCombo;

		public float Points => _points;

		public float TotalDriftPoints => _totalDriftPoints;

		public void ChangeDriftMultiplyValue(float value)
		{
			_driftMultiplyValue = value;
		}

		private void Awake()
		{
			if (!Car)
			{
				Car = GetComponent<Powertrain>();
			}
		}

		private void Start()
		{
			_analyticData = new DriftAnalyticData();
		}

		private void DriftFail()
		{
			_currentSignForShow = 0;
			CurrentComboCount = 1f;
			_driftState = DriftState.No;
			EarnPoints(resetCombo: true);
			_points = 0f;
			_pCheck = 0L;
			_drift = false;
			_fastDrift = false;
			_combo = false;
			_driftTime = 0f;
			_driftTimeSide = 0f;
			_noDriftTime = 0f;
			_lowVelocityTime = 0f;
			DriftFailedEvent?.Invoke();
		}

		private void Update()
		{
			if (Time.timeScale == 0f || !Car)
			{
				return;
			}
			if (_stopped)
			{
				_stopped = false;
				return;
			}
			float deltaTime = Time.deltaTime;
			Vector3 forward = Car.transform.forward;
			Vector3 velocity = Car.Velocity;
			float speedMps = Car.SpeedMps;
			Vector3 lhs = velocity;
			float num = 0f;
			float num2 = 0f;
			if (speedMps > 0.01f)
			{
				lhs.Normalize();
				num2 = Vector3.Dot(lhs, forward);
				num = GetAngle();
			}
			float num3 = speedMps * 3.6f;
			_combo = false;
			if (num3 <= DriftBeginVelocityLimit)
			{
				_lowVelocityTime += deltaTime;
				if (_lowVelocityTime >= DriftResetCooldown && _driftState != DriftState.No)
				{
					_analyticData.DriftBreakData.NoDriftTimeOut++;
					_analyticData.DriftBreakData.LowSpeedCount++;
					DriftFail();
				}
			}
			else
			{
				_lowVelocityTime = 0f;
			}
			float b = 10f;
			float t = Mathf.Clamp01((num3 - 30f) / 90f);
			float num4 = Mathf.Lerp(15f, b, t);
			_driftAng = Mathf.Abs(num);
			switch (_driftState)
			{
			case DriftState.No:
				_noDriftTime = 0f;
				_prevSign = 0;
				_driftTime = 0f;
				_driftTimeSide = 0f;
				_numCombo = 0;
				_lowVelocityTime = 0f;
				if (_points > 0f && EarnedPointsEvent != null)
				{
					EarnPoints();
				}
				_points = 0f;
				_pCheck = 0L;
				CurrentComboCount = 1f;
				if (num3 > DriftBeginVelocityLimit && _driftAng > num4 && num2 > 0f)
				{
					_driftState = DriftState.Drift;
				}
				break;
			case DriftState.Drift:
				_driftTime += deltaTime;
				_driftTimeSide += deltaTime;
				if (_driftAng >= num4 || num2 <= 0f)
				{
					_prevSign = ((num >= 0f) ? 1 : (-1));
				}
				else
				{
					_driftState = DriftState.Change;
					_noDriftTime = 0f;
				}
				if (num2 < -0.5f)
				{
					_analyticData.DriftBreakData.RotationCount++;
					DriftFail();
				}
				_currentSignForShow = _prevSign;
				break;
			case DriftState.Change:
				_driftTime += deltaTime;
				_noDriftTime += deltaTime;
				if (_noDriftTime > DriftNoDriftTimeLimit)
				{
					_driftState = DriftState.No;
				}
				else if (_driftAng > num4 && num2 > 0f)
				{
					_driftState = DriftState.Drift;
					int num5 = ((num >= 0f) ? 1 : (-1));
					if (num5 == _prevSign || _driftTimeSide <= ComboReadyTimeLimit)
					{
						_numCombo = 0;
					}
					else
					{
						_combo = true;
						_numCombo++;
						_analyticData.DriftChangeComboCount++;
						OnComboAdded?.Invoke(PointsForChange);
					}
					_currentSignForShow = num5;
					_driftTimeSide = 0f;
				}
				break;
			}
			_drift = (_driftState == DriftState.Drift || _driftState == DriftState.Change) && _driftTime > DriftBeginPointsTimeLimit;
			_fastDrift = _drift && num3 > 120f;
			if (_driftState == DriftState.Drift && _driftTime > DriftBeginPointsTimeLimit)
			{
				float num6 = Mathf.Max(_driftAng, 10f) * num3 * deltaTime * _driftMultiplyValue;
				float num7 = ((_numCombo <= 1) ? 0f : 1f);
				float num8 = num7;
				CurrentComboCount = Mathf.Clamp(_points / ComboStepMaxValue + 1f, 0f, 50f);
				if (CurrentComboCount > (float)_analyticData.DriftComboXMax)
				{
					_analyticData.DriftComboXMax = (int)CurrentComboCount;
				}
				float num9 = num6 + num8 * PointsForChange;
				_points += num9;
				_pCheck += (long)(num9 * (float)_cf);
				_numCombo = 0;
			}
		}

		private float GetAngle()
		{
			Vector3 vector = base.transform.InverseTransformDirection(Car.Velocity);
			return Mathf.Atan2(vector.x, Mathf.Abs(vector.z)) * 57.29578f;
		}

		private void EarnPoints(bool resetCombo = false)
		{
			float num = CurrentComboCount;
			if ((num < 1f) | resetCombo)
			{
				num = 1f;
			}
			if (_cf != 0)
			{
				float num2 = (float)_pCheck / (float)_cf;
				if (num2 > 0f && Mathf.Abs(_points - num2) > num2 * 0.15f)
				{
					_points = num2;
				}
				float num3 = (float)_tCheck / (float)_cf;
				if (num3 > 0f && Mathf.Abs(_totalDriftPoints - num3) > num3 * 0.15f)
				{
					_totalDriftPoints = num3;
				}
			}
			_analyticData.DriftBaseScoreTotal += (int)_points;
			_totalDriftPoints += _points * num;
			_tCheck += (long)((float)_pCheck * num);
			_analyticData.DriftScoreTotal = (int)_totalDriftPoints;
			EarnedPointsEvent?.Invoke(resetCombo);
		}

		public void ForceEarnPoints()
		{
			EarnPoints();
			if (!(_points > 0f))
			{
				return;
			}
			_points = 0f;
			_pCheck = 0L;
			if (_cf != 0)
			{
				float num = (float)_tCheck / (float)_cf;
				if (num > 0f && Mathf.Abs(_totalDriftPoints - num) > num * 0.15f)
				{
					_totalDriftPoints = num;
					_analyticData.DriftScoreTotal = (int)_totalDriftPoints;
				}
			}
		}

		private void OnEnable()
		{
			_driftState = DriftState.No;
			_drift = false;
			_fastDrift = false;
			_driftAng = 0f;
			_noDriftTime = 0f;
			_driftTime = 0f;
			_driftTimeSide = 0f;
			_prevSign = 0;
			_combo = false;
			_numCombo = 0;
			_stopped = false;
			_points = 0f;
			_lowVelocityTime = 0f;
			_cf = UnityEngine.Random.Range(7, 31);
			_pCheck = 0L;
			_tCheck = 0L;
		}

		public void StopDrift()
		{
			if (_driftState != DriftState.No)
			{
				DriftFail();
			}
			_stopped = true;
		}

		public void ResetDrift()
		{
			_totalDriftPoints = 0f;
			_tCheck = 0L;
			StopDrift();
		}

		public void DestroyComponent()
		{
			UnityEngine.Object.Destroy(this);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (base.enabled && collision != null && (_driftState == DriftState.No || (collision.gameObject.layer != LayerViaName.DriverPersonLayer && collision.gameObject.layer != LayerViaName.RoadLayer)) && collision.relativeVelocity.magnitude > 10f)
			{
				_analyticData.DriftBreakData.CollisionCount++;
				StopDrift();
			}
		}
	}
}
