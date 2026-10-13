using System.Collections.Generic;
using VContainer;

namespace CPM.Ghost.Core
{
	[Preserve]
	internal class GhostConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GhostConfig ghostConfig = new GhostConfig();
			Inject(ghostConfig, resolver, parameters);
			return ghostConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
