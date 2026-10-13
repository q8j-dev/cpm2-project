using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Common;
using Cysharp.Threading.Tasks;
using FunctionalExtensions;
using GMESDK;
using Photon.Pun;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Providers;

namespace GME_voice_chat
{
	public class GMEVoiceChatManager : IDisposable
	{
		public const string VoiceChatGmePrefKey = "VoiceChat";

		public static int VoiceChatDefault = 1;

		private const float CheckNearestOnlinePlayersDelay = 3f;

		private const float MinDistanceForConnect = 70f;

		private const int MaxVelocityToConnect = 30;

		private const int NumberToHashUserId = 10001;

		private const int CurrentRegion = 1;

		private const int DefaultTeamMode = 0;

		public readonly uint PlaySendGme = AudioWrapper.GetId("Play_gme_send");

		public readonly uint StopSendGme = AudioWrapper.GetId("Stop_gme_send");

		public readonly uint PlayReceiveGme = AudioWrapper.GetId("Play_gme_recive");

		public readonly uint StopReceiveGme = AudioWrapper.GetId("Stop_gme_recive");

		private string _playerId;

		private bool _isJoinedInRoom;

		private bool _isPressed;

		private List<string> _mutedUsers = new List<string>();

		private IDisposable _timerToCheckNearestPlayers;

		private Vector3 _driverPos;

		private const int Range = 50;

		private const int SetGainDbValue = 0;

		private IDisposable _sendDriverPersonPositionToGmeDisposable;

		private MainPerson _mainPerson;

		private GameObject _headPerson;

		private bool _isDestroyed;

		private MenuLevelParent _menuLevelParent;

		private MultiPlayer _multiPlayer;

		private OnlineCollectionsProvider _onlineProvider;

		private SoundBankLoadService _soundBankLoadService;

		private AppStateHandler _appStateHandler;

		private bool _soundBankLoading;

		public List<MultiPersonControl> SpeakingPlayers { get; private set; } = new List<MultiPersonControl>();

		[Inject]
		private void Construct(SoundBankLoadService soundBankLoadService, OnlineCollectionsProvider onlineProvider, MenuLevelParent menuLevelParent, AppStateHandler appStateHandler, MultiPlayer multiPlayer = null)
		{
			_onlineProvider = onlineProvider;
			_menuLevelParent = menuLevelParent;
			_multiPlayer = multiPlayer;
			_soundBankLoadService = soundBankLoadService;
			_appStateHandler = appStateHandler;
		}

		public void Init()
		{
			_menuLevelParent.VoiceRecordStarted += StartSpeaking;
			_menuLevelParent.VoiceRecordReleased += StopSpeaking;
			_multiPlayer.JoinedInRoom += SetStartSettings;
			_multiPlayer.Disconnected += OnMultiplayerDisconnected;
			_appStateHandler.ApplicationPause += OnApplicationPauseChanged;
			CoreGameManager coreGameManager = V.CoreGameManager;
			coreGameManager.OnPlayerMuted = (Action<MultiPersonControl, bool>)Delegate.Combine(coreGameManager.OnPlayerMuted, new Action<MultiPersonControl, bool>(AddRemoveUserToMutedUsers));
			_mainPerson = V.DriverPerson.GetComponent<MainPerson>();
			_headPerson = _mainPerson.ik.references.head.gameObject;
			SphereCollider sphereCollider = _headPerson.AddComponent<SphereCollider>();
			sphereCollider.isTrigger = true;
			sphereCollider.radius = 0.16f;
			_playerId = GetHashUserId(V.DriverPerson.GetComponent<PhotonView>().ViewID);
			GMEWWisePluginNative.GMEWWisePlugin_SetRegion(1);
			GMEWWisePluginNative.GMEWWisePlugin_SetRoomID(PhotonNetwork.CurrentRoom.Name);
			GMEWWisePluginNative.GMEWWisePlugin_SetUserID(_playerId);
			GMEWWisePluginNative.GMEWWisePlugin_SetRangeAudioTeamID(int.Parse(_playerId));
			GMEWWisePluginNative.GMEWWisePlugin_SetRangeAudioTeamMode(0);
			GMEWWisePluginNative.GMEWWisePlugin_SetRangeAudioRecvRange(50);
			GMEWWisePluginNative.GMEWWisePlugin_SetGainDB(0);
		}

		private void StartSendDriverPos()
		{
			_sendDriverPersonPositionToGmeDisposable?.Dispose();
			_sendDriverPersonPositionToGmeDisposable = Observable.Interval(TimeSpan.FromSeconds(0.20000000298023224)).Subscribe((long _) =>
			{
				SendDriverPosToGmeAndCheckSpeakingPlayer();
			}).AddTo(V.DriverPerson);
		}

		public async UniTask SwitchVoiceChat(bool isOn, CancellationToken token)
		{
			if (_soundBankLoading || _soundBankLoadService == null)
			{
				return;
			}
			if (isOn)
			{
				if (_soundBankLoadService.IsLoaded(SoundBankType.CpmVoiceChat))
				{
					EnableDisableVoiceChat(value: true);
					return;
				}
				_soundBankLoading = true;
				await _soundBankLoadService.TryLoadBank(SoundBankType.CpmVoiceChat);
				_soundBankLoading = false;
				if (_isDestroyed || token.IsCancellationRequested)
				{
					_soundBankLoadService.TryUnloadBank(SoundBankType.CpmVoiceChat).Forget();
					return;
				}
				bool flag = PlayerPrefs.GetInt("VoiceChat", VoiceChatDefault) == 1;
				if (_soundBankLoadService.IsLoaded(SoundBankType.CpmVoiceChat) & flag)
				{
					EnableDisableVoiceChat(value: true);
					return;
				}
				_soundBankLoading = true;
				await _soundBankLoadService.TryUnloadBank(SoundBankType.CpmVoiceChat);
				_soundBankLoading = false;
			}
			else if (_soundBankLoadService.IsLoaded(SoundBankType.CpmVoiceChat))
			{
				EnableDisableVoiceChat(value: false);
				_soundBankLoading = true;
				await _soundBankLoadService.TryUnloadBank(SoundBankType.CpmVoiceChat);
				_soundBankLoading = false;
				if (!_isDestroyed && !token.IsCancellationRequested && PlayerPrefs.GetInt("VoiceChat", VoiceChatDefault) == 1)
				{
					await SwitchVoiceChat(isOn: true, token);
				}
			}
		}

		private void EnableDisableVoiceChat(bool value)
		{
			if (value)
			{
				Connect();
				StartSendDriverPos();
				StartTimer();
			}
			else
			{
				_sendDriverPersonPositionToGmeDisposable?.Dispose();
				_sendDriverPersonPositionToGmeDisposable = null;
				_timerToCheckNearestPlayers?.Dispose();
				_timerToCheckNearestPlayers = null;
				Disconnect();
			}
		}

		public void Dispose()
		{
			if ((bool)_menuLevelParent)
			{
				_menuLevelParent.VoiceRecordStarted -= StartSpeaking;
				_menuLevelParent.VoiceRecordReleased -= StopSpeaking;
			}
			if ((bool)_multiPlayer)
			{
				_multiPlayer.JoinedInRoom -= SetStartSettings;
				_multiPlayer.Disconnected -= OnMultiplayerDisconnected;
			}
			if ((bool)_appStateHandler)
			{
				_appStateHandler.ApplicationPause -= OnApplicationPauseChanged;
			}
			if ((bool)V.CoreGameManager)
			{
				CoreGameManager coreGameManager = V.CoreGameManager;
				coreGameManager.OnPlayerMuted = (Action<MultiPersonControl, bool>)Delegate.Remove(coreGameManager.OnPlayerMuted, new Action<MultiPersonControl, bool>(AddRemoveUserToMutedUsers));
			}
			Disconnect();
			_isDestroyed = true;
			if (_soundBankLoadService != null && _soundBankLoadService.IsLoaded(SoundBankType.CpmVoiceChat))
			{
				_soundBankLoadService.TryUnloadBank(SoundBankType.CpmVoiceChat).Forget();
			}
			_sendDriverPersonPositionToGmeDisposable?.Dispose();
			_sendDriverPersonPositionToGmeDisposable = null;
			_timerToCheckNearestPlayers?.Dispose();
			_timerToCheckNearestPlayers = null;
		}

		private void SendDriverPosToGmeAndCheckSpeakingPlayer()
		{
			if ((bool)V.DriverPerson)
			{
				_driverPos = V.DriverPerson.transform.localPosition;
				GMEWWisePluginNative.GMEWWisePlugin_SetSelfPosition((int)_driverPos.x, (int)_driverPos.y, (int)_driverPos.z);
				if (_isJoinedInRoom)
				{
					CheckSpeakingPlayer();
				}
			}
		}

		private void AddRemoveUserToMutedUsers(MultiPersonControl user, bool isMute)
		{
			PhotonView component = user.GetComponent<PhotonView>();
			string hashUserId = GetHashUserId(component.ViewID);
			if (isMute)
			{
				_mutedUsers.Add(hashUserId);
				GMEWWisePluginNative.GMEWWisePlugin_AddAudioBlockList(hashUserId);
			}
			else
			{
				_mutedUsers.Remove(hashUserId);
				GMEWWisePluginNative.GMEWWisePlugin_RemoveAudioBlockList(hashUserId);
			}
		}

		private string GetHashUserId(int userID)
		{
			return (userID + 10001).ToString();
		}

		private void SetStartSettings(string roomId)
		{
			StartTimer();
		}

		private void OnApplicationPauseChanged(bool pauseStatus)
		{
			if (pauseStatus)
			{
				Disconnect();
			}
		}

		private void OnMultiplayerDisconnected()
		{
			EnableDisableVoiceChat(value: false);
		}

		private void StartSpeaking()
		{
			if (_isJoinedInRoom)
			{
				AkSoundEngine.PostEvent(PlaySendGme, _headPerson);
				_isPressed = true;
			}
		}

		private void StopSpeaking()
		{
			if (_isPressed)
			{
				AkSoundEngine.PostEvent(StopSendGme, _headPerson);
				_isPressed = false;
			}
		}

		private void Connect()
		{
			if (_isJoinedInRoom)
			{
				return;
			}
			if ((bool)V.DriverPerson)
			{
				GMEWWisePluginNative.GMEWWisePlugin_ReceivePlugin_SetReceiveOpenIDWithGameObjectID((ulong)_headPerson.GetInstanceID(), _playerId);
			}
			AkSoundEngine.PostEvent(PlayReceiveGme, _headPerson);
			_isJoinedInRoom = true;
			foreach (string mutedUser in _mutedUsers)
			{
				GMEWWisePluginNative.GMEWWisePlugin_AddAudioBlockList(mutedUser);
			}
		}

		private void Disconnect()
		{
			if (!_isJoinedInRoom)
			{
				return;
			}
			AkSoundEngine.PostEvent(StopReceiveGme, _headPerson);
			AkSoundEngine.PostEvent(StopSendGme, _headPerson);
			foreach (MultiPersonControl speakingPlayer in SpeakingPlayers)
			{
				AkSoundEngine.PostEvent(StopReceiveGme, speakingPlayer.gameObject);
			}
			SpeakingPlayers.Clear();
			_isJoinedInRoom = false;
		}

		private void StartTimer()
		{
			_timerToCheckNearestPlayers?.Dispose();
			_timerToCheckNearestPlayers = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				CheckNearestOnlinePlayers();
			}).AddTo(V.DriverPerson);
		}

		private void CheckNearestOnlinePlayers()
		{
			float num = 0f;
			if (!FlagsStorage.OutOfCar && (bool)V.Car)
			{
				num = V.Car.GetComponent<Rigidbody>().velocity.magnitude;
			}
			foreach (MultiPersonControl item in _onlineProvider.PlayersOnlineNow.Where(F.Exists))
			{
				if ((bool)V.DriverPerson && (bool)item && Vector3.Distance(V.DriverPerson.transform.position, item.transform.position) < 70f)
				{
					if (num > 30f)
					{
						StartTimer();
						return;
					}
					Connect();
					StartTimer();
					return;
				}
			}
			if (_isJoinedInRoom)
			{
				Disconnect();
			}
			StartTimer();
		}

		private void CheckSpeakingPlayer()
		{
			MultiPersonControl[] playersOnlineNow = _onlineProvider.PlayersOnlineNow;
			foreach (MultiPersonControl onlinePlayer in playersOnlineNow)
			{
				if (!onlinePlayer)
				{
					continue;
				}
				if (GMEWWisePluginNative.GMEWWisePlugin_GetAudioRecvStreamLevel(GetHashUserId(onlinePlayer.photonView.ViewID)) > 2)
				{
					onlinePlayer.ResetSpeakerTimerDisposable?.Dispose();
					onlinePlayer.ResetSpeakerTimerDisposable = null;
					onlinePlayer.isSpeak = true;
					if (SpeakingPlayers.Count == 0)
					{
						AkSoundEngine.PostEvent(StopReceiveGme, _headPerson);
						AddPostEvent(onlinePlayer);
						break;
					}
					if (!SpeakingPlayers.Any((MultiPersonControl p) => p.photonView.ViewID == onlinePlayer.photonView.ViewID))
					{
						AddPostEvent(onlinePlayer);
					}
				}
				else if (onlinePlayer.ResetSpeakerTimerDisposable == null && onlinePlayer.isSpeak)
				{
					onlinePlayer.ResetSpeakerTimerDisposable = Observable.Timer(TimeSpan.FromSeconds(0.2)).SubscribeWithState(onlinePlayer, SetSpeakStatus).AddTo(onlinePlayer);
				}
			}
		}

		private void AddPostEvent(MultiPersonControl onlinePlayer)
		{
			string hashUserId = GetHashUserId(onlinePlayer.photonView.ViewID);
			GMEWWisePluginNative.GMEWWisePlugin_ReceivePlugin_SetReceiveOpenIDWithGameObjectID((ulong)onlinePlayer.gameObject.GetInstanceID(), hashUserId);
			SpeakingPlayers.Add(onlinePlayer);
			AkSoundEngine.PostEvent(PlayReceiveGme, onlinePlayer.gameObject);
		}

		private static void SetSpeakStatus(long _, MultiPersonControl targetPlayer)
		{
			targetPlayer.isSpeak = false;
		}
	}
}
