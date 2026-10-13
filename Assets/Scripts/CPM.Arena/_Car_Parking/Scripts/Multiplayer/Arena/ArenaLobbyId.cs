using System;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaLobbyId
	{
		public const string Key = "aL";

		public static string Generate()
		{
			return Guid.NewGuid().ToString("N");
		}
	}
}
