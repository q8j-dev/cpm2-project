using System.Collections.Generic;
using CPM.Connection;
using CPM.Database;
using CPM.PlayerData;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	internal class PurchaseSystemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PurchaseSystem purchaseSystem = new PurchaseSystem((PlayerDataStorage)resolver.ResolveOrParameter(typeof(PlayerDataStorage), "playerDataStorage", parameters), (IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (RealPurchaseAnalytic)resolver.ResolveOrParameter(typeof(RealPurchaseAnalytic), "realPurchaseAnalytic", parameters), (PurchaseItemsCollection)resolver.ResolveOrParameter(typeof(PurchaseItemsCollection), "purchaseItemsCollection", parameters), (IEnumerable<IPurchaseHandler>)resolver.ResolveOrParameter(typeof(IEnumerable<IPurchaseHandler>), "handlers", parameters), (IObjectResolver)resolver.ResolveOrParameter(typeof(IObjectResolver), "container", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (IGuestAccountCreator)resolver.ResolveOrParameter(typeof(IGuestAccountCreator), "guestAccountCreator", parameters), (ISteamPurchaseService)resolver.ResolveOrParameter(typeof(ISteamPurchaseService), "steamPurchaseService", parameters), (ISteamItemCatalog)resolver.ResolveOrParameter(typeof(ISteamItemCatalog), "steamItemCatalog", parameters), (ISteamItemApplier)resolver.ResolveOrParameter(typeof(ISteamItemApplier), "steamItemApplier", parameters), (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters), (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters), (ISteamPurchaseGate)resolver.ResolveOrParameter(typeof(ISteamPurchaseGate), "steamPurchaseGate", parameters));
			Inject(purchaseSystem, resolver, parameters);
			return purchaseSystem;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
