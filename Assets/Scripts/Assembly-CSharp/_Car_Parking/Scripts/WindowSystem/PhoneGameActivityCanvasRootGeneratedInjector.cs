using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class PhoneGameActivityCanvasRootGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:PhoneGameActivityCanvasRoot cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PhoneGameActivityCanvasRoot obj = (PhoneGameActivityCanvasRoot)instance;
			IGameActivityCatalog catalog = (IGameActivityCatalog)resolver.ResolveOrParameter(typeof(IGameActivityCatalog), "catalog", parameters);
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			obj.Construct(catalog, eventService, prefabFactory);
		}
	}
}
