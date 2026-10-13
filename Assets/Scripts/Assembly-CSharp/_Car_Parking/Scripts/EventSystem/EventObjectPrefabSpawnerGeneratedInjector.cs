using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class EventObjectPrefabSpawnerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EventObjectPrefabSpawner cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventObjectPrefabSpawner obj = (EventObjectPrefabSpawner)instance;
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			OptionalDependency<IPrefabFactory> prefabFactory = (OptionalDependency<IPrefabFactory>)resolver.ResolveOrParameter(typeof(OptionalDependency<IPrefabFactory>), "prefabFactory", parameters);
			obj.Construct(eventService, prefabFactory);
		}
	}
}
