using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public class AdDailyLimit
	{
		[JsonProperty("id")]
		public string Id;

		[JsonProperty("dailyCap")]
		public int DailyCap;

		[JsonProperty("spent")]
		public int Spent;
	}
}
