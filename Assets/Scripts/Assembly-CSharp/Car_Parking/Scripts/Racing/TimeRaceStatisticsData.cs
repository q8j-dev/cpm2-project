using UnityEngine.Scripting;

namespace Car_Parking.Scripts.Racing
{
	[Preserve]
	public class TimeRaceStatisticsData
	{
		public int TrackId { get; set; }

		public int CarClassId { get; set; }

		public float Time { get; set; }
	}
}
