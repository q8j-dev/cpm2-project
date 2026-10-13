using System.Collections.Generic;
using CPM.Scripts.Garage;
using VContainer;

namespace _Car_Parking.Scripts.ToRoad.Handlers
{
	[Preserve]
	internal class GarageToRoadHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageToRoadHandler garageToRoadHandler = new GarageToRoadHandler((GarageController)resolver.ResolveOrParameter(typeof(GarageController), "garageController", parameters));
			Inject(garageToRoadHandler, resolver, parameters);
			return garageToRoadHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
