using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	[Preserve]
	public class ObstacleRaceCheckpointDto
	{
		[JsonProperty("id")]
		public int Id;

		[JsonProperty("time")]
		public float? Time;

		[JsonProperty("restarts")]
		public int Restarts;

		[JsonProperty("missed")]
		public bool Missed;
	}
}
