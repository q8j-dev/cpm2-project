using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.SellExchangeFree
{
	[Preserve]
	internal class SellExchangeFreeServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SellExchangeFreeService sellExchangeFreeService = new SellExchangeFreeService((ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (RpcRrService)resolver.ResolveOrParameter(typeof(RpcRrService), "rpcRrService", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (MultiPlayer)resolver.ResolveOrParameter(typeof(MultiPlayer), "multiPlayer", parameters), (CarSellTriggersCollection)resolver.ResolveOrParameter(typeof(CarSellTriggersCollection), "carSellTriggersCollection", parameters), (ITradeService)resolver.ResolveOrParameter(typeof(ITradeService), "tradeService", parameters), (TradeServiceFactory)resolver.ResolveOrParameter(typeof(TradeServiceFactory), "tradeServiceFactory", parameters), (TradeAnalytic)resolver.ResolveOrParameter(typeof(TradeAnalytic), "tradeAnalytic", parameters), (IP2PAntiSpam)resolver.ResolveOrParameter(typeof(IP2PAntiSpam), "p2PAntiSpam", parameters), (CarSellTriggerExitHandler)resolver.ResolveOrParameter(typeof(CarSellTriggerExitHandler), "carSellTriggerExitHandler", parameters));
			Inject(sellExchangeFreeService, resolver, parameters);
			return sellExchangeFreeService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
