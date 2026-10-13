using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Economics;

[Preserve]
internal class WeightTollStationGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:WeightTollStation cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		WeightTollStation obj = (WeightTollStation)instance;
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
		obj.Construct(currencyManagerController, pricesService);
	}
}
