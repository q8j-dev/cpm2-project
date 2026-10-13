using System;
using System.Collections.Generic;
using CPM.PlayerData;
using Photon.Pun;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class MatchmakingLobbyAnalytic : AnalyticProviderBase, IInitializable, IDisposable
	{
		private const string MultiplayerGameMode = "multi_player";

		private readonly ArenaFlowController _flow;

		private readonly IArenaNetwork _net;

		private readonly ArenaModeRegistry _registry;

		private readonly IEventService _eventService;

		private readonly PlayerDataStorage _playerData;

		private readonly MatchmakingFunnelTracker _tracker;

		private bool HasPlayerId => !string.IsNullOrEmpty(_playerData?.PlayerID?.ToString());

		[Preserve]
		public MatchmakingLobbyAnalytic(AnalyticEventFactory analyticEventFactory, ArenaFlowController flow, IArenaNetwork net, ArenaModeRegistry registry, IEventService eventService, PlayerDataStorage playerData)
			: base(analyticEventFactory)
		{
			_flow = flow;
			_net = net;
			_registry = registry;
			_eventService = eventService;
			_playerData = playerData;
			_tracker = new MatchmakingFunnelTracker(net);
		}

		public void Initialize()
		{
			_tracker.OnSnapshot += SendSnapshot;
			_flow.OnPhaseChanged += OnPhaseChanged;
			_net.OnCreatedRoom += _tracker.OnRoomCreated;
			_net.OnJoinedRoom += _tracker.OnRoomJoined;
			_net.OnRoomStateChanged += _tracker.OnRoomStateChanged;
		}

		public void Dispose()
		{
			_tracker.OnSnapshot -= SendSnapshot;
			_flow.OnPhaseChanged -= OnPhaseChanged;
			_net.OnCreatedRoom -= _tracker.OnRoomCreated;
			_net.OnJoinedRoom -= _tracker.OnRoomJoined;
			_net.OnRoomStateChanged -= _tracker.OnRoomStateChanged;
		}

		private void OnPhaseChanged(ArenaPhase phase)
		{
			switch (phase)
			{
			case ArenaPhase.JoiningWaitingArea:
				_tracker.BeginAttempt(_flow.EntrySource, _flow.EntryFriendsOnly, ReadLimits());
				break;
			case ArenaPhase.Idle:
				_tracker.EndAttempt(_flow.LastLeaveReason, _flow.LastLeaveException);
				break;
			default:
				_tracker.OnPhase(phase);
				break;
			}
		}

		private LobbyLimits ReadLimits()
		{
			if (_registry.TryGetConfig(_flow.ArenaId, out var config, onlyEnabled: false) && (bool)config)
			{
				return new LobbyLimits(config.MinPlayers, config.MaxPlayers, config.WaitSeconds, config.CountdownSeconds, config.MinPlayersWaitSeconds);
			}
			return default;
		}

		private void SendSnapshot(LobbyFunnelSnapshot snapshot)
		{
			_Car_Parking.Scripts.EventSystem.Data.Event obj = ResolveGameEvent();
			LobbyFailReason? lobbyFailReason = MapFailReason(snapshot.LeaveReason);
			bool flag = lobbyFailReason == LobbyFailReason.Error;
			base.AnalyticEventFactory.CreateEvent<MatchmakingLobbyEvent, MatchmakingLobbyContext>(new MatchmakingLobbyContext
			{
				Status = MapStatus(snapshot.Status),
				LobbyId = snapshot.LobbyId,
				EntryPath = MapEntryPath(snapshot.EntrySource),
				Visibility = MapVisibility(snapshot.FriendsOnly),
				PlayersCurrent = snapshot.PlayersCurrent,
				PlayersMin = snapshot.PlayersMin,
				PlayersMax = snapshot.PlayersMax,
				FailReason = lobbyFailReason,
				ErrorCode = (flag ? (LobbyErrorCode.From(snapshot.LeaveError) ?? "OTHER") : null),
				ErrorMessage = (flag ? LobbyErrorCode.MessageOf(snapshot.LeaveError) : null),
				TimeLeft = snapshot.TimeLeft,
				TimeLimit = snapshot.TimeLimit,
				GameEventId = obj?.Id,
				EventVariant = obj?.Type.ToString(),
				GameMode = "multi_player",
				Region = (PhotonNetwork.IsConnected ? PhotonNetwork.CloudRegion : null),
				RegionPing = (PhotonNetwork.InRoom ? new int?(PhotonNetwork.GetPing()) : ((int?)null)),
				RaceId = MapRaceId(snapshot.Status),
				ArenaId = MapArenaId(_flow.ArenaId)
			}).Send();
		}

		private _Car_Parking.Scripts.EventSystem.Data.Event ResolveGameEvent()
		{
			if (!ArenaActivityMap.TryGetActivity(_flow.CurrentMode, out var activityType))
			{
				return null;
			}
			if (!EventActivityMap.TryGetEvent(activityType, out var eventType))
			{
				return null;
			}
			IReadOnlyList<_Car_Parking.Scripts.EventSystem.Data.Event> readOnlyList = _eventService?.CurrentEvents;
			if (readOnlyList == null)
			{
				return null;
			}
			foreach (_Car_Parking.Scripts.EventSystem.Data.Event item in readOnlyList)
			{
				if (item != null && item.Type == eventType)
				{
					return item;
				}
			}
			return null;
		}

		private static MatchmakingLobbyStatus MapStatus(LobbyFunnelStatus status)
		{
			return status switch
			{
				LobbyFunnelStatus.SearchStart => MatchmakingLobbyStatus.SearchStart, 
				LobbyFunnelStatus.RoomCreated => MatchmakingLobbyStatus.RoomCreated, 
				LobbyFunnelStatus.RoomEntered => MatchmakingLobbyStatus.RoomEntered, 
				LobbyFunnelStatus.MinReached => MatchmakingLobbyStatus.MinReached, 
				LobbyFunnelStatus.Full => MatchmakingLobbyStatus.Full, 
				LobbyFunnelStatus.Locked => MatchmakingLobbyStatus.Locked, 
				LobbyFunnelStatus.Started => MatchmakingLobbyStatus.Started, 
				_ => MatchmakingLobbyStatus.Left, 
			};
		}

		private string MapRaceId(LobbyFunnelStatus status)
		{
			if (status != LobbyFunnelStatus.Locked && status != LobbyFunnelStatus.Started)
			{
				return null;
			}
			return _flow.RaceId;
		}

		private static string MapArenaId(ArenaId arenaId)
		{
			if (arenaId != ArenaId.Free)
			{
				return arenaId.ToString();
			}
			return null;
		}

		private static LobbyEntryPath? MapEntryPath(ArenaEntrySource? source)
		{
			return source switch
			{
				ArenaEntrySource.QuickMatch => LobbyEntryPath.QuickMatch, 
				ArenaEntrySource.Create => LobbyEntryPath.Create, 
				ArenaEntrySource.JoinFriend => LobbyEntryPath.JoinFriend, 
				_ => null, 
			};
		}

		private static LobbyVisibility? MapVisibility(bool? friendsOnly)
		{
			if (!friendsOnly.HasValue)
			{
				return null;
			}
			return (!friendsOnly.Value) ? LobbyVisibility.Open : LobbyVisibility.Friends;
		}

		private LobbyFailReason? MapFailReason(ArenaLeaveReason? reason)
		{
			if (!reason.HasValue || reason.Value == ArenaLeaveReason.Unknown)
			{
				return null;
			}
			if (!HasPlayerId)
			{
				return LobbyFailReason.NoAccount;
			}
			return reason.Value switch
			{
				ArenaLeaveReason.UserLeft => LobbyFailReason.UserLeft, 
				ArenaLeaveReason.Disconnect => LobbyFailReason.Disconnect, 
				ArenaLeaveReason.Error => LobbyFailReason.Error, 
				_ => null, 
			};
		}
	}
}
