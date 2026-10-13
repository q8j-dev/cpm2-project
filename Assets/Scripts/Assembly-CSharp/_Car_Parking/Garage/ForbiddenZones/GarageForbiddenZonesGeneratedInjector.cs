using System;
using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Garage.ForbiddenZones
{
	[Preserve]
	internal class GarageForbiddenZonesGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:GarageForbiddenZones cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GarageForbiddenZones obj = (GarageForbiddenZones)instance;
			IForbiddenZonesRegistrar registrar = (IForbiddenZonesRegistrar)resolver.ResolveOrParameter(typeof(IForbiddenZonesRegistrar), "registrar", parameters);
			obj.Construct(registrar);
		}
	}
}
