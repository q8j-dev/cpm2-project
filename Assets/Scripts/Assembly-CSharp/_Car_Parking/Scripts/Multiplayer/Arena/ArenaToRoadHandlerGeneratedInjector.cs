using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ArenaToRoadHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ArenaToRoadHandler arenaToRoadHandler = new ArenaToRoadHandler((ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "arenaFlow", parameters), (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "modeRegistry", parameters), (ArenaPointsContainer)resolver.ResolveOrParameter(typeof(ArenaPointsContainer), "pointsContainer", parameters), (IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "gameFactManager", parameters));
			Inject(arenaToRoadHandler, resolver, parameters);
			return arenaToRoadHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
