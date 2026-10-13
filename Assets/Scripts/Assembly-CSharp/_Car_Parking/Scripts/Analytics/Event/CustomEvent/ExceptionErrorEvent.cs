using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class ExceptionErrorEvent : DefaultEvent<NullRefErrorContext>
	{
		protected override string Name => "null_ref_error";

		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"condition",
				base.Context.Condition
			},
			{
				"error",
				base.Context.StackTrace
			},
			{
				"scenes",
				base.Context.Scenes
			},
			{
				"error_type",
				base.Context.ErrorType
			}
		};

		public ExceptionErrorEvent(CPMAnalytics analytics, NullRefErrorContext context)
			: base(analytics, context)
		{
		}
	}
}
