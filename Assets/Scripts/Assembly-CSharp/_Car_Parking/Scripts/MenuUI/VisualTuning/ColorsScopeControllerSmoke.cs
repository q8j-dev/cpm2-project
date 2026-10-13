using System;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class ColorsScopeControllerSmoke : ColorsScopeController
	{
		private IDisposable _showSmokeUpdate;

		private ParticleSystem.EmitParams _smokeEmitParams;

		private ColorsScopeSmokeConfig _colorsScopeSmokeConfig;

		public override ColorsScope.ColorsType ColorsType => ColorsScope.ColorsType.Smoke;

		public ColorsScopeControllerSmoke(ColorsScopeSmokeConfig colorsScopeSmokeConfig, ManagerUI managerUI)
			: base(managerUI)
		{
			_colorsScopeSmokeConfig = colorsScopeSmokeConfig;
		}

		public override void Disable()
		{
			HideSmoke();
		}

		public override void SetColor(CarPaintColor carPaintColor)
		{
			carPaintColor.Color = new Color(carPaintColor.Color.r, carPaintColor.Color.g, carPaintColor.Color.b, 1f);
			_smokeEmitParams.startColor = carPaintColor.Color;
			ShowSmoke();
		}

		public override void Apply(CarPaintColor carPaintColor)
		{
			carInfo.Colors.SmokeColor = carPaintColor.Color;
			carInfo.Save();
			PlayerDatas.SetSmokeColor(vehicleData.Axles, carPaintColor.Color);
			managerUI.ShowNotificationWindow(new LocalizedString("ColorsScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallPaintAcceptSwitch).Forget();
		}

		public override void Discard()
		{
			_smokeEmitParams.startColor = carInfo.Colors.SmokeColor;
			PlayerDatas.SetSmokeColor(vehicleData.Axles, carInfo.Colors.SmokeColor);
		}

		public override void SetDefaultColor()
		{
			Color white = Color.white;
			SetColor(new CarPaintColor(white));
			carInfo.Colors.SmokeColor = white;
			carInfo.Save();
		}

		public override void Revert()
		{
			Discard();
			HideSmoke();
		}

		private void ShowSmoke()
		{
			if (_showSmokeUpdate != null)
			{
				return;
			}
			PlayerDatas.CreateSkid(vehicleData.gameObject);
			HideSmoke();
			float lastTimeSmokeSpawn = Time.time;
			_showSmokeUpdate = Observable.EveryUpdate().Subscribe((long x) =>
			{
				if ((bool)vehicleData && (bool)vehicleData.WheelComposition.SkidSmoke && (bool)vehicleData.Axles.RearAxle.LeftWheel && (bool)vehicleData.Axles.RearAxle.RightWheel)
				{
					int num = Mathf.RoundToInt((Time.time - lastTimeSmokeSpawn) / (1f / _colorsScopeSmokeConfig.SmokeParticleAmountPerSec));
					for (int i = 0; i < num; i++)
					{
						_smokeEmitParams.startLifetime = _colorsScopeSmokeConfig.GetRandomLifeTime();
						_smokeEmitParams.startSize = _colorsScopeSmokeConfig.GetRandomSize();
						_smokeEmitParams.position = _colorsScopeSmokeConfig.GetPosition(vehicleData);
						_smokeEmitParams.velocity = _colorsScopeSmokeConfig.GetVelocity(vehicleData);
						vehicleData.WheelComposition.SkidSmoke.Emit(_smokeEmitParams, 1);
					}
					if (num > 0)
					{
						lastTimeSmokeSpawn = Time.time;
					}
				}
			});
		}

		private void HideSmoke()
		{
			_showSmokeUpdate?.Dispose();
			_showSmokeUpdate = null;
		}
	}
}
