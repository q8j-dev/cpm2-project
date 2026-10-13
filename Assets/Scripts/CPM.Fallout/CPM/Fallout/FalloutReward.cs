using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutReward
	{
		[JsonProperty("coins")]
		public long Coins;

		[JsonProperty("money")]
		public long Money;
	}
}
