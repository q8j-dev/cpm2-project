using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts
{
	[Preserve]
	internal class CarSellTriggerExitHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			CarSellTriggerExitHandler carSellTriggerExitHandler = new CarSellTriggerExitHandler((CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (FreeDriveProvider)resolver.ResolveOrParameter(typeof(FreeDriveProvider), "freeDriveProvider", parameters));
			Inject(carSellTriggerExitHandler, resolver, parameters);
			return carSellTriggerExitHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
