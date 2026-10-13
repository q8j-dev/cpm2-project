using System.Collections.Generic;
using VContainer;

namespace CPM.Ghost
{
	[Preserve]
	internal class GhostStorageGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GhostStorage ghostStorage = new GhostStorage((string)resolver.ResolveOrParameter(typeof(string), "rootDirectory", parameters), (IGhostLogger)resolver.ResolveOrParameter(typeof(IGhostLogger), "log", parameters));
			Inject(ghostStorage, resolver, parameters);
			return ghostStorage;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
