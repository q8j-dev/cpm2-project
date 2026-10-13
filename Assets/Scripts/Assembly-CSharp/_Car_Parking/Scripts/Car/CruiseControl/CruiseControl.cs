using System;
using System.Collections.Generic;
using CPMEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Car.CruiseControl
{
	public class CruiseControl
	{
		private const int MinLockedSpeed = 10;

		private const float SpeedFilterFactor = 0.1f;

		private List<IDisposable> _disposables = new List<IDisposable>();

		private float _filteredSpeed;

		private float _throttleSmoothVelocity;

		private float _previousThrottle;

		public ReactiveProperty<int> LockedSpeed;

		public ReactiveProperty<bool> IsEnabled;

		private readonly CoreGameManager _coreGameManager;

		private CruiseControlConfig _config;

		private Powertrain _powertrain;

		private CarInputManager _carInputManager;

		[Inject]
		public CruiseControl(CoreGameManager coreGameManager, CruiseControlConfig config, CarInputManager carInputManager)
		{
			_config = config;
			_coreGameManager = coreGameManager;
			IsEnabled = new ReactiveProperty<bool>();
			LockedSpeed = new ReactiveProperty<int>();
			_carInputManager = carInputManager;
			_carInputManager.SetCruiseControl(this);
		}

		public void SetCar(Powertrain powertrain)
		{
			if (ScenesHelper.IsFreeDriveSceneActive())
			{
				_powertrain = powertrain;
				ShifterController.OnShifterSet -= OnShifterSetHandle;
				ShifterController.OnShifterSet += OnShifterSetHandle;
				ShifterController.OnReleaseGearEvent -= OnReleaseGearHandle;
				ShifterController.OnReleaseGearEvent += OnReleaseGearHandle;
				_powertrain.OnDestroyAsObservable().Subscribe((Unit _) =>
				{
					ShifterController.OnShifterSet -= OnShifterSetHandle;
					ShifterController.OnReleaseGearEvent -= OnReleaseGearHandle;
				});
				DisableCruiseControl();
			}
		}

		public void ClearCar()
		{
			ShifterController.OnShifterSet -= OnShifterSetHandle;
			ShifterController.OnReleaseGearEvent -= OnReleaseGearHandle;
			_powertrain = null;
			DisableCruiseControl();
		}

		public void EnableToggle()
		{
			if (IsEnabled.Value)
			{
				DisableCruiseControl();
			}
			else
			{
				EnableCruiseControl();
			}
		}

		public void Plus()
		{
			if (LockedSpeed.Value != 500)
			{
				LockedSpeed.Value += _config.speedStep;
			}
		}

		public void Minus()
		{
			if (LockedSpeed.Value == 10)
			{
				DisableCruiseControl();
			}
			else
			{
				LockedSpeed.Value -= _config.speedStep;
			}
		}

		private void OnReleaseGearHandle()
		{
			if (!_powertrain.Automatic)
			{
				DisableCruiseControl();
			}
		}

		private void OnShifterSetHandle(bool setManual)
		{
			if (setManual)
			{
				DisableCruiseControl();
			}
		}

		private void EnableCruiseControl()
		{
			if (CheckMinimalSpeedToEnable(out var carSpeed))
			{
				_coreGameManager.SetMessage(new LocalizedString("CruiseControl.CruiseControlWorkWhenCarIsMoving"), 3f);
				return;
			}
			if (!FlagsStorage.PRND)
			{
				_coreGameManager.SetMessage(new LocalizedString("CruiseControl.CruiseControlDonTWorkInManualMode"), 3f);
				return;
			}
			if (!_carInputManager.GetPowertrain.Automatic)
			{
				_coreGameManager.SetMessage(new LocalizedString("CruiseControl.CruiseControlWorkOnlyInAutomaticTransmission"), 3f);
				return;
			}
			if (_carInputManager.GetPowertrain.Gear == -1)
			{
				_coreGameManager.SetMessage(new LocalizedString("CruiseControl.CruiseControlDonTWorkWithReverseGear"), 3f);
				return;
			}
			_carInputManager.IsCruiseControlEnabled = true;
			float num = carSpeed;
			num /= (float)_config.speedStep;
			LockedSpeed.Value = _config.speedStep * Mathf.RoundToInt(num);
			DisposeAll();
			Subscribes();
			IsEnabled.Value = true;
			Debug.LogFormat("Enable Cruise Control With Speed: {0}", LockedSpeed);
		}

		private void Subscribes()
		{
			_disposables.Add(Observable.EveryUpdate().Subscribe((long _) =>
			{
				SetThrottle();
			}).AddTo(_carInputManager));
		}

		private void DisableCruiseControl()
		{
			_carInputManager.IsCruiseControlEnabled = false;
			DisposeAll();
			IsEnabled.Value = false;
		}

		private void DisposeAll()
		{
			_disposables.ForEach((IDisposable x) =>
			{
				x?.Dispose();
			});
			_disposables.Clear();
		}

		private void SetThrottle()
		{
			if (CheckMinimalSpeedToEnable(out var carSpeed))
			{
				return;
			}
			if (_powertrain.Gear < 1)
			{
				DisableCruiseControl();
				return;
			}
			float num = (float)LockedSpeed.Value + _config.maxSpeedToleranceValue * _config.speedTolerance;
			float num2 = (float)LockedSpeed.Value - _config.minSpeedToleranceValue * _config.speedTolerance;
			bool flag = _powertrain.CurrentRpm >= _powertrain.RedlineRpm && carSpeed > num2;
			if ((carSpeed > num) | flag)
			{
				_carInputManager.SetThrottleForCruiseControl();
			}
			else if (carSpeed > num2)
			{
				float t = (num - carSpeed) / _config.speedTolerance;
				float throttleForCruiseControl = Mathf.Lerp(0f, 1f, t);
				_carInputManager.SetThrottleForCruiseControl(throttleForCruiseControl);
			}
			else
			{
				_carInputManager.SetThrottleForCruiseControl(1f);
			}
		}

		private bool CheckMinimalSpeedToEnable(out float carSpeed)
		{
			carSpeed = _powertrain.SpeedKmh;
			if (!FlagsStorage.Kph)
			{
				carSpeed *= 0.621371f;
			}
			if (carSpeed < _config.speedLimitForDisable)
			{
				DisableCruiseControl();
				return true;
			}
			return false;
		}

		public bool GetPlusAndMinusButtonActiveState()
		{
			return IsEnabled.Value;
		}
	}
}
