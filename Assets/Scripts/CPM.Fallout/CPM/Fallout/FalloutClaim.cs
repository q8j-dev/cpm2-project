using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutClaim
	{
		[JsonProperty("place")]
		public int Place;

		[JsonProperty("rewardData")]
		public FalloutReward RewardData;
	}
}
