using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	public class SceneLoadingTimer
	{
		private float _timeOnStartLoading;

		public void StartLoadingTimer()
		{
			_timeOnStartLoading = Time.realtimeSinceStartup;
		}

		public void StopLoadingTimer()
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartup - _timeOnStartLoading);
			_ = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
		}
	}
}
