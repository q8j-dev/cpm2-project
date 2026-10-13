using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class AchievementStatusContext : IEventContext
	{
		public AchievementStatus Status;

		public AchievementService Service;

		public string AchievementId;

		public string PlatformPlayerId;

		public AchievementFailReason? FailReason;

		public AchievementResult? Result;

		public int? ProgressCurrent;

		public int? ProgressTarget;

		public string Source;

		public bool? IsRetry;

		public string ErrorMessage;

		public string Properties;
	}
}
