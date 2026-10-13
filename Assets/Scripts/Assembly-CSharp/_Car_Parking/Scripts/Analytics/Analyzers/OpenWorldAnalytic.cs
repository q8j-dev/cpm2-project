using System;
using Service.Taxi;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class OpenWorldAnalytic : AnalyticProviderBase, IInitializable, IDisposable
	{
		public const string ExitReason_QuitManual = "quit_manual";

		public const string ExitReason_Disconnect = "disconnect";

		public const string ExitReason_Kicked = "kicked";

		public const string ExitReason_AppQuit = "app_quit";

		private readonly LevelsProvider _levelsProvider;

		private bool _sessionInProgress;

		public OpenWorldAnalytic(LevelsProvider levelsProvider, AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
			_levelsProvider = levelsProvider;
		}

		public void Initialize()
		{
			Application.quitting += OnApplicationQuitting;
		}

		public void Dispose()
		{
			Application.quitting -= OnApplicationQuitting;
		}

		private void OnApplicationQuitting()
		{
			if (_sessionInProgress)
			{
				FinishEvent("app_quit");
			}
		}

		public void StartEvent(OpenWorldType gameMode)
		{
			_sessionInProgress = true;
			base.AnalyticEventFactory.CreateEvent<OpenWorldStartEvent, OpenWorldStartContext>(new OpenWorldStartContext
			{
				GameMode = gameMode
			}).Send();
		}

		public void FinishEvent(string exitReason)
		{
			_sessionInProgress = false;
			if ((bool)V.DriverPerson)
			{
				TaxiController component = V.DriverPerson.GetComponent<TaxiController>();
				if ((bool)component)
				{
					base.AnalyticEventFactory.CreateEvent<OpenWorldFinishEvent, OpenWorldFinishContext>(new OpenWorldFinishContext
					{
						ModeDuration = _levelsProvider.LevelScript.ModeDuration,
						TaxiOrdersCompleted = component.AllOrders,
						TaxiOrdersEarned = component.AllRewards,
						CheckpointEarned = AnalyticStorage.TallRewardInSession,
						ExitReason = exitReason
					}).Send();
				}
			}
		}
	}
}
