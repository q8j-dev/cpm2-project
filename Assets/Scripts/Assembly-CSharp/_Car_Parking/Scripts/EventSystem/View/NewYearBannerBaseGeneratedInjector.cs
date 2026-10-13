using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.EventSystem.View
{
	[Preserve]
	internal class NewYearBannerBaseGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:NewYearBannerBase cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			NewYearBannerBase obj = (NewYearBannerBase)instance;
			ServerTimeService serverTimeService = (ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters);
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			EventsAnalytic eventsAnalytic = (EventsAnalytic)resolver.ResolveOrParameter(typeof(EventsAnalytic), "eventsAnalytic", parameters);
			obj.Construct(serverTimeService, eventService, eventsAnalytic);
		}
	}
}
