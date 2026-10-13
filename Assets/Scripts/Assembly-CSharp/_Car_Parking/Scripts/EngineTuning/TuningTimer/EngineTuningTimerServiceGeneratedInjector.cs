using System.Collections.Generic;
using CPM.Account;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	internal class EngineTuningTimerServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EngineTuningTimerService engineTuningTimerService = new EngineTuningTimerService((TimesToWaitTuningConfig)resolver.ResolveOrParameter(typeof(TimesToWaitTuningConfig), "timesToWaitTuningConfig", parameters), (ICarTuningFactory)resolver.ResolveOrParameter(typeof(ICarTuningFactory), "carTuningFactory", parameters), (IEngineKitsInventoryService)resolver.ResolveOrParameter(typeof(IEngineKitsInventoryService), "engineKitsInventoryService", parameters), (IInventoryPartsService)resolver.ResolveOrParameter(typeof(IInventoryPartsService), "inventoryPartsService", parameters), (OptionalDependency<ManagerUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ManagerUI>), "managerUI", parameters), (IInstallTimeCatalog)resolver.ResolveOrParameter(typeof(IInstallTimeCatalog), "installTimes", parameters), (IAccountController)resolver.ResolveOrParameter(typeof(IAccountController), "accountController", parameters));
			Inject(engineTuningTimerService, resolver, parameters);
			return engineTuningTimerService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
