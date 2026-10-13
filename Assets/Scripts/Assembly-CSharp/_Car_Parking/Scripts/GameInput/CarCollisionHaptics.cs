using System;
using UnityEngine;

namespace _Car_Parking.Scripts.GameInput
{
	public class CarCollisionHaptics : MonoBehaviour
	{
		private GamepadHapticService _haptics;

		private HapticConfigSO _config;

		private Func<bool> _canRumble;

		private int _wheelLayer = -1;

		private float _lastPulseUnscaled = -99f;

		public void Configure(GamepadHapticService haptics, HapticConfigSO config, Func<bool> canRumble)
		{
			_haptics = haptics;
			_config = config;
			_canRumble = canRumble;
			_wheelLayer = LayerMask.NameToLayer("Wheel");
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (_haptics == null || _config == null || collision == null || collision.contactCount <= 0 || base.gameObject != V.Car || (_canRumble != null && !_canRumble()) || Time.unscaledTime - _lastPulseUnscaled < _config.CollisionCooldownSeconds)
			{
				return;
			}
			ContactPoint contact = collision.GetContact(0);
			if ((!contact.thisCollider || contact.thisCollider.gameObject.layer != _wheelLayer) && !collision.transform.CompareTag("DriverPerson") && !collision.transform.CompareTag("Road"))
			{
				Vector3 relativeVelocity = collision.relativeVelocity;
				float num = relativeVelocity.magnitude * 3.6f * Mathf.Clamp01(0.3f + Mathf.Abs(Vector3.Dot(relativeVelocity.normalized, contact.normal)));
				float num2 = Mathf.Max(0.1f, _config.CollisionFullImpactKmh - _config.CollisionMinImpactKmh);
				float num3 = Mathf.Clamp01((num - _config.CollisionMinImpactKmh) / num2);
				if (!(num3 <= 0f))
				{
					_lastPulseUnscaled = Time.unscaledTime;
					_haptics.PulseCollision(num3);
				}
			}
		}
	}
}
