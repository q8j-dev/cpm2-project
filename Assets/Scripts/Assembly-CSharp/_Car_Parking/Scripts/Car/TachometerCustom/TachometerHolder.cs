using CPMEngine;
using Unity.Mathematics;
using UnityEngine;

namespace _Car_Parking.Scripts.Car.TachometerCustom
{
	public class TachometerHolder : MonoBehaviour
	{
		public CarIndicatorsGraphicReferences carIndicatorsGraphicReferences;

		public float Koef1 = 1f;

		public float Koef2 = 2f;

		public float Koef3 = 3f;

		public float Koef4 = 4f;

		private Powertrain _driveTrain;

		private VehicleData _vehicleData;

		private int _rpm;

		private float _powerValue;

		private float _torqueValue;

		private float _lastTimeEnabled;

		public void Init(Powertrain powertrain, VehicleData vehicleData)
		{
			_driveTrain = powertrain;
			_vehicleData = vehicleData;
		}

		public void LateUpdate()
		{
			if ((bool)_vehicleData && (bool)_driveTrain && (bool)carIndicatorsGraphicReferences)
			{
				_rpm = (int)_driveTrain.CurrentRpm;
				float redlineRpm = _driveTrain.RedlineRpm;
				float maxRpm = _driveTrain.MaxRpm;
				_driveTrain.CalcMaxPowerMaxTorque(out var _, out var _, out var maxTq, out var _, out var _);
				float srcEnd = ((maxRpm > redlineRpm) ? maxRpm : redlineRpm);
				_powerValue = math.remap(_driveTrain.EngineIdleRpm, srcEnd, 0f, 1f, _driveTrain.CurrentRpm);
				_powerValue = Mathf.Clamp01(_powerValue);
				_torqueValue = Mathf.Clamp01(_driveTrain.EngineCurTorque / maxTq);
				Calculation(carIndicatorsGraphicReferences);
			}
		}

		public void Calculation(CarIndicatorsGraphicReferences refs)
		{
			GreyZoneUpdate(refs);
			YellowZoneUpdate(refs);
			RedLightUpdate(refs);
			SetTextsAndCircles(refs);
		}

		private void GreyZoneUpdate(CarIndicatorsGraphicReferences refs)
		{
			if ((bool)refs.GrayZone)
			{
				refs.GrayZone.fillAmount = GetValueFromApproximation(_rpm);
			}
		}

		private void YellowZoneUpdate(CarIndicatorsGraphicReferences refs)
		{
			if ((bool)refs.YellowZone)
			{
				if (_rpm < 5000)
				{
					refs.YellowZone.fillAmount = 0f;
				}
				else if (_rpm < 6000)
				{
					refs.YellowZone.fillAmount = (float)(_rpm - 5000) / 1000f;
				}
				else
				{
					refs.YellowZone.fillAmount = 0f;
				}
			}
		}

		private void SetTextsAndCircles(CarIndicatorsGraphicReferences refs)
		{
			if ((bool)refs.PowerCircle)
			{
				refs.PowerCircle.fillAmount = _powerValue;
			}
			if ((bool)refs.PowerText)
			{
				refs.PowerText.text = (_powerValue * 100f).ToString("F0");
			}
			if ((bool)refs.TorqueCircle)
			{
				refs.TorqueCircle.fillAmount = _torqueValue;
			}
			if ((bool)refs.TorqueText)
			{
				refs.TorqueText.text = (_torqueValue * 100f).ToString("F0");
			}
		}

		private float GetValueFromApproximation(float rpm)
		{
			rpm /= 1000f;
			return (Koef1 * rpm * rpm * rpm + Koef2 * rpm * rpm + Koef3 * rpm + Koef4) / 1000f;
		}

		private void RedLightUpdate(CarIndicatorsGraphicReferences refs)
		{
			if ((bool)refs.RedZone)
			{
				if (_rpm < 6000)
				{
					refs.RedZone.enabled = false;
				}
				else if (Time.time - _lastTimeEnabled > 0.06f)
				{
					refs.RedZone.enabled = !refs.RedZone.enabled;
					_lastTimeEnabled = Time.time;
				}
			}
		}
	}
}
