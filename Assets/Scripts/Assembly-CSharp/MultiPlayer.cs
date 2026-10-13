using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using CPM.Scripts.Plugins.Multiplayer.Player;
using CPMEngine;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using GME_voice_chat;
using Photon.Pun;
using Photon.Realtime;
using RootMotion.FinalIK;
using Service;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Car.Sounds;
using _Car_Parking.Scripts.Connection;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.Multiplayer.RpcMethods;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Other.FsoStrobeLights;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

public class MultiPlayer : MonoBehaviourPunCallbacks, IOnEventCallback
{
	public class PendingCarData
	{
		public float ExpireTime;

		public bool HasRawPosition;

		public Vector3Int RawPosition;

		public float RawYRotation;

		public readonly List<(byte Code, string Target, string Value)> Rpc = new List<(byte, string, string)>();
	}

	public enum PhotonViewSource
	{
		Auto = 0,
		FirstCarMultiPlayer = 1
	}

	public delegate void PlayerConnectedDelegate(Player player);

	public delegate void PlayerDisconnectedDelegate(Player player);

	public delegate void RemoveFromScaleDelegate();

	[Serializable]
	public class RoomDatas
	{
		public string RoomName;

		public int password;

		public bool forFriends;

		public List<string> playersEmail;

		public RoomDatas()
		{
		}

		public RoomDatas(string roomName, int password, List<string> playersEmail, bool forFriends)
		{
			RoomName = roomName;
			this.password = password;
			this.playersEmail = playersEmail;
			this.forFriends = forFriends;
		}
	}

	public const byte PassGlobalPointEventCode = 53;

	private const byte AllControlledCarsRowEventCode = 52;

	private const byte CarRawEventCode = 50;

	private const byte PersonRawEventCode = 51;

	private const float RawDataInterval = 0.5f;

	private int _compressedRawData;

	private readonly RaiseEventOptions _defaultRaiseEventOptions = RaiseEventOptions.Default;

	[Inject]
	private InterestGroupManager _interestGroupManager;

	private int _timesCompressedSameValue;

	private MultiPersonControl _driverPerson;

	private readonly byte[] _carRawBytes = new byte[5];

	private readonly byte[] _personRawBytes = new byte[4];

	private const float PendingCarDataLifetimeSeconds = 5f;

	private readonly Dictionary<int, PendingCarData> _pendingCarData = new Dictionary<int, PendingCarData>();

	private const int RelaxSameLangAndMoney = 0;

	private const int RelaxSameLangAnyMoney = 1;

	private const int RelaxAnyLang = 2;

	private int _joinRelaxLevel;

	private const string PlainLangKey = "LP";

	private bool _roomListDiagLogged;

	public const byte RoomCacheDrainedEventCode = 58;

	private const float RoomCacheDrainFallbackSeconds = 4f;

	private bool _roomCacheDrainHandled;

	private List<GameObject> carsDisableAgain;

	public bool ignoreFriendRequest;

	public bool ignoreRacingRequest;

	public bool hideAllChat;

	public ObscuredInt reportReceivedCount;

	public List<string> idFrom;

	public List<GameObject> allLabels;

	public List<GameObject> photonObjects;

	public int lastOfferSendID;

	public ClientState state;

	public int roomCount;

	public int playerCount;

	public List<PlayerMainData> listOfActivePlayers = new List<PlayerMainData>();

	private PlayerDataStorage playerDataStorage;

	private IAccountController accountController;

	private PhotonReconnectionHandler _photonReconnectionHandler;

	private LockCarButtonController _lockCarButtonController;

	[SerializeField]
	public bool EnableRpcLogs;

	private IDisposable _toMainMenuOnRemoveTimer;

	private IDisposable _toMainMenuOnBanTimer;

	private IDisposable _disconnectOnDuplicateIdTimer;

	private IDisposable _forceToSpeakerTimer;

	private RpcCollection _rpcCollection;

	private Dictionary<CarDoorType, bool> _openedDoors;

	private CoreGameManager _coreGameManager;

	private MenuLevelParent _menuLevelParent;

	private SceneControl _sceneControl;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private FreeDriveControl _freeDriveControl;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private OpenWorldAnalytic _openWorldAnalytic;

	private IPrefabFactory _prefabFactory;

	private ArenaPointsContainer _arenaPointsContainer;

	private ArenaModeRegistry _arenaModeRegistry;

	private ArenaFlowController _arenaFlowController;

	private IGameFactManager _gameFactManager;

	private IDisposable _disconnectDisposable;

	private IDisposable _connectDisposable;

	private IDisposable _joinedRoomDisposable;

	private IDisposable _enableCamDisposable;

	private bool _isOnline;

	private InputMapSetEvent _inputMapSetEvent;

	private const float VoiceChatDriverWaitSeconds = 30f;

	private bool _steamInitialCarsDone;

	private CancellationTokenSource _carStateTrackingCts;

	private const float CarStateTrackWindowSeconds = 10f;

	private const float CarStateSettleDelaySeconds = 0.5f;

	private static readonly HashSet<byte> PersonRoutedCodes = new HashSet<byte> { 52 };

	private int RoomMaxPlayers
	{
		get
		{
			if (_freeDriveProvider.RoomArenaId == ArenaId.Free || !_arenaModeRegistry.TryGetConfig(_freeDriveProvider.RoomArenaId, out var config, onlyEnabled: false))
			{
				return 20;
			}
			return config.MaxPlayers;
		}
	}

	private PhotonConnectionProfile ConnectionProfile
	{
		get
		{
			if (_freeDriveProvider.RoomArenaId == ArenaId.Free)
			{
				return PhotonConnectionProfile.Normal;
			}
			return PhotonConnectionProfile.Arena;
		}
	}

	public GMEVoiceChatManager VoiceChat { get; set; }

	public event PlayerConnectedDelegate OnPlayerConnected;

	public event PlayerDisconnectedDelegate OnPlayerDisconnected;

	public event RemoveFromScaleDelegate OnRemoveFromScale;

	public event Action<PlayerMainData> OnPlayerConnectedWithDataEvent;

	public event Action<PlayerMainData> OnPlayerDisconnectedWithDataEvent;

	public event Action<string> JoinedInRoom;

	public event Action Disconnected;

	private bool IsPendingCarDataExpired(PendingCarData pending)
	{
		return pending.ExpireTime <= Time.time;
	}

	private void RemoveExpiredPendingCarData()
	{
		List<int> list = null;
		foreach (KeyValuePair<int, PendingCarData> pendingCarDatum in _pendingCarData)
		{
			if (IsPendingCarDataExpired(pendingCarDatum.Value))
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(pendingCarDatum.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (int item in list)
		{
			_pendingCarData.Remove(item);
		}
	}

	private PendingCarData GetOrCreatePendingCarData(int viewId)
	{
		if (_pendingCarData.TryGetValue(viewId, out var value) && !IsPendingCarDataExpired(value))
		{
			return value;
		}
		RemoveExpiredPendingCarData();
		value = new PendingCarData
		{
			ExpireTime = Time.time + 5f
		};
		_pendingCarData[viewId] = value;
		return value;
	}

	public bool TryStorePendingCarRpc(int viewId, byte code, string target, string value)
	{
		if ((bool)GetCarByPhotonViewID(viewId))
		{
			return false;
		}
		GetOrCreatePendingCarData(viewId).Rpc.Add((code, target, value));
		return true;
	}

	private void StorePendingCarRawPosition(int viewId, Vector3Int position, float yRotation)
	{
		PendingCarData orCreatePendingCarData = GetOrCreatePendingCarData(viewId);
		orCreatePendingCarData.HasRawPosition = true;
		orCreatePendingCarData.RawPosition = position;
		orCreatePendingCarData.RawYRotation = yRotation;
	}

	public PendingCarData TakePendingCarData(int viewId)
	{
		if (!_pendingCarData.TryGetValue(viewId, out var value))
		{
			return null;
		}
		_pendingCarData.Remove(viewId);
		if (!IsPendingCarDataExpired(value))
		{
			return value;
		}
		return null;
	}

	public void ClearPendingCarData()
	{
		_pendingCarData.Clear();
	}

	public void OnEvent(EventData photonEvent)
	{
		HandleRoomCacheDrainedMarker(photonEvent);
		HandleGlobalPointMoveEvent(photonEvent);
		HandleAllCarsRawPositionEvent(photonEvent);
		byte code = photonEvent.Code;
		if (code != 50 && code != 51)
		{
			return;
		}
		byte[] array = (byte[])photonEvent.CustomData;
		UnpackData((array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3], out var value, out var value2, out var smallValue);
		int x = value - 6000;
		int z = value2;
		float yRotation = (float)smallValue * 45f;
		Vector3Int vector3Int = new Vector3Int(x, 0, z);
		switch (photonEvent.Code)
		{
		case 51:
		{
			MultiPersonControl multiPersonControl = null;
			for (int j = 0; j < _onlineCollectionsProvider.PlayersOnlineNow.Length; j++)
			{
				MultiPersonControl multiPersonControl2 = _onlineCollectionsProvider.PlayersOnlineNow[j];
				if ((bool)multiPersonControl2 && multiPersonControl2.photonView.ControllerActorNr == photonEvent.Sender)
				{
					multiPersonControl = multiPersonControl2;
					break;
				}
			}
			if ((bool)multiPersonControl)
			{
				multiPersonControl.SetLastReceivedPosition(vector3Int);
			}
			break;
		}
		case 50:
		{
			MultiPlayerPosControl multiPlayerPosControl = null;
			if (array.Length <= 4)
			{
				break;
			}
			byte b = array[4];
			MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
			foreach (MultiPlayerPosControl multiPlayerPosControl2 in carsOnlineNow)
			{
				if ((bool)multiPlayerPosControl2 && multiPlayerPosControl2.photonView.ControllerActorNr == photonEvent.Sender && multiPlayerPosControl2.InterestGroup == b)
				{
					multiPlayerPosControl = multiPlayerPosControl2;
					break;
				}
			}
			if ((bool)multiPlayerPosControl)
			{
				multiPlayerPosControl.PredictNextRawPosition(vector3Int, yRotation);
			}
			break;
		}
		}
	}

	private void HandleGlobalPointMoveEvent(EventData photonEvent)
	{
		if (photonEvent.Code == 53 && IsValidGlobalPointSender(photonEvent.Sender))
		{
			long current = (long)photonEvent.CustomData;
			Vector3 vector = MultiPlayerPosControl.GlobalOffsetPacker.Unpack(ref current);
			vector.x += 6000f;
			vector.y = 0f;
			V.GlobalPoint.position = vector;
			V.GlobalOffset = vector;
			Global.GetTrailer(out var trailer);
			MoveElements._object.Move();
			if (ScenesHelper.IsFreeDriveSceneActive() && (bool)trailer)
			{
				trailer.Attach(trailer.Car);
			}
			Physics.SyncTransforms();
		}
	}

	private bool IsValidGlobalPointSender(int senderActor)
	{
		if (!V.DriverPerson || !V.DriverPerson.TryGetComponent<MultiPersonControl>(out var component))
		{
			return false;
		}
		if (!component.isPassanger || !component.currentCar)
		{
			return false;
		}
		PhotonView component2 = component.currentCar.GetComponent<PhotonView>();
		if (component2 != null && !component2.IsMine)
		{
			return component2.ControllerActorNr == senderActor;
		}
		return false;
	}

	private void HandleAllCarsRawPositionEvent(EventData photonEvent)
	{
		if (photonEvent.Code != 52)
		{
			return;
		}
		byte[] array = (byte[])photonEvent.CustomData;
		for (int i = 0; i < array.Length; i += 8)
		{
			int packed = (array[i] << 24) | (array[i + 1] << 16) | (array[i + 2] << 8) | array[i + 3];
			int num = (array[i + 4] << 24) | (array[i + 5] << 16) | (array[i + 6] << 8) | array[i + 7];
			UnpackData(packed, out var value, out var value2, out var smallValue);
			int x = value - 6000;
			int z = value2;
			float yRotation = (float)smallValue * 45f;
			Vector3Int position = new Vector3Int(x, 0, z);
			MultiPlayerPosControl multiPlayerPosControl = null;
			MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
			foreach (MultiPlayerPosControl multiPlayerPosControl2 in carsOnlineNow)
			{
				if ((bool)multiPlayerPosControl2 && multiPlayerPosControl2.photonView.ViewID == num)
				{
					multiPlayerPosControl = multiPlayerPosControl2;
					multiPlayerPosControl2.PredictNextRawPosition(position, yRotation);
					break;
				}
			}
			if (!multiPlayerPosControl)
			{
				StorePendingCarRawPosition(num, position, yRotation);
			}
		}
	}

	private async UniTaskVoid StartRaisingEvent()
	{
		while (_isOnline)
		{
			RaiseRawPositionToAll();
			await UniTask.Delay(TimeSpan.FromSeconds(PhotonNetwork.SerializationRate switch
			{
				3 => 1f, 
				5 => 0.5f, 
				6 => 0.45f, 
				8 => 0.35f, 
				_ => 0.5f, 
			}), ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
		}
	}

	public void RaiseRawPositionToAll(bool? overrideSendCar = null, bool forceSendValue = false)
	{
		bool flag = V.Car;
		if (overrideSendCar.HasValue)
		{
			flag = overrideSendCar.Value;
		}
		GameObject gameObject = V.Car;
		if (!gameObject)
		{
			gameObject = _freeDriveProvider.FirstCarMultiPlayer;
		}
		GameObject gameObject2 = (flag ? gameObject : V.DriverPerson);
		if (!gameObject2)
		{
			return;
		}
		MultiPlayerPosControl component = null;
		if (flag && (bool)gameObject && gameObject.TryGetComponent<MultiPlayerPosControl>(out component) && !component.photonView.IsMine)
		{
			return;
		}
		int num = PackThisTarget(gameObject2, out var xPos, out var zPos);
		if (num == _compressedRawData)
		{
			_timesCompressedSameValue++;
		}
		if (_timesCompressedSameValue >= 10)
		{
			forceSendValue = true;
		}
		if (!forceSendValue && num == _compressedRawData)
		{
			return;
		}
		_compressedRawData = num;
		_timesCompressedSameValue = 0;
		Vector3Int vector3Int = new Vector3Int(xPos - 6000, 0, zPos);
		byte eventCode = (byte)(flag ? 50 : 51);
		byte[] eventContent;
		if (flag)
		{
			_carRawBytes[0] = (byte)(num >> 24);
			_carRawBytes[1] = (byte)(num >> 16);
			_carRawBytes[2] = (byte)(num >> 8);
			_carRawBytes[3] = (byte)num;
			_carRawBytes[4] = (byte)(((bool)gameObject && (bool)component) ? component.InterestGroup : 0);
			eventContent = _carRawBytes;
			if ((bool)gameObject && (bool)component)
			{
				component.PredictSentPosition(vector3Int);
			}
		}
		else
		{
			_personRawBytes[0] = (byte)(num >> 24);
			_personRawBytes[1] = (byte)(num >> 16);
			_personRawBytes[2] = (byte)(num >> 8);
			_personRawBytes[3] = (byte)num;
			eventContent = _personRawBytes;
			if (!_driverPerson && (bool)V.DriverPerson)
			{
				_driverPerson = V.DriverPerson.GetComponent<MultiPersonControl>();
			}
			if ((bool)_driverPerson)
			{
				_driverPerson.LastSentRawPosition = vector3Int;
			}
		}
		PhotonNetwork.RaiseEvent(eventCode, eventContent, _defaultRaiseEventOptions, SendOptions.SendUnreliable);
	}

	private void RaiseAllControlledCarsRawPosition(int connectedActorNumber)
	{
		byte[] dataToRaiseAllControllerCars = GetDataToRaiseAllControllerCars();
		if (dataToRaiseAllControllerCars != null && dataToRaiseAllControllerCars.Length != 0)
		{
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			raiseEventOptions.TargetActors = new int[1] { connectedActorNumber };
			RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
			PhotonNetwork.RaiseEvent(52, dataToRaiseAllControllerCars, raiseEventOptions2, SendOptions.SendUnreliable);
		}
	}

	private byte[] GetDataToRaiseAllControllerCars()
	{
		List<MultiPlayerPosControl> list = new List<MultiPlayerPosControl>();
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if ((bool)multiPlayerPosControl && multiPlayerPosControl.photonView.IsMine)
			{
				list.Add(multiPlayerPosControl);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		byte[] array = new byte[list.Count * 8];
		for (int j = 0; j < list.Count; j++)
		{
			MultiPlayerPosControl multiPlayerPosControl2 = list[j];
			int num = PackThisTarget(multiPlayerPosControl2.gameObject, out var xPos, out var zPos);
			Vector3Int position = new Vector3Int(xPos - 6000, 0, zPos);
			multiPlayerPosControl2.PredictSentPosition(position);
			int num2 = j * 8;
			array[num2] = (byte)(num >> 24);
			array[num2 + 1] = (byte)(num >> 16);
			array[num2 + 2] = (byte)(num >> 8);
			array[num2 + 3] = (byte)num;
			int viewID = list[j].photonView.ViewID;
			array[num2 + 4] = (byte)(viewID >> 24);
			array[num2 + 5] = (byte)(viewID >> 16);
			array[num2 + 6] = (byte)(viewID >> 8);
			array[num2 + 7] = (byte)viewID;
		}
		return array;
	}

	public void RaiseAllControlledCarsRawPosition()
	{
		byte[] dataToRaiseAllControllerCars = GetDataToRaiseAllControllerCars();
		if (dataToRaiseAllControllerCars != null && dataToRaiseAllControllerCars.Length != 0)
		{
			PhotonNetwork.RaiseEvent(52, dataToRaiseAllControllerCars, _defaultRaiseEventOptions, SendOptions.SendUnreliable);
		}
	}

	public static int PackThisTarget(GameObject target, out int xPos, out int zPos)
	{
		Vector3 vector = target.transform.position - V.GlobalOffset;
		Quaternion rotation = target.transform.rotation;
		xPos = (int)Mathf.Clamp(vector.x + 6000f, -16384f, 16383f);
		zPos = (int)Mathf.Clamp(vector.z, -8192f, 8191f);
		int smallValue = (int)(Mathf.Repeat(rotation.eulerAngles.y, 360f) / 45f);
		return PackData(xPos, zPos, smallValue);
	}

	public static int PackData(int value1, int value2, int smallValue)
	{
		int num = value1 + 16384;
		int num2 = value2 + 8192;
		int num3 = num & 0x7FFF;
		num2 &= 0x3FFF;
		smallValue &= 7;
		return (num3 << 17) | (num2 << 3) | smallValue;
	}

	public static void UnpackData(int packed, out int value1, out int value2, out int smallValue)
	{
		value1 = ((packed >> 17) & 0x7FFF) - 16384;
		value2 = ((packed >> 3) & 0x3FFF) - 8192;
		smallValue = packed & 7;
	}

	public void Connect(long _ = 0L)
	{
		if (!FlagsStorage.IsMultiFree)
		{
			DisconnectPhoton(0L);
			Observable.Timer(TimeSpan.FromSeconds(2.0)).Where(IsNotMultiFree).Subscribe(DisconnectPhoton)
				.AddTo(this);
			return;
		}
		if (PhotonNetwork.IsConnected)
		{
			if (V.RoomNameToJoin == "")
			{
				if (FlagsStorage.CreateForFriendOnly)
				{
					CreateRoomForFriends();
				}
				else if (PhotonNetwork.InLobby)
				{
					JoinRandomRoom();
				}
				else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
				{
					PhotonNetwork.JoinLobby();
				}
			}
			else if (PhotonNetwork.InRoom)
			{
				OnJoinedRoom();
			}
			else if (FlagsStorage.CreateForFriendOnly)
			{
				CreateRoomForFriends(V.RoomNameToJoin);
			}
			else
			{
				PhotonNetwork.JoinRoom(V.RoomNameToJoin);
			}
			_isOnline = true;
			StartRaisingEvent().Forget();
		}
		if (FlagsStorage.IsMultiFree && !PhotonNetwork.IsConnected)
		{
			_photonReconnectionHandler.ReconnectSoft(ConnectionProfile);
			_connectDisposable?.Dispose();
			_connectDisposable = Observable.Timer(TimeSpan.FromSeconds(2.0)).Where(IsMultiFree).Subscribe(Connect)
				.AddTo(this);
		}
		if (!FlagsStorage.IsMultiFree && PhotonNetwork.IsConnected)
		{
			PhotonNetwork.Disconnect();
		}
	}

	private bool IsMultiFree(long _ = 0L)
	{
		return FlagsStorage.IsMultiFree;
	}

	private bool IsNotMultiFree(long _)
	{
		return !FlagsStorage.IsMultiFree;
	}

	public static void DisconnectPhoton(long _ = 0L)
	{
		PhotonNetwork.Disconnect();
	}

	private static int GetPlainLangIndex()
	{
		return PlayerPrefs.GetInt("languageIndex", (int)Application.systemLanguage);
	}

	private int GetLangIndexByMoney()
	{
		return MatchmakingBucket.LanguageAndMoney(GetPlainLangIndex(), CurrencyManagerController.Money);
	}

	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master Server: " + PhotonNetwork.Server);
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("Joined Lobby rooms: " + PhotonNetwork.CountOfRooms);
		JoinRandomRoom();
	}

	public void JoinRandomRoom()
	{
		_joinRelaxLevel = 0;
		JoinRandomRoomRelaxed();
	}

	private void JoinRandomRoomRelaxed()
	{
		Hashtable roomDataForRandomRoom = GetRoomDataForRandomRoom(_joinRelaxLevel);
		if (_freeDriveProvider.RoomArenaId == ArenaId.Free)
		{
			roomDataForRandomRoom.Remove("M");
		}
		Debug.Log($"[MP-Diag] JoinRandomRoom relax={_joinRelaxLevel} expectedMax={RoomMaxPlayers} " + "filter=" + SupportClass.DictionaryToString(roomDataForRandomRoom));
		PhotonNetwork.JoinRandomRoom(roomDataForRandomRoom, (byte)RoomMaxPlayers);
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		if (_roomListDiagLogged || roomList.Count == 0)
		{
			return;
		}
		_roomListDiagLogged = true;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append($"[MP-Diag] lobby list rooms={roomList.Count}, первые: ");
		int num = 0;
		foreach (RoomInfo room in roomList)
		{
			if (num++ >= 8)
			{
				break;
			}
			stringBuilder.Append($"[{room.Name} max={room.MaxPlayers} p={room.PlayerCount} open={room.IsOpen} vis={room.IsVisible} " + "props=" + SupportClass.DictionaryToString(room.CustomProperties) + "] ");
		}
		Debug.Log(stringBuilder.ToString());
	}

	private void CreateRoomForFriends(string overrideRoomName = null)
	{
		RoomOptions roomOptions = new RoomOptions();
		Hashtable hashtable = new Hashtable();
		bool num = _freeDriveProvider.RoomArenaId != ArenaId.Free;
		if (!num)
		{
			hashtable.Add("L", GetLangIndexByMoney());
			hashtable.Add("LP", GetPlainLangIndex());
		}
		hashtable.Add("S", (int)V.PlaceToSpawn);
		hashtable.Add("P", V.RoomPassword);
		hashtable.Add("M", (int)_freeDriveProvider.RoomArenaId);
		hashtable.Add("aL", ArenaLobbyId.Generate());
		hashtable.Add("aRT", ArenaRaceToken.Generate());
		string[] customRoomPropertiesForLobby = ((!num) ? new string[5] { "L", "LP", "S", "P", "M" } : new string[3] { "S", "P", "M" });
		roomOptions.PublishUserId = true;
		roomOptions.MaxPlayers = (byte)RoomMaxPlayers;
		roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;
		roomOptions.CustomRoomProperties = hashtable;
		PhotonNetwork.JoinOrCreateRoom(string.IsNullOrEmpty(overrideRoomName) ? ArenaRoomNames.BuildFriendRoomName(playerDataStorage.PlayerID.ToString(), V.RoomPassword, _freeDriveProvider.ArenaFriendRoomInstance, _freeDriveProvider.RoomArenaId) : overrideRoomName, roomOptions, TypedLobby.Default);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Observable.Timer(TimeSpan.FromSeconds(4.0)).Subscribe((long _) =>
		{
			Disconnect();
		}).AddTo(this);
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		Observable.Timer(TimeSpan.FromSeconds(4.0)).Subscribe((long _) =>
		{
			Disconnect();
		}).AddTo(this);
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		if (_freeDriveProvider.RoomArenaId == ArenaId.Free && _joinRelaxLevel < 2)
		{
			_joinRelaxLevel++;
			Debug.Log($"Random join failed ({message}); ослабляю фильтр до уровня {_joinRelaxLevel}");
			JoinRandomRoomRelaxed();
			return;
		}
		RoomOptions roomOptions = new RoomOptions();
		Hashtable roomDataForRandomRoom = GetRoomDataForRandomRoom(0);
		if (_freeDriveProvider.RoomArenaId == ArenaId.Free)
		{
			roomDataForRandomRoom.Add("LP", GetPlainLangIndex());
		}
		roomDataForRandomRoom.Add("aL", ArenaLobbyId.Generate());
		roomDataForRandomRoom.Add("aRT", ArenaRaceToken.Generate());
		string[] customRoomPropertiesForLobby = ((_freeDriveProvider.RoomArenaId == ArenaId.Free) ? new string[5] { "L", "LP", "S", "P", "M" } : new string[3] { "S", "P", "M" });
		roomOptions.PublishUserId = true;
		roomOptions.MaxPlayers = (byte)RoomMaxPlayers;
		roomOptions.CustomRoomProperties = roomDataForRandomRoom;
		roomOptions.CustomRoomPropertiesForLobby = customRoomPropertiesForLobby;
		PhotonNetwork.CreateRoom(ArenaRoomNames.AppendArenaSuffix(playerDataStorage.PlayerID.ToString() + "_" + UnityEngine.Random.Range(10000, 90000), _freeDriveProvider.RoomArenaId), roomOptions, TypedLobby.Default);
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Disconnected?.Invoke();
		if (!ApplicationShutdown.IsQuitting && FlagsStorage.IsMultiFree && V.CoreGameManager.isFreeDrive)
		{
			_arenaFlowController?.LeaveArena(ArenaLeaveReason.Disconnect);
			_openWorldAnalytic.FinishEvent("disconnect");
			V.CoreGameManager.SetMessage(new LocalizedString("Multiplayer.ConnectionIssues"), 2f, NotificationType.Error);
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long l) =>
			{
				_levelsProvider.LevelScript.OpenMainMenu();
			}).AddTo(this);
		}
	}

	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.CurrentRoom.MaxPlayers = (byte)RoomMaxPlayers;
		}
		Time.timeScale = 1f;
		if (FlagsStorage.IsMultiFree)
		{
			_interestGroupManager.TryReserveInterestGroupID(out var _);
			PhotonNetwork.SetInterestGroups(Array.Empty<byte>(), new byte[2]
			{
				InterestGroupManager.CarInterestGroup,
				InterestGroupManager.PersonInterestGroup
			});
		}
		if (FlagsStorage.IsMultiFree)
		{
			SteamBuildInitialCarsAsync(base.destroyCancellationToken).Forget();
		}
		_joinedRoomDisposable?.Dispose();
		_roomCacheDrainHandled = false;
		ClearPendingCarData();
		SendRoomCacheDrainedMarker();
		_joinedRoomDisposable = Observable.Timer(TimeSpan.FromSeconds(4.0)).Subscribe((long _) =>
		{
			OnRoomCacheDrained();
		}).AddTo(this);
		JoinedInRoom?.Invoke(PhotonNetwork.CurrentRoom.Name);
	}

	private void SendRoomCacheDrainedMarker()
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		raiseEventOptions.TargetActors = new int[1] { PhotonNetwork.LocalPlayer.ActorNumber };
		RaiseEventOptions raiseEventOptions2 = raiseEventOptions;
		PhotonNetwork.RaiseEvent(58, null, raiseEventOptions2, SendOptions.SendReliable);
	}

	private void HandleRoomCacheDrainedMarker(EventData photonEvent)
	{
		if (photonEvent.Code == 58 && PhotonNetwork.LocalPlayer != null && photonEvent.Sender == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			OnRoomCacheDrained();
		}
	}

	private void OnRoomCacheDrained()
	{
		if (!_roomCacheDrainHandled)
		{
			_roomCacheDrainHandled = true;
			_joinedRoomDisposable?.Dispose();
			_joinedRoomDisposable = FlagsStorage.FirstMainSceneLoaded.Where((bool loaded) => loaded).First().DelayFrame(1)
				.Subscribe((bool _) =>
				{
					JoinedRoom();
				})
				.AddTo(this);
		}
	}

	private void Disconnect()
	{
		PhotonNetwork.Disconnect();
	}

	private Hashtable GetRoomDataForRandomRoom(int relaxLevel)
	{
		Hashtable hashtable = new Hashtable();
		if (_freeDriveProvider.RoomArenaId == ArenaId.Free)
		{
			if (relaxLevel <= 0)
			{
				hashtable.Add("L", GetLangIndexByMoney());
			}
			else if (relaxLevel == 1)
			{
				hashtable.Add("LP", GetPlainLangIndex());
			}
		}
		hashtable.Add("S", (int)V.PlaceToSpawn);
		hashtable.Add("P", 0);
		hashtable.Add("M", (int)_freeDriveProvider.RoomArenaId);
		return hashtable;
	}

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, MenuLevelParent menuLevelParent, IAccountController accountController, PhotonReconnectionHandler photonReconnectionHandler, LockCarButtonController lockCarButtonController, RpcCollection rpcCollection, CoreGameManager coreGameManager, SceneControl sceneControl, InputMapSetEvent inputMapSetEvent, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveControl freeDriveControl, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, OpenWorldAnalytic openWorldAnalytic, IPrefabFactory prefabFactory, ArenaPointsContainer arenaPointsContainer, ArenaModeRegistry arenaModeRegistry, ArenaFlowController arenaFlowController, IGameFactManager gameFactManager)
	{
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_freeDriveControl = freeDriveControl;
		_inputMapSetEvent = inputMapSetEvent;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_sceneControl = sceneControl;
		_menuLevelParent = menuLevelParent;
		_coreGameManager = coreGameManager;
		_rpcCollection = rpcCollection;
		_lockCarButtonController = lockCarButtonController;
		_photonReconnectionHandler = photonReconnectionHandler;
		this.playerDataStorage = playerDataStorage;
		this.accountController = accountController;
		_openWorldAnalytic = openWorldAnalytic;
		_prefabFactory = prefabFactory;
		_arenaPointsContainer = arenaPointsContainer;
		_arenaModeRegistry = arenaModeRegistry;
		_arenaFlowController = arenaFlowController;
		_gameFactManager = gameFactManager;
	}

	private void Awake()
	{
		_freeDriveProvider.MultiPlayer = this;
		allLabels = new List<GameObject>();
		ignoreFriendRequest = PlayerPrefs.GetInt("ignoreFriendRequest", 0) == 1;
		ignoreRacingRequest = PlayerPrefs.GetInt("ignoreRacingRequest", 0) == 1;
		hideAllChat = PlayerPrefs.GetInt("hideAllChat", 0) == 1;
		_openedDoors = new Dictionary<CarDoorType, bool>();
	}

	private void OnDestroy()
	{
		_freeDriveProvider.MultiPlayer = null;
		_isOnline = false;
		VoiceChat?.Dispose();
		_carStateTrackingCts?.Dispose();
	}

	private void ActivateVoiceChatWhenDriverReady()
	{
		bool flag = PlayerPrefs.GetInt("VoiceChat", GMEVoiceChatManager.VoiceChatDefault) == 1;
		ActivateVoiceChatWhenDriverReadyAsync(flag).Forget();
	}

	private async UniTaskVoid ActivateVoiceChatWhenDriverReadyAsync(bool enabled)
	{
		try
		{
			await UniTask.WaitUntil(IsDriverReadyForVoiceChat, PlayerLoopTiming.Update, base.destroyCancellationToken).Timeout(TimeSpan.FromSeconds(30.0));
		}
		catch (OperationCanceledException)
		{
			return;
		}
		catch (TimeoutException)
		{
			Debug.LogWarning("[MultiPlayer] Водитель не собрался за отведённое время — голосовой чат не поднят.", this);
			return;
		}
		ActivateDeactivateVoiceChat(enabled);
	}

	private static bool IsDriverReadyForVoiceChat()
	{
		if (!V.DriverPerson)
		{
			return false;
		}
		if (!V.DriverPerson.TryGetComponent<MainPerson>(out var component))
		{
			return false;
		}
		FullBodyBipedIK ik = component.ik;
		if ((bool)ik && ik.references != null)
		{
			return ik.references.head;
		}
		return false;
	}

	public void ActivateDeactivateVoiceChat(bool activate)
	{
		if (VoiceChat == null)
		{
			if (!activate)
			{
				return;
			}
			GMEVoiceChatManager gMEVoiceChatManager = _prefabFactory.Create<GMEVoiceChatManager>(Array.Empty<object>());
			try
			{
				gMEVoiceChatManager.Init();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception, this);
				gMEVoiceChatManager?.Dispose();
				return;
			}
			VoiceChat = gMEVoiceChatManager;
		}
		VoiceChat.SwitchVoiceChat(activate, base.destroyCancellationToken).Forget();
	}

	public static string EncryptName(string playerId)
	{
		return new string(ObscuredString.Encrypt(playerId, "1"));
	}

	public static string DecryptName(string playerId)
	{
		return ObscuredString.Decrypt(playerId.ToCharArray(), "1");
	}

	public void OnReport(string playerIDFrom)
	{
		if (playerIDFrom == null)
		{
			return;
		}
		string text = DecryptName(playerIDFrom);
		playerIDFrom = text;
		if (!idFrom.Contains(playerIDFrom))
		{
			if (!_onlineCollectionsProvider.PlayersOnlineNow.Where((MultiPersonControl x) => x != null).ToList().Exists((MultiPersonControl x) => x.ThisPlayerID == playerIDFrom))
			{
				Debug.Log("Player ID From report was not confirmed. Do not count report. :" + playerIDFrom);
				return;
			}
			idFrom.Add(playerIDFrom);
			++reportReceivedCount;
		}
	}

	public void OnRemoveFromRoom()
	{
		_openWorldAnalytic.FinishEvent("kicked");
		int value = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		ObscuredPrefs.SetInt(PhotonNetwork.CurrentRoom.Name, value);
		V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayer.YouWillBeRemovedFromThisRoom"), 4f);
		SendValueStr(51, playerDataStorage.PlayerName, "");
		_toMainMenuOnRemoveTimer?.Dispose();
		_toMainMenuOnRemoveTimer = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			ToMainMenu();
		}).AddTo(this);
	}

	private void Start()
	{
		idFrom = new List<string>();
		PhotonNetwork.UseRpcMonoBehaviourCache = true;
		Connect(0L);
		Observable.Timer(TimeSpan.FromSeconds(8.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(8.0))).Subscribe(UpdatePlayerListReactive)
			.AddTo(this);
	}

	private void Update()
	{
		state = PhotonNetwork.NetworkClientState;
		playerCount = PhotonNetwork.CountOfPlayers;
		roomCount = PhotonNetwork.CountOfRooms;
		if (PhotonNetwork.CurrentRoom != null)
		{
			playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		}
	}

	public void PlayerEnteredToCar(int personID, int carID, int place, bool playAnimation, bool playDefaultDoorSound = true)
	{
		PlayerEnteredToCar(GetPersonByID(personID), GetCarByPhotonViewID(carID), place, playAnimation, playDefaultDoorSound);
	}

	public void PlayerEnteredToCar(GameObject person, GameObject car, int place, bool playAnimation, bool playDefaultDoorSound = true)
	{
		VehicleData vehicleData = (car ? car.GetComponent<VehicleData>() : null);
		bool flag = (bool)vehicleData && vehicleData.CarLocked;
		if (flag && (bool)person && person.TryGetPhotonView(out var photonView) && (bool)car && car.TryGetPhotonView(out var photonView2) && photonView.CreatorActorNr == photonView2.CreatorActorNr)
		{
			flag = false;
		}
		if (!((!person || !car) | flag))
		{
			MainPerson component = person.GetComponent<MainPerson>();
			PersonAnimationsController personAnimationsController = (component ? component.PersonAnimationsController : null);
			if (((bool)personAnimationsController & playAnimation) && (bool)vehicleData && (bool)vehicleData.CarAnimationReferences)
			{
				personAnimationsController.SitGetOutCarSystem.SitIntoCarOtherPlayer(car, place);
			}
			else if (place == 0)
			{
				PlayerEnteredToCar(person, car, playDefaultDoorSound);
			}
			else
			{
				PlayerEnteredToCarPassenger(person, car, place, playDefaultDoorSound);
			}
		}
	}

	public void OpenCloseCarElement(int carID, bool isOpen, CarDoorType carDoorType)
	{
		GameObject carByPhotonViewID = GetCarByPhotonViewID(carID);
		if (!carByPhotonViewID)
		{
			return;
		}
		CarAnimationsController component = carByPhotonViewID.GetComponent<CarAnimationsController>();
		if ((bool)component)
		{
			if (carDoorType == CarDoorType.Trunk)
			{
				component.OpenCloseTrunk(isOpen, sendData: false).Forget();
			}
			else
			{
				component.OpenCloseCarElement(carDoorType, isOpen);
			}
		}
	}

	public void OpenCloseRoof(int carID, bool isOpen)
	{
		GameObject carByPhotonViewID = GetCarByPhotonViewID(carID);
		if ((bool)carByPhotonViewID)
		{
			CarAnimationsController component = carByPhotonViewID.GetComponent<CarAnimationsController>();
			if ((bool)component)
			{
				component.OpenCloseRoof(isOpen, isSendData: false).Forget();
			}
		}
	}

	public void OpenCloseCarDoor(int personId, int carId, int place, CarAnimationType carAnimationType)
	{
		GameObject personByID = GetPersonByID(personId);
		GameObject carByPhotonViewID = GetCarByPhotonViewID(carId);
		if ((bool)personByID && (bool)carByPhotonViewID)
		{
			MainPerson component = personByID.GetComponent<MainPerson>();
			PersonAnimationsController personAnimationsController = (component ? component.PersonAnimationsController : null);
			CarAnimationsController component2 = carByPhotonViewID.GetComponent<CarAnimationsController>();
			CarAnimationReferences carAnimationReferences = (component2 ? component2.CarAnimationReferences : null);
			if ((bool)personAnimationsController && (bool)carAnimationReferences)
			{
				personAnimationsController.SitGetOutCarSystem.OpenCloseDoorOtherPlayer(carByPhotonViewID, place, carAnimationType);
			}
			else if ((bool)component2)
			{
				bool isOpen = carAnimationType == CarAnimationType.OpenInside || carAnimationType == CarAnimationType.OpenOutside;
				component2.FastOpenCloseDoor((CarDoorType)place, isOpen);
			}
		}
	}

	public void LockCar(int carID, int sound)
	{
		GameObject carByPhotonViewID = GetCarByPhotonViewID(carID);
		if ((bool)carByPhotonViewID)
		{
			if (!carByPhotonViewID.TryGetComponent<VehicleData>(out var component))
			{
				return;
			}
			component.CarLocked = true;
			if (sound == 1)
			{
				if (carByPhotonViewID.TryGetComponent<SoundController>(out var component2))
				{
					component2.LockCar();
				}
				if (carByPhotonViewID.TryGetComponent<LightController>(out var component3))
				{
					component3.EmergencyLightOnForTime(0.76f);
				}
			}
		}
		if (!V.DriverPerson)
		{
			return;
		}
		MainPerson component4 = V.DriverPerson.GetComponent<MainPerson>();
		if ((bool)component4)
		{
			PersonAnimationsController personAnimationsController = (component4 ? component4.PersonAnimationsController : null);
			bool flag = (bool)personAnimationsController && personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate;
			if ((flag && personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentCar.gameObject == carByPhotonViewID) & flag)
			{
				personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.CancelSeat();
			}
		}
	}

	public void UnlockCar(int carID, int sound)
	{
		GameObject carByPhotonViewID = GetCarByPhotonViewID(carID);
		if ((bool)carByPhotonViewID && carByPhotonViewID.TryGetComponent<VehicleData>(out var component))
		{
			component.CarLocked = false;
			if (sound == 1)
			{
				carByPhotonViewID.GetComponent<SoundController>().UnlockCar();
				carByPhotonViewID.GetComponent<LightController>().EmergencyLightOnForTime(1.52f);
			}
		}
	}

	public void PlayerEnteredToCarPassenger(GameObject person, GameObject car, int place, bool playDoorSound = true)
	{
		if ((bool)person && (bool)car)
		{
			DriverController component = car.GetComponent<DriverController>();
			MultiPersonControl component2 = person.GetComponent<MultiPersonControl>();
			VehicleData component3 = car.GetComponent<VehicleData>();
			if ((bool)person.GetComponent<Rigidbody>())
			{
				person.GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
			person.GetComponent<DriverActionController>().isPassenger = true;
			component2.placeOnCar = place;
			component3.VehicleMembersController.SetMember(place, person);
			component2.currentCar = car;
			component2.isPassanger = true;
			person.transform.parent = car.transform;
			person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: false);
			FullBodyBipedIK ik = person.GetComponent<MainPerson>().ik;
			component.PreSetPerson(person, place);
			if ((bool)ik)
			{
				ik.solver.IKPositionWeight = 1f;
			}
			if (car.GetComponent<PhotonView>().IsMine)
			{
				PlayerDatas.SetOnlineCarBool(car, isOnline: false);
			}
			else
			{
				PlayerDatas.SetOnlineCarBool(car, isOnline: true);
			}
			if ((bool)component3.Driver && component3.Driver == V.DriverPerson)
			{
				car.GetComponent<Powertrain>().OnLineCar = false;
			}
			EnableAllComponent(person);
			_coreGameManager.CarOutInControl.SetAllWheelOwn(car, car == V.Car);
			person.GetComponent<DriverActionController>().HidePersonLabel();
			person.gameObject.SetActive(car.gameObject.activeSelf);
			if (playDoorSound)
			{
				_coreGameManager.CarOutInControl.playCarCloseSound(car);
			}
			person.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
		}
	}

	public void PlayerEnteredToCar(GameObject person, GameObject car, bool playDoorSound = true)
	{
		if ((bool)person && (bool)car)
		{
			DriverController component = car.GetComponent<DriverController>();
			MultiPersonControl component2 = person.GetComponent<MultiPersonControl>();
			VehicleData component3 = car.GetComponent<VehicleData>();
			MultiPlayerPosControl component4 = car.GetComponent<MultiPlayerPosControl>();
			DriverActionController component5 = person.GetComponent<DriverActionController>();
			Rigidbody component6 = person.GetComponent<Rigidbody>();
			if ((bool)component6 && !component6.isKinematic)
			{
				component6.velocity = Vector3.zero;
			}
			if ((bool)V.DriverPerson && person != V.DriverPerson)
			{
				component3.GetComponent<Rigidbody>().isKinematic = true;
			}
			if (!component2.FirstAndOwnCar)
			{
				component2.FirstAndOwnCar = car;
			}
			component2.currentCar = car;
			component2.placeOnCar = 0;
			component5.isPassenger = false;
			component2.isPassanger = false;
			component3.VehicleMembersController.SetDriver(person);
			person.transform.parent = car.transform;
			MainPerson component7 = person.GetComponent<MainPerson>();
			FullBodyBipedIK ik = component7.ik;
			if ((bool)component7.mainBodySkin)
			{
				component7.mainBodySkin.shadowCastingMode = ShadowCastingMode.Off;
			}
			if ((bool)component7.CombinedBodySkin)
			{
				component7.CombinedBodySkin.shadowCastingMode = ShadowCastingMode.Off;
			}
			component.PreSetPerson(person, 0);
			if ((bool)ik)
			{
				ik.solver.IKPositionWeight = 1f;
			}
			PlayerDatas.SetOnlineCarBool(car, isOnline: true);
			EnableAllComponent(person);
			component4.updateLabel(person, Show: true);
			_coreGameManager.CarOutInControl.SetAllWheelOwn(car, Own: false);
			person.GetComponent<DriverActionController>().HidePersonLabel();
			person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: false);
			person.gameObject.SetActive(car.gameObject.activeSelf);
			if (playDoorSound)
			{
				_coreGameManager.CarOutInControl.playCarCloseSound(car);
			}
			person.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
		}
	}

	public void PlayerExitFromCar(int personID, int carID, int place, bool playAnimation)
	{
		GameObject personByID = GetPersonByID(personID);
		GameObject carByPhotonViewID = GetCarByPhotonViewID(carID);
		if ((bool)personByID && (bool)carByPhotonViewID)
		{
			MainPerson component = personByID.GetComponent<MainPerson>();
			PersonAnimationsController personAnimationsController = (component ? component.PersonAnimationsController : null);
			if ((bool)personAnimationsController && personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate)
			{
				personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.CancelSeat();
			}
			else if ((bool)personAnimationsController & playAnimation)
			{
				personAnimationsController.SitGetOutCarSystem.GetOutCarOtherPlayer(carByPhotonViewID, place);
			}
			else
			{
				PlayerExitFromCar(personByID, carByPhotonViewID);
			}
		}
	}

	public void PlayerExitFromCar(GameObject person, GameObject car, bool playDoorSound = true)
	{
		if ((bool)person && (bool)car)
		{
			person.gameObject.SetActive(value: true);
			MultiPersonControl component = person.GetComponent<MultiPersonControl>();
			DriverActionController component2 = person.GetComponent<DriverActionController>();
			if ((bool)car)
			{
				MultiPlayerPosControl component3 = car.GetComponent<MultiPlayerPosControl>();
				car.GetComponent<DriverController>();
				car.GetComponent<VehicleData>().VehicleMembersController.SetMember(component.placeOnCar, null);
				component3.updateLabel(person, Show: false, hideLabel: true);
			}
			component.currentCar = null;
			component.isPassanger = false;
			component2.isPassenger = false;
			person.transform.parent = V.CoreGameManager.globalPoint.transform;
			component.correctPlayerPos = person.transform.localPosition;
			MainPerson component4 = person.GetComponent<MainPerson>();
			if ((bool)component4.PersonAnimationsController)
			{
				component4.PersonAnimationsController.StopAnimation();
			}
			FullBodyBipedIK ik = component4.ik;
			if ((bool)ik)
			{
				ik.solver.IKPositionWeight = 0f;
			}
			Rigidbody component5 = person.GetComponent<Rigidbody>();
			if ((bool)component5)
			{
				component5.isKinematic = false;
			}
			EnableAllComponent(person);
			person.GetComponent<DriverActionController>().ShowPersonLabel();
			person.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: true);
			if (playDoorSound)
			{
				_coreGameManager.CarOutInControl.playCarCloseSound(car);
			}
			person.GetComponent<PhotonView>().Synchronization = ViewSynchronization.UnreliableOnChange;
			person.transform.localScale = new Vector3(1f, 1f, 1f);
			DriverController.RestoreOutOfCarOutfit(component4);
		}
	}

	public GameObject GetPersonByID(int personID)
	{
		PhotonView playerByPhotonViewID = _onlineCollectionsProvider.GetPlayerByPhotonViewID(personID);
		if (!playerByPhotonViewID)
		{
			return null;
		}
		return playerByPhotonViewID.gameObject;
	}

	public MultiPersonControl GetMultiPersonControl(int ownerActorId, bool checkVDriverPerson = false)
	{
		if (checkVDriverPerson && PhotonNetwork.LocalPlayer.ActorNumber == ownerActorId && (bool)V.DriverPerson)
		{
			return V.DriverPerson.GetComponent<MultiPersonControl>();
		}
		return _onlineCollectionsProvider.GetPlayerByOwnerActorNr(ownerActorId);
	}

	public GameObject GetCarByPhotonViewID(int viewID)
	{
		return _onlineCollectionsProvider.GetCarByPhotonViewID(viewID);
	}

	private async UniTaskVoid SteamBuildInitialCarsAsync(CancellationToken ct)
	{
		if (_steamInitialCarsDone || V.SceneControl == null)
		{
			return;
		}
		_steamInitialCarsDone = true;
		V.SceneControl.SetMpInitialCarsBuilding(value: true);
		try
		{
			float deadline = Time.realtimeSinceStartup + 20f;
			float settleUntil = Time.realtimeSinceStartup + 3f;
			while (Time.realtimeSinceStartup < deadline && !ct.IsCancellationRequested)
			{
				MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
				bool flag = true;
				bool flag2 = false;
				foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
				{
					if (!multiPlayerPosControl)
					{
						continue;
					}
					flag2 = true;
					if (!multiPlayerPosControl.VisualBuilt)
					{
						flag = false;
						if (!multiPlayerPosControl.VisualBuildStarted && multiPlayerPosControl.DataApplied && V.SceneControl != null && V.SceneControl.TryStartMainBodyLoad())
						{
							multiPlayerPosControl.ForceBuildVisual();
						}
					}
				}
				if (Time.realtimeSinceStartup >= settleUntil && (flag || !flag2))
				{
					break;
				}
				await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}
		finally
		{
			if (V.SceneControl != null)
			{
				V.SceneControl.SetMpInitialCarsBuilding(value: false);
			}
		}
	}

	public void JoinedRoom()
	{
		if (FlagsStorage.FirstMainSceneLoaded.Value)
		{
			if (_gameFactManager.IsTrue(GameFact.Carless))
			{
				_freeDriveControl.PreparePedestrian(V.PlaceToSpawn);
				if (TryGetArenaPoints(out var cfg))
				{
					PlacePedestrianAtArenaPoint(cfg);
					ArenaSpawnGroundHold.HoldPersonUntilGroundedAsync(V.DriverPerson, base.destroyCancellationToken).Forget();
				}
			}
			else
			{
				_freeDriveControl.PrepareCar(V.PlaceToSpawn).Forget();
				FindPlaceToSpawn(base.destroyCancellationToken).Forget();
			}
			_freeDriveControl.OnFirtsCarSpawn();
			_enableCamDisposable?.Dispose();
			_enableCamDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				EnableCam();
			}).AddTo(this);
			ActivateVoiceChatWhenDriverReady();
			CheckForBan();
		}
		else
		{
			_joinedRoomDisposable?.Dispose();
			_joinedRoomDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				JoinedRoom();
			}).AddTo(this);
		}
	}

	private void EnableCam()
	{
		bool num = _gameFactManager.IsTrue(GameFact.Carless) && !V.Car;
		_freeDriveControl.ShowMainDrivingUI();
		if (num)
		{
			_freeDriveControl.ShowWalkingUI();
		}
		GetComponent<Camera>().enabled = true;
		Physics.SyncTransforms();
		if (!num && _menuLevelParent.IOSPlayer)
		{
			AfterForce();
		}
		CheckIfIDExistsInRoom();
		if (!num)
		{
			if (V.Steering == 0)
			{
				_menuLevelParent.SteeringWheel.SetActive(active: true);
			}
			else if (V.Steering == 2)
			{
				_menuLevelParent.LeftRight.SetActive(value: true);
			}
		}
	}

	private void CheckForBan()
	{
		string key = PhotonNetwork.CurrentRoom.Name;
		if (!ObscuredPrefs.HasKey(key))
		{
			return;
		}
		int num = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		int num2 = ObscuredPrefs.GetInt(key, num);
		if (num - num2 < 7200)
		{
			V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayer.YouCanTPlayInThisRoom"), 3f);
			_toMainMenuOnBanTimer?.Dispose();
			_toMainMenuOnBanTimer = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				ToMainMenu();
			}).AddTo(this);
		}
		else
		{
			ObscuredPrefs.DeleteKey(key);
		}
	}

	private void ToMainMenu()
	{
		_menuLevelParent.OpenMainMenu();
	}

	private void CheckIfIDExistsInRoom()
	{
		if ((bool)_onlineCollectionsProvider.GetPlayerByPlayerID(playerDataStorage.PlayerID))
		{
			V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayer.ThereIsAPlayerWithTheSameId"), 3f);
			_disconnectOnDuplicateIdTimer?.Dispose();
			_disconnectOnDuplicateIdTimer = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				Disconnect();
			}).AddTo(this);
		}
	}

	private async UniTask FindPlaceToSpawn(CancellationToken cancellationToken)
	{
		if (TryGetArenaPoints(out var cfg))
		{
			await SpawnCarAtArenaPoint(cfg, cancellationToken);
			return;
		}
		Transform[] spawnPoints;
		if (FlagsStorage.IsLoadingFromEvent)
		{
			(spawnPoints, V.CurrentIndexArrayPositionCar) = V.SceneControl.SpecialPointsContainer.GetSpawnPoints(V.PlaceToSpawn);
		}
		else
		{
			(spawnPoints, V.CurrentIndexArrayPositionCar) = V.SceneControl.SpawnPointsContainer.GetSpawnPoints(V.PlaceToSpawn);
		}
		FlagsStorage.IsLoadingFromEvent = false;
		if (V.PlaceToSpawn == V.placeToSpawn.City2)
		{
			_freeDriveControl.current_tar_City2 = 630;
		}
		V.CurrentIndexArrayPositionCar -= 15;
		await UniTask.WaitUntil(() => V.Car, PlayerLoopTiming.Update, cancellationToken);
		Transform randomSpawnPoint = spawnPoints.GetRandomSpawnPoint(out var forceUsePoint);
		Vector3 vector = randomSpawnPoint.position;
		if (!forceUsePoint)
		{
			vector = SpawnPointRandomizer.CheckPoints(vector, spawnPoints);
		}
		VehicleData component = V.Car.GetComponent<VehicleData>();
		vector += Vector3.up * (component.GroundOffset + 0.2f);
		V.Car.transform.position = vector;
		V.Car.transform.eulerAngles = randomSpawnPoint.eulerAngles;
		if ((bool)V.DriverPerson && V.DriverPerson.TryGetComponent<MultiPersonControl>(out var component2) && !component2.currentCar)
		{
			V.DriverPerson.transform.position = vector;
		}
		component.KeepRowComposition.CurrentTargetIndex = V.CurrentIndexArrayPositionCar;
	}

	private bool TryGetArenaPoints(out ArenaPointsConfig cfg)
	{
		cfg = null;
		if (_freeDriveProvider.RoomArenaId != ArenaId.Free && _arenaModeRegistry != null && _arenaModeRegistry.TryGetConfig(_freeDriveProvider.RoomArenaId, out var config) && _arenaPointsContainer != null && _arenaPointsContainer.TryGet(config.WaitingLocation, out cfg))
		{
			return cfg.Points.Count > 0;
		}
		return false;
	}

	private static PositionsAndRotation PickArenaPoint(ArenaPointsConfig cfg, out Vector3 position)
	{
		List<PositionsAndRotation> points = cfg.Points;
		PositionsAndRotation positionsAndRotation = points[UnityEngine.Random.Range(0, points.Count)];
		List<Vector3> list = new List<Vector3>(points.Count);
		foreach (PositionsAndRotation item in points)
		{
			list.Add(item.Position);
		}
		position = SpawnPointRandomizer.CheckPoints(positionsAndRotation.Position, list);
		return positionsAndRotation;
	}

	private async UniTask SpawnCarAtArenaPoint(ArenaPointsConfig cfg, CancellationToken cancellationToken)
	{
		await UniTask.WaitUntil(() => V.Car, PlayerLoopTiming.Update, cancellationToken);
		PositionsAndRotation positionsAndRotation = PickArenaPoint(cfg, out var position);
		VehicleData component = V.Car.GetComponent<VehicleData>();
		position += Vector3.up * (component.GroundOffset + 0.2f);
		V.Car.transform.position = position;
		V.Car.transform.rotation = positionsAndRotation.Rotation;
	}

	private void PlacePedestrianAtArenaPoint(ArenaPointsConfig cfg)
	{
		if ((bool)V.DriverPerson)
		{
			PositionsAndRotation positionsAndRotation = PickArenaPoint(cfg, out var position);
			Vector3 position2 = position + Vector3.up;
			V.DriverPerson.transform.SetPositionAndRotation(position2, positionsAndRotation.Rotation);
			if (V.DriverPerson.TryGetComponent<Rigidbody>(out var component))
			{
				component.position = position2;
				component.rotation = positionsAndRotation.Rotation;
				component.velocity = Vector3.zero;
			}
			Physics.SyncTransforms();
		}
	}

	public void SendValueStr<TRpcType>(string b, string name, PhotonViewSource source = PhotonViewSource.Auto) where TRpcType : RpcObjectBase
	{
		byte rpcCode = _rpcCollection.GetRpcCode<TRpcType>();
		SendValueStr(rpcCode, b, name, _rpcCollection.GetRpcObject(rpcCode)?.SendToInterestGroup ?? false, source);
	}

	public void SendValueStrToPlayer<TRpcType>(string b, string name, int actorNumber) where TRpcType : RpcObjectBase
	{
		byte rpcCode = _rpcCollection.GetRpcCode<TRpcType>();
		SendValueStrToPlayer(rpcCode, b, name, actorNumber);
	}

	public void SendValueStrToPlayer<TRpcType>(string b, string name, string target) where TRpcType : RpcObjectBase
	{
		byte rpcCode = _rpcCollection.GetRpcCode<TRpcType>();
		SendValueStrToPlayer(rpcCode, b, name, target);
	}

	public void SendValueStrToPlayer<TRpcType>(string b, string name, Player player) where TRpcType : RpcObjectBase
	{
		byte rpcCode = _rpcCollection.GetRpcCode<TRpcType>();
		SendValueStrToPlayer(rpcCode, b, name, player);
	}

	public void SendValueStr(byte a, string b, string name, bool sendToInterestGroup = false, PhotonViewSource source = PhotonViewSource.Auto)
	{
		PhotonView photonView = null;
		bool num = PersonRoutedCodes.Contains(a);
		if (num && (bool)V.DriverPerson)
		{
			photonView = PhotonView.Get(V.DriverPerson.GetComponent<PhotonView>());
		}
		else if (FlagsStorage.InGarageFree)
		{
			photonView = V.LastUsedCar.GetComponent<PhotonView>();
		}
		else if ((bool)V.Car)
		{
			photonView = PhotonView.Get(V.Car.GetComponent<PhotonView>());
		}
		else if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			photonView = PhotonView.Get(_freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>());
		}
		if (!num && source == PhotonViewSource.FirstCarMultiPlayer && (bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			photonView = PhotonView.Get(_freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>());
		}
		if (photonView != null && PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
		{
			if (sendToInterestGroup)
			{
				photonView.InterestGroupRPC("SendValueStr", photonView.Group, RpcTarget.Others, a, b, name);
			}
			else
			{
				photonView.RpcToDefaultGroup("SendValueStr", RpcTarget.Others, a, b, name);
			}
		}
	}

	public void SendValueStrToPlayer(byte a, string b, string name, int actorNumber)
	{
		if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
		{
			PhotonView photonViewVCarOrFirstCarMultiPlayer = GetPhotonViewVCarOrFirstCarMultiPlayer();
			Player player = PhotonNetwork.CurrentRoom.Players.Values.FirstOrDefault((Player p) => p.ActorNumber == actorNumber);
			if (player != null && (bool)photonViewVCarOrFirstCarMultiPlayer)
			{
				photonViewVCarOrFirstCarMultiPlayer.RPC("SendValueStr", player, a, b, name);
			}
		}
	}

	public void SendValueStrToPlayer(byte a, string b, string name, string target)
	{
		if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
		{
			PhotonView photonViewVCarOrFirstCarMultiPlayer = GetPhotonViewVCarOrFirstCarMultiPlayer();
			Player player = PhotonNetwork.CurrentRoom.Players.Values.FirstOrDefault((Player p) => p.UserId == target);
			if (player != null && photonViewVCarOrFirstCarMultiPlayer != null)
			{
				photonViewVCarOrFirstCarMultiPlayer.RPC("SendValueStr", player, a, b, name);
			}
		}
	}

	public void SendValueStrToPlayer(byte a, string b, string name, Player player)
	{
		PhotonView photonViewVCarOrFirstCarMultiPlayer = GetPhotonViewVCarOrFirstCarMultiPlayer();
		if (player != null && photonViewVCarOrFirstCarMultiPlayer != null)
		{
			photonViewVCarOrFirstCarMultiPlayer.RPC("SendValueStr", player, a, b, name);
		}
	}

	public void OnNewPlayerCreated(int connectedActorNumber, string targetPlayerOwnerUserId)
	{
		if (SceneManager.GetActiveScene().buildIndex == 13)
		{
			FlagsStorage.ForceSendValues = true;
			ResetForceSend();
			SendOwnPersonAndCar(connectedActorNumber, targetPlayerOwnerUserId);
			carsDisableAgain = new List<GameObject>();
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		OnPlayerConnected?.Invoke(newPlayer);
		V.PlayerConnected(newPlayer);
		ShowHideRoom();
	}

	private void ResetForceSendAfter()
	{
		if (!FlagsStorage.IsMainMenu)
		{
			FlagsStorage.ForceSendValues = false;
		}
	}

	public void ResetForceSend()
	{
		Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			ResetForceSendAfter();
		}).AddTo(this);
	}

	private void ShowHideRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
			{
				PhotonNetwork.CurrentRoom.IsVisible = false;
			}
			else
			{
				PhotonNetwork.CurrentRoom.IsVisible = true;
			}
		}
	}

	private void SendOwnPersonAndCar(int connectedActorNumber, string targetPlayerOwnerUserId)
	{
		if (!V.DriverPerson)
		{
			return;
		}
		MultiPersonControl multiPersonControl = V.DriverPerson.GetComponent<MultiPersonControl>();
		if (!multiPersonControl)
		{
			return;
		}
		RaiseRawPositionToAll(true, forceSendValue: true);
		RaiseRawPositionToAll(false, forceSendValue: true);
		RaiseAllControlledCarsRawPosition(connectedActorNumber);
		(from _ in Observable.EveryUpdate()
			where !FlagsStorage.IsDoorOpenAnimation
			select _).Take(1).Subscribe((long _) =>
		{
			SetDriverPersonPlace(multiPersonControl, targetPlayerOwnerUserId);
		}).AddTo(this);
		if (FlagsStorage.InGarageFree)
		{
			return;
		}
		if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			if (_freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>().CarLocked)
			{
				SendValueStrToPlayer(22, _freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>().ViewID.ToString(), "0", connectedActorNumber);
			}
			else
			{
				SendValueStrToPlayer(23, _freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>().ViewID.ToString(), "0", connectedActorNumber);
			}
		}
		Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
		{
			SendCarOtherStates(connectedActorNumber);
		}).AddTo(this);
		Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			SendNowSellingCar();
		}).AddTo(this);
		Observable.Timer(TimeSpan.FromSeconds(2.5)).Subscribe((long _) =>
		{
			SendOwnCarsWheelsTo(connectedActorNumber);
		}).AddTo(this);
	}

	private static void SetDriverPersonPlace(MultiPersonControl multiPersonControl, string targetPlayerOwnerUserId)
	{
		if (FlagsStorage.IsPassenger)
		{
			int placeOnCar = multiPersonControl.placeOnCar;
			multiPersonControl.SendCarEnterPassanger(V.DriverPerson, V.Car, placeOnCar, isSendMulti: true, targetPlayerOwnerUserId, playDefaultDoorSound: false);
		}
		else if (!FlagsStorage.OutOfCar)
		{
			if ((bool)V.Car && (bool)V.DriverPerson)
			{
				multiPersonControl.SendCarEnter(V.DriverPerson, V.Car, isSendMulti: true, targetPlayerOwnerUserId, playDefaultDoorSound: false);
			}
		}
		else if ((bool)V.DriverPerson && (bool)V.LastUsedCar)
		{
			multiPersonControl.SendCarExit(V.DriverPerson, V.LastUsedCar, isSendMulti: true, targetPlayerOwnerUserId, playDefaultDoorSound: false);
		}
	}

	private void SendCarOtherStates(int connectedActorNumber)
	{
		if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			VehicleData component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			if (component.CarHealthComposition.Damaged)
			{
				PhotonView component2 = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>();
				SendValueStrToPlayer(36, component2.ViewID.ToString(), "1", connectedActorNumber);
			}
			if (component.SpacerOpen)
			{
				SendValueStrToPlayer(40, _freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>().ViewID.ToString(), 1.ToString(), connectedActorNumber);
			}
		}
		Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			SendLightStates(connectedActorNumber);
		}).AddTo(this);
		Observable.Timer(TimeSpan.FromSeconds(6.0)).Subscribe((long _) =>
		{
			SendFsoModeInfo(connectedActorNumber);
		}).AddTo(this);
	}

	private void SendLightStates(int connectedActorNumber)
	{
		if ((bool)_freeDriveProvider.QuickCarSettings)
		{
			_freeDriveProvider.QuickCarSettings.SendStateOtherTo(connectedActorNumber);
		}
	}

	private void SendFsoModeInfo(int connectedActorNumber)
	{
		if (V.Car.TryGetComponentInChildren<FsoStrobeLights>(out var component, includeInactive: true))
		{
			component.SendModeByRpcTo(connectedActorNumber);
		}
		else
		{
			Debug.Log("Cant find fso strobe lights in children of car to sent info.");
		}
	}

	private void SendNowSellingCar()
	{
		if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			VehicleData component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
			if (component.СurrentSellCarTrigger != null)
			{
				component.СurrentSellCarTrigger.PriceCar.Get(out int value);
				RpcCarEnteredCarSellTrigger.Send(this, _freeDriveProvider.FirstCarMultiPlayer, component.СurrentSellCarTrigger.CarModel, value, component.СurrentSellCarTrigger.Index, component.СurrentSellCarTrigger.TradeOfferId);
			}
		}
	}

	private void SendOwnCarsWheelsTo(int actorNumber)
	{
		if (PhotonNetwork.CurrentRoom == null)
		{
			return;
		}
		Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);
		if (player == null)
		{
			return;
		}
		if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			MultiPlayerPosControl component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<MultiPlayerPosControl>();
			if ((bool)component && (bool)component.photonView && component.photonView.IsMine && !component.gameObject.activeSelf)
			{
				RpcSendWheelsState.Send(component, player);
			}
		}
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if ((bool)multiPlayerPosControl && (bool)multiPlayerPosControl.photonView && multiPlayerPosControl.photonView.IsMine && !multiPlayerPosControl.gameObject.activeSelf)
			{
				RpcSendWheelsState.Send(multiPlayerPosControl, player);
			}
		}
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		try
		{
			if (Global.TaskManager.Task.Type == TYPE.TAXI)
			{
				Global.TaskManager.TaxiManager.OnUserDisconnect(otherPlayer.ActorNumber);
			}
			else
			{
				Global.TaskManager.DeliveryManager.OnUserDisconnect(otherPlayer.ActorNumber);
			}
			Debug.Log("Henlo");
		}
		catch (Exception)
		{
		}
		OnPlayerDisconnected?.Invoke(otherPlayer);
		V.PlayerDisconnected(otherPlayer);
		ShowHideRoom();
		GameObject carByOwnerID = GetCarByOwnerID(otherPlayer.ActorNumber);
		if ((bool)carByOwnerID)
		{
			GameObject driver = carByOwnerID.GetComponent<VehicleData>().Driver;
			if ((bool)driver)
			{
				PlayerExitFromCar(driver, null);
			}
			VehicleData component = carByOwnerID.GetComponent<VehicleData>();
			component.CleanupMapIcon();
			if ((bool)component.TowingComposition.LoadCar && FlagsStorage.IsFreeDriveMode)
			{
				component.TowingComposition.LoadCar.transform.parent = V.CoreGameManager.globalPoint.transform;
			}
		}
		GameObject personByOwnerID = GetPersonByOwnerID(otherPlayer.ActorNumber);
		GameObject gameObject = null;
		if ((bool)personByOwnerID)
		{
			gameObject = personByOwnerID.GetComponent<MultiPersonControl>().currentCar;
		}
		if (carByOwnerID == _freeDriveProvider.RacingCar2 && (FlagsStorage.RacingGameMode || FlagsStorage.ChasingRaceModeStarted) && _freeDriveProvider.DragRacingControlMultiplayer.TimerOverAll < 2f)
		{
			_freeDriveControl.CloseFinishMenu();
			V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayer.PlayerHasTCar"), 2f);
		}
		if ((bool)personByOwnerID && (bool)gameObject)
		{
			PlayerExitFromCar(personByOwnerID, gameObject);
			OnCarDriverLeft(gameObject);
		}
		if (!FlagsStorage.OutOfCar && (bool)V.Car && otherPlayer.ActorNumber == V.Car.GetComponent<VehicleData>().CarOwnerID)
		{
			bool racingGameMode = FlagsStorage.RacingGameMode;
			if ((FlagsStorage.RacingGameMode && !FlagsStorage.RaceStarted) || FlagsStorage.ChasingRaceModeStarted)
			{
				_freeDriveProvider.DragRacingControlMultiplayer.newRacingMultiplayerController.OnRaceFinished();
			}
			OwnerCarLeft(racingGameMode, carByOwnerID);
		}
		if (FlagsStorage.RacingGameMode && otherPlayer.ActorNumber == _freeDriveControl.LastInvitationReceivedId && !FlagsStorage.RaceStarted)
		{
			_freeDriveControl.CloseFinishMenu();
			V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayer.DisconnectedFromPlayer"), 2f);
		}
		if ((bool)carByOwnerID)
		{
			carByOwnerID.GetComponent<SoundController>();
			VehicleData component2 = carByOwnerID.GetComponent<VehicleData>();
			if ((bool)component2.TowingComposition.LoadCar)
			{
				component2.TowingComposition.UnTowMultiplayer(component2.TowingComposition.LoadCar);
			}
			if ((bool)component2.TowingComposition.TowingTruck)
			{
				component2.TowingComposition.TowingTruck.GetComponent<VehicleData>().TowingComposition.UnTowMultiplayer(carByOwnerID);
			}
			component2.VinylsComposition.DestroyAllRenderTextures();
			OnRemoveFromScale?.Invoke();
		}
	}

	private void OnCarDriverLeft(GameObject personCar)
	{
		if (!personCar.TryGetComponent<VehicleData>(out var component) || (bool)component.Driver)
		{
			return;
		}
		bool flag = false;
		if (component.VehicleMembersController.TryGetFirstPassenger(out var passenger) && passenger.TryGetComponent<MultiPersonControl>(out var component2))
		{
			component.photonView.TransferOwnership(component2.photonView.Owner.ActorNumber);
			flag = V.DriverPerson == passenger;
		}
		else
		{
			component.TryResetCarOwner();
			if (component.mppc.FarFromPerson)
			{
				component.mppc.TeleportToOwner();
			}
			flag = _freeDriveProvider.FirstCarMultiPlayer == personCar;
		}
		if (flag)
		{
			component.GetComponent<Rigidbody>().isKinematic = false;
			component.GetComponent<Powertrain>().OnLineCar = false;
		}
	}

	public GameObject GetCarByOwnerID(int ownerId, bool needToCheckFirstCar = false)
	{
		if (needToCheckFirstCar && PhotonNetwork.LocalPlayer.ActorNumber == ownerId && _freeDriveProvider != null)
		{
			return _freeDriveProvider.FirstCarMultiPlayer;
		}
		return _onlineCollectionsProvider.GetCarByOwnerID(ownerId);
	}

	private GameObject GetPersonByOwnerID(int ownerId)
	{
		return _onlineCollectionsProvider.GetPlayerByOwnerActorNumber(ownerId);
	}

	private void BeginCarStateTracking()
	{
		_carStateTrackingCts?.Cancel();
		_carStateTrackingCts?.Dispose();
		_carStateTrackingCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
		TrackCarStateAfterDoorAsync(_carStateTrackingCts.Token).Forget();
	}

	private async UniTaskVoid TrackCarStateAfterDoorAsync(CancellationToken ct)
	{
		bool startOut = FlagsStorage.OutOfCar;
		bool startPass = FlagsStorage.IsPassenger;
		for (float elapsed = 0f; elapsed < 10f; elapsed += Time.deltaTime)
		{
			if (FlagsStorage.OutOfCar != startOut)
			{
				break;
			}
			if (FlagsStorage.IsPassenger != startPass)
			{
				break;
			}
			await UniTask.Yield(PlayerLoopTiming.Update, ct);
		}
		if (FlagsStorage.OutOfCar != startOut || FlagsStorage.IsPassenger != startPass)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			while (_inputMapSetEvent.CurrentInputGroup != _inputMapSetEvent.GameplayRootGroup)
			{
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
			}
			ApplyInputGroupFromFlags();
		}
	}

	private void ApplyInputGroupFromFlags()
	{
		InputGroupsEnum newGroup = (FlagsStorage.OutOfCar ? InputGroupsEnum.Person : ((!FlagsStorage.IsPassenger) ? ((!FlagsStorage.RacingGameMode) ? InputGroupsEnum.CarFree : (FlagsStorage.SingeDragRacingMode ? InputGroupsEnum.CarDrag : InputGroupsEnum.CarRacing)) : InputGroupsEnum.CarPassenger));
		_inputMapSetEvent.TrySetGroup(newGroup, setRoot: true);
	}

	private void OwnerCarLeft(bool wasRacing, GameObject carWillDestroy)
	{
		Vector3 position = V.Car.transform.position;
		V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayer.OwnerOfThisCarLeft"), 3f);
		bool flag = false;
		BeginCarStateTracking();
		if ((bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			flag = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>().Driver == null;
		}
		if (flag)
		{
			FlagsStorage.IsPassenger = false;
			_lockCarButtonController.UnLockCar(_freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>());
			_coreGameManager.CarOutInControl.InCar(V.DriverPerson, _freeDriveProvider.FirstCarMultiPlayer).Forget();
		}
		else
		{
			GameObject emptyCar = GetEmptyCar(carWillDestroy);
			if ((bool)emptyCar)
			{
				if (!emptyCar.GetComponent<VehicleData>().CarLocked)
				{
					FlagsStorage.IsPassenger = false;
					_coreGameManager.CarOutInControl.InCar(V.DriverPerson, emptyCar).Forget();
				}
				else
				{
					FlagsStorage.IsPassenger = false;
					_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car);
				}
			}
			else
			{
				FlagsStorage.IsPassenger = false;
				_coreGameManager.CarOutInControl.OutCar(V.DriverPerson, V.Car);
			}
		}
		if ((bool)V.Car)
		{
			_freeDriveControl.LoadEnvironmentBySphere(V.SceneControl.getEnvironmentByColliders(V.Car.transform.position));
		}
		else
		{
			_freeDriveControl.LoadEnvironmentBySphere(V.SceneControl.getEnvironmentByColliders(V.DriverPerson.transform.position));
		}
		EnableAllComponent(V.DriverPerson);
		if (wasRacing && (bool)V.Car)
		{
			V.Car.transform.position = position;
		}
		Rigidbody component = V.DriverPerson.GetComponent<Rigidbody>();
		if ((bool)component)
		{
			component.velocity = Vector3.zero;
			component.angularVelocity = Vector3.zero;
		}
	}

	public GameObject GetEmptyCar(GameObject carWillDestroy)
	{
		if (!_freeDriveProvider.FirstCarMultiPlayer)
		{
			return null;
		}
		if (!_freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>().Driver)
		{
			return _freeDriveProvider.FirstCarMultiPlayer;
		}
		return _onlineCollectionsProvider.GetEmptyCar(carWillDestroy);
	}

	private void EnableAllComponent(GameObject person)
	{
		MonoBehaviour[] components = person.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in components)
		{
			if ((bool)monoBehaviour)
			{
				monoBehaviour.enabled = true;
			}
		}
		person.GetComponent<PhotonView>().enabled = true;
	}

	public void AfterForce()
	{
		_forceToSpeakerTimer?.Dispose();
		_forceToSpeakerTimer = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			ForceToSpeaker();
		}).AddTo(this);
	}

	public void ForceToSpeaker()
	{
		iPhoneSpeaker.SetSpeakerOnStart();
	}

	private void UpdatePlayerListReactive(long _ = 0L)
	{
		MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
		foreach (MultiPersonControl mpc in playersOnlineNow)
		{
			if (!(mpc == null) && !listOfActivePlayers.Exists((PlayerMainData x) => x.PlayerId == mpc.ThisPlayerID))
			{
				PlayerMainData playerMainData = new PlayerMainData(mpc.ThisPlayerID, mpc.ThisPlayerAccountID, mpc.ThisPlayerName);
				listOfActivePlayers.Add(playerMainData);
				OnPlayerConnectedWithDataEvent?.Invoke(playerMainData);
			}
		}
		PlayerMainData[] array = listOfActivePlayers.ToArray();
		foreach (PlayerMainData playerMainData2 in array)
		{
			if (!_onlineCollectionsProvider.GetPlayerByPlayerID(playerMainData2.PlayerId))
			{
				OnPlayerDisconnectedWithDataEvent?.Invoke(playerMainData2);
				listOfActivePlayers.Remove(playerMainData2);
			}
		}
	}

	public void KickedFromCar()
	{
		V.CoreGameManager.SetMessage(new LocalizedString("MultiPlayer.Youarekickedfromcar"), 3f);
		FlagsStorage.IsPassenger = false;
		PersonAnimationsController personAnimationsController = V.DriverPerson.GetComponent<MainPerson>().PersonAnimationsController;
		bool flag = (bool)personAnimationsController && personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate;
		bool flag2 = (bool)personAnimationsController && personAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
		try
		{
			if (flag)
			{
				personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.CancelSeat();
			}
			else if (flag2)
			{
				personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.ForceGetOut();
			}
			else if ((bool)personAnimationsController)
			{
				GameObject gameObject = V.Car;
				if (V.DriverPerson.TryGetComponent<MultiPersonControl>(out var component) && (bool)component.currentCar)
				{
					gameObject = component.currentCar;
				}
				if ((bool)gameObject && (bool)gameObject.GetComponent<VehicleData>())
				{
					personAnimationsController.SitGetOutCarSystem.GetOutCar(gameObject, playAnimation: false);
				}
			}
		}
		catch (Exception)
		{
		}
		Debug.Log("Enable driver Person components");
		EnableAllComponent(V.DriverPerson);
		if (!flag && !flag2 && (bool)personAnimationsController && personAnimationsController.AnimationService != null)
		{
			personAnimationsController.AnimationService.TurnOnOffCollider(V.DriverPerson, isOn: true);
		}
		if (V.DriverPerson.TryGetComponent<NewMoveController>(out var component2))
		{
			component2.enabled = true;
		}
		_inputMapSetEvent.TrySetGroup(InputGroupsEnum.Person, setRoot: true);
		if (V.DriverPerson.TryGetComponent<Rigidbody>(out var component3))
		{
			component3.velocity = Vector3.zero;
			component3.angularVelocity = Vector3.zero;
		}
	}

	public void UpdateDataOpenDoors(int carId, string data)
	{
		GameObject carByPhotonViewID = GetCarByPhotonViewID(carId);
		if (!carByPhotonViewID || !carByPhotonViewID.TryGetComponent<CarAnimationsController>(out var component))
		{
			return;
		}
		int num = 0;
		foreach (KeyValuePair<CarDoorType, bool> openDoor in component.OpenDoors)
		{
			_openedDoors.Add(openDoor.Key, data[num] == '1');
			num++;
		}
		foreach (KeyValuePair<CarDoorType, bool> openedDoor in _openedDoors)
		{
			component.OpenDoors[openedDoor.Key] = openedDoor.Value;
		}
		_openedDoors.Clear();
	}

	private PhotonView GetPhotonViewVCarOrFirstCarMultiPlayer()
	{
		if ((bool)V.Car)
		{
			return PhotonView.Get(V.Car);
		}
		if (_freeDriveProvider != null && (bool)_freeDriveProvider.FirstCarMultiPlayer)
		{
			return PhotonView.Get(_freeDriveProvider.FirstCarMultiPlayer);
		}
		if ((bool)V.DriverPerson)
		{
			return PhotonView.Get(V.DriverPerson);
		}
		return null;
	}
}
