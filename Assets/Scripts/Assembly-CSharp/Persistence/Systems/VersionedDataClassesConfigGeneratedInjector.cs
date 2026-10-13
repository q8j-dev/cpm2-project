using System.Collections.Generic;
using VContainer;

namespace Persistence.Systems
{
	[Preserve]
	internal class VersionedDataClassesConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VersionedDataClassesConfig versionedDataClassesConfig = new VersionedDataClassesConfig();
			Inject(versionedDataClassesConfig, resolver, parameters);
			return versionedDataClassesConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
