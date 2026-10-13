using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class IntegrityCheckEvent : DefaultEvent<IntegrityCheckContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object> { 
		{
			"detection_type",
			base.Context.DetectionType
		} };

		protected override string Name => "integrity_check";

		public IntegrityCheckEvent(CPMAnalytics analytics, IntegrityCheckContext context)
			: base(analytics, context)
		{
		}
	}
}
