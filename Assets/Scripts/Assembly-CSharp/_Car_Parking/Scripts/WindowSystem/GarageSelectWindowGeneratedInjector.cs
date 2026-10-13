using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class GarageSelectWindowGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GarageSelectWindow cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageSelectWindow obj = (GarageSelectWindow)instance;
			IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
			ProgressContainer progressContainer = (ProgressContainer)resolver.ResolveOrParameter(typeof(ProgressContainer), "progressContainer", parameters);
			IGarageBuyService garageBuyService = (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters);
			ISlotService selectedSlotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "selectedSlotService", parameters);
			obj.Construct(garageSelectorService, progressContainer, garageBuyService, selectedSlotService);
		}
	}
}
