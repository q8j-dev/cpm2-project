using System.Collections.Generic;
using CPM.Database;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class WheelsTuningScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WheelsTuningScope wheelsTuningScope = new WheelsTuningScope();
			Inject(wheelsTuningScope, resolver, parameters);
			return wheelsTuningScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WheelsTuningScope obj = (WheelsTuningScope)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			MenuControl menuControl = (MenuControl)resolver.ResolveOrParameter(typeof(MenuControl), "menuControl", parameters);
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			IWheelsDataService wheelsDataService = (IWheelsDataService)resolver.ResolveOrParameter(typeof(IWheelsDataService), "wheelsDataService", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			WheelsDiskTuningConfig wheelsDiskTuningConfig = (WheelsDiskTuningConfig)resolver.ResolveOrParameter(typeof(WheelsDiskTuningConfig), "wheelsDiskTuningConfig", parameters);
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			ICarTuningFactory carTuningFactory = (ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters);
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			obj.Construct(managerUI, menuControl, cameraManager, wheelsDataService, carInfoService, wheelsDiskTuningConfig, currencyManagerController, carTuningFactory, steamAchievementService);
		}
	}
}
