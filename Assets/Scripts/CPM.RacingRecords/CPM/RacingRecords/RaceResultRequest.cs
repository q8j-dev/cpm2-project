using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.RacingRecords
{
	[Preserve]
	public class RaceResultRequest
	{
		[JsonProperty("raceUid")]
		public string RaceUid;

		[JsonProperty("raceKey")]
		public string RaceKey;

		[JsonProperty("nickname")]
		public string Nickname;

		[JsonProperty("carId")]
		public int CarId;

		[JsonProperty("carClass")]
		public int CarClass;

		[JsonProperty("score")]
		public long Score;
	}
}
