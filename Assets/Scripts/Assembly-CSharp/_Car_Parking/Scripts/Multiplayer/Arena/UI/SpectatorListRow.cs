namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public readonly struct SpectatorListRow
	{
		public readonly int Actor;

		public readonly int Place;

		public readonly string UserName;

		public readonly string ValueText;

		public readonly bool Watchable;

		public readonly bool Selected;

		public SpectatorListRow(int actor, int place, string userName, string valueText, bool watchable, bool selected)
		{
			Actor = actor;
			Place = place;
			UserName = userName;
			ValueText = valueText;
			Watchable = watchable;
			Selected = selected;
		}
	}
}
