using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;

[Preserve]
internal class DriverActionControllerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DriverActionController cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DriverActionController obj = (DriverActionController)instance;
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		HomesConfig homesConfig = (HomesConfig)resolver.ResolveOrParameter(typeof(HomesConfig), "homesConfig", parameters);
		GlobalCollectionsProvider collectionsProvider = (GlobalCollectionsProvider)resolver.ResolveOrParameter(typeof(GlobalCollectionsProvider), "collectionsProvider", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		LevelsProvider levelsProvider = (LevelsProvider)resolver.ResolveOrParameter(typeof(LevelsProvider), "levelsProvider", parameters);
		OptionalDependency<PhotoModeObserver> photoModeObserver = (OptionalDependency<PhotoModeObserver>)resolver.ResolveOrParameter(typeof(OptionalDependency<PhotoModeObserver>), "photoModeObserver", parameters);
		CarInputManager carInputManager = (CarInputManager)resolver.ResolveOrParameter(typeof(CarInputManager), "carInputManager", parameters);
		obj.Construct(currencyManagerController, homesConfig, collectionsProvider, freeDriveProvider, levelsProvider, photoModeObserver, carInputManager);
	}
}
