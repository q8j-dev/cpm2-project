using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class RaceStartEvent : DefaultEvent<RaceStartContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"car_id",
				base.Context.CarId
			},
			{
				"racing_mode",
				base.Context.RaceMode
			}
		};

		protected override string Name => "race_start";

		public RaceStartEvent(CPMAnalytics analytics, RaceStartContext context)
			: base(analytics, context)
		{
		}
	}
}
