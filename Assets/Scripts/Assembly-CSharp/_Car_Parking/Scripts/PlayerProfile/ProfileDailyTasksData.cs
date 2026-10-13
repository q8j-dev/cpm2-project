using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileDailyTasksData
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("status")]
		public int Status { get; set; }

		[JsonProperty("tasks")]
		public List<ProfileDailyTaskItemData> Tasks { get; set; }
	}
}
