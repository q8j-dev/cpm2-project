using System.Collections.Generic;
using _Car_Parking.Scripts.RemoteConfig;

namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	public static class MigrationDecider
	{
		public static MigrationDecision Evaluate(in MigrationSnapshot snapshot)
		{
			MasterMigrationRemoteConfig config = snapshot.Config;
			if (config == null || !config.enabled)
			{
				return MigrationDecision.None("disabled");
			}
			if (snapshot.PlayerCount < 2)
			{
				return MigrationDecision.None("single player");
			}
			if (snapshot.TimeSinceLastMigrationSec < config.cooldownSec)
			{
				return MigrationDecision.None("cooldown");
			}
			PlayerKey target = default;
			int num = int.MaxValue;
			bool flag = false;
			foreach (KeyValuePair<PlayerKey, PingEntry> candidate in snapshot.Candidates)
			{
				PingEntry value = candidate.Value;
				if (value.AvgPing + config.pingBufferMs < snapshot.MasterAvgPing && value.AvgPing < num)
				{
					num = value.AvgPing;
					target = candidate.Key;
					flag = true;
				}
			}
			if (!flag)
			{
				return MigrationDecision.None("no eligible candidate");
			}
			return MigrationDecision.MigrateTo(target, $"master={snapshot.MasterAvgPing}ms, candidate={num}ms");
		}
	}
}
