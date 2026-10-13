using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class LogOutEvent : DefaultEvent<ProfileContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{ "status", "success" },
			{
				"session_duration",
				base.Context.SessionDuration
			}
		};

		protected override string Name => "profile_logout";

		public LogOutEvent(CPMAnalytics analytics, ProfileContext context)
			: base(analytics, context)
		{
		}
	}
}
