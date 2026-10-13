namespace CPM.Ads.Policy
{
	public interface IAdPolicyStore
	{
		AdPolicyState State { get; }

		void Persist();
	}
}
