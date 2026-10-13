using System.Text.RegularExpressions;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class ProfileAnalytic : AnalyticProviderBase
	{
		public ProfileAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogRegistration(RegistrationStatus status, string email)
		{
			base.AnalyticEventFactory.CreateEvent<ProfileRegistrationEvent, ProfileContext>(new ProfileContext
			{
				Status = status.ToString(),
				AuthProvider = "email"
			}).Send();
		}

		public void LogLogin(LoginStatus status, string email, string authProvider = "email")
		{
			base.AnalyticEventFactory.CreateEvent<ProfileLoginEvent, ProfileContext>(new ProfileContext
			{
				Status = status.ToString(),
				AuthProvider = authProvider
			}).Send();
		}

		public void LogLogout(long sessionDuration)
		{
			base.AnalyticEventFactory.CreateEvent<LogOutEvent, ProfileContext>(new ProfileContext
			{
				SessionDuration = sessionDuration
			}).Send();
		}

		private string ExtractDomain(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return null;
			}
			email = email.Trim();
			Match match = Regex.Match(email, "@([A-Za-z0-9.-]+\\.[A-Za-z]{2,})", RegexOptions.IgnoreCase);
			if (match.Success)
			{
				return match.Groups[1].Value.ToLower();
			}
			return null;
		}
	}
}
