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
		private MapIconsDataSo _mapIconsDataSo;

		[SerializeField]
		private MapIconPositionsDataSo _mapIconPositionsDataSo;

		[SerializeField]
		private TimeAttackDataSO _timeAttackDataSo;

		[SerializeField]
		private NamesScriptables _namesScriptables;

		[SerializeField]
		private MapIconsStarter _mapIconsStarter;

		[SerializeField]
		private WwiseFreeContainer _wwiseFreeContainer;

		[SerializeField]
		private WindshieldBrakeVfx _windshieldBrakeVfx;

		[SerializeField]
		private RpcRrService _rpcRrService;

		[SerializeField]
		private AnimationShop AnimationShop;

		[SerializeField]
		private FreeDriveDB FreeDriveDB;

		[SerializeField]
		private GarageController GarageController;

		[SerializeField]
		private CameraManager CameraManager;

		[SerializeField]
		private GPSNavigationCrossView GPSNavigationCrossView;

		[SerializeField]
		private CoreGameManager CoreGameManager;

		[SerializeField]
		private FreeDriveControl FreeDriveControl;

		[SerializeField]
		private FreeDriveTraffic FreeDriveTraffic;

		[SerializeField]
		private MapControl MapControl;

		[SerializeField]
		private ChatControl ChatControl;

		[SerializeField]
		private SkyboxBlender SkyboxBlender;

		[SerializeField]
		private CarCamerasController CarCamerasController;

		[SerializeField]
		private CarCameras CarCameras;

		[SerializeField]
		private MultiPlayer MultiPlayer;

		[SerializeField]
		private FreeCameraController FreeCameraController;

		[SerializeField]
		private SceneControl SceneControl;

		[SerializeField]
		private LevelScript LevelScript;

		[SerializeField]
		private Camera MainCamera;

		[SerializeField]
		private ObiSolverProvider ObiSolverProvider;

		[SerializeField]
		private TriggerCore TriggerCore;

		[SerializeField]
		private Sort2 Sort2;

		[SerializeField]
		private MapCamera MapCamera;

		[SerializeField]
		private MoveElements MoveElements;

		[SerializeField]
		private ParkingMenuUI ParkingMenuUI;

		[SerializeField]
		private HudColorChanger HudColorChanger;

		[SerializeField]
		private PhoneController PhoneController;

		[SerializeField]
		private SceneCollidersContainer SceneCollidersContainer;

		[SerializeField]
		private CollectiblePoolContainer CollectiblePoolContainer;

		[SerializeField]
		private SpawnPointsContainer SpawnPointsContainer;

		[SerializeField]
		private UGUISteeringWheel UGUISteeringWheel;

		[SerializeField]
		private TimeBannersManager TimeBannersManager;

		[SerializeField]
		private MenuLevelParent MenuLevelParent;

		[SerializeField]
		private QuickCarSettings QuickCarSettings;

		[SerializeField]
		private ManagerUI ManagerUi;

		[SerializeField]
		private UIReferencesMediator UIReferencesMediator;

		[SerializeField]
		private ParkTronicControll ParkTronicControll;

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
