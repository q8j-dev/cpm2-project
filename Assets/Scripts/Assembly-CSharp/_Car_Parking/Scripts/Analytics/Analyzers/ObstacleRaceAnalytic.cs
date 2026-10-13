using System;
using System.Collections.Generic;
using CPM.Fallout;
using Newtonsoft.Json;
using Photon.Pun;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class ObstacleRaceAnalytic : AnalyticProviderBase, IInitializable, IDisposable
	{
		private static readonly JsonSerializerSettings CheckpointJson = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore
		};

		private readonly ObstacleRaceRunTracker _run;

		private readonly IArenaNetwork _net;

		private readonly ArenaFlowController _flow;

		private readonly FalloutRaceSession _raceSession;

		private readonly HashSet<FalloutRaceState> _sentResults = new HashSet<FalloutRaceState>();

		private string _raceId;

		private string _lobbyId;

		private string _arenaId;

		private int? _regionPing;

		[Preserve]
		public ObstacleRaceAnalytic(AnalyticEventFactory analyticEventFactory, ObstacleRaceRunTracker run, IArenaNetwork net, ArenaFlowController flow, FalloutRaceSession raceSession)
			: base(analyticEventFactory)
		{
			_run = run;
			_net = net;
			_flow = flow;
			_raceSession = raceSession;
		}

		public void Initialize()
		{
			_run.OnSnapshot += OnRunSnapshot;
			_raceSession.ResultUpdated += OnRaceResult;
		}

		public void Dispose()
		{
			_run.OnSnapshot -= OnRunSnapshot;
			_raceSession.ResultUpdated -= OnRaceResult;
		}

		private void OnRunSnapshot(ObstacleRaceRunSnapshot snapshot)
		{
			if (snapshot != null)
			{
				if (snapshot.Status == ObstacleRaceRunStatus.Go)
				{
					CacheIdentity(snapshot);
				}
				LobbyFailReason? lobbyFailReason = ((snapshot.Status == ObstacleRaceRunStatus.Left) ? MapFailReason(_flow.LastLeaveReason) : ((LobbyFailReason?)null));
				bool flag = lobbyFailReason == LobbyFailReason.Error;
				base.AnalyticEventFactory.CreateEvent<ObstacleRaceEvent, ObstacleRaceContext>(new ObstacleRaceContext
				{
					Status = MapStatus(snapshot.Status),
					RaceId = _raceId,
					LobbyId = _lobbyId,
					ArenaId = _arenaId,
					RegionPing = _regionPing,
					PlayersCurrent = ((snapshot.Status == ObstacleRaceRunStatus.Go) ? snapshot.PlayersAtStart : PlayerCountOrNull()),
					IsMaster = snapshot.IsMaster,
					TrackSeed = snapshot.TrackSeed,
					TrackPlan = FormatPlan(snapshot.TrackPlan),
					RacePosition = snapshot.Place,
					DurationMs = snapshot.DurationMs,
					CheckpointCount = snapshot.CheckpointCount,
					CheckpointEarned = snapshot.CheckpointEarned,
					CheckpointMissed = snapshot.CheckpointMissed,
					RestartCount = snapshot.RestartCount,
					Checkpoints = FormatCheckpoints(snapshot.Checkpoints),
					ProgressCurrent = snapshot.ProgressCurrent,
					FailReason = lobbyFailReason,
					ErrorCode = (flag ? (LobbyErrorCode.From(_flow.LastLeaveException) ?? "OTHER") : null),
					ErrorMessage = (flag ? LobbyErrorCode.MessageOf(_flow.LastLeaveException) : null)
				}).Send();
			}
		}

		private void OnRaceResult(FalloutRaceResult result)
		{
			if (result != null && !string.IsNullOrEmpty(_raceId) && !(result.RaceId != _raceId) && _sentResults.Add(result.State))
			{
				bool flag = result.State == FalloutRaceState.Claimed;
				base.AnalyticEventFactory.CreateEvent<ObstacleRaceEvent, ObstacleRaceContext>(new ObstacleRaceContext
				{
					Status = ObstacleRaceStatus.Result,
					RaceId = _raceId,
					LobbyId = _lobbyId,
					ArenaId = _arenaId,
					Result = MapResult(result.State),
					RacePosition = ((result.Place > 0) ? new int?(result.Place) : ((int?)null)),
					RewardHard = (flag ? new int?((int)result.Coins) : ((int?)null)),
					RewardSoft = (flag ? new int?((int)result.Money) : ((int?)null))
				}).Send();
			}
		}

		private void CacheIdentity(ObstacleRaceRunSnapshot snapshot)
		{
			_raceId = snapshot.RaceId;
			_lobbyId = (_net.InRoom ? _net.RoomLobbyId : null);
			_arenaId = MapArenaId(_flow.ArenaId);
			_regionPing = (_net.InRoom ? new int?(PhotonNetwork.GetPing()) : ((int?)null));
			_sentResults.Clear();
		}

		private int? PlayerCountOrNull()
		{
			if (!_net.InRoom)
			{
				return null;
			}
			return _net.PlayerCount;
		}

		private static string MapArenaId(ArenaId arenaId)
		{
			if (arenaId != ArenaId.Free)
			{
				return arenaId.ToString();
			}
			return null;
		}

		private static ObstacleRaceStatus MapStatus(ObstacleRaceRunStatus status)
		{
			return status switch
			{
				ObstacleRaceRunStatus.Go => ObstacleRaceStatus.Go, 
				ObstacleRaceRunStatus.Finished => ObstacleRaceStatus.Finished, 
				_ => ObstacleRaceStatus.Left, 
			};
		}

		private static ObstacleRaceResultStatus MapResult(FalloutRaceState state)
		{
			return state switch
			{
				FalloutRaceState.Finished => ObstacleRaceResultStatus.Finished, 
				FalloutRaceState.Claimed => ObstacleRaceResultStatus.Claimed, 
				_ => ObstacleRaceResultStatus.Failed, 
			};
		}

		private static LobbyFailReason? MapFailReason(ArenaLeaveReason reason)
		{
			return reason switch
			{
				ArenaLeaveReason.UserLeft => LobbyFailReason.UserLeft, 
				ArenaLeaveReason.Disconnect => LobbyFailReason.Disconnect, 
				ArenaLeaveReason.Error => LobbyFailReason.Error, 
				_ => null, 
			};
		}

		private static string FormatPlan(string[] plan)
		{
			if (plan == null || plan.Length == 0)
			{
				return null;
			}
			return string.Join(",", plan);
		}

		private static string FormatCheckpoints(IReadOnlyList<ObstacleRaceCheckpointRecord> records)
		{
			if (records == null || records.Count == 0)
			{
				return null;
			}
			List<ObstacleRaceCheckpointDto> list = new List<ObstacleRaceCheckpointDto>(records.Count);
			for (int i = 0; i < records.Count; i++)
			{
				ObstacleRaceCheckpointRecord obstacleRaceCheckpointRecord = records[i];
				list.Add(new ObstacleRaceCheckpointDto
				{
					Id = obstacleRaceCheckpointRecord.Id,
					Time = (obstacleRaceCheckpointRecord.Time.HasValue ? new float?((float)Math.Round(obstacleRaceCheckpointRecord.Time.Value, 2)) : ((float?)null)),
					Restarts = obstacleRaceCheckpointRecord.Restarts,
					Missed = obstacleRaceCheckpointRecord.Missed
				});
			}
			return JsonConvert.SerializeObject(list, CheckpointJson);
		}
	}
}
