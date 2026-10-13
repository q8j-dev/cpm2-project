using Newtonsoft.Json;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.EventSystem.Data.Response;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutFinishObstacleRaceData : FalloutFinishData
	{
		[JsonProperty("progress")]
		public int Progress;

		[JsonProperty("eventDataBase")]
		public ObstacleRaceEventData ObstacleRaceEventData;
	}
}
