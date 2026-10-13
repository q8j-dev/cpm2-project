using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	internal class FreeDriveProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			FreeDriveProvider freeDriveProvider = new FreeDriveProvider();
			Inject(freeDriveProvider, resolver, parameters);
			return freeDriveProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
