using JsonSubTypes;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	[JsonSubtypes.KnownSubType(typeof(FalloutFinishObstacleRaceData), 4)]
	[JsonConverter(typeof(JsonSubtypes), new object[] { "Type" })]
	public class FalloutFinishData
	{
		[JsonProperty("type")]
		public int Type;
	}
}
