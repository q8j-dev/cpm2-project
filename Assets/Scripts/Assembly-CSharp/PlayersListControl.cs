using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Account;
using CPM.PlayerData;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;

public class PlayersListControl : WindowScript, IBackableWindow
{
	public const float MoveAnimDuration = 0.2f;

	public Transform ParentAllPlayers;

	public OnePlayerUI PrefabOnePlayerUI;

	[SerializeField]
	private TextMeshProUGUI _playerCountLabel;

	[SerializeField]
	private RectTransform _mask;

	[SerializeField]
	private RectTransform _root;

	[SerializeField]
	private Button _buttonClose;

	[SerializeField]
	private Button _buttonMainPlayerProfile;

	[SerializeField]
	private FaderCanvas _faderCanvasMainPlayerProfile;

	[SerializeField]
	private CanvasGroup CanvasGroup;

	private PlayerProfileWindow _playerProfile;

	private UniTask _loadProfileTask;

	private PlayerDataStorage _playerDataStorage;

	private IPrefabFactory _prefabFactory;

	private Action<MultiPersonControl> _onPlayerJoinedHandler;

	private Dictionary<MultiPersonControl, OnePlayerUI> _players = new Dictionary<MultiPersonControl, OnePlayerUI>();

	private bool _openProfileState;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private ChatControl _chatControl;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	[Inject]
	public void Construct(PlayerDatas.FriendsUtility friendsUtility, PlayerDataStorage playerDataStorage, IAccountController accountController, IPrefabFactory prefabFactory, LevelsProvider levelsProvider, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider, ChatControl chatControl)
	{
		_chatControl = chatControl;
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_onlineCollectionsProvider = onlineCollectionsProvider;
		_prefabFactory = prefabFactory;
		_playerDataStorage = playerDataStorage;
	}

	protected override void Awake()
	{
		_freeDriveProvider.PlayerListControl = this;
		_onPlayerJoinedHandler = (MultiPersonControl player) =>
		{
			AddOnePlayer(player.gameObject, 1);
		};
		PersonMultiplayerDataSending.OnPersonDataApplied += _onPlayerJoinedHandler;
		PersonMultiplayerDataSending.OnPersonDataRemoved += RemoveOnePlayer;
	}

	protected override void Start()
	{
		InitBackButton();
		base.Start();
		GlobalEventSystem.OnGamePaused?.Invoke(obj: true);
		_managerUI.ShowUserBalanceOnlyWindow().Forget();
		_loadProfileTask = LoadPlayerProfileWindow().Preserve();
		UpdatePlayersList();
		_buttonClose.onClick.AddListener(() =>
		{
			CloseWindow();
		});
		_buttonMainPlayerProfile.onClick.AddListener(OpenPlayerProfileWindow);
	}

	protected override void OnEnable()
	{
		_buttonClose.gameObject.SetActive(value: false);
		base.OnEnable();
	}

	public void Back()
	{
		CloseWindow();
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: false);
		_managerUI.SetCurrentWindow(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_freeDriveProvider.PlayerListControl = null;
		PersonMultiplayerDataSending.OnPersonDataApplied -= _onPlayerJoinedHandler;
		PersonMultiplayerDataSending.OnPersonDataRemoved -= RemoveOnePlayer;
		if ((bool)_playerProfile)
		{
			_playerProfile.Close();
		}
	}

	public void CloseWindow(bool needUnclean = true)
	{
		if (needUnclean)
		{
			_levelsProvider.LevelScript.UnCleanScreen();
		}
		if (FlagsStorage.IsChatAllowed)
		{
			_chatControl.ShowPartiallyChat();
		}
		_managerUI.CloseSeveralWindows();
		GlobalEventSystem.OnGamePaused?.Invoke(obj: false);
		Close();
	}

	public void UpdatePlayersList()
	{
		string text = (PhotonNetwork.IsConnected ? $"({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})" : "");
		_playerCountLabel.text = string.Concat(new LocalizedString("ChatWindow.Players"), ": ", text);
		UpdateOnlinePlayers();
	}

	private void OpenPlayerProfileWindow()
	{
		ShowAchievements().Forget();
	}

	private void UpdateOnlinePlayers()
	{
		int num = 0;
		foreach (Transform item in ParentAllPlayers.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		List<GameObject> list = (from player in _onlineCollectionsProvider.PlayersOnlineNow
			where player
			select player.gameObject).ToList();
		list.Insert(0, V.DriverPerson);
		foreach (GameObject item2 in list)
		{
			if ((bool)item2)
			{
				AddOnePlayer(item2, num);
				num++;
			}
		}
	}

	private void AddOnePlayer(GameObject person, int i)
	{
		MultiPersonControl component = person.GetComponent<MultiPersonControl>();
		if (!_players.ContainsKey(component))
		{
			OnePlayerUI onePlayerUI = _prefabFactory.Instantiate(PrefabOnePlayerUI, ParentAllPlayers);
			_players.Add(component, onePlayerUI);
			OnePlayerInListData onePlayerData = new OnePlayerInListData();
			if (i == 0)
			{
				onePlayerData.PlayerName = _playerDataStorage.PlayerName;
				onePlayerData.PlayerId = _playerDataStorage.PlayerID;
				onePlayerData.AccountId = _playerDataStorage.AccountID;
				component.rank = GetOwnRank();
				component.FirstAndOwnCar = _freeDriveProvider.FirstCarMultiPlayer;
			}
			else
			{
				onePlayerData.PlayerName = component.ThisPlayerName;
				onePlayerData.PlayerId = component.ThisPlayerID;
				onePlayerData.AccountId = component.ThisPlayerAccountID;
			}
			onePlayerData.Person = person;
			onePlayerData.MultiPersonControl = component;
			onePlayerUI.Init(onePlayerData, () =>
			{
				_ = CreateProfileWindow(onePlayerData);
			});
		}
	}

	private void RemoveOnePlayer(MultiPersonControl multiPersonControl)
	{
		if ((bool)multiPersonControl && _players.TryGetValue(multiPersonControl, out var value) && (bool)value)
		{
			UnityEngine.Object.Destroy(value.gameObject);
			_players.Remove(multiPersonControl);
			string text = (PhotonNetwork.IsConnected ? $"({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})" : "");
			_playerCountLabel.text = string.Concat(new LocalizedString("ChatWindow.Players"), ": ", text);
			bool flag = (bool)_managerUI.AchievementsWindow && _managerUI.AchievementsWindow.gameObject.activeInHierarchy;
			if ((bool)_playerProfile && _playerProfile.CurrentPlayer == multiPersonControl && !flag)
			{
				TryCloseProfileWindow();
			}
		}
	}

	private void TryCloseProfileWindow()
	{
		_managerUI.ShowNotificationWindow(new LocalizedString("PlayersListControl.Playeralreadylefttheroom"), 2f, Color.red);
		OpenProfile(state: false);
	}

	private async UniTaskVoid CreateProfileWindow(OnePlayerInListData onePlayerInListData)
	{
		if (onePlayerInListData.Person == null)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("PlayersListControl.Playeralreadylefttheroom"), 2f, Color.red);
			return;
		}
		if (!_playerProfile)
		{
			await _loadProfileTask;
		}
		if ((bool)_playerProfile)
		{
			CanvasGroup.alpha = 0f;
			CanvasGroup.interactable = false;
			_playerProfile.OpenInPlayerList(onePlayerInListData, OpenProfile);
			OpenProfile(state: true);
		}
	}

	public void SetInteractableTouch(bool state)
	{
		CanvasGroup.blocksRaycasts = state;
	}

	private void OpenProfile(bool state)
	{
		if (_openProfileState != state)
		{
			_openProfileState = state;
			CanvasGroup.alpha = ((!state) ? 1 : 0);
			CanvasGroup.interactable = !state;
			_playerProfile.gameObject.SetActive(state);
			_faderCanvasMainPlayerProfile.SetActive(active: true);
			_faderCanvasMainPlayerProfile.Fade((!state) ? 1 : 0, 0.2f, () =>
			{
				_faderCanvasMainPlayerProfile.SetActive(!state);
			});
			if (!state)
			{
				_playerProfile.Reset();
			}
			if (state)
			{
				_playerProfile.InitBackButton();
			}
		}
	}

	public static int GetOwnRank()
	{
		return ObscuredPrefs.GetInt("rank" + DateTime.Now.Day);
	}

	private async UniTask LoadPlayerProfileWindow()
	{
		_playerProfile = await _managerUI.ShowPlayerProfileWindow();
	}

	private async UniTaskVoid ShowAchievements()
	{
		HideWindow();
		await _managerUI.ShowWindow("AchievmentsWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
		await UniTask.Yield();
		if ((bool)_managerUI.AchievementsWindow)
		{
			AchievementsWindow achievementsWindow = _managerUI.AchievementsWindow;
			achievementsWindow.OnAchievementsWindowClosed = (Action)Delegate.Combine(achievementsWindow.OnAchievementsWindowClosed, new Action(ShowWindow));
			_managerUI.AchievementsWindow.gameObject.SetActive(value: true);
			_managerUI.AchievementsWindow.OnAchievementsWindowOpen();
		}
		else
		{
			ShowWindow();
		}
	}

	private void HideWindow()
	{
		ShowUI(state: false);
	}

	private void ShowWindow()
	{
		if ((bool)_managerUI.AchievementsWindow)
		{
			AchievementsWindow achievementsWindow = _managerUI.AchievementsWindow;
			achievementsWindow.OnAchievementsWindowClosed = (Action)Delegate.Remove(achievementsWindow.OnAchievementsWindowClosed, new Action(ShowWindow));
		}
		ShowUI(state: true);
	}
}
