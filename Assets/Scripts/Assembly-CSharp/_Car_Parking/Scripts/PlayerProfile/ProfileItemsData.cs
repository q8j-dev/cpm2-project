using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PlayerProfile
{
	[Preserve]
	public class ProfileItemsData
	{
		[JsonProperty("poses")]
		public List<int> Poses { get; set; }

		[JsonProperty("animations")]
		public List<int> Animations { get; set; }
	}
}
