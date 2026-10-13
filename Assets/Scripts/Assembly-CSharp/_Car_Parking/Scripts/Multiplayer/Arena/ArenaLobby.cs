using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaLobby
	{
		private readonly IArenaNetwork _net;

		private readonly ArenaConfig _config;

		private readonly ArenaPhaseModel _phase;

		public const int MinForceStartPlayers = 2;

		public bool CanForceStart
		{
			get
			{
				if (_phase.Phase == ArenaPhase.SearchPlayers && _net.IsMasterClient && _net.PlayerCount >= 2)
				{
					return _net.IsFriendsRoom;
				}
				return false;
			}
		}

		public ArenaLobby(IArenaNetwork net, ArenaConfig config, ArenaPhaseModel phase)
		{
			_net = net;
			_config = config;
			_phase = phase;
		}

		public async UniTask AssembleAsync(CancellationToken ct)
		{
			_net.SetPlayerReady(ready: false);
			_phase.Phase = ArenaPhase.SearchPlayers;
			if (_net.IsMasterClient)
			{
				ArenaLobbyState state = new ArenaLobbyState
				{
					Phase = ArenaPhase.SearchPlayers,
					Deadline = _net.ServerTime + (double)_config.WaitSeconds
				};
				_net.WriteLobbyState(state);
				_net.SetRoomOpen(open: true);
			}
			await RunMasterLoopAsync(ct);
		}

		private async UniTask RunMasterLoopAsync(CancellationToken ct)
		{
			while (!ct.IsCancellationRequested && (_phase.Phase == ArenaPhase.SearchPlayers || _phase.Phase == ArenaPhase.Countdown))
			{
				if (_net.IsMasterClient)
				{
					ArenaLobbyState arenaLobbyState = _net.ReadLobbyState();
					ArenaStateMachine.Inputs input = new ArenaStateMachine.Inputs
					{
						MinPlayers = _config.MinPlayers,
						MaxPlayers = _config.MaxPlayers,
						PlayerCount = _net.PlayerCount,
						ServerTime = _net.ServerTime,
						Deadline = arenaLobbyState.Deadline,
						MinPlayersWaitSeconds = _config.MinPlayersWaitSeconds
					};
					ArenaDecision d = ArenaStateMachine.Tick(_phase.Phase, input);
					ExecuteMasterCommand(d, arenaLobbyState);
					_phase.Phase = d.NextPhase;
				}
				else
				{
					ArenaLobbyState arenaLobbyState2 = _net.ReadLobbyState();
					if (arenaLobbyState2.Phase >= ArenaPhase.SearchPlayers)
					{
						_phase.Phase = arenaLobbyState2.Phase;
					}
				}
				if (_phase.Phase == ArenaPhase.InProgress)
				{
					break;
				}
				await UniTask.Delay(TimeSpan.FromSeconds(0.25), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}

		private void ExecuteMasterCommand(ArenaDecision d, ArenaLobbyState s)
		{
			switch (d.Command)
			{
			case ArenaCommand.StartCountdown:
				s.Phase = ArenaPhase.Countdown;
				s.Deadline = _net.ServerTime + (double)_config.CountdownSeconds;
				_net.SetRoomOpen(open: false);
				_net.WriteLobbyState(s);
				break;
			case ArenaCommand.ExtendWait:
				s.Deadline = _net.ServerTime + (double)_config.WaitSeconds;
				_net.WriteLobbyState(s);
				break;
			case ArenaCommand.ClampWait:
			{
				double num = Math.Min(s.Deadline, _net.ServerTime + (double)_config.MinPlayersWaitSeconds);
				if (num < s.Deadline && _config.MinPlayersWaitSeconds > 0f)
				{
					s.Deadline = num;
					_net.WriteLobbyState(s);
				}
				break;
			}
			case ArenaCommand.BeginMode:
				s.Phase = ArenaPhase.InProgress;
				_net.WriteLobbyState(s);
				break;
			case ArenaCommand.Return:
				break;
			}
		}

		public void ForceStartCountdown()
		{
			if (CanForceStart)
			{
				StartCountdownNow();
			}
		}

		private void StartCountdownNow()
		{
			ArenaLobbyState s = _net.ReadLobbyState();
			ExecuteMasterCommand(new ArenaDecision(ArenaPhase.Countdown, ArenaCommand.StartCountdown), s);
			_phase.Phase = ArenaPhase.Countdown;
		}
	}
}
