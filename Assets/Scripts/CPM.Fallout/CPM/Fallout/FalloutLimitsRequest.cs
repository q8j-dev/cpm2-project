using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutLimitsRequest
	{
		[JsonProperty("race_type")]
		public long RaceType;
	}
}
