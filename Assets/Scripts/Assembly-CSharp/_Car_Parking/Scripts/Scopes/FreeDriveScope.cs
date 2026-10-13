using CPM.Database;
using CPM.Scripts.Garage;
using Car_Parking.Scripts.Racing;
using MVPFree.MapIcons;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using Racing.CameraManagement;
using Service;
using Services.Drone;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Car.Visual;
using _Car_Parking.Scripts.DiscordPresence;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drone;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Free_drive;
using _Car_Parking.Scripts.Free_drive.Scenes;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.GameActivity.Providers;
using _Car_Parking.Scripts.Multiplayer.Arena.UI;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Multiplayer.MasterMigration;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.PlayerProperty;
using _Car_Parking.Scripts.PlayerProperty.Business;
using _Car_Parking.Scripts.Police;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.ToRoad;
using _Car_Parking.Scripts.ToRoad.Handlers;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.UIElement.HUD;
using _Car_Parking.Scripts.VinylsAi;
using _Car_Parking.Scripts.WindowSystem.Mediators;

namespace _Car_Parking.Scripts.Scopes
{
	public class FreeDriveScope : RootSceneScope
	{
		[SerializeField]
		private MapIconsDataSo _mapIconsDataSo = default;

		[SerializeField]
		private MapIconPositionsDataSo _mapIconPositionsDataSo = default;

		[SerializeField]
		private TimeAttackDataSO _timeAttackDataSo = default;

		[SerializeField]
		private NamesScriptables _namesScriptables = default;

		[SerializeField]
		private MapIconsStarter _mapIconsStarter = default;

		[SerializeField]
		private WwiseFreeContainer _wwiseFreeContainer = default;

		[SerializeField]
		private WindshieldBrakeVfx _windshieldBrakeVfx = default;

		[SerializeField]
		private RpcRrService _rpcRrService = default;

		[SerializeField]
		private AnimationShop AnimationShop = default;

		[SerializeField]
		private FreeDriveDB FreeDriveDB = default;

		[SerializeField]
		private GarageController GarageController = default;

		[SerializeField]
		private CameraManager CameraManager = default;

		[SerializeField]
		private GPSNavigationCrossView GPSNavigationCrossView = default;

		[SerializeField]
		private CoreGameManager CoreGameManager = default;

		[SerializeField]
		private FreeDriveControl FreeDriveControl = default;

		[SerializeField]
		private FreeDriveTraffic FreeDriveTraffic = default;

		[SerializeField]
		private MapControl MapControl = default;

		[SerializeField]
		private ChatControl ChatControl = default;

		[SerializeField]
		private SkyboxBlender SkyboxBlender = default;

		[SerializeField]
		private CarCamerasController CarCamerasController = default;

		[SerializeField]
		private CarCameras CarCameras = default;

		[SerializeField]
		private MultiPlayer MultiPlayer = default;

		[SerializeField]
		private FreeCameraController FreeCameraController = default;

		[SerializeField]
		private SceneControl SceneControl = default;

		[SerializeField]
		private LevelScript LevelScript = default;

		[SerializeField]
		private Camera MainCamera = default;

		[SerializeField]
		private ObiSolverProvider ObiSolverProvider = default;

		[SerializeField]
		private TriggerCore TriggerCore = default;

		[SerializeField]
		private Sort2 Sort2 = default;

		[SerializeField]
		private MapCamera MapCamera = default;

		[SerializeField]
		private MoveElements MoveElements = default;

		[SerializeField]
		private ParkingMenuUI ParkingMenuUI = default;

		[SerializeField]
		private HudColorChanger HudColorChanger = default;

		[SerializeField]
		private PhoneController PhoneController = default;

		[SerializeField]
		private SceneCollidersContainer SceneCollidersContainer = default;

		[SerializeField]
		private CollectiblePoolContainer CollectiblePoolContainer = default;

		[SerializeField]
		private SpawnPointsContainer SpawnPointsContainer = default;

		[SerializeField]
		private UGUISteeringWheel UGUISteeringWheel = default;

		[SerializeField]
		private TimeBannersManager TimeBannersManager = default;

		[SerializeField]
		private MenuLevelParent MenuLevelParent = default;

		[SerializeField]
		private QuickCarSettings QuickCarSettings = default;

		[SerializeField]
		private ManagerUI ManagerUi = default;

		[SerializeField]
		private UIReferencesMediator UIReferencesMediator = default;

		[SerializeField]
		private ParkTronicControll ParkTronicControll = default;

		protected override void Awake()
		{
			base.Awake();
		}

		private void Start()
		{
			CommonScopeUtility.Start(base.Container);
		}

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterEntryPoint<DiscordFreeDrivePresence>();
			CommonScopeUtility.Initialize(builder);
			GameStatesScopeUtility.Initialize(builder);
			VContainerRpcRrBinder.Initialize(builder, _rpcRrService);
			VContainerTradeServiceBinder.Initialize(builder);
			ActivitiesScopeUtility.Initialize(builder);
			builder.Register<FreeGarage>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<DroneProvider>(Lifetime.Singleton);
			builder.Register<PlayerVinylCensorChecker>(Lifetime.Singleton);
			builder.Register<MapIconsModel>(Lifetime.Singleton);
			builder.Register<FreeDrivePlayerPropertyCarSitListener>(Lifetime.Singleton);
			builder.RegisterInstance(_mapIconsDataSo);
			builder.RegisterInstance(_mapIconPositionsDataSo);
			builder.RegisterComponent(_mapIconsStarter);
			builder.RegisterInstance(_wwiseFreeContainer);
			builder.RegisterEntryPoint<FreeTimeAttackController>().AsSelf();
			builder.RegisterEntryPoint<TimedActivityZoneController>().AsSelf();
			builder.Register<PropertyTriggerService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<OpenWorldToRoadHandler>(Lifetime.Singleton).As<IToRoadHandler>();
			builder.Register<GarageToRoadHandler>(Lifetime.Singleton).As<IToRoadHandler>();
			builder.Register<ToRoadRouter>(Lifetime.Singleton).AsSelf();
			builder.RegisterInstance(_timeAttackDataSo);
			builder.Register<TimeAttackActivityProvider>(Lifetime.Singleton).As<IGameActivityProvider>();
			builder.RegisterInstance(_namesScriptables);
			builder.Register<SellExchangeFreeService>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<MoneySendService>(Lifetime.Singleton);
			builder.Register<CarSellTriggersCollection>(Lifetime.Singleton);
			builder.RegisterEntryPoint<CarSellTriggerExitHandler>().AsSelf();
			builder.Register<BusinessProfitControl>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<AddFriendService>(Lifetime.Singleton);
			builder.RegisterEntryPoint<ChatExtraDataController>().AsSelf();
			builder.Register<WindshieldBrakeService>(Lifetime.Singleton);
			builder.RegisterInstance(new WindshieldBrakeVfxPool(_windshieldBrakeVfx));
			builder.Register<UiNotificationsManager>(Lifetime.Singleton);
			builder.RegisterComponentOnNewGameObject<MasterClientPingMigrationService>(Lifetime.Singleton, "MasterClientPingMigrationService");
			builder.RegisterComponent(ObiSolverProvider);
			builder.RegisterComponent(TriggerCore);
			builder.RegisterComponent(AnimationShop);
			builder.RegisterComponent(FreeDriveDB);
			builder.RegisterComponent(GarageController);
			builder.RegisterComponent(CameraManager);
			builder.RegisterInstance(GPSNavigationCrossView);
			builder.RegisterComponent(CoreGameManager);
			builder.RegisterComponent(FreeDriveControl);
			builder.RegisterInstance(FreeDriveTraffic);
			builder.RegisterComponent(MapControl);
			builder.RegisterComponent(ChatControl);
			builder.RegisterInstance(SkyboxBlender);
			builder.RegisterComponent(CarCamerasController);
			builder.RegisterComponent(CarCameras);
			builder.RegisterComponent(MultiPlayer);
			builder.RegisterInstance(FreeCameraController);
			builder.RegisterComponent(SceneControl);
			builder.RegisterComponent(LevelScript);
			builder.RegisterInstance(MainCamera);
			builder.RegisterInstance(Sort2);
			builder.RegisterComponent(MapCamera);
			builder.RegisterInstance(MoveElements);
			builder.RegisterComponent(ParkingMenuUI);
			builder.RegisterInstance(HudColorChanger);
			builder.RegisterInstance(PhoneController);
			builder.RegisterInstance(SceneCollidersContainer);
			builder.RegisterComponent(CollectiblePoolContainer);
			builder.RegisterInstance(SpawnPointsContainer);
			builder.RegisterInstance(UGUISteeringWheel);
			builder.RegisterComponent(TimeBannersManager);
			builder.RegisterComponent(MenuLevelParent);
			builder.RegisterComponent(QuickCarSettings);
			builder.RegisterComponent(ManagerUi);
			builder.RegisterInstance(UIReferencesMediator);
			builder.RegisterComponent(ParkTronicControll);
			builder.Register<PoliceButtonsControllerRegistry>(Lifetime.Singleton);
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<MapIconsModel>();
				c.Resolve<FreeDrivePlayerPropertyCarSitListener>();
				c.Resolve<ISellExchangeFreeService>();
				c.Resolve<UiNotificationsManager>();
				c.Resolve<MasterClientPingMigrationService>();
			});
			builder.RegisterEntryPoint<FreeDriveStartup>().AsSelf();
			builder.RegisterEntryPoint<ArenaLobbyWindowWatcher>().AsSelf();
		}
	}
}
