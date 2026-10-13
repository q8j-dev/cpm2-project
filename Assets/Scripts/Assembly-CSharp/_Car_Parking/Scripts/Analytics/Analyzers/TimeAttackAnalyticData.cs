namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public class TimeAttackAnalyticData
	{
		public string EventVariant { get; set; }

		public string RequiredCarClasses { get; set; }

		public float RequiredValue { get; set; }

		public string TrackLayoutId { get; set; }

		public string TrajectoryId { get; set; }

		public int TtdCode { get; set; }

		public DriftAnalyticData DriftData { get; set; }
	}
}
