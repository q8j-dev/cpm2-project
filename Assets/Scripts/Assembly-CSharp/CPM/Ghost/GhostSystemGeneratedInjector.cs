using System.Collections.Generic;
using VContainer;

namespace CPM.Ghost
{
	[Preserve]
	internal class GhostSystemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GhostSystem ghostSystem = new GhostSystem((IGhostCarFactory)resolver.ResolveOrParameter(typeof(IGhostCarFactory), "factory", parameters), (GhostStorage)resolver.ResolveOrParameter(typeof(GhostStorage), "storage", parameters), (IGhostLogger)resolver.ResolveOrParameter(typeof(IGhostLogger), "log", parameters));
			Inject(ghostSystem, resolver, parameters);
			return ghostSystem;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
