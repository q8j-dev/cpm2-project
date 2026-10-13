using System;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public readonly struct LobbyFunnelSnapshot
	{
		public readonly LobbyFunnelStatus Status;

		public readonly string LobbyId;

		public readonly ArenaEntrySource? EntrySource;

		public readonly bool? FriendsOnly;

		public readonly ArenaLeaveReason? LeaveReason;

		public readonly Exception LeaveError;

		public readonly int? PlayersCurrent;

		public readonly int? PlayersMin;

		public readonly int? PlayersMax;

		public readonly float? TimeLeft;

		public readonly float? TimeLimit;

		public LobbyFunnelSnapshot(LobbyFunnelStatus status, string lobbyId, ArenaEntrySource? entrySource, bool? friendsOnly, ArenaLeaveReason? leaveReason, Exception leaveError, int? playersCurrent, int? playersMin, int? playersMax, float? timeLeft, float? timeLimit)
		{
			LeaveError = leaveError;
			Status = status;
			LobbyId = lobbyId;
			EntrySource = entrySource;
			FriendsOnly = friendsOnly;
			LeaveReason = leaveReason;
			PlayersCurrent = playersCurrent;
			PlayersMin = playersMin;
			PlayersMax = playersMax;
			TimeLeft = timeLeft;
			TimeLimit = timeLimit;
		}
	}
}
