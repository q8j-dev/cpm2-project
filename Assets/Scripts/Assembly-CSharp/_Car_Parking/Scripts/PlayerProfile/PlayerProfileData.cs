using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class PlayerProfileData
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("localId")]
		public string LocalId { get; set; }

		[JsonProperty("progress")]
		public ProfileProgressData Progress { get; set; }
	}
}
