using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Car.Physics.AirSuspension
{
	[Preserve]
	internal class AirSuspensionSystemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AirSuspensionSystem airSuspensionSystem = new AirSuspensionSystem((IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters), (PurchaseItemService)resolver.ResolveOrParameter(typeof(PurchaseItemService), "purchaseItemService", parameters));
			Inject(airSuspensionSystem, resolver, parameters);
			return airSuspensionSystem;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
