using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Preserve]
	public class RallyData
	{
		[JsonProperty("totalCups")]
		public int[] TotalCups;

		[JsonProperty("ngAvailable")]
		public bool NgAvailable;

		[JsonProperty("lastNgPlusAt")]
		public long LastNgPlusAt;

		[JsonProperty("trajectories")]
		public Dictionary<string, RallyProgress> RallyProgress;

		[JsonProperty("ngTimeFactor")]
		public float NgTimeFactor;

		[JsonProperty("ngCooldown")]
		public long NgCooldown;

		[JsonProperty("ngCount")]
		public float NgCount;

		[JsonIgnore]
		public bool DataUpdated;

		[JsonIgnore]
		public float EffectiveNgFactor
		{
			get
			{
				if (!(NgTimeFactor > 0f))
				{
					return 1f;
				}
				return NgTimeFactor;
			}
		}

		[Preserve]
		public RallyData()
		{
		}

		public void CopyFrom(RallyData source)
		{
			TotalCups = source.TotalCups;
			NgAvailable = source.NgAvailable;
			LastNgPlusAt = source.LastNgPlusAt;
			RallyProgress = source.RallyProgress;
			NgTimeFactor = source.NgTimeFactor;
			NgCooldown = source.NgCooldown;
			NgCount = source.NgCount;
		}

		public static string ProgressKey(int locationId, int trajectoryId)
		{
			return $"{locationId};{trajectoryId}";
		}
	}
}
