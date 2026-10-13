using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class LevelRestartEvent : DefaultEvent<LevelContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"level_id",
				base.Context.LevelId
			},
			{
				"map_id",
				base.Context.MapId
			}
		};

		protected override string Name => "level_restart";

		public LevelRestartEvent(CPMAnalytics analytics, LevelContext context)
			: base(analytics, context)
		{
		}
	}
}
