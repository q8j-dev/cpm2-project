using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	internal class TimesToWaitTuningConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			TimesToWaitTuningConfig timesToWaitTuningConfig = new TimesToWaitTuningConfig();
			Inject(timesToWaitTuningConfig, resolver, parameters);
			return timesToWaitTuningConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
