using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	[Preserve]
	internal class LeaderboardPickControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:LeaderboardPickController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LeaderboardPickController obj = (LeaderboardPickController)instance;
			GhostRaceState raceState = (GhostRaceState)resolver.ResolveOrParameter(typeof(GhostRaceState), "raceState", parameters);
			obj.Construct(raceState);
		}
	}
}
