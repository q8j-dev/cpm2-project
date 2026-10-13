using System.Collections.Generic;
using CPM.Account;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	internal class TimerTuningServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TimerTuningService timerTuningService = new TimerTuningService((TimesToWaitTuningConfig)resolver.ResolveOrParameter(typeof(TimesToWaitTuningConfig), "timesToWaitTuningConfig", parameters), (ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters), (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters), (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters), (IInventoryPartsService)resolver.ResolveOrParameter(typeof(IInventoryPartsService), "inventoryPartsService", parameters), (IEngineKitsInventoryService)resolver.ResolveOrParameter(typeof(IEngineKitsInventoryService), "engineKitsInventoryService", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters), (IInstallTimeCatalog)resolver.ResolveOrParameter(typeof(IInstallTimeCatalog), "installTimes", parameters), (ISteamAchievementService)resolver.ResolveOrParameter(typeof(ISteamAchievementService), "steamAchievementService", parameters));
			Inject(timerTuningService, resolver, parameters);
			return timerTuningService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
