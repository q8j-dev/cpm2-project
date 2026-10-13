using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class EventGameFactSourceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventGameFactSource eventGameFactSource = new EventGameFactSource((IGameFactManager)resolver.ResolveOrParameter(typeof(IGameFactManager), "manager", parameters), (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (IReadOnlyDictionary<EventType, GameFact>)resolver.ResolveOrParameter(typeof(IReadOnlyDictionary<EventType, GameFact>), "map", parameters));
			Inject(eventGameFactSource, resolver, parameters);
			return eventGameFactSource;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
