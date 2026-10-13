using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class DealerWindowFiltersControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:DealerWindowFiltersController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DealerWindowFiltersController obj = (DealerWindowFiltersController)instance;
			AllCarsInfo allCarsInfo = (AllCarsInfo)resolver.ResolveOrParameter(typeof(AllCarsInfo), "allCarsInfo", parameters);
			IOfferSystem offerSystem = (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters);
			CarAvailabilityService carAvailabilityService = (CarAvailabilityService)resolver.ResolveOrParameter(typeof(CarAvailabilityService), "carAvailabilityService", parameters);
			CarPriceHelperV2 carPriceHelper = (CarPriceHelperV2)resolver.ResolveOrParameter(typeof(CarPriceHelperV2), "carPriceHelper", parameters);
			IPurchaseSystem purchaseSystem = (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters);
			obj.Construct(allCarsInfo, offerSystem, carAvailabilityService, carPriceHelper, purchaseSystem);
		}
	}
}
