namespace _Car_Parking.Scripts.DiscordPresence
{
	public readonly struct DiscordActivityContext
	{
		public readonly string LocationName;

		public readonly int Lap;

		public readonly int TotalLaps;

		public DiscordActivityContext(string locationName, int lap, int totalLaps)
		{
			LocationName = locationName;
			Lap = lap;
			TotalLaps = totalLaps;
		}

		public bool Equals(in DiscordActivityContext other)
		{
			if (Lap == other.Lap && TotalLaps == other.TotalLaps)
			{
				return string.Equals(LocationName, other.LocationName);
			}
			return false;
		}
	}
}
