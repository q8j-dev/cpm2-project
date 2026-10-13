using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;

[Preserve]
internal class MaintenanceConfirmWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:MaintenanceConfirmWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		MaintenanceConfirmWindow obj = (MaintenanceConfirmWindow)instance;
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
		VirtualPurchaseAnalytic virtualPurchaseAnalytic = (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters);
		obj.Construct(managerUI, carInfoService, currencyManagerController, pricesService, virtualPurchaseAnalytic);
	}
}
