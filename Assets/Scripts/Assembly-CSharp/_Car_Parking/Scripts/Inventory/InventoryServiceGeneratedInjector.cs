using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	[Preserve]
	internal class InventoryServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			InventoryService inventoryService = new InventoryService((AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters), (IStorageService)resolver.ResolveOrParameter(typeof(IStorageService), "storageService", parameters));
			Inject(inventoryService, resolver, parameters);
			return inventoryService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
