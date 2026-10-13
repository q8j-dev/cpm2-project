using System;
using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class SwingingPlatform : Obstacle, IRagdollImpact
	{
		public enum RotationAxis
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		[Tooltip("The object that actually swings: holds the collider and the kinematic Rigidbody. Its origin is the pivot, and its authored rotation is the centre of the swing.")]
		[SerializeField]
		private Transform Target;

		[Tooltip("Axis of rotation in the Target's authored local space. Default Z rocks a normally oriented arm left and right.")]
		[SerializeField]
		private RotationAxis LocalAxis = RotationAxis.Z;

		[Tooltip("The absolute extreme angle to each side of the centre, in degrees. 45 makes the arm swing between +45 and -45. Zero or less parks it at the centre.")]
		[SerializeField]
		private float AmplitudeDegrees = 45f;

		[SerializeField]
		[Tooltip("Where in the swing the cycle begins: 0 = -amplitude, 0.5 = the centre, 1 = +amplitude. In the editor Target snaps here, so the starting pose is visible without pressing Play.")]
		[Range(0f, 1f)]
		private float StartPosition;

		[Tooltip("Time shift within the shared cycle, in seconds. Positive values delay this platform relative to the shared clock; negative values advance it. The cycle duration is unchanged.")]
		[SerializeField]
		private float PhaseOffsetSeconds;

		[Tooltip("Seconds for one full swing: out to one extreme, across to the other, and back. Counts sweeping only - HoldSeconds is added on top. Zero or less parks the arm at the start pose.")]
		[SerializeField]
		private float PeriodSeconds = 2f;

		[Tooltip("Seconds the arm waits at each extreme before sweeping back. Zero reverses on arrival. Used for both sides unless Use Separate Side Holds is enabled. Zero also removes the FinishMove / FinishReturn sounds; the centre passes stay either way.")]
		[SerializeField]
		private float HoldSeconds;

		[SerializeField]
		[Tooltip("Use separate waits for the +amplitude and -amplitude extremes instead of Hold Seconds.")]
		private bool UseSeparateSideHolds;

		[Tooltip("Seconds the arm waits at +amplitude before sweeping back toward -amplitude.")]
		[SerializeField]
		private float PositiveHoldSeconds;

		[SerializeField]
		[Tooltip("Seconds the arm waits at -amplitude before sweeping back toward +amplitude.")]
		private float NegativeHoldSeconds;

		[Tooltip("Shapes the sweep. 1 = constant speed with a hard reversal; 2 = quadratic ease; 3 and up whip through the centre and linger longer at each end (a soft stop). Below 1 is clamped to 1.")]
		[SerializeField]
		private float SwingExponent = 3f;

		[Tooltip("Which way the arm heads first from StartPosition. On: toward +amplitude (e.g. +45). Off: toward -amplitude (e.g. -45).")]
		[SerializeField]
		private bool StartTowardPositive = true;

		[Tooltip("How hard the arm knocks the player down. The spin is measured about Target's origin, so the far end of the arm hits harder than the part by the pivot.")]
		[SerializeField]
		private RagdollImpact Impact = new RagdollImpact();

		private const float MaxPositionStep = 0.5f;

		private const float MaxMountStepDegrees = 45f;

		private Rigidbody _body;

		private Transform _space;

		private Quaternion _spaceRotation;

		private Vector3 _localPosition;

		private Quaternion _centreLocalRotation;

		private Vector3 _axis;

		private float _startAngleDegrees;

		private float _startOffsetSeconds;

		private float _maxStepDegrees;

		private static readonly EventType[] PhaseEvents = new EventType[6]
		{
			EventType.StartMove,
			EventType.Action,
			EventType.FinishMove,
			EventType.StartReturn,
			EventType.Action,
			EventType.FinishReturn
		};

		private void Awake()
		{
			if (!Target)
			{
				Debug.LogError("[SwingingPlatform] " + base.name + ": Target must be assigned.", this);
				return;
			}
			_body = Target.GetComponent<Rigidbody>();
			if (!_body)
			{
				Debug.LogError("[SwingingPlatform] " + base.name + ": Target needs a Rigidbody to push anything.", this);
				return;
			}
			_body.isKinematic = true;
			_body.interpolation = RigidbodyInterpolation.Interpolate;
			_space = Target.parent;
			_spaceRotation = (_space ? _space.rotation : Quaternion.identity);
			_localPosition = Target.localPosition;
			_axis = AuthoredLocalAxis(Target.localRotation, LocalAxis);
			_centreLocalRotation = CentreOf(Target.localRotation, _axis);
			RebuildSwing();
		}

		private void RebuildSwing()
		{
			if ((bool)_body)
			{
				_startAngleDegrees = StartAngle();
				_startOffsetSeconds = StartOffsetSeconds(_startAngleDegrees, AmplitudeDegrees, PeriodSeconds, SwingExponent, StartTowardPositive, PositiveHold(), NegativeHold());
				float num = ((PeriodSeconds > 0f) ? (4f * Mathf.Abs(AmplitudeDegrees) * Mathf.Max(1f, SwingExponent) / PeriodSeconds) : 0f);
				_maxStepDegrees = Mathf.Max(num * Time.fixedDeltaTime * 4f, 5f);
			}
		}

		private float StartAngle()
		{
			float num = Mathf.Abs(AmplitudeDegrees);
			return Mathf.Lerp(0f - num, num, Mathf.Clamp01(StartPosition));
		}

		private float StartPhaseAngle()
		{
			float num = StartAngle();
			if (PeriodSeconds <= 0f || Mathf.Abs(AmplitudeDegrees) <= 0f)
			{
				return num;
			}
			float startOffsetSeconds = StartOffsetSeconds(num, AmplitudeDegrees, PeriodSeconds, SwingExponent, StartTowardPositive, PositiveHold(), NegativeHold());
			return PhaseAngle(0.0, startOffsetSeconds);
		}

		private float PhaseAngle(double elapsedSeconds, float startOffsetSeconds)
		{
			return Angle(elapsedSeconds + (double)startOffsetSeconds - (double)PhaseOffsetSeconds, AmplitudeDegrees, PeriodSeconds, SwingExponent, PositiveHold(), NegativeHold());
		}

		public override void EvaluateAt(double elapsedSeconds)
		{
			if ((bool)_body)
			{
				bool num = PeriodSeconds > 0f && Mathf.Abs(AmplitudeDegrees) > 0f;
				double elapsedSeconds2 = elapsedSeconds + (double)_startOffsetSeconds - (double)PhaseOffsetSeconds;
				int num2 = (num ? PhaseAt(elapsedSeconds2, PeriodSeconds, PositiveHold(), NegativeHold()) : 5);
				ReportPhase(num2, PhaseEvents[num2]);
				Quaternion quaternion = Quaternion.AngleAxis(num ? PhaseAngle(elapsedSeconds, _startOffsetSeconds) : _startAngleDegrees, _axis) * _centreLocalRotation;
				Quaternion quaternion2 = (_space ? _space.rotation : Quaternion.identity);
				Quaternion quaternion3 = quaternion2 * quaternion;
				Vector3 vector = (_space ? _space.TransformPoint(_localPosition) : _localPosition);
				float num3 = Mathf.Min(Quaternion.Angle(_spaceRotation, quaternion2), 45f);
				_spaceRotation = quaternion2;
				if (Quaternion.Angle(_body.rotation, quaternion3) > _maxStepDegrees + num3 || (vector - _body.position).sqrMagnitude > 0.25f)
				{
					_body.position = vector;
					_body.rotation = quaternion3;
					Impact.Reset();
				}
				else
				{
					_body.MovePosition(vector);
					_body.MoveRotation(quaternion3);
					Impact.Sample(vector, quaternion3, Time.fixedDeltaTime);
				}
			}
		}

		public bool TryGetRagdollImpact(Vector3 contactPoint, out Vector3 impulse)
		{
			return Impact.TryGetImpact(contactPoint, out impulse);
		}

		public static float Angle(double elapsedSeconds, float amplitudeDegrees, float periodSeconds, float exponent, float holdSeconds = 0f)
		{
			return Angle(elapsedSeconds, amplitudeDegrees, periodSeconds, exponent, holdSeconds, holdSeconds);
		}

		public static float Angle(double elapsedSeconds, float amplitudeDegrees, float periodSeconds, float exponent, float positiveHoldSeconds, float negativeHoldSeconds)
		{
			float num = Mathf.Abs(amplitudeDegrees);
			if (periodSeconds <= 0f)
			{
				return 0f - num;
			}
			float exponent2 = Mathf.Max(1f, exponent);
			double num2 = (double)periodSeconds * 0.5;
			double num3 = Mathf.Max(0f, positiveHoldSeconds);
			double num4 = Mathf.Max(0f, negativeHoldSeconds);
			double num5 = (double)periodSeconds + num3 + num4;
			double num6 = elapsedSeconds - Math.Floor(elapsedSeconds / num5) * num5;
			if (num6 < num2)
			{
				return 0f - num + 2f * num * Ease((float)(num6 / num2), exponent2);
			}
			num6 -= num2;
			if (num6 < num3)
			{
				return num;
			}
			num6 -= num3;
			if (num6 < num2)
			{
				return num - 2f * num * Ease((float)(num6 / num2), exponent2);
			}
			return 0f - num;
		}

		public static int PhaseAt(double elapsedSeconds, float periodSeconds, float positiveHoldSeconds, float negativeHoldSeconds)
		{
			if (periodSeconds <= 0f)
			{
				return 5;
			}
			double num = (double)periodSeconds * 0.5;
			double num2 = Mathf.Max(0f, positiveHoldSeconds);
			double num3 = Mathf.Max(0f, negativeHoldSeconds);
			double num4 = (double)periodSeconds + num2 + num3;
			double num5 = elapsedSeconds - Math.Floor(elapsedSeconds / num4) * num4;
			if (num5 < num)
			{
				if (!(num5 < num * 0.5))
				{
					return 1;
				}
				return 0;
			}
			num5 -= num;
			if (num5 < num2)
			{
				return 2;
			}
			num5 -= num2;
			if (num5 < num)
			{
				if (!(num5 < num * 0.5))
				{
					return 4;
				}
				return 3;
			}
			return 5;
		}

		public static float StartOffsetSeconds(float startAngleDegrees, float amplitudeDegrees, float periodSeconds, float exponent, bool towardPositive, float holdSeconds = 0f)
		{
			return StartOffsetSeconds(startAngleDegrees, amplitudeDegrees, periodSeconds, exponent, towardPositive, holdSeconds, holdSeconds);
		}

		public static float StartOffsetSeconds(float startAngleDegrees, float amplitudeDegrees, float periodSeconds, float exponent, bool towardPositive, float positiveHoldSeconds, float negativeHoldSeconds)
		{
			float num = Mathf.Abs(amplitudeDegrees);
			if (num <= 0f || periodSeconds <= 0f)
			{
				return 0f;
			}
			float exponent2 = Mathf.Max(1f, exponent);
			float num2 = Mathf.Clamp01((Mathf.Clamp(startAngleDegrees, 0f - num, num) + num) / (2f * num));
			float num3 = periodSeconds * 0.5f;
			float num4 = Mathf.Max(0f, positiveHoldSeconds);
			if (!towardPositive)
			{
				return num3 + num4 + InverseEase(1f - num2, exponent2) * num3;
			}
			return InverseEase(num2, exponent2) * num3;
		}

		private float PositiveHold()
		{
			if (!UseSeparateSideHolds)
			{
				return HoldSeconds;
			}
			return PositiveHoldSeconds;
		}

		private float NegativeHold()
		{
			if (!UseSeparateSideHolds)
			{
				return HoldSeconds;
			}
			return NegativeHoldSeconds;
		}

		private static float Ease(float u, float exponent)
		{
			if (!(u < 0.5f))
			{
				return 1f - 0.5f * Mathf.Pow(2f * (1f - u), exponent);
			}
			return 0.5f * Mathf.Pow(2f * u, exponent);
		}

		private static float InverseEase(float eased, float exponent)
		{
			if (!(eased < 0.5f))
			{
				return 1f - 0.5f * Mathf.Pow(2f * (1f - eased), 1f / exponent);
			}
			return 0.5f * Mathf.Pow(2f * eased, 1f / exponent);
		}

		public static Vector3 AuthoredLocalAxis(Quaternion authoredRotation, RotationAxis axis)
		{
			return authoredRotation * axis switch
			{
				RotationAxis.X => Vector3.right, 
				RotationAxis.Y => Vector3.up, 
				_ => Vector3.forward, 
			};
		}

		private static float SignedTwistDegrees(Quaternion q, Vector3 axis)
		{
			float y = q.x * axis.x + q.y * axis.y + q.z * axis.z;
			return 2f * Mathf.Atan2(y, q.w) * 57.29578f;
		}

		private static Quaternion CentreOf(Quaternion localRotation, Vector3 axis)
		{
			return Quaternion.AngleAxis(0f - SignedTwistDegrees(localRotation, axis), axis) * localRotation;
		}

		private void OnDrawGizmosSelected()
		{
			if (!Target)
			{
				return;
			}
			Transform parent = Target.parent;
			Vector3 vector = AuthoredLocalAxis(Target.localRotation, LocalAxis);
			Vector3 vector2 = (parent ? parent.TransformDirection(vector) : vector);
			if (!(vector2.sqrMagnitude < 1E-06f))
			{
				vector2.Normalize();
				Vector3 vector3 = Vector3.ProjectOnPlane(Vector3.down, vector2);
				if (vector3.sqrMagnitude < 1E-06f)
				{
					vector3 = Vector3.ProjectOnPlane(Vector3.forward, vector2);
				}
				vector3 = vector3.normalized * 1.5f;
				Vector3 position = Target.position;
				float num = Mathf.Abs(AmplitudeDegrees);
				float angle = StartPhaseAngle();
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(position, position + Quaternion.AngleAxis(num, vector2) * vector3);
				Gizmos.DrawLine(position, position + Quaternion.AngleAxis(0f - num, vector2) * vector3);
				Gizmos.color = Color.green;
				Gizmos.DrawLine(position, position + Quaternion.AngleAxis(angle, vector2) * vector3);
			}
		}
	}
}
