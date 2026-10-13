using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.GooglePlayServices
{
	public readonly struct UnlockResult
	{
		public bool Success { get; }

		public AchievementFailReason? FailReason { get; }

		private UnlockResult(bool success, AchievementFailReason? failReason)
		{
			Success = success;
			FailReason = failReason;
		}

		public static UnlockResult Succeeded()
		{
			return new UnlockResult(success: true, null);
		}

		public static UnlockResult Failed(AchievementFailReason failReason)
		{
			return new UnlockResult(success: false, failReason);
		}
	}
}
