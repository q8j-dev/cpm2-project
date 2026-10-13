using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	internal class RacingProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RacingProvider racingProvider = new RacingProvider();
			Inject(racingProvider, resolver, parameters);
			return racingProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
