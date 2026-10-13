namespace _Car_Parking.Scripts.Drag
{
	public readonly struct RacingRewardAmounts
	{
		public readonly int Coins;

		public readonly int Money;

		public bool IsEmpty
		{
			get
			{
				if (Coins <= 0)
				{
					return Money <= 0;
				}
				return false;
			}
		}

		public RacingRewardAmounts(int coins, int money)
		{
			Coins = coins;
			Money = money;
		}
	}
}
