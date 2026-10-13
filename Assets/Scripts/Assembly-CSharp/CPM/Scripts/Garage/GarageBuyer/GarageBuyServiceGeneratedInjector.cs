using System.Collections.Generic;
using CPM.Account;
using CPM.RemoteConfig;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace CPM.Scripts.Garage.GarageBuyer
{
	[Preserve]
	internal class GarageBuyServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageBuyService garageBuyService = new GarageBuyService((IWalletService)resolver.ResolveOrParameter(typeof(IWalletService), "walletService", parameters), (PurchaseItemService)resolver.ResolveOrParameter(typeof(PurchaseItemService), "purchaseItemService", parameters), (IGuestAccountCreator)resolver.ResolveOrParameter(typeof(IGuestAccountCreator), "guestAccountCreator", parameters), (IDeleterUserDataGetter)resolver.ResolveOrParameter(typeof(IDeleterUserDataGetter), "deleterUserDataGetter", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfigManager", parameters));
			Inject(garageBuyService, resolver, parameters);
			return garageBuyService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
