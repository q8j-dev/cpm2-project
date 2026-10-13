namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	public readonly struct PingEntry
	{
		public readonly int AvgPing;

		public PingEntry(int avgPing)
		{
			AvgPing = avgPing;
		}
	}
}
