using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.VinylsAi
{
	[Preserve]
	internal class VinylsCheckApiGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			VinylsCheckApi vinylsCheckApi = new VinylsCheckApi();
			Inject(vinylsCheckApi, resolver, parameters);
			return vinylsCheckApi;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
