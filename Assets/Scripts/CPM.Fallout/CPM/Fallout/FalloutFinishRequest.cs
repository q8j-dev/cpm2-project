using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutFinishRequest
	{
		[JsonProperty("unique_race_id")]
		public string UniqueRaceId;

		[JsonProperty("time")]
		public long TimeSpend;

		[JsonProperty("place")]
		public int Place;
	}
}
