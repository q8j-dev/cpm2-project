using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class MigrationEvent : DefaultEvent<MigrationContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"status",
				base.Context.Status
			},
			{
				"reason",
				base.Context.Reason
			},
			{
				"duration_ms",
				base.Context.DurationMs
			}
		};

		protected override string Name => "anonymous_migration";

		public MigrationEvent(CPMAnalytics analytics, MigrationContext context)
			: base(analytics, context)
		{
		}
	}
}
