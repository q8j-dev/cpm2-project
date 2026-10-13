using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRacePodiumMirror
	{
		private readonly IArenaContext _ctx;

		private readonly OnlineCollectionsProvider _online;

		private readonly PodiumOnline _podium;

		private readonly Func<SurvivalEliminationBoard> _readBoard;

		private readonly int _myActor;

		public ObstacleRacePodiumMirror(IArenaContext ctx, OnlineCollectionsProvider online, PodiumOnline podium, Func<SurvivalEliminationBoard> readBoard, int myActor)
		{
			_ctx = ctx;
			_online = online;
			_podium = podium;
			_readBoard = readBoard;
			_myActor = myActor;
		}

		public void OnStateChanged()
		{
			if (!_podium || _online == null || _ctx == null)
			{
				return;
			}
			SurvivalEliminationBoard survivalEliminationBoard = _readBoard?.Invoke();
			IReadOnlyList<int> participantActorNumbers = _ctx.ParticipantActorNumbers;
			if (survivalEliminationBoard == null || participantActorNumbers == null)
			{
				return;
			}
			IReadOnlyList<int> eliminationOrder = survivalEliminationBoard.EliminationOrder;
			for (int i = 0; i < eliminationOrder.Count; i++)
			{
				int num = eliminationOrder[i];
				if (num == _myActor)
				{
					continue;
				}
				MultiPersonControl playerByOwnerActorNr = _online.GetPlayerByOwnerActorNr(num);
				if ((bool)playerByOwnerActorNr)
				{
					PositionsAndRotation placePose = _podium.GetPlacePose(i + 1, participantActorNumbers.Count);
					if (placePose != null)
					{
						playerByOwnerActorNr.SnapForPose(placePose.Position, placePose.Rotation);
					}
				}
			}
		}
	}
}
