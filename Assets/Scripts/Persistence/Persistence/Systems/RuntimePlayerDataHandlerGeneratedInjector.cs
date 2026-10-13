using System.Collections.Generic;
using VContainer;

namespace Persistence.Systems
{
	[Preserve]
	internal class RuntimePlayerDataHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RuntimePlayerDataHandler runtimePlayerDataHandler = new RuntimePlayerDataHandler((DataClassesConfig)resolver.ResolveOrParameter(typeof(DataClassesConfig), "dataClassesConfig", parameters));
			Inject(runtimePlayerDataHandler, resolver, parameters);
			return runtimePlayerDataHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
