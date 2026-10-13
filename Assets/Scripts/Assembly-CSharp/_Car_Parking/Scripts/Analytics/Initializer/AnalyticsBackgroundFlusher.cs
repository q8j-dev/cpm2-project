using System;
using CPM.Common;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Initializer
{
	[Preserve]
	public sealed class AnalyticsBackgroundFlusher : IInitializable, IDisposable
	{
		private readonly AppStateHandler _appStateHandler;

		private readonly CPMAnalytics _analytics;

		public AnalyticsBackgroundFlusher(AppStateHandler appStateHandler, CPMAnalytics analytics)
		{
			_appStateHandler = appStateHandler;
			_analytics = analytics;
		}

		public void Initialize()
		{
			_appStateHandler.ApplicationPause += OnApplicationPause;
		}

		public void Dispose()
		{
			_appStateHandler.ApplicationPause -= OnApplicationPause;
		}

		private void OnApplicationPause(bool paused)
		{
			if (paused)
			{
				_analytics.FlushPending();
			}
		}
	}
}
