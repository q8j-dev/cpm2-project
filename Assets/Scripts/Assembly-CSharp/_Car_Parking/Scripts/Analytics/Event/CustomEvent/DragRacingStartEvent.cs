using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class DragRacingStartEvent : DefaultEvent<DefaultContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object> { 
		{
			"car_id",
			AnalyticsHelper.GetCurrentCarId()
		} };

		protected override string Name => "drag_racing_start";

		public DragRacingStartEvent(CPMAnalytics analytics, DefaultContext context)
			: base(analytics, context)
		{
		}
	}
}
