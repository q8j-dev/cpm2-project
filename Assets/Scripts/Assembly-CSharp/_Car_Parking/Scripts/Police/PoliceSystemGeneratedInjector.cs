using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Database.Cars;

namespace _Car_Parking.Scripts.Police
{
	[Preserve]
	internal class PoliceSystemGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			PoliceSystem policeSystem = new PoliceSystem((IDatabaseCarsService)resolver.ResolveOrParameter(typeof(IDatabaseCarsService), "databaseCarsService", parameters), (PoliceSystemConfig)resolver.ResolveOrParameter(typeof(PoliceSystemConfig), "policeSystemConfig", parameters));
			Inject(policeSystem, resolver, parameters);
			return policeSystem;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
