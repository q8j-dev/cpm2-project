using System.Collections.Generic;
using CPM.Connection;
using VContainer;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.GrantService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	internal class OffersPurchaseHandlerGeneratedInjector : IInjector
	{
		public object CreateInstance(IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
			OffersPurchaseHandler offersPurchaseHandler = new OffersPurchaseHandler((IConnectionManager)resolver.ResolveOrParameter(typeof(IConnectionManager), "connectionManager", parameters), (IOfferSystem)resolver.ResolveOrParameter(typeof(IOfferSystem), "offerSystem", parameters), (CommonProvider)resolver.ResolveOrParameter(typeof(CommonProvider), "commonProvider", parameters), (IWalletService)resolver.ResolveOrParameter(typeof(IWalletService), "walletService", parameters), (RewardGranter)resolver.ResolveOrParameter(typeof(RewardGranter), "rewardGranter", parameters), (AnimationShopOpenRequest)resolver.ResolveOrParameter(typeof(AnimationShopOpenRequest), "animationShopOpenRequest", parameters), (EmoteLockerOpenRequest)resolver.ResolveOrParameter(typeof(EmoteLockerOpenRequest), "emoteLockerOpenRequest", parameters));
			Inject(offersPurchaseHandler, resolver, parameters);
			return offersPurchaseHandler;
		}

		public void Inject(object instance, IObjectResolver resolver, IReadOnlyList<IInjectParameter> parameters)
		{
		}
	}
}
