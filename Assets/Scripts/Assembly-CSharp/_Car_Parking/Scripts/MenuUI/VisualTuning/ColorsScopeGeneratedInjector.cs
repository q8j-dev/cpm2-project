using System.Collections.Generic;
using CPM.Database;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class ColorsScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ColorsScope colorsScope = new ColorsScope();
			Inject(colorsScope, resolver, parameters);
			return colorsScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ColorsScope obj = (ColorsScope)instance;
			ColorsService colorsService = (ColorsService)resolver.ResolveOrParameter(typeof(ColorsService), "colorsService", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			IPaintDataService paintDataService = (IPaintDataService)resolver.ResolveOrParameter(typeof(IPaintDataService), "paintDataService", parameters);
			VisualTuningPricesService visualTuningPricesService = (VisualTuningPricesService)resolver.ResolveOrParameter(typeof(VisualTuningPricesService), "visualTuningPricesService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ColorsScopeSmokeConfig colorsScopeSmokeConfig = (ColorsScopeSmokeConfig)resolver.ResolveOrParameter(typeof(ColorsScopeSmokeConfig), "colorsScopeSmokeConfig", parameters);
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			obj.Construct(colorsService, carInfoService, cameraManager, currencyManagerController, paintDataService, visualTuningPricesService, managerUI, colorsScopeSmokeConfig, steamAchievementService);
		}
	}
}
