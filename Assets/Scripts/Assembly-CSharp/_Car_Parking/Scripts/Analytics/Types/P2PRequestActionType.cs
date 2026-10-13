using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum P2PRequestActionType
	{
		[EnumMember(Value = "SELL")]
		Sell = 1,
		[EnumMember(Value = "EXCHANGE")]
		Exchange = 2
	}
}
