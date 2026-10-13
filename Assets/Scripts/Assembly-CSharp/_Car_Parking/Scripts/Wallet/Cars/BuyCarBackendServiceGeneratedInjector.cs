using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.Database;
using VContainer;

namespace _Car_Parking.Scripts.Wallet.Cars
{
	[Preserve]
	internal class BuyCarBackendServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BuyCarBackendService buyCarBackendService = new BuyCarBackendService((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters));
			Inject(buyCarBackendService, resolver, parameters);
			return buyCarBackendService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
