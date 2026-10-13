using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class FalloutTypeProfileData
	{
		[JsonProperty("coinsEarned")]
		public long CoinsEarned { get; set; }

		[JsonProperty("moneyEarned")]
		public long MoneyEarned { get; set; }

		[JsonProperty("lastReceived")]
		public long LastReceived { get; set; }

		[JsonProperty("latestSet")]
		public long LatestSet { get; set; }
	}
}
