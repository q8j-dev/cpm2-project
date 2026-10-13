using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	[Preserve]
	internal class GhostRaceStateGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GhostRaceState ghostRaceState = new GhostRaceState();
			Inject(ghostRaceState, resolver, parameters);
			return ghostRaceState;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
