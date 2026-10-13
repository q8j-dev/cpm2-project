using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class DragRacingProfileData
	{
		[JsonProperty("claimedSlots")]
		public List<string> ClaimedSlots { get; set; }

		[JsonProperty("latestDragRacingSet")]
		public long LatestDragRacingSet { get; set; }

		[JsonProperty("migrated")]
		public bool Migrated { get; set; }
	}
}
