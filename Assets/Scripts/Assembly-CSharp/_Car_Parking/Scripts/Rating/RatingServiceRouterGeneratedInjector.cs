using System.Collections.Generic;
using CPM.RemoteConfig;
using VContainer;

namespace _Car_Parking.Scripts.Rating
{
	[Preserve]
	internal class RatingServiceRouterGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			RatingServiceRouter ratingServiceRouter = new RatingServiceRouter((FirebaseRatingService)resolver.ResolveOrParameter(typeof(FirebaseRatingService), "firebase", parameters), (BackendRatingService)resolver.ResolveOrParameter(typeof(BackendRatingService), "backend", parameters), (IRemoteConfigManager)resolver.ResolveOrParameter(typeof(IRemoteConfigManager), "remoteConfig", parameters));
			Inject(ratingServiceRouter, resolver, parameters);
			return ratingServiceRouter;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
