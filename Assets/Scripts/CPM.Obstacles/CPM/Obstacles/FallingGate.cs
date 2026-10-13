using System;
using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class FallingGate : Obstacle, IRagdollImpact
	{
		[SerializeField]
		[Tooltip("Raised anchor A: the open pose the gate lifts back to. A child transform, moved by the designer.")]
		private Transform Start;

		[Tooltip("Lowered anchor B: the blocking pose on the ground the gate drops onto. A child transform, moved by the designer.")]
		[SerializeField]
		private Transform Finish;

		[Tooltip("The object that actually moves: holds the collider and the kinematic Rigidbody. Author it at Start (raised).")]
		[SerializeField]
		private Transform Target;

		[Tooltip("How quick the drop is, in units per second. Sets the fall duration (drop distance / FallSpeed); the slab accelerates and bounces within that time. Zero or less parks the gate.")]
		[SerializeField]
		private float FallSpeed = 20f;

		[Tooltip("The slow lift speed in units per second - constant, so this reads as the return speed literally. Zero or less parks the gate.")]
		[SerializeField]
		private float RiseSpeed = 2f;

		[Tooltip("Seconds the gate stays down at B, blocking the passage, before it starts to lift. Zero removes the FinishMove sound; the impact hangs on Action and survives.")]
		[SerializeField]
		private float HoldDownSeconds = 1.5f;

		[Tooltip("Seconds the gate waits up at A, passage open, before it drops again. Zero removes the FinishReturn sound.")]
		[SerializeField]
		private float HoldUpSeconds = 2f;

		[Range(0f, 1f)]
		[Tooltip("Where in the cycle the gate starts, 0..1 across the whole fall/hold/rise/hold loop. 0 = raised at A about to drop. Offset different gates to desync them.")]
		[SerializeField]
		private float InitialProgress;

		[Tooltip("How hard the slab knocks the player down. The drop accelerates and the lift is slow, so the same settings flatten anyone under a falling gate and spare anyone it lifts.")]
		[SerializeField]
		private RagdollImpact Impact = new RagdollImpact();

		private const float MaxMountStepDegrees = 45f;

		private Rigidbody _body;

		private Transform _space;

		private Quaternion _spaceRotation;

		private Quaternion _localRotation;

		private Vector3 _startLocal;

		private Vector3 _finishLocal;

		private float _travelDistance;

		private float _fallSeconds;

		private float _riseSeconds;

		private float _cycleSeconds;

		private float _startOffsetSeconds;

		private float _maxStepSqr;

		private const float BounceScale = 7.5625f;

		private const float BounceDivisor = 2.75f;

		private const float BounceMaxSlope = 5.5f;

		private const float GroundContact = 372f / 1023f;

		private static readonly EventType[] PhaseEvents = new EventType[5]
		{
			EventType.StartMove,
			EventType.Action,
			EventType.FinishMove,
			EventType.StartReturn,
			EventType.FinishReturn
		};

		private void Awake()
		{
			if (!Start || !Finish || !Target || !Target.parent)
			{
				Debug.LogError("[FallingGate] " + base.name + ": Start, Finish and Target must all be assigned, and Target must be a child, not a scene root.", this);
				return;
			}
			_body = Target.GetComponent<Rigidbody>();
			if (!_body)
			{
				Debug.LogError("[FallingGate] " + base.name + ": Target needs a Rigidbody to push anything.", this);
				return;
			}
			_body.isKinematic = true;
			_body.interpolation = RigidbodyInterpolation.Interpolate;
			_space = Target.parent;
			_spaceRotation = _space.rotation;
			_localRotation = Target.localRotation;
			_startLocal = _space.InverseTransformPoint(Start.position);
			_finishLocal = _space.InverseTransformPoint(Finish.position);
			_travelDistance = Vector3.Distance(Start.position, Finish.position);
			RebuildTravel();
		}

		private void RebuildTravel()
		{
			if ((bool)_body)
			{
				_fallSeconds = ((FallSpeed > 0f) ? (_travelDistance / FallSpeed) : 0f);
				_riseSeconds = ((RiseSpeed > 0f) ? (_travelDistance / RiseSpeed) : 0f);
				_cycleSeconds = _fallSeconds + Mathf.Max(0f, HoldDownSeconds) + _riseSeconds + Mathf.Max(0f, HoldUpSeconds);
				_startOffsetSeconds = Mathf.Clamp01(InitialProgress) * _cycleSeconds;
				float num = Mathf.Max(Mathf.Max(Mathf.Max(0f, FallSpeed) * 5.5f, Mathf.Max(0f, RiseSpeed)) * Time.fixedDeltaTime * 4f, 0.05f);
				_maxStepSqr = num * num;
			}
		}

		public override void EvaluateAt(double elapsedSeconds)
		{
			if ((bool)_body)
			{
				double elapsedSeconds2 = elapsedSeconds + (double)_startOffsetSeconds;
				int num = PhaseAt(elapsedSeconds2, _fallSeconds, HoldDownSeconds, _riseSeconds, HoldUpSeconds);
				ReportPhase(num, PhaseEvents[num]);
				float t = Progress(elapsedSeconds2, _fallSeconds, HoldDownSeconds, _riseSeconds, HoldUpSeconds);
				Vector3 position = Vector3.LerpUnclamped(_startLocal, _finishLocal, t);
				Vector3 vector = _space.TransformPoint(position);
				Quaternion rotation = _space.rotation;
				Quaternion quaternion = rotation * _localRotation;
				bool num2 = Quaternion.Angle(_spaceRotation, rotation) > 45f;
				_spaceRotation = rotation;
				if (num2 || (vector - _body.position).sqrMagnitude > _maxStepSqr)
				{
					_body.position = vector;
					_body.rotation = quaternion;
					Impact.Reset();
				}
				else
				{
					_body.MovePosition(vector);
					_body.MoveRotation(quaternion);
					Impact.Sample(vector, quaternion, Time.fixedDeltaTime);
				}
			}
		}

		public bool TryGetRagdollImpact(Vector3 contactPoint, out Vector3 impulse)
		{
			return Impact.TryGetImpact(contactPoint, out impulse);
		}

		public static float Progress(double elapsedSeconds, float fallSeconds, float holdDownSeconds, float riseSeconds, float holdUpSeconds)
		{
			float num = Mathf.Max(0f, fallSeconds);
			float num2 = Mathf.Max(0f, holdDownSeconds);
			float num3 = Mathf.Max(0f, riseSeconds);
			float num4 = Mathf.Max(0f, holdUpSeconds);
			double num5 = (double)num + (double)num2 + (double)num3 + (double)num4;
			if (num5 <= 0.0)
			{
				return 0f;
			}
			double num6 = elapsedSeconds - Math.Floor(elapsedSeconds / num5) * num5;
			if (num6 < (double)num)
			{
				return EaseOutBounce((float)(num6 / (double)num));
			}
			num6 -= (double)num;
			if (num6 < (double)num2)
			{
				return 1f;
			}
			num6 -= (double)num2;
			if (num6 < (double)num3)
			{
				if (!(num3 > 0f))
				{
					return 0f;
				}
				return 1f - (float)(num6 / (double)num3);
			}
			return 0f;
		}

		public static float EaseOutBounce(float t)
		{
			t = Mathf.Clamp01(t);
			if (t < 372f / 1023f)
			{
				return 7.5625f * t * t;
			}
			if (t < 744f / 1023f)
			{
				t -= 558f / 1023f;
				return 7.5625f * t * t + 0.75f;
			}
			if (t < 930f / 1023f)
			{
				t -= 837f / 1023f;
				return 7.5625f * t * t + 0.9375f;
			}
			t -= 21f / 22f;
			return 7.5625f * t * t + 63f / 64f;
		}

		public static int PhaseAt(double elapsedSeconds, float fallSeconds, float holdDownSeconds, float riseSeconds, float holdUpSeconds)
		{
			float num = Mathf.Max(0f, fallSeconds);
			float num2 = Mathf.Max(0f, holdDownSeconds);
			float num3 = Mathf.Max(0f, riseSeconds);
			float num4 = Mathf.Max(0f, holdUpSeconds);
			double num5 = (double)num + (double)num2 + (double)num3 + (double)num4;
			if (num5 <= 0.0)
			{
				return 4;
			}
			double num6 = elapsedSeconds - Math.Floor(elapsedSeconds / num5) * num5;
			if (num6 < (double)num)
			{
				float num7 = num * (372f / 1023f);
				if (!(num6 < (double)num7))
				{
					return 1;
				}
				return 0;
			}
			num6 -= (double)num;
			if (num6 < (double)num2)
			{
				return 2;
			}
			num6 -= (double)num2;
			if (num6 < (double)num3)
			{
				return 3;
			}
			return 4;
		}

		private void OnDrawGizmosSelected()
		{
			if ((bool)Start && (bool)Finish)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(Start.position, Finish.position);
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(Start.position, 0.15f);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(Finish.position, 0.15f);
			}
		}
	}
}
