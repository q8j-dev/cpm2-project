namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class LobbyRosterExtensions
	{
		public static string GetDisplayName(this LobbyRoster roster, int actor)
		{
			string text = roster?.GetName(actor);
			if (string.IsNullOrEmpty(text))
			{
				return $"Player {actor}";
			}
			return text;
		}
	}
}
