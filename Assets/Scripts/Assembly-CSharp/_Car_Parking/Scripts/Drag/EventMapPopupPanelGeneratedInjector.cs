using System;
using System.Collections.Generic;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class EventMapPopupPanelGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:EventMapPopupPanel cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EventMapPopupPanel obj = (EventMapPopupPanel)instance;
			TimeAttackDataSO timeAttackDataSo = (TimeAttackDataSO)resolver.ResolveOrParameter(typeof(TimeAttackDataSO), "timeAttackDataSo", parameters);
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			ServerTimeService serverTimeService = (ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(timeAttackDataSo, eventService, serverTimeService, managerUI);
		}
	}
}
