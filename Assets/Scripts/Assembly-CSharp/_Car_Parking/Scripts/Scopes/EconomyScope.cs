using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.CDN;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DailyRewards;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GrantService;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Offers.Reward;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Scopes
{
	public static class EconomyScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<DailyRewardsServerFirebase>(Lifetime.Singleton).As<IDailyRewardsServer>();
			builder.RegisterEntryPoint<OfferPriceUpdater>();
			builder.RegisterEntryPoint<OfferSystemGame>().As<IOfferSystem>();
			builder.RegisterEntryPoint<_Car_Parking.Scripts.PurchaseSystem.PurchaseSystem>().As<IPurchaseSystem>();
			builder.Register<PurchaseSystemPurchaseProductsAdder>(Lifetime.Transient);
			builder.Register<CoinPurchaseHandler>(Lifetime.Singleton).As<IPurchaseHandler>();
			builder.Register<GarageOldPurchaseHandler>(Lifetime.Singleton).As<IPurchaseHandler>();
			builder.Register<GaragePurchaseHandler>(Lifetime.Singleton).As<IPurchaseHandler>();
			builder.Register<MoneyPurchaseHandler>(Lifetime.Singleton).As<IPurchaseHandler>();
			builder.Register<NewCarsPurchaseHandler>(Lifetime.Singleton).As<IPurchaseHandler>();
			builder.Register<OffersPurchaseHandler>(Lifetime.Singleton).As<IPurchaseHandler>();
			builder.Register<CharactersSuitInventory>(Lifetime.Singleton).As<ISuitInventory>();
			builder.Register<GrantHandlerCar>(Lifetime.Singleton).As<IGrantHandler>();
			builder.Register<GrantHandlerSuit>(Lifetime.Singleton).As<IGrantHandler>();
			builder.Register<GrantHandlerSuitpack>(Lifetime.Singleton).As<IGrantHandler>();
			builder.Register<GrantHandlerIKPoses>(Lifetime.Singleton).As<IGrantHandler>();
			builder.Register<GrantHandlerAnimations>(Lifetime.Singleton).As<IGrantHandler>();
			builder.Register<RewardGranter>(Lifetime.Singleton);
			builder.RegisterEntryPoint<BundleOfferCatalog>().AsSelf();
			builder.Register<AnimationShopOpenRequest>(Lifetime.Singleton);
			builder.Register<EmoteLockerOpenRequest>(Lifetime.Singleton);
			builder.RegisterInstance(PersistentConfigPreloader.Get<SuitPacksConfig>("Configs/SuitPacksConfig")).As<ISuitPacksConfig>();
			builder.RegisterInstance(PersistentConfigPreloader.Get<EmoteOfferBannersConfig>("Configs/EmoteOfferBannersConfig")).As<IEmoteOfferBannersConfig>();
			builder.Register<OfferCarVisualDataDownloader>(Lifetime.Singleton).As<IOfferCarVisualDataDownloader>();
			builder.RegisterEntryPoint<CDNDownloader>().As<ICDNDownloader>();
			builder.RegisterEntryPoint<EventCdnPrefetcher>();
			builder.Register<CarPriceHelperV2>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<CarPriceHelperV2>();
			});
		}
	}
}
