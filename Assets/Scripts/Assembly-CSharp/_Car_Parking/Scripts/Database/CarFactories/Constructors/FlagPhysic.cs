using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Database.CarFactories.Constructors
{
	public class FlagPhysic : MonoBehaviour
	{
		[SerializeField]
		private float SmoothTime = 0.3f;

		[SerializeField]
		private float OscillationSpeedThreshold = 15f;

		[SerializeField]
		private float OscillationAmplitude = 5f;

		[SerializeField]
		private float OscillationFrequency = 3f;

		private Vector3 _currentExternalAcceleration;

		private Vector3 _velocity;

		private float _oscillationTime;

		private Rigidbody _source;

		private Cloth _flagCloth;

		public void Initialize(Rigidbody source, Cloth flagCloth)
		{
			_source = source;
			_flagCloth = flagCloth;
		}

		private void FixedUpdate()
		{
			Vector3 target = -_source.velocity;
			float magnitude = _source.velocity.magnitude;
			_currentExternalAcceleration = Vector3.SmoothDamp(_currentExternalAcceleration, target, ref _velocity, SmoothTime, float.PositiveInfinity, Time.fixedDeltaTime);
			if (magnitude > OscillationSpeedThreshold)
			{
				_oscillationTime += Time.fixedDeltaTime;
				float num = Mathf.InverseLerp(OscillationSpeedThreshold, OscillationSpeedThreshold * 1.5f, magnitude);
				float num2 = Mathf.Sin(_oscillationTime * OscillationFrequency * (float)Math.PI * 2f) * OscillationAmplitude * num;
				_currentExternalAcceleration += Vector3.forward * num2;
			}
			if ((bool)_flagCloth)
			{
				_flagCloth.externalAcceleration = _currentExternalAcceleration;
			}
		}
	}
}
