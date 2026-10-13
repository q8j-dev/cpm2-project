using System;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class MatchmakingFunnelTracker
	{
		private readonly IArenaNetwork _net;

		private bool _active;

		private ArenaEntrySource _source;

		private bool _friendsOnly;

		private LobbyLimits _limits;

		private ArenaPhase _phase;

		private bool _createdRoom;

		private bool _roomEntered;

		private bool _minReached;

		private bool _full;

		private bool _locked;

		private bool _terminated;

		public event Action<LobbyFunnelSnapshot> OnSnapshot;

		public MatchmakingFunnelTracker(IArenaNetwork net)
		{
			_net = net;
		}

		public void BeginAttempt(ArenaEntrySource source, bool friendsOnly, LobbyLimits limits)
		{
			_source = source;
			_friendsOnly = friendsOnly;
			_limits = limits;
			_phase = ArenaPhase.JoiningWaitingArea;
			_createdRoom = false;
			_roomEntered = false;
			_minReached = false;
			_full = false;
			_locked = false;
			_terminated = false;
			_active = true;
			Emit(LobbyFunnelStatus.SearchStart);
		}

		public void OnRoomCreated()
		{
			if (_active && !_terminated)
			{
				_createdRoom = true;
			}
		}

		public void OnRoomJoined()
		{
			if (_active && !_terminated && !_roomEntered)
			{
				_roomEntered = true;
				if (_createdRoom)
				{
					Emit(LobbyFunnelStatus.RoomCreated);
				}
				Emit(LobbyFunnelStatus.RoomEntered);
				EvaluateComposition();
			}
		}

		public void OnRoomStateChanged()
		{
			EvaluateComposition();
		}

		public void OnPhase(ArenaPhase phase)
		{
			if (!_active || _terminated)
			{
				return;
			}
			_phase = phase;
			switch (phase)
			{
			case ArenaPhase.SearchPlayers:
				EvaluateComposition();
				break;
			case ArenaPhase.Countdown:
				if (!_locked)
				{
					_locked = true;
					Emit(LobbyFunnelStatus.Locked);
				}
				break;
			case ArenaPhase.InProgress:
				_terminated = true;
				Emit(LobbyFunnelStatus.Started);
				break;
			}
		}

		public void EndAttempt(ArenaLeaveReason reason, Exception error = null)
		{
			if (_active)
			{
				if (!_terminated)
				{
					_terminated = true;
					Emit(LobbyFunnelStatus.Left, reason, error);
				}
				_active = false;
			}
		}

		private void EvaluateComposition()
		{
			if (_active && !_terminated && _roomEntered && !_locked)
			{
				int playerCount = _net.PlayerCount;
				if (!_minReached && playerCount >= _limits.MinPlayers)
				{
					_minReached = true;
					Emit(LobbyFunnelStatus.MinReached);
				}
				if (!_full && playerCount >= _limits.MaxPlayers)
				{
					_full = true;
					Emit(LobbyFunnelStatus.Full);
				}
			}
		}

		private void Emit(LobbyFunnelStatus status, ArenaLeaveReason reason = ArenaLeaveReason.Unknown, Exception error = null)
		{
			bool flag = _roomEntered && status != LobbyFunnelStatus.SearchStart && status != LobbyFunnelStatus.RoomCreated && status != LobbyFunnelStatus.Left;
			bool flag2 = status == LobbyFunnelStatus.MinReached || status == LobbyFunnelStatus.Full || status == LobbyFunnelStatus.Locked;
			bool flag3 = flag2 || status == LobbyFunnelStatus.RoomEntered;
			bool flag4 = status == LobbyFunnelStatus.SearchStart || status == LobbyFunnelStatus.RoomEntered;
			bool flag5 = status switch
			{
				LobbyFunnelStatus.SearchStart => _source == ArenaEntrySource.Create, 
				LobbyFunnelStatus.RoomCreated => true, 
				_ => false, 
			};
			OnSnapshot?.Invoke(new LobbyFunnelSnapshot(status, _roomEntered ? _net.RoomLobbyId : string.Empty, flag4 ? new ArenaEntrySource?(_source) : ((ArenaEntrySource?)null), flag5 ? new bool?(_friendsOnly) : ((bool?)null), (status == LobbyFunnelStatus.Left) ? new ArenaLeaveReason?(reason) : ((ArenaLeaveReason?)null), (status == LobbyFunnelStatus.Left) ? error : null, flag ? new int?(_net.PlayerCount) : ((int?)null), flag ? new int?(_limits.MinPlayers) : ((int?)null), flag ? new int?(_limits.MaxPlayers) : ((int?)null), flag2 ? RemainingSeconds(status) : ((float?)null), flag3 ? new float?(PhaseLimitSeconds()) : ((float?)null)));
		}

		private float? RemainingSeconds(LobbyFunnelStatus status)
		{
			double deadline = _net.ReadLobbyState().Deadline;
			if (deadline <= 0.0)
			{
				return null;
			}
			double num = deadline - _net.ServerTime;
			if (num <= 0.0)
			{
				return 0f;
			}
			if (status == LobbyFunnelStatus.MinReached && _limits.MinPlayersWaitSeconds > 0f)
			{
				num = Math.Min(num, _limits.MinPlayersWaitSeconds);
			}
			return (float)num;
		}

		private float PhaseLimitSeconds()
		{
			if (_phase != ArenaPhase.Countdown)
			{
				return _limits.WaitSeconds;
			}
			return _limits.CountdownSeconds;
		}
	}
}
