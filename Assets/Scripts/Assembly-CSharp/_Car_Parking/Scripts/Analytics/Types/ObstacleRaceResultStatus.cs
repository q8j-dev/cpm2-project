using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum ObstacleRaceResultStatus
	{
		[EnumMember(Value = "FINISHED")]
		Finished = 1,
		[EnumMember(Value = "CLAIMED")]
		Claimed = 2,
		[EnumMember(Value = "FAILED")]
		Failed = 3
	}
}
