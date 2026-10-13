using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Garage.ForbiddenZones
{
	[Preserve]
	internal class GarageForbiddenZonesProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageForbiddenZonesProvider garageForbiddenZonesProvider = new GarageForbiddenZonesProvider();
			Inject(garageForbiddenZonesProvider, resolver, parameters);
			return garageForbiddenZonesProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
