using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Economics
{
	[Preserve]
	internal class AllPricesConfigConstGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AllPricesConfigConst allPricesConfigConst = new AllPricesConfigConst();
			Inject(allPricesConfigConst, resolver, parameters);
			return allPricesConfigConst;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
