using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Scopes
{
	[Preserve]
	internal class SceneScopeRegistryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SceneScopeRegistry sceneScopeRegistry = new SceneScopeRegistry();
			Inject(sceneScopeRegistry, resolver, parameters);
			return sceneScopeRegistry;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
