using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.RacingRecords
{
	[Preserve]
	public class RacingRecord
	{
		[JsonProperty("rank")]
		public int Rank;

		[JsonProperty("userId")]
		public string UserId;

		[JsonProperty("nickname")]
		public string Nickname;

		[JsonProperty("carId")]
		public int CarId;

		[JsonProperty("carClass")]
		public int CarClass;

		[JsonProperty("score")]
		public long Score;

		[JsonProperty("achievedAt")]
		public long AchievedAt;

		[JsonProperty("rain")]
		public bool Rain;
	}
}
