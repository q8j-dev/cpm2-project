using System;
using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.BannerSystem;
using _Car_Parking.Scripts.DeepLink;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.EventSystem
{
	[Preserve]
	internal class BannerShowControllerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			throw new NotSupportedException("UnityEngine.Component:BannerShowController cannot be `new`");
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			BannerShowController obj = (BannerShowController)instance;
			IEventService eventService = (IEventService)resolver.ResolveOrParameter(typeof(IEventService), "eventService", parameters);
			IBannerService bannerService = (IBannerService)resolver.ResolveOrParameter(typeof(IBannerService), "bannerService", parameters);
			ManagerUI managerUI = (ManagerUI)resolver.ResolveOrParameter(typeof(ManagerUI), "managerUI", parameters);
			DeepLinkProvider deepLinkProvider = (DeepLinkProvider)resolver.ResolveOrParameter(typeof(DeepLinkProvider), "deepLinkProvider", parameters);
			IOfferSystem offerSystem = (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters);
			obj.Construct(eventService, bannerService, managerUI, deepLinkProvider, offerSystem);
		}
	}
}
