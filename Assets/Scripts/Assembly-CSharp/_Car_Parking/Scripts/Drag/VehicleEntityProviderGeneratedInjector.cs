using System;
using System.Collections.Generic;
using CPM.Database;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	internal class VehicleEntityProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:VehicleEntityProvider cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VehicleEntityProvider obj = (VehicleEntityProvider)instance;
			ICarInfoService carInfoService = (ICarInfoService)resolver.ResolveOrParameter(typeof(ICarInfoService), "carInfoService", parameters);
			obj.Construct(carInfoService);
		}
	}
}
