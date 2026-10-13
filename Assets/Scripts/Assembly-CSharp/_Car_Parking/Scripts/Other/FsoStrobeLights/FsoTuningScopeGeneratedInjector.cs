using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	[Preserve]
	internal class FsoTuningScopeGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FsoTuningScope fsoTuningScope = new FsoTuningScope();
			Inject(fsoTuningScope, resolver, parameters);
			return fsoTuningScope;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FsoTuningScope obj = (FsoTuningScope)instance;
			ExteriorTuning exteriorTuning = (ExteriorTuning)resolver.ResolveOrParameter(typeof(ExteriorTuning), "exteriorTuning", parameters);
			CoinPurchaseWindow coinPurchaseWindow = (CoinPurchaseWindow)resolver.ResolveOrParameter(typeof(CoinPurchaseWindow), "coinPurchaseWindow", parameters);
			IPrefabFactory prefabFactory = (IPrefabFactory)resolver.ResolveOrParameter(typeof(IPrefabFactory), "prefabFactory", parameters);
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			CurrencyManagerController currencyManagerController = (CurrencyManagerController)resolver.ResolveOrParameter(typeof(CurrencyManagerController), "currencyManagerController", parameters);
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			ISteamAchievementService steamAchievementService = (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters);
			obj.Construct(exteriorTuning, coinPurchaseWindow, prefabFactory, slotService, currencyManagerController, carInfoService, managerUI, steamAchievementService);
		}
	}
}
