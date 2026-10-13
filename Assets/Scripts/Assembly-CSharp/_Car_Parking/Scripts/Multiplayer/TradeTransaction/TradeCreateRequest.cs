using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.TradeTransaction
{
	[Preserve]
	public class TradeCreateRequest
	{
		[JsonProperty("type")]
		public string Type;

		[JsonProperty("creatorCarUid", NullValueHandling = NullValueHandling.Ignore)]
		public string CreatorCarUid;

		[JsonProperty("acceptorUid")]
		public string AcceptorUid;

		[JsonProperty("acceptorCarUid")]
		public string AcceptorCarUid;

		[JsonProperty("money")]
		public int Money;
	}
}
