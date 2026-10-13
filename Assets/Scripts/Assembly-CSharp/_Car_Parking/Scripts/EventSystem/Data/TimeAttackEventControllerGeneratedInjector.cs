using System.Collections.Generic;
using Car_Parking.Scripts.Racing;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;

namespace _Car_Parking.Scripts.EventSystem.Data
{
	[Preserve]
	internal class TimeAttackEventControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TimeAttackEventController timeAttackEventController = new TimeAttackEventController();
			Inject(timeAttackEventController, resolver, parameters);
			return timeAttackEventController;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TimeAttackEventController obj = (TimeAttackEventController)instance;
			EventRestrictionService eventRestrictionService = (EventRestrictionService)resolver.ResolveOrParameter(typeof(EventRestrictionService), "eventRestrictionService", parameters);
			TimeBannersManager timeBannersManager = (TimeBannersManager)resolver.ResolveOrParameter(typeof(TimeBannersManager), "timeBannersManager", parameters);
			FreeTimeAttackController freeTimeAttackController = (FreeTimeAttackController)resolver.ResolveOrParameter(typeof(FreeTimeAttackController), "freeTimeAttackController", parameters);
			UiNotificationsManager uiNotificationsManager = (UiNotificationsManager)resolver.ResolveOrParameter(typeof(UiNotificationsManager), "uiNotificationsManager", parameters);
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			obj.Construct(eventRestrictionService, timeBannersManager, freeTimeAttackController, uiNotificationsManager, eventService);
		}
	}
}
