using System;
using UnityEngine;

namespace CPM.Obstacles
{
	[Serializable]
	public sealed class RagdollImpact
	{
		[Tooltip("Off means this obstacle only pushes the player around and never knocks them down.")]
		[SerializeField]
		private bool RagdollOnHit = true;

		[Tooltip("How fast the part that touches the player has to move, in units per second, for the hit to knock them down. Below it the obstacle just shoves them.")]
		[SerializeField]
		private float MinImpactSpeed = 2f;

		[Tooltip("Ragdoll force per unit of impact speed. Higher throws the body further; the ragdoll scales it once more by its own forceAdjust.")]
		[SerializeField]
		private float ForcePerSpeed = 400f;

		[Tooltip("Upper bound on the force, whatever the speed. Keeps one oversized step from launching the player out of the level.")]
		[SerializeField]
		private float MaxForce = 6000f;

		private bool _sampled;

		private Vector3 _position;

		private Quaternion _rotation = Quaternion.identity;

		private Vector3 _linear;

		private Vector3 _angular;

		public void Sample(Vector3 position, Quaternion rotation, float deltaTime)
		{
			if (_sampled && deltaTime > 0f)
			{
				_linear = (position - _position) / deltaTime;
				_angular = AngularVelocity(_rotation, rotation, deltaTime);
			}
			_position = position;
			_rotation = rotation;
			_sampled = true;
		}

		public void Reset()
		{
			_sampled = false;
			_linear = Vector3.zero;
			_angular = Vector3.zero;
		}

		public Vector3 VelocityAt(Vector3 point)
		{
			return _linear + Vector3.Cross(_angular, point - _position);
		}

		public bool TryGetImpact(Vector3 contactPoint, out Vector3 impulse)
		{
			impulse = Vector3.zero;
			if (!RagdollOnHit)
			{
				return false;
			}
			Vector3 vector = VelocityAt(contactPoint);
			float magnitude = vector.magnitude;
			if (magnitude <= 0f || magnitude < Mathf.Max(0f, MinImpactSpeed))
			{
				return false;
			}
			float num = Mathf.Min(magnitude * Mathf.Max(0f, ForcePerSpeed), Mathf.Max(0f, MaxForce));
			if (num <= 0f)
			{
				return false;
			}
			impulse = vector * (num / magnitude);
			return true;
		}

		public static Vector3 AngularVelocity(Quaternion from, Quaternion to, float deltaTime)
		{
			if (deltaTime <= 0f)
			{
				return Vector3.zero;
			}
			Quaternion quaternion = to * Quaternion.Inverse(from);
			if (quaternion.w < 0f)
			{
				quaternion = new Quaternion(0f - quaternion.x, 0f - quaternion.y, 0f - quaternion.z, 0f - quaternion.w);
			}
			Vector3 vector = new Vector3(quaternion.x, quaternion.y, quaternion.z);
			float magnitude = vector.magnitude;
			if (magnitude < 1E-06f)
			{
				return Vector3.zero;
			}
			float num = 2f * Mathf.Atan2(magnitude, quaternion.w);
			return vector * (num / (magnitude * deltaTime));
		}
	}
}
