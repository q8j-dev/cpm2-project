namespace CPM.Ads.Policy
{
	public interface IAdPlacementCatalog
	{
		bool SystemEnabled { get; }

		GlobalRules Global { get; }

		bool TryGet(AdPlacement placement, out PlacementRules rules);
	}
}
