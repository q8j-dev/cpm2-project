using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	[Preserve]
	internal class AllWheelsCounterConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AllWheelsCounterConfig allWheelsCounterConfig = new AllWheelsCounterConfig();
			Inject(allWheelsCounterConfig, resolver, parameters);
			return allWheelsCounterConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
