using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.Scripts.BuyNewCar
{
	[Preserve]
	internal class BuyCarBodyKitsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:BuyCarBodyKits cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BuyCarBodyKits obj = (BuyCarBodyKits)instance;
			IPricesService pricesService = (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(pricesService, managerUI);
		}
	}
}
