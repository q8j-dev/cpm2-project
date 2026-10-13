namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ArenaFriendPresence
	{
		public readonly string AccountId;

		public readonly string DisplayName;

		public readonly string RoomName;

		public readonly int PlayerCount;

		public readonly int MaxPlayers;

		public readonly bool IsOpen;

		public readonly int Password;

		public ArenaFriendPresence(string accountId, string displayName, string roomName, int playerCount, int maxPlayers, bool isOpen, int password)
		{
			AccountId = accountId;
			DisplayName = displayName;
			RoomName = roomName;
			PlayerCount = playerCount;
			MaxPlayers = maxPlayers;
			IsOpen = isOpen;
			Password = password;
		}
	}
}
