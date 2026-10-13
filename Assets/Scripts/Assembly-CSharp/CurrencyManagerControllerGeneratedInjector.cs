using System.Collections.Generic;
using CPM.Account;
using CPM.DailyTasks;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Wallet;

[Preserve]
internal class CurrencyManagerControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CurrencyManagerController currencyManagerController = new CurrencyManagerController();
		Inject(currencyManagerController, resolver, parameters);
		return currencyManagerController;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CurrencyManagerController obj = (CurrencyManagerController)instance;
		DailyTasksManager dailyTasksManager = (DailyTasksManager)resolver.ResolveOrParameter(typeof(DailyTasksManager), "dailyTasksManager", parameters);
		IWalletService walletDatabaseChangeService = (IWalletService)resolver.ResolveOrParameter(typeof(IWalletService), "walletDatabaseChangeService", parameters);
		IAccountController accountController = (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters);
		IDatabaseSaveLoadService databaseSaveLoadService = (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters);
		obj.Construct(dailyTasksManager, walletDatabaseChangeService, accountController, databaseSaveLoadService);
	}
}
