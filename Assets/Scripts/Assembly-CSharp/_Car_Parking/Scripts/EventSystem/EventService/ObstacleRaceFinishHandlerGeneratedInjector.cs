using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	[Preserve]
	internal class ObstacleRaceFinishHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ObstacleRaceFinishHandler obstacleRaceFinishHandler = new ObstacleRaceFinishHandler((IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters));
			Inject(obstacleRaceFinishHandler, resolver, parameters);
			return obstacleRaceFinishHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
