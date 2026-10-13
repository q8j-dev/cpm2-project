using System.Collections.Generic;
using CPM.Database;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;

[Preserve]
internal class CalipersVisualTuningScopeGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CalipersVisualTuningScope calipersVisualTuningScope = new CalipersVisualTuningScope();
		Inject(calipersVisualTuningScope, resolver, parameters);
		return calipersVisualTuningScope;
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CalipersVisualTuningScope obj = (CalipersVisualTuningScope)instance;
		ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
		IWheelsDataService wheelsDataService = (IWheelsDataService)resolver.ResolveOrParameter(typeof(IWheelsDataService), "wheelsDataService", parameters);
		BrakeTuningConfig brakeTuningConfig = (BrakeTuningConfig)resolver.ResolveOrParameter(typeof(BrakeTuningConfig), "brakeTuningConfig", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
		MenuControl menuControl = (MenuControl)resolver.ResolveOrParameter(typeof(MenuControl), "menuControl", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		obj.Construct(carInfoService, wheelsDataService, brakeTuningConfig, currencyManagerController, cameraManager, menuControl, managerUI);
	}
}
