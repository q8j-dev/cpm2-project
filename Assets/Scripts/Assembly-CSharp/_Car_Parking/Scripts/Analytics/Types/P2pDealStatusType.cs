using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum P2pDealStatusType
	{
		[EnumMember(Value = "OFFER_SENT")]
		OfferSent = 1,
		[EnumMember(Value = "BUYER_CONFIRMED")]
		BuyerConfirmed = 2,
		[EnumMember(Value = "BUYER_DECLINED")]
		BuyerDeclined = 3,
		[EnumMember(Value = "BUYER_TIMEOUT")]
		BuyerTimeout = 4,
		[EnumMember(Value = "INSUFFICIENT_FUNDS")]
		InsufficientFunds = 5,
		[EnumMember(Value = "PLAYER_OFFLINE")]
		PlayerOffline = 6,
		[EnumMember(Value = "COMPLETED")]
		Completed = 7,
		[EnumMember(Value = "ERROR_FIREBASE")]
		ErrorFirebase = 8
	}
}
