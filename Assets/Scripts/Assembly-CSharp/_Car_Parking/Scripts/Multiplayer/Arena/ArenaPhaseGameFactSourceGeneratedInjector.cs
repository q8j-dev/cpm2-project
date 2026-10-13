using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaPhaseGameFactSourceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaPhaseGameFactSource arenaPhaseGameFactSource = new ArenaPhaseGameFactSource((IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "manager", parameters), (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arena", parameters), (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "registry", parameters));
			Inject(arenaPhaseGameFactSource, resolver, parameters);
			return arenaPhaseGameFactSource;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
