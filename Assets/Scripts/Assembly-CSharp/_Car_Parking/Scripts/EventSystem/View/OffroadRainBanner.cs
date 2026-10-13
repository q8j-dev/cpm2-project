using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class OffroadRainBanner : NotEventViewBase
	{
		[SerializeField]
		private Button StartButton = default;

		[SerializeField]
		private float RainIntensity = 0.6f;

		private ITimerTuningService _timerTuningService;

		private MultiPlayerMenu _multiPlayerMenu;

		[Inject]
		public void Construct(ITimerTuningService timerTuningService, MultiPlayerMenu multiPlayerMenu)
		{
			_multiPlayerMenu = multiPlayerMenu;
			_timerTuningService = timerTuningService;
		}

		protected override void Start()
		{
			base.Start();
			StartButton.onClick.AddListener(StartOffroad);
		}

		private void StartOffroad()
		{
			V.PlaceToSpawn = V.placeToSpawn.Offroad;
			GameObject gameObject = V.LastUsedCar;
			if (!gameObject)
			{
				gameObject = V.Car;
			}
			if ((bool)gameObject && gameObject.TryGetComponent<VehicleEntityProvider>(out var component))
			{
				if (component.CarEntity.GetValue().IsBroken)
				{
					_ = _managerUI.ShowNotificationWindow(new LocalizedString("SelectMapWindow.CarIsBrokenPleaseRepair"), 3f);
					return;
				}
				if (_timerTuningService.IsCarBlocked(V.SlotID))
				{
					_ = _managerUI.ShowNotificationWindow(new LocalizedString("SelectMapWindow.Yourcarcurrentlyisinservice"), 3f);
					return;
				}
			}
			_managerUI.ShowWaitingWindow(set: true).Forget();
			SetRain();
			_multiPlayerMenu.StartSingleFreeDrive();
			PlayerPrefs.SetInt("NewPlaceIconSingle" + V.placeToSpawn.Offroad, 1);
		}

		private void SetRain()
		{
			RaceEnvironment.RequestForNextScene(WeatherType.Rain, DayTimeType.Day, RainIntensity);
		}
	}
}
