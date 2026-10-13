using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.GrantService
{
	[Preserve]
	internal class GrantHandlerCarGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			GrantHandlerCar grantHandlerCar = new GrantHandlerCar();
			Inject(grantHandlerCar, resolver, parameters);
			return grantHandlerCar;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
