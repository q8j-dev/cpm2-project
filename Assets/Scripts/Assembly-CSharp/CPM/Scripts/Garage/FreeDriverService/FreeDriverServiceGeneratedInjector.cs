using System.Collections.Generic;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;

namespace CPM.Scripts.Garage.FreeDriverService
{
	[Preserve]
	internal class FreeDriverServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeDriverService freeDriverService = new FreeDriverService((ICharacterEquipmentDataService)resolver.ResolveOrParameter(typeof(ICharacterEquipmentDataService), "characterEquipmentDataService", parameters), (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters), (GaragePositionsConfig)resolver.ResolveOrParameter(typeof(GaragePositionsConfig), "garagePositionsConfig", parameters), (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters));
			Inject(freeDriverService, resolver, parameters);
			return freeDriverService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
