using System;
using System.Collections.Generic;
using VContainer;

namespace CPM.Scripts.Garage
{
	[Preserve]
	internal class GarageSpotsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GarageSpots cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageSpots obj = (GarageSpots)instance;
			GarageController garageController = (GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters);
			obj.Construct(garageController);
		}
	}
}
