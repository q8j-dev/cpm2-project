using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	[Preserve]
	internal class GeoGetterServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GeoGetterService geoGetterService = new GeoGetterService((ServerTimeService)resolver.ResolveOrParameter(typeof(ServerTimeService), "serverTimeService", parameters));
			Inject(geoGetterService, resolver, parameters);
			return geoGetterService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
