using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.LanguagesServers;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Multiplayer
{
	public class MultiplayerLobbyHandler : MonoBehaviourPunCallbacks
	{
		private List<FriendInfo> _friendListArray;

		private RoomInfo[] _roomListOld;

		private bool _friendListReceived;

		private bool _friendRequestSend;

		private List<string> _lastInts = new List<string>();

		private PlayerDataStorage _playerDataStorage;

		private PlayerDatas.FriendsUtility _friendsUtility;

		private ManagerUI _managerUI;

		private ITimerTuningService _timerTuningService;

		private FriendsListItemUI _friendsListItem;

		private readonly RoomJoinWatchdog _joinWatchdog = new RoomJoinWatchdog();

		private IDisposable _friendsRequestDelay;

		private IDisposable _friendsRequestRepeat;

		private IDisposable _updateListDelay;

		private IDisposable _updateListRepeat;

		private PhotonReconnectionHandler _photonReconnectionHandler;

		public bool ShowPasswordInput;

		[SerializeField]
		private LobbyWindow LobbyWindow = default;

		private IAccountController _accountController;

		private IAutoSaveService _autoSaveService;

		private IRemoteConfigManager _remoteConfigManager;

		private ArenaFlowController _arenaFlowController;

		[Inject]
		public void Construct(PlayerDatas.FriendsUtility friendsUtility, PlayerDataStorage playerDataStorage, ManagerUI managerUI, ITimerTuningService timerTuningService, CoreGameManager coreGameManager, PhotonReconnectionHandler photonReconnectionHandler, IAccountController accountController, IAutoSaveService autoSaveService, IRemoteConfigManager remoteConfigManager, ArenaFlowController arenaFlowController)
		{
			_arenaFlowController = arenaFlowController;
			_remoteConfigManager = remoteConfigManager;
			_autoSaveService = autoSaveService;
			_accountController = accountController;
			_photonReconnectionHandler = photonReconnectionHandler;
			_friendsUtility = friendsUtility;
			_timerTuningService = timerTuningService;
			_managerUI = managerUI;
			_playerDataStorage = playerDataStorage;
		}

		private void Start()
		{
			ShowPasswordInput = false;
			OpenMultiPlayerMenu().Forget();
			LobbyWindow lobbyWindow = LobbyWindow;
			lobbyWindow.OnMultiplayerStarted = (Action<SpawnPlaceItemUI>)Delegate.Combine(lobbyWindow.OnMultiplayerStarted, new Action<SpawnPlaceItemUI>(StartMultiplayer));
			LobbyWindow lobbyWindow2 = LobbyWindow;
			lobbyWindow2.OnNewFriendAdded = (Action<string>)Delegate.Combine(lobbyWindow2.OnNewFriendAdded, new Action<string>(AddFriendById));
			TryEnablePrivacy().Forget();
		}

		private void OnDestroy()
		{
			LobbyWindow lobbyWindow = LobbyWindow;
			lobbyWindow.OnMultiplayerStarted = (Action<SpawnPlaceItemUI>)Delegate.Remove(lobbyWindow.OnMultiplayerStarted, new Action<SpawnPlaceItemUI>(StartMultiplayer));
			LobbyWindow lobbyWindow2 = LobbyWindow;
			lobbyWindow2.OnNewFriendAdded = (Action<string>)Delegate.Remove(lobbyWindow2.OnNewFriendAdded, new Action<string>(AddFriendById));
		}

		private void StartMultiplayer(SpawnPlaceItemUI chosenSpawnPlace)
		{
			if (_timerTuningService.IsCarBlocked(V.SlotID))
			{
				LobbyWindow.ShowUI(state: true);
				_managerUI.ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Yourcarcurrentlyisinservice"), 3f).Forget();
				return;
			}
			FlagsStorage.IsMultiFree = true;
			V.PlaceToSpawn = chosenSpawnPlace.SpawnPlace;
			StartFreeDrive();
			PlayerPrefs.SetInt("NewPlaceIconMulti" + chosenSpawnPlace.SpawnPlace, 1);
		}

		public void UpdateListFriendUI()
		{
			if (ShowPasswordInput || PhotonNetwork.NetworkClientState == ClientState.ConnectingToMasterServer || _friendsListItem != null)
			{
				return;
			}
			if (!PhotonNetwork.IsConnected)
			{
				_photonReconnectionHandler.ReconnectHard(PhotonConnectionProfile.Normal);
			}
			LobbyWindow.ShowLoading();
			LobbyWindow.ClearFriendsList();
			if (_friendsUtility.Friends.Count == 0)
			{
				LobbyWindow.ShowLoading();
			}
			else
			{
				if (!_friendListReceived || !PhotonNetwork.IsConnected)
				{
					return;
				}
				List<PrefFriendData> list = new List<PrefFriendData>();
				if (_friendListArray != null)
				{
					foreach (FriendInfo item in _friendListArray)
					{
						PrefFriendData prefFriendData = new PrefFriendData();
						prefFriendData.accountID = item.UserId;
						if (item.IsOnline)
						{
							prefFriendData.Status = "online";
						}
						if (item.IsInRoom)
						{
							prefFriendData.Status = "inroom";
							prefFriendData.RoomName = item.Room;
						}
						if (item.IsOnline || item.IsInRoom)
						{
							list.Add(prefFriendData);
						}
					}
				}
				foreach (PrefFriendData friend in _friendsUtility.Friends)
				{
					bool flag = false;
					foreach (PrefFriendData item2 in list)
					{
						flag = false;
						string text = friend.accountID;
						if (text == "")
						{
							text = friend.id;
						}
						if (text == item2.accountID)
						{
							flag = true;
							if (friend.Name != "")
							{
								item2.Name = friend.Name;
							}
							else if (friend.id != "")
							{
								item2.Name = friend.id.ToString();
							}
							break;
						}
						if (flag)
						{
							continue;
						}
						text = friend.id.ToString();
						if (text == item2.accountID)
						{
							flag = true;
							if (friend.Name != "")
							{
								item2.Name = friend.Name;
							}
							else if (friend.id != "")
							{
								item2.Name = friend.id.ToString();
							}
							item2.id = text;
						}
					}
					if (!flag)
					{
						list.Add(friend);
					}
				}
				LobbyWindow.UpdateFriendsList(list);
				LobbyWindow.HideLoading();
			}
		}

		public void TryToJoinRoom(FriendsListItemUI friendsListItem)
		{
			if (_joinWatchdog.IsWaiting)
			{
				return;
			}
			if (ArenaRoomNames.TryParseArenaId(friendsListItem.RoomNameToJoin, out var arenaId))
			{
				friendsListItem.SetText(new LocalizedString("MultiplayerLobbyHandler.Connecting"));
				LobbyWindow.ShowUI(state: false);
				if (!_arenaFlowController.JoinArenaRoom(arenaId, friendsListItem.RoomNameToJoin, ArenaEntrySource.JoinFriend))
				{
					LobbyWindow.ShowUI(state: true);
					friendsListItem.SetText(new LocalizedString("MultiplayerLobbyHandler.RoomIsFull"));
				}
				return;
			}
			FlagsStorage.IsPrivateRoom = true;
			_friendsListItem = friendsListItem;
			if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.JoinRoom(_friendsListItem.RoomNameToJoin))
			{
				_friendsListItem = null;
				friendsListItem.SetText(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"));
				_managerUI.ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f, NotificationType.Error).Forget();
			}
			else
			{
				_joinWatchdog.Start(Time.unscaledTime);
				_friendsListItem.SetText(new LocalizedString("MultiplayerLobbyHandler.Connecting"));
				LobbyWindow.ShowUI(state: false);
			}
		}

		private async UniTaskVoid OpenMultiPlayerMenu()
		{
			_friendsUtility.EnsureFriendsLoaded();
			if (!InternetHelper.HasInternetConnection())
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f).Forget();
			}
			else if (_autoSaveService.TrySendSaveData(AutoSaveService.AutoSaveFrom.MultiPlayerMenu) || !(await _accountController.CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType.MultiplayerMenu)))
			{
				_photonReconnectionHandler.ReconnectHard(PhotonConnectionProfile.Normal);
				_friendRequestSend = false;
				_friendListReceived = false;
				_friendListArray = null;
				LobbyWindow.ClearFriendsList();
				V.PlaceToSpawn = V.placeToSpawn.MainCity;
				V.RoomNameToJoin = "";
				if (_friendsUtility.Friends.Count != 0)
				{
					LobbyWindow.ShowLoading();
				}
				_friendsListItem = null;
			}
		}

		private void RequestFriendListFromPhoton()
		{
			if (!PhotonNetwork.IsConnectedAndReady)
			{
				return;
			}
			LoadBalancingClient networkingClient = PhotonNetwork.NetworkingClient;
			if (networkingClient != null && networkingClient.Server == ServerConnection.MasterServer)
			{
				string[] friendsToFind = _friendsUtility.Friends.Select((PrefFriendData n) => n.id).ToArray();
				if (_friendsUtility.Friends.Count > 0 && !_friendRequestSend)
				{
					PhotonNetwork.FindFriends(friendsToFind);
					_friendRequestSend = true;
				}
			}
		}

		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			_joinWatchdog.Stop();
			if ((bool)_friendsListItem)
			{
				_friendsListItem.SetText(new LocalizedString("MultiplayerLobbyHandler.RoomIsFull"));
				_friendsListItem = null;
			}
			LobbyWindow.ShowUI(state: true);
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			if (!ApplicationShutdown.IsQuitting && _joinWatchdog.IsWaiting)
			{
				_joinWatchdog.Stop();
				Debug.LogWarning($"[Lobby] Disconnected while joining a room: {cause}");
				AbortJoin();
			}
		}

		private void Update()
		{
			if (_joinWatchdog.CheckExpired(Time.unscaledTime))
			{
				Debug.LogWarning("[Lobby] Вход в комнату не ответил за " + $"{20f:0} с (state={PhotonNetwork.NetworkClientState}, " + $"server={PhotonNetwork.NetworkingClient?.Server}) — возвращаю меню");
				if (PhotonNetwork.IsConnected)
				{
					PhotonNetwork.Disconnect();
				}
				AbortJoin();
			}
		}

		private void AbortJoin()
		{
			_joinWatchdog.Stop();
			if ((bool)_friendsListItem)
			{
				_friendsListItem.SetText(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"));
				_friendsListItem = null;
			}
			LobbyWindow.HideLoading();
			LobbyWindow.ShowUI(state: true);
			_managerUI.ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 3f, NotificationType.Error).Forget();
		}

		public override void OnJoinedRoom()
		{
			if (!_joinWatchdog.IsWaiting)
			{
				_friendsListItem = null;
				return;
			}
			_joinWatchdog.Stop();
			if ((bool)_friendsListItem)
			{
				_friendsListItem.SetText(new LocalizedString("LevelLoader.Loading"));
			}
			string roomNameToJoin = ((PhotonNetwork.CurrentRoom != null) ? PhotonNetwork.CurrentRoom.Name : string.Empty);
			object obj = PhotonNetwork.CurrentRoom?.CustomProperties["S"];
			if (obj != null)
			{
				FlagsStorage.CreateForFriendOnly = false;
				V.RoomNameToJoin = roomNameToJoin;
				V.CoreGameManager.menuControl.ShowLoadingAndLoadMultiFreeDrive();
				V.PlaceToSpawn = (V.placeToSpawn)(int)obj;
			}
			else
			{
				_friendsListItem = null;
				LobbyWindow.ShowUI(state: true);
				UpdateListFriendUI();
			}
			_friendsListItem = null;
		}

		private void AddFriendById(string friendId)
		{
			friendId = friendId.Trim().ToUpper();
			if (_friendsUtility.CanAddFriend() && friendId.Length > 3 && (ObscuredString)friendId != _playerDataStorage.PlayerID && !_friendsUtility.HasFriendWithID(friendId))
			{
				_friendsUtility.AddFriend(friendId, "", "");
				StartRequestFriendList();
			}
			UpdateListFriendUI();
		}

		public override void OnFriendListUpdate(List<FriendInfo> friendInfos)
		{
			if (friendInfos != null && friendInfos.Count > 0)
			{
				_friendListArray = friendInfos;
				_friendListReceived = true;
				_friendRequestSend = false;
				UpdateListFriendUI();
			}
		}

		public override void OnRoomListUpdate(List<RoomInfo> roomInfos)
		{
			_roomListOld = roomInfos.ToArray();
		}

		public override void OnJoinedLobby()
		{
		}

		public override void OnConnectedToMaster()
		{
			Debug.Log("[MP-Diag] region=" + PhotonNetwork.CloudRegion + " appVersion=" + PhotonNetwork.NetworkingClient?.AppVersion + " " + $"playersTotal={PhotonNetwork.CountOfPlayers} rooms={PhotonNetwork.CountOfRooms} " + $"inRooms={PhotonNetwork.CountOfPlayersInRooms} onMaster={PhotonNetwork.CountOfPlayersOnMaster}");
			LobbyWindow.HideConnectingPanel();
			LobbyWindow.UpdateOnlinePlayersCount();
			if (_friendsUtility.Friends.Count == 0)
			{
				LobbyWindow.HideLoading();
			}
			else
			{
				StartRequestFriendList();
			}
			_updateListDelay?.Dispose();
			_updateListRepeat?.Dispose();
			_updateListDelay = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				UpdateListFriendUI();
				_updateListRepeat = Observable.Timer(TimeSpan.FromSeconds(6.0)).Repeat().Subscribe((long num) =>
				{
					UpdateListFriendUI();
				})
					.AddTo(this);
			}).AddTo(this);
		}

		private void StartRequestFriendList()
		{
			_friendsRequestDelay?.Dispose();
			_friendsRequestRepeat?.Dispose();
			_friendsRequestDelay = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				RequestFriendListFromPhoton();
				_friendsRequestRepeat = Observable.Timer(TimeSpan.FromSeconds(15.0)).Repeat().Subscribe((long num) =>
				{
					RequestFriendListFromPhoton();
				})
					.AddTo(this);
			}).AddTo(this);
		}

		private void StartFreeDrive()
		{
			FlagsStorage.CreateForFriendOnly = (FlagsStorage.IsPrivateRoom = LobbyWindow.IsForFriendOnly());
			if (FlagsStorage.CreateForFriendOnly)
			{
				V.RoomPassword = LobbyWindow.GetLobbyPassword();
			}
			V.RoomNameToJoin = "";
			V.CoreGameManager.menuControl.ShowLoadingAndLoadMultiFreeDrive();
		}

		public void SetLanguage(LanguageIconsProvider.CpmLanguages language)
		{
			PlayerPrefs.SetInt("languageIndex", (int)language);
			OpenMultiPlayerMenu().Forget();
		}

		private async UniTaskVoid TryEnablePrivacy()
		{
			LobbyWindow.OnlinePlayerWarningButton.SetActive(active: false);
			ConfigValuesStorage configValuesStorage = _remoteConfigManager.GetDefaultConfigStorage();
			try
			{
				configValuesStorage = await _remoteConfigManager.GetConfigValuesStorage().Timeout(TimeSpan.FromSeconds(5.0));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			_ = configValuesStorage.parentalControlStatus;
		}
	}
}
