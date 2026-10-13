using System.Collections.Generic;
using VContainer;

namespace Persistence.Systems
{
	[Preserve]
	internal class DataClassesConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DataClassesConfig dataClassesConfig = new DataClassesConfig();
			Inject(dataClassesConfig, resolver, parameters);
			return dataClassesConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
