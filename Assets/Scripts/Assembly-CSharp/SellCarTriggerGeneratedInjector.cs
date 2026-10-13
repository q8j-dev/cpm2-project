using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Trade;

[Preserve]
internal class SellCarTriggerGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:SellCarTrigger cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		SellCarTrigger obj = (SellCarTrigger)instance;
		ISellExchangeFreeService sellExchangeFreeService = (ISellExchangeFreeService)resolver.ResolveOrParameter(typeof(ISellExchangeFreeService), "sellExchangeFreeService", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
		ITradeService tradeService = (ITradeService)resolver.ResolveOrParameter(typeof(ITradeService), "tradeService", parameters);
		FreeDriveProvider freeDriveProvider = (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters);
		CarSellTriggersCollection carSellTriggersCollection = (CarSellTriggersCollection)resolver.ResolveOrParameter(typeof(CarSellTriggersCollection), "carSellTriggersCollection", parameters);
		MultiPlayer multiPlayer = (MultiPlayer)resolver.ResolveOrParameter(typeof(MultiPlayer), "multiPlayer", parameters);
		CarSellTriggerExitHandler carSellTriggerExitHandler = (CarSellTriggerExitHandler)resolver.ResolveOrParameter(typeof(CarSellTriggerExitHandler), "carSellTriggerExitHandler", parameters);
		obj.Construct(sellExchangeFreeService, managerUI, prefabFactory, tradeService, freeDriveProvider, carSellTriggersCollection, multiPlayer, carSellTriggerExitHandler);
	}
}
