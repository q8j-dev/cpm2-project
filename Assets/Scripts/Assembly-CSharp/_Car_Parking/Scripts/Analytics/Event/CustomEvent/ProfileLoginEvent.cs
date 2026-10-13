using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class ProfileLoginEvent : DefaultEvent<ProfileContext>
	{
		protected override Dictionary<string, object> CustomParameters => new Dictionary<string, object>
		{
			{
				"status",
				base.Context.Status
			},
			{
				"auth_provider",
				base.Context.AuthProvider
			}
		};

		protected override string Name => "profile_login";

		public ProfileLoginEvent(CPMAnalytics analytics, ProfileContext context)
			: base(analytics, context)
		{
		}
	}
}
