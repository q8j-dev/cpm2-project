using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	public class CarBuyRequestFactory
	{
		private IAccountController _accountController;

		[Inject]
		public CarBuyRequestFactory(IAccountController accountController)
		{
			_accountController = accountController;
		}

		public CarBackendRequest CreateRequest(int id, bool withStyle, AdvancedInt moneyBodyKitsCount1, AdvancedInt addCoinsPrice1, List<int> bodyKits, int style, int slotIndex)
		{
			if (_accountController.Authorized)
			{
				return new CarBackendRequestAuthorized(id, withStyle, moneyBodyKitsCount1, addCoinsPrice1, bodyKits, style, slotIndex);
			}
			return new CarBackendRequestUnauthorized(id, moneyBodyKitsCount1, new AdvancedInt(CurrencyManagerController.Money));
		}
	}
}
