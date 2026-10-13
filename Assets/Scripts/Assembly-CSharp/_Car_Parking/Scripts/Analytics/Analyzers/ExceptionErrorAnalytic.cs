using System;
using System.Collections.Generic;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public class ExceptionErrorAnalytic : AnalyticProviderBase, IInitializable, IDisposable
	{
		private readonly HashSet<string> _sentStackTraces = new HashSet<string>();

		private readonly IRemoteConfigManager _remoteConfigManager;

		private readonly HashSet<AnalyticsEnvironment> _allowedEnvironments = new HashSet<AnalyticsEnvironment>();

		[Inject]
		public ExceptionErrorAnalytic(AnalyticEventFactory analyticEventFactory, IRemoteConfigManager remoteConfigManager)
			: base(analyticEventFactory)
		{
			_remoteConfigManager = remoteConfigManager;
		}

		public void Initialize()
		{
			ApplyAllowedEnvironments(_remoteConfigManager.GetDefaultConfigStorage()?.analytics?.errorReportingEnvironments);
			LoadAllowedEnvironmentsAsync().Forget();
			Application.logMessageReceived += OnLogMessageReceived;
		}

		public void Dispose()
		{
			Application.logMessageReceived -= OnLogMessageReceived;
		}

		private async UniTaskVoid LoadAllowedEnvironmentsAsync()
		{
			List<string> list = (await _remoteConfigManager.GetConfigValuesStorage())?.analytics?.errorReportingEnvironments;
			if (list != null)
			{
				ApplyAllowedEnvironments(list);
			}
		}

		private void ApplyAllowedEnvironments(IEnumerable<string> environments)
		{
			_allowedEnvironments.Clear();
			if (environments == null)
			{
				return;
			}
			foreach (string environment in environments)
			{
				string text = environment?.Trim();
				if (!string.IsNullOrEmpty(text) && !text.Contains(",") && Enum.IsDefined(typeof(AnalyticsEnvironment), text.ToUpperInvariant()) && Enum.TryParse<AnalyticsEnvironment>(text, ignoreCase: true, out var result) && result != AnalyticsEnvironment.UNKNOWN)
				{
					_allowedEnvironments.Add(result);
				}
			}
		}

		private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
		{
			if ((type == LogType.Exception || type == LogType.Error) && _allowedEnvironments.Contains(AnalyticsEnvironmentResolver.Current) && _sentStackTraces.Add(stackTrace))
			{
				NullRefErrorContext context = new NullRefErrorContext
				{
					Condition = condition,
					StackTrace = stackTrace,
					Scenes = GetActiveSceneNames(),
					ErrorType = type.ToString()
				};
				base.AnalyticEventFactory.CreateEvent<ExceptionErrorEvent, NullRefErrorContext>(context).Send();
			}
		}

		private string GetActiveSceneNames()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				list.Add(SceneManager.GetSceneAt(i).name);
			}
			return string.Join(", ", list);
		}
	}
}
