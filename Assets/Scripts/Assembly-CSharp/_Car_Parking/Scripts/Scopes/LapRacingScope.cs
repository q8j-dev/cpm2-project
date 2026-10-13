using CPM.Ghost;
using CPM.Ghost.Core;
using RacingScripts.RaceControllers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.DiscordPresence;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Analytics;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost;
using _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard;
using _Car_Parking.Scripts.Racing_Scripts.RallyCrowd;
using _Car_Parking.Scripts.Racing_Scripts.RallyProps;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WindowSystem.Mediators;

namespace _Car_Parking.Scripts.Scopes
{
	public class LapRacingScope : LifetimeScope
	{
		private const string CircuitRacingConfigPath = "Configs/RaceConfig/Circuit/CircuitRacingConfig";

		private const string RallyCarsConfigPath = "Configs/RaceConfig/Rally/RallyCarsDatasSO";

		private const string RallyNPCCatalogPath = "Configs/RallyConfigs/NpcCatalogSO";

		private const string RallyPropCatalogPath = "Configs/RallyConfigs/RallyPropCatalogSO";

		[SerializeField]
		private ManagerUI ManagerUI;

		[SerializeField]
		private CoreGameManager CoreGameManager;

		[SerializeField]
		private PersonCameraControll PersonCameraControll;

		[SerializeField]
		private CarOutInControl CarOutInControl;

		[SerializeField]
		private Camera Camera;

		[SerializeField]
		private RacingCameraController RacingCameraController;

		[SerializeField]
		private QuickCarSettings QuickCarSettings;

		[SerializeField]
		private MenuLevelParent MenuLevelParent;

		[SerializeField]
		private LevelScript LevelScript;

		[SerializeField]
		private RaceModeController RacingModeControl;

		[SerializeField]
		private UIReferencesMediator UIReferencesMediator;

		[SerializeField]
		private LapRaceTrafficLightUI LapRaceTrafficLightUI;

		[SerializeField]
		private CircuitRacingWaitingWindow CircuitRacingWaitingWindow;

		[SerializeField]
		private RaceModeDataSO CircuitRacingConfig;

		[SerializeField]
		private RaceModeDataSO DragRacingConfig;

		[SerializeField]
		private RaceModeDataSO RallyRacingConfig;

		[SerializeField]
		private RallyCrowdControllerConfig RallyCrowdControllerConfig;

		[SerializeField]
		private RallyPropControllerConfig RallyPropControllerConfig;

		private void Start()
		{
			CommonScopeUtility.Start(base.Container);
		}

		protected override void Configure(IContainerBuilder builder)
		{
			CommonScopeUtility.Initialize(builder);
			builder.RegisterComponent(ManagerUI);
			builder.RegisterComponent(CoreGameManager);
			builder.RegisterComponent(PersonCameraControll);
			builder.RegisterComponent(CarOutInControl);
			builder.RegisterComponent(Camera);
			builder.RegisterComponent(RacingModeControl);
			builder.RegisterComponent(LapRaceTrafficLightUI);
			builder.RegisterComponent(RacingCameraController);
			builder.RegisterComponent(QuickCarSettings);
			builder.RegisterComponent(MenuLevelParent);
			builder.RegisterComponent(UIReferencesMediator);
			builder.RegisterComponent(CircuitRacingWaitingWindow);
			builder.RegisterComponent(LevelScript);
			builder.Register<IGhostLogger, WDebugGhostLogger>(Lifetime.Scoped);
			builder.RegisterInstance(Resources.Load<GhostConfig>("Configs/Ghost/GhostConfig"));
			builder.Register((IObjectResolver c) => new GhostStorage(c.Resolve<IGhostLogger>()), Lifetime.Scoped);
			builder.Register<IGhostCarFactory, GhostCarFactory>(Lifetime.Scoped);
			builder.Register<GhostSystem>(Lifetime.Scoped).AsSelf();
			switch (V.CurrentRaceModeType)
			{
			case RacingModeType.CircuitRacing:
				builder.RegisterInstance(Resources.Load<CircuitRacingConfig>("Configs/RaceConfig/Circuit/CircuitRacingConfig"));
				builder.RegisterComponent(CircuitRacingConfig);
				builder.RegisterEntryPoint<DiscordCircuitLapReporter>();
				break;
			case RacingModeType.DragRacing:
				builder.RegisterComponent(DragRacingConfig);
				builder.RegisterEntryPoint<DiscordScenePresence>().WithParameter("kind", DiscordActivityKind.DragRacing).WithParameter("reportEnvironment", true);
				break;
			case RacingModeType.Rally:
				builder.RegisterEntryPoint<DiscordScenePresence>().WithParameter("kind", DiscordActivityKind.Rally).WithParameter("reportEnvironment", true);
				builder.RegisterComponent(RallyRacingConfig);
				builder.RegisterComponent(Resources.Load<CPMCrowdNpcCatalogSO>("Configs/RallyConfigs/NpcCatalogSO"));
				builder.RegisterComponent(RallyCrowdControllerConfig);
				builder.RegisterInstance(Resources.Load<RallyCarsDatasSO>("Configs/RaceConfig/Rally/RallyCarsDatasSO"));
				builder.Register<RallySessionTracker>(Lifetime.Scoped);
				builder.RegisterEntryPoint<CPMCrowdManager>().As<ICPMCrowdManager>();
				builder.RegisterEntryPoint<RallyCrowdController>().AsSelf();
				builder.RegisterComponent(Resources.Load<RallyPropCatalogSO>("Configs/RallyConfigs/RallyPropCatalogSO"));
				builder.RegisterComponent(RallyPropControllerConfig);
				builder.RegisterEntryPoint<RallyPropManager>().As<IRallyPropManager>();
				builder.RegisterEntryPoint<RallyPropController>().AsSelf();
				builder.RegisterEntryPoint<RallyGhostController>().AsSelf();
				builder.Register<GhostRaceState>(Lifetime.Scoped).AsSelf();
				builder.Register<RallyRecordsBank>(Lifetime.Scoped).AsSelf();
				builder.Register<RallyRecordsSync>(Lifetime.Scoped).AsSelf();
				break;
			}
		}
	}
}
