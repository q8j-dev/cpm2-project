using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.UIElement
{
	[Preserve]
	internal class ButtonBuyNewCarGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ButtonBuyNewCar cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ButtonBuyNewCar obj = (ButtonBuyNewCar)instance;
			VirtualPurchaseAnalytic virtualPurchaseAnalytic = (VirtualPurchaseAnalytic)resolver.ResolveOrParameter(typeof(VirtualPurchaseAnalytic), "virtualPurchaseAnalytic", parameters);
			ISlotService slotService = (ISlotService)resolver.ResolveOrParameter(typeof(ISlotService), "slotService", parameters);
			obj.Construct(virtualPurchaseAnalytic, slotService);
		}
	}
}
