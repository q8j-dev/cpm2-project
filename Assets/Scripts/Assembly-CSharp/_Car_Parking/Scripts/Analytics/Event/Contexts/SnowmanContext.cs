using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class SnowmanContext : IEventContext
	{
		public SnowmanStatus Status { get; set; }

		public string GameEventId { get; set; }

		public int CountTotal { get; set; }

		public int CountDone { get; set; }

		public float TimeLeft { get; set; }

		public float TimeLimit { get; set; }

		public string RewardId { get; set; }

		public bool RewardReceived { get; set; }

		public string MapId { get; set; }

		public string ZoneId { get; set; }

		public int AttemptNumber { get; set; }

		public string CarId { get; set; }

		public string CarClass { get; set; }

		public string ModeType { get; set; }
	}
}
