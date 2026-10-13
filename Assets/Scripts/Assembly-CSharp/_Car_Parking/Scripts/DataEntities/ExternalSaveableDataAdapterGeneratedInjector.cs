using System.Collections.Generic;
using Persistence.Systems;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Persistence.Migration;

namespace _Car_Parking.Scripts.DataEntities
{
	[Preserve]
	internal class ExternalSaveableDataAdapterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ExternalSaveableDataAdapter externalSaveableDataAdapter = new ExternalSaveableDataAdapter((IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters), (IMigrationManager)resolver.ResolveOrParameter(typeof(IMigrationManager), "migrationManager", parameters), (VersionedDataClassesConfig)resolver.ResolveOrParameter(typeof(VersionedDataClassesConfig), "dataClassesConfig", parameters), (IDataVersionsService)resolver.ResolveOrParameter(typeof(IDataVersionsService), "dataVersionService", parameters), (RuntimePlayerDataHandler)resolver.ResolveOrParameter(typeof(RuntimePlayerDataHandler), "runtimePlayerDataHandler", parameters));
			Inject(externalSaveableDataAdapter, resolver, parameters);
			return externalSaveableDataAdapter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
