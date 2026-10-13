using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.Garage.FreeDriverService;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Garage.ForbiddenZones;
using _Car_Parking.Garage.Poses;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;

namespace CPM.Scripts.Garage
{
	[Preserve]
	internal class MenuGarageGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MenuGarage menuGarage = new MenuGarage();
			Inject(menuGarage, resolver, parameters);
			return menuGarage;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			MenuGarage obj = (MenuGarage)instance;
			PlayerDatas.CarLoadingUtility loadingUtility = (PlayerDatas.CarLoadingUtility)resolver.ResolveOrParameter(typeof(PlayerDatas.CarLoadingUtility), "loadingUtility", parameters);
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			IStorageService storageService = (IStorageService)resolver.ResolveOrParameter(typeof(IStorageService), "storageService", parameters);
			InputItemsProvider controlItemsProvider = (InputItemsProvider)resolver.ResolveOrParameter(typeof(InputItemsProvider), "controlItemsProvider", parameters);
			ScreenFader screenFader = (ScreenFader)resolver.ResolveOrParameter(typeof(ScreenFader), "screenFader", parameters);
			CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
			IGarageCarPoseService garagePoseService = (IGarageCarPoseService)resolver.ResolveOrParameter(typeof(IGarageCarPoseService), "garagePoseService", parameters);
			IGarageSelectorService garageSelectorService = (IGarageSelectorService)resolver.ResolveOrParameter(typeof(IGarageSelectorService), "garageSelectorService", parameters);
			IForbiddenZonesProvider zonesProvider = (IForbiddenZonesProvider)resolver.ResolveOrParameter(typeof(IForbiddenZonesProvider), "zonesProvider", parameters);
			AnalyticEventFactory analyticEventFactory = (AnalyticEventFactory)resolver.ResolveOrParameter(typeof(AnalyticEventFactory), "analyticEventFactory", parameters);
			AssetBundleManager assetBundleManager = (AssetBundleManager)resolver.ResolveOrParameter(typeof(AssetBundleManager), "assetBundleManager", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
			OptionalDependency<ManagerUI> managerUI = (OptionalDependency<ManagerUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ManagerUI>), "managerUI", parameters);
			OptionalDependency<CameraManager> cameraManager = (OptionalDependency<CameraManager>)resolver.ResolveOrParameter(typeof(OptionalDependency<CameraManager>), "cameraManager", parameters);
			OptionalDependency<ICarTuningFactory> carTuningFactory = (OptionalDependency<ICarTuningFactory>)resolver.ResolveOrParameter(typeof(OptionalDependency<ICarTuningFactory>), "carTuningFactory", parameters);
			OptionalDependency<IFreeDriverService> freeDriverService = (OptionalDependency<IFreeDriverService>)resolver.ResolveOrParameter(typeof(OptionalDependency<IFreeDriverService>), "freeDriverService", parameters);
			RainSoundState rainSoundState = (RainSoundState)resolver.ResolveOrParameter(typeof(RainSoundState), "rainSoundState", parameters);
			SoundControllerExtension carInOutSound = (SoundControllerExtension)resolver.ResolveOrParameter(typeof(SoundControllerExtension), "carInOutSound", parameters);
			obj.Construct(loadingUtility, slotService, storageService, controlItemsProvider, screenFader, carInputManager, garagePoseService, garageSelectorService, zonesProvider, analyticEventFactory, assetBundleManager, carInfoService, allCarsInfo, managerUI, cameraManager, carTuningFactory, freeDriverService, rainSoundState, carInOutSound);
		}
	}
}
