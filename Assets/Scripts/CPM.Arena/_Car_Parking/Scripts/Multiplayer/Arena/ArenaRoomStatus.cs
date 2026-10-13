namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ArenaRoomStatus
	{
		public readonly int PlayerCount;

		public readonly int MaxPlayers;

		public readonly bool IsOpen;

		public ArenaRoomStatus(int playerCount, int maxPlayers, bool isOpen)
		{
			PlayerCount = playerCount;
			MaxPlayers = maxPlayers;
			IsOpen = isOpen;
		}
	}
}
