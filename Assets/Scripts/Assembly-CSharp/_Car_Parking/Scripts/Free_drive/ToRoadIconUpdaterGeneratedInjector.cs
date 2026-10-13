using System;
using System.Collections.Generic;
using CPM.Scripts.Garage;
using VContainer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.Free_drive
{
	[Preserve]
	internal class ToRoadIconUpdaterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:ToRoadIconUpdater cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			ToRoadIconUpdater obj = (ToRoadIconUpdater)instance;
			OptionalDependency<GarageController> garageController = (OptionalDependency<GarageController>)resolver.ResolveOrParameter(typeof(OptionalDependency<GarageController>), "garageController", parameters);
			obj.Construct(garageController);
		}
	}
}
