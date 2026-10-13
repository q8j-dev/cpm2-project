using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum CarTradeRequestStatusType
	{
		[EnumMember(Value = "REQUEST_SENT")]
		RequestSent = 1,
		[EnumMember(Value = "REQUEST_SHOWN")]
		RequestShown = 2,
		[EnumMember(Value = "ACCEPTED")]
		Accepted = 3,
		[EnumMember(Value = "DECLINED")]
		Declined = 4,
		[EnumMember(Value = "IGNORED")]
		Ignored = 5,
		[EnumMember(Value = "TIMEOUT")]
		Timeout = 6,
		[EnumMember(Value = "NOT_DELIVERED_PLAYER_OFFLINE")]
		NotDeliveredPlayerOffline = 7,
		[EnumMember(Value = "BLOCKED_BY_IGNORE")]
		BlockedByIgnore = 8,
		[EnumMember(Value = "CANCELED_BY_SENDER")]
		CanceledBySender = 9
	}
}
