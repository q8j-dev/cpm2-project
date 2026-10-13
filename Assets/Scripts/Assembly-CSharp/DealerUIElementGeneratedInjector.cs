using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;

[Preserve]
internal class DealerUIElementGeneratedInjector : IInjector
{
	public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		throw new NotSupportedException("UnityEngine.Component:DealerUIElement cannot be `new`");
	}

	public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
	{
		DealerUIElement obj = (DealerUIElement)instance;
		CarPriceHelperV2 carPriceHelperV = (CarPriceHelperV2)resolver.ResolveOrParameter(typeof(CarPriceHelperV2), "carPriceHelperV2", parameters);
		NewCarsMarkController newCarsMarkController = (NewCarsMarkController)resolver.ResolveOrParameter(typeof(NewCarsMarkController), "newCarsMarkController", parameters);
		IOfferSystem offerSystem = (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters);
		ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
		obj.Construct(carPriceHelperV, newCarsMarkController, offerSystem, managerUI);
	}
}
