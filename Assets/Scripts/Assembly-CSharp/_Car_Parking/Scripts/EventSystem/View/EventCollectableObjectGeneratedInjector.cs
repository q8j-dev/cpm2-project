using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EventSystem.View
{
	[Preserve]
	internal class EventCollectableObjectGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EventCollectableObject cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventCollectableObject obj = (EventCollectableObject)instance;
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			EventCollectablesPopupConfig eventCollectablesPopupConfig = (EventCollectablesPopupConfig)resolver.ResolveOrParameter(typeof(EventCollectablesPopupConfig), "eventCollectablesPopupConfig", parameters);
			OptionalDependency<UiNotificationsManager> uiNotificationsManager = (OptionalDependency<UiNotificationsManager>)resolver.ResolveOrParameter(typeof(OptionalDependency<UiNotificationsManager>), "uiNotificationsManager", parameters);
			obj.Construct(eventService, eventCollectablesPopupConfig, uiNotificationsManager);
		}
	}
}
