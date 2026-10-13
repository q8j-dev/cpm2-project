using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[Preserve]
	internal class ObstacleRaceRunTrackerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceRunTracker obstacleRaceRunTracker = new ObstacleRaceRunTracker();
			Inject(obstacleRaceRunTracker, resolver, parameters);
			return obstacleRaceRunTracker;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
