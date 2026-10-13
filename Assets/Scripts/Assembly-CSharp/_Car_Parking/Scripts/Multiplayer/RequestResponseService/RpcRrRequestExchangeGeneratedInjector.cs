using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	internal class RpcRrRequestExchangeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RpcRrRequestExchange rpcRrRequestExchange = new RpcRrRequestExchange((FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters), (ITradeService)resolver.ResolveOrParameter(typeof(ITradeService), "tradeService", parameters), (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (IPoliceSystem)resolver.ResolveOrParameter(typeof(IPoliceSystem), "policeSystem", parameters), (IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters), (LockCarButtonController)resolver.ResolveOrParameter(typeof(LockCarButtonController), "lockCarButtonController", parameters), (TradeAnalytic)resolver.ResolveOrParameter(typeof(TradeAnalytic), "tradeAnalytic", parameters), (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters), (IProgressBackend)resolver.ResolveOrParameter(typeof(IProgressBackend), "progressBackend", parameters), (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters), (ITradeTransactionBackend)resolver.ResolveOrParameter(typeof(ITradeTransactionBackend), "tradeTransactionBackend", parameters));
			Inject(rpcRrRequestExchange, resolver, parameters);
			return rpcRrRequestExchange;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
