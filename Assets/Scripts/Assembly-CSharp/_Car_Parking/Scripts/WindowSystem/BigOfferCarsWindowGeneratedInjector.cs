using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class BigOfferCarsWindowGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:BigOfferCarsWindow cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BigOfferCarsWindow obj = (BigOfferCarsWindow)instance;
			CoreGameManager coreGameManager = (CoreGameManager)resolver.ResolveOrParameter(typeof(CoreGameManager), "coreGameManager", parameters);
			IOfferSystem offerSystem = (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters);
			OptionalDependency<GarageController> garage = (OptionalDependency<GarageController>)resolver.ResolveOrParameter(typeof(OptionalDependency<GarageController>), "garage", parameters);
			IPurchaseSystem purchaseSystem = (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			obj.Construct(coreGameManager, offerSystem, garage, purchaseSystem, managerUI);
		}
	}
}
