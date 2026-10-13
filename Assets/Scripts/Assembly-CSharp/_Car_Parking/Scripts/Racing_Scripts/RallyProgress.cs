using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	public class RallyProgress
	{
		[JsonProperty("PersonalBest")]
		public float PersonalBest;

		[JsonProperty("RallyTier")]
		public int RallyTier;

		[Preserve]
		public RallyProgress()
		{
		}
	}
}
