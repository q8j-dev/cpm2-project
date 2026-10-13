using System;
using System.Collections.Generic;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Rally.Data;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyCar : MonoBehaviour, IRaceCar
	{
		private const int ReportCollisionsCount = 25;

		private const float CollisionTimeout = 5f;

		private const float OffRoadWarningCooldown = 10f;

		private const int RequredMissCheckpointsCount = 2;

		private Rigidbody _rb;

		private RallyController _rallyController;

		private Powertrain _powertrain;

		private RallyTriggersControl _triggersControl;

		private RaceGaragePoint _stagePoint;

		private float _raceTime;

		private float _penalty;

		private readonly List<PenaltyRecord> _penaltyLog = new List<PenaltyRecord>();

		private IDisposable _timerDisposable;

		private int _collisionCount;

		[SerializeField]
		private float DefaultRoadMargin = 10f;

		[SerializeField]
		private float OffRoadPenaltyInterval = 5f;

		[SerializeField]
		private float OffRoadDisqualifyTime = 10f;

		private float _collisionWindowStart = float.MinValue;

		private float _offRoadTime;

		private float _offRoadTimer;

		private float _lastOffRoadWarningTime = float.MinValue;

		private bool _isOffRoad;

		private int _offRoadFrameCount;

		private const int OffRoadFrameThreshold = 5;

		private int _centerlineNearestIdx;

		private RallySeqenceTriggerLine _lastPassedTrigger;

		private bool _isReturningToRoad;

		private int _lastCrossedTriggerIndex = -1;

		private int _wrongWayFrameCount;

		private const int WrongWayFrameThreshold = 10;

		private bool _isWrongWay;

		private float _wrongWayTimer;

		private const float WrongWayDisqualifyTime = 5f;

		private const float WrongWaySoundDelay = 2f;

		private float _lastWrongWaySoundTime = float.MinValue;

		private const float WrongWaySoundCooldown = 8f;

		private QuickCarSettings _quickCarSettings;

		private bool _isInJumpZone;

		private bool _isDestroyed;

		private bool _isRaceStopped;

		public float TotalTime;

		public Action<float, float> OnTimerUpdate;

		public Action<bool> OnOffRoadChanged;

		public Action<bool> OnWrongWay;

		public Action OnOffRoadWarning;

		public Action OnFrequentCrash;

		public Action OnCheckpointSkipped;

		public TyreType TyreType;

		public float DisqualifyTimeout => OffRoadDisqualifyTime;

		public float RaceTime => _raceTime;

		public float Penalty => _penalty;

		public int MissedCheckpoints { get; private set; }

		public IReadOnlyList<PenaltyRecord> PenaltyLog => _penaltyLog;

		public int CenterlineNearestIdx => _centerlineNearestIdx;

		public event Action<float> OnOffRoadTimerChanged;

		public event Action<PenaltyRecord> OnPenaltyAdded;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(QuickCarSettings quickCarSettings)
		{
			_quickCarSettings = quickCarSettings;
		}

		private void Awake()
		{
			_powertrain = GetComponent<Powertrain>();
		}

		private void Start()
		{
			_rb = base.gameObject.GetComponent<Rigidbody>();
			if (TimeProvider.skyboxSelectedRuntime != 0)
			{
				_quickCarSettings.TurnOnLights();
			}
		}

		public void SetStagePoint(RaceGaragePoint stagePoint)
		{
			_stagePoint = stagePoint;
		}

		public void Init(RallyController rallyController, RallyTriggersControl triggersControl)
		{
			_rallyController = rallyController;
			_triggersControl = triggersControl;
			RallyTriggersControl triggersControl2 = _triggersControl;
			triggersControl2.CheckpointCrossed = (Action<int>)Delegate.Combine(triggersControl2.CheckpointCrossed, new Action<int>(OnCheckpointCrossed));
		}

		private void OnCheckpointCrossed(int idx)
		{
			if (_lastCrossedTriggerIndex >= 0 && idx - _lastCrossedTriggerIndex > 1)
			{
				int num = idx - _lastCrossedTriggerIndex - 1;
				MissedCheckpoints += num;
				AddPenalty((float)num * 10f, PenaltyType.MissedCheckpoint);
				OnCheckpointSkipped?.Invoke();
			}
			List<RallySeqenceTriggerLine> list = _triggersControl?.TriggerList;
			if (list != null && list.Count > 0)
			{
				for (int i = Mathf.Max(0, _lastCrossedTriggerIndex + 1); i <= idx && i < list.Count; i++)
				{
					bool flag = list[i].HasMark(RallySequenceMark.OverJump);
					if (flag && !_isInJumpZone)
					{
						_isInJumpZone = true;
					}
					else if (!flag && _isInJumpZone)
					{
						_isInJumpZone = false;
					}
				}
			}
			_lastCrossedTriggerIndex = idx;
		}

		public void StartTimer()
		{
			_isRaceStopped = false;
			_isOffRoad = false;
			OnOffRoadChanged?.Invoke(_isOffRoad);
			_timerDisposable = Observable.EveryUpdate().Subscribe((long _) =>
			{
				Tick();
			}).AddTo(this);
		}

		public void StopTimer()
		{
			_isRaceStopped = true;
			_timerDisposable?.Dispose();
		}

		private void Tick()
		{
			if (!_isDestroyed)
			{
				_raceTime += Time.deltaTime;
				TotalTime = _raceTime + _penalty;
				OnTimerUpdate?.Invoke(_raceTime, _penalty);
				CheckOffRoad();
				CheckWrongWay();
			}
		}

		private void CheckOffRoad()
		{
			if (_triggersControl == null || _isRaceStopped)
			{
				return;
			}
			bool flag = IsCarOffRoad(out var _);
			_offRoadFrameCount = (flag ? (_offRoadFrameCount + 1) : 0);
			bool flag2 = _offRoadFrameCount >= 5;
			int cachedActiveIndex = _triggersControl.CachedActiveIndex;
			if (cachedActiveIndex > 0 && cachedActiveIndex - 1 < _triggersControl.TriggerList.Count)
			{
				_lastPassedTrigger = _triggersControl.TriggerList[cachedActiveIndex - 1];
			}
			if (flag2 != _isOffRoad)
			{
				_isOffRoad = flag2;
				OnOffRoadChanged?.Invoke(_isOffRoad);
				if (_isOffRoad && Time.time - _lastOffRoadWarningTime >= 10f)
				{
					_lastOffRoadWarningTime = Time.time;
					OnOffRoadWarning?.Invoke();
				}
			}
			if (_isOffRoad)
			{
				if (!_isReturningToRoad)
				{
					_offRoadTimer += Time.deltaTime;
					OnOffRoadTimerChanged?.Invoke(_offRoadTimer / OffRoadDisqualifyTime);
					if (_offRoadTimer >= OffRoadDisqualifyTime)
					{
						bool moveToStartPoint = (bool)_powertrain && !_powertrain.OnGround;
						_rallyController.Disqualify(RallyDisqualifyType.StopCar, moveToStartPoint);
						return;
					}
				}
				_offRoadTime += Time.deltaTime;
				if (_offRoadTime >= OffRoadPenaltyInterval)
				{
					AddPenalty(10f, PenaltyType.OffRoad);
					_offRoadTime = 0f;
				}
			}
			else
			{
				if (_offRoadTimer > 0f)
				{
					_offRoadTimer = 0f;
					OnOffRoadTimerChanged?.Invoke(0f);
				}
				_offRoadTime = 0f;
			}
		}

		private void CheckWrongWay()
		{
			if (_triggersControl == null || _isRaceStopped || _rb == null)
			{
				return;
			}
			List<Vector3> roadCenterline = _triggersControl.RoadCenterline;
			if (roadCenterline == null || roadCenterline.Count < 2)
			{
				return;
			}
			if (_rb.velocity.magnitude < 2f)
			{
				if (_wrongWayFrameCount > 0)
				{
					_wrongWayFrameCount = 0;
				}
				return;
			}
			int index = Mathf.Min(_centerlineNearestIdx + 1, roadCenterline.Count - 1);
			Vector3 normalized = (roadCenterline[index] - roadCenterline[_centerlineNearestIdx]).normalized;
			bool flag = Vector3.Dot(_rb.velocity.normalized, normalized) < -0.5f;
			_wrongWayFrameCount = (flag ? (_wrongWayFrameCount + 1) : 0);
			bool flag2 = _wrongWayFrameCount >= 10;
			if (flag2 && !_isWrongWay)
			{
				_isWrongWay = true;
			}
			else if (!flag2 && _isWrongWay)
			{
				_isWrongWay = false;
				_wrongWayTimer = 0f;
				OnWrongWay?.Invoke(obj: false);
			}
			if (_isWrongWay)
			{
				_wrongWayTimer += Time.deltaTime;
				if (_wrongWayTimer >= 2f && Time.time - _lastWrongWaySoundTime >= 8f)
				{
					_lastWrongWaySoundTime = Time.time;
					OnWrongWay?.Invoke(obj: true);
				}
				if (_wrongWayTimer >= 5f)
				{
					_rallyController.Disqualify(RallyDisqualifyType.WrongWay);
				}
			}
		}

		private bool IsCarOffRoad(out float distance)
		{
			Vector3 position = base.transform.position;
			position.y = 0f;
			List<Vector3> roadCenterline = _triggersControl.RoadCenterline;
			int num = Mathf.Max(0, _centerlineNearestIdx - 30);
			int num2 = Mathf.Min(roadCenterline.Count - 1, _centerlineNearestIdx + 40);
			float num3 = float.MaxValue;
			for (int i = num; i <= num2; i++)
			{
				Vector3 b = roadCenterline[i];
				b.y = 0f;
				float num4 = Vector3.Distance(position, b);
				if (num4 < num3)
				{
					num3 = num4;
					_centerlineNearestIdx = i;
				}
			}
			distance = num3;
			return num3 > _triggersControl.RoadHalfWidth + DefaultRoadMargin;
		}

		public void OnRallyPointTriggered(RallyTriggerPointType pointType)
		{
			switch (pointType)
			{
			case RallyTriggerPointType.StartTrigger:
				_rallyController.OnCarEnteredStartZone();
				break;
			case RallyTriggerPointType.StartLine:
				CollideWithStartPoint();
				break;
			case RallyTriggerPointType.DontCutMarker:
				AddPenalty(5f, PenaltyType.CutMarker);
				break;
			case RallyTriggerPointType.People:
				ReturnToRoad(20f, PenaltyType.PeopleCollision);
				break;
			case RallyTriggerPointType.FinishTrigger:
				_rallyController.OnCrossFinishLine();
				break;
			case RallyTriggerPointType.DisqualifyTrigger:
				_rallyController.Disqualify();
				break;
			case RallyTriggerPointType.StopLine:
				_rallyController.OnCarCrossedStopLine();
				break;
			case RallyTriggerPointType.StartMarshal:
				CollisionWithMarshall(isStart: true);
				break;
			case RallyTriggerPointType.FinishMarshal:
				CollisionWithMarshall(isStart: false);
				break;
			}
		}

		public void AddPenalty(float amount, PenaltyType type = PenaltyType.Other)
		{
			_penalty += amount;
			TotalTime = _raceTime + _penalty;
			PenaltyRecord penaltyRecord = new PenaltyRecord(type, amount);
			_penaltyLog.Add(penaltyRecord);
			OnPenaltyAdded?.Invoke(penaltyRecord);
		}

		public void ApplyTailMissedCheckpointsPenalty()
		{
			int num = _triggersControl.TotalCheckpoints - 2 - _lastCrossedTriggerIndex;
			if (num > 0)
			{
				MissedCheckpoints += num;
				AddPenalty((float)num * 10f, PenaltyType.MissedCheckpoint);
				OnCheckpointSkipped?.Invoke();
			}
		}

		public void ApplyMissedCheckpointsPenalty()
		{
			int totalCheckpoints = _triggersControl.TotalCheckpoints;
			int passedCheckpoints = _triggersControl.PassedCheckpoints;
			int num = totalCheckpoints - passedCheckpoints;
			if (num > 0)
			{
				float num2 = (float)num * 10f;
				AddPenalty(num2, PenaltyType.MissedCheckpoint);
				Debug.Log($"[RallyCar] Missed {num}/{totalCheckpoints} checkpoints | Penalty: +{num2:F0}s");
			}
		}

		public void FixCar(bool state)
		{
			_powertrain.FixVehicle(state);
		}

		private void CollisionWithMarshall(bool isStart)
		{
			_powertrain.ResetTransmission();
			if (isStart)
			{
				AddPenalty(60f, PenaltyType.MarshalCollision);
				CollideWithStartPoint();
			}
			else
			{
				_rallyController.FinishWithMarshalPenalty();
			}
		}

		public void CollideWithStartPoint()
		{
			FixCar(state: false);
			_rallyController.MoveToPoint(start: true);
			_rallyController.OnMarshalHit();
		}

		private async UniTaskVoid ClearReturnFlag()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1.5));
			_isReturningToRoad = false;
		}

		public void ReturnToRoad(float basePenalty, PenaltyType penaltyType)
		{
			if ((bool)_rallyController && _rallyController.RallyState == RallyState.Stage)
			{
				AddPenalty(basePenalty, penaltyType);
				CollideWithStartPoint();
				return;
			}
			if (_isInJumpZone && _triggersControl != null)
			{
				AddPenalty(basePenalty, penaltyType);
				int index = FindTriggerAfterJump();
				List<RallySeqenceTriggerLine> triggerList = _triggersControl.TriggerList;
				if (triggerList != null && triggerList.Count > 0)
				{
					RallySeqenceTriggerLine rallySeqenceTriggerLine = triggerList[index];
					if (_rb != null)
					{
						_rb.velocity = Vector3.zero;
						_rb.angularVelocity = Vector3.zero;
					}
					SetPosition(rallySeqenceTriggerLine.transform.position, rallySeqenceTriggerLine.transform.rotation);
					return;
				}
			}
			_isReturningToRoad = true;
			ClearReturnFlag().Forget();
			List<Vector3> list = _triggersControl?.RoadCenterline;
			if (list != null && list.Count >= 2)
			{
				int num = Mathf.Max(0, _centerlineNearestIdx - 60);
				int num2 = Mathf.Min(list.Count - 1, _centerlineNearestIdx + 60);
				float num3 = float.MaxValue;
				int num4 = _centerlineNearestIdx;
				Vector3 position = base.transform.position;
				position.y = 0f;
				for (int i = num; i <= num2; i++)
				{
					Vector3 b = list[i];
					b.y = 0f;
					float num5 = Vector3.Distance(position, b);
					if (num5 < num3)
					{
						num3 = num5;
						num4 = i;
					}
				}
				_centerlineNearestIdx = num4;
				int index2 = Mathf.Min(num4 + 1, list.Count - 1);
				Vector3 vector = list[num4];
				Vector3 normalized = (list[index2] - vector).normalized;
				Quaternion rot = ((normalized != Vector3.zero) ? Quaternion.LookRotation(normalized) : base.transform.rotation);
				float num6 = Vector3.Distance(base.transform.position, vector);
				AddPenalty(basePenalty + (_isOffRoad ? (num6 * 0.1f) : 0f), penaltyType);
				if (_rb != null)
				{
					_rb.velocity = Vector3.zero;
					_rb.angularVelocity = Vector3.zero;
				}
				SetPosition(vector + Vector3.up, rot);
				return;
			}
			List<RallySeqenceTriggerLine> list2 = _triggersControl?.TriggerList;
			int num7 = ((_triggersControl != null) ? _triggersControl.CachedActiveIndex : (-1));
			Vector3 vector2;
			Quaternion rot2;
			float num11;
			if (list2 != null && list2.Count >= 2 && num7 >= 0)
			{
				int num8 = Mathf.Max(0, num7 - 1);
				int num9 = Mathf.Max(0, num8 - 1);
				int num10 = Mathf.Min(list2.Count - 2, num8 + 1);
				vector2 = base.transform.position;
				rot2 = base.transform.rotation;
				num11 = float.MaxValue;
				for (int j = num9; j <= num10; j++)
				{
					list2[j].GetClosestPointOnCurve(base.transform.position, out var closestPoint, out var rotation);
					float num12 = Vector3.Distance(base.transform.position, closestPoint);
					if (num12 < num11)
					{
						num11 = num12;
						vector2 = closestPoint;
						rot2 = rotation;
					}
				}
			}
			else if (_lastPassedTrigger != null)
			{
				vector2 = _lastPassedTrigger.transform.position;
				rot2 = _lastPassedTrigger.transform.rotation;
				num11 = Vector3.Distance(base.transform.position, vector2);
			}
			else
			{
				vector2 = _stagePoint.Position;
				rot2 = _stagePoint.Rotation;
				num11 = Vector3.Distance(base.transform.position, vector2);
			}
			float num13 = (_isOffRoad ? (num11 * 0.1f) : 0f);
			AddPenalty(basePenalty + num13, penaltyType);
			if (_rb != null)
			{
				_rb.velocity = Vector3.zero;
				_rb.angularVelocity = Vector3.zero;
			}
			SetPosition(vector2 + Vector3.up, rot2);
		}

		private int FindTriggerAfterJump()
		{
			List<RallySeqenceTriggerLine> triggerList = _triggersControl.TriggerList;
			if (triggerList == null || triggerList.Count == 0)
			{
				return 0;
			}
			for (int i = Mathf.Max(0, _lastCrossedTriggerIndex); i < triggerList.Count; i++)
			{
				if (!triggerList[i].HasMark(RallySequenceMark.OverJump))
				{
					return i;
				}
			}
			return triggerList.Count - 1;
		}

		public void TeleportToNextTrigger()
		{
			if (_triggersControl == null)
			{
				return;
			}
			List<RallySeqenceTriggerLine> triggerList = _triggersControl.TriggerList;
			if (triggerList != null && triggerList.Count != 0)
			{
				int index = Mathf.Clamp(_lastCrossedTriggerIndex + 1, 0, triggerList.Count - 1);
				RallySeqenceTriggerLine rallySeqenceTriggerLine = triggerList[index];
				if (_rb != null)
				{
					_rb.velocity = Vector3.zero;
					_rb.angularVelocity = Vector3.zero;
				}
				SetPosition(rallySeqenceTriggerLine.transform.position + Vector3.up, rallySeqenceTriggerLine.transform.rotation);
			}
		}

		private Vector3 GetNewPos(GameObject car, Vector3 point)
		{
			int mask = LayerMask.GetMask("Road", "Default", "Terrain");
			if (Physics.Raycast(point, Vector3.down, out var hitInfo, 10f, mask))
			{
				return VectorExtensions.GetPosByWheel(car, hitInfo.point);
			}
			return point;
		}

		public void SetPosition(Vector3 pos, Quaternion rot)
		{
			base.transform.rotation = rot;
			Vector3 newPos = GetNewPos(base.gameObject, pos + Vector3.up * 5f);
			if (_rb != null)
			{
				_rb.velocity = Vector3.zero;
				_rb.angularVelocity = Vector3.zero;
			}
			base.transform.SetPositionAndRotation(newPos, rot);
		}

		public void ResetTimer()
		{
			StopTimer();
			_raceTime = 0f;
			_penalty = 0f;
			_penaltyLog.Clear();
			TotalTime = 0f;
			_offRoadTime = 0f;
			_offRoadTimer = 0f;
			_offRoadFrameCount = 0;
			_lastOffRoadWarningTime = float.MinValue;
			_collisionCount = 0;
			_collisionWindowStart = float.MinValue;
			_centerlineNearestIdx = 0;
			_isOffRoad = false;
			_isRaceStopped = false;
			_isReturningToRoad = false;
			_isInJumpZone = false;
			_lastPassedTrigger = null;
			_lastCrossedTriggerIndex = -1;
			MissedCheckpoints = 0;
			_wrongWayFrameCount = 0;
			_isWrongWay = false;
			_wrongWayTimer = 0f;
			_lastWrongWaySoundTime = float.MinValue;
			OnWrongWay?.Invoke(obj: false);
			OnOffRoadTimerChanged?.Invoke(0f);
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
			_timerDisposable?.Dispose();
			if (_triggersControl != null)
			{
				RallyTriggersControl triggersControl = _triggersControl;
				triggersControl.CheckpointCrossed = (Action<int>)Delegate.Remove(triggersControl.CheckpointCrossed, new Action<int>(OnCheckpointCrossed));
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!base.enabled || collision == null || _isRaceStopped || _isReturningToRoad || (collision.gameObject.layer != LayerViaName.DriverPersonLayer && collision.gameObject.layer != LayerViaName.RoadLayer && collision.gameObject.layer != LayerViaName.DefaultLayer))
			{
				return;
			}
			if (Time.time - _collisionWindowStart > 5f)
			{
				_collisionWindowStart = Time.time;
				_collisionCount = 1;
				return;
			}
			_collisionCount++;
			if (_collisionCount >= 25)
			{
				_collisionCount = 0;
				_collisionWindowStart = Time.time;
				OnFrequentCrash?.Invoke();
			}
		}
	}
}
