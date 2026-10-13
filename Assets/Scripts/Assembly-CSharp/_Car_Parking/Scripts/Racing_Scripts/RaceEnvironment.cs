using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public static class RaceEnvironment
	{
		public readonly struct TimeSettings
		{
			public readonly int CustomHours;

			public readonly int Skybox;

			public TimeSettings(int customHours, int skybox)
			{
				CustomHours = customHours;
				Skybox = skybox;
			}
		}

		private const int CustomTimeSkybox = 5;

		public const float DefaultRainIntensity = 0.6f;

		private static bool _hasPendingRequest;

		private static WeatherType _pendingWeather;

		private static DayTimeType _pendingDayTime;

		private static float _pendingRainIntensity;

		public static void RequestForNextScene(WeatherType weatherType, DayTimeType dayTimeType, float rainIntensity = 0.6f)
		{
			_hasPendingRequest = true;
			_pendingWeather = weatherType;
			_pendingDayTime = dayTimeType;
			_pendingRainIntensity = rainIntensity;
		}

		public static void ConsumePendingRequest()
		{
			if (_hasPendingRequest)
			{
				_hasPendingRequest = false;
				int customHoursPrefs = TimeProvider.CustomHoursPrefs;
				Apply(_pendingWeather, _pendingDayTime, _pendingRainIntensity);
				TimeProvider.CustomHoursPrefs = customHoursPrefs;
			}
		}

		public static void CancelPendingRequest()
		{
			_hasPendingRequest = false;
		}

		public static TimeSettings CaptureTimeSettings()
		{
			return new TimeSettings(TimeProvider.CustomHoursPrefs, TimeProvider.skyboxSelectedRuntime);
		}

		public static void RestoreTimeSettings(TimeSettings settings)
		{
			TimeProvider.CustomHours = settings.CustomHours;
			TimeProvider.skyboxSelectedRuntime = settings.Skybox;
		}

		public static void Apply(WeatherType weatherType, DayTimeType dayTimeType, float rainIntensity = 0.6f)
		{
			SetDayTime(dayTimeType);
			SetWeather(weatherType, rainIntensity);
		}

		public static void SetDayTime(DayTimeType dayTimeType)
		{
			TimeProvider.CustomHours = (int)dayTimeType;
			TimeProvider.skyboxSelectedRuntime = 5;
			V.OnSkyboxChanged?.Invoke();
			GlobalEventSystem.RequestTimeController()?.SnapSunToTime();
		}

		public static void SetWeather(WeatherType weatherType, float rainIntensity = 0.6f)
		{
			bool flag = weatherType == WeatherType.Rain;
			if (!flag)
			{
				RainWetnessDriver.ClearForGarage();
			}
			SunAndSkyController sunAndSkyController = GlobalEventSystem.RequestSunController();
			if (sunAndSkyController != null)
			{
				sunAndSkyController.SetRainIntensity(flag ? rainIntensity : 0f, instant: true);
			}
			else
			{
				RainWetnessDriver.SetRainActive(flag);
			}
			if ((bool)SnowParticlesMover.Instance)
			{
				SnowParticlesMover.Instance.SetState(weatherType == WeatherType.Snow);
			}
		}
	}
}
