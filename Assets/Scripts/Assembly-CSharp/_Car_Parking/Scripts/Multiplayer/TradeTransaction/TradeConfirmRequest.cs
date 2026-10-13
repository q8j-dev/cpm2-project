using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Multiplayer.TradeTransaction
{
	[Preserve]
	public class TradeConfirmRequest
	{
		[JsonProperty("type")]
		public string Type;

		[JsonProperty("transactionId")]
		public string TransactionId;

		[JsonProperty("creatorUid")]
		public string CreatorUid;

		[JsonProperty("creatorCarUid", NullValueHandling = NullValueHandling.Ignore)]
		public string CreatorCarUid;

		[JsonProperty("acceptorCarUid")]
		public string AcceptorCarUid;

		[JsonProperty("money")]
		public int Money;
	}
}
