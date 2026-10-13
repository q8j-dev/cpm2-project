using System;
using System.Collections.Generic;
using System.Linq;
using CPM.DailyTasks;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Photon.Pun;
using Photon.Realtime;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;

public class MultiPlayerMenu : MonoBehaviourPunCallbacks
{
	public class CheckData
	{
		public string var2 = "";

		public static bool UseHash => !IsTestBuild.Check;

		public CheckData()
		{
			if (!UseHash)
			{
				var2 = "ios7";
			}
		}
	}

	private float _timer;

	private IDisposable _updateListFriendUITick;

	private IDisposable _requestFriendListTick;

	public GameObject RoomItemPrefab;

	public Transform ParentRoomList;

	public int SelectedEnvironment;

	public GameObject ConnectingWindow;

	public GameObject StartWindow;

	public GameObject BuyHouseWindow;

	public GameObject SinglePlayerSelect;

	public GameObject allEnvironmentParent;

	public GameObject AllHomeParent;

	private bool friendSearchDone;

	private bool friendExists;

	private string mailChecking;

	public GameObject passwordWindow;

	public bool showingPasswordInput;

	public List<FriendInfo> friendListArray;

	public RoomInfo[] roomList_old;

	public bool friendListReceived;

	public bool friendRequestSend;

	public GameObject loadingIcon;

	private List<string> Lastints = new List<string>();

	public RoomDataItem lastRoomDataItem;

	public string verConnect;

	public string editorVer;

	public LeaderWindow leaderWindow;

	public GameObject addByEmailWindow;

	public GameObject addByIDWindow;

	private ObscuredInt selectedHome = 0;

	private ObscuredInt idMain = 0;

	private ObscuredInt CorrectHash = 0;

	public Color selectedSinglePlayer = Color.blue;

	public List<GameObject> photonObjects;

	private bool firstOpen = true;

	private PlayerDatas.FriendsUtility friendsUtility;

	private PlayerDataStorage playerDataStorage;

	private DailyTasksManager dailyTasksManager;

	private IPrefabFactory prefabFactory;

	private const string excludedInput = "Enter ID to ADD...";

	private ManagerUI _managerUI;

	private CurrencyManagerController _currencyManagerController;

	private CoreGameManager _coreGameManager;

	private FreeDriveProvider _freeDriveProvider;

	[Inject]
	public void Construct(PlayerDatas.FriendsUtility friendsUtility, CoreGameManager coreGameManager, FreeDriveProvider freeDriveProvider, PlayerDataStorage playerDataStorage, DailyTasksManager dailyTasksManager, IPrefabFactory prefabFactory, ManagerUI managerUI, CurrencyManagerController currencyManagerController)
	{
		_freeDriveProvider = freeDriveProvider;
		_coreGameManager = coreGameManager;
		_currencyManagerController = currencyManagerController;
		this.friendsUtility = friendsUtility;
		this.playerDataStorage = playerDataStorage;
		this.dailyTasksManager = dailyTasksManager;
		this.prefabFactory = prefabFactory;
		_managerUI = managerUI;
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public void OpenMultiPlayerMenu()
	{
	}

	private void RemoveFriendWithID(string IDRemove)
	{
		foreach (PrefFriendData friend in friendsUtility.Friends)
		{
			if (IDRemove == friend.id)
			{
				PlayerPrefs.DeleteKey("FriendNo" + friend.OrderNumber);
				friendsUtility.Friends.Remove(friend);
				break;
			}
		}
	}

	private void AddFriend(string IDAdd, string NameAdd, string accountID)
	{
		if (friendsUtility.Friends == null)
		{
			return;
		}
		for (int i = 0; i <= 120; i++)
		{
			if (!PlayerPrefs.HasKey("FriendNo" + i))
			{
				PrefFriendData prefFriendData = new PrefFriendData();
				PlayerPrefs.SetInt("FriendNo" + i, 1);
				prefFriendData.OrderNumber = i;
				PlayerPrefs.SetString("FriendID" + i, IDAdd);
				prefFriendData.id = IDAdd;
				PlayerPrefs.SetString("FriendName" + i, NameAdd);
				prefFriendData.Name = NameAdd;
				PlayerPrefs.SetString("FriendAccountID" + i, accountID);
				prefFriendData.accountID = accountID;
				friendsUtility.Friends.Add(prefFriendData);
				break;
			}
		}
	}

	private void RequestFriendListFromPhoton()
	{
		List<string> list = new List<string>();
		list = friendsUtility.Friends.Select((PrefFriendData n) => n.id).ToList();
		if (friendsUtility.Friends.Count > 0 && !friendRequestSend)
		{
			PhotonNetwork.FindFriends(list.ToArray());
			friendRequestSend = true;
		}
	}

	public void UpdateListFriendUI()
	{
		if (showingPasswordInput || PhotonNetwork.NetworkClientState == ClientState.ConnectingToMasterServer || lastRoomDataItem != null)
		{
			return;
		}
		if (!PhotonNetwork.IsConnected)
		{
			if ((int)idMain == 2)
			{
				if ((int)CorrectHash != 0)
				{
				}
			}
			else
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = verConnect;
				Debug.Log("CONNECT TO PHOTON 5");
				PhotonNetwork.ConnectUsingSettings();
			}
		}
		else
		{
			ConnectingWindow.SetActive(value: false);
			StartWindow.SetActive(value: true);
		}
		loadingIcon.gameObject.SetActive(value: true);
		foreach (Transform parentRoom in ParentRoomList)
		{
			UnityEngine.Object.Destroy(parentRoom.gameObject);
		}
		if (friendsUtility.Friends.Count == 0)
		{
			loadingIcon.gameObject.SetActive(value: false);
		}
		else
		{
			if (!friendListReceived || !PhotonNetwork.IsConnected)
			{
				return;
			}
			List<PrefFriendData> list = new List<PrefFriendData>();
			if (friendListArray != null)
			{
				foreach (FriendInfo item in friendListArray)
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
			foreach (PrefFriendData friend in friendsUtility.Friends)
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
			int num = 0;
			foreach (PrefFriendData item3 in list)
			{
				LoadFrientItemList(item3, num);
				num++;
			}
			loadingIcon.gameObject.SetActive(value: false);
			if (firstOpen)
			{
				Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
				{
					ResetScrollBar();
				}).AddTo(this);
			}
		}
	}

	private void LoadFrientItemList(PrefFriendData friend, int i)
	{
		string id = friend.Name;
		int num = 0;
		if (friend.Name == "")
		{
			id = friend.id;
		}
		GameObject obj = prefabFactory.Instantiate(RoomItemPrefab, Vector3.zero, Quaternion.identity);
		obj.transform.parent = ParentRoomList;
		obj.transform.localScale = new Vector3(0.87f, 0.8f, 1f);
		obj.transform.localPosition = new Vector3(31f, i * -85 + 900, 15f);
		RoomDataItem component = obj.GetComponent<RoomDataItem>();
		component.accountID = friend.id;
		component.FriendIDInRoom = friend.id;
		component.strLabel = id;
		if (friend.Status == "online" || friend.Status == "inroom")
		{
			if (friend.Status == "online")
			{
				id += " Online";
			}
			if (friend.Status == "inroom")
			{
				component.FriendInRoom = true;
				component.RoomNameToJoin = friend.RoomName;
				component.maxPlayerInRoom = 20;
				component.currentPlayerInRoom = 1;
				component.ResetLabelText();
			}
			if (friend.RoomName != null)
			{
				num = int.Parse(friend.RoomName.Split("_"[0])[1]);
				if (num > 10000)
				{
					component.passWord = 0;
				}
				else
				{
					component.passWord = num;
				}
				component.isOnlinePoint.SetActive(value: true);
			}
		}
		else
		{
			component.isOnlinePoint.SetActive(value: false);
		}
	}

	public void TryToJoinRoom(RoomDataItem roomDataItem)
	{
		lastRoomDataItem = roomDataItem;
		PhotonNetwork.JoinRoom(lastRoomDataItem.RoomNameToJoin);
	}

	private void ResetScrollBar()
	{
		firstOpen = false;
	}

	private void StartInvokeRequestFriendList()
	{
		_requestFriendListTick?.Dispose();
		_requestFriendListTick = Observable.Timer(TimeSpan.FromSeconds(7.0), TimeSpan.FromSeconds(15.0)).Subscribe((long _) =>
		{
			RequestFriendListFromPhoton();
		}).AddTo(this);
	}

	private bool hasHouse(int check)
	{
		if (ObscuredPrefs.GetInt("homeOpen") == 1)
		{
			ObscuredPrefs.SetInt("homeOpen0" + 0, 1);
		}
		if (check == 2)
		{
			return true;
		}
		return ObscuredPrefs.GetInt("homeOpen" + check) == 1;
	}

	public void StartSingleFreeDrive()
	{
		if (V.PlaceToSpawn == V.placeToSpawn.House)
		{
			if (hasHouse(selectedHome) && ((int)selectedHome != 2 || ObscuredPrefs.GetInt("homeOpen2") != 0))
			{
				if ((int)selectedHome == 0 || (int)selectedHome == 3)
				{
					V.PlaceToSpawn = V.placeToSpawn.MainCity;
				}
				else
				{
					V.PlaceToSpawn = V.placeToSpawn.City2;
				}
				FlagsStorage.StartInHome = true;
				ObscuredPrefs.SetInt("homeSelected", selectedHome);
				V.CoreGameManager.menuControl.LoadFreeDrive(loadingWindowOnTop: false);
			}
		}
		else
		{
			FlagsStorage.StartInHome = false;
			_managerUI.CloseMenuControllerHUD();
			V.CoreGameManager.menuControl.LoadFreeDrive(loadingWindowOnTop: false);
		}
	}

	public void StartFreeDrive()
	{
		V.RoomNameToJoin = "";
		V.CoreGameManager.menuControl.ShowLoadingAndLoadMultiFreeDrive();
	}

	public void OnCreateForFriendsOnlyPressed()
	{
		passwordWindow.gameObject.SetActive(FlagsStorage.CreateForFriendOnly);
	}

	public void ShowAllHomes()
	{
		allEnvironmentParent.SetActive(value: false);
		AllHomeParent.SetActive(value: true);
		BuyHouseWindow.SetActive(value: false);
	}

	public void ShowAllEnvironment()
	{
		allEnvironmentParent.SetActive(value: true);
		BuyHouseWindow.SetActive(value: false);
		AllHomeParent.SetActive(value: false);
		SelectedSinglePlayer();
	}

	public void BuyHouseCancel()
	{
		BuyHouseWindow.SetActive(value: false);
		SinglePlayerSelect.SetActive(value: true);
	}

	public void SelectedSinglePlayer()
	{
		if (SelectedEnvironment == 7)
		{
			ShowAllHomes();
		}
	}

	public void UpdatePlayerRecords()
	{
		int num = ObscuredPrefs.GetInt("RaceWin");
		int num2 = ObscuredPrefs.GetInt("RaceLose");
		int num3 = num + num2;
		if (num3 != 0)
		{
			_ = (float)num / (float)num3;
		}
	}

	public void CloseMultiPlayerMenu()
	{
		V.CoreGameManager.menuControl.ShowIsMultiMenu();
	}
}
