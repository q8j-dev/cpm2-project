using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutLimits
	{
		[JsonProperty("coinLimit")]
		public long CoinLimit;

		[JsonProperty("moneyLimit")]
		public long MoneyLimit;

		[JsonProperty("coinsEarned")]
		public long CoinsEarned;

		[JsonProperty("moneyEarned")]
		public long MoneyEarned;

		[JsonProperty("secondsUntilReset")]
		public long SecondsUntilReset;
	}
}
