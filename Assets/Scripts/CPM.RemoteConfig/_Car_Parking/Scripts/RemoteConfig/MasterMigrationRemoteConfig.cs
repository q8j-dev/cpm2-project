using System;

namespace _Car_Parking.Scripts.RemoteConfig
{
	[Serializable]
	public class MasterMigrationRemoteConfig
	{
		public bool enabled = true;

		public float sampleIntervalSec = 1f;

		public float warmupSec = 15f;

		public int maxPingBufferSize = 30;

		public int minSamplesForAverage = 5;

		public int pingBufferMs = 40;

		public float cooldownSec = 60f;

		public int masterPingThresholdMs = 120;

		public float masterPingCheckIntervalSec = 10f;

		public float fallbackPollIntervalSec = 180f;

		public float responseBufferSec = 3f;

		public int maxWaitForRpcRrClearMs = 2000;
	}
}
