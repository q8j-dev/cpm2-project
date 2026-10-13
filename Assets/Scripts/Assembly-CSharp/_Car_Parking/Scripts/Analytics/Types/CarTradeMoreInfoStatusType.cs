using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum CarTradeMoreInfoStatusType
	{
		[EnumMember(Value = "PREVIEW_OPENED")]
		PreviewOpened = 1,
		[EnumMember(Value = "MORE_INFO_CLICKED")]
		MoreInfoClicked = 2,
		[EnumMember(Value = "PURCHASED")]
		Purchased = 3,
		[EnumMember(Value = "CANCELED_BY_USER")]
		CanceledByUser = 4,
		[EnumMember(Value = "INSUFFICIENT_HARD")]
		InsufficientHard = 5
	}
}
