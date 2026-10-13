using System.Collections.Generic;
using VContainer;

namespace _Car_Parking.Scripts.Backend
{
	[Preserve]
	internal class UnityHttpClientGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			UnityHttpClient unityHttpClient = new UnityHttpClient();
			Inject(unityHttpClient, resolver, parameters);
			return unityHttpClient;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
