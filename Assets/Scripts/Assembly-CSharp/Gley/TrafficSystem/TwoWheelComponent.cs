using System;
using UnityEngine;

namespace Gley.TrafficSystem
{
	public class TwoWheelComponent : VehicleComponent
	{
		[Header("2 Wheel Vehicle Properties")]
		[SerializeField]
		[Tooltip("The maximum allowed tilt (lean) angle in degrees during turns.")]
		private float _maxTilt = 15f;

		[SerializeField]
		[Tooltip("How strongly the vehicle resists falling sideways. Higher = more upright stability.")]
		private float _stabilityFactor = 1500f;

		[SerializeField]
		[Tooltip("How much angular velocity is damped on the Z axis to prevent oscillation.")]
		private float _damping = 0.1f;

		private float _targetAngle;

		private float _lerpSpeed;

		private float _targetZRotation;

		private Vector3 _localAngularVelocity;

		public override void ApplyAdditionalForces(float wheelTurnAngle)
		{
			base.ApplyAdditionalForces(wheelTurnAngle);
			_localAngularVelocity = rb.transform.InverseTransformDirection(rb.angularVelocity);
			_localAngularVelocity.z *= 1f - _damping;
			rb.angularVelocity = rb.transform.TransformDirection(_localAngularVelocity);
			Vector3 torque = Vector3.Project(Vector3.Cross(rb.transform.up, Vector3.up) * _stabilityFactor, rb.transform.forward);
			rb.AddTorque(torque, ForceMode.Acceleration);
			_lerpSpeed = ((Mathf.Abs(wheelTurnAngle) > Mathf.Abs(_targetAngle)) ? 1f : 3f);
			_targetAngle = Mathf.Lerp(_targetAngle, wheelTurnAngle, _lerpSpeed * Time.fixedDeltaTime);
			float num = wheelDistance / Mathf.Tan(_targetAngle * ((float)Math.PI / 180f));
			float num2 = Mathf.Atan(GetCurrentSpeedMS() * GetCurrentSpeedMS() / (num * Mathf.Abs(Physics.gravity.y)));
			_targetZRotation = 0f - Mathf.Clamp(num2 * 57.29578f, 0f - _maxTilt, _maxTilt);
			carHolder.localEulerAngles = new Vector3(0f, 0f, _targetZRotation);
		}
	}
}
