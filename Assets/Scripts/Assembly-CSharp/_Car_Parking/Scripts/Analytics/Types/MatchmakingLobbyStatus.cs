using System.Runtime.Serialization;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public enum MatchmakingLobbyStatus
	{
		[EnumMember(Value = "SEARCH_START")]
		SearchStart = 1,
		[EnumMember(Value = "ROOM_CREATED")]
		RoomCreated = 2,
		[EnumMember(Value = "ROOM_ENTERED")]
		RoomEntered = 3,
		[EnumMember(Value = "MIN_REACHED")]
		MinReached = 4,
		[EnumMember(Value = "FULL")]
		Full = 5,
		[EnumMember(Value = "LOCKED")]
		Locked = 6,
		[EnumMember(Value = "STARTED")]
		Started = 7,
		[EnumMember(Value = "LEFT")]
		Left = 8,
		[EnumMember(Value = "DISBANDED")]
		Disbanded = 9
	}
}
