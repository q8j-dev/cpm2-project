using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileRatingData
	{
		[JsonProperty("migratedFromFirestore")]
		public bool MigratedFromFirestore { get; set; }

		[JsonProperty("tasks")]
		public Dictionary<string, double> Tasks { get; set; }

		[JsonProperty("achievements")]
		public Dictionary<string, int> Achievements { get; set; }

		[JsonProperty("basic")]
		public ProfileBasicRatingData Basic { get; set; }

		[JsonProperty("dailyTasks")]
		public ProfileDailyTasksData DailyTasks { get; set; }
	}
}
