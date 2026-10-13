using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileBasicRatingData
	{
		[JsonProperty("level")]
		public int Level { get; set; }
	}
}
