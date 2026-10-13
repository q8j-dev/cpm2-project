using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.RacingRecords
{
	[Preserve]
	public class StartRaceRequest
	{
		[JsonProperty("raceUid")]
		public string RaceUid;

		[JsonProperty("raceKey")]
		public string RaceKey;

		[JsonProperty("roomId")]
		public string RoomId;

		[JsonProperty("carId")]
		public int CarId;

		[JsonProperty("carClass")]
		public int CarClass;

		[JsonProperty("rain")]
		public bool Rain;
	}
}
