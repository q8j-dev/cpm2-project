using CPM.CarVisualModel;
using CPM.Database;
using CPM.Rating;
using Flags;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Garage.GarageSellService;
using _Car_Parking.Scripts.Analytics;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarEligibilityService;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Car.CruiseControl;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.DataTypes;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.View.ButtonActions;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.Scopes
{
	public static class CommonScopeUtility
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<PlayerDatas.VinylsDataUtility>(Lifetime.Singleton);
			builder.Register<PlayerDatas.CarLoadingUtility>(Lifetime.Singleton);
			builder.Register<PrefabFactory>(Lifetime.Singleton).As<IPrefabFactory>();
			builder.Register<SteamStorePage>(Lifetime.Singleton);
			builder.Register<CloseBannerAction>(Lifetime.Singleton).As<IBannerButtonAction>();
			builder.Register<OpenStoreAction>(Lifetime.Singleton).As<IBannerButtonAction>();
			builder.Register<ClaimRewardAction>(Lifetime.Singleton).As<IBannerButtonAction>();
			builder.Register<CarShopVisualService>(Lifetime.Singleton);
			builder.Register<CarShopPurchaseService>(Lifetime.Singleton);
			builder.RegisterEntryPoint<CarAvailabilityService>().AsSelf();
			builder.RegisterEntryPoint<AchievementPopupPresenter>();
			builder.Register<RacingRewardAdService>(Lifetime.Singleton).As<IRacingRewardAdService>();
			builder.Register<LevelRatingSender>(Lifetime.Singleton);
			builder.Register<CruiseControl>(Lifetime.Singleton);
			builder.Register<PrefabFactoryLinkage>(Lifetime.Singleton);
			builder.RegisterInstance(Resources.Load<FlagsDataSO>("Configs/VisualTuning/FlagsData"));
			builder.Register<ColorsService>(Lifetime.Singleton);
			builder.Register<CarFactoryService>(Lifetime.Singleton);
			builder.RegisterEntryPoint<CarInfoService>().As<ICarInfoService>();
			builder.Register<CarVisualModelBuilder>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<VehicleSlotManager>(Lifetime.Singleton).As<ISlotService>();
			builder.Register<GaragePropertyAnalytic>(Lifetime.Singleton);
			builder.Register<GarageSellService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<VehicleSlotPaymentService>(Lifetime.Singleton);
			builder.RegisterEntryPoint<PlayerStorageService>().As<IPlayerStorageService>().As<IWheelsDataService>()
				.As<ICharacterEquipmentDataService>()
				.As<IPaintDataService>()
				.As<IDeleteUserData>();
			builder.Register<CharactersService>(Lifetime.Singleton);
			builder.Register<ProgressContainer>(Lifetime.Singleton);
			builder.RegisterComponentOnNewGameObject<LockCarButtonController>(Lifetime.Singleton, "LockCarButtonController");
			builder.Register<CarEligibilityService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.Register<PhotoModeObserver>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<ISlotService>();
				c.Resolve<ICarEligibilityService>();
				c.Resolve<LockCarButtonController>();
			});
		}

		public static void Start(IObjectResolver objectResolver)
		{
			objectResolver.Resolve<PrefabFactoryLinkage>();
			CommonProvider commonProvider = objectResolver.Resolve<CommonProvider>();
			commonProvider.SlotService = objectResolver.Resolve<ISlotService>();
			commonProvider.CharactersService = objectResolver.Resolve<CharactersService>();
			commonProvider.PrefabFactory = objectResolver.Resolve<IPrefabFactory>();
			AnalyticsDependencies.Unregister(commonProvider.SlotService);
			AnalyticsDependencies.RegisterDependence(commonProvider.SlotService);
		}
	}
}
