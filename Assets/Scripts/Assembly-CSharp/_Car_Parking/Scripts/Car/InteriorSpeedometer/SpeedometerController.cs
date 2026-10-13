using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Car.TachometerCustom;

namespace _Car_Parking.Scripts.Car.InteriorSpeedometer
{
	public class SpeedometerController : MonoBehaviour
	{
		private const float ClampSpeedCoefficient = 100f;

		[SerializeField]
		private CarIndicatorsGraphicReferences SpeedometerGraphicsReferences;

		[SerializeField]
		[Header("Approximation Coefficients")]
		private float SpeedCoefficient1 = 1f;

		[SerializeField]
		private float SpeedCoefficient2 = 2f;

		[SerializeField]
		private float SpeedCoefficient3 = 3f;

		[SerializeField]
		private float SpeedCoefficient4 = 4f;

		private Powertrain _powertrain;

		private float _currentSpeed;

		public void Init(Powertrain powertrain)
		{
			_powertrain = powertrain;
		}

		private void LateUpdate()
		{
			if ((bool)_powertrain)
			{
				SetSpeedometerFillAmount();
			}
		}

		private void SetSpeedometerFillAmount()
		{
			if ((bool)SpeedometerGraphicsReferences.SpeedometerFillImage)
			{
				_currentSpeed = (FlagsStorage.Kph ? _powertrain.SpeedKmh : _powertrain.SpeedMph);
				float valueFromApproximation = GetValueFromApproximation(_currentSpeed);
				SpeedometerGraphicsReferences.SpeedometerFillImage.fillAmount = valueFromApproximation;
			}
		}

		private float GetValueFromApproximation(float currentSpeed)
		{
			currentSpeed /= 100f;
			return Mathf.Clamp01(PolynomialFunction(currentSpeed));
		}

		private float PolynomialFunction(float currentSpeed)
		{
			return SpeedCoefficient1 * Mathf.Pow(currentSpeed, 3f) + SpeedCoefficient2 * Mathf.Pow(currentSpeed, 2f) + SpeedCoefficient3 * currentSpeed + SpeedCoefficient4;
		}
	}
}
