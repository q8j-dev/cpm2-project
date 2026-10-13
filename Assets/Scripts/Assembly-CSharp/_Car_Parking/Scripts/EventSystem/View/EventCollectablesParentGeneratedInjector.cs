using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.EventSystem.View
{
	[Preserve]
	internal class EventCollectablesParentGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EventCollectablesParent cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventCollectablesParent obj = (EventCollectablesParent)instance;
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			obj.Construct(eventService);
		}
	}
}
