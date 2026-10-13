using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.UIElement;

namespace Service.Delivery
{
	[Preserve]
	internal class DeliveryManagerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:DeliveryManager cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			DeliveryManager obj = (DeliveryManager)instance;
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			OptionalDependency<ParkingMenuUI> parkingMenuUI = (OptionalDependency<ParkingMenuUI>)resolver.ResolveOrParameter(typeof(OptionalDependency<ParkingMenuUI>), "parkingMenuUI", parameters);
			obj.Construct(managerUI, parkingMenuUI);
		}
	}
}
