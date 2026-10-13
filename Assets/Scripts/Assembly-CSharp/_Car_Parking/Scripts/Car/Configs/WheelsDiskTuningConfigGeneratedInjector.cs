using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Car.Configs
{
	[Preserve]
	internal class WheelsDiskTuningConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			WheelsDiskTuningConfig wheelsDiskTuningConfig = new WheelsDiskTuningConfig();
			Inject(wheelsDiskTuningConfig, resolver, parameters);
			return wheelsDiskTuningConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
