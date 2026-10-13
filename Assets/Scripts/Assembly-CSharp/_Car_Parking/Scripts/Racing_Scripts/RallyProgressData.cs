using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	public class RallyProgressData
	{
		[JsonProperty("LocationId")]
		public int LocationId;

		[JsonProperty("TrajectoryId")]
		public int TrajectoryId;

		[JsonProperty("Time")]
		public float Time;

		[Preserve]
		public RallyProgressData()
		{
		}
	}
}
