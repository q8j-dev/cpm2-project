using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event
{
	[Preserve]
	internal class ItemEventObstacleRaceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ItemEventObstacleRace cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ItemEventObstacleRace obj = (ItemEventObstacleRace)instance;
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			obj.Construct(eventService);
		}
	}
}
