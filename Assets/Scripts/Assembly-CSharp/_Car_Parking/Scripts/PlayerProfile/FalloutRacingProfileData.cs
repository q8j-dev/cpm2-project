using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class FalloutRacingProfileData
	{
		[JsonProperty("byType")]
		public Dictionary<string, FalloutTypeProfileData> ByType { get; set; }
	}
}
