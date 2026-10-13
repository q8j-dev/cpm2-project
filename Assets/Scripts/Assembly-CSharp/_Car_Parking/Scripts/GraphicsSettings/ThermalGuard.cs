using UnityEngine;

namespace _Car_Parking.Scripts.GraphicsSettings
{
	public class ThermalGuard : MonoBehaviour
	{
		[Tooltip("FPS при перегреве")]
		public int ThrottledFps = 30;

		[Tooltip("Интервал опроса термалки, сек (не чаще ~1с — иначе API вернёт NaN)")]
		public float PollSeconds = 5f;

		[Tooltip("headroom для входа в троттлинг (1.0 = у порога троттлинга ОС, >1 = уже троттлит)")]
		[Range(0.5f, 1.2f)]
		public float ThrottleAt = 0.92f;

		[Tooltip("headroom для выхода из троттлинга (гистерезис)")]
		[Range(0.3f, 1f)]
		public float RecoverAt = 0.75f;

		[Tooltip("Горизонт прогноза для getThermalHeadroom, сек")]
		public int ForecastSeconds = 10;

		private bool _throttled;

		private float _timer;

		private void OnEnable()
		{
		}

		public void SetThrottled(bool on)
		{
			_throttled = on;
			Application.targetFrameRate = (on ? ThrottledFps : ResolveNormalFps());
			Debug.Log($"[ThermalGuard] throttled={on} → targetFrameRate={Application.targetFrameRate}");
		}

		private int ResolveNormalFps()
		{
			CPMGraphicsSettingsManager instance = CPMGraphicsSettingsManager.Instance;
			if (instance != null && instance.MaxFramerate > 0)
			{
				return instance.MaxFramerate;
			}
			if (Application.targetFrameRate <= 0)
			{
				return 60;
			}
			return Application.targetFrameRate;
		}

		private float ReadHeadroom()
		{
			return float.NaN;
		}
	}
}
