using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileVersionData
	{
		[JsonProperty("version")]
		public int Version { get; set; }
	}
}
