using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Economics;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	[Preserve]
	internal class VisualTuningPricesServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VisualTuningPricesService visualTuningPricesService = new VisualTuningPricesService((IWheelsDataService)resolver.ResolveOrParameter(typeof(IWheelsDataService), "wheelsDataService", parameters), (WheelsDiskTuningConfig)resolver.ResolveOrParameter(typeof(WheelsDiskTuningConfig), "wheelsDiskTuningConfig", parameters), (BrakeTuningConfig)resolver.ResolveOrParameter(typeof(BrakeTuningConfig), "brakeTuningConfig", parameters), (IPricesService)resolver.ResolveOrParameter(typeof(IPricesService), "pricesService", parameters));
			Inject(visualTuningPricesService, resolver, parameters);
			return visualTuningPricesService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
