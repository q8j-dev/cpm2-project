using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Trade
{
	[Preserve]
	internal class TradeOfferHandlerCarMoneyPublicGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TradeOfferHandlerCarMoneyPublic tradeOfferHandlerCarMoneyPublic = new TradeOfferHandlerCarMoneyPublic((ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (RpcRrService)resolver.ResolveOrParameter(typeof(RpcRrService), "rpcRrService", parameters), (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters), (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters), (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters), (IProgressBackend)resolver.ResolveOrParameter(typeof(IProgressBackend), "progressBackend", parameters), (TradeAnalytic)resolver.ResolveOrParameter(typeof(TradeAnalytic), "tradeAnalytic", parameters), (ITradeTransactionBackend)resolver.ResolveOrParameter(typeof(ITradeTransactionBackend), "tradeTransactionBackend", parameters));
			Inject(tradeOfferHandlerCarMoneyPublic, resolver, parameters);
			return tradeOfferHandlerCarMoneyPublic;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
