using System;
using DG.Tweening;
using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class RouteRunner : Obstacle, IRagdollImpact
	{
		[Tooltip("The route, in order: at least two transforms the designer moves in the scene. Their positions are read once at startup.")]
		[SerializeField]
		private Transform[] Waypoints = default;

		[Tooltip("The object that actually moves: holds the collider and the kinematic Rigidbody. It is switched off between runs, so everything under it vanishes with it.")]
		[SerializeField]
		private Transform Target = default;

		[SerializeField]
		[Tooltip("Travel speed in units per second along the smoothed path. With an ease it is the average speed: the trip always takes route length / Speed. Zero or less parks the runner at the first waypoint.")]
		private float Speed = 3f;

		[SerializeField]
		[Tooltip("How the trip is spread over its time. Linear holds a constant speed; the InOut eases set off and arrive gently. Back and Elastic overshoot the ends - the runner simply sits on the last waypoint while they do.")]
		private Ease TravelEase = Ease.Linear;

		[Tooltip("Hand-drawn alternative to the ease: time through the run across, fraction of the route up. It takes over as soon as it has two keys, and it is stretched over the run however wide it was drawn. Normally it climbs 0 to 1; coming back down to 0 sends the runner out and back, which no ease can do. Values outside 0..1 are clamped to the waypoints.")]
		[SerializeField]
		private AnimationCurve TravelCurve = new AnimationCurve();

		[Tooltip("Seconds the runner stands at the last waypoint, still visible, before it vanishes. Zero removes the FinishMove sound: there is no moment of arrival to hang it on.")]
		[SerializeField]
		private float HoldSeconds = 1f;

		[Tooltip("Seconds the runner stays hidden before reappearing at the first waypoint. Zero makes the return a visible jump.")]
		[SerializeField]
		private float HiddenSeconds = 0.5f;

		[SerializeField]
		[Tooltip("Seconds the runner stays hidden before its very first run, counted from the match start.")]
		private float StartDelaySeconds = default;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Where in the cycle the runner starts, 0..1 across the whole run/hold/hide loop. 0 = about to set off from the first waypoint. Offset copies to desync them.")]
		private float InitialProgress = default;

		[SerializeField]
		[Range(1f, 32f)]
		[Tooltip("How many straight samples each leg of the route is smoothed into. Higher is rounder and slightly more accurate in length; it costs memory once, not per step.")]
		private int SamplesPerLeg = 12;

		[Tooltip("Optional mesh under Target that rolls as the runner travels - a ball, a barrel. Only its rotation is driven: the collider and the Rigidbody are left alone.")]
		[SerializeField]
		private Transform TargetGraphics;

		[Tooltip("Radius of that mesh. The roll is path length / radius, so a wrong figure reads as the ball skidding or over-spinning. Zero or less leaves it unrotated.")]
		[SerializeField]
		private float RollRadius = 1f;

		[Tooltip("How hard the runner knocks the player down. It hits with its travel speed, so a slow route can be made harmless by keeping the minimum speed above it.")]
		[SerializeField]
		private RagdollImpact Impact = new RagdollImpact();

		private Rigidbody _body;

		private Transform _space;

		private Vector3[] _points;

		private float[] _cumulative;

		private Quaternion[] _roll;

		private Quaternion _graphicsBase;

		private float _totalLength;

		private float _travelSeconds;

		private float _maxStepSqr;

		private static readonly EventType[] PhaseEvents = new EventType[4]
		{
			EventType.StartMove,
			EventType.FinishMove,
			EventType.Action,
			EventType.Action
		};

		private const float EaseOvershoot = 1.70158f;

		private const float EasePeriod = 0f;

		private void Awake()
		{
			if (!Target || !Target.parent || Waypoints == null || Waypoints.Length < 2)
			{
				Debug.LogError("[RouteRunner] " + base.name + ": Target must be assigned and be a child, not a scene root, and the route needs at least two Waypoints.", this);
				return;
			}
			Rigidbody component = Target.GetComponent<Rigidbody>();
			if (!component)
			{
				Debug.LogError("[RouteRunner] " + base.name + ": Target needs a Rigidbody to push anything.", this);
				return;
			}
			_space = Target.parent;
			if (!RebuildRoute())
			{
				Debug.LogError("[RouteRunner] " + base.name + ": the route could not be built - an empty Waypoint?", this);
				return;
			}
			if ((bool)TargetGraphics)
			{
				if (!TargetGraphics.IsChildOf(Target))
				{
					Debug.LogError("[RouteRunner] " + base.name + ": TargetGraphics must sit under Target to travel with it.", this);
					TargetGraphics = null;
				}
				else
				{
					_graphicsBase = Quaternion.Inverse(_space.rotation) * TargetGraphics.rotation;
				}
			}
			if (HasTravelCurve(TravelCurve) && (!IsRouteEnd(Shape(0f, TravelCurve, TravelEase)) || !IsRouteEnd(Shape(1f, TravelCurve, TravelEase))))
			{
				Debug.LogWarning("[RouteRunner] " + base.name + ": TravelCurve starts or ends between the waypoints rather than on one - its ends should be 0 or 1.", this);
			}
			_body = component;
			_body.isKinematic = true;
			_body.interpolation = RigidbodyInterpolation.Interpolate;
			RebuildTravel();
		}

		private bool RebuildRoute()
		{
			if (!BuildRoute(_space, out var points, out var cumulative))
			{
				return false;
			}
			_points = points;
			_cumulative = cumulative;
			_totalLength = _cumulative[_cumulative.Length - 1];
			_roll = BuildRoll(_points, _cumulative, RollRadius);
			return true;
		}

		private void RebuildTravel()
		{
			if ((bool)_body)
			{
				_travelSeconds = ((Speed > 0f) ? (_totalLength / Speed) : 0f);
				float num = Mathf.Max(Mathf.Max(0f, Speed) * PeakSlope(TravelCurve, TravelEase) * Time.fixedDeltaTime * 4f, 0.05f);
				_maxStepSqr = num * num;
			}
		}

		public override void EvaluateAt(double elapsedSeconds)
		{
			if (!_body)
			{
				return;
			}
			int num = PhaseAt(elapsedSeconds, _travelSeconds, HoldSeconds, HiddenSeconds, StartDelaySeconds, InitialProgress);
			ReportPhase(num, PhaseEvents[num]);
			bool num2 = TryProgress(elapsedSeconds, _travelSeconds, HoldSeconds, HiddenSeconds, StartDelaySeconds, InitialProgress, out var progress);
			float distance = Shape(progress, TravelCurve, TravelEase) * _totalLength;
			Vector3 vector = _space.TransformPoint(PointAt(_points, _cumulative, distance));
			if ((bool)TargetGraphics)
			{
				TargetGraphics.rotation = _space.rotation * RollAt(distance) * _graphicsBase;
			}
			bool activeSelf = Target.gameObject.activeSelf;
			if (!num2)
			{
				if (activeSelf)
				{
					Target.position = vector;
					Target.gameObject.SetActive(value: false);
					Impact.Reset();
				}
			}
			else if (!activeSelf)
			{
				Target.position = vector;
				Target.gameObject.SetActive(value: true);
				Impact.Reset();
			}
			else if ((vector - _body.position).sqrMagnitude > _maxStepSqr)
			{
				_body.position = vector;
				Impact.Reset();
			}
			else
			{
				_body.MovePosition(vector);
				Impact.Sample(vector, _body.rotation, Time.fixedDeltaTime);
			}
		}

		public bool TryGetRagdollImpact(Vector3 contactPoint, out Vector3 impulse)
		{
			return Impact.TryGetImpact(contactPoint, out impulse);
		}

		public static bool TryProgress(double elapsedSeconds, float travelSeconds, float holdSeconds, float hiddenSeconds, float startDelaySeconds, float initialProgress, out float progress)
		{
			progress = 0f;
			float num = Mathf.Max(0f, travelSeconds);
			float num2 = Mathf.Max(0f, holdSeconds);
			float num3 = Mathf.Max(0f, hiddenSeconds);
			double num4 = (double)num + (double)num2 + (double)num3;
			if (num <= 0f || num4 <= 0.0)
			{
				return true;
			}
			double num5 = elapsedSeconds - (double)Mathf.Max(0f, startDelaySeconds);
			if (num5 < 0.0)
			{
				return false;
			}
			num5 += (double)Mathf.Clamp01(initialProgress) * num4;
			num5 -= Math.Floor(num5 / num4) * num4;
			if (num5 < (double)num)
			{
				progress = (float)(num5 / (double)num);
				return true;
			}
			num5 -= (double)num;
			if (num5 < (double)num2)
			{
				progress = 1f;
				return true;
			}
			return false;
		}

		public static int PhaseAt(double elapsedSeconds, float travelSeconds, float holdSeconds, float hiddenSeconds, float startDelaySeconds, float initialProgress)
		{
			float num = Mathf.Max(0f, travelSeconds);
			float num2 = Mathf.Max(0f, holdSeconds);
			float num3 = Mathf.Max(0f, hiddenSeconds);
			double num4 = (double)num + (double)num2 + (double)num3;
			if (num <= 0f || num4 <= 0.0)
			{
				return 0;
			}
			double num5 = elapsedSeconds - (double)Mathf.Max(0f, startDelaySeconds);
			if (num5 < 0.0)
			{
				return 3;
			}
			num5 += (double)Mathf.Clamp01(initialProgress) * num4;
			num5 -= Math.Floor(num5 / num4) * num4;
			if (num5 < (double)num)
			{
				return 0;
			}
			num5 -= (double)num;
			if (num5 < (double)num2)
			{
				return 1;
			}
			return 2;
		}

		public static float Shape(float progress, AnimationCurve curve, Ease ease)
		{
			if (!HasTravelCurve(curve))
			{
				return Shape(progress, ease);
			}
			Keyframe keyframe = curve[0];
			Keyframe keyframe2 = curve[curve.length - 1];
			float num = Mathf.Clamp01(progress);
			if (num <= 0f)
			{
				return Mathf.Clamp01(keyframe.value);
			}
			if (num >= 1f)
			{
				return Mathf.Clamp01(keyframe2.value);
			}
			return Mathf.Clamp01(curve.Evaluate(keyframe.time + num * (keyframe2.time - keyframe.time)));
		}

		private static bool HasTravelCurve(AnimationCurve curve)
		{
			if (curve != null)
			{
				return curve.length >= 2;
			}
			return false;
		}

		private static bool IsRouteEnd(float value)
		{
			if (!(Mathf.Abs(value) < 0.001f))
			{
				return Mathf.Abs(value - 1f) < 0.001f;
			}
			return true;
		}

		public static float Shape(float progress, Ease ease)
		{
			float num = Mathf.Clamp01(progress);
			if ((uint)ease <= 1u || (uint)(ease - 32) <= 5u)
			{
				return num;
			}
			return Mathf.Clamp01(DOVirtual.EasedValue(0f, 1f, num, ease, 1.70158f, 0f));
		}

		public static float PeakSlope(Func<float, float> shape)
		{
			float num = 1f;
			float num2 = shape(0f);
			for (int i = 1; i <= 128; i++)
			{
				float num3 = shape((float)i / 128f);
				num = Mathf.Max(num, Mathf.Abs(num3 - num2) * 128f);
				num2 = num3;
			}
			return num;
		}

		public static float PeakSlope(Ease ease)
		{
			return PeakSlope((float t) => Shape(t, ease));
		}

		public static float PeakSlope(AnimationCurve curve, Ease ease)
		{
			return PeakSlope((float t) => Shape(t, curve, ease));
		}

		public static Vector3 PointAt(Vector3[] points, float[] cumulative, float distance)
		{
			if (points == null || points.Length == 0)
			{
				return Vector3.zero;
			}
			if (points.Length == 1 || cumulative == null || cumulative.Length != points.Length)
			{
				return points[0];
			}
			float num = cumulative[cumulative.Length - 1];
			if (num <= 0f || distance <= 0f)
			{
				return points[0];
			}
			if (distance >= num)
			{
				return points[points.Length - 1];
			}
			int num2 = LegAt(cumulative, distance);
			float num3 = cumulative[num2];
			float num4 = cumulative[num2 + 1] - num3;
			float t = ((num4 > 0f) ? ((distance - num3) / num4) : 0f);
			return Vector3.LerpUnclamped(points[num2], points[num2 + 1], t);
		}

		private static int LegAt(float[] cumulative, float distance)
		{
			int num = Array.BinarySearch(cumulative, distance);
			if (num < 0)
			{
				num = ~num - 1;
			}
			return Mathf.Clamp(num, 0, cumulative.Length - 2);
		}

		private static Quaternion Turn(Vector3 from, Vector3 to, float distance, float radius)
		{
			if (distance <= 0f)
			{
				return Quaternion.identity;
			}
			Vector3 vector = Vector3.Cross(Vector3.up, to - from);
			if (vector.sqrMagnitude <= 1E-12f)
			{
				return Quaternion.identity;
			}
			return Quaternion.AngleAxis(distance / radius * 57.29578f, vector.normalized);
		}

		private static Quaternion[] BuildRoll(Vector3[] points, float[] cumulative, float radius)
		{
			if (radius <= 0f || points == null || points.Length < 2)
			{
				return null;
			}
			Quaternion[] array = new Quaternion[points.Length];
			array[0] = Quaternion.identity;
			for (int i = 1; i < points.Length; i++)
			{
				array[i] = Turn(points[i - 1], points[i], cumulative[i] - cumulative[i - 1], radius) * array[i - 1];
			}
			return array;
		}

		private Quaternion RollAt(float distance)
		{
			if (_roll == null)
			{
				return Quaternion.identity;
			}
			if (_totalLength <= 0f || distance <= 0f)
			{
				return _roll[0];
			}
			if (distance >= _totalLength)
			{
				return _roll[_roll.Length - 1];
			}
			int num = LegAt(_cumulative, distance);
			return Turn(_points[num], _points[num + 1], distance - _cumulative[num], RollRadius) * _roll[num];
		}

		private bool BuildRoute(Transform space, out Vector3[] points, out float[] cumulative)
		{
			points = null;
			cumulative = null;
			if (Waypoints == null || Waypoints.Length < 2)
			{
				return false;
			}
			Vector3[] array = new Vector3[Waypoints.Length];
			for (int i = 0; i < Waypoints.Length; i++)
			{
				if (!Waypoints[i])
				{
					return false;
				}
				array[i] = (space ? space.InverseTransformPoint(Waypoints[i].position) : Waypoints[i].position);
			}
			int num = array.Length - 1;
			int num2 = Mathf.Clamp(SamplesPerLeg, 1, 32);
			points = new Vector3[num * num2 + 1];
			cumulative = new float[points.Length];
			points[0] = array[0];
			int num3 = 0;
			for (int j = 0; j < num; j++)
			{
				for (int k = 1; k <= num2; k++)
				{
					points[++num3] = CatmullRom(array, j, (float)k / (float)num2);
					cumulative[num3] = cumulative[num3 - 1] + Vector3.Distance(points[num3 - 1], points[num3]);
				}
			}
			return true;
		}

		private static Vector3 CatmullRom(Vector3[] knots, int leg, float t)
		{
			Vector3 vector = knots[leg];
			Vector3 vector2 = knots[leg + 1];
			Vector3 vector3 = ((leg > 0) ? knots[leg - 1] : (vector + (vector - vector2)));
			Vector3 vector4 = ((leg + 2 < knots.Length) ? knots[leg + 2] : (vector2 + (vector2 - vector)));
			float num = t * t;
			float num2 = num * t;
			return 0.5f * (2f * vector + (-vector3 + vector2) * t + (2f * vector3 - 5f * vector + 4f * vector2 - vector4) * num + (-vector3 + 3f * vector - 3f * vector2 + vector4) * num2);
		}

		private void OnDrawGizmos()
		{
			if (BuildRoute(null, out var points, out var _))
			{
				Gizmos.color = Color.cyan;
				for (int i = 1; i < points.Length; i++)
				{
					Gizmos.DrawLine(points[i - 1], points[i]);
				}
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(points[0], 0.15f);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(points[points.Length - 1], 0.15f);
			}
		}
	}
}
