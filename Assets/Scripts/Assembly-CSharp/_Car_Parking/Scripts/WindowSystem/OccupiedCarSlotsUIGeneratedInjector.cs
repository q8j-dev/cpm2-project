using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.CarSlots;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	internal class OccupiedCarSlotsUIGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:OccupiedCarSlotsUI cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OccupiedCarSlotsUI obj = (OccupiedCarSlotsUI)instance;
			CarSlotsObserver carSlotsObserver = (CarSlotsObserver)resolver.ResolveOrParameter(typeof(CarSlotsObserver), "carSlotsObserver", parameters);
			obj.Construct(carSlotsObserver);
		}
	}
}
