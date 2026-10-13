namespace CPM.Ads
{
	public readonly struct AdImpressionInfo
	{
		public readonly string Network;

		public readonly string CreativeId;

		public readonly string SourceUnitId;

		public readonly double RevenueUsd;

		public readonly double Cpm;

		public readonly string RevenuePrecision;

		public readonly bool RevenueDisclosed;

		public AdImpressionInfo(string network, string creativeId, string sourceUnitId, double revenueUsd, double cpm, string revenuePrecision, bool revenueDisclosed)
		{
			Network = network;
			CreativeId = creativeId;
			SourceUnitId = sourceUnitId;
			RevenueUsd = revenueUsd;
			Cpm = cpm;
			RevenuePrecision = revenuePrecision;
			RevenueDisclosed = revenueDisclosed;
		}
	}
}
