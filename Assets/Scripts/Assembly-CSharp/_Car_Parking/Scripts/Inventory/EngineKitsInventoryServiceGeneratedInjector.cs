using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	[Preserve]
	internal class EngineKitsInventoryServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EngineKitsInventoryService engineKitsInventoryService = new EngineKitsInventoryService();
			Inject(engineKitsInventoryService, resolver, parameters);
			return engineKitsInventoryService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			EngineKitsInventoryService obj = (EngineKitsInventoryService)instance;
			AllParts allParts = (AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters);
			IStorageService storageService = (IStorageService)resolver.ResolveOrParameter(typeof(IStorageService), "storageService", parameters);
			obj.Construct(allParts, storageService);
		}
	}
}
