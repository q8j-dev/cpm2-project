using System;
using System.Collections.Generic;
using CPM.Ghost;
using VContainer;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	[Preserve]
	internal class GhostToggleWidgetGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GhostToggleWidget cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GhostToggleWidget obj = (GhostToggleWidget)instance;
			GhostStorage storage = (GhostStorage)resolver.ResolveOrParameter(typeof(GhostStorage), "storage", parameters);
			GhostRaceState raceState = (GhostRaceState)resolver.ResolveOrParameter(typeof(GhostRaceState), "raceState", parameters);
			obj.Construct(storage, raceState);
		}
	}
}
