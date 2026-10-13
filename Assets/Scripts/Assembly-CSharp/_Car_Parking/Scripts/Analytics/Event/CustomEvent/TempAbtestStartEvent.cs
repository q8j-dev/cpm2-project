using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class TempAbtestStartEvent : DefaultEvent<TempAbtestStartContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object> { 
		{
			"type",
			base.Context.Type
		} };

		protected override string Name => "temp_abtest_start";

		public TempAbtestStartEvent(CPMAnalytics analytics, TempAbtestStartContext context)
			: base(analytics, context)
		{
		}
	}
}
