using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;

namespace _Car_Parking.Garage.Poses
{
	[Preserve]
	internal class GarageCarPoseServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageCarPoseService garageCarPoseService = new GarageCarPoseService((IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters));
			Inject(garageCarPoseService, resolver, parameters);
			return garageCarPoseService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
