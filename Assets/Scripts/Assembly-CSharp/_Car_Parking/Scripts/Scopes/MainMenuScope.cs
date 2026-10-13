using CPM.Database;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.FreeDriverService;
using CPM.Scripts.VinylDrawer;
using Cinemachine;
using Racing.CameraManagement;
using Racing.Scripts.UserColors;
using Rating.DailyTasks.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Initializer;
using _Car_Parking.Scripts.Car.Physics.AirSuspension;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.DataStorageService.MenuNumberPlates;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.DiscordPresence;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.VinylsAi;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Scopes
{
	public class MainMenuScope : LifetimeScope
	{
		private const string VisualTuningSmokeConfigPath = "Configs/VisualTuning/ColorsScopeSmokeConfig";

		[SerializeField]
		private GarageAnalyticaInitializer GarageAnalyticaInitializer;

		[SerializeField]
		private AnimationShop AnimationShop;

		[SerializeField]
		private CarSelectorByTouch CarSelectorByTouch;

		[SerializeField]
		private GarageController GarageController;

		[SerializeField]
		private LightForChangeRoom LightForChangeRoom;

		[SerializeField]
		private MenuControl MenuControl;

		[SerializeField]
		private Camera MainCameraNew;

		[SerializeField]
		private CoreGameManager CoreGameManager;

		[SerializeField]
		private SceneControl SceneControl;

		[SerializeField]
		private ExteriorTuning ExteriorTuning;

		[SerializeField]
		private MultiPlayerMenu MultiPlayerMenu;

		[SerializeField]
		private CarCameras CarCameras;

		[SerializeField]
		private CinemachineBrain CinemachineBrain;

		[SerializeField]
		private ManagerUI ManagerUI;

		[SerializeField]
		private CameraManager CameraManager;

		[SerializeField]
		private FreeDriveDB FreeDriveDB;

		[SerializeField]
		private StageRender StageRender;

		[SerializeField]
		private MenuLevelParent MenuLevelParent;

		[SerializeField]
		private VinylsEditorDependencyProvider VinylsEditorDependencyProvider;

		[SerializeField]
		private SetStaticData SetStaticData;

		[SerializeField]
		private LeaderBoard LeaderBoard;

		protected override void Awake()
		{
			base.Awake();
		}

		private void Start()
		{
			CommonScopeUtility.Start(base.Container);
			autoInjectGameObjects?.Clear();
		}

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterEntryPoint<DiscordTuningPresence>().WithParameter("idleKind", DiscordActivityKind.MainMenu);
			builder.RegisterEntryPoint<DiscordScenePresence>().WithParameter("kind", DiscordActivityKind.MainMenu).WithParameter("reportEnvironment", false);
			CommonScopeUtility.Initialize(builder);
			GameStatesScopeUtility.Initialize(builder);
			VisualTuningScopeUtility.Initialize(builder);
			ActivitiesScopeUtility.Initialize(builder);
			builder.RegisterEntryPoint<CarSlotsObserver>().AsSelf();
			builder.Register<MenuWindowsMediator>(Lifetime.Singleton).As<IMenuWindowsMediator>();
			builder.RegisterEntryPoint<_Car_Parking.Scripts.MenuUI.VisualTuning.VisualTuning>().AsSelf();
			builder.Register<SavedColorCollection>(Lifetime.Singleton);
			builder.RegisterEntryPoint<MenuPlatesHolder>().AsSelf();
			builder.RegisterEntryPoint<VisualTuningViewModel>().As<IVisualTuningViewModel>().As<IVisualTuningViewModelComplexOne>()
				.As<IVisualTuningViewModelComplexParams>()
				.As<IVisualTuningUpdateAction>();
			builder.RegisterInstance(Resources.Load<ColorsScopeSmokeConfig>("Configs/VisualTuning/ColorsScopeSmokeConfig"));
			builder.Register<MenuGarage>(Lifetime.Singleton).As<IGarageEntity>();
			builder.RegisterEntryPoint<TimerTuningService>().As<ITimerTuningService>();
			builder.Register<EngineTimerAdService>(Lifetime.Singleton).As<IEngineTimerAdService>();
			builder.RegisterEntryPoint<ClientAdRewardRecoveryService>();
			builder.RegisterEntryPoint<EngineTuningTimerService>().As<IEngineTuningTimerService>();
			builder.Register<FreeDriverService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<RegistrationController>(Lifetime.Singleton).As<IRegistrationController>();
			builder.RegisterEntryPoint<DataBase>().AsSelf();
			builder.RegisterEntryPoint<AirSuspensionSystem>().As<IAirSuspension>();
			builder.Register<UiNotificationsManager>(Lifetime.Singleton);
			builder.Register<DailyServicesInitializer>(Lifetime.Singleton).As<IDailyServicesDependant>();
			builder.Register<AutoSaveService>(Lifetime.Singleton).As<IAutoSaveService>();
			builder.Register<UITouchHelper>(Lifetime.Singleton);
			builder.Register<PlayerVinylCensorChecker>(Lifetime.Singleton);
			builder.RegisterEntryPoint<PersonClothesEventChecker>().AsSelf();
			builder.RegisterComponent(GarageAnalyticaInitializer);
			builder.RegisterComponent(AnimationShop);
			builder.RegisterComponent(CarSelectorByTouch);
			builder.RegisterComponent(GarageController);
			builder.RegisterInstance(LightForChangeRoom);
			builder.RegisterComponent(MenuControl);
			builder.RegisterInstance(MainCameraNew);
			builder.RegisterComponent(CoreGameManager);
			builder.RegisterComponent(SceneControl);
			builder.RegisterComponent(ExteriorTuning);
			builder.RegisterComponent(MultiPlayerMenu);
			builder.RegisterComponent(CarCameras);
			builder.RegisterInstance(CinemachineBrain);
			builder.RegisterComponent(ManagerUI);
			builder.RegisterComponent(CameraManager);
			builder.RegisterComponent(FreeDriveDB);
			builder.RegisterComponent(StageRender);
			builder.RegisterComponent(MenuLevelParent);
			builder.RegisterInstance(VinylsEditorDependencyProvider);
			builder.RegisterComponent(SetStaticData);
			builder.RegisterComponent(LeaderBoard);
			builder.RegisterEntryPoint<NewCarsMarkController>().AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<UiNotificationsManager>();
				c.Resolve<IAutoSaveService>();
			});
			builder.RegisterEntryPoint<MainMenuStartup>().AsSelf();
			builder.RegisterComponentInHierarchy<UiCursorSpawner>().AsSelf();
			builder.Register<PoliceButtonsControllerRegistry>(Lifetime.Singleton);
		}
	}
}
