using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileFeaturesData
	{
		[JsonProperty("circuitRacing")]
		public CircuitRacingProfileData CircuitRacing { get; set; }

		[JsonProperty("dragRacing")]
		public DragRacingProfileData DragRacing { get; set; }

		[JsonProperty("rallyRacing")]
		public RallyRacingProfileData RallyRacing { get; set; }

		[JsonProperty("falloutRacing")]
		public FalloutRacingProfileData FalloutRacing { get; set; }
	}
}
