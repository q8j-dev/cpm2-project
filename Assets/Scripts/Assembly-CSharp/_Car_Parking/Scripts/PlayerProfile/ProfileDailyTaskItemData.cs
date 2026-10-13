using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileDailyTaskItemData
	{
		[JsonProperty("current")]
		public double Current { get; set; }

		[JsonProperty("status")]
		public int Status { get; set; }
	}
}
