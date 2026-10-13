namespace CPM.Balance
{
	public class BalanceDocument : IConfigDocument
	{
		public ShopFeature Shop;

		public AdsFeature Ads;

		public TuningFeature Tuning;

		public int Version { get; set; } = 1;
	}
}
