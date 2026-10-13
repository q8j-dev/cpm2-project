namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct ArenaPlayerSnapshot
	{
		public readonly int Actor;

		public readonly string Name;

		public ArenaPlayerSnapshot(int actor, string name)
		{
			Actor = actor;
			Name = name;
		}
	}
}
