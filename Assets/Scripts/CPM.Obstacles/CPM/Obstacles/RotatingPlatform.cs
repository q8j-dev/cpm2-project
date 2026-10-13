using System;
using UnityEngine;

namespace CPM.Obstacles
{
	public sealed class RotatingPlatform : Obstacle, IRagdollImpact
	{
		public enum RotationAxis
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		[Tooltip("The object that actually spins: holds the collider and the kinematic Rigidbody. Its authored rotation is where the spin starts.")]
		[SerializeField]
		private Transform Target = default;

		[SerializeField]
		[Tooltip("Axis of rotation in the Target's authored local space.")]
		private RotationAxis LocalAxis = RotationAxis.Y;

		[Tooltip("Spin speed in degrees per second around the selected local axis. Negative spins the other way; zero parks it at the authored angle.")]
		[SerializeField]
		private float DegreesPerSecond = 90f;

		[Tooltip("How hard the platform knocks the player down. The spin is measured about Target's origin, so the rim hits harder than the middle.")]
		[SerializeField]
		private RagdollImpact Impact = new RagdollImpact();

		private const float MaxPositionStep = 0.5f;

		private const float MaxMountStepDegrees = 45f;

		private Rigidbody _body;

		private Transform _space;

		private Quaternion _spaceRotation;

		private Vector3 _localPosition;

		private Quaternion _baseLocalRotation;

		private float _maxStepDegrees;

		private void Awake()
		{
			if (!Target)
			{
				Debug.LogError("[RotatingPlatform] " + base.name + ": Target must be assigned.", this);
				return;
			}
			_body = Target.GetComponent<Rigidbody>();
			if (!_body)
			{
				Debug.LogError("[RotatingPlatform] " + base.name + ": Target needs a Rigidbody to push anything.", this);
				return;
			}
			_body.isKinematic = true;
			_body.interpolation = RigidbodyInterpolation.Interpolate;
			_space = Target.parent;
			_spaceRotation = (_space ? _space.rotation : Quaternion.identity);
			_localPosition = Target.localPosition;
			_baseLocalRotation = Target.localRotation;
			RebuildSpin();
		}

		private void RebuildSpin()
		{
			if ((bool)_body)
			{
				_maxStepDegrees = Mathf.Max(Mathf.Abs(DegreesPerSecond) * Time.fixedDeltaTime * 4f, 5f);
			}
		}

		public override void EvaluateAt(double elapsedSeconds)
		{
			if ((bool)_body)
			{
				float angle = Angle(elapsedSeconds, DegreesPerSecond);
				Quaternion quaternion = LocalRotation(_baseLocalRotation, angle, LocalAxis);
				Quaternion quaternion2 = (_space ? _space.rotation : Quaternion.identity);
				Quaternion quaternion3 = quaternion2 * quaternion;
				Vector3 vector = (_space ? _space.TransformPoint(_localPosition) : _localPosition);
				float num = Mathf.Min(Quaternion.Angle(_spaceRotation, quaternion2), 45f);
				_spaceRotation = quaternion2;
				if (Quaternion.Angle(_body.rotation, quaternion3) > _maxStepDegrees + num || (vector - _body.position).sqrMagnitude > 0.25f)
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

		public static float Angle(double elapsedSeconds, float degreesPerSecond)
		{
			double num = elapsedSeconds * (double)degreesPerSecond;
			return (float)(num - Math.Floor(num / 360.0) * 360.0);
		}

		public static Quaternion LocalRotation(Quaternion authoredRotation, float angle, RotationAxis axis)
		{
			return authoredRotation * Quaternion.AngleAxis(angle, axis switch
			{
				RotationAxis.X => Vector3.right, 
				RotationAxis.Z => Vector3.forward, 
				_ => Vector3.up, 
			});
		}
	}
}
