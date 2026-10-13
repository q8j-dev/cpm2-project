using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Rally.Data;

public class RallySeqenceTriggerLine : MonoBehaviour
{
	[SerializeField]
	private float TriggerCoolDown = 3f;

	[SerializeField]
	private BoxCollider Collider;

	private RallySequenceItem _rallySequenceItem;

	private GameObject _mainCar;

	private float _totalDistance;

	private RallySeqenceTriggerLine _nextRallySeqenceTriggerLine;

	private RallySeqenceTriggerLine _previousActiveTrigger;

	private float _progress;

	private bool _isPassed;

	private IDisposable _distanceCounter;

	private CancellationTokenSource _reEnableCts;

	private Transform[] _segmentControlTransforms;

	private readonly Vector3[] _controlPoints = new Vector3[4];

	private float _roadHalfWidth = 25f;

	private const int CurveResolution = 20;

	public Action<bool> OnActivated;

	private float _lastPassTime;

	public RallySequenceItem RallySequenceItem => _rallySequenceItem;

	public bool IsActive { get; private set; }

	public int TriggerIndex { get; private set; }

	public event Action<float> ProgressTick;

	public void SetIndex(int index)
	{
		TriggerIndex = index;
	}

	public bool HasMark(RallySequenceMark mark)
	{
		if (_rallySequenceItem?.Mark == null)
		{
			return false;
		}
		RallySequenceMark[] mark2 = _rallySequenceItem.Mark;
		for (int i = 0; i < mark2.Length; i++)
		{
			if (mark2[i] == mark)
			{
				return true;
			}
		}
		return false;
	}

	public void Init(RallySequenceItem rallySequenceItem)
	{
		_rallySequenceItem = rallySequenceItem;
		_totalDistance = rallySequenceItem.Distance;
		if (Collider != null && rallySequenceItem.TriggerSize.Size != Vector3.zero)
		{
			Collider.size = rallySequenceItem.TriggerSize.Size;
			Collider.center = rallySequenceItem.TriggerSize.Center;
			_roadHalfWidth = Mathf.Max(rallySequenceItem.TriggerSize.Size.x * 0.5f, _roadHalfWidth);
			Collider.enabled = false;
		}
	}

	public void SetNextTrigger(RallySeqenceTriggerLine next)
	{
		_nextRallySeqenceTriggerLine = next;
	}

	public void SetSegmentControlPoints(Transform prev, Transform start, Transform end, Transform nextEnd)
	{
		_segmentControlTransforms = new Transform[4] { prev, start, end, nextEnd };
	}

	private Vector3[] GetControlPoints()
	{
		if (_segmentControlTransforms == null)
		{
			return null;
		}
		for (int i = 0; i < _controlPoints.Length; i++)
		{
			Transform transform = _segmentControlTransforms[i];
			if (transform == null)
			{
				return null;
			}
			_controlPoints[i] = transform.position;
		}
		return _controlPoints;
	}

	private static Vector3 CatmullRomPosition(float t, Vector3[] p)
	{
		float num = t * t;
		float num2 = num * t;
		return 0.5f * (2f * p[1] + (-p[0] + p[2]) * t + (2f * p[0] - 5f * p[1] + 4f * p[2] - p[3]) * num + (-p[0] + 3f * p[1] - 3f * p[2] + p[3]) * num2);
	}

	private float ProjectCarOntoSegment()
	{
		if (_mainCar == null)
		{
			return -1f;
		}
		Vector3[] controlPoints = GetControlPoints();
		if (controlPoints == null)
		{
			return -1f;
		}
		float num = float.MaxValue;
		float result = 0f;
		for (int i = 0; i <= 20; i++)
		{
			float num2 = (float)i / 20f;
			float num3 = Vector3.Distance(_mainCar.transform.position, CatmullRomPosition(num2, controlPoints));
			if (num3 < num)
			{
				num = num3;
				result = num2;
			}
		}
		return result;
	}

	public void Activate()
	{
		IsActive = true;
		if (Collider != null)
		{
			Collider.enabled = true;
		}
	}

	public void RegisterCar(GameObject car)
	{
		_mainCar = car;
	}

	public void GetClosestPointOnCurve(Vector3 worldPos, out Vector3 closestPoint, out Quaternion rotation)
	{
		closestPoint = base.transform.position;
		rotation = base.transform.rotation;
		Vector3[] controlPoints = GetControlPoints();
		if (controlPoints == null)
		{
			return;
		}
		float num = float.MaxValue;
		int num2 = 0;
		for (int i = 0; i <= 20; i++)
		{
			Vector3 vector = CatmullRomPosition((float)i / 20f, controlPoints);
			float num3 = worldPos.x - vector.x;
			float num4 = worldPos.z - vector.z;
			float num5 = num3 * num3 + num4 * num4;
			if (num5 < num)
			{
				num = num5;
				num2 = i;
			}
		}
		float num6 = (float)num2 / 20f;
		closestPoint = CatmullRomPosition(num6, controlPoints);
		Vector3 normalized = (CatmullRomPosition(Mathf.Min(1f, num6 + 0.05f), controlPoints) - closestPoint).normalized;
		if (normalized != Vector3.zero)
		{
			rotation = Quaternion.LookRotation(normalized, Vector3.up);
		}
	}

	public void CompleteProgress()
	{
		_distanceCounter?.Dispose();
		ProgressTick?.Invoke(1f);
		OnActivated?.Invoke(obj: false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(_mainCar == null) && !(other.gameObject != _mainCar) && !(Time.time - _lastPassTime <= TriggerCoolDown))
		{
			_lastPassTime = Time.time;
			if (other.TryGetComponent<IRaceCar>(out var _) && !_isPassed)
			{
				ProcessCrossing();
			}
		}
	}

	private void ProcessCrossing()
	{
		_isPassed = true;
		IsActive = false;
		if (Collider != null)
		{
			Collider.enabled = false;
		}
		_previousActiveTrigger?.CompleteProgress();
		_progress = 0f;
		OnActivated?.Invoke(obj: true);
		if (_nextRallySeqenceTriggerLine != null)
		{
			_distanceCounter?.Dispose();
			_distanceCounter = Observable.EveryUpdate().Subscribe((long _) =>
			{
				TrackToNext();
			}).AddTo(this);
			_nextRallySeqenceTriggerLine._previousActiveTrigger = this;
			_nextRallySeqenceTriggerLine.Activate();
			ReEnableCollider().Forget();
		}
		else
		{
			ProgressTick?.Invoke(1f);
			OnActivated?.Invoke(obj: false);
		}
	}

	private async UniTaskVoid ReEnableCollider()
	{
		_reEnableCts?.Cancel();
		_reEnableCts?.Dispose();
		_reEnableCts = new CancellationTokenSource();
		if (!(await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, _reEnableCts.Token).SuppressCancellationThrow()) && !(this == null) && Collider != null)
		{
			Collider.enabled = true;
		}
	}

	public void ResetPassedState()
	{
		_isPassed = false;
		_distanceCounter?.Dispose();
		_distanceCounter = null;
		_previousActiveTrigger = null;
		_reEnableCts?.Cancel();
		_reEnableCts?.Dispose();
		_reEnableCts = null;
		if (Collider != null)
		{
			Collider.enabled = false;
		}
	}

	private void TrackToNext()
	{
		if (_mainCar == null || _nextRallySeqenceTriggerLine == null)
		{
			return;
		}
		float num = ProjectCarOntoSegment();
		if (num >= 0f)
		{
			_progress = num;
		}
		else
		{
			if (_totalDistance <= 0f)
			{
				return;
			}
			float num2 = Vector3.Distance(_mainCar.transform.position, _nextRallySeqenceTriggerLine.transform.position);
			_progress = Mathf.Clamp01(1f - num2 / _totalDistance);
		}
		ProgressTick?.Invoke(_progress);
	}

	private void OnDestroy()
	{
		_distanceCounter?.Dispose();
		_reEnableCts?.Cancel();
		_reEnableCts?.Dispose();
	}
}
