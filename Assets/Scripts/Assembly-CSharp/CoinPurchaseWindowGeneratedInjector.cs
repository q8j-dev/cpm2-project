using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

[Preserve]
internal class CoinPurchaseWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CoinPurchaseWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CoinPurchaseWindow obj = (CoinPurchaseWindow)instance;
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		OptionalDependency<ParkingMenuUI> parkingMenuUI = (OptionalDependency<ParkingMenuUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ParkingMenuUI>), "parkingMenuUI", parameters);
		obj.Construct(currencyManagerController, levelsProvider, freeDriveProvider, parkingMenuUI);
	}
}
