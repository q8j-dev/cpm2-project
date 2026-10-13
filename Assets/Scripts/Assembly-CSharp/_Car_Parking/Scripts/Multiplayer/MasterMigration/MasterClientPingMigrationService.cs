using System;
using System.Collections.Generic;
using System.Threading;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Multiplayer.RequestResponseService;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.RemoteConfig;

namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	public sealed class MasterClientPingMigrationService : MonoBehaviourPunCallbacks
	{
		private IRemoteConfigManager _remoteConfig;

		private RpcRrService _rpcRr;

		private FreeDriveProvider _freeDriveProvider;

		private PingSampler _sampler;

		private Dictionary<PlayerKey, PingEntry> _candidatesCache;

		private CancellationTokenSource _loopCts;

		private MasterMigrationRemoteConfig _sessionCfg;

		private float _joinedAtRealtime;

		private float _lastMigrationAtRealtime;

		private float _lastPollStartedAt = float.NegativeInfinity;

		private float _becameMasterAtRealtime = float.NegativeInfinity;

		private int _pollSeq;

		private int _activePollId;

		private float _setMasterCalledAtRealtime = float.NegativeInfinity;

		private string _lastWatchdogSkipReason;

		private bool _migrationFlagRaised;

		private bool _initStarted;

		private CancellationTokenSource _pendingPollCts;

		private CancellationTokenSource _fallbackTimerCts;

		private const float POST_WARMUP_BUFFER_SEC = 1f;

		private const float POST_COOLDOWN_BUFFER_SEC = 0.5f;

		private const bool LOGS_ENABLED = true;

		private const float FRESH_CONFIG_TIMEOUT_SEC = 3f;

		[Inject]
		public void Construct(IRemoteConfigManager remoteConfig, RpcRrService rpcRr, FreeDriveProvider freeDriveProvider)
		{
			_remoteConfig = remoteConfig;
			_rpcRr = rpcRr;
			_freeDriveProvider = freeDriveProvider;
		}

		private void Awake()
		{
			if (PhotonNetwork.OfflineMode)
			{
				base.enabled = false;
				Log("[MasterMigration] disabled — offline mode");
			}
		}

		private void OnDestroy()
		{
			RpcRequestPublishAvgPing.OnReceived -= OnRequestPublishAvgPingReceived;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			if (PhotonNetwork.InRoom && _sessionCfg == null)
			{
				OnJoinedRoom();
			}
		}

		public override void OnDisable()
		{
			base.OnDisable();
			StopLoops();
		}

		public override void OnJoinedRoom()
		{
			_joinedAtRealtime = Time.realtimeSinceStartup;
			_lastMigrationAtRealtime = _joinedAtRealtime;
			if (PhotonNetwork.IsMasterClient)
			{
				_becameMasterAtRealtime = _joinedAtRealtime;
			}
			_lastPollStartedAt = float.NegativeInfinity;
			if (_sessionCfg != null)
			{
				_sampler?.Clear();
				StartLoops();
			}
			else if (!_initStarted)
			{
				_initStarted = true;
				InitializeAndStartAsync().Forget();
			}
		}

		private async UniTaskVoid InitializeAndStartAsync()
		{
			try
			{
				CancellationToken ct = this.GetCancellationTokenOnDestroy();
				var (flag, masterMigrationRemoteConfig) = await GetFreshConfigAsync(ct).TimeoutWithoutException(TimeSpan.FromSeconds(3.0));
				if (ct.IsCancellationRequested)
				{
					return;
				}
				MasterMigrationRemoteConfig masterMigrationRemoteConfig2;
				if (flag || masterMigrationRemoteConfig == null)
				{
					masterMigrationRemoteConfig2 = GetBootstrapConfig();
					Log($"[MasterMigration] fresh config timeout ({3f}s) — falling back to bootstrap (enabled={masterMigrationRemoteConfig2.enabled})");
				}
				else
				{
					masterMigrationRemoteConfig2 = masterMigrationRemoteConfig;
				}
				_sessionCfg = masterMigrationRemoteConfig2;
				if (!masterMigrationRemoteConfig2.enabled)
				{
					Log("[MasterMigration] disabled by config at multiplayer start — shutting down service");
					base.enabled = false;
					return;
				}
				_sampler = new PingSampler(masterMigrationRemoteConfig2.maxPingBufferSize, masterMigrationRemoteConfig2.minSamplesForAverage);
				_candidatesCache = new Dictionary<PlayerKey, PingEntry>();
				RpcRequestPublishAvgPing.OnReceived += OnRequestPublishAvgPingReceived;
				Log($"[MasterMigration] initialized enabled=true masterThreshold={masterMigrationRemoteConfig2.masterPingThresholdMs}ms watchdogInterval={masterMigrationRemoteConfig2.masterPingCheckIntervalSec}s fallbackPollInterval={masterMigrationRemoteConfig2.fallbackPollIntervalSec}s cooldown={masterMigrationRemoteConfig2.cooldownSec}s");
				if (PhotonNetwork.InRoom)
				{
					StartLoops();
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception arg)
			{
				LogError($"[MasterMigration] init failed: {arg}");
			}
		}

		public override void OnLeftRoom()
		{
			LowerFlag();
			StopLoops();
			_sampler?.Clear();
			_candidatesCache?.Clear();
		}

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			_lastMigrationAtRealtime = Time.realtimeSinceStartup;
			if (newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				_becameMasterAtRealtime = _lastMigrationAtRealtime;
			}
			Log($"[MasterMigration] OnMasterClientSwitched -> actor#{newMasterClient.ActorNumber}");
			if (_activePollId > 0 && _setMasterCalledAtRealtime > 0f && _lastPollStartedAt > 0f)
			{
				int num = (int)((_lastMigrationAtRealtime - _setMasterCalledAtRealtime) * 1000f);
				int num2 = (int)((_lastMigrationAtRealtime - _lastPollStartedAt) * 1000f);
				Log($"[MasterMigration] poll#{_activePollId} DONE total={num2}ms photonSwitch={num}ms (target actor#{newMasterClient.ActorNumber})");
				_activePollId = 0;
				_setMasterCalledAtRealtime = float.NegativeInfinity;
			}
			if (newMasterClient.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				LowerFlag();
			}
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			if (PhotonNetwork.IsMasterClient && _migrationFlagRaised)
			{
				MasterMigrationRemoteConfig sessionCfg = _sessionCfg;
				float delaySec = sessionCfg.warmupSec + 1f;
				SchedulePoll(sessionCfg, delaySec, $"player entered actor#{newPlayer.ActorNumber}");
			}
		}

		private MasterMigrationRemoteConfig GetBootstrapConfig()
		{
			return (_remoteConfig?.GetDefaultConfigStorage())?.masterMigration ?? new MasterMigrationRemoteConfig();
		}

		private async UniTask<MasterMigrationRemoteConfig> GetFreshConfigAsync(CancellationToken ct)
		{
			if (_remoteConfig == null)
			{
				return new MasterMigrationRemoteConfig();
			}
			return (await _remoteConfig.GetConfigValuesStorage().AttachExternalCancellation(ct))?.masterMigration ?? GetBootstrapConfig();
		}

		private void StartLoops()
		{
			StopLoops();
			_loopCts = new CancellationTokenSource();
			SampleLoop(_loopCts.Token).Forget();
			MasterWatchdogLoop(_loopCts.Token).Forget();
		}

		private void StopLoops()
		{
			if (_loopCts != null)
			{
				_loopCts.Cancel();
				_loopCts.Dispose();
				_loopCts = null;
			}
		}

		private async UniTaskVoid SampleLoop(CancellationToken ct)
		{
			try
			{
				while (!ct.IsCancellationRequested)
				{
					if (PhotonNetwork.InRoom)
					{
						_sampler.AddSample(PhotonNetwork.GetPing());
					}
					await UniTask.Delay(TimeSpan.FromSeconds(_sessionCfg.sampleIntervalSec), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception arg)
			{
				LogError($"[MasterMigration] sample loop crashed: {arg}");
			}
		}

		private async UniTaskVoid MasterWatchdogLoop(CancellationToken ct)
		{
			try
			{
				while (!ct.IsCancellationRequested)
				{
					MasterMigrationRemoteConfig cfg = _sessionCfg;
					await UniTask.Delay(TimeSpan.FromSeconds(cfg.masterPingCheckIntervalSec), DelayType.Realtime, PlayerLoopTiming.Update, ct);
					string text = null;
					if (!PhotonNetwork.InRoom)
					{
						text = "not-in-room";
					}
					else if ((PhotonNetwork.CurrentRoom?.PlayerCount ?? 0) <= 1)
					{
						text = "alone";
					}
					else if (!PhotonNetwork.IsMasterClient)
					{
						text = "not-master";
					}
					else if (Time.realtimeSinceStartup - _joinedAtRealtime < cfg.warmupSec)
					{
						text = "warmup-joined";
					}
					else if (Time.realtimeSinceStartup - _becameMasterAtRealtime < cfg.warmupSec)
					{
						text = "warmup-master";
					}
					if (text != null)
					{
						if (_migrationFlagRaised)
						{
							LowerFlag();
						}
						if (_lastWatchdogSkipReason != text)
						{
							_lastWatchdogSkipReason = text;
							Log("[MasterMigration] watchdog skip — " + text);
						}
						continue;
					}
					int? average = _sampler.GetAverage();
					if (!average.HasValue || average.Value <= cfg.masterPingThresholdMs)
					{
						if (_migrationFlagRaised)
						{
							Log(string.Format("[MasterMigration] avg recovered (avg={0}ms <= {1}ms) — lowering flag", average.HasValue ? average.Value.ToString() : "n/a", cfg.masterPingThresholdMs));
							LowerFlag();
						}
						if (_lastWatchdogSkipReason != "ping-below-threshold")
						{
							_lastWatchdogSkipReason = "ping-below-threshold";
							Log(string.Format("[MasterMigration] watchdog skip — ping-below-threshold (avg={0}ms threshold={1}ms)", average.HasValue ? average.Value.ToString() : "n/a", cfg.masterPingThresholdMs));
						}
						continue;
					}
					_lastWatchdogSkipReason = null;
					if (!_migrationFlagRaised)
					{
						_migrationFlagRaised = true;
						float num = Time.realtimeSinceStartup - _lastMigrationAtRealtime;
						if (num < cfg.cooldownSec)
						{
							float num2 = cfg.cooldownSec - num + 0.5f;
							Log($"[MasterMigration] flag raised (avg={average.Value}ms > {cfg.masterPingThresholdMs}ms) — cooldown active ({num:F1}s < {cfg.cooldownSec}s), scheduling in {num2:F1}s");
							SchedulePoll(cfg, num2, "migration-cooldown");
						}
						else
						{
							Log($"[MasterMigration] flag raised (avg={average.Value}ms > {cfg.masterPingThresholdMs}ms) — immediate RPC");
							TriggerPoll(cfg);
						}
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception arg)
			{
				LogError($"[MasterMigration] watchdog loop crashed: {arg}");
			}
		}

		private void LowerFlag()
		{
			if (_migrationFlagRaised || _pendingPollCts != null || _fallbackTimerCts != null)
			{
				_migrationFlagRaised = false;
				_pendingPollCts?.Cancel();
				_pendingPollCts?.Dispose();
				_pendingPollCts = null;
				_fallbackTimerCts?.Cancel();
				_fallbackTimerCts?.Dispose();
				_fallbackTimerCts = null;
			}
		}

		private void SchedulePoll(MasterMigrationRemoteConfig cfg, float delaySec, string reason)
		{
			if (_loopCts != null)
			{
				if (_pendingPollCts != null)
				{
					Log("[MasterMigration] SchedulePoll skipped — pending poll already active (" + reason + " ignored)");
					return;
				}
				DelayedTriggerAsync(cfg, delaySec, reason, _pendingPollCts = CancellationTokenSource.CreateLinkedTokenSource(_loopCts.Token)).Forget();
			}
		}

		private async UniTaskVoid DelayedTriggerAsync(MasterMigrationRemoteConfig cfg, float delaySec, string reason, CancellationTokenSource cts)
		{
			try
			{
				Log($"[MasterMigration] poll scheduled in {delaySec:F1}s ({reason})");
				await UniTask.Delay(TimeSpan.FromSeconds(delaySec), DelayType.Realtime, PlayerLoopTiming.Update, cts.Token);
				if (_migrationFlagRaised && PhotonNetwork.IsMasterClient)
				{
					TriggerPoll(cfg);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception arg)
			{
				LogError($"[MasterMigration] delayed-trigger crashed: {arg}");
			}
			finally
			{
				if (_pendingPollCts == cts)
				{
					_pendingPollCts = null;
				}
				cts.Dispose();
			}
		}

		private void StartFallbackTimer(MasterMigrationRemoteConfig cfg)
		{
			if (_loopCts != null)
			{
				_fallbackTimerCts?.Cancel();
				_fallbackTimerCts?.Dispose();
				_fallbackTimerCts = CancellationTokenSource.CreateLinkedTokenSource(_loopCts.Token);
				FallbackLoopAsync(cfg, _fallbackTimerCts.Token).Forget();
			}
		}

		private async UniTaskVoid FallbackLoopAsync(MasterMigrationRemoteConfig cfg, CancellationToken ct)
		{
			try
			{
				await UniTask.Delay(TimeSpan.FromSeconds(cfg.fallbackPollIntervalSec), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				if (_migrationFlagRaised && PhotonNetwork.IsMasterClient)
				{
					Log($"[MasterMigration] fallback timer elapsed ({cfg.fallbackPollIntervalSec}s) — triggering poll");
					TriggerPoll(cfg);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception arg)
			{
				LogError($"[MasterMigration] fallback loop crashed: {arg}");
			}
		}

		private void TriggerPoll(MasterMigrationRemoteConfig cfg)
		{
			MultiPlayer multiPlayer = _freeDriveProvider?.MultiPlayer;
			if (multiPlayer == null)
			{
				Log("[MasterMigration] poll aborted — MultiPlayer not yet initialised in FreeDriveProvider");
				StartFallbackTimer(cfg);
				return;
			}
			int num = (_activePollId = ++_pollSeq);
			_setMasterCalledAtRealtime = float.NegativeInfinity;
			_lastPollStartedAt = Time.realtimeSinceStartup;
			Log($"[MasterMigration] poll#{num} started");
			multiPlayer.SendValueStr<RpcRequestPublishAvgPing>(string.Empty, string.Empty);
			WaitAndDecideAsync(cfg, _loopCts.Token, num).Forget();
			StartFallbackTimer(cfg);
		}

		private async UniTaskVoid WaitAndDecideAsync(MasterMigrationRemoteConfig cfg, CancellationToken ct, int pollId)
		{
			try
			{
				float t0 = _lastPollStartedAt;
				Log($"[MasterMigration] poll#{pollId} t+{(int)((Time.realtimeSinceStartup - t0) * 1000f)}ms waiting responseBufferSec={cfg.responseBufferSec}s for replies");
				await UniTask.Delay(TimeSpan.FromSeconds(cfg.responseBufferSec), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				Log($"[MasterMigration] poll#{pollId} t+{(int)((Time.realtimeSinceStartup - t0) * 1000f)}ms buffer elapsed");
				if (!PhotonNetwork.IsMasterClient)
				{
					Log($"[MasterMigration] poll#{pollId} abort: no longer master after buffer (master switched during buffer)");
					_activePollId = 0;
				}
				else
				{
					CollectAnswersAndDecide(cfg, ct, pollId);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception arg)
			{
				LogError($"[MasterMigration] wait-and-decide crashed: {arg}");
			}
		}

		private void CollectAnswersAndDecide(MasterMigrationRemoteConfig cfg, CancellationToken ct, int pollId)
		{
			float t0 = _lastPollStartedAt;
			int? average = _sampler.GetAverage();
			if (!average.HasValue)
			{
				Log($"[MasterMigration] poll#{pollId} t+{StageMs()}ms abort: masterAvg not available");
				_activePollId = 0;
				return;
			}
			int value = average.Value;
			Player[] playerList = PhotonNetwork.PlayerList;
			int num = 0;
			int num2 = 0;
			Log($"[MasterMigration] poll#{pollId} t+{StageMs()}ms scanning {playerList.Length} players (masterAvg={value}ms)");
			_candidatesCache.Clear();
			Dictionary<PlayerKey, PingEntry> candidatesCache = _candidatesCache;
			Player[] array = playerList;
			foreach (Player player in array)
			{
				if (!player.IsMasterClient)
				{
					num++;
					if (!TryGetAvgPing(player, out var ping))
					{
						Log($"[MasterMigration] poll#{pollId} actor#{player.ActorNumber} no AvgPing in CustomProperties");
						continue;
					}
					if (ping <= 0)
					{
						Log($"[MasterMigration] poll#{pollId} actor#{player.ActorNumber} invalid ping={ping}");
						continue;
					}
					num2++;
					Log($"[MasterMigration] ap read: actor#{player.ActorNumber} ping={ping}ms");
					candidatesCache[new PlayerKey(player.ActorNumber)] = new PingEntry(ping);
				}
			}
			Log($"[MasterMigration] poll#{pollId} t+{StageMs()}ms replied {num2}/{num} within responseBufferSec");
			int num3 = 0;
			foreach (KeyValuePair<PlayerKey, PingEntry> item in candidatesCache)
			{
				int num4 = value - item.Value.AvgPing;
				bool flag = num4 > cfg.pingBufferMs;
				if (flag)
				{
					num3++;
				}
				Log($"[MasterMigration] poll#{pollId} candidate actor#{item.Key.ActorNumber} " + $"ping={item.Value.AvgPing}ms master={value}ms delta={num4}ms " + string.Format("buffer={0}ms → {1}", cfg.pingBufferMs, flag ? "eligible" : "REJECTED (delta<=buffer)"));
			}
			float timeSinceLastMigrationSec = Time.realtimeSinceStartup - _lastMigrationAtRealtime;
			int num5 = PhotonNetwork.CurrentRoom?.PlayerCount ?? 0;
			MigrationDecision migrationDecision = MigrationDecider.Evaluate(new MigrationSnapshot(value, candidatesCache, timeSinceLastMigrationSec, num5, cfg));
			Log($"[MasterMigration] poll#{pollId} SUMMARY master={value}ms " + $"playerCount={num5} others={num} replied={num2} " + string.Format("eligible={0} decision={1} ", num3, migrationDecision.ShouldMigrate ? "Migrate" : "None") + "reason=\"" + migrationDecision.Reason + "\"");
			Log(string.Format("[MasterMigration] poll#{0} t+{1}ms decision: {2} ({3})", pollId, StageMs(), migrationDecision.ShouldMigrate ? $"Migrate->#{migrationDecision.Target.ActorNumber}" : "None", migrationDecision.Reason));
			if (!migrationDecision.ShouldMigrate)
			{
				Log($"[MasterMigration] poll#{pollId} abort: decision.ShouldMigrate=false ({migrationDecision.Reason})");
				_activePollId = 0;
				return;
			}
			Player player2 = FindPlayerByActor(migrationDecision.Target.ActorNumber);
			if (player2 == null)
			{
				Log($"[MasterMigration] poll#{pollId} abort: target actor#{migrationDecision.Target.ActorNumber} not in PlayerList anymore");
				_activePollId = 0;
			}
			else
			{
				TryMigrateAsync(player2, cfg, ct, pollId).Forget();
			}
			int StageMs()
			{
				return (int)((Time.realtimeSinceStartup - t0) * 1000f);
			}
		}

		private async UniTaskVoid TryMigrateAsync(Player target, MasterMigrationRemoteConfig cfg, CancellationToken ct, int pollId)
		{
			try
			{
				float t0 = _lastPollStartedAt;
				int waited = 0;
				int budget = cfg.maxWaitForRpcRrClearMs;
				bool initialActive = _rpcRr != null && _rpcRr.HasActiveRequests;
				Log($"[MasterMigration] poll#{pollId} t+{StageMs()}ms rpcRr.HasActiveRequests={initialActive} (budget={budget}ms)");
				while (_rpcRr != null && _rpcRr.HasActiveRequests && waited < budget)
				{
					await UniTask.Delay(100, DelayType.Realtime, PlayerLoopTiming.Update, ct);
					waited += 100;
				}
				if (_rpcRr != null && _rpcRr.HasActiveRequests)
				{
					Log($"[MasterMigration] poll#{pollId} t+{StageMs()}ms RpcRr still active after {budget}ms budget — deferring (next watchdog tick)");
					_activePollId = 0;
					return;
				}
				if (initialActive)
				{
					Log($"[MasterMigration] poll#{pollId} t+{StageMs()}ms rpcRr cleared after {waited}ms");
				}
				_setMasterCalledAtRealtime = Time.realtimeSinceStartup;
				bool flag = PhotonNetwork.SetMasterClient(target);
				Log($"[MasterMigration] poll#{pollId} t+{StageMs()}ms SetMasterClient(actor#{target.ActorNumber}) result={flag}");
				if (!flag)
				{
					_activePollId = 0;
					_setMasterCalledAtRealtime = float.NegativeInfinity;
				}
				int StageMs()
				{
					return (int)((Time.realtimeSinceStartup - t0) * 1000f);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception arg)
			{
				LogError($"[MasterMigration] try-migrate crashed: {arg}");
			}
		}

		private void OnRequestPublishAvgPingReceived()
		{
			if (!PhotonNetwork.InRoom)
			{
				Log("[MasterMigration] publish request ignored — not in room");
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				Log("[MasterMigration] publish request ignored — I am master");
				return;
			}
			float num = Time.realtimeSinceStartup - _joinedAtRealtime;
			if (num < _sessionCfg.warmupSec)
			{
				Log($"[MasterMigration] publish request ignored — warmup ({num:F1}s < {_sessionCfg.warmupSec}s)");
			}
			else
			{
				PublishMyAvgPing();
			}
		}

		private void PublishMyAvgPing()
		{
			int? average = _sampler.GetAverage();
			if (!average.HasValue)
			{
				Log("[MasterMigration] publish skipped — sampler has no average yet");
				return;
			}
			if (average.Value <= 0)
			{
				Log($"[MasterMigration] publish skipped — avg={average.Value} (<=0)");
				return;
			}
			Hashtable propertiesToSet = new Hashtable { { "ap", average.Value } };
			PhotonNetwork.LocalPlayer.SetCustomProperties(propertiesToSet);
			Log($"[MasterMigration] Published avgPing={average.Value}ms");
		}

		private static bool TryGetAvgPing(Player p, out int ping)
		{
			ping = 0;
			if (p?.CustomProperties == null)
			{
				return false;
			}
			if (!p.CustomProperties.TryGetValue("ap", out var value))
			{
				return false;
			}
			if (value is int num)
			{
				ping = num;
				return true;
			}
			return false;
		}

		private static Player FindPlayerByActor(int actorNumber)
		{
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player player in playerList)
			{
				if (player.ActorNumber == actorNumber)
				{
					return player;
				}
			}
			return null;
		}

		private void Log(string msg)
		{
		}

		private void LogError(string msg)
		{
		}
	}
}
