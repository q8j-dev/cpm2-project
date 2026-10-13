using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	[Preserve]
	internal class ScrollViewSlotsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ScrollViewSlots cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ScrollViewSlots obj = (ScrollViewSlots)instance;
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ITimerTuningService timerTuningService = (ITimerTuningService)resolver.ResolveOrParameter(typeof(ITimerTuningService), "timerTuningService", parameters);
			obj.Construct(slotService, prefabFactory, managerUI, timerTuningService);
		}
	}
}
