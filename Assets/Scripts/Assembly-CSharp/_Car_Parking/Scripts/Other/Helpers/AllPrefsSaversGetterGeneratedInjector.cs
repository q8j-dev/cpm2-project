using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	internal class AllPrefsSaversGetterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AllPrefsSaversGetter allPrefsSaversGetter = new AllPrefsSaversGetter();
			Inject(allPrefsSaversGetter, resolver, parameters);
			return allPrefsSaversGetter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
