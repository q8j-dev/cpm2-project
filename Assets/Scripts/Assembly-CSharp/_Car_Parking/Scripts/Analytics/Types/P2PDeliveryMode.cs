using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum P2PDeliveryMode
	{
		[EnumMember(Value = "NORMAL")]
		Normal = 1,
		[EnumMember(Value = "MUTED")]
		Muted = 2,
		[EnumMember(Value = "BLOCKED")]
		Blocked = 3
	}
}
