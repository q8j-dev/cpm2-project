using System;
using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class MovingPlatform : Obstacle
	{
		[SerializeField]
		[Tooltip("Point A of the travel. A child transform, moved by the designer in the scene.")]
		private Transform Start = default;

		[SerializeField]
		[Tooltip("Point B of the travel. A child transform, moved by the designer in the scene.")]
		private Transform Finish = default;

		[SerializeField]
		[Tooltip("The object that actually moves: holds the collider and the kinematic Rigidbody. StartPosition decides where along the leg it begins.")]
		private Transform Target = default;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Where along the Start->Finish leg the cycle begins: 0 = Start, 1 = Finish. In the editor Target snaps here, so the starting pose is visible without pressing Play.")]
		private float StartPosition = default;

		[Tooltip("Travel speed in units per second. Zero or less parks the platform at Start. Also parks when Start and Finish coincide, with Target snapped to Start on the first physics step.")]
		[SerializeField]
		private float Speed = 2f;

		[SerializeField]
		[Tooltip("Seconds the platform waits at each end before heading back. Zero means it never stops, so there is no moment of arrival and no FinishMove / FinishReturn sound.")]
		private float HoldSeconds = 1f;

		private Rigidbody _body;

		private Transform _space;

		private Vector3 _startLocal;

		private Vector3 _finishLocal;

		private float _travelDistance;

		private float _startProjection;

		private float _travelSeconds;

		private float _startOffset;

		private float _maxStepSqr;

		private static readonly EventType[] PhaseEvents = new EventType[4]
		{
			EventType.StartMove,
			EventType.FinishMove,
			EventType.StartReturn,
			EventType.FinishReturn
		};

		private void Awake()
		{
			if (!Start || !Finish || !Target || !Target.parent)
			{
				Debug.LogError("[MovingPlatform] " + base.name + ": Start, Finish and Target must all be assigned, and Target must be a child, not a scene root.", this);
				return;
			}
			_body = Target.GetComponent<Rigidbody>();
			if (!_body)
			{
				Debug.LogError("[MovingPlatform] " + base.name + ": Target needs a Rigidbody to push anything.", this);
				return;
			}
			_body.isKinematic = true;
			_body.interpolation = RigidbodyInterpolation.Interpolate;
			_space = Target.parent;
			_startLocal = _space.InverseTransformPoint(Start.position);
			_finishLocal = _space.InverseTransformPoint(Finish.position);
			_travelDistance = Vector3.Distance(Start.position, Finish.position);
			RebuildTravel();
		}

		private void RebuildTravel()
		{
			if ((bool)_body)
			{
				_startProjection = Mathf.Clamp01(StartPosition);
				_travelSeconds = ((Speed > 0f) ? (_travelDistance / Speed) : 0f);
				_startOffset = _startProjection * _travelSeconds;
				float num = Mathf.Max(Mathf.Max(0f, Speed) * Time.fixedDeltaTime * 4f, 0.05f);
				_maxStepSqr = num * num;
			}
		}

		public override void EvaluateAt(double elapsedSeconds)
		{
			if ((bool)_body)
			{
				double elapsedSeconds2 = elapsedSeconds + (double)_startOffset;
				int num = PhaseAt(elapsedSeconds2, _travelSeconds, HoldSeconds);
				ReportPhase(num, PhaseEvents[num]);
				float t = Progress(elapsedSeconds2, _travelSeconds, HoldSeconds);
				Vector3 position = Vector3.LerpUnclamped(_startLocal, _finishLocal, t);
				Vector3 vector = _space.TransformPoint(position);
				if ((vector - _body.position).sqrMagnitude > _maxStepSqr)
				{
					_body.position = vector;
				}
				else
				{
					_body.MovePosition(vector);
				}
			}
		}

		public static float Progress(double elapsedSeconds, float travelSeconds, float holdSeconds)
		{
			if (travelSeconds <= 0f)
			{
				return 0f;
			}
			float num = Mathf.Max(0f, holdSeconds);
			double num2 = 2.0 * (double)travelSeconds + 2.0 * (double)num;
			double num3 = elapsedSeconds - Math.Floor(elapsedSeconds / num2) * num2;
			if (num3 < (double)travelSeconds)
			{
				return (float)(num3 / (double)travelSeconds);
			}
			num3 -= (double)travelSeconds;
			if (num3 < (double)num)
			{
				return 1f;
			}
			num3 -= (double)num;
			if (num3 < (double)travelSeconds)
			{
				return 1f - (float)(num3 / (double)travelSeconds);
			}
			return 0f;
		}

		public static int PhaseAt(double elapsedSeconds, float travelSeconds, float holdSeconds)
		{
			if (travelSeconds <= 0f)
			{
				return 3;
			}
			float num = Mathf.Max(0f, holdSeconds);
			double num2 = 2.0 * (double)travelSeconds + 2.0 * (double)num;
			double num3 = elapsedSeconds - Math.Floor(elapsedSeconds / num2) * num2;
			if (num3 < (double)travelSeconds)
			{
				return 0;
			}
			num3 -= (double)travelSeconds;
			if (num3 < (double)num)
			{
				return 1;
			}
			num3 -= (double)num;
			if (num3 < (double)travelSeconds)
			{
				return 2;
			}
			return 3;
		}

		private void OnDrawGizmosSelected()
		{
			if ((bool)Start && (bool)Finish)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(Start.position, Finish.position);
			}
		}
	}
}
