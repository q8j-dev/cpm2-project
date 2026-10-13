using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class CheckHashEvent : DefaultEvent<CheckHashContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"source",
				base.Context.Source
			},
			{
				"amount_retry",
				base.Context.AmountRetry
			},
			{
				"result",
				base.Context.Result
			},
			{
				"is_validated",
				base.Context.IsValidated
			},
			{
				"error",
				base.Context.Error
			}
		};

		protected override string Name => "check_hash";

		public CheckHashEvent(CPMAnalytics analytics, CheckHashContext context)
			: base(analytics, context)
		{
		}
	}
}
