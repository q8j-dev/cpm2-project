using System.Collections.Generic;
using VContainer;

namespace Service
{
	[Preserve]
	internal class NamesScriptablesGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			NamesScriptables namesScriptables = new NamesScriptables();
			Inject(namesScriptables, resolver, parameters);
			return namesScriptables;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
