using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PurchaseSystem;

[Preserve]
internal class IGMShopWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:IGMShopWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		IGMShopWindow obj = (IGMShopWindow)instance;
		IPurchaseSystem purchaseSystem = (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		obj.Construct(purchaseSystem, currencyManagerController);
	}
}
