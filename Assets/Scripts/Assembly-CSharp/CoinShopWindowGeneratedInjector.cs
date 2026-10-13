using System;
using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.PurchaseSystem;

[Preserve]
internal class CoinShopWindowGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:CoinShopWindow cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		CoinShopWindow obj = (CoinShopWindow)instance;
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
		IRemoteConfigManager remoteConfigManager = (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters);
		IGuestAccountCreator guestAccountCreator = (IGuestAccountCreator)resolver.ResolveOrParameter(typeof(IGuestAccountCreator), "guestAccountCreator", parameters);
		IPurchaseSystem purchaseSystem = (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters);
		obj.Construct(freeDriveProvider, currencyManagerController, remoteConfigManager, guestAccountCreator, purchaseSystem);
	}
}
