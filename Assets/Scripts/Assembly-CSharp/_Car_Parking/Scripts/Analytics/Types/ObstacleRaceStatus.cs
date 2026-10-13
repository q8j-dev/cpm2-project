using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum ObstacleRaceStatus
	{
		[EnumMember(Value = "GO")]
		Go = 1,
		[EnumMember(Value = "FINISHED")]
		Finished = 2,
		[EnumMember(Value = "LEFT")]
		Left = 3,
		[EnumMember(Value = "RESULT")]
		Result = 4
	}
}
