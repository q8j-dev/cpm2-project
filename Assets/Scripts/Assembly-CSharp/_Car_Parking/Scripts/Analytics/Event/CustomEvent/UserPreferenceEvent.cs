using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class UserPreferenceEvent : DefaultEvent<UserPreferenceContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"action",
				base.Context.UserPreferenceAction
			},
			{
				"old_value",
				base.Context.OldValue
			},
			{
				"new_value",
				base.Context.NewValue
			}
		};

		protected override string Name => "user_preference";

		public UserPreferenceEvent(CPMAnalytics analytics, UserPreferenceContext context)
			: base(analytics, context)
		{
		}
	}
}
