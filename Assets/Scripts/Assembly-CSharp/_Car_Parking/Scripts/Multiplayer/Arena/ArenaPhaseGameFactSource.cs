using System;
using VContainer.Unity;
using _Car_Parking.Scripts.GameFacts.Base;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaPhaseGameFactSource : IStartable, IDisposable
	{
		private readonly IGameFactManager _manager;

		private readonly ArenaFlowController _arena;

		private readonly ArenaModeRegistry _registry;

		public ArenaPhaseGameFactSource(IGameFactManager manager, ArenaFlowController arena, ArenaModeRegistry registry)
		{
			_manager = manager;
			_arena = arena;
			_registry = registry;
		}

		public void Start()
		{
			_arena.OnPhaseChanged += OnPhaseChanged;
			OnPhaseChanged(_arena.Phase);
		}

		public void Dispose()
		{
			_arena.OnPhaseChanged -= OnPhaseChanged;
			_manager.Release(this);
		}

		private void OnPhaseChanged(ArenaPhase phase)
		{
			bool flag = ArenaPhaseFacts.IsActive(phase);
			_manager.Set(GameFact.ArenaGatheringPlayers, this, ArenaPhaseFacts.IsGatheringPlayers(phase));
			_manager.Set(GameFact.ArenaPlaying, this, ArenaPhaseFacts.IsPlaying(phase));
			_manager.Set(GameFact.ArenaActive, this, flag);
			_manager.Set(GameFact.ArenaActiveModeObstacleRace, this, flag && _arena.CurrentMode == ArenaModeType.ObstacleRace);
			_manager.Set(GameFact.ArenaWaitingLocationRacing, this, flag && _arena.CurrentWaitingLocation == ArenaLocation.Racing);
			_manager.Set(GameFact.ArenaWaitingLocationTropicalIsland, this, flag && _arena.CurrentWaitingLocation == ArenaLocation.TropicalIsland);
			_manager.Set(GameFact.Carless, this, flag && IsCarless(_arena.CurrentMode));
		}

		private static bool IsCarless(ArenaModeType mode)
		{
			return mode == ArenaModeType.ObstacleRace;
		}
	}
}
