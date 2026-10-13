using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class RallyRacingProfileData
	{
		[JsonProperty("campaign")]
		public RallyCampaignProfileData Campaign { get; set; }

		[JsonProperty("totalWins")]
		public Dictionary<string, int> TotalWins { get; set; }

		[JsonProperty("rallyCoinsEarned")]
		public long RallyCoinsEarned { get; set; }

		[JsonProperty("rallyMoneyEarned")]
		public long RallyMoneyEarned { get; set; }

		[JsonProperty("lastDay")]
		public long LastDay { get; set; }

		[JsonProperty("ngCountAllTime")]
		public int NgCountAllTime { get; set; }

		[JsonProperty("lastNgPlusAt")]
		public long LastNgPlusAt { get; set; }

		[JsonProperty("clothesAwarded")]
		public bool ClothesAwarded { get; set; }

		[JsonProperty("latestRallyRacingSet")]
		public long LatestRallyRacingSet { get; set; }

		[JsonProperty("migrated")]
		public bool Migrated { get; set; }
	}
}
