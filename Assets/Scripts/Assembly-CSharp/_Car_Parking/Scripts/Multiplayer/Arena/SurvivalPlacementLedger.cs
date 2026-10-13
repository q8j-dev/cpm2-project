using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using _Car_Parking.Scripts.Multiplayer.Arena.UI;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class SurvivalPlacementLedger
	{
		private const string FinishKey = "orF";

		private const float ClaimRetrySeconds = 1f;

		private readonly LobbyRoster _roster;

		private readonly Dictionary<int, double> _claimTimes = new Dictionary<int, double>();

		private SurvivalEliminationBoard _board;

		private CancellationToken _runCt;

		private int _myActor;

		private bool _claimed;

		public SurvivalPlacementLedger(LobbyRoster roster)
		{
			_roster = roster;
		}

		public void ClearOwnClaim(IArenaContext ctx)
		{
			ctx?.ClearOwnModeState("orE");
		}

		public void Begin(IArenaContext ctx, int myActor, CancellationToken ct)
		{
			_myActor = myActor;
			_runCt = ct;
			if (ctx.IsMasterClient)
			{
				_board = SurvivalEliminationBoard.FromProperty(ctx.ReadModeState("orF"));
			}
		}

		public bool TryClaim(IArenaContext ctx, out double elapsed)
		{
			elapsed = 0.0;
			if (_claimed)
			{
				return false;
			}
			_claimed = true;
			elapsed = ctx.ServerTime - ctx.MatchStartTime;
			PublishClaim(ctx, elapsed);
			if (ctx.IsMasterClient)
			{
				RegisterElimination(ctx, _myActor);
			}
			ConfirmClaimAsync(ctx, elapsed, _runCt).Forget();
			return true;
		}

		public void Reconcile(IArenaContext ctx)
		{
			if (ctx.IsMasterClient)
			{
				ReconcileClaims(ctx);
			}
			CacheClaimTimes(ctx);
			StampLeaveTimes(ctx);
		}

		public int PlaceOf(IArenaContext ctx)
		{
			return SurvivalPlace.For(ReadBoard(ctx), ctx.ParticipantActorNumbers, _myActor);
		}

		public IReadOnlyList<ObstacleRaceResultRow> BuildStandings(IArenaContext ctx)
		{
			CacheClaimTimes(ctx);
			return SurvivalStandings.Build(ReadBoard(ctx), ctx.ParticipantActorNumbers, _myActor, _roster, ClaimTimeOf);
		}

		public SurvivalEliminationBoard ReadBoard(IArenaContext ctx)
		{
			if (!ctx.IsMasterClient)
			{
				return SurvivalEliminationBoard.FromProperty(ctx.ReadModeState("orF"));
			}
			return EnsureBoard(ctx);
		}

		public double? ClaimTimeOf(int actor)
		{
			if (!_claimTimes.TryGetValue(actor, out var value))
			{
				return null;
			}
			return value;
		}

		public void Reset()
		{
			_board = null;
			_claimTimes.Clear();
			_claimed = false;
			_myActor = 0;
			_runCt = default;
		}

		private void PublishClaim(IArenaContext ctx, double elapsed)
		{
			ctx.WriteOwnModeState("orE", SurvivalClaim.Encode(elapsed));
		}

		private async UniTaskVoid ConfirmClaimAsync(IArenaContext ctx, double elapsed, CancellationToken ct)
		{
			try
			{
				while (!ReadBoard(ctx).Contains(_myActor))
				{
					await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
					if (!ReadBoard(ctx).Contains(_myActor))
					{
						PublishClaim(ctx, elapsed);
						continue;
					}
					break;
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void RegisterElimination(IArenaContext ctx, int actor)
		{
			SurvivalEliminationBoard survivalEliminationBoard = EnsureBoard(ctx);
			if (survivalEliminationBoard.TryAdd(actor))
			{
				ctx.WriteModeState("orF", survivalEliminationBoard.ToProperty());
			}
		}

		private void ReconcileClaims(IArenaContext ctx)
		{
			IReadOnlyList<int> readOnlyList = SurvivalClaimReconciler.Pending(ctx.ParticipantActorNumbers, (int actor) => ctx.ReadPlayerModeState(actor, "orE"), EnsureBoard(ctx));
			for (int num = 0; num < readOnlyList.Count; num++)
			{
				RegisterElimination(ctx, readOnlyList[num]);
			}
		}

		private void CacheClaimTimes(IArenaContext ctx)
		{
			IReadOnlyList<int> participantActorNumbers = ctx.ParticipantActorNumbers;
			if (participantActorNumbers == null)
			{
				return;
			}
			for (int i = 0; i < participantActorNumbers.Count; i++)
			{
				int num = participantActorNumbers[i];
				if (!_claimTimes.ContainsKey(num) && SurvivalClaim.TryParse(ctx.ReadPlayerModeState(num, "orE"), out var elapsed))
				{
					_claimTimes[num] = elapsed;
				}
			}
		}

		private void StampLeaveTimes(IArenaContext ctx)
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			IReadOnlyList<int> participantActorNumbers = ctx.ParticipantActorNumbers;
			if (currentRoom == null || participantActorNumbers == null)
			{
				return;
			}
			for (int i = 0; i < participantActorNumbers.Count; i++)
			{
				int key = participantActorNumbers[i];
				if (!_claimTimes.ContainsKey(key) && !currentRoom.Players.ContainsKey(key))
				{
					_claimTimes[key] = Math.Max(0.0, ctx.ServerTime - ctx.MatchStartTime);
				}
			}
		}

		private SurvivalEliminationBoard EnsureBoard(IArenaContext ctx)
		{
			return _board ?? (_board = SurvivalEliminationBoard.FromProperty(ctx.ReadModeState("orF")));
		}
	}
}
