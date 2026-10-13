using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class RallyCampaignProfileData
	{
		[JsonProperty("progress")]
		public Dictionary<string, RallyStageProfileData> Progress { get; set; }
	}
}
