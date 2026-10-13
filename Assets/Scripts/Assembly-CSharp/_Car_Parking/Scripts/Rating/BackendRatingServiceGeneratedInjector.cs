using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Rating
{
	[Preserve]
	internal class BackendRatingServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BackendRatingService backendRatingService = new BackendRatingService((BackendClient)resolver.ResolveOrParameter(typeof(BackendClient), "backendClient", parameters));
			Inject(backendRatingService, resolver, parameters);
			return backendRatingService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
