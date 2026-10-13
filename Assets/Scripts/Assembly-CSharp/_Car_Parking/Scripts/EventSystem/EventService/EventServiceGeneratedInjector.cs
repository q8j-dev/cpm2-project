using System.Collections.Generic;
using CPM.Database;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Notification;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	[Preserve]
	internal class EventServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventService eventService = new EventService((EventStorage)resolver.ResolveOrParameter(typeof(EventStorage), "eventStorage", parameters), (PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (EventsAnalytic)resolver.ResolveOrParameter(typeof(EventsAnalytic), "eventsAnalytic", parameters), (INotificationService)resolver.ResolveOrParameter(typeof(INotificationService), "notificationService", parameters), (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters));
			Inject(eventService, resolver, parameters);
			return eventService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
