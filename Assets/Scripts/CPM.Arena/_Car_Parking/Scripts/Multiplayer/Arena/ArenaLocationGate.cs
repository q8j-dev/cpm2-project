namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaLocationGate
	{
		public static bool Matches(ArenaConfig active, ArenaLocation required)
		{
			if ((bool)active)
			{
				return active.WaitingLocation == required;
			}
			return false;
		}
	}
}
