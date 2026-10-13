namespace CommonUtility.Contracts
{
	public interface IPendingRewardsProcessor
	{
		void ProcessRewards(string[] rewards);

		void ProcessReward(string key, string value);
	}
}
