using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class DragRacingFinishEvent : DefaultEvent<DragRacingFinishContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object> { 
		{
			"mode_duration_ms",
			base.Context.Duration
		} };

		protected override string Name => "drag_racing_finish";

		public DragRacingFinishEvent(CPMAnalytics analytics, DragRacingFinishContext context)
			: base(analytics, context)
		{
		}
	}
}
