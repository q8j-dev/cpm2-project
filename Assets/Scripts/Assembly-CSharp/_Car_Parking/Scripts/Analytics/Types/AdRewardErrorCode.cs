using System;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public static class AdRewardErrorCode
	{
		public const string ServerRejected = "SERVER_REJECTED";

		public const string LimitReached = "LIMIT_REACHED";

		public const string GrantError = "GRANT_ERROR";

		public static string FromClaimStatus(string claimStatus)
		{
			if (string.IsNullOrEmpty(claimStatus))
			{
				return "SERVER_REJECTED";
			}
			if (claimStatus.IndexOf("LIMIT", StringComparison.OrdinalIgnoreCase) < 0 && claimStatus.IndexOf("CAP", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return "SERVER_REJECTED";
			}
			return "LIMIT_REACHED";
		}
	}
}
