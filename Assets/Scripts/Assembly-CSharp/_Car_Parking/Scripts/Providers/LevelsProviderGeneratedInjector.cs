using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Providers
{
	[Preserve]
	internal class LevelsProviderGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			LevelsProvider levelsProvider = new LevelsProvider();
			Inject(levelsProvider, resolver, parameters);
			return levelsProvider;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
