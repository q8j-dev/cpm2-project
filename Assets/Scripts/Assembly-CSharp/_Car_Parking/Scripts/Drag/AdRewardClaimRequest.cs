using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public class AdRewardClaimRequest
	{
		[JsonProperty("claims")]
		public List<AdRewardClaimEntry> Claims;
	}
}
