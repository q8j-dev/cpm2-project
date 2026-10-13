using System.Collections.Generic;
using CPM.Ghost;
using CPM.Ghost.Core;
using RacingScripts.RaceControllers;
using VContainer;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	[Preserve]
	internal class RallyGhostControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RallyGhostController rallyGhostController = new RallyGhostController((GhostSystem)resolver.ResolveOrParameter(typeof(GhostSystem), "system", parameters), (GhostStorage)resolver.ResolveOrParameter(typeof(GhostStorage), "storage", parameters), (GhostConfig)resolver.ResolveOrParameter(typeof(GhostConfig), "config", parameters), (RaceModeController)resolver.ResolveOrParameter(typeof(RaceModeController), "raceModeController", parameters), (RallyRecordsSync)resolver.ResolveOrParameter(typeof(RallyRecordsSync), "records", parameters), (RallyRecordsBank)resolver.ResolveOrParameter(typeof(RallyRecordsBank), "bank", parameters), (GhostRaceState)resolver.ResolveOrParameter(typeof(GhostRaceState), "raceState", parameters));
			Inject(rallyGhostController, resolver, parameters);
			return rallyGhostController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
