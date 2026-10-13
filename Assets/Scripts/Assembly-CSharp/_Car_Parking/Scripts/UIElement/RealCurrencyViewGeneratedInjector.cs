using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class RealCurrencyViewGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:RealCurrencyView cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RealCurrencyView obj = (RealCurrencyView)instance;
			IPurchaseSystem purchaseSystem = (IPurchaseSystem)resolver.ResolveOrParameter(typeof(IPurchaseSystem), "purchaseSystem", parameters);
			obj.Construct(purchaseSystem);
		}
	}
}
