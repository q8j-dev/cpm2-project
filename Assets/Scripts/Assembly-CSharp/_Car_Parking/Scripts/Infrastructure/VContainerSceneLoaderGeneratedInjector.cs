using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	internal class VContainerSceneLoaderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VContainerSceneLoader vContainerSceneLoader = new VContainerSceneLoader();
			Inject(vContainerSceneLoader, resolver, parameters);
			return vContainerSceneLoader;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
