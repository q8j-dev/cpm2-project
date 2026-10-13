using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonPlatformRider
	{
		private const bool DiagPlatformRide = false;

		private readonly Rigidbody _rigidbody;

		private readonly PersonLocomotionConfig _config;

		private int _platformId;

		private bool _hasCache;

		private Vector3 _cachedLocalPoint;

		private Vector3 _cachedWorldPoint;

		private Quaternion _cachedRotation;

		private bool _sampled;

		private Vector3 _sampledPosition;

		private Quaternion _sampledRotation;

		private Vector3 _carryVelocity;

		private float _yawDelta;

		private Vector3 _lastValidCarry;

		private bool _canLatchExit;

		private Vector3 _pendingExit;

		private bool _hasPendingExit;

		private const int DiagBurstTicks = 60;

		private string _diagName;

		private int _diagBurst;

		public Vector3 CarryVelocity => _carryVelocity;

		public float YawDelta => _yawDelta;

		public PersonPlatformRider(Rigidbody rigidbody, PersonLocomotionConfig config)
		{
			_rigidbody = rigidbody;
			_config = config;
		}

		public void Sample(Collider groundCollider, bool grounded)
		{
			_carryVelocity = Vector3.zero;
			_yawDelta = 0f;
			_sampled = false;
			if (!_rigidbody || !_config || !_config.PlatformRideEnabled)
			{
				Release(latch: false);
				return;
			}
			if (!grounded || !groundCollider)
			{
				Release(latch: true);
				return;
			}
			Rigidbody attachedRigidbody = groundCollider.attachedRigidbody;
			Transform transform = groundCollider.transform;
			_sampledPosition = (attachedRigidbody ? attachedRigidbody.position : transform.position);
			_sampledRotation = (attachedRigidbody ? attachedRigidbody.rotation : transform.rotation);
			_sampled = true;
			int num = (attachedRigidbody ? attachedRigidbody.GetInstanceID() : transform.GetInstanceID());
			if (!_hasCache || num != _platformId)
			{
				_platformId = num;
				_hasCache = false;
				_canLatchExit = false;
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (fixedDeltaTime <= 0f)
			{
				return;
			}
			Vector3 vector = _sampledPosition + _sampledRotation * _cachedLocalPoint - _cachedWorldPoint;
			Vector3 vector2 = vector / fixedDeltaTime;
			if (vector2.magnitude > _config.PlatformMaxCarrySpeed)
			{
				_canLatchExit = false;
				return;
			}
			_carryVelocity = vector2;
			_lastValidCarry = vector2;
			_canLatchExit = true;
			if (_config.PlatformRotationCarryEnabled)
			{
				_yawDelta = TwistAboutUp(_sampledRotation * Quaternion.Inverse(_cachedRotation));
			}
		}

		public void Commit()
		{
			if (_sampled && (bool)_rigidbody)
			{
				Vector3 position = _rigidbody.position;
				_cachedLocalPoint = Quaternion.Inverse(_sampledRotation) * (position - _sampledPosition);
				_cachedWorldPoint = position;
				_cachedRotation = _sampledRotation;
				_hasCache = true;
			}
		}

		public void Reset()
		{
			_hasCache = false;
			_sampled = false;
			_platformId = 0;
			_carryVelocity = Vector3.zero;
			_yawDelta = 0f;
			_lastValidCarry = Vector3.zero;
			_canLatchExit = false;
			_pendingExit = Vector3.zero;
			_hasPendingExit = false;
			_diagBurst = 0;
		}

		public bool TryConsumeExitVelocity(out Vector3 velocity)
		{
			velocity = _pendingExit;
			bool hasPendingExit = _hasPendingExit;
			_pendingExit = Vector3.zero;
			_hasPendingExit = false;
			return hasPendingExit;
		}

		private void Release(bool latch)
		{
			if (latch && _canLatchExit && _hasCache)
			{
				Vector3 lastValidCarry = _lastValidCarry;
				lastValidCarry.y = 0f;
				if (lastValidCarry.sqrMagnitude > 0.0001f)
				{
					_pendingExit = lastValidCarry;
					_hasPendingExit = true;
				}
			}
			_hasCache = false;
			_platformId = 0;
			_lastValidCarry = Vector3.zero;
			_canLatchExit = false;
		}

		private static float TwistAboutUp(Quaternion delta)
		{
			if (delta.w < 0f)
			{
				delta = new Quaternion(0f - delta.x, 0f - delta.y, 0f - delta.z, 0f - delta.w);
			}
			return 2f * Mathf.Atan2(delta.y, delta.w) * 57.29578f;
		}
	}
}
