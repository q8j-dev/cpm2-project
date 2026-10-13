using System;
using System.Collections.Generic;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class BuyGarageTriggerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:BuyGarageTrigger cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BuyGarageTrigger obj = (BuyGarageTrigger)instance;
			IGarageBuyService garageBuyService = (IGarageBuyService)resolver.ResolveOrParameter(typeof(IGarageBuyService), "garageBuyService", parameters);
			obj.Construct(garageBuyService);
		}
	}
}
