using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutStartRequest
	{
		[JsonProperty("unique_race_id")]
		public string UniqueRaceId;
	}
}
