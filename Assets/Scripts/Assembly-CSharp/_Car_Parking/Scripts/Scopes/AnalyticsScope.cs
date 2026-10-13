using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Initializer;
using _Car_Parking.Scripts.Analytics.Root;

namespace _Car_Parking.Scripts.Scopes
{
	public static class AnalyticsScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<LevelsAnalytic>(Lifetime.Singleton);
			builder.Register<RacingAnalytic>(Lifetime.Singleton);
			builder.Register<VirtualPurchaseAnalytic>(Lifetime.Singleton);
			builder.Register<RealPurchaseAnalytic>(Lifetime.Singleton);
			builder.RegisterEntryPoint<OpenWorldAnalytic>().AsSelf();
			builder.Register<ProfileAnalytic>(Lifetime.Singleton);
			builder.Register<CharacterCreationAnalytic>(Lifetime.Singleton);
			builder.Register<UserPreferenceAnalytic>(Lifetime.Singleton);
			builder.Register<TempAbtestAnalytic>(Lifetime.Singleton);
			builder.Register<MigrationAnalytic>(Lifetime.Singleton);
			builder.Register<TradeAnalytic>(Lifetime.Singleton);
			builder.Register<EventsAnalytic>(Lifetime.Singleton);
			builder.Register<RallyAnalytic>(Lifetime.Singleton);
			builder.Register<DailyRewardAnalytic>(Lifetime.Singleton);
			builder.Register<DailyTaskAnalytic>(Lifetime.Singleton);
			builder.Register<AccountLinkAnalytic>(Lifetime.Singleton);
			builder.RegisterEntryPoint<BackendErrorAnalytic>().AsSelf();
			builder.RegisterEntryPoint<MatchmakingLobbyAnalytic>().AsSelf();
			builder.RegisterEntryPoint<ObstacleRaceAnalytic>().AsSelf();
			builder.RegisterEntryPoint<AchievementAnalytic>().AsSelf();
			builder.RegisterEntryPoint<AdsAnalytic>().AsSelf();
			builder.RegisterEntryPoint<ExceptionErrorAnalytic>().AsSelf();
			builder.RegisterEntryPoint<IntegrityAnalytic>().AsSelf();
			builder.Register<AnalyticEventFactory>(Lifetime.Singleton);
			builder.Register<CPMAnalytics>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterEntryPoint<AnalyticsInitializer>();
			builder.RegisterEntryPoint<AnalyticsBackgroundFlusher>();
			builder.RegisterEntryPoint<CheckHashAnalyticsSender>().AsSelf();
		}
	}
}
