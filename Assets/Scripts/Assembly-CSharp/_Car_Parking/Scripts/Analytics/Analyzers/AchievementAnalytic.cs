using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class AchievementAnalytic : AnalyticProviderBase, IStartable, IDisposable
	{
		private const int MaxBufferedEvents = 256;

		private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromSeconds(1.0);

		private readonly CPMAnalytics _analytics;

		private readonly List<AchievementStatusContext> _buffered = new List<AchievementStatusContext>();

		private CancellationTokenSource _cts;

		public AchievementAnalytic(AnalyticEventFactory analyticEventFactory, CPMAnalytics analytics)
			: base(analyticEventFactory)
		{
			_analytics = analytics;
		}

		void IStartable.Start()
		{
			_cts = new CancellationTokenSource();
			FlushWhenReady(_cts.Token).Forget();
		}

		public void Log(AchievementStatusContext context)
		{
			if (context != null)
			{
				if (_analytics != null && _analytics.IsInitialized)
				{
					FlushBuffered();
					Send(context);
				}
				else if (_buffered.Count < 256)
				{
					_buffered.Add(context);
				}
			}
		}

		private async UniTaskVoid FlushWhenReady(CancellationToken ct)
		{
			try
			{
				while (_analytics == null || !_analytics.IsInitialized)
				{
					await UniTask.Delay(ReadyPollInterval, ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
			FlushBuffered();
		}

		private void FlushBuffered()
		{
			if (_buffered.Count == 0)
			{
				return;
			}
			List<AchievementStatusContext> list = new List<AchievementStatusContext>(_buffered);
			_buffered.Clear();
			foreach (AchievementStatusContext item in list)
			{
				Send(item);
			}
		}

		private void Send(AchievementStatusContext context)
		{
			base.AnalyticEventFactory.CreateEvent<AchievementStatusEvent, AchievementStatusContext>(context).Send();
		}

		public void Dispose()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
		}
	}
}
