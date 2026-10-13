using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;

[Preserve]
internal class CollectiblePoolContainerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CollectiblePoolContainer cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CollectiblePoolContainer obj = (CollectiblePoolContainer)instance;
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
		obj.Construct(prefabFactory, eventService);
	}
}
