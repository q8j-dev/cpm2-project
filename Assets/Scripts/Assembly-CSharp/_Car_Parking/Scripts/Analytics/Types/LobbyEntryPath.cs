using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum LobbyEntryPath
	{
		[EnumMember(Value = "quick_match")]
		QuickMatch = 1,
		[EnumMember(Value = "browse")]
		Browse = 2,
		[EnumMember(Value = "join_friend")]
		JoinFriend = 3,
		[EnumMember(Value = "create")]
		Create = 4
	}
}
