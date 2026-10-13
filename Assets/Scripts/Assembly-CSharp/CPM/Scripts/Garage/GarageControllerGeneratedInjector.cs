using System;
using System.Collections.Generic;
using CPM.Scripts.Garage.FreeDriverService;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;

namespace CPM.Scripts.Garage
{
	[Preserve]
	internal class GarageControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GarageController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageController obj = (GarageController)instance;
			IGarageEntity garageEntity = (IGarageEntity)resolver.ResolveOrParameter(typeof(IGarageEntity), "garageEntity", parameters);
			IStorageService storageService = (IStorageService)resolver.ResolveOrParameter(typeof(IStorageService), "storageService", parameters);
			ScreenFader screenFader = (ScreenFader)resolver.ResolveOrParameter(typeof(ScreenFader), "screenFader", parameters);
			ICharacterEquipmentDataService characterEquipmentDataService = (ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters);
			IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
			OptionalDependency<IFreeDriverService> freeDriverService = (OptionalDependency<IFreeDriverService>)resolver.ResolveOrParameter(typeof(OptionalDependency<IFreeDriverService>), "freeDriverService", parameters);
			obj.Construct(garageEntity, storageService, screenFader, characterEquipmentDataService, garageSelectorService, slotService, freeDriveProvider, freeDriverService);
		}
	}
}
