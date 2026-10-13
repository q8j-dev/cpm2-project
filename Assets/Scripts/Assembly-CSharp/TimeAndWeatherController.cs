using System;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.ScenesLogic.WorldTime;

public class TimeAndWeatherController : MonoBehaviour
{
	public SunAndSkyController Sun;

	public float Latitude = 37.7749f;

	public float Longitude = 122.4194f;

	public const int StartYear = 2017;

	public const int StartMonth = 3;

	public const int StartDay = 20;

	public const int StartHour = 6;

	public const int StartMinute = 0;

	public const int StartSecond = 0;

	public const int DayTimeHours = 14;

	public const int EveningTimeHours = 16;

	public const int NightTimeHours = 1;

	private const float Deg2Rad = (float)Math.PI / 180f;

	private const float Rad2Deg = 57.29578f;

	private const float PI = (float)Math.PI;

	public const int TimeZoneFix = -7;

	public float TimeScale = 60f;

	public float WindDirectionDegrees;

	[Min(0f)]
	public float WindSpeed = 8f;

	private float _lastWindDirection = float.NaN;

	private float _lastWindSpeed = float.NaN;

	private float _julianDate;

	private Quaternion previousRotation;

	public float angularSpeed;

	public DateTime CurrentTime { get; private set; }

	public DateTime DayTime { get; private set; }

	public DateTime EveningTime { get; private set; }

	public DateTime NightTime { get; private set; }

	public DateTime CustomTime => new DateTime(2017, 3, 20, TimeProvider.CustomHours, 0, 0, DateTimeKind.Utc).AddHours(-7.0);

	private void Start()
	{
		TimeProvider.Init();
		CurrentTime = new DateTime(2017, 3, 20, 6, 0, 0, DateTimeKind.Utc).AddHours(-7.0);
		DayTime = new DateTime(2017, 3, 20, 14, 0, 0, DateTimeKind.Utc).AddHours(-7.0);
		EveningTime = new DateTime(2017, 3, 20, 16, 45, 0, DateTimeKind.Utc).AddHours(-7.0);
		NightTime = new DateTime(2017, 3, 20, 1, 25, 0, DateTimeKind.Utc).AddHours(-7.0);
		TimeProvider.skyboxSelectedRuntime = TimeProvider.SelectedSkyboxPrefs;
		_julianDate = GetJulianDate(CurrentTime);
		UpdateTimeHandler();
		SetPositionQuick();
		ApplyWindIfChanged();
		V.OnSkyboxChanged = (Action)Delegate.Combine(V.OnSkyboxChanged, new Action(UpdateTimeHandler));
		GlobalEventSystem.OnTimeControllerRequested -= RequestInstance;
		GlobalEventSystem.OnTimeControllerRequested += RequestInstance;
	}

	private void OnDestroy()
	{
		V.OnSkyboxChanged = (Action)Delegate.Remove(V.OnSkyboxChanged, new Action(UpdateTimeHandler));
		GlobalEventSystem.OnTimeControllerRequested -= RequestInstance;
	}

	private TimeAndWeatherController RequestInstance()
	{
		return this;
	}

	private void UpdateTimeHandler()
	{
		CurrentTime = TimeProvider.skyboxSelectedRuntime switch
		{
			0 => DayTime, 
			1 => EveningTime, 
			2 => NightTime, 
			5 => CustomTime, 
			_ => CurrentTime, 
		};
		_julianDate = GetJulianDate(CurrentTime);
	}

	public void SnapSunToTime()
	{
		SetPositionQuick();
	}

	public void SetTime(DateTime TargetTime)
	{
		CurrentTime = TargetTime;
		_julianDate = GetJulianDate(CurrentTime);
	}

	private float GetJulianDate(DateTime dateTime)
	{
		return (float)(367 * dateTime.Year - Mathf.FloorToInt((float)(7 * (dateTime.Year + Mathf.FloorToInt((float)(dateTime.Month + 9) / 12f))) / 4f) + Mathf.FloorToInt((float)(275 * dateTime.Month) / 9f) + dateTime.Day) - 730531.5f + ((float)dateTime.Hour + (float)dateTime.Minute / 60f + (float)dateTime.Second / 3600f) / 24f;
	}

	public void SetTime(int hour, int minutes = 0, int seconds = 0)
	{
		CurrentTime = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, hour, minutes, seconds, DateTimeKind.Utc).AddHours(-7.0);
		_julianDate = GetJulianDate(CurrentTime);
	}

	public void SetWeather(Weather weather)
	{
		if (Sun != null)
		{
			Sun.SetWeather(weather);
		}
	}

	private void Update()
	{
		ApplyWindIfChanged();
		double value = (double)Time.deltaTime * (double)TimeScale;
		if (TimeProvider.skyboxSelectedRuntime == 3)
		{
			CurrentTime = CurrentTime.AddSeconds(value);
			_julianDate = GetJulianDate(CurrentTime);
		}
		if (TimeProvider.skyboxSelectedRuntime == 5)
		{
			CurrentTime = CustomTime;
			_julianDate = GetJulianDate(CurrentTime);
		}
		SetPositionSmooth();
	}

	private void OnValidate()
	{
		WindSpeed = Mathf.Max(0f, WindSpeed);
		ApplyWindIfChanged();
	}

	private void ApplyWindIfChanged()
	{
		if (!(Sun == null))
		{
			float num = Mathf.Repeat(WindDirectionDegrees, 360f);
			float num2 = Mathf.Max(0f, WindSpeed);
			if (!(Mathf.Abs(Mathf.DeltaAngle(_lastWindDirection, num)) < 0.01f) || !(Mathf.Abs(_lastWindSpeed - num2) < 0.0005f))
			{
				_lastWindDirection = num;
				_lastWindSpeed = num2;
				Sun.SetCloudWind(num, num2);
			}
		}
	}

	private void SetPositionSmooth()
	{
		if ((bool)Sun)
		{
			CalculateSunPosition(_julianDate, Latitude, Longitude, out var outAzimuth, out var outAltitude, out var _);
			Vector3 euler = new Vector3((float)outAltitude, (float)outAzimuth, 0f);
			Sun.transform.localRotation = Quaternion.Lerp(Sun.transform.localRotation, Quaternion.Euler(euler), Time.deltaTime * 0.5f);
			Quaternion quaternion = Sun.transform.rotation * Quaternion.Inverse(previousRotation);
			quaternion.Normalize();
			angularSpeed = QuaternionToAngularVelocity(quaternion).magnitude;
			previousRotation = Sun.transform.rotation;
		}
	}

	private void SetPositionQuick()
	{
		if ((bool)Sun)
		{
			CalculateSunPosition(_julianDate, Latitude, Longitude, out var outAzimuth, out var outAltitude, out var _);
			Vector3 euler = new Vector3((float)outAltitude, (float)outAzimuth, 0f);
			Sun.transform.localRotation = Quaternion.Euler(euler);
			angularSpeed = 0f;
			previousRotation = Sun.transform.rotation;
		}
	}

	private Vector3 QuaternionToAngularVelocity(Quaternion quaternion)
	{
		quaternion.ToAngleAxis(out var angle, out var axis);
		angle *= (float)Math.PI / 180f;
		if (angle > (float)Math.PI)
		{
			angle -= (float)Math.PI * 2f;
		}
		return axis * angle;
	}

	private static void CalculateSunPosition(float julianDate, float latitude, float longitude, out double outAzimuth, out double outAltitude, out float outSideRealTIme)
	{
		double num = 357.5291 + 0.98560028 * (double)julianDate;
		double num2 = 280.46646 + 0.98564736 * (double)julianDate + 1.9148 * Math.Sin(num * 0.01745329238474369) + 0.02 * Math.Sin(2.0 * num * 0.01745329238474369) + 0.0003 * Math.Sin(3.0 * num * 0.01745329238474369);
		num2 %= 360.0;
		double num3 = 23.4393 - 3.563E-07 * (double)julianDate;
		double num4 = Math.Atan2(Math.Cos(num3 * 0.01745329238474369) * Math.Sin(num2 * 0.01745329238474369), Math.Cos(num2 * 0.01745329238474369)) * 57.295780181884766;
		num4 %= 360.0;
		double num5 = Math.Asin(Math.Sin(num3 * 0.01745329238474369) * Math.Sin(num2 * 0.01745329238474369)) * 57.295780181884766;
		double num6 = (280.16 + 360.9856235 * (double)julianDate) % 360.0;
		double num7 = num6 + (double)longitude - num4;
		num7 %= 360.0;
		num7 *= 0.01745329238474369;
		num5 *= 0.01745329238474369;
		latitude *= (float)Math.PI / 180f;
		outAltitude = Math.Asin(Math.Sin(latitude) * Math.Sin(num5) + Math.Cos(latitude) * Math.Cos(num5) * Math.Cos(num7)) * 57.295780181884766;
		double num8 = Math.Acos((Math.Sin(num5) - Math.Sin(outAltitude * 0.01745329238474369) * Math.Sin(latitude)) / (Math.Cos(outAltitude * 0.01745329238474369) * Math.Cos(latitude)));
		if (Math.Sin(num7) > 0.0)
		{
			num8 = 6.2831854820251465 - num8;
		}
		outAzimuth = num8 * 57.295780181884766;
		outSideRealTIme = (float)num6;
	}

	private static double CorrectAngle(double angleInRadians)
	{
		if (angleInRadians < 0.0)
		{
			return Math.PI * 2.0 - Math.Abs(angleInRadians) % (Math.PI * 2.0);
		}
		if (angleInRadians > Math.PI * 2.0)
		{
			return angleInRadians % (Math.PI * 2.0);
		}
		return angleInRadians;
	}
}
