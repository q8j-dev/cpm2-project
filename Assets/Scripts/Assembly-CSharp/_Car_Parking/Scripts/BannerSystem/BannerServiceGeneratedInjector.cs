using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.BannerSystem
{
	[Preserve]
	internal class BannerServiceGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BannerService bannerService = new BannerService((IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters), (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters), (IPlayerStatusSystem)resolver.ResolveOrParameter(typeof(IPlayerStatusSystem), "playerStatusSystem", parameters));
			Inject(bannerService, resolver, parameters);
			return bannerService;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
