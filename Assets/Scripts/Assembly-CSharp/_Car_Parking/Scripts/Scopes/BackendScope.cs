using CPM.Account;
using CPM.Fallout;
using CPM.RacingRecords;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Backend.Session;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Multiplayer.TradeTransaction;
using _Car_Parking.Scripts.PlayerProfile;

namespace _Car_Parking.Scripts.Scopes
{
	public static class BackendScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<SessionBackend>(Lifetime.Singleton);
			builder.Register<ProgressBackend>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<PlayerProfileBackend>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<PlayerProfileCache>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<PlayerProfileService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				IPlayerProfileService service = c.Resolve<IPlayerProfileService>();
				PoseOwnership.Initialize(service);
				AnimationOwnership.Initialize(service);
			});
			builder.Register<RacingRecordsBackend>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register((IObjectResolver c) => new RacingRecordsCache(c.Resolve<IRacingRecordsBackend>(), "race", 250, 120.0, () => Time.realtimeSinceStartupAsDouble), Lifetime.Singleton);
			builder.Register<FalloutBackend>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register((IObjectResolver c) => new FalloutLimitsCache(c.Resolve<IFalloutBackend>(), () => Time.realtimeSinceStartupAsDouble), Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<IDeleterUserDataGetter>().Add(c.Resolve<FalloutLimitsCache>());
			});
			builder.Register<FalloutRaceSession>(Lifetime.Singleton).AsSelf();
			builder.Register<TradeTransactionBackend>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<CarLegitService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterInstance(new BackendClient.Config
			{
				HttpBaseUrl = "https://cpm-2.ogames.kz/api/",
				DefaultHttpTimeoutInSeconds = 60
			});
			builder.Register<BackendClient>(Lifetime.Singleton);
			builder.Register<UnityHttpClient>(Lifetime.Singleton).As<IHttpClient>();
			builder.Register<CheckBackendService>(Lifetime.Singleton);
		}
	}
}
