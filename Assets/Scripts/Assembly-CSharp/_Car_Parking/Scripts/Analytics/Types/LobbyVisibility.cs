using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum LobbyVisibility
	{
		[EnumMember(Value = "open")]
		Open = 1,
		[EnumMember(Value = "friends")]
		Friends = 2
	}
}
