using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum LobbyFailReason
	{
		[EnumMember(Value = "USER_LEFT")]
		UserLeft = 1,
		[EnumMember(Value = "DISCONNECT")]
		Disconnect = 2,
		[EnumMember(Value = "ERROR")]
		Error = 3,
		[EnumMember(Value = "MIN_NOT_REACHED")]
		MinNotReached = 4,
		[EnumMember(Value = "NO_ACCOUNT")]
		NoAccount = 5,
		[EnumMember(Value = "ELIMINATED")]
		Eliminated = 6
	}
}
