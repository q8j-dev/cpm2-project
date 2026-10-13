namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ArenaEntryOptions
	{
		public const int NoPassword = -1;

		public bool FriendsOnly { get; }

		public int Password { get; }

		public string JoinRoomName { get; }

		public bool CreateIfMissing { get; }

		public ArenaEntrySource Source { get; }

		private ArenaEntryOptions(bool friendsOnly, int password, string joinRoomName, bool createIfMissing, ArenaEntrySource source)
		{
			FriendsOnly = friendsOnly;
			Password = password;
			JoinRoomName = joinRoomName;
			CreateIfMissing = createIfMissing;
			Source = source;
		}

		public static ArenaEntryOptions Public(ArenaEntrySource source)
		{
			return new ArenaEntryOptions(friendsOnly: false, -1, null, createIfMissing: false, source);
		}

		public static ArenaEntryOptions ForFriends(int password, ArenaEntrySource source)
		{
			return new ArenaEntryOptions(friendsOnly: true, password, null, createIfMissing: false, source);
		}

		public static ArenaEntryOptions JoinRoom(string roomName, ArenaEntrySource source)
		{
			return new ArenaEntryOptions(friendsOnly: false, -1, roomName, createIfMissing: false, source);
		}

		public static ArenaEntryOptions Rematch(string roomName, int password, ArenaEntrySource source)
		{
			return new ArenaEntryOptions(friendsOnly: false, password, roomName, createIfMissing: true, source);
		}
	}
}
