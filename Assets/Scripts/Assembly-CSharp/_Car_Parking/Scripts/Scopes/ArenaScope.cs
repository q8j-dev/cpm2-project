using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.ToRoad;

namespace _Car_Parking.Scripts.Scopes
{
	public static class ArenaScope
	{
		public static void Initialize(IContainerBuilder builder)
		{
			builder.Register<IArenaNetwork, PhotonArenaNetwork>(Lifetime.Singleton).AsSelf();
			ArenaConfig[] array = Resources.LoadAll<ArenaConfig>("Configs/Arena");
			foreach (ArenaConfig instance in array)
			{
				builder.RegisterInstance(instance).As<ArenaConfig>();
			}
			builder.Register((IObjectResolver _) => new ArenaPointsContainer(Resources.LoadAll<ArenaPointsConfig>("Configs/Arena")), Lifetime.Singleton).AsSelf();
			builder.Register<ArenaToRoadHandler>(Lifetime.Singleton).As<IToRoadHandler>();
			builder.Register<ObstacleRaceMusic>(Lifetime.Singleton).AsSelf();
			builder.Register<ObstacleRaceRunTracker>(Lifetime.Singleton).AsSelf();
			builder.Register<ObstacleRaceArenaMode>(Lifetime.Singleton).As<IArenaModeController>();
			builder.Register<ArenaModeRegistry>(Lifetime.Singleton).AsSelf();
			builder.Register<ArenaForcedOutfitProvider>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
			builder.Register<ArenaSceneNavigator>(Lifetime.Singleton).AsSelf();
			builder.Register<ArenaFlowController>(Lifetime.Singleton).AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<ArenaFlowController>();
			});
			builder.Register<LobbyRoster>(Lifetime.Singleton).AsSelf();
			builder.RegisterBuildCallback((IObjectResolver c) =>
			{
				c.Resolve<LobbyRoster>();
			});
			builder.RegisterEntryPoint<ArenaPhaseGameFactSource>();
		}
	}
}
