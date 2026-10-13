namespace CPM.Fallout
{
	public sealed class FalloutRaceResult
	{
		public string RaceId { get; }

		public FalloutRaceState State { get; }

		public int Place { get; }

		public long Coins { get; }

		public long Money { get; }

		public FalloutRaceResult(string raceId, FalloutRaceState state, int place, long coins, long money)
		{
			RaceId = raceId;
			State = state;
			Place = place;
			Coins = coins;
			Money = money;
		}
	}
}
