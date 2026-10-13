using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class RallyStageProfileData
	{
		[JsonProperty("tier")]
		public int Tier { get; set; }

		[JsonProperty("pb")]
		public double Pb { get; set; }
	}
}
