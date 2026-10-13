using CommonUtility.Contracts;
using _Car_Parking.Scripts.CustomDataTypes;

namespace _Car_Parking.Scripts.Wallet
{
	public class RewardWallet : IRewardWallet
	{
		private readonly CurrencyManagerController _currencyManagerController;

		public RewardWallet(CurrencyManagerController currencyManagerController)
		{
			_currencyManagerController = currencyManagerController;
		}

		public void Credit(long coins, long money)
		{
			if (coins > 0)
			{
				GetterCoinsManager.AddCoinsToPrefs(new AdvancedInt(ToInt(coins)));
				_currencyManagerController.UpdateCoinsAnimated();
			}
			if (money > 0)
			{
				_currencyManagerController.AddMoney(new AdvancedInt(ToInt(money)));
			}
		}

		private static int ToInt(long value)
		{
			if (value <= int.MaxValue)
			{
				return (int)value;
			}
			return int.MaxValue;
		}
	}
}
