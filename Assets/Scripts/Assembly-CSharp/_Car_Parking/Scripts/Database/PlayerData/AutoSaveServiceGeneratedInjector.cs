using System.Collections.Generic;
using CPM.Database;
using CPM.RemoteConfig;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	internal class AutoSaveServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AutoSaveService autoSaveService = new AutoSaveService((IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters), (DataBase)resolver.ResolveOrParameter(typeof(DataBase), "dataBase", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters));
			Inject(autoSaveService, resolver, parameters);
			return autoSaveService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
