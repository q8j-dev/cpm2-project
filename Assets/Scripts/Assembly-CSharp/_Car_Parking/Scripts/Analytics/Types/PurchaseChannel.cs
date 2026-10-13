using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum PurchaseChannel
	{
		[EnumMember(Value = "garage_store")]
		GarageStore = 1,
		[EnumMember(Value = "offer")]
		Offer = 2,
		[EnumMember(Value = "multiplayer_market")]
		MultiplayerMarket = 3,
		[EnumMember(Value = "p2p_direct")]
		P2PDirect = 4,
		[EnumMember(Value = "other")]
		Other = 5
	}
}
