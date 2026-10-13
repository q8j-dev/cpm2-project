using System;
using System.Collections.Generic;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Root.Batching;
using _Car_Parking.Scripts.Analytics.Root.Data;
using _Car_Parking.Scripts.RemoteConfig;

namespace _Car_Parking.Scripts.Analytics.Root
{
	[Preserve]
	public class CPMAnalytics : IDisposable
	{
		private const string LastSendTimeKey = "CPM_A_lastSendTime";

		private readonly IRemoteConfigManager _remoteConfigManager;

		private readonly AnalyticConfig _analyticConfig;

		private AnalyticEventStorage _eventStorage;

		private BatchTracker _batchTracker;

		private DateTime _lastSendTime;

		private IDisposable _eventSender;

		public static AnalyticCoreSetting AnalyticSetting { get; private set; }

		public bool IsInitialized { get; private set; }

		public CPMAnalytics(IRemoteConfigManager remoteConfigManager, AnalyticConfig analyticConfig)
		{
			_remoteConfigManager = remoteConfigManager;
			_analyticConfig = analyticConfig;
		}

		public async UniTask Initialize(Env environment)
		{
			SetEnv(environment);
			LoadLastSendTime();
			await SetSetting(environment);
			_eventStorage = new AnalyticEventStorage(AnalyticSetting);
			_batchTracker = new BatchTracker(_eventStorage, AnalyticSetting);
			_eventStorage.Initialize();
			_eventSender = Observable.Interval(TimeSpan.FromSeconds(AnalyticSetting.SendCooldown)).Subscribe((long _) =>
			{
				SendBatches().Forget();
			});
			IsInitialized = true;
		}

		private async UniTask SetSetting(Env environment)
		{
			if (environment == Env.prod)
			{
				AnalyticsRemoteConfig analytics = (await _remoteConfigManager.GetConfigValuesStorage()).analytics;
				AnalyticSetting = new AnalyticCoreSetting
				{
					BatchSize = analytics.eventBatchSize,
					EventTTL = analytics.eventTTL,
					SendCooldown = analytics.eventSendCooldown
				};
			}
			else
			{
				AnalyticSetting = new AnalyticCoreSetting(_analyticConfig.AnalyticDevSetting);
			}
		}

		public void FlushPending()
		{
			if (IsInitialized)
			{
				_eventStorage.FlushToBatch();
				SendBatches(ignoreCooldown: true).Forget();
			}
		}

		public void LogEvent(Parameter[] parameters)
		{
			_eventStorage.AddEvent(parameters);
		}

		private void SetEnv(Env environment)
		{
			AnalyticsTransport.SetEnv(environment.ToString());
		}

		private async UniTask SendBatches(bool ignoreCooldown = false)
		{
			Batch processedBatch = _batchTracker.FirstBatch;
			if (processedBatch == null || (!ignoreCooldown && (DateTime.Now - _lastSendTime).TotalSeconds < (double)AnalyticSetting.SendCooldown))
			{
				return;
			}
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (Event @event in processedBatch.Events)
			{
				if (!((DateTime.UtcNow - @event.SendTime).TotalSeconds >= (double)AnalyticSetting.EventTTL))
				{
					list.Add(@event.ToDictionary());
				}
			}
			if (list.Count == 0)
			{
				_batchTracker.RemoveBatch(processedBatch.Uuid);
			}
			else if (await AnalyticsTransport.SendBatchAsync(list))
			{
				_batchTracker.RemoveBatch(processedBatch.Uuid);
				SetLastSendTime();
			}
		}

		private void SetLastSendTime()
		{
			_lastSendTime = DateTime.Now;
			PlayerPrefs.SetString("CPM_A_lastSendTime", _lastSendTime.Ticks.ToString());
			PlayerPrefs.Save();
		}

		private void LoadLastSendTime()
		{
			if (PlayerPrefs.HasKey("CPM_A_lastSendTime"))
			{
				long ticks = long.Parse(PlayerPrefs.GetString("CPM_A_lastSendTime"));
				_lastSendTime = new DateTime(ticks);
			}
			else
			{
				_lastSendTime = DateTime.MinValue;
			}
		}

		public void Dispose()
		{
			IsInitialized = false;
			_batchTracker?.Dispose();
			_eventSender?.Dispose();
		}
	}
}
