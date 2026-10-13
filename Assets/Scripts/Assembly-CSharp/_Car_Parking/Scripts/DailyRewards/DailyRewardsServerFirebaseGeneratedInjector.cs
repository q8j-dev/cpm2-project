using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.Database;
using VContainer;

namespace _Car_Parking.Scripts.DailyRewards
{
	[Preserve]
	internal class DailyRewardsServerFirebaseGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DailyRewardsServerFirebase dailyRewardsServerFirebase = new DailyRewardsServerFirebase((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (IDatabaseSaveLoadService)resolver.ResolveOrParameter(typeof(IDatabaseSaveLoadService), "databaseSaveLoadService", parameters), (IDeleterUserDataGetter)resolver.ResolveOrParameter(typeof(IDeleterUserDataGetter), "deleterUserDataGetter", parameters));
			Inject(dailyRewardsServerFirebase, resolver, parameters);
			return dailyRewardsServerFirebase;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
