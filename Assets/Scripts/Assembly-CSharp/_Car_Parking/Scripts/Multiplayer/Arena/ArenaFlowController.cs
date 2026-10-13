using System;
using System.Threading;
using CPM.Fallout;
using CPM.PlayerData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaFlowController : IDisposable
	{
		private readonly IArenaNetwork _net;

		private readonly ArenaModeRegistry _registry;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly ArenaSceneNavigator _navigator;

		private readonly PlayerDataStorage _playerData;

		private readonly FalloutRaceSession _raceSession;

		private readonly ArenaForcedOutfitProvider _forcedOutfit;

		private readonly ArenaPhaseModel _phaseModel = new ArenaPhaseModel();

		private CancellationTokenSource _cts;

		private ArenaConfig _config;

		private IArenaModeController _mode;

		private ArenaReturnContext _return;

		private ArenaLobby _lobby;

		private ArenaEntryOptions _entry;

		private const string RematchUnavailableKey = "SurvivorRace.RematchUnavailable";

		public ArenaPhase Phase => _phaseModel.Phase;

		public ArenaModeType CurrentMode
		{
			get
			{
				if (!_config)
				{
					return ArenaModeType.None;
				}
				return _config.ModeType;
			}
		}

		public ArenaId ArenaId
		{
			get
			{
				if (!_config)
				{
					return ArenaId.Free;
				}
				return _config.ArenaId;
			}
		}

		public ArenaLocation CurrentWaitingLocation
		{
			get
			{
				if (!_config)
				{
					return ArenaLocation.Racing;
				}
				return _config.WaitingLocation;
			}
		}

		public ArenaEntrySource EntrySource => _entry.Source;

		public bool EntryFriendsOnly => _entry.FriendsOnly;

		public ArenaLeaveReason LastLeaveReason { get; private set; }

		public Exception LastLeaveException { get; private set; }

		public string RaceId => BuildRaceId(_net.ReadRoomProperty("aRT") as string);

		public bool CanForceStartCountdown => _lobby?.CanForceStart ?? false;

		public event Action<ArenaPhase> OnPhaseChanged
		{
			add
			{
				_phaseModel.OnPhaseChanged += value;
			}
			remove
			{
				_phaseModel.OnPhaseChanged -= value;
			}
		}

		public ArenaFlowController(IArenaNetwork net, ArenaModeRegistry registry, FreeDriveProvider freeDriveProvider, ArenaSceneNavigator navigator, PlayerDataStorage playerData, FalloutRaceSession raceSession, ArenaForcedOutfitProvider forcedOutfit)
		{
			_net = net;
			_registry = registry;
			_freeDriveProvider = freeDriveProvider;
			_navigator = navigator;
			_playerData = playerData;
			_raceSession = raceSession;
			_forcedOutfit = forcedOutfit;
			_net.OnJoinedRoom += HandleJoinedRoom;
			_net.OnEnterRoomFailed += HandleEnterRoomFailed;
		}

		public void Dispose()
		{
			_net.OnJoinedRoom -= HandleJoinedRoom;
			_net.OnEnterRoomFailed -= HandleEnterRoomFailed;
			ReleaseRun();
		}

		public bool StartArena(ArenaId arenaId, ArenaEntrySource source)
		{
			return StartArena(arenaId, ArenaEntryOptions.Public(source));
		}

		public bool StartArenaForFriends(ArenaId arenaId, int password, ArenaEntrySource source)
		{
			return StartArena(arenaId, ArenaEntryOptions.ForFriends(password, source));
		}

		public bool JoinArenaRoom(ArenaId arenaId, string roomName, ArenaEntrySource source)
		{
			return StartArena(arenaId, ArenaEntryOptions.JoinRoom(roomName, source));
		}

		private bool StartArena(ArenaId arenaId, ArenaEntryOptions entry)
		{
			if (_phaseModel.Phase != ArenaPhase.Idle)
			{
				return false;
			}
			return BeginEnter(arenaId, entry, preserveReturn: false);
		}

		private bool BeginEnter(ArenaId arenaId, ArenaEntryOptions entry, bool preserveReturn)
		{
			if (!_registry.TryGet(arenaId, out var controller, out var config))
			{
				return false;
			}
			ReleaseRun();
			_mode = controller;
			_config = config;
			_forcedOutfit.Clear();
			_registry.ClearAllModes();
			_net.SetLocalNickName(_playerData.PlayerName);
			_lobby = new ArenaLobby(_net, _config, _phaseModel);
			_mode.Enter();
			_entry = entry;
			_cts = new CancellationTokenSource();
			_phaseModel.Phase = ArenaPhase.JoiningWaitingArea;
			ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false).Forget();
			EnterFlowAsync(entry, preserveReturn, _cts.Token).Forget();
			return true;
		}

		public void ForceStartCountdown()
		{
			_lobby?.ForceStartCountdown();
		}

		private async UniTaskVoid EnterFlowAsync(ArenaEntryOptions entry, bool preserveReturn, CancellationToken ct)
		{
			Time.timeScale = 1f;
			try
			{
				ArenaReturnContext arenaReturnContext = await _navigator.EnterAsync(_config, entry, ct);
				if (!preserveReturn)
				{
					_return = arenaReturnContext;
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception error)
			{
				LeaveArena(ArenaLeaveReason.Error, error);
				_navigator.RecoverToMenu();
			}
		}

		public void LeaveArena(ArenaLeaveReason reason, Exception error = null, bool leaveRoom = true)
		{
			if (_phaseModel.Phase != ArenaPhase.Idle)
			{
				LastLeaveReason = reason;
				LastLeaveException = error;
				if (_phaseModel.Phase == ArenaPhase.JoiningWaitingArea)
				{
					ManagerUIHelper.CloseWaitingWindow().Forget();
				}
				ReleaseRun();
				if (leaveRoom && _net.InRoom)
				{
					_net.LeaveRoom();
				}
				_freeDriveProvider.ResetArenaId();
				_phaseModel.Phase = ArenaPhase.Idle;
			}
		}

		private void ReleaseRun()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cts);
			_mode?.Clear();
			_mode = null;
			_lobby = null;
		}

		private void HandleJoinedRoom()
		{
			if (_phaseModel.Phase == ArenaPhase.JoiningWaitingArea)
			{
				RunSessionAsync(_cts.Token).Forget();
			}
		}

		private async UniTaskVoid RunSessionAsync(CancellationToken ct)
		{
			_ = 2;
			try
			{
				string roomName = _net.RoomName;
				int roomPassword = ((_net.ReadRoomProperty("P") is int num) ? num : 0);
				await _lobby.AssembleAsync(ct);
				if (await BeginModeAsync(ct))
				{
					BeginRestart(roomName, roomPassword);
				}
				else
				{
					await ReturnToMenuAsync(ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception error)
			{
				LeaveArena(ArenaLeaveReason.Error, error);
				_navigator.RecoverToMenu();
			}
		}

		private void BeginRestart(string roomName, int roomPassword)
		{
			if (roomPassword == 0)
			{
				BeginEnter(ArenaId, ArenaEntryOptions.Public(_entry.Source), preserveReturn: true);
				return;
			}
			string text = ArenaRoomNames.BuildRematchRoomName(roomName, ArenaId);
			if (string.IsNullOrEmpty(text))
			{
				BeginEnter(ArenaId, ArenaEntryOptions.Public(_entry.Source), preserveReturn: true);
			}
			else
			{
				BeginEnter(ArenaId, ArenaEntryOptions.Rematch(text, roomPassword, _entry.Source), preserveReturn: true);
			}
		}

		private void HandleEnterRoomFailed(short returnCode, string message)
		{
			if (_phaseModel.Phase == ArenaPhase.JoiningWaitingArea && _entry.CreateIfMissing)
			{
				FailRematchToMenu(new Exception($"rematch room unavailable ({returnCode}): {message}"));
			}
		}

		private void FailRematchToMenu(Exception error)
		{
			LeaveArena(ArenaLeaveReason.Error, error);
			_navigator.RecoverToMenu();
			ManagerUIHelper.SetMessageOnMenu(new LocalizedString("SurvivorRace.RematchUnavailable"), 5f, NotificationType.Error).Forget();
		}

		private async UniTask<bool> BeginModeAsync(CancellationToken ct)
		{
			using ArenaContext ctx = new ArenaContext(_net, _config);
			await _mode.Init(ctx, ct);
			_net.SetPlayerReady(ready: true);
			string raceId = await ResolveRaceIdAsync(ctx, ct);
			ctx.SetRaceId(raceId);
			double matchStartTime = await AwaitStartScheduledAsync(ct);
			float num = (float)(matchStartTime - _net.ServerTime);
			if (num < 0f)
			{
				num = 0f;
			}
			await _mode.PreStartAsync(ctx, num, ct);
			await UniTask.WaitUntil(() => _net.ServerTime >= matchStartTime, PlayerLoopTiming.Update, ct);
			if (!string.IsNullOrEmpty(raceId))
			{
				_raceSession.Register(raceId);
			}
			await _mode.RunAsync(ctx, ct);
			return ctx.RestartRequested;
		}

		private async UniTask<string> ResolveRaceIdAsync(IArenaContext ctx, CancellationToken ct)
		{
			if (!ArenaActivityMap.TryGetActivity(_config.ModeType, out var _))
			{
				return null;
			}
			string text = await ArenaRaceToken.ResolveAsync(ctx, ct);
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			string text2 = BuildRaceId(text);
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			return text2;
		}

		private string BuildRaceId(string token)
		{
			if (string.IsNullOrEmpty(token) || !_config)
			{
				return null;
			}
			if (!ArenaActivityMap.TryGetActivity(_config.ModeType, out var activityType))
			{
				return null;
			}
			Result<string> result = FalloutRaceId.Build((int)activityType, token);
			if (!result.IsFail)
			{
				return result.Value;
			}
			return null;
		}

		private async UniTask<double> AwaitStartScheduledAsync(CancellationToken ct)
		{
			double timeoutAt = _net.ServerTime + (double)_config.ReadyBarrierTimeoutSeconds;
			while (!ct.IsCancellationRequested)
			{
				ArenaLobbyState arenaLobbyState = _net.ReadLobbyState();
				if (_net.IsMasterClient && arenaLobbyState.MatchStartTime <= 0.0 && (_net.AllPlayersReady || _net.ServerTime >= timeoutAt))
				{
					arenaLobbyState.MatchStartTime = _net.ServerTime + (double)_config.MatchStartCountdownSeconds;
					_net.WriteLobbyState(arenaLobbyState);
				}
				if (arenaLobbyState.MatchStartTime > 0.0)
				{
					return arenaLobbyState.MatchStartTime;
				}
				await UniTask.Delay(TimeSpan.FromSeconds(0.1), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
			ct.ThrowIfCancellationRequested();
			return 0.0;
		}

		private async UniTask ReturnToMenuAsync(CancellationToken ct)
		{
			_phaseModel.Phase = ArenaPhase.Returning;
			if (!(await _navigator.ReturnAsync(_return, ct)))
			{
				_navigator.RecoverToMenu();
			}
			ReleaseRun();
			_phaseModel.Phase = ArenaPhase.Idle;
		}
	}
}
