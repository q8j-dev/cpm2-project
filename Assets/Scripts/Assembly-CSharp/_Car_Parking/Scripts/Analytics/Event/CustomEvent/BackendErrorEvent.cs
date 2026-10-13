using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class BackendErrorEvent : DefaultEvent<BackendErrorContext>
	{
		protected override string Name => "error_reported";

		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"error_origin",
				(base.Context.FunctionStatus < 0) ? "CLIENT" : "BACKEND"
			},
			{
				"code_reason",
				Mathf.Abs(base.Context.FunctionStatus)
			},
			{
				"error_function",
				base.Context.FunctionId
			},
			{
				"window_id",
				base.Context.WindowId
			}
		};

		[Preserve]
		public BackendErrorEvent(CPMAnalytics analytics, BackendErrorContext context)
			: base(analytics, context)
		{
		}
	}
}
