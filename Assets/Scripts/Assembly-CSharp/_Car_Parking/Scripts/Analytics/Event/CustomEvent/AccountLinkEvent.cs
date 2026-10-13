using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class AccountLinkEvent : DefaultEvent<AccountLinkContext>
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
			},
			{
				"steam_id",
				base.Context.SteamId
			},
			{
				"steam_account_id",
				base.Context.SteamAccountId
			},
			{
				"is_steam_only_account",
				base.Context.IsSteamOnlyAccount
			},
			{
				"steam_language",
				base.Context.SteamLanguage
			},
			{
				"steam_level",
				base.Context.SteamLevel
			},
			{
				"fail_reason",
				base.Context.FailReason
			}
		};

		protected override string Name => "account_link";

		[Preserve]
		public AccountLinkEvent(CPMAnalytics analytics, AccountLinkContext context)
			: base(analytics, context)
		{
		}
	}
}
