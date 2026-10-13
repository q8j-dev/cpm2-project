using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Preserve]
	internal class CarTuningFactoryGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarTuningFactory carTuningFactory = new CarTuningFactory((AllParts)resolver.ResolveOrParameter(typeof(AllParts), "allParts", parameters), (IInventoryPartsService)resolver.ResolveOrParameter(typeof(IInventoryPartsService), "inventoryPartsService", parameters), (IEngineKitsInventoryService)resolver.ResolveOrParameter(typeof(IEngineKitsInventoryService), "engineKitsInventoryService", parameters), (IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters));
			Inject(carTuningFactory, resolver, parameters);
			return carTuningFactory;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
