using System.Collections.Generic;
using Persistence.Systems;
using VContainer;

namespace _Car_Parking.Scripts.Persistence.Migration
{
	[Preserve]
	internal class MigrationManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MigrationManager migrationManager = new MigrationManager((VersionedDataClassesConfig)resolver.ResolveOrParameter(typeof(VersionedDataClassesConfig), "versionedDataClassesConfig", parameters));
			Inject(migrationManager, resolver, parameters);
			return migrationManager;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
