using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class TempAbtestAnalytic : AnalyticProviderBase
	{
		private const string NewAccountSystemAnalyticsEventPrefsKey = "NewAccountSystemAnalyticsEventPrefsKey";

		private const string NewSaveCarSystemAnalyticsEventPrefsKey = "NewSaveCarSystemAnalyticsEventPrefsKey";

		public TempAbtestAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogAccountMigrateIfNeeds()
		{
			if (PlayerPrefs.GetInt("NewAccountSystemAnalyticsEventPrefsKey", 0) == 0)
			{
				PlayerPrefs.SetInt("NewAccountSystemAnalyticsEventPrefsKey", 1);
				LogStart("ACCOUNT_MIGRATE");
			}
		}

		public void LogCarSaveMigrateIfNeeds()
		{
			if (PlayerPrefs.GetInt("NewSaveCarSystemAnalyticsEventPrefsKey", 0) == 0)
			{
				PlayerPrefs.SetInt("NewSaveCarSystemAnalyticsEventPrefsKey", 1);
				LogStart("CARS_SAVE_MIGRATE");
			}
		}

		private void LogStart(string type)
		{
			base.AnalyticEventFactory.CreateEvent<TempAbtestStartEvent, TempAbtestStartContext>(new TempAbtestStartContext
			{
				Type = type
			}).Send();
		}
	}
}
