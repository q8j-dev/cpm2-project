namespace CPM.RouteStandings
{
	public readonly struct RacerStanding
	{
		public readonly int Id;

		public readonly int Place;

		public readonly float Remaining;

		public RacerStanding(int id, int place, float remaining)
		{
			Id = id;
			Place = place;
			Remaining = remaining;
		}
	}
}
