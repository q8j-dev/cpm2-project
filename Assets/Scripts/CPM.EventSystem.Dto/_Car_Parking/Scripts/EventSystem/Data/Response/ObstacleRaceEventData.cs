using System;
using Newtonsoft.Json;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	public class ObstacleRaceEventData : EvenDataBase
	{
		[JsonProperty("unclaimed")]
		public int Unclaimed;

		[JsonProperty("claimable")]
		public int Claimable;

		[JsonProperty("total")]
		public int TotalPresents;

		[JsonProperty("steps")]
		public int[] Steps;
	}
}
