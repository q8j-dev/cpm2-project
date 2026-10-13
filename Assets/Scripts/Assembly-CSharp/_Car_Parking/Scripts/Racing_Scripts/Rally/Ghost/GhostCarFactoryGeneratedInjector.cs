using System.Collections.Generic;
using CPM.Ghost;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	[Preserve]
	internal class GhostCarFactoryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GhostCarFactory ghostCarFactory = new GhostCarFactory((IGhostLogger)resolver.ResolveOrParameter(typeof(IGhostLogger), "log", parameters));
			Inject(ghostCarFactory, resolver, parameters);
			return ghostCarFactory;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
