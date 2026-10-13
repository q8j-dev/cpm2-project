using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace CPM.Fallout
{
	[Preserve]
	public class FalloutFinish
	{
		[JsonProperty("place")]
		public int Place;

		[JsonProperty("data")]
		public FalloutFinishData Data;
	}
}
