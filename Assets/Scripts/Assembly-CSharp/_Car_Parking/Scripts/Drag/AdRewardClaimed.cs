using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public class AdRewardClaimed
	{
		[JsonProperty("type")]
		public string Type;

		[JsonProperty("raceType")]
		public string RaceType;

		[JsonProperty("coins")]
		public int Coins;

		[JsonProperty("money")]
		public int Money;
	}
}
