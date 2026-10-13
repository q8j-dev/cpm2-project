using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Analytics.Root.Data
{
	[Serializable]
	public class AnalyticCoreSetting
	{
		[Tooltip("Count events in one batch")]
		public int BatchSize = 15;

		[Tooltip("Event life time in seconds")]
		public int EventTTL = 604800;

		[Tooltip("Send time between events sending")]
		public float SendCooldown = 15f;

		public AnalyticCoreSetting(AnalyticCoreSetting parent)
		{
			BatchSize = parent.BatchSize;
			EventTTL = parent.EventTTL;
			SendCooldown = parent.SendCooldown;
		}

		public AnalyticCoreSetting()
		{
		}
	}
}
