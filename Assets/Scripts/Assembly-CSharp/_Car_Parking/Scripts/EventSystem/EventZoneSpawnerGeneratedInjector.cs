using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class EventZoneSpawnerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EventZoneSpawner cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventZoneSpawner obj = (EventZoneSpawner)instance;
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			OptionalDependency<IPrefabFactory> prefabFactory = (OptionalDependency<IPrefabFactory>)resolver.ResolveOrParameter(typeof(OptionalDependency<IPrefabFactory>), "prefabFactory", parameters);
			obj.Construct(eventService, prefabFactory);
		}
	}
}
