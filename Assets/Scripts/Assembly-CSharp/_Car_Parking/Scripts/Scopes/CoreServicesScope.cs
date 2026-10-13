using System.Collections.Generic;
using CPM.Account;
using CPM.Common;
using CPM.DailyTasks;
using CPM.Database;
using CPM.Fallout;
using CPM.Rating;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using CPM.Scripts.VinylDrawer;
using Car_Parking.Scripts.Drag.CarCompositions;
using Persistence.Systems;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Garage.ForbiddenZones;
using _Car_Parking.Garage.Poses;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.BannerSystem;
using _Car_Parking.Scripts.CPMRating.AchievementsService;
using _Car_Parking.Scripts.CPMRating.DailyTasks.Scripts;
using _Car_Parking.Scripts.Car.Physics.Vfx;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.DataEntities;
using _Car_Parking.Scripts.Database.Cars;
using _Car_Parking.Scripts.DeepLink;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Licensing;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Persistence.Migration;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Rating;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.WwisePrefabs.WiseSoundScripts;

namespace _Car_Parking.Scripts.Scopes
{
	public static class CoreServicesScope
	{
		public static void Initialize(IContainerBuilder builder, VinylDrawer vinylsDrawerPrefab)
		{
			BindProviders(builder);
			BindRatingSystems(builder);
			BindOtherUtilities(builder, vinylsDrawerPrefab);
			BindPersistenceSystems(builder);
			BindLicensingServices(builder);
			BindTopLevel(builder);
			VContainerRpcCollectionBinder.Initialize(builder);
		}

		private static void BindProviders(IContainerBuilder builder)
		{
			builder.Register<OnlineCollectionsProvider>(Lifetime.Singleton);
			builder.Register<GlobalCollectionsProvider>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<GlobalCollectionsProvider>();
			});
			builder.Register<InputItemsProvider>(Lifetime.Singleton);
			builder.Register<FreeDriveProvider>(Lifetime.Singleton);
			builder.Register<RacingProvider>(Lifetime.Singleton);
			builder.Register<LevelsProvider>(Lifetime.Singleton);
			builder.Register<ServerTimeService>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<ServerTimeService>();
			});
			builder.RegisterEntryPoint<GeoGetterService>().AsSelf();
			builder.Register<GarageForbiddenZonesProvider>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<IForbiddenZonesProvider>();
			});
		}

		private static void BindRatingSystems(IContainerBuilder builder)
		{
			builder.Register<FirebaseDailyTasksFetchService>(Lifetime.Singleton);
			builder.Register<BackendDailyTasksFetchService>(Lifetime.Singleton);
			builder.Register<DailyTasksFetchServiceRouter>(Lifetime.Singleton).As<IDailyTasksFetchService>();
			builder.Register<DailyTasksManager>(Lifetime.Singleton);
			builder.Register<DailyTasksInfoContainer>(Lifetime.Singleton);
			builder.Register<AchievementPopupChannel>(Lifetime.Singleton);
			builder.Register<FirebaseRatingService>(Lifetime.Singleton);
			builder.Register<BackendRatingService>(Lifetime.Singleton);
			builder.Register<RatingServiceRouter>(Lifetime.Singleton).As<IRatingService>();
			builder.RegisterEntryPoint<RatingSender>().AsSelf();
		}

		private static void BindOtherUtilities(IContainerBuilder builder, VinylDrawer vinylsDrawerPrefab)
		{
			builder.Register<ChatFilter>(Lifetime.Singleton).As<IChatFilterUtility>();
			builder.RegisterComponentOnNewGameObject<AppStateHandler>(Lifetime.Singleton, "AppStateHandler").DontDestroyOnLoad();
			builder.RegisterComponentOnNewGameObject<CoroutineHelper>(Lifetime.Singleton, "CoroutineHelper").DontDestroyOnLoad();
			builder.Register<SoundPlayerHandler>(Lifetime.Singleton).As<ISoundPlayerHandler>();
			builder.RegisterEntryPoint<SoundBankLoadService>().AsSelf();
			builder.Register<RainSoundState>(Lifetime.Singleton);
			builder.Register<RainSound>(Lifetime.Singleton);
			builder.Register<SoundControllerExtension>(Lifetime.Singleton);
			builder.Register<DrawQueue>(Lifetime.Singleton);
			builder.RegisterComponentInNewPrefab(vinylsDrawerPrefab, Lifetime.Singleton).DontDestroyOnLoad();
			builder.Register<PhotonCarValidator>(Lifetime.Singleton).As<IPhotonCarValidator>();
			CPMGraphicsSettingsManager component = PersistentConfigPreloader.GetComponent<CPMGraphicsSettingsManager>("Graphics/GraphicsManager");
			builder.RegisterComponentInNewPrefab(component, Lifetime.Singleton).DontDestroyOnLoad().AsImplementedInterfaces()
				.AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<CPMGraphicsSettingsManager>();
			});
			ScreenFader component2 = PersistentConfigPreloader.GetComponent<ScreenFader>("Graphics/ScreenFader");
			builder.RegisterComponentInNewPrefab(component2, Lifetime.Singleton).DontDestroyOnLoad();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<ScreenFader>();
			});
			MirrorComposition component3 = PersistentConfigPreloader.GetComponent<MirrorComposition>("Graphics/MirrorComposition");
			builder.RegisterComponentInNewPrefab(component3, Lifetime.Singleton).DontDestroyOnLoad();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<MirrorComposition>();
			});
			builder.RegisterComponentOnNewGameObject<MoneyChangedSound>(Lifetime.Singleton, "MoneyChangedSound").DontDestroyOnLoad();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<MoneyChangedSound>();
			});
			builder.Register<PlayerPrefsTimer>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<AllPrefsSaversGetter>(Lifetime.Singleton).As<IPrefsSaversGetter>();
			builder.Register<DeleterUserDataGetter>(Lifetime.Singleton).As<IDeleterUserDataGetter>();
			builder.RegisterEntryPoint<PoliceSystem>().As<IPoliceSystem>();
			builder.Register<PlayerStatusSystem>(Lifetime.Singleton).As<IPlayerStatusSystem>();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<IPlayerStatusSystem>();
			});
			builder.Register<InterestGroupManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterEntryPoint<OfflineModeSystem>().As<IOfflineModeSystem>();
			builder.Register<CheckCarDataBase>(Lifetime.Singleton).As<ICheckCarDataBase>();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<ICheckCarDataBase>();
			});
			builder.RegisterEntryPoint<RamHelper>().AsSelf();
			WheelSurfaceVfxLibrary wheelSurfaceVfxLibrary = PersistentConfigPreloader.Get<WheelSurfaceVfxLibrary>("Configs/WheelSurfaceVfxLibrary");
			if ((bool)wheelSurfaceVfxLibrary)
			{
				builder.RegisterInstance(wheelSurfaceVfxLibrary);
			}
			builder.RegisterEntryPoint<WheelSurfaceVfxService>().AsSelf();
		}

		private static void BindPersistenceSystems(IContainerBuilder builder)
		{
			builder.Register<MigrationManager>(Lifetime.Singleton).As<IMigrationManager>();
			builder.Register<RuntimePlayerDataHandler>(Lifetime.Singleton);
			builder.Register<ExternalSaveableDataAdapter>(Lifetime.Singleton);
			builder.Register<DataVersionsService>(Lifetime.Singleton).As<IDataVersionsService>();
			builder.Register<LocalDataMigrationManager>(Lifetime.Singleton);
			builder.RegisterEntryPoint<FirebaseCarsService>().As<IDatabaseCarsService>();
			builder.Register<DatabaseOutputHandler>(Lifetime.Singleton).As<IDatabaseOutputHandler>();
			builder.Register<VContainerSceneLoader>(Lifetime.Singleton).As<ISceneLoader>();
			builder.RegisterEntryPoint<GarageSelectorService>().As<IGarageSelectorService>().AsSelf();
			builder.RegisterEntryPoint<GarageBuyService>().As<IGarageBuyService>().As<IDeleteUserData>()
				.AsSelf();
			builder.Register<EventRestrictionService>(Lifetime.Singleton);
			builder.RegisterEntryPoint<EventService>().As<IEventService>();
			builder.Register<ObstacleRaceFinishHandler>(Lifetime.Singleton).As<IFalloutFinishHandler>();
			builder.Register<GameFactManager>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterEntryPoint<EventGameFactSource>().WithParameter((IReadOnlyDictionary<EventType, GameFact>)new Dictionary<EventType, GameFact> { 
			{
				EventType.NewYearPresents,
				GameFact.EventNewYearPresents
			} });
			builder.Register<BannerService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterEntryPoint<GarageCarPoseService>().As<IGarageCarPoseService>().AsSelf();
			builder.RegisterEntryPoint<EquipmentNotificationService>().As<INotificationService>();
			builder.Register<EmoteFavoritesService>(Lifetime.Singleton);
			builder.Register<EmoteNoticeTracker>(Lifetime.Singleton);
			builder.Register<PresetGrantSession>(Lifetime.Singleton);
			builder.Register<EngineSaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.Register<PlatesSaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.Register<PartsInventorySaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.Register<PlayerStorageSaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.Register<PlayerAnonStorageSaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.Register<WalletSaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.Register<FriendsSaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.Register<SlotsOnDatabaseSaveLoadService>(Lifetime.Singleton).AsSelf().As<IReInitPreMainMenu>();
			builder.RegisterEntryPoint<ParentalControlUtility>().AsSelf();
			builder.RegisterEntryPoint<DeepLinkProvider>().AsSelf();
		}

		private static void BindLicensingServices(IContainerBuilder builder)
		{
			builder.RegisterEntryPoint<LicenseCheckerService>();
		}

		private static void BindTopLevel(IContainerBuilder builder)
		{
			builder.Register<RideableMeshesGraph>(Lifetime.Singleton);
			builder.Register<SceneLoadingTimer>(Lifetime.Singleton);
			builder.Register<ExitToMainMenuService>(Lifetime.Singleton);
			builder.Register<VfxPoolProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<VfxPoolProvider>();
			});
			builder.Register<BusinessService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<AchievementsService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterEntryPoint<AssetBundleManager>().AsSelf();
			builder.RegisterEntryPoint<GameDataRepository>().As<IGameDataRepository>().AsSelf();
			builder.RegisterEntryPoint<GuestAccountCreator>().As<IGuestAccountCreator>().AsSelf();
			builder.Register<BootStartData>(Lifetime.Singleton);
		}
	}
}
