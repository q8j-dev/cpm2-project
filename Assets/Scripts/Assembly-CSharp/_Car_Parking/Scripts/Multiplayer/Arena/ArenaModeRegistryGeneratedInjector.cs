using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaModeRegistryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaModeRegistry arenaModeRegistry = new ArenaModeRegistry((IEnumerable<IArenaModeController>)resolver.ResolveOrParameter(typeof(IEnumerable<IArenaModeController>), "controllers", parameters), (IEnumerable<ArenaConfig>)resolver.ResolveOrParameter(typeof(IEnumerable<ArenaConfig>), "configs", parameters));
			Inject(arenaModeRegistry, resolver, parameters);
			return arenaModeRegistry;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
