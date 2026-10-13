using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.CarSlots
{
	[Preserve]
	internal class SlotUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SlotUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SlotUI obj = (SlotUI)instance;
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
			IGarageEntity garageEntity = (IGarageEntity)resolver.ResolveOrParameter(typeof(IGarageEntity), "garageEntity", parameters);
			VirtualPurchaseAnalytic virtualPurchaseAnalytic = (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters);
			obj.Construct(carInfoService, managerUI, slotService, garageController, garageEntity, virtualPurchaseAnalytic);
		}
	}
}
