using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Steam.Perf;

namespace Traffic
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(NavMeshAgent))]
	public class Behaviours : MonoBehaviour
	{
		private NavMeshAgent _agent;

		private Animator _animator;

		private DeactivatePerson _deactivatePerson;

		private RuntimeAnimatorController _defaultAnimatorController;

		private NavMeshPath _navMeshPath;

		private float _initWalkProbability;

		private float _initCheerProbability;

		private int _traverseDirection = 1;

		[Range(0.01f, 0.5f)]
		[SerializeField]
		[Tooltip("Used to detect the distance if agent has reached target (less = close to target). Better set big value.")]
		private float _epsilon = 0.1f;

		[SerializeField]
		private AnimatorOverrideController _angryOverrideController;

		[SerializeField]
		private AnimatorOverrideController _happyOverrideController;

		[Space]
		[Range(0f, 1f)]
		[Header("Mood probability (Sum of all mast be equal 1)")]
		[SerializeField]
		private float _normalMoodProbability = 0.5f;

		[Range(0f, 1f)]
		[SerializeField]
		private float _happyMoodProbability = 0.25f;

		[Range(0f, 1f)]
		[SerializeField]
		private float _angryMoodProbability = 0.25f;

		public readonly int hashIsMovingParam = Animator.StringToHash("IsMoving");

		public readonly int hashIsCheeringParam = Animator.StringToHash("IsCheering");

		public readonly int hashStartTalkParam = Animator.StringToHash("StartTalk");

		public readonly int hashStartListenParam = Animator.StringToHash("StartListen");

		public readonly int hashAction1Param = Animator.StringToHash("Action1Cond");

		public readonly int hashAction2Param = Animator.StringToHash("Action2Cond");

		[Header("Actions probability")]
		[Space]
		[Range(0f, 1f)]
		public float startWalkProbability = 0.2f;

		[Range(0f, 1f)]
		public float startCheerProbability = 0.1f;

		[Range(0f, 1f)]
		public float startAction1Probability;

		[Range(0f, 1f)]
		public float startAction2Probability;

		[HideInInspector]
		public bool isInConversation;

		[HideInInspector]
		public Mood currentMood;

		[HideInInspector]
		public int currentPointIndex;

		private const float SteamAgentNavMeshDistance = 1f;

		private const int SteamAvoidancePriorityMin = 48;

		private const int SteamAvoidancePriorityMax = 53;

		public const int SteamDodgeVariants = 1;

		public static int SteamDodgeForcedVariant = -1;

		public static float SteamDodgeRootScale = 1f;

		public static float SteamDodgeSeconds = 4f;

		private const float SteamDodgeClipStep = 4.93f;

		public static float SteamDodgeMinClearance = 0.5f;

		public static float SteamDodgeWallMargin = 0.5f;

		private const float SteamDodgeRayHeight = 1.1f;

		private const float SteamDodgeGroundUp = 1.5f;

		private const float SteamDodgeGroundRay = 6f;

		private const float SteamDodgeReturnRadius = 4f;

		public static float SteamDodgeMaxOffNavmesh = 5.5f;

		private const float SteamDodgeCooldown = 3f;

		private const float SteamDodgeReturnArrive = 0.15f;

		private const float SteamDodgeReturnSlack = 2f;

		private const float SteamDodgeReturnSlideDistance = 0.6f;

		private const float SteamDodgeReturnWalkFacing = 0.5f;

		private bool _steamDodgeReturning;

		private bool _steamDodgeReturnWalk;

		private Vector3 _steamDodgeReturnTo;

		private static readonly Vector2[] SteamDodgeClipDir = new Vector2[1]
		{
			new Vector2(0f, 1f)
		};

		private static readonly Vector3[] SteamDodgeCandidates = new Vector3[3];

		private static int _steamDodgeWallMask = -1;

		private static int _steamDodgeGroundMask = -1;

		private static readonly int SteamDodgeHash = Animator.StringToHash("Dodge");

		private static readonly int SteamDodgeVariantHash = Animator.StringToHash("DodgeVariant");

		private static readonly int SteamDodgeStateHash = Animator.StringToHash("Dodge");

		private float _steamDodgeUntil;

		private float _steamDodgeReadyAt;

		private Vector3 _steamDodgeDir;

		private bool _steamDodgeEntered;

		private bool _steamDodgeFlying;

		private bool _steamDodgeSavedRotate;

		private float _steamDodgeMaxTravel;

		private float _steamDodgeTravelled;

		private int _steamDodgeAnimated = -1;

		private const float SteamBlockedSpeed = 0.25f;

		private const float SteamBlockedSeconds = 0.5f;

		private const float SteamSideStepSpeed = 0.7f;

		private float _steamBlockedTime;

		private const int SteamIdleVariants = 8;

		private const float SteamSpeedJitter = 0.05f;

		private const float SteamWalkSpeedJitter = 0.18f;

		private const float SteamMinWalkSpeed = 0.85f;

		public static float SteamAgentRadiusScale = 0.6f;

		private float _steamBaseAgentRadius = -1f;

		private float _steamSpeedFactor = 1f;

		private static readonly int SteamIdleVariantHash = Animator.StringToHash("IdleVariant");

		private static readonly Dictionary<(RuntimeAnimatorController, AnimatorOverrideController), AnimatorOverrideController> SteamMoodRebase = new Dictionary<(RuntimeAnimatorController, AnimatorOverrideController), AnimatorOverrideController>();

		public bool SteamStranded { get; private set; }

		public int SteamLookId { get; set; } = -1;

		public static bool SteamAlwaysWalk => SteamPerfFlags.IsOn("pedAlwaysWalk");

		private static float SteamDodgeReturnSearch => Mathf.Max(4f, SteamDodgeMaxOffNavmesh + 1f);

		public bool SteamDodgeReady
		{
			get
			{
				if (Time.time >= _steamDodgeReadyAt)
				{
					return _steamDodgeUntil <= 0f;
				}
				return false;
			}
		}

		public bool SteamDodging => _steamDodgeUntil > 0f;

		private static int SteamWallMask
		{
			get
			{
				if (_steamDodgeWallMask < 0)
				{
					_steamDodgeWallMask = ~(LayerMaskOf(LayerViaName.PersonLayer) | LayerMaskOf(LayerViaName.PedestrainLayer) | LayerMaskOf(LayerViaName.CarLayer) | LayerMaskOf(LayerViaName.CarBody) | LayerMaskOf(LayerViaName.WheelLayer) | LayerMaskOf(LayerViaName.TrafficLayer) | LayerMaskOf(LayerViaName.TrailerLayer) | LayerMaskOf(LayerViaName.IgnoreRaycast) | LayerMaskOf(LayerViaName.Map) | LayerMaskOf(LayerViaName.LayerWater));
				}
				return _steamDodgeWallMask;
			}
		}

		private static int SteamGroundMask
		{
			get
			{
				if (_steamDodgeGroundMask < 0)
				{
					_steamDodgeGroundMask = LayerMaskOf(LayerViaName.RoadLayer) | LayerMaskOf(LayerViaName.TerrainLayer) | LayerMaskOf(LayerViaName.DefaultLayer);
				}
				return _steamDodgeGroundMask;
			}
		}

		public bool IsAgentStopped
		{
			set
			{
				if (_agent.isActiveAndEnabled && _agent.isOnNavMesh)
				{
					_agent.isStopped = value;
				}
			}
		}

		public bool IsTargetReached
		{
			get
			{
				if (_agent.isOnNavMesh)
				{
					float remainingDistance = _agent.remainingDistance;
					NavMeshPathStatus pathStatus = _agent.pathStatus;
					if (!float.IsPositiveInfinity(remainingDistance) && (pathStatus == NavMeshPathStatus.PathComplete || pathStatus == NavMeshPathStatus.PathPartial))
					{
						return FloatIsZero(remainingDistance, _epsilon);
					}
					return false;
				}
				return true;
			}
		}

		public bool IsVisible
		{
			get
			{
				if (_deactivatePerson != null)
				{
					return _deactivatePerson.IsVisible;
				}
				return false;
			}
		}

		public void SteamSkipDodge()
		{
			_steamDodgeReadyAt = Time.time + 3f;
		}

		public void SteamStartDodge(Vector3 escapeDir)
		{
			if (!_agent || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
			{
				return;
			}
			escapeDir.y = 0f;
			if (escapeDir.sqrMagnitude < 0.0001f)
			{
				return;
			}
			if (!_animator || !_animator.enabled || !SteamHasDodgeState())
			{
				SteamSkipDodge();
				return;
			}
			if (!SteamPickDodgeDirection(escapeDir.normalized, out var chosen, out var travel))
			{
				SteamSkipDodge();
				return;
			}
			_steamDodgeDir = chosen;
			_steamDodgeMaxTravel = travel;
			_steamDodgeTravelled = 0f;
			_steamDodgeEntered = false;
			_steamDodgeReturning = false;
			_steamDodgeUntil = Time.time + SteamDodgeSeconds;
			_steamDodgeReadyAt = Time.time + SteamDodgeSeconds + 3f;
			int num = ((SteamDodgeForcedVariant >= 0) ? Mathf.Min(SteamDodgeForcedVariant, 0) : Random.Range(0, 1));
			Vector2 vector = SteamDodgeClipDir[num];
			float num2 = Mathf.Atan2(vector.x, vector.y) * 57.29578f;
			float num3 = Mathf.Atan2(_steamDodgeDir.x, _steamDodgeDir.z) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, num3 - num2, 0f);
			_steamDodgeSavedRotate = _agent.updateRotation;
			_agent.enabled = false;
			_steamDodgeFlying = true;
			if ((bool)_animator)
			{
				_animator.applyRootMotion = true;
			}
			_animator.SetFloat(SteamDodgeVariantHash, num);
			_animator.SetTrigger(SteamDodgeHash);
		}

		private bool SteamPickDodgeDirection(Vector3 preferred, out Vector3 chosen, out float travel)
		{
			float need = 4.93f * SteamDodgeRootScale;
			Vector3 vector = Vector3.Cross(Vector3.up, preferred);
			SteamDodgeCandidates[0] = preferred;
			SteamDodgeCandidates[1] = (preferred + vector).normalized;
			SteamDodgeCandidates[2] = (preferred - vector).normalized;
			chosen = preferred;
			travel = 0f;
			float num = -1f;
			for (int i = 0; i < SteamDodgeCandidates.Length; i++)
			{
				Vector3 vector2 = SteamDodgeCandidates[i];
				float num2 = SteamWallClearance(vector2, need);
				float num3 = num2 + Vector3.Dot(vector2, preferred) * 0.5f;
				if (!(num3 <= num))
				{
					num = num3;
					travel = num2;
					chosen = vector2;
				}
			}
			return travel >= SteamDodgeMinClearance;
		}

		private static int LayerMaskOf(int layer)
		{
			if (layer < 0)
			{
				return 0;
			}
			return 1 << layer;
		}

		private float SteamWallClearance(Vector3 dir, float need)
		{
			if (SteamWallMask == 0)
			{
				return need;
			}
			if (!Physics.Raycast(base.transform.position + Vector3.up * 1.1f, dir, out var hitInfo, need + SteamDodgeWallMargin, SteamWallMask, QueryTriggerInteraction.Ignore))
			{
				return need;
			}
			return Mathf.Max(0f, hitInfo.distance - SteamDodgeWallMargin);
		}

		private bool SteamHasDodgeState()
		{
			if (_steamDodgeAnimated >= 0)
			{
				return _steamDodgeAnimated == 1;
			}
			_steamDodgeAnimated = 0;
			AnimatorControllerParameter[] parameters = _animator.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].nameHash == SteamDodgeHash)
				{
					_steamDodgeAnimated = 1;
					break;
				}
			}
			return _steamDodgeAnimated == 1;
		}

		public bool SteamStepDodge()
		{
			if (_steamDodgeUntil <= 0f)
			{
				return false;
			}
			if (_steamDodgeReturning)
			{
				return SteamStepReturn();
			}
			bool flag = !_animator || !_animator.enabled;
			bool flag2 = false;
			if (!flag)
			{
				if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == SteamDodgeStateHash)
				{
					_steamDodgeEntered = true;
				}
				else if (_steamDodgeEntered)
				{
					flag2 = true;
				}
			}
			if (!flag && !flag2 && Time.time < _steamDodgeUntil)
			{
				return true;
			}
			if (!flag && SteamBeginReturn())
			{
				return true;
			}
			SteamFinishDodge();
			return false;
		}

		private bool SteamBeginReturn()
		{
			_steamDodgeFlying = false;
			_animator.applyRootMotion = false;
			Vector3 position = base.transform.position;
			if (!NavMesh.SamplePosition(position, out var hit, SteamDodgeReturnSearch, 8))
			{
				return false;
			}
			Vector3 vector = hit.position - position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			if (magnitude <= 0.15f)
			{
				return false;
			}
			_steamDodgeReturning = true;
			_steamDodgeReturnWalk = magnitude > 0.6f;
			_steamDodgeReturnTo = hit.position;
			float num = (_agent ? Mathf.Max(_agent.speed, 0.5f) : 1f);
			_steamDodgeUntil = Time.time + magnitude / num + 2f;
			return true;
		}

		private bool SteamStepReturn()
		{
			if (!_animator || !_animator.enabled || Time.time >= _steamDodgeUntil)
			{
				SteamFinishDodge();
				return false;
			}
			Vector3 position = base.transform.position;
			Vector3 vector = _steamDodgeReturnTo - position;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			if (magnitude <= 0.15f)
			{
				SteamFinishDodge();
				return false;
			}
			Vector3 vector2 = vector / magnitude;
			float num = (_agent ? _agent.speed : 1f);
			if (!_steamDodgeReturnWalk)
			{
				position += vector2 * Mathf.Min(magnitude, num * Time.deltaTime);
				SteamSnapToGround(ref position);
				base.transform.position = position;
				return true;
			}
			float num2 = (_agent ? _agent.angularSpeed : 200f);
			Quaternion quaternion = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(vector2), num2 * Time.deltaTime);
			float num3 = Vector3.Dot(quaternion * Vector3.forward, vector2);
			bool flag = num3 >= 0.5f;
			_animator.SetBool(hashIsMovingParam, flag);
			if (flag)
			{
				position += vector2 * Mathf.Min(magnitude, num * num3 * Time.deltaTime);
			}
			SteamSnapToGround(ref position);
			base.transform.SetPositionAndRotation(position, quaternion);
			return true;
		}

		private void SteamSnapToGround(ref Vector3 position)
		{
			if (SteamGroundMask != 0 && Physics.Raycast(position + Vector3.up * 1.5f, Vector3.down, out var hitInfo, 7.5f, SteamGroundMask, QueryTriggerInteraction.Ignore))
			{
				position.y = hitInfo.point.y;
			}
		}

		private void SteamFinishDodge()
		{
			_steamDodgeUntil = 0f;
			_steamDodgeFlying = false;
			_steamDodgeReturning = false;
			if ((bool)_animator)
			{
				_animator.applyRootMotion = false;
			}
			if ((bool)_agent)
			{
				bool num = NavMesh.SamplePosition(base.transform.position, out var hit, SteamDodgeReturnSearch, 8);
				if (!_agent.enabled)
				{
					_agent.enabled = true;
				}
				if (num)
				{
					_agent.Warp(hit.position);
				}
				else
				{
					SteamStranded = true;
				}
				_agent.updateRotation = _steamDodgeSavedRotate;
				if (_agent.isOnNavMesh)
				{
					_agent.isStopped = false;
				}
			}
		}

		private void OnAnimatorMove()
		{
			if (!_steamDodgeFlying || !_animator)
			{
				return;
			}
			float y = _animator.deltaRotation.eulerAngles.y;
			if (y > 0.001f && y < 359.999f)
			{
				base.transform.rotation = Quaternion.Euler(0f, base.transform.eulerAngles.y + y, 0f);
			}
			Vector3 vector = _animator.deltaPosition * SteamDodgeRootScale;
			vector.y = 0f;
			float magnitude = vector.magnitude;
			float num = _steamDodgeMaxTravel - _steamDodgeTravelled;
			if (!(magnitude <= 0.0001f) && !(num <= 0f))
			{
				if (magnitude > num)
				{
					vector *= num / magnitude;
				}
				_steamDodgeTravelled += Mathf.Min(magnitude, num);
				Vector3 position = base.transform.position + vector;
				if (!NavMesh.SamplePosition(position, out var _, SteamDodgeMaxOffNavmesh, 8))
				{
					_steamDodgeTravelled = _steamDodgeMaxTravel;
					return;
				}
				SteamSnapToGround(ref position);
				base.transform.position = position;
			}
		}

		public void SteamApplyAvoidance()
		{
			if ((bool)_agent)
			{
				_agent.obstacleAvoidanceType = (SteamPerfFlags.IsOn("pedAvoidance") ? ObstacleAvoidanceType.GoodQualityObstacleAvoidance : ObstacleAvoidanceType.NoObstacleAvoidance);
				if (_steamBaseAgentRadius < 0f)
				{
					_steamBaseAgentRadius = _agent.radius;
				}
				_agent.radius = _steamBaseAgentRadius * SteamAgentRadiusScale;
			}
		}

		public void SteamResolveHeadOn()
		{
			if (!_agent || !_agent.isOnNavMesh || _agent.isStopped || _agent.pathPending)
			{
				_steamBlockedTime = 0f;
				return;
			}
			if (_agent.velocity.sqrMagnitude > 0.0625f)
			{
				_steamBlockedTime = 0f;
				return;
			}
			_steamBlockedTime += Time.deltaTime;
			if (!(_steamBlockedTime < 0.5f))
			{
				_agent.Move(base.transform.right * (0.7f * Time.deltaTime));
			}
		}

		public void SteamRandomizeInstance()
		{
			if (!_agent)
			{
				_agent = GetComponent<NavMeshAgent>();
			}
			if ((bool)_agent)
			{
				_agent.avoidancePriority = Random.Range(48, 53);
				SteamApplyAvoidance();
			}
			if (!_animator)
			{
				_animator = GetComponent<Animator>();
			}
			if (!_animator)
			{
				return;
			}
			_animator.speed = _steamSpeedFactor;
			AnimatorControllerParameter[] parameters = _animator.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].nameHash == SteamIdleVariantHash)
				{
					_animator.SetFloat(SteamIdleVariantHash, Random.Range(0, 8));
					break;
				}
			}
		}

		private void Awake()
		{
			_agent = GetComponent<NavMeshAgent>();
			_animator = GetComponent<Animator>();
			_deactivatePerson = GetComponentInChildren<DeactivatePerson>();
			_defaultAnimatorController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
			RuntimeAnimatorController runtimeAnimatorController = _animator.runtimeAnimatorController;
			_happyOverrideController = SteamRebaseMood(_happyOverrideController, runtimeAnimatorController);
			_angryOverrideController = SteamRebaseMood(_angryOverrideController, runtimeAnimatorController);
			_navMeshPath = new NavMeshPath();
			_initWalkProbability = startWalkProbability;
			_initCheerProbability = startCheerProbability;
			_agent.stoppingDistance = Random.Range(0.5f, 2.5f);
			_epsilon += _agent.stoppingDistance;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void SteamClearMoodRebase()
		{
			SteamMoodRebase.Clear();
		}

		private static AnimatorOverrideController SteamRebaseMood(AnimatorOverrideController source, RuntimeAnimatorController moodBase)
		{
			if (!source || moodBase == null)
			{
				return source;
			}
			(RuntimeAnimatorController, AnimatorOverrideController) key = (moodBase, source);
			if (SteamMoodRebase.TryGetValue(key, out var value))
			{
				return value;
			}
			AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(moodBase);
			List<KeyValuePair<AnimationClip, AnimationClip>> list = new List<KeyValuePair<AnimationClip, AnimationClip>>(source.overridesCount);
			source.GetOverrides(list);
			List<KeyValuePair<AnimationClip, AnimationClip>> list2 = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
			animatorOverrideController.GetOverrides(list2);
			int num = 0;
			for (int i = 0; i < list2.Count; i++)
			{
				AnimationClip key2 = list2[i].Key;
				if (!key2)
				{
					continue;
				}
				for (int j = 0; j < list.Count; j++)
				{
					if ((bool)list[j].Key && (bool)list[j].Value && !(list[j].Key.name != key2.name))
					{
						list2[i] = new KeyValuePair<AnimationClip, AnimationClip>(key2, list[j].Value);
						num++;
						break;
					}
				}
			}
			animatorOverrideController.ApplyOverrides(list2);
			SteamMoodRebase[key] = animatorOverrideController;
			return animatorOverrideController;
		}

		private void OnEnable()
		{
			SteamStranded = false;
			float value = Random.value;
			if (value <= _normalMoodProbability)
			{
				currentMood = Mood.Normal;
				_animator.runtimeAnimatorController = _defaultAnimatorController;
				startWalkProbability = _initWalkProbability;
				startCheerProbability = _initCheerProbability;
				_agent.speed = 1.05f;
			}
			else
			{
				float num = value;
				if (num > _normalMoodProbability && num <= _normalMoodProbability + _happyMoodProbability)
				{
					currentMood = Mood.Happy;
					_animator.runtimeAnimatorController = _happyOverrideController;
					startCheerProbability = _initCheerProbability * 1.5f;
					startWalkProbability = _initWalkProbability - startCheerProbability;
					_agent.speed = 1.15f;
				}
				else if (value > _normalMoodProbability + _happyMoodProbability)
				{
					currentMood = Mood.Angry;
					_animator.runtimeAnimatorController = _angryOverrideController;
					startWalkProbability = _initWalkProbability;
					startCheerProbability = 0f;
					_agent.speed = 0.9f;
				}
			}
			_steamDodgeAnimated = -1;
			float speed = _agent.speed;
			float num2 = Mathf.Max(speed * (1f + Random.Range(-0.18f, 0.18f)), 0.85f);
			_agent.speed = num2;
			_steamSpeedFactor = ((speed > 0.01f) ? (num2 / speed) : 1f);
			if ((bool)_animator)
			{
				_animator.speed = _steamSpeedFactor;
			}
			SceneLinkedSMB<Behaviours>.Initialise(_animator, this);
		}

		public bool CheckIsFar()
		{
			if (V.DriverPerson == null)
			{
				return true;
			}
			return (base.transform.position - V.DriverPerson.transform.position).sqrMagnitude > 22500f;
		}

		public void DeactivateBehaviours()
		{
			if ((bool)_animator)
			{
				_animator.enabled = false;
			}
			if ((bool)_agent && _agent.enabled)
			{
				_agent.enabled = false;
			}
		}

		public void ActivateBehaviours()
		{
			if ((bool)_animator)
			{
				_animator.enabled = true;
			}
			if ((bool)_agent && !_agent.enabled && !SteamDodging)
			{
				if (NavMesh.SamplePosition(base.transform.position, out var _, 1f, 8))
				{
					_agent.enabled = true;
				}
				else
				{
					SteamStranded = true;
				}
			}
		}

		public void StartTalk(Vector3 target)
		{
			base.transform.LookAt(target);
			_animator.SetTrigger(hashStartTalkParam);
		}

		public void StartListen(Vector3 target)
		{
			base.transform.LookAt(target);
			_animator.SetTrigger(hashStartListenParam);
		}

		public bool ChangeAgentPosition(Vector3 newPos)
		{
			return _agent.Warp(newPos);
		}

		public bool MoveToRandomPosition()
		{
			if (!_agent || !_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
			{
				return false;
			}
			if (SteamDodging)
			{
				return false;
			}
			if (CalculateAndSetPath())
			{
				return true;
			}
			_traverseDirection *= -1;
			if (CalculateAndSetPath())
			{
				return true;
			}
			return false;
		}

		private bool CalculateAndSetPath()
		{
			int num = Random.Range(1, 4);
			int num2 = currentPointIndex + _traverseDirection * num;
			if (num2 >= PeopleTraffic.Instance.points.pointPositions.Length || num2 < 0)
			{
				return false;
			}
			if (!NavMesh.CalculatePath(base.transform.position, PeopleTraffic.Instance.points.pointPositions[num2], 8, _navMeshPath))
			{
				return false;
			}
			if (_navMeshPath.status != NavMeshPathStatus.PathComplete)
			{
				SteamStranded = true;
				return false;
			}
			SteamStranded = false;
			currentPointIndex = num2;
			_agent.SetPath(_navMeshPath);
			return true;
		}

		private bool FloatIsZero(float value, float epsilon)
		{
			if (value < epsilon)
			{
				return value > 0f - epsilon;
			}
			return false;
		}
	}
}
