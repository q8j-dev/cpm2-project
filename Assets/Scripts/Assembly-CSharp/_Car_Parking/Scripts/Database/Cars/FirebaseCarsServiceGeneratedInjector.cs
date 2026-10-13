using System.Collections.Generic;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using Persistence.Systems;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Persistence.Migration;

namespace _Car_Parking.Scripts.Database.Cars
{
	[Preserve]
	internal class FirebaseCarsServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FirebaseCarsService firebaseCarsService = new FirebaseCarsService((IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (RuntimePlayerDataHandler)resolver.ResolveOrParameter(typeof(RuntimePlayerDataHandler), "runtimePlayerDataHandler", parameters), (IDataVersionsService)resolver.ResolveOrParameter(typeof(IDataVersionsService), "dataVersionsService", parameters), (LocalDataMigrationManager)resolver.ResolveOrParameter(typeof(LocalDataMigrationManager), "localDataMigrationManager", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (IDeleterUserDataGetter)resolver.ResolveOrParameter(typeof(IDeleterUserDataGetter), "deleterUserDataGetter", parameters), (IProgressBackend)resolver.ResolveOrParameter(typeof(IProgressBackend), "progressBackend", parameters));
			Inject(firebaseCarsService, resolver, parameters);
			return firebaseCarsService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
