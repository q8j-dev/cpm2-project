using System.Collections.Generic;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class MatchmakingLobbyAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MatchmakingLobbyAnalytic matchmakingLobbyAnalytic = new MatchmakingLobbyAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "flow", parameters), (IArenaNetwork)resolver.ResolveOrParameter(typeof(IArenaNetwork), "net", parameters), (ArenaModeRegistry)resolver.ResolveOrParameter(typeof(ArenaModeRegistry), "registry", parameters), (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerData", parameters));
			Inject(matchmakingLobbyAnalytic, resolver, parameters);
			return matchmakingLobbyAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
