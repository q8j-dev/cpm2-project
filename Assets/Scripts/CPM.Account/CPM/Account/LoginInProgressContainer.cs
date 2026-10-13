using CodeStage.AntiCheat.Storage;

namespace CPM.Account
{
	public class LoginInProgressContainer
	{
		private const string LogInInProgressKey = "LoginUnderway";

		private const string PendingFullBootstrapKey = "PendingFullBootstrap";

		public static bool IsLoginInProgress
		{
			get
			{
				return ObscuredPrefs.GetInt("LoginUnderway") == 1;
			}
			set
			{
				ObscuredPrefs.SetInt("LoginUnderway", value ? 1 : 0);
				ObscuredPrefs.Save();
			}
		}

		public static bool PendingFullBootstrap
		{
			get
			{
				return ObscuredPrefs.GetInt("PendingFullBootstrap") == 1;
			}
			set
			{
				ObscuredPrefs.SetInt("PendingFullBootstrap", value ? 1 : 0);
				ObscuredPrefs.Save();
			}
		}
	}
}
