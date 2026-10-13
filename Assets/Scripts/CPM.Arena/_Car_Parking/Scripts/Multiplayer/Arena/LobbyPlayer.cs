namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class LobbyPlayer
	{
		public int Actor { get; }

		public string Name { get; }

		public LobbyPlayer(int actor, string name)
		{
			Actor = actor;
			Name = name;
		}
	}
}
