using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class CircuitRacingProfileData
	{
		[JsonProperty("lastReceived")]
		public long LastReceived { get; set; }

		[JsonProperty("currentCoins")]
		public long CurrentCoins { get; set; }

		[JsonProperty("currentMoney")]
		public long CurrentMoney { get; set; }

		[JsonProperty("latestCircuitRacingSet")]
		public long LatestCircuitRacingSet { get; set; }
	}
}
