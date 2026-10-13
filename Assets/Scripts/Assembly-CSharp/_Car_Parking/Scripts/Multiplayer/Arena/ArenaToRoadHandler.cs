using UnityEngine;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.ToRoad;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaToRoadHandler : IToRoadHandler
	{
		private readonly ArenaFlowController _arenaFlow;

		private readonly ArenaModeRegistry _modeRegistry;

		private readonly ArenaPointsContainer _pointsContainer;

		private readonly IGameFactManager _gameFactManager;

		public ToRoadPriority Priority => ToRoadPriority.GameMode;

		public ArenaToRoadHandler(ArenaFlowController arenaFlow, ArenaModeRegistry modeRegistry, ArenaPointsContainer pointsContainer, IGameFactManager gameFactManager)
		{
			_arenaFlow = arenaFlow;
			_modeRegistry = modeRegistry;
			_pointsContainer = pointsContainer;
			_gameFactManager = gameFactManager;
		}

		public bool TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			result = ToRoadResult.NotHandled();
			if (_gameFactManager.IsTrue(GameFact.ArenaGatheringPlayers))
			{
				return TryResolveSpawnPoint(in request, out result);
			}
			if (!_gameFactManager.IsTrue(GameFact.ArenaPlaying))
			{
				return false;
			}
			if (_modeRegistry.TryGet(_arenaFlow.CurrentMode, out var controller) && controller is IArenaToRoadPolicy arenaToRoadPolicy && arenaToRoadPolicy.TryResolveToRoad(in request, out result))
			{
				return true;
			}
			result = ToRoadResult.Handled();
			return true;
		}

		private bool TryResolveSpawnPoint(in ToRoadRequest request, out ToRoadResult result)
		{
			result = ToRoadResult.NotHandled();
			if (!_pointsContainer.TryGet(_arenaFlow.CurrentWaitingLocation, out var config) || !config)
			{
				return false;
			}
			GameObject gameObject = (request.MovesPerson ? request.Person : request.Car);
			if (!gameObject)
			{
				return false;
			}
			Vector3 vector = gameObject.transform.position - V.GlobalOffset;
			if (!ArenaRespawnPoints.TrySelectNearest(config.Points, vector, out var result2))
			{
				return false;
			}
			result = ToRoadResult.Place(result2.Position + V.GlobalOffset, result2.Rotation, loadEnvironment: false);
			return true;
		}

		bool IToRoadHandler.TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			return TryResolve(in request, out result);
		}
	}
}
