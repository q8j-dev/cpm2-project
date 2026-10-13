namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class AccountLinkContext : IEventContext
	{
		public string Status;

		public string AuthProvider;

		public string SteamId;

		public string SteamAccountId;

		public bool IsSteamOnlyAccount;

		public string SteamLanguage;

		public int SteamLevel;

		public string FailReason;
	}
}
