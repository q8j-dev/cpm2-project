using System.Collections.Generic;
using CPM.Fallout;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	internal class ObstacleRaceAnalyticGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceAnalytic obstacleRaceAnalytic = new ObstacleRaceAnalytic((AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters), (ObstacleRaceRunTracker)resolver.ResolveOrParameter(typeof(ObstacleRaceRunTracker), "run", parameters), (IArenaNetwork)resolver.ResolveOrParameter(typeof(IArenaNetwork), "net", parameters), (ArenaFlowController)resolver.ResolveOrParameter(typeof(ArenaFlowController), "flow", parameters), (FalloutRaceSession)resolver.ResolveOrParameter(typeof(FalloutRaceSession), "raceSession", parameters));
			Inject(obstacleRaceAnalytic, resolver, parameters);
			return obstacleRaceAnalytic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
