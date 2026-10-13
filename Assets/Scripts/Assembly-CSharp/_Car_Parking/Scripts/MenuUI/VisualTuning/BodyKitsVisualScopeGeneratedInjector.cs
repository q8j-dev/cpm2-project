using System.Collections.Generic;
using CPM.Database;
using Racing.CameraManagement;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class BodyKitsVisualScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BodyKitsVisualScope bodyKitsVisualScope = new BodyKitsVisualScope();
			Inject(bodyKitsVisualScope, resolver, parameters);
			return bodyKitsVisualScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BodyKitsVisualScope obj = (BodyKitsVisualScope)instance;
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			CameraManager cameraManager = (CameraManager)resolver.ResolveOrParameter(typeof(CameraManager), "cameraManager", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
			BodyKitsTrimConfigs trimConfigs = (BodyKitsTrimConfigs)resolver.ResolveOrParameter(typeof(BodyKitsTrimConfigs), "trimConfigs", parameters);
			obj.Construct(currencyManagerController, carInfoService, cameraManager, managerUI, pricesService, trimConfigs);
		}
	}
}
