namespace CPM.Ads.Policy
{
	public interface IAdsDailyLimits
	{
		bool TryGetSpent(AdPlacement placement, out int spent);

		bool TryGetTotalSpent(out int totalSpent);

		void RegisterSpent(AdPlacement placement);
	}
}
