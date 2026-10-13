using CPM.Account;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Wallet
{
	public class LoadWalletFromDataBase : IAuthorizationDependant
	{
		private CurrencyManagerController _currencyManagerController;

		public LoadWalletFromDataBase(CurrencyManagerController currencyManagerController)
		{
			_currencyManagerController = currencyManagerController;
		}

		public void Load()
		{
		}

		public UniTaskVoid OnSuccessfulAuthorization()
		{
			Load();
			return default;
		}

		public void OnSignOut()
		{
		}
	}
}
