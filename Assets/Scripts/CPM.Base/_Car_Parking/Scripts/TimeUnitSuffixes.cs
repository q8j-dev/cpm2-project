namespace _Car_Parking.Scripts
{
	public readonly struct TimeUnitSuffixes
	{
		public readonly string Day;

		public readonly string Hour;

		public readonly string Minute;

		public readonly string Second;

		public TimeUnitSuffixes(string day, string hour, string minute, string second)
		{
			Day = day;
			Hour = hour;
			Minute = minute;
			Second = second;
		}
	}
}
