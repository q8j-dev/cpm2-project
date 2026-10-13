using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.RemoteConfig
{
	[Serializable]
	public class AnalyticsRemoteConfig
	{
		public bool enabled = true;

		public int eventBatchSize = 15;

		public int eventTTL = 604800;

		public float eventSendCooldown = 15f;

		public List<string> disableEvents = new List<string>();

		public List<string> errorReportingEnvironments = new List<string> { "DEV", "BETA" };
	}
}
