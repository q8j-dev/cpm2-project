using System;
using System.Collections.Generic;
using System.Threading;
using CPM.RouteStandings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ObstacleRaceStandingsDriver : IDisposable
	{
		private const float TickInterval = 0.5f;

		private readonly LobbyRoster _roster;

		private readonly OnlineCollectionsProvider _online;

		private readonly int _localActor;

		private readonly Func<SurvivalEliminationBoard> _readBoard;

		private readonly Func<int, double?> _claimTimeOf;

		private readonly Func<int, string> _nameOf;

		private readonly Func<int, float?> _remainingOf;

		private readonly RouteMetrics _metrics;

		private readonly StandingsCalculator _calculator;

		private readonly List<int> _participants = new List<int>();

		private readonly List<RacerProgress> _racers = new List<RacerProgress>();

		private readonly List<ObstacleRaceStandingRow> _rows = new List<ObstacleRaceStandingRow>();

		private readonly List<int> _runners = new List<int>();

		private CancellationTokenSource _cts;

		public int? LastProgress { get; private set; }

		public ObstacleRaceStandingsUpdate Latest { get; private set; }

		public event Action<ObstacleRaceStandingsUpdate> Updated;

		public ObstacleRaceStandingsDriver(RoutePath route, IArenaContext ctx, LobbyRoster roster, OnlineCollectionsProvider online, int localActor, Func<SurvivalEliminationBoard> readBoard, Func<int, double?> claimTimeOf)
		{
			_roster = roster;
			_online = online;
			_localActor = localActor;
			_readBoard = readBoard;
			_claimTimeOf = claimTimeOf;
			_nameOf = roster.GetDisplayName;
			_remainingOf = RemainingOf;
			_metrics = new RouteMetrics(BakeRoute(route));
			_calculator = new StandingsCalculator(_metrics);
			IReadOnlyList<int> participantActorNumbers = ctx.ParticipantActorNumbers;
			for (int i = 0; i < participantActorNumbers.Count; i++)
			{
				_participants.Add(participantActorNumbers[i]);
			}
			if (_metrics.IsValid)
			{
				_cts = new CancellationTokenSource();
				TickAsync(_cts.Token).Forget();
			}
		}

		public void Dispose()
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
		}

		private async UniTaskVoid TickAsync(CancellationToken ct)
		{
			while (!(await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow()))
			{
				Refresh();
			}
		}

		private static List<Vector3> BakeRoute(RoutePath route)
		{
			if (!route)
			{
				return new List<Vector3>();
			}
			Transform[] waypoints = route.Waypoints;
			if (waypoints == null)
			{
				return new List<Vector3>();
			}
			List<Vector3> list = new List<Vector3>(waypoints.Length);
			foreach (Transform transform in waypoints)
			{
				if ((bool)transform)
				{
					list.Add(transform.position - V.GlobalOffset);
				}
			}
			return list;
		}

		private void Refresh()
		{
			CollectRacers();
			_calculator.Update(_racers);
			SurvivalEliminationBoard board = _readBoard();
			_runners.Clear();
			IReadOnlyList<RacerStanding> standings = _calculator.Standings;
			for (int i = 0; i < standings.Count; i++)
			{
				_runners.Add(standings[i].Id);
			}
			ObstacleRaceLiveStandings.Build(_rows, board, _participants, _runners, _localActor, _nameOf, _claimTimeOf, _remainingOf);
			ReadLocalProgress(board, out var progress, out var remainingKm);
			Latest = new ObstacleRaceStandingsUpdate(_rows, ObstacleRaceLiveStandings.LocalPlace(_rows), progress, remainingKm);
			Updated?.Invoke(Latest);
		}

		private float? RemainingOf(int actor)
		{
			if (!_calculator.TryGetRemaining(actor, out var meters))
			{
				return null;
			}
			return meters;
		}

		private void ReadLocalProgress(SurvivalEliminationBoard board, out float? progress, out float? remainingKm)
		{
			progress = null;
			remainingKm = null;
			if (!board.Contains(_localActor) && _calculator.TryGetRemaining(_localActor, out var meters))
			{
				float totalLength = _metrics.TotalLength;
				float num = ((totalLength > 0f) ? Mathf.Clamp01(1f - meters / totalLength) : 0f);
				LastProgress = Mathf.RoundToInt(num * 100f);
				progress = num;
				remainingKm = meters / 1000f;
			}
		}

		private void CollectRacers()
		{
			_racers.Clear();
			Vector3 globalOffset = V.GlobalOffset;
			MultiPersonControl[] playersOnlineNow = _online.PlayersOnlineNow;
			foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
			{
				if ((bool)multiPersonControl)
				{
					int ownerActorNr = multiPersonControl.photonView.OwnerActorNr;
					if (ownerActorNr != _localActor && _participants.Contains(ownerActorNr))
					{
						_racers.Add(new RacerProgress(ownerActorNr, multiPersonControl.transform.position - globalOffset));
					}
				}
			}
			if ((bool)V.DriverPerson)
			{
				_racers.Add(new RacerProgress(_localActor, V.DriverPerson.transform.position - globalOffset));
			}
		}
	}
}
