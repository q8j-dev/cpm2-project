using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public class AdRewardClaimResult
	{
		[JsonProperty("ad_id")]
		public string AdId;

		[JsonProperty("status")]
		public string Status;

		[JsonProperty("coins")]
		public int Coins;

		[JsonProperty("money")]
		public int Money;

		[JsonProperty("claimed")]
		public AdRewardClaimed Claimed;
	}
}
