using System;
using CPM.Ghost.Core;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.RacingRecords
{
	[Preserve]
	public class RaceReplayRequest
	{
		[JsonProperty("raceUid")]
		public string RaceUid;

		[JsonProperty("raceKey")]
		public string RaceKey;

		[JsonProperty("roomId")]
		public string RoomId;

		[JsonProperty("score")]
		public long Score;

		[JsonProperty("replayData")]
		public string ReplayData;

		public static RaceReplayRequest FromRecording(string raceUid, string raceKey, string roomId, long score, GhostRecording recording)
		{
			return new RaceReplayRequest
			{
				RaceUid = raceUid,
				RaceKey = raceKey,
				RoomId = roomId,
				Score = score,
				ReplayData = Convert.ToBase64String(GhostSerializer.Serialize(recording))
			};
		}
	}
}
