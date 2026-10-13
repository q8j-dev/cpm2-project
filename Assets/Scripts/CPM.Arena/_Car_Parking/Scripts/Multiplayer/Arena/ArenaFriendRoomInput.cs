namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ArenaFriendRoomInput
	{
		public readonly string UserId;

		public readonly string RoomName;

		public ArenaFriendRoomInput(string userId, string roomName)
		{
			UserId = userId;
			RoomName = roomName;
		}
	}
}
