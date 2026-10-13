using System.Collections.Generic;
using _Car_Parking.Scripts.RemoteConfig;

namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	public readonly struct MigrationSnapshot
	{
		public readonly int MasterAvgPing;

		public readonly IReadOnlyDictionary<PlayerKey, PingEntry> Candidates;

		public readonly float TimeSinceLastMigrationSec;

		public readonly int PlayerCount;

		public readonly MasterMigrationRemoteConfig Config;

		public MigrationSnapshot(int masterAvgPing, IReadOnlyDictionary<PlayerKey, PingEntry> candidates, float timeSinceLastMigrationSec, int playerCount, MasterMigrationRemoteConfig config)
		{
			MasterAvgPing = masterAvgPing;
			Candidates = candidates;
			TimeSinceLastMigrationSec = timeSinceLastMigrationSec;
			PlayerCount = playerCount;
			Config = config;
		}
	}
}
