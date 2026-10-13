using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class SecondLoadingStartupGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SecondLoadingStartup secondLoadingStartup = new SecondLoadingStartup((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IPlayerStorageService)resolver.ResolveOrParameter(typeof(IPlayerStorageService), "playerStorageService", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (DataBase)resolver.ResolveOrParameter(typeof(DataBase), "dataBase", parameters), (IConnectionDataHandler)resolver.ResolveOrParameter(typeof(IConnectionDataHandler), "connectionDataHandler", parameters));
			Inject(secondLoadingStartup, resolver, parameters);
			return secondLoadingStartup;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
