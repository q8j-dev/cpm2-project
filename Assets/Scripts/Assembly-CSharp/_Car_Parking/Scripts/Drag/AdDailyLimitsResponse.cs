using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public class AdDailyLimitsResponse
	{
		[JsonProperty("limits")]
		public List<AdDailyLimit> Limits;
	}
}
