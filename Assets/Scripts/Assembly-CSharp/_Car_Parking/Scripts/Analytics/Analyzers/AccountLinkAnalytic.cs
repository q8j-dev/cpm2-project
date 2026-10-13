using CPM.Account;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class AccountLinkAnalytic : AnalyticProviderBase
	{
		public AccountLinkAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogAccountLink(AccountLinkStatus status, string steamId, string steamAccountId, string steamLanguage, int steamLevel, string failReason = null)
		{
			string text = AnalyticsDependencies.Get<IAccountController>()?.Email;
			bool isSteamOnlyAccount = !string.IsNullOrEmpty(text) && AccountSystemV2Extension.IsSteamOnlyAccount(text);
			base.AnalyticEventFactory.CreateEvent<AccountLinkEvent, AccountLinkContext>(new AccountLinkContext
			{
				Status = status.ToString(),
				AuthProvider = "steam",
				SteamId = steamId,
				SteamAccountId = steamAccountId,
				IsSteamOnlyAccount = isSteamOnlyAccount,
				SteamLanguage = steamLanguage,
				SteamLevel = steamLevel,
				FailReason = failReason
			}).Send();
		}
	}
}
