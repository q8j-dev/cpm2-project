namespace _Car_Parking.Scripts.Connection
{
	public static class MatchmakingBucket
	{
		public const int PoorBelow = 200000;

		public const int MiddleBelow = 1500000;

		public const int RichBelow = 5000000;

		public const int MoneyTierCount = 4;

		public static int MoneyTier(int money)
		{
			if (money < 200000)
			{
				return 0;
			}
			if (money < 1500000)
			{
				return 1;
			}
			if (money < 5000000)
			{
				return 2;
			}
			return 3;
		}

		public static int LanguageAndMoney(int languageIndex, int money)
		{
			if (languageIndex < 0)
			{
				languageIndex = 0;
			}
			return languageIndex * 4 + MoneyTier(money);
		}
	}
}
