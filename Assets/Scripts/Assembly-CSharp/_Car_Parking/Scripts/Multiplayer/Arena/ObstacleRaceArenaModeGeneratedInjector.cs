using System.Collections.Generic;
using CPM.Fallout;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ObstacleRaceArenaModeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceArenaMode obstacleRaceArenaMode = new ObstacleRaceArenaMode((LobbyRoster)resolver.ResolveOrParameter(typeof(LobbyRoster), "roster", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (OnlineCollectionsProvider)resolver.ResolveOrParameter(typeof(OnlineCollectionsProvider), "online", parameters), (FalloutRaceSession)resolver.ResolveOrParameter(typeof(FalloutRaceSession), "raceSession", parameters), (ObstacleRaceMusic)resolver.ResolveOrParameter(typeof(ObstacleRaceMusic), "music", parameters), (ObstacleRaceRunTracker)resolver.ResolveOrParameter(typeof(ObstacleRaceRunTracker), "analyticsRun", parameters), (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters));
			Inject(obstacleRaceArenaMode, resolver, parameters);
			return obstacleRaceArenaMode;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
