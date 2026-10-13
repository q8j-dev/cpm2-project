using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Preserve]
	internal class AllPartsGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			AllParts allParts = new AllParts();
			Inject(allParts, resolver, parameters);
			return allParts;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
