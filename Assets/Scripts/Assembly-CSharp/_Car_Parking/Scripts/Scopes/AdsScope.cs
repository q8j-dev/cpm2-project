using System;
using CPM.Account;
using CPM.Ads;
using CPM.Ads.Policy;
using CPM.Balance;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Scopes
{
	public static class AdsScope
	{
		private const int MinDeviceMemoryMb = 3072;

		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<NullAdsService>(Lifetime.Singleton).As<IAdsService>().As<IAdsUserIdentity>();
			builder.Register<AdsUserIdBridge>(Lifetime.Singleton).As<IAuthorizationDependant>();
			builder.Register((Func<IObjectResolver, IAdPolicyStore>)((IObjectResolver _) => new Es3AdPolicyStore(AdPolicyStorePath.Build())), Lifetime.Singleton);
			builder.Register((Func<IObjectResolver, IRawUtcTime>)((IObjectResolver c) => new ServerTimeRawUtc(c.Resolve<ServerTimeService>())), Lifetime.Singleton);
			builder.Register((Func<IObjectResolver, IUtcClock>)((IObjectResolver c) => new MonotonicUtcClock(c.Resolve<IRawUtcTime>(), c.Resolve<IAdPolicyStore>())), Lifetime.Singleton);
			builder.Register((Func<IObjectResolver, IAdPlacementCatalog>)((IObjectResolver c) => new BalanceAdPlacementCatalog(c.Resolve<IBalanceDocumentStore>())), Lifetime.Singleton);
			builder.Register<AdsDailyLimitsService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<AdOfferService>(Lifetime.Singleton).As<IAdOfferService>();
			builder.Register<AdsRewardsBackend>(Lifetime.Singleton).As<IAdsRewardsBackend>();
			builder.Register<RacingAdRewardReclaimService>(Lifetime.Singleton).As<IRacingAdRewardReclaimService>();
			builder.Register<ClientAdRewardConsumeService>(Lifetime.Singleton).As<IClientAdRewardConsumeService>();
		}
	}
}
