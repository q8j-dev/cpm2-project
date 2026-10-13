namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct LobbyLimits
	{
		public readonly int MinPlayers;

		public readonly int MaxPlayers;

		public readonly float WaitSeconds;

		public readonly float CountdownSeconds;

		public readonly float MinPlayersWaitSeconds;

		public LobbyLimits(int minPlayers, int maxPlayers, float waitSeconds, float countdownSeconds, float minPlayersWaitSeconds)
		{
			MinPlayers = minPlayers;
			MaxPlayers = maxPlayers;
			WaitSeconds = waitSeconds;
			CountdownSeconds = countdownSeconds;
			MinPlayersWaitSeconds = minPlayersWaitSeconds;
		}
	}
}
