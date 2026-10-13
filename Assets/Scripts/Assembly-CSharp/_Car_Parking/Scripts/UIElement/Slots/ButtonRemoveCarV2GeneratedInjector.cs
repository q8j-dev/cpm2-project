using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	[Preserve]
	internal class ButtonRemoveCarV2GeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ButtonRemoveCarV2 cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ButtonRemoveCarV2 obj = (ButtonRemoveCarV2)instance;
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			obj.Construct(slotService, managerUI, carInfoService);
		}
	}
}
