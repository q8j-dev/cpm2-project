using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public class DebugAdsWatchData
	{
		[JsonProperty("ad_id")]
		public string AdId;

		[JsonProperty("placement")]
		public string Placement;
	}
}
