using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class MigrationAnalytic : AnalyticProviderBase
	{
		public MigrationAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogMigration(MigrationStatus status, string reason = "", int duration = 0)
		{
			base.AnalyticEventFactory.CreateEvent<MigrationEvent, MigrationContext>(new MigrationContext
			{
				Status = status.ToString(),
				Reason = reason,
				DurationMs = duration
			}).Send();
		}
	}
}
