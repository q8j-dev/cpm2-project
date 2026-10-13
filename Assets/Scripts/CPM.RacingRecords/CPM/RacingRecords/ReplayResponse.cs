using System;
using CPM.Ghost.Core;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.RacingRecords
{
	[Preserve]
	public class ReplayResponse
	{
		[JsonProperty("raceKey")]
		public string RaceKey;

		[JsonProperty("userId")]
		public string UserId;

		[JsonProperty("carId")]
		public int CarId;

		[JsonProperty("carClass")]
		public int CarClass;

		[JsonProperty("score")]
		public long Score;

		[JsonProperty("data")]
		public string Data;

		public bool TryGetRecording(out GhostRecording recording)
		{
			recording = null;
			if (string.IsNullOrEmpty(Data))
			{
				return false;
			}
			recording = GhostSerializer.Deserialize(Convert.FromBase64String(Data));
			return recording != null;
		}
	}
}
