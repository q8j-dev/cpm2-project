using System;
using System.Collections.Generic;
using UnityEngine;

namespace ExitGames.Client.Photon.LoadBalancing
{
	public class LoadBalancingClient : IPhotonPeerListener
	{
		public LoadBalancingPeer loadBalancingPeer;

		public AuthModeOption AuthMode;

		public EncryptionMode EncryptionMode;

		private ConnectionProtocol ExpectedProtocol;

		public string NameServerHost = "ns.exitgames.com";

		public string NameServerHttp = "http://ns.exitgames.com:80/photon/n";

		private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>
		{
			{
				ConnectionProtocol.Udp,
				5058
			},
			{
				ConnectionProtocol.Tcp,
				4533
			},
			{
				ConnectionProtocol.WebSocket,
				9093
			},
			{
				ConnectionProtocol.WebSocketSecure,
				19093
			}
		};

		private ClientState state;

		private bool inLobby;

		private bool autoJoinLobby = true;

		public bool EnableLobbyStatistics;

		private List<TypedLobbyInfo> lobbyStatistics = new List<TypedLobbyInfo>();

		public Dictionary<string, RoomInfo> RoomInfoList = new Dictionary<string, RoomInfo>();

		public Room CurrentRoom;

		private JoinType lastJoinType;

		protected internal EnterRoomParams enterRoomParamsCache;

		private bool didAuthenticate;

		private string[] friendListRequested;

		private int friendListTimestamp;

		private bool isFetchingFriendList;

		private byte[] encryptionSecret;

		public string AppVersion { get; set; }

		public string AppId { get; set; }

		public AuthenticationValues AuthValues { get; set; }

		public ConnectionProtocol TransportProtocol
		{
			get
			{
				return loadBalancingPeer.TransportProtocol;
			}
			set
			{
				if (loadBalancingPeer == null || loadBalancingPeer.PeerState != PeerStateValue.Disconnected)
				{
					DebugReturn(DebugLevel.WARNING, "Can't set TransportProtocol. Disconnect first! " + ((loadBalancingPeer != null) ? ("PeerState: " + loadBalancingPeer.PeerState) : "loadBalancingPeer is null."));
				}
				else
				{
					loadBalancingPeer.TransportProtocol = value;
				}
			}
		}

		public Dictionary<ConnectionProtocol, Type> SocketImplementationConfig => loadBalancingPeer.SocketImplementationConfig;

		private string TokenForInit
		{
			get
			{
				if (AuthMode == AuthModeOption.Auth)
				{
					return null;
				}
				if (AuthValues == null)
				{
					return null;
				}
				return AuthValues.Token;
			}
		}

		public bool IsUsingNameServer { get; protected internal set; }

		public string NameServerAddress => GetNameServerAddress();

		public string CurrentServerAddress => loadBalancingPeer.ServerAddress;

		public string MasterServerAddress { get; protected internal set; }

		public string GameServerAddress { get; protected internal set; }

		public ServerConnection Server { get; private set; }

		public ClientState State
		{
			get
			{
				return state;
			}
			protected internal set
			{
				state = value;
				if (OnStateChangeAction != null)
				{
					OnStateChangeAction(state);
				}
			}
		}

		public bool IsConnected
		{
			get
			{
				if (loadBalancingPeer != null && State != ClientState.PeerCreated)
				{
					return State != ClientState.Disconnected;
				}
				return false;
			}
		}

		public bool IsConnectedAndReady
		{
			get
			{
				if (loadBalancingPeer == null)
				{
					return false;
				}
				switch (State)
				{
				case ClientState.PeerCreated:
				case ClientState.Authenticating:
				case ClientState.ConnectingToGameserver:
				case ClientState.Joining:
				case ClientState.Leaving:
				case ClientState.ConnectingToMasterserver:
				case ClientState.Disconnecting:
				case ClientState.Disconnected:
				case ClientState.ConnectingToNameServer:
					return false;
				default:
					return true;
				}
			}
		}

		public DisconnectCause DisconnectedCause { get; protected set; }

		public TypedLobby CurrentLobby { get; protected internal set; }

		public bool AutoJoinLobby
		{
			get
			{
				return autoJoinLobby;
			}
			set
			{
				autoJoinLobby = value;
			}
		}

		public List<TypedLobbyInfo> LobbyStatistics
		{
			get
			{
				return lobbyStatistics;
			}
			private set
			{
				lobbyStatistics = value;
			}
		}

		public Player LocalPlayer { get; internal set; }

		public string NickName
		{
			get
			{
				return LocalPlayer.NickName;
			}
			set
			{
				if (LocalPlayer != null)
				{
					LocalPlayer.NickName = value;
				}
			}
		}

		public string UserId
		{
			get
			{
				if (AuthValues != null)
				{
					return AuthValues.UserId;
				}
				return null;
			}
			set
			{
				if (AuthValues == null)
				{
					AuthValues = new AuthenticationValues();
				}
				AuthValues.UserId = value;
			}
		}

		public int PlayersOnMasterCount { get; internal set; }

		public int PlayersInRoomsCount { get; internal set; }

		public int RoomsCount { get; internal set; }

		public List<FriendInfo> FriendList { get; private set; }

		public int FriendListAge
		{
			get
			{
				if (!isFetchingFriendList && friendListTimestamp != 0)
				{
					return Environment.TickCount - friendListTimestamp;
				}
				return 0;
			}
		}

		protected bool IsAuthorizeSecretAvailable
		{
			get
			{
				if (AuthValues != null)
				{
					return !string.IsNullOrEmpty(AuthValues.Token);
				}
				return false;
			}
		}

		public string[] AvailableRegions { get; private set; }

		public string[] AvailableRegionsServers { get; private set; }

		public string CloudRegion { get; private set; }

		public event Action<ClientState> OnStateChangeAction;

		public event Action<EventData> OnEventAction;

		public event Action<OperationResponse> OnOpResponseAction;

		public LoadBalancingClient(ConnectionProtocol protocol = ConnectionProtocol.Udp)
		{
			loadBalancingPeer = new LoadBalancingPeer(this, protocol);
			LocalPlayer = CreatePlayer(string.Empty, -1, isLocal: true, null);
			State = ClientState.PeerCreated;
		}

		public LoadBalancingClient(string masterAddress, string appId, string gameVersion, ConnectionProtocol protocol = ConnectionProtocol.Udp)
			: this(protocol)
		{
			MasterServerAddress = masterAddress;
			AppId = appId;
			AppVersion = gameVersion;
		}

		private string GetNameServerAddress()
		{
			int value = 0;
			ProtocolToNameServerPort.TryGetValue(loadBalancingPeer.TransportProtocol, out value);
			switch (loadBalancingPeer.TransportProtocol)
			{
			case ConnectionProtocol.Udp:
			case ConnectionProtocol.Tcp:
				return $"{NameServerHost}:{value}";
			case ConnectionProtocol.WebSocket:
				return $"ws://{NameServerHost}:{value}";
			case ConnectionProtocol.WebSocketSecure:
				return $"wss://{NameServerHost}:{value}";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public bool Connect(string masterServerAddress, string appId, string appVersion, string nickName, AuthenticationValues authValues)
		{
			if (!string.IsNullOrEmpty(masterServerAddress))
			{
				MasterServerAddress = masterServerAddress;
			}
			if (!string.IsNullOrEmpty(appId))
			{
				AppId = appId;
			}
			if (!string.IsNullOrEmpty(appVersion))
			{
				AppVersion = appVersion;
			}
			if (!string.IsNullOrEmpty(nickName))
			{
				NickName = nickName;
			}
			AuthValues = authValues;
			return Connect();
		}

		public virtual bool Connect()
		{
			DisconnectedCause = DisconnectCause.None;
			if (loadBalancingPeer.Connect(MasterServerAddress, AppId, TokenForInit))
			{
				State = ClientState.ConnectingToMasterserver;
				return true;
			}
			return false;
		}

		public bool ConnectToNameServer()
		{
			IsUsingNameServer = true;
			CloudRegion = null;
			if (AuthMode == AuthModeOption.AuthOnceWss)
			{
				ExpectedProtocol = loadBalancingPeer.TransportProtocol;
				loadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
			}
			if (!loadBalancingPeer.Connect(NameServerAddress, "NameServer", TokenForInit))
			{
				return false;
			}
			State = ClientState.ConnectingToNameServer;
			return true;
		}

		public bool ConnectToRegionMaster(string region)
		{
			IsUsingNameServer = true;
			if (State == ClientState.ConnectedToNameServer)
			{
				CloudRegion = region;
				return CallAuthenticate();
			}
			loadBalancingPeer.Disconnect();
			CloudRegion = region;
			if (AuthMode == AuthModeOption.AuthOnceWss)
			{
				ExpectedProtocol = loadBalancingPeer.TransportProtocol;
				loadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
			}
			if (!loadBalancingPeer.Connect(NameServerAddress, "NameServer"))
			{
				return false;
			}
			State = ClientState.ConnectingToNameServer;
			return true;
		}

		public void Disconnect()
		{
			if (State != ClientState.Disconnected)
			{
				State = ClientState.Disconnecting;
				loadBalancingPeer.Disconnect();
			}
		}

		private bool CallAuthenticate()
		{
			if (AuthMode == AuthModeOption.Auth)
			{
				return loadBalancingPeer.OpAuthenticate(AppId, AppVersion, AuthValues, CloudRegion, EnableLobbyStatistics && Server == ServerConnection.MasterServer);
			}
			return loadBalancingPeer.OpAuthenticateOnce(AppId, AppVersion, AuthValues, CloudRegion, EncryptionMode, ExpectedProtocol);
		}

		public void Service()
		{
			if (loadBalancingPeer != null)
			{
				loadBalancingPeer.Service();
			}
		}

		private void DisconnectToReconnect()
		{
			switch (Server)
			{
			case ServerConnection.NameServer:
				State = ClientState.DisconnectingFromNameServer;
				break;
			case ServerConnection.MasterServer:
				State = ClientState.DisconnectingFromMasterserver;
				break;
			case ServerConnection.GameServer:
				State = ClientState.DisconnectingFromGameserver;
				break;
			}
			loadBalancingPeer.Disconnect();
		}

		private bool ConnectToGameServer()
		{
			if (loadBalancingPeer.Connect(GameServerAddress, AppId, TokenForInit))
			{
				State = ClientState.ConnectingToGameserver;
				return true;
			}
			return false;
		}

		public bool OpGetRegions()
		{
			if (Server != ServerConnection.NameServer)
			{
				return false;
			}
			bool num = loadBalancingPeer.OpGetRegions(AppId);
			if (num)
			{
				AvailableRegions = null;
			}
			return num;
		}

		public bool OpFindFriends(string[] friendsToFind)
		{
			if (loadBalancingPeer == null)
			{
				DebugReturn(DebugLevel.ERROR, "OpFindFriends aborted: loadBalancingPeer is null.");
				return false;
			}
			if (isFetchingFriendList || Server != ServerConnection.MasterServer)
			{
				DebugReturn(DebugLevel.WARNING, "OpFindFriends skipped: already fetching friends list.");
				return false;
			}
			if (friendsToFind == null || friendsToFind.Length == 0)
			{
				DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friendsToFind array is null or empty.");
				return false;
			}
			List<string> list = new List<string>(friendsToFind.Length);
			for (int i = 0; i < friendsToFind.Length; i++)
			{
				string text = friendsToFind[i];
				if (string.IsNullOrEmpty(text))
				{
					DebugReturn(DebugLevel.WARNING, $"friendsToFind array contains a null or empty UserId, element at position {i} skipped.");
				}
				else if (text.Equals(UserId))
				{
					DebugReturn(DebugLevel.WARNING, $"friendsToFind array contains local player's UserId \"{text}\", element at position {i} skipped.");
				}
				else if (list.Contains(text))
				{
					DebugReturn(DebugLevel.WARNING, $"friendsToFind array contains duplicate UserId \"{text}\", element at position {i} skipped.");
				}
				else
				{
					list.Add(text);
				}
			}
			if (list.Count == 0)
			{
				DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friends list to find is empty.");
				return false;
			}
			isFetchingFriendList = true;
			friendListRequested = list.ToArray();
			return loadBalancingPeer.OpFindFriends(friendsToFind);
		}

		public bool OpJoinLobby(TypedLobby lobby)
		{
			if (lobby == null)
			{
				lobby = TypedLobby.Default;
			}
			bool num = loadBalancingPeer.OpJoinLobby(lobby);
			if (num)
			{
				CurrentLobby = lobby;
			}
			return num;
		}

		public bool OpLeaveLobby()
		{
			return loadBalancingPeer.OpLeaveLobby();
		}

		public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, string[] expectedUsers = null)
		{
			return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, TypedLobby.Default, null, expectedUsers);
		}

		public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchmakingMode)
		{
			return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, matchmakingMode, TypedLobby.Default, null);
		}

		public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchmakingMode, TypedLobby lobby, string sqlLobbyFilter, string[] expectedUsers = null)
		{
			if (lobby == null)
			{
				lobby = TypedLobby.Default;
			}
			State = ClientState.Joining;
			lastJoinType = JoinType.JoinRandomRoom;
			CurrentLobby = lobby;
			enterRoomParamsCache = new EnterRoomParams();
			enterRoomParamsCache.Lobby = lobby;
			enterRoomParamsCache.ExpectedUsers = expectedUsers;
			OpJoinRandomRoomParams opJoinRandomRoomParams = new OpJoinRandomRoomParams();
			opJoinRandomRoomParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
			opJoinRandomRoomParams.ExpectedMaxPlayers = expectedMaxPlayers;
			opJoinRandomRoomParams.MatchingType = matchmakingMode;
			opJoinRandomRoomParams.TypedLobby = lobby;
			opJoinRandomRoomParams.SqlLobbyFilter = sqlLobbyFilter;
			opJoinRandomRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
		}

		public bool OpJoinRoom(string roomName, string[] expectedUsers = null)
		{
			State = ClientState.Joining;
			lastJoinType = JoinType.JoinRoom;
			bool onGameServer = Server == ServerConnection.GameServer;
			EnterRoomParams enterRoomParams = (enterRoomParamsCache = new EnterRoomParams());
			enterRoomParams.RoomName = roomName;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpJoinRoom(enterRoomParams);
		}

		public bool OpReJoinRoom(string roomName)
		{
			State = ClientState.Joining;
			lastJoinType = JoinType.JoinRoom;
			bool onGameServer = Server == ServerConnection.GameServer;
			EnterRoomParams enterRoomParams = (enterRoomParamsCache = new EnterRoomParams());
			enterRoomParams.RoomName = roomName;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.RejoinOnly = true;
			return loadBalancingPeer.OpJoinRoom(enterRoomParams);
		}

		public bool OpJoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, string[] expectedUsers = null)
		{
			State = ClientState.Joining;
			lastJoinType = JoinType.JoinOrCreateRoom;
			CurrentLobby = lobby;
			bool onGameServer = Server == ServerConnection.GameServer;
			EnterRoomParams enterRoomParams = (enterRoomParamsCache = new EnterRoomParams());
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = lobby;
			enterRoomParams.CreateIfNotExists = true;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpJoinRoom(enterRoomParams);
		}

		public bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, string[] expectedUsers = null)
		{
			State = ClientState.Joining;
			lastJoinType = JoinType.CreateRoom;
			CurrentLobby = lobby;
			bool onGameServer = Server == ServerConnection.GameServer;
			EnterRoomParams enterRoomParams = (enterRoomParamsCache = new EnterRoomParams());
			enterRoomParams.RoomName = roomName;
			enterRoomParams.RoomOptions = roomOptions;
			enterRoomParams.Lobby = lobby;
			enterRoomParams.OnGameServer = onGameServer;
			enterRoomParams.ExpectedUsers = expectedUsers;
			return loadBalancingPeer.OpCreateRoom(enterRoomParams);
		}

		public bool OpLeaveRoom()
		{
			return OpLeaveRoom(becomeInactive: false);
		}

		public bool OpLeaveRoom(bool becomeInactive, bool sendAuthCookie = false)
		{
			if (CurrentRoom == null || Server != ServerConnection.GameServer || State == ClientState.DisconnectingFromGameserver)
			{
				return false;
			}
			State = ClientState.Leaving;
			return loadBalancingPeer.OpLeaveRoom(becomeInactive, sendAuthCookie);
		}

		public bool OpGetGameList(TypedLobby typedLobby, string sqlLobbyFilter)
		{
			return loadBalancingPeer.OpGetGameList(typedLobby, sqlLobbyFilter);
		}

		public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (CurrentRoom == null)
			{
				if (expectedProperties == null && webFlags == null && LocalPlayer != null && LocalPlayer.ID == actorNr)
				{
					LocalPlayer.SetCustomProperties(propertiesToSet);
					return true;
				}
				if ((int)loadBalancingPeer.DebugOut >= 1)
				{
					DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. To use expectedProperties or webForward, you have to be in a room. State: " + State);
				}
				return false;
			}
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(propertiesToSet);
			return OpSetPropertiesOfActor(actorNr, hashtable, expectedProperties, webFlags);
		}

		[Obsolete("Use the overload with WebFlags.")]
		public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet, Hashtable expectedProperties, bool webForward)
		{
			return OpSetCustomPropertiesOfActor(actorNr, propertiesToSet, expectedProperties, webForward ? new WebFlags(1) : null);
		}

		protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (CurrentRoom == null)
			{
				if ((int)loadBalancingPeer.DebugOut >= 1)
				{
					DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfActor() failed because this client is not in a room currently. State: " + State);
				}
				return false;
			}
			if (expectedProperties == null || expectedProperties.Count == 0)
			{
				CurrentRoom.GetPlayer(actorNr)?.InternalCacheProperties(actorProperties);
			}
			return loadBalancingPeer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties, webFlags);
		}

		public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.MergeStringKeys(propertiesToSet);
			return OpSetPropertiesOfRoom(hashtable, expectedProperties, webFlags);
		}

		[Obsolete("Use the overload with WebFlags.")]
		public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties, bool webForward)
		{
			return OpSetCustomPropertiesOfRoom(propertiesToSet, expectedProperties, webForward ? new WebFlags(1) : null);
		}

		protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
		{
			if (CurrentRoom == null)
			{
				if ((int)loadBalancingPeer.DebugOut >= 1)
				{
					DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfRoom() failed because this client is not in a room currently. State: " + State);
				}
				return false;
			}
			if (expectedProperties == null || expectedProperties.Count == 0)
			{
				CurrentRoom.InternalCacheProperties(gameProperties);
			}
			return loadBalancingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, webFlags);
		}

		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
		{
			if (loadBalancingPeer == null)
			{
				return false;
			}
			return loadBalancingPeer.OpRaiseEvent(eventCode, customEventContent, raiseEventOptions, sendOptions);
		}

		[Obsolete("Use OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions) instead. Parameter 'sendReliable' moved to: RaiseEventOptions.SendOptions.")]
		public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
		{
			if (loadBalancingPeer == null)
			{
				return false;
			}
			return loadBalancingPeer.OpRaiseEvent(eventCode, customEventContent, raiseEventOptions, new SendOptions
			{
				Reliability = sendReliable,
				Channel = raiseEventOptions.SequenceChannel
			});
		}

		public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
		{
			if (loadBalancingPeer == null)
			{
				return false;
			}
			return loadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
		}

		private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
		{
			if (CurrentRoom != null && gameProperties != null)
			{
				CurrentRoom.InternalCacheProperties(gameProperties);
			}
			if (actorProperties == null || actorProperties.Count <= 0)
			{
				return;
			}
			if (targetActorNr > 0)
			{
				Player player = CurrentRoom.GetPlayer(targetActorNr);
				if (player != null)
				{
					Hashtable properties = ReadoutPropertiesForActorNr(actorProperties, targetActorNr);
					player.InternalCacheProperties(properties);
				}
				return;
			}
			foreach (object key in actorProperties.Keys)
			{
				int num = (int)key;
				Hashtable hashtable = (Hashtable)actorProperties[key];
				string actorName = (string)hashtable[byte.MaxValue];
				Player player2 = CurrentRoom.GetPlayer(num);
				if (player2 == null)
				{
					player2 = CreatePlayer(actorName, num, isLocal: false, hashtable);
					CurrentRoom.StorePlayer(player2);
				}
				player2.InternalCacheProperties(hashtable);
			}
		}

		private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
		{
			if (actorProperties.ContainsKey(actorNr))
			{
				return (Hashtable)actorProperties[actorNr];
			}
			return actorProperties;
		}

		protected internal void ChangeLocalID(int newID)
		{
			if (LocalPlayer == null)
			{
				DebugReturn(DebugLevel.WARNING, $"Local actor is null or not in mActors! mLocalActor: {LocalPlayer} mActors==null: {CurrentRoom.Players == null} newID: {newID}");
			}
			if (CurrentRoom == null)
			{
				LocalPlayer.ChangeLocalID(newID);
				LocalPlayer.RoomReference = null;
			}
			else
			{
				CurrentRoom.RemovePlayer(LocalPlayer);
				LocalPlayer.ChangeLocalID(newID);
				CurrentRoom.StorePlayer(LocalPlayer);
			}
		}

		private void CleanCachedValues()
		{
			ChangeLocalID(-1);
			isFetchingFriendList = false;
			if (Server == ServerConnection.GameServer || State == ClientState.Disconnecting || State == ClientState.PeerCreated)
			{
				CurrentRoom = null;
			}
			if (Server == ServerConnection.MasterServer || State == ClientState.Disconnecting || State == ClientState.PeerCreated)
			{
				RoomInfoList.Clear();
			}
		}

		private void GameEnteredOnGameServer(OperationResponse operationResponse)
		{
			CurrentRoom = CreateRoom(enterRoomParamsCache.RoomName, enterRoomParamsCache.RoomOptions);
			CurrentRoom.LoadBalancingClient = this;
			CurrentRoom.IsLocalClientInside = true;
			int newID = (int)operationResponse[254];
			ChangeLocalID(newID);
			if (operationResponse.Parameters.ContainsKey(252))
			{
				int[] actorsInGame = (int[])operationResponse.Parameters[252];
				UpdatedActorList(actorsInGame);
			}
			Hashtable actorProperties = (Hashtable)operationResponse[249];
			Hashtable gameProperties = (Hashtable)operationResponse[248];
			ReadoutProperties(gameProperties, actorProperties, 0);
			State = ClientState.Joined;
			byte operationCode = operationResponse.OperationCode;
			if ((uint)(operationCode - 225) > 1u)
			{
				_ = 227;
			}
		}

		private void UpdatedActorList(int[] actorsInGame)
		{
			if (actorsInGame == null)
			{
				return;
			}
			foreach (int num in actorsInGame)
			{
				if (CurrentRoom.GetPlayer(num) == null)
				{
					CurrentRoom.StorePlayer(CreatePlayer(string.Empty, num, isLocal: false, null));
				}
			}
		}

		protected internal virtual Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
		{
			return new Player(actorName, actorNumber, isLocal, actorProperties);
		}

		protected internal virtual Room CreateRoom(string roomName, RoomOptions opt)
		{
			return new Room(roomName, opt);
		}

		public virtual void DebugReturn(DebugLevel level, string message)
		{
			if (loadBalancingPeer.DebugOut == DebugLevel.ALL || (int)level <= (int)loadBalancingPeer.DebugOut)
			{
				switch (level)
				{
				case DebugLevel.ERROR:
					Debug.LogError(message);
					break;
				case DebugLevel.WARNING:
					Debug.LogWarning(message);
					break;
				case DebugLevel.INFO:
					Debug.Log(message);
					break;
				case DebugLevel.ALL:
					Debug.Log(message);
					break;
				}
			}
		}

		public virtual void OnOperationResponse(OperationResponse operationResponse)
		{
			if (operationResponse.Parameters.ContainsKey(221))
			{
				if (AuthValues == null)
				{
					AuthValues = new AuthenticationValues();
				}
				AuthValues.Token = operationResponse[221] as string;
			}
			switch (operationResponse.OperationCode)
			{
			case 230:
			case 231:
				if (operationResponse.ReturnCode != 0)
				{
					DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull() + " Server: " + Server.ToString() + " Address: " + loadBalancingPeer.ServerAddress);
					switch (operationResponse.ReturnCode)
					{
					case short.MaxValue:
						DisconnectedCause = DisconnectCause.InvalidAuthentication;
						break;
					case 32755:
						DisconnectedCause = DisconnectCause.CustomAuthenticationFailed;
						break;
					case 32756:
						DisconnectedCause = DisconnectCause.InvalidRegion;
						break;
					case 32757:
						DisconnectedCause = DisconnectCause.MaxCcuReached;
						break;
					case -3:
						DisconnectedCause = DisconnectCause.OperationNotAllowedInCurrentState;
						break;
					}
					State = ClientState.Disconnecting;
					Disconnect();
					break;
				}
				if (Server == ServerConnection.NameServer || Server == ServerConnection.MasterServer)
				{
					if (operationResponse.Parameters.ContainsKey(225))
					{
						string text2 = (string)operationResponse.Parameters[225];
						if (!string.IsNullOrEmpty(text2))
						{
							UserId = text2;
							DebugReturn(DebugLevel.INFO, $"Received your UserID from server. Updating local value to: {UserId}");
						}
					}
					if (operationResponse.Parameters.ContainsKey(202))
					{
						NickName = (string)operationResponse.Parameters[202];
						DebugReturn(DebugLevel.INFO, $"Received your NickName from server. Updating local value to: {NickName}");
					}
					if (operationResponse.Parameters.ContainsKey(192))
					{
						SetupEncryption((Dictionary<byte, object>)operationResponse.Parameters[192]);
					}
				}
				if (Server == ServerConnection.NameServer)
				{
					MasterServerAddress = operationResponse[230] as string;
					if (AuthMode == AuthModeOption.AuthOnceWss)
					{
						DebugReturn(DebugLevel.INFO, $"Due to AuthOnceWss, switching TransportProtocol to ExpectedProtocol: {ExpectedProtocol}.");
						loadBalancingPeer.TransportProtocol = ExpectedProtocol;
					}
					DisconnectToReconnect();
				}
				else if (Server == ServerConnection.MasterServer)
				{
					State = ClientState.ConnectedToMasterserver;
					if (AuthMode != AuthModeOption.Auth)
					{
						loadBalancingPeer.OpSettings(EnableLobbyStatistics);
					}
					if (AutoJoinLobby)
					{
						loadBalancingPeer.OpJoinLobby(CurrentLobby);
					}
				}
				else if (Server == ServerConnection.GameServer)
				{
					State = ClientState.Joining;
					enterRoomParamsCache.PlayerProperties = LocalPlayer.AllProperties;
					enterRoomParamsCache.OnGameServer = true;
					if (lastJoinType == JoinType.JoinRoom || lastJoinType == JoinType.JoinRandomRoom || lastJoinType == JoinType.JoinOrCreateRoom)
					{
						loadBalancingPeer.OpJoinRoom(enterRoomParamsCache);
					}
					else if (lastJoinType == JoinType.CreateRoom)
					{
						loadBalancingPeer.OpCreateRoom(enterRoomParamsCache);
					}
				}
				break;
			case 220:
				AvailableRegions = operationResponse[210] as string[];
				AvailableRegionsServers = operationResponse[230] as string[];
				break;
			case 225:
			case 226:
			case 227:
			{
				if (operationResponse.ReturnCode != 0)
				{
					if (Server == ServerConnection.GameServer)
					{
						DisconnectToReconnect();
					}
					else
					{
						State = (inLobby ? ClientState.JoinedLobby : ClientState.ConnectedToMasterserver);
					}
					break;
				}
				if (Server == ServerConnection.GameServer)
				{
					GameEnteredOnGameServer(operationResponse);
					break;
				}
				GameServerAddress = (string)operationResponse[230];
				string text3 = operationResponse[byte.MaxValue] as string;
				if (!string.IsNullOrEmpty(text3))
				{
					enterRoomParamsCache.RoomName = text3;
				}
				DisconnectToReconnect();
				break;
			}
			case 217:
			{
				if (operationResponse.ReturnCode != 0)
				{
					DebugReturn(DebugLevel.ERROR, "GetGameList failed: " + operationResponse.ToStringFull());
					break;
				}
				RoomInfoList = new Dictionary<string, RoomInfo>();
				Hashtable hashtable = (Hashtable)operationResponse[222];
				foreach (string key in hashtable.Keys)
				{
					RoomInfo value = new RoomInfo(key, (Hashtable)hashtable[key]);
					RoomInfoList[key] = value;
				}
				break;
			}
			case 229:
				State = ClientState.JoinedLobby;
				inLobby = true;
				break;
			case 228:
				State = ClientState.ConnectedToMasterserver;
				inLobby = false;
				break;
			case 254:
				DisconnectToReconnect();
				break;
			case 222:
			{
				if (operationResponse.ReturnCode != 0)
				{
					DebugReturn(DebugLevel.ERROR, "OpFindFriends failed: " + operationResponse.ToStringFull());
					isFetchingFriendList = false;
					break;
				}
				bool[] array = operationResponse[1] as bool[];
				string[] array2 = operationResponse[2] as string[];
				List<FriendInfo> list = new List<FriendInfo>(friendListRequested.Length);
				for (int i = 0; i < friendListRequested.Length; i++)
				{
					FriendInfo friendInfo = new FriendInfo();
					friendInfo.UserId = friendListRequested[i];
					friendInfo.Room = array2[i];
					friendInfo.IsOnline = array[i];
					list.Insert(i, friendInfo);
				}
				FriendList = list;
				friendListRequested = null;
				isFetchingFriendList = false;
				friendListTimestamp = Environment.TickCount;
				if (friendListTimestamp == 0)
				{
					friendListTimestamp = 1;
				}
				break;
			}
			}
			if (OnOpResponseAction != null)
			{
				OnOpResponseAction(operationResponse);
			}
		}

		public virtual void OnStatusChanged(StatusCode statusCode)
		{
			switch (statusCode)
			{
			case StatusCode.Connect:
				inLobby = false;
				if (State == ClientState.ConnectingToNameServer)
				{
					if ((int)loadBalancingPeer.DebugOut >= 5)
					{
						DebugReturn(DebugLevel.ALL, "Connected to nameserver.");
					}
					Server = ServerConnection.NameServer;
					if (AuthValues != null)
					{
						AuthValues.Token = null;
					}
				}
				if (State == ClientState.ConnectingToGameserver)
				{
					if ((int)loadBalancingPeer.DebugOut >= 5)
					{
						DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
					}
					Server = ServerConnection.GameServer;
				}
				if (State == ClientState.ConnectingToMasterserver)
				{
					if ((int)loadBalancingPeer.DebugOut >= 5)
					{
						DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
					}
					Server = ServerConnection.MasterServer;
				}
				if (loadBalancingPeer.TransportProtocol != ConnectionProtocol.WebSocketSecure)
				{
					if (Server == ServerConnection.NameServer || AuthMode == AuthModeOption.Auth)
					{
						loadBalancingPeer.EstablishEncryption();
					}
					break;
				}
				goto case StatusCode.EncryptionEstablished;
			case StatusCode.EncryptionEstablished:
				if (Server == ServerConnection.NameServer)
				{
					State = ClientState.ConnectedToNameServer;
				}
				if ((Server == ServerConnection.NameServer || (AuthMode != AuthModeOption.AuthOnce && AuthMode != AuthModeOption.AuthOnceWss)) && !didAuthenticate && (!IsUsingNameServer || CloudRegion != null))
				{
					didAuthenticate = CallAuthenticate();
					if (didAuthenticate)
					{
						State = ClientState.Authenticating;
					}
					else
					{
						DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + State);
					}
				}
				break;
			case StatusCode.Disconnect:
				CleanCachedValues();
				didAuthenticate = false;
				inLobby = false;
				switch (State)
				{
				case ClientState.PeerCreated:
				case ClientState.Disconnecting:
					if (AuthValues != null)
					{
						AuthValues.Token = null;
					}
					State = ClientState.Disconnected;
					break;
				case ClientState.DisconnectingFromGameserver:
				case ClientState.DisconnectingFromNameServer:
					Connect();
					break;
				case ClientState.DisconnectingFromMasterserver:
					ConnectToGameServer();
					break;
				default:
				{
					string text = "";
					DebugReturn(DebugLevel.WARNING, "Got a unexpected Disconnect in LoadBalancingClient State: " + State.ToString() + ". Server: " + Server.ToString() + " Trace: " + text);
					if (AuthValues != null)
					{
						AuthValues.Token = null;
					}
					State = ClientState.Disconnected;
					break;
				}
				}
				break;
			case StatusCode.DisconnectByServerUserLimit:
				DebugReturn(DebugLevel.ERROR, "The Photon license's CCU Limit was reached. Server rejected this connection. Wait and re-try.");
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.DisconnectByServerUserLimit;
				State = ClientState.Disconnected;
				break;
			case StatusCode.SecurityExceptionOnConnect:
			case StatusCode.ExceptionOnConnect:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.ExceptionOnConnect;
				State = ClientState.Disconnected;
				break;
			case StatusCode.DisconnectByServerTimeout:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.DisconnectByServer;
				State = ClientState.Disconnected;
				break;
			case StatusCode.DisconnectByServerLogic:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.DisconnectByServerLogic;
				State = ClientState.Disconnected;
				break;
			case StatusCode.TimeoutDisconnect:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.TimeoutDisconnect;
				State = ClientState.Disconnected;
				break;
			case StatusCode.Exception:
			case StatusCode.ExceptionOnReceive:
				if (AuthValues != null)
				{
					AuthValues.Token = null;
				}
				DisconnectedCause = DisconnectCause.Exception;
				State = ClientState.Disconnected;
				break;
			}
		}

		public virtual void OnEvent(EventData photonEvent)
		{
			int num = 0;
			Player player = null;
			if (photonEvent.Parameters.ContainsKey(254))
			{
				num = (int)photonEvent[254];
				if (CurrentRoom != null)
				{
					player = CurrentRoom.GetPlayer(num);
				}
			}
			switch (photonEvent.Code)
			{
			case 229:
			case 230:
			{
				if (photonEvent.Code == 230)
				{
					RoomInfoList = new Dictionary<string, RoomInfo>();
				}
				Hashtable hashtable2 = (Hashtable)photonEvent[222];
				foreach (string key in hashtable2.Keys)
				{
					RoomInfo roomInfo = new RoomInfo(key, (Hashtable)hashtable2[key]);
					if (roomInfo.removedFromList)
					{
						RoomInfoList.Remove(key);
					}
					else
					{
						RoomInfoList[key] = roomInfo;
					}
				}
				break;
			}
			case byte.MaxValue:
			{
				Hashtable hashtable = (Hashtable)photonEvent[249];
				if (player == null)
				{
					Player player2 = CreatePlayer(string.Empty, num, isLocal: false, hashtable);
					CurrentRoom.StorePlayer(player2);
				}
				else
				{
					player.InternalCacheProperties(hashtable);
					player.IsInactive = false;
				}
				if (num == LocalPlayer.ID)
				{
					int[] actorsInGame = (int[])photonEvent[252];
					UpdatedActorList(actorsInGame);
				}
				break;
			}
			case 254:
			{
				bool flag = false;
				if (photonEvent.Parameters.ContainsKey(233))
				{
					flag = (bool)photonEvent.Parameters[233];
				}
				if (flag)
				{
					player.IsInactive = true;
				}
				else
				{
					CurrentRoom.RemovePlayer(num);
				}
				if (photonEvent.Parameters.ContainsKey(203))
				{
					int num2 = (int)photonEvent[203];
					if (num2 != 0)
					{
						CurrentRoom.masterClientId = num2;
					}
				}
				break;
			}
			case 253:
			{
				int num3 = 0;
				if (photonEvent.Parameters.ContainsKey(253))
				{
					num3 = (int)photonEvent[253];
				}
				Hashtable gameProperties = null;
				Hashtable actorProperties = null;
				if (num3 == 0)
				{
					gameProperties = (Hashtable)photonEvent[251];
				}
				else
				{
					actorProperties = (Hashtable)photonEvent[251];
				}
				ReadoutProperties(gameProperties, actorProperties, num3);
				break;
			}
			case 226:
				PlayersInRoomsCount = (int)photonEvent[229];
				RoomsCount = (int)photonEvent[228];
				PlayersOnMasterCount = (int)photonEvent[227];
				break;
			case 224:
			{
				string[] array = photonEvent[213] as string[];
				byte[] array2 = photonEvent[212] as byte[];
				int[] array3 = photonEvent[229] as int[];
				int[] array4 = photonEvent[228] as int[];
				lobbyStatistics.Clear();
				for (int i = 0; i < array.Length; i++)
				{
					TypedLobbyInfo typedLobbyInfo = new TypedLobbyInfo();
					typedLobbyInfo.Name = array[i];
					typedLobbyInfo.Type = (LobbyType)array2[i];
					typedLobbyInfo.PlayerCount = array3[i];
					typedLobbyInfo.RoomCount = array4[i];
					lobbyStatistics.Add(typedLobbyInfo);
				}
				break;
			}
			case 251:
				if (OnEventAction != null)
				{
					OnEventAction(photonEvent);
				}
				break;
			case 223:
				if (AuthValues == null)
				{
					AuthValues = new AuthenticationValues();
				}
				AuthValues.Token = photonEvent[221] as string;
				break;
			}
			if (OnEventAction != null)
			{
				OnEventAction(photonEvent);
			}
		}

		public virtual void OnMessage(object message)
		{
			DebugReturn(DebugLevel.ALL, $"got OnMessage {message}");
		}

		private void SetupEncryption(Dictionary<byte, object> encryptionData)
		{
			EncryptionMode encryptionMode = (EncryptionMode)(byte)encryptionData[0];
			switch (encryptionMode)
			{
			case EncryptionMode.PayloadEncryption:
			{
				byte[] secret = (byte[])encryptionData[1];
				loadBalancingPeer.InitPayloadEncryption(secret);
				break;
			}
			case EncryptionMode.DatagramEncryption:
			case EncryptionMode.DatagramEncryptionRandomSequence:
			{
				byte[] array = (byte[])encryptionData[1];
				byte[] hmacSecret = (byte[])encryptionData[2];
				loadBalancingPeer.InitDatagramEncryption(array, hmacSecret, encryptionMode == EncryptionMode.DatagramEncryptionRandomSequence);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public bool OpWebRpc(string uriPath, object parameters, bool sendAuthCookie = false)
		{
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(209, uriPath);
			dictionary.Add(208, parameters);
			if (sendAuthCookie)
			{
				dictionary.Add(234, (byte)2);
			}
			return loadBalancingPeer.SendOperation(219, dictionary, new SendOptions
			{
				Reliability = true
			});
		}
	}
}
