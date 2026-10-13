using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.CarSlots
{
	[Preserve]
	internal class SlotCollectionUIHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:SlotCollectionUIHandler cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SlotCollectionUIHandler obj = (SlotCollectionUIHandler)instance;
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			GaragePositionsConfig garagePositionsConfig = (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters);
			GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
			obj.Construct(slotService, prefabFactory, garagePositionsConfig, garageController);
		}
	}
}
