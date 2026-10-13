using System.Collections.Generic;
using Persistence.Systems;
using VContainer;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	[Preserve]
	internal class LocalDataMigrationManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LocalDataMigrationManager localDataMigrationManager = new LocalDataMigrationManager((RuntimePlayerDataHandler)resolver.ResolveOrParameter(typeof(RuntimePlayerDataHandler), "runtimePlayerDataHandler", parameters), (VersionedDataClassesConfig)resolver.ResolveOrParameter(typeof(VersionedDataClassesConfig), "versionedDataClassesConfig", parameters), (IMigrationManager)resolver.ResolveOrParameter(typeof(IMigrationManager), "migrationManager", parameters));
			Inject(localDataMigrationManager, resolver, parameters);
			return localDataMigrationManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
