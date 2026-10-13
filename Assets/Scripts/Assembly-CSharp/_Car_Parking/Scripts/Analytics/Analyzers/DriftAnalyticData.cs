namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public class DriftAnalyticData
	{
		public int DriftScoreTotal { get; set; }

		public int DriftBaseScoreTotal { get; set; }

		public int DriftComboXMax { get; set; }

		public int DriftChangeComboCount { get; set; }

		public DriftBreakData DriftBreakData { get; set; }

		public DriftAnalyticData()
		{
			DriftBreakData = new DriftBreakData();
		}
	}
}
