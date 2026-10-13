using UnityEngine;

namespace _Car_Parking.Scripts.Steam.Perf
{
	public static class SteamReflectionCubeThrottle
	{
		private enum SpeedBand
		{
			Slow = 0,
			Mid = 1,
			Fast = 2
		}

		private const int FaceCount = 6;

		private const int AllFacesMask = 63;

		private const float FastFacesPerSecond = 60f;

		private const float MidFacesPerSecond = 30f;

		private const float SlowFacesPerSecond = 20f;

		private const float DefaultMaxFacesPerFrame = 1f;

		private static float _maxFacesPerFrame = 1f;

		private const float HighFpsRateMultiplier = 1.5f;

		private const float HighFpsOn = 105f;

		private const float HighFpsOff = 95f;

		private const float TwoFacesFpsOn = 65f;

		private const float TwoFacesFpsOff = 55f;

		private const float TwoFacesRateMultiplier = 0.5f;

		private const float SlowSpeedMps = 1.5f;

		private const float FastSpeedMps = 6f;

		private const float Hysteresis = 0.5f;

		private static readonly int[] FaceSchedule = new int[12]
		{
			2, 0, 4, 2, 1, 5, 2, 0, 4, 3,
			1, 5
		};

		private static int _nextFace;

		private static float _faceBudget;

		private static GameObject _car;

		private static Rigidbody _carBody;

		private static SpeedBand _band = SpeedBand.Slow;

		private static bool _highFps;

		private static bool _twoFacesFps;

		public static float MaxFacesPerFrame
		{
			get
			{
				return _maxFacesPerFrame;
			}
			set
			{
				_maxFacesPerFrame = Mathf.Clamp(value, 1f, 6f);
			}
		}

		public static bool TryGetFaceMask(int quality, out int faceMask)
		{
			if (!SteamPerfFlags.IsOn("reflCube"))
			{
				faceMask = 1 << Time.frameCount % 6;
				return true;
			}
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			SpeedBand band = UpdateSpeedBand();
			float num = Mathf.Max(_maxFacesPerFrame, TwoFacesCap(quality, band));
			_faceBudget = Mathf.Min(_faceBudget + unscaledDeltaTime * FacesPerSecond(quality, band, num), num);
			faceMask = 0;
			int num2 = 0;
			while (_faceBudget >= 1f)
			{
				_faceBudget--;
				faceMask |= 1 << FaceSchedule[_nextFace];
				_nextFace = (_nextFace + 1) % FaceSchedule.Length;
				num2++;
			}
			return (faceMask & 0x3F) != 0;
		}

		private static float FacesPerSecond(int quality, SpeedBand band, float cap)
		{
			float num;
			switch (band)
			{
			case SpeedBand.Slow:
				return 20f;
			default:
				num = ((quality >= 2) ? 30f : 20f);
				break;
			case SpeedBand.Fast:
				num = 60f * (1f + (cap - 1f) * 0.5f);
				break;
			}
			float num2 = num;
			if (!UpdateHighFps())
			{
				return num2;
			}
			return num2 * 1.5f;
		}

		private static float TwoFacesCap(int quality, SpeedBand band)
		{
			if (quality < 3 || band != SpeedBand.Fast)
			{
				return 1f;
			}
			float smoothDeltaTime = Time.smoothDeltaTime;
			float num = ((smoothDeltaTime > 0f) ? (1f / smoothDeltaTime) : 0f);
			if (_twoFacesFps ? (num < 55f) : (num > 65f))
			{
				_twoFacesFps = !_twoFacesFps;
			}
			if (!_twoFacesFps)
			{
				return 1f;
			}
			return 2f;
		}

		private static bool UpdateHighFps()
		{
			float smoothDeltaTime = Time.smoothDeltaTime;
			float num = ((smoothDeltaTime > 0f) ? (1f / smoothDeltaTime) : 0f);
			if (_highFps ? (num < 95f) : (num > 105f))
			{
				_highFps = !_highFps;
			}
			return _highFps;
		}

		private static SpeedBand UpdateSpeedBand()
		{
			GameObject car = V.Car;
			if (car != _car)
			{
				_car = car;
				_carBody = (car ? car.GetComponent<Rigidbody>() : null);
			}
			float num = (_carBody ? _carBody.velocity.magnitude : 0f);
			SpeedBand band;
			switch (_band)
			{
			case SpeedBand.Slow:
				if (num > 2f)
				{
					band = ((!(num > 6f)) ? SpeedBand.Mid : SpeedBand.Fast);
					break;
				}
				goto default;
			case SpeedBand.Mid:
				if (num > 6.5f)
				{
					band = SpeedBand.Fast;
					break;
				}
				if (num < 1f)
				{
					band = SpeedBand.Slow;
					break;
				}
				goto default;
			case SpeedBand.Fast:
				if (num < 5.5f)
				{
					band = ((!(num < 1.5f)) ? SpeedBand.Mid : SpeedBand.Slow);
					break;
				}
				goto default;
			default:
				band = _band;
				break;
			}
			_band = band;
			return _band;
		}
	}
}
