using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public class AdRewardClaimEntry
	{
		[JsonProperty("ad_id")]
		public string AdId;

		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public string Type;

		[JsonProperty("id")]
		public string Id;
	}
}
