using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Offers.Reward
{
	[Preserve]
	internal class SuitPacksConfigGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			SuitPacksConfig suitPacksConfig = new SuitPacksConfig();
			Inject(suitPacksConfig, resolver, parameters);
			return suitPacksConfig;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
