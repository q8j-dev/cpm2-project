using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	public class RallyProgressResult
	{
		[JsonProperty("progress")]
		public int Progress;

		[JsonProperty("ngAvailable")]
		public int NGAvailable;

		[JsonProperty("awardCoins")]
		public int AwardCoins;

		[JsonProperty("awardMoney")]
		public int AwardMoney;

		[JsonProperty("awardSpecific")]
		public int AwardSpecific;

		[Preserve]
		public RallyProgressResult()
		{
		}
	}
}
