using System;
using CPM.Account;
using CPM.Connection;
using CPM.PlayerData;
using CPMEngine;
using Car_Parking.Scripts.Racing;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Photon.Pun;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameFacts.Consumers;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement;

public class PlayerProfileWindow : WindowScript, IBackableWindow
{
	public const float MinDistanceToUser = 200f;

	public const float WaitingWindowInterval = 5f;

	private const int SendMinLimit = 50000;

	private const int SendMinLimitPerDay = 500000;

	private string _notYourRoomMsg;

	private string _thanksForReportMsg;

	private string _playerLeftMsg;

	private string _requestSendMsg;

	private string _alreadyAddedMsg;

	private string _carExchangeOfferSentMsg;

	private string _cantExchangeNoCarMsg;

	private string _youDontHaveSuchMoneyMsg;

	private string _youMustHaveAccToSendMoney;

	private string _enterCorrectNumberMsg;

	private string _userNotAuthorized;

	[SerializeField]
	private GameObject[] CarLoadingIndicators;

	[SerializeField]
	private Button ButtonAddFriend;

	[SerializeField]
	private Button ButtonKick;

	[SerializeField]
	private Button ButtonClose;

	[SerializeField]
	private Button ButtonConfirm;

	[SerializeField]
	private Button ButtonCancel;

	[SerializeField]
	private Image ImageRank;

	[SerializeField]
	private Image ImageCar;

	[SerializeField]
	private Image ImageCarToExchange;

	[SerializeField]
	private Image ImageExchangeOwnCar;

	[SerializeField]
	private Image ImageUnmuted;

	[SerializeField]
	private Image ImageMuted;

	[SerializeField]
	private TextMeshProUGUI TextTorque;

	[SerializeField]
	private TextMeshProUGUI TextHorsePower;

	[SerializeField]
	private TextMeshProUGUI TextId;

	[SerializeField]
	private TextMeshProUGUI TextFriend;

	[SerializeField]
	private TextMeshProUGUI TextReport;

	[SerializeField]
	private TextMeshProUGUI RankText;

	[SerializeField]
	private TMP_InputField InputFieldMoneyToSend;

	[SerializeField]
	private TMP_InputField InputFieldSellCar;

	[SerializeField]
	private GameObject MainButtonsPanel;

	[SerializeField]
	private GameObject PanelSendMoney;

	[SerializeField]
	private GameObject PanelCarExchange;

	[SerializeField]
	private GameObject PanelReport;

	[SerializeField]
	private GameObject PanelSellCar;

	[SerializeField]
	private GameObject PanelKick;

	[SerializeField]
	private GameObject SubmitButtonsPanel;

	[SerializeField]
	private RectTransform RectMoveAnimation;

	[SerializeField]
	private RectTransform RootRect;

	[SerializeField]
	private Button ButtonAchievements;

	[SerializeField]
	private Button ButtonMessage;

	[SerializeField]
	private Button ButtonSendMoney;

	[SerializeField]
	private Button ButtonCarExchange;

	[SerializeField]
	private Button ButtonReport;

	[SerializeField]
	private Button ButtonSellCar;

	[SerializeField]
	private Button ButtonSpectate;

	[SerializeField]
	private CanvasGroup CurrentCanvas;

	[SerializeField]
	private ShowHideVinylForOtherCarButton ShowHideVinylForOtherCarButton;

	[SerializeField]
	private Text TextName;

	[SerializeField]
	private SpectatorWindow SpectatorWindow;

	[SerializeField]
	private GameObject MainPanel;

	[SerializeField]
	private Image MainBackground;

	[SerializeField]
	private Image MainOutlineBackground;

	[SerializeField]
	private Toggle MuteToggle;

	[SerializeField]
	private Toggle BlockToggle;

	[SerializeField]
	private ToggleSwitchHandler MuteSwitcher;

	[SerializeField]
	private ToggleSwitchHandler BlockSwitcher;

	[SerializeField]
	private Button CloseWindowFromChatButton;

	private UnityAction _backActionOnProfileWindow;

	private bool _buttonsState = true;

	private bool _isAnimating;

	private bool _fromPlayerList;

	private bool _isDestroyed;

	private GameObject _openedPanel;

	private OnePlayerInListData _onePlayerInListData;

	private PlayerDataStorage _playerDataStorage;

	private MultiPersonControl _mpc;

	private GameObject _removeConfirmPanel;

	private IAccountController _accountController;

	private PlayerDatas.FriendsUtility _friendsUtility;

	private ChatControl _chatControl;

	private IConnectionManager _connectionManager;

	private CurrencyManagerController _currencyManagerController;

	private LevelScript _levelScript;

	private FreeDriveControl _freeDriveControl;

	private MultiPlayer _multiPlayer;

	private FreeDriveProvider _freeDriveProvider;

	private bool _isMainPanelShowing;

	private Powertrain _mainUsersCar;

	private ISellExchangeFreeService _sellExchangeFreeService;

	private MoneySendService _moneySendService;

	private AddFriendService _addFriendService;

	private FreeTimeAttackController _timeAttackController;

	public bool IsFromChat;

	private ISteamAchievementService _steamAchievementService;

	protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

	public MultiPersonControl CurrentPlayer => _mpc;

	public bool IsChatProfileOpen
	{
		get
		{
			if (!_fromPlayerList)
			{
				return MainPanel.activeSelf;
			}
			return false;
		}
	}

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, ManagerUI managerUI, IAccountController accountController, PlayerDatas.FriendsUtility friendsUtility, IConnectionManager connectionManager, ChatControl chatControl, CurrencyManagerController currencyManagerController, MultiPlayer multiPlayer, LevelScript levelScript, FreeDriveControl freeDriveControl, MoneySendService moneySendService, FreeDriveProvider freeDriveProvider, ISellExchangeFreeService sellExchangeFreeService, AddFriendService addFriendService, FreeTimeAttackController timeAttackController, ISteamAchievementService steamAchievementService)
	{
		_timeAttackController = timeAttackController;
		_addFriendService = addFriendService;
		_moneySendService = moneySendService;
		_freeDriveProvider = freeDriveProvider;
		_multiPlayer = multiPlayer;
		_freeDriveControl = freeDriveControl;
		_levelScript = levelScript;
		_currencyManagerController = currencyManagerController;
		_chatControl = chatControl;
		_playerDataStorage = playerDataStorage;
		_managerUI = managerUI;
		_accountController = accountController;
		_friendsUtility = friendsUtility;
		_connectionManager = connectionManager;
		_sellExchangeFreeService = sellExchangeFreeService;
		_steamAchievementService = steamAchievementService;
	}

	protected override void Start()
	{
		base.Start();
		_isMainPanelShowing = true;
		_notYourRoomMsg = new LocalizedString("PlayerProfile.Itisnotyourroom.\nYouneedtocreateownroom.");
		_thanksForReportMsg = new LocalizedString("PlayerProfile.Thankyouforreport");
		_playerLeftMsg = new LocalizedString("PlayerProfile.Playerleft!");
		_requestSendMsg = new LocalizedString("PlayerProfile.Requestsent!");
		_alreadyAddedMsg = new LocalizedString("PlayerProfile.Alreadyadded!");
		_carExchangeOfferSentMsg = new LocalizedString("PlayerProfile.CarExchangeoffersent");
		_cantExchangeNoCarMsg = new LocalizedString("Mainmenuwindow.YouNeedCar");
		_youDontHaveSuchMoneyMsg = new LocalizedString("PlayerProfile.Youdon'thavesuchmoney:)");
		_youMustHaveAccToSendMoney = new LocalizedString("PlayerProfile.Youmusthaveanaccounttosendmoneytoanotherplayer!");
		_enterCorrectNumberMsg = new LocalizedString("PlayerProfile.Entercorrectnumber");
		_userNotAuthorized = new LocalizedString("PlayerProfile.Userisnotauthorized!");
		_managerUI.CurrentPlayerProfile = this;
		BlockToggle.onValueChanged.AddListener(TryBlockPlayer);
		MuteToggle.onValueChanged.AddListener(TryMutePlayer);
		ButtonKick.onClick.AddListener(() =>
		{
			ShowPanel(PanelKick);
			ButtonKick.interactable = false;
		});
		ButtonAddFriend.onClick.AddListener(InviteToFriend);
		ButtonMessage.onClick.AddListener(MessagePlayer);
		ButtonSendMoney.onClick.AddListener(OnSendMoneyClicked);
		ButtonCarExchange.onClick.AddListener(OnCarExchangeClicked);
		ButtonReport.onClick.AddListener(() =>
		{
			ShowPanel(PanelReport);
		});
		ButtonSellCar.onClick.AddListener(() =>
		{
			ShowMainPanelOutline(state: true);
			ShowPanel(PanelSellCar);
		});
		ButtonConfirm.onClick.AddListener(ConfirmClicked);
		ButtonCancel.onClick.AddListener(ShowMainPanel);
		if (FlagsStorage.IsPrivateRoom)
		{
			ButtonSpectate.onClick.AddListener(() =>
			{
				SpectatePlayer(_onePlayerInListData.MultiPersonControl, state: true);
			});
		}
		CloseWindowFromChatButton.onClick.AddListener(CloseWindowFromChat);
		PersonMultiplayerDataSending.OnPersonDataRemoved += TryCloseProfile;
		SceneManager.sceneLoaded += OnSceneLoaded;
		ManagerUI managerUI = _managerUI;
		managerUI.OnShowScreen = (Action<bool>)Delegate.Combine(managerUI.OnShowScreen, new Action<bool>(ShowUI));
	}

	private void OnSendMoneyClicked()
	{
		if (!CanSendOffer())
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("MultiPlayerPosControl.PlayerIsBusy"), 3f).Forget();
		}
		else
		{
			ShowPanel(PanelSendMoney);
		}
	}

	private void OnCarExchangeClicked()
	{
		if (!_freeDriveProvider.FirstCarMultiPlayer || ((bool)_mpc && !_mpc.FirstAndOwnCar))
		{
			_managerUI.ShowNotificationWindow(_cantExchangeNoCarMsg, 3f).Forget();
		}
		else
		{
			ShowPanel(PanelCarExchange);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
	{
		if (SpectatorWindow.gameObject.activeSelf)
		{
			_managerUI.CloseNotTransparentWaitingWindow();
		}
		else
		{
			ReturnToUsersCar().Forget();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		_isDestroyed = true;
		SceneManager.sceneLoaded -= OnSceneLoaded;
		if ((bool)_managerUI.AchievementsWindow)
		{
			AchievementsWindow achievementsWindow = _managerUI.AchievementsWindow;
			achievementsWindow.OnAchievementsWindowClosed = (Action)Delegate.Remove(achievementsWindow.OnAchievementsWindowClosed, new Action(ShowWindow));
		}
		if (IsFromChat)
		{
			_managerUI.OnShowScreen?.Invoke(obj: true);
		}
		PersonMultiplayerDataSending.OnPersonDataRemoved -= TryCloseProfile;
		ChatControl chatControl = _chatControl;
		chatControl.OnProfileWindowClosedFromChat = (Action)Delegate.Remove(chatControl.OnProfileWindowClosedFromChat, new Action(CloseMainPanel));
		ManagerUI managerUI = _managerUI;
		managerUI.OnShowScreen = (Action<bool>)Delegate.Remove(managerUI.OnShowScreen, new Action<bool>(ShowUI));
		ButtonClose.onClick.RemoveAllListeners();
		BlockToggle.onValueChanged.RemoveAllListeners();
	}

	private void TryCloseProfile(MultiPersonControl multiPersonControl)
	{
		if (!(_mpc != multiPersonControl) && !_fromPlayerList)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("PlayersListControl.Playeralreadylefttheroom"), 2f, Color.red);
			Reset();
			CloseWithUnclean();
		}
	}

	private void ShowMainPanelOutline(bool state)
	{
		MainBackground.enabled = !state;
		MainOutlineBackground.SetActive(state);
	}

	public void CloseWithUnclean()
	{
		if (!base.IsCloseCanceled)
		{
			base.Close();
			_managerUI.CloseSeveralWindows();
			_levelScript.UnCleanScreen();
		}
	}

	public void Back()
	{
		if (IsFromChat)
		{
			CloseWindowFromChat();
		}
		else if (_fromPlayerList)
		{
			_backActionOnProfileWindow();
		}
	}

	public void InitBackButton()
	{
		_managerUI.OnBackButtonActivated?.Invoke(obj: false);
		_managerUI.SetCurrentWindow(this);
	}

	public void MoveAnimation(float toMove)
	{
		Vector2 endValue = RootRect.anchoredPosition + new Vector2(toMove, 0f);
		RootRect.DOAnchorPos(endValue, 0.2f).SetUpdate(isIndependentUpdate: true);
	}

	public void OpenInPlayerList(OnePlayerInListData onePlayerInListData, Action<bool> callback)
	{
		Init(onePlayerInListData);
		_backActionOnProfileWindow = () =>
		{
			SubmitButtonsPanel?.SetActive(value: false);
			ButtonClose.onClick.RemoveListener(_backActionOnProfileWindow);
			callback?.Invoke(obj: false);
		};
		ButtonClose.onClick.AddListener(_backActionOnProfileWindow);
		_fromPlayerList = true;
		IsFromChat = false;
		CloseWindowFromChatButton.SetActive(active: false);
	}

	public void OpenInChat(MultiPersonControl multiPersonControl)
	{
		InitByMultiPersonControl(multiPersonControl);
		ButtonClose.onClick.AddListener(CloseWithUnclean);
		InitBackButton();
		_fromPlayerList = false;
		IsFromChat = true;
		base.gameObject.SetActive(value: true);
		CloseWindowFromChatButton.SetActive(active: true);
		if (FlagsStorage.IsSpectatorMode)
		{
			MainPanel.SetActive(value: true);
			ChatControl chatControl = _chatControl;
			chatControl.OnProfileWindowClosedFromChat = (Action)Delegate.Combine(chatControl.OnProfileWindowClosedFromChat, new Action(CloseMainPanel));
		}
	}

	private void CloseWindowFromChat()
	{
		_chatControl.OnProfileWindowCloseClicked?.Invoke();
		CloseWindowFromChatButton.SetActive(active: false);
	}

	private void InitByMultiPersonControl(MultiPersonControl multiPersonControl)
	{
		OnePlayerInListData onePlayerInListData = new OnePlayerInListData();
		if (!multiPersonControl)
		{
			_managerUI.ShowNotificationWindow(_playerLeftMsg, 2f).Forget();
			Debug.LogError("Player not found");
			return;
		}
		onePlayerInListData.PlayerId = multiPersonControl.ThisPlayerID;
		onePlayerInListData.AccountId = multiPersonControl.ThisPlayerAccountID;
		onePlayerInListData.MultiPersonControl = multiPersonControl;
		onePlayerInListData.PlayerName = multiPersonControl.ThisPlayerName;
		onePlayerInListData.Person = multiPersonControl.gameObject;
		Init(onePlayerInListData);
	}

	private void Init(OnePlayerInListData playerData)
	{
		_onePlayerInListData = playerData;
		_mpc = _onePlayerInListData.MultiPersonControl;
		ShowHideVinylForOtherCarButton.Init(_mpc);
		ShowOnlyProfileButton(FlagsStorage.IsMultiFree && IsOtherPlayer(_mpc));
		int num = _mpc.rank.GetValueOrDefault();
		ImageRank.sprite = UtilsRank.LoadRankSpriteFromResources((UtilsRank.RankType)num);
		TextName.text = _onePlayerInListData.PlayerName;
		string rankNameByIndex = Utils.GetRankNameByIndex(num);
		RankText.text = string.Format("{0}:{1}", new LocalizedString("Settingsmenuwindow.Rank:"), rankNameByIndex);
		TextId.text = "ID:" + _onePlayerInListData.PlayerId;
		ImageExchangeOwnCar.sprite = Resources.Load<Sprite>($"UI/CarImages/b{V.CarId}");
		GameObject car = (FlagsStorage.IsMultiFree ? _multiPlayer.GetCarByOwnerID(_mpc.photonView.CreatorActorNr, needToCheckFirstCar: true) : _freeDriveProvider.FirstCarMultiPlayer);
		ShowCarInfoAsync(car, _mpc).Forget();
		BlockToggle.SetIsOnWithoutNotify(_mpc.IsBlocked);
		BlockSwitcher.Switch(_mpc.IsBlocked);
		MuteToggle.SetIsOnWithoutNotify(_mpc.isMuted);
		MuteSwitcher.Switch(_mpc.isMuted);
		if (_friendsUtility.HasFriendWithID(_onePlayerInListData.PlayerId))
		{
			TextFriend.text = new LocalizedString("PlayerProfile.YourFriend");
		}
		if (_mpc.Reported)
		{
			ButtonReport.interactable = false;
			TextReport.text = new LocalizedString("PlayerProfile.Reported");
		}
		ButtonAchievements.onClick.RemoveAllListeners();
		ButtonAchievements.onClick.AddListener(() =>
		{
			Time.timeScale = 1f;
			ShowAchievements(playerData).Forget();
		});
	}

	protected override void OnEnable()
	{
		ButtonClose.gameObject.SetActive(value: false);
		base.OnEnable();
	}

	private async UniTaskVoid ShowCarInfoAsync(GameObject car, MultiPersonControl owner)
	{
		ShowDefaultCarInfo();
		if (!car)
		{
			return;
		}
		if (car.TryGetComponent<MultiPlayerPosControl>(out var component))
		{
			ShowCarLoading(state: true);
			if (await component.ApplyDataIfNeedAsync(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow() || _mpc != owner)
			{
				return;
			}
			if (!car)
			{
				ShowDefaultCarInfo();
				return;
			}
		}
		VehicleData component2 = car.GetComponent<VehicleData>();
		Powertrain component3 = car.GetComponent<Powertrain>();
		if (!component2 || !component3)
		{
			ShowDefaultCarInfo();
			return;
		}
		ImageCar.sprite = Resources.Load<Sprite>($"UI/CarImages/b{component2.CarId}");
		component3.CalcMaxPowerMaxTorque(out var maxHp, out var _, out var maxTq, out var _, out var _);
		TextHorsePower.text = string.Concat(new LocalizedString("PlayerProfile.PowerUtilsGetpowerMaxpower"), Utils.GetPower(maxHp));
		TextTorque.text = string.Concat(new LocalizedString("PlayerProfile.TorqueUtilsGettorqueMaxtorqueU"), Utils.GetTorque(maxTq), " ", Utils.GetTorquePostfix);
		ImageCarToExchange.sprite = ImageCar.sprite;
		ShowCarLoading(state: false);
	}

	private void ShowDefaultCarInfo()
	{
		ImageCar.sprite = Resources.Load<Sprite>("UI/CarImages/DefaultCar");
		TextHorsePower.text = "Power: N/A";
		TextTorque.text = "Torque: N/A";
		ImageCarToExchange.sprite = ImageCar.sprite;
		ShowCarLoading(state: false);
	}

	private void ShowCarLoading(bool state)
	{
		if (CarLoadingIndicators == null)
		{
			return;
		}
		GameObject[] carLoadingIndicators = CarLoadingIndicators;
		foreach (GameObject gameObject in carLoadingIndicators)
		{
			if ((bool)gameObject)
			{
				gameObject.SetActive(state);
			}
		}
	}

	private bool IsOtherPlayer(MultiPersonControl multiPersonControl)
	{
		if ((bool)multiPersonControl)
		{
			return !SellExchangeFreeHelperCanSellOrExchange.IsSelf(multiPersonControl.gameObject, multiPersonControl.photonView);
		}
		return false;
	}

	private void ShowOnlyProfileButton(bool state)
	{
		if ((bool)MainButtonsPanel && MainButtonsPanel.TryGetComponent<GameFactVisibility>(out var component))
		{
			component.enabled = state;
		}
		if (state)
		{
			int num = (int)PhotonNetwork.CurrentRoom.CustomProperties["P"];
			if (PhotonNetwork.CurrentRoom.Name.Contains(_playerDataStorage.PlayerID) && num != 0 && _playerDataStorage.PlayerID.ToString().Trim() != "")
			{
				ButtonKick.SetActive(active: true);
				BlockToggle.SetActive(active: false);
			}
		}
		else
		{
			ButtonKick.SetActive(active: false);
		}
		BlockToggle.SetActive(state);
		MuteToggle.SetActive(state);
		ButtonMessage.SetActive(state);
		ButtonReport.SetActive(state);
		ButtonAddFriend.SetActive(state);
		ButtonSellCar.SetActive(state);
		ButtonSendMoney.SetActive(state);
		ButtonCarExchange.SetActive(state);
		if (FlagsStorage.IsPrivateRoom)
		{
			if (FlagsStorage.OutOfCar || ((bool)V.Car && _freeDriveProvider.FirstCarMultiPlayer != V.Car) || FlagsStorage.IsSpectatorMode)
			{
				state = false;
			}
			ButtonSpectate.SetActive(state);
		}
	}

	private async UniTaskVoid ShowAchievements(OnePlayerInListData onePlayerInListData)
	{
		if (FlagsStorage.IsMultiFree && (bool)_mpc && !_mpc.ownPlayer)
		{
			_steamAchievementService?.Unlock("ACH_PROFILE_PEEK");
		}
		if (string.IsNullOrEmpty(onePlayerInListData.AccountId))
		{
			_managerUI.ShowNotificationWindow(_userNotAuthorized, 3f, NotificationType.Error).Forget();
			return;
		}
		if (FlagsStorage.IsMultiFree && (bool)_mpc && !_mpc.ownPlayer)
		{
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQEQ", "player_profile");
		}
		HideWindow();
		if (IsFromChat)
		{
			_managerUI.OnShowScreen?.Invoke(obj: false);
			await _managerUI.ShowUserBalanceOnlyWindow();
		}
		_managerUI.ShowAchievmentsWindow(cleanScreen: false);
		await UniTask.Delay(TimeSpan.FromSeconds(0.5));
		if (!_isDestroyed)
		{
			if ((bool)_managerUI.AchievementsWindow)
			{
				AchievementsWindow achievementsWindow = _managerUI.AchievementsWindow;
				achievementsWindow.OnAchievementsWindowClosed = (Action)Delegate.Combine(achievementsWindow.OnAchievementsWindowClosed, new Action(ShowWindow));
				_managerUI.AchievementsWindow.OnAchievementsWindowOpenFreeDrive(onePlayerInListData, _fromPlayerList).Forget();
			}
			else
			{
				ShowWindow();
			}
		}
	}

	private void ReportPlayer()
	{
		if (_onePlayerInListData.Person == null)
		{
			_freeDriveProvider.PlayerListControl.UpdatePlayersList();
			_managerUI.ShowNotificationWindow(_playerLeftMsg, 3f, Color.red);
		}
		else if (!_mpc.Reported)
		{
			ShowPanel(PanelReport);
		}
	}

	public void Reset()
	{
		if ((bool)_openedPanel)
		{
			_openedPanel.SetActive(value: false);
		}
		MainButtonsPanel.SetActive(value: true);
		base.gameObject.SetActive(value: false);
	}

	private void ShowPanel(GameObject panel)
	{
		_isMainPanelShowing = false;
		if ((bool)_openedPanel)
		{
			_openedPanel.SetActive(value: false);
		}
		panel.SetActive(value: true);
		_openedPanel = panel;
		ShowButtons(state: false);
	}

	private void ShowButtons(bool state)
	{
		if (!state)
		{
			ButtonKick.interactable = false;
		}
		MainButtonsPanel.SetActive(state);
		SubmitButtonsPanel.SetActive(!state);
	}

	private void MessagePlayer()
	{
		if (FlagsStorage.IsSpectatorMode)
		{
			CloseMainPanel();
		}
		else
		{
			CloseWithUnclean();
		}
		_chatControl.OpenPersonalChat(_onePlayerInListData.PlayerId, _onePlayerInListData.PlayerName);
	}

	public async UniTaskVoid ReturnToUsersCar()
	{
		if ((bool)_mainUsersCar)
		{
			await UniTask.WaitUntil(() => _mainUsersCar.OnGround);
			if (!_isDestroyed)
			{
				_managerUI.CloseNotTransparentWaitingWindow();
			}
		}
	}

	public async UniTaskVoid DelayedCloseWaitingWindow(float delay)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(delay));
		if (!_isDestroyed)
		{
			_managerUI.CloseNotTransparentWaitingWindow();
		}
	}

	private async UniTaskVoid ShowSpectatorWindow(MultiPersonControl multiPersonControl, bool show)
	{
		FreezeUser(show);
		if (NeedWaitWindow(multiPersonControl) & show)
		{
			_managerUI.ShowWaitingWindowNotTransparent(set: true).Forget();
		}
		if (!show)
		{
			return;
		}
		await UniTask.Yield();
		if (!multiPersonControl || !multiPersonControl.FirstAndOwnCar)
		{
			_managerUI.CloseNotTransparentWaitingWindow();
			FreezeUser(state: false);
			return;
		}
		if (V.CurrentCamIndex == 0)
		{
			if (multiPersonControl.FirstAndOwnCar.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
			{
				_freeDriveProvider.CarCameras.Target = component.camInsideView.transform;
			}
		}
		else
		{
			_freeDriveProvider.CarCameras.Target = multiPersonControl.FirstAndOwnCar.transform;
		}
		DelayedCloseWaitingWindow(5f).Forget();
		bool isSpectatorMode = FlagsStorage.IsSpectatorMode;
		SpectatorWindow.gameObject.SetActive(value: true);
		SpectatorWindow.Init(multiPersonControl, IsFromChat);
		if (IsFromChat && !isSpectatorMode)
		{
			_levelScript.CleanScreen();
		}
		_managerUI.CloseSeveralWindows(userBalanceWindowOnly: true);
	}

	public bool NeedWaitWindow(MultiPersonControl multiPersonControl)
	{
		if (!multiPersonControl || !multiPersonControl.FirstAndOwnCar || !_freeDriveProvider.FirstCarMultiPlayer)
		{
			return true;
		}
		return Vector3.Distance(multiPersonControl.FirstAndOwnCar.transform.position, _freeDriveProvider.FirstCarMultiPlayer.transform.position) > 200f;
	}

	public void SpectatePlayer(MultiPersonControl multiPersonControl, bool state)
	{
		if (state && (bool)multiPersonControl && (bool)multiPersonControl.photonView)
		{
			GameObject carByOwnerID = _multiPlayer.GetCarByOwnerID(multiPersonControl.photonView.CreatorActorNr);
			if ((bool)carByOwnerID)
			{
				multiPersonControl.FirstAndOwnCar = carByOwnerID;
			}
		}
		if ((bool)_freeDriveProvider.CarCameras)
		{
			if (!multiPersonControl || !multiPersonControl.FirstAndOwnCar || !multiPersonControl.FirstAndOwnCar.TryGetComponent<PhotonView>(out var component))
			{
				FreezeUser(state: false);
				return;
			}
			int viewID = component.ViewID;
			V.CoreGameManager.FreeDriveProvider.MultiPlayer.SendValueStr<RpcSpectator>($"{viewID},{_playerDataStorage.PlayerID}", state ? "1" : "0");
			ShowSpectatorWindow(multiPersonControl, state).Forget();
		}
	}

	public void OnSpectatorClose()
	{
		InputMapSetEvent.SetPreviousRootGroup();
		InputMapSetEvent.TrySetGroup(WindowInputGroup);
	}

	private void FreezeUser(bool state)
	{
		if ((bool)_freeDriveProvider.FirstCarMultiPlayer && _freeDriveProvider.FirstCarMultiPlayer.TryGetComponent<Rigidbody>(out var component))
		{
			component.isKinematic = state;
			component.detectCollisions = !state;
		}
		if ((bool)V.DriverPerson)
		{
			V.DriverPerson.SetActive(!state);
		}
	}

	private void KickPlayerConfirmed()
	{
		int num = (int)PhotonNetwork.CurrentRoom.CustomProperties["P"];
		if (PhotonNetwork.CurrentRoom.Name.Contains(_playerDataStorage.PlayerID) && num != 0 && _playerDataStorage.PlayerID.ToString().Trim() != "")
		{
			ShowButtons(state: true);
			StartRemove();
		}
		else
		{
			_managerUI.ShowNotificationWindow(_notYourRoomMsg, 3f, Color.red);
		}
	}

	private void TryBlockPlayer(bool isBlocked)
	{
		if (isBlocked)
		{
			_managerUI.ShowSimpleConfirmWindow(new LocalizedString("PlayerProfileWindow.BlockPlayer"), new LocalizedString("PlayerProfileWindow.PlayerBlockDescription"), BlockPlayer, SetOffBlockToggle, new LocalizedString("PlayerProfileWindow.Block")).Forget();
		}
		else
		{
			SetBlockState(isBlocked: false);
		}
	}

	private void SetOffBlockToggle()
	{
		BlockToggle.SetIsOnWithoutNotify(value: false);
		BlockSwitcher.Switch(isOn: false);
	}

	private void BlockPlayer()
	{
		SetBlockState(isBlocked: true);
	}

	private void SetBlockState(bool isBlocked)
	{
		_mpc.IsBlocked = isBlocked;
		_chatControl.OnPlayerBlocked?.Invoke(_mpc);
		V.CoreGameManager.OnPlayerMuted?.Invoke(_mpc, _mpc.IsBlocked);
		_multiPlayer.SendValueStrToPlayer<RpcOtherPlayerBlockedYou>(RpcOtherPlayerBlockedYou.GetRequestData(_mpc.IsBlocked), string.Empty, _mpc.photonView.CreatorActorNr);
	}

	private void TryMutePlayer(bool isMuted)
	{
		if (isMuted)
		{
			_managerUI.ShowSimpleConfirmWindow(new LocalizedString("Chat.MutePlayer"), new LocalizedString("PlayerProfileWindow.PlayerMuteDescription"), MutePlayer, SetOffMuteToggle, new LocalizedString("PlayerProfile.Mute")).Forget();
		}
		else
		{
			SetMuteState(isMuted: false);
		}
	}

	private void SetOffMuteToggle()
	{
		MuteToggle.SetIsOnWithoutNotify(value: false);
		MuteSwitcher.Switch(isOn: false);
	}

	private void MutePlayer()
	{
		SetMuteState(isMuted: true);
	}

	private void SetMuteState(bool isMuted)
	{
		_mpc.isMuted = isMuted;
		_chatControl.OnPlayerMuted?.Invoke(_mpc.ThisPlayerID, isMuted);
		_multiPlayer.SendValueStrToPlayer<RpcOtherPlayerMutedYou>(RpcOtherPlayerMutedYou.GetRequestData(_mpc.isMuted), string.Empty, _mpc.photonView.CreatorActorNr);
	}

	private void ConfirmClicked()
	{
		if (_openedPanel == PanelSendMoney)
		{
			SendMoneyConfirmed();
			PanelSendMoney.SetActive(value: false);
		}
		else if (_openedPanel == PanelCarExchange)
		{
			_sellExchangeFreeService.TryExchangeCarToOtherPlayerAsync(_onePlayerInListData.Person).Forget();
			PanelCarExchange.SetActive(value: false);
		}
		else if (_openedPanel == PanelReport)
		{
			OnReportConfirmed();
			PanelReport.SetActive(value: false);
		}
		else if (_openedPanel == PanelSellCar)
		{
			int result = 0;
			if (!string.IsNullOrEmpty(InputFieldSellCar.text))
			{
				int.TryParse(InputFieldSellCar.text, out result);
			}
			_sellExchangeFreeService.TrySellCarToOtherPlayerAsync(result, _onePlayerInListData.Person).Forget();
			PanelSellCar.SetActive(value: false);
		}
		else if (_openedPanel == PanelKick)
		{
			KickPlayerConfirmed();
			PanelKick.SetActive(value: false);
		}
		ShowMainPanel();
	}

	private void ShowMainPanel()
	{
		ShowMainPanelOutline(state: false);
		MainButtonsPanel.SetActive(value: true);
		if ((bool)_openedPanel)
		{
			_openedPanel.SetActive(value: false);
		}
		if (!ButtonKick.interactable)
		{
			ButtonKick.interactable = true;
		}
		SubmitButtonsPanel.SetActive(value: false);
	}

	private void OnReportConfirmed()
	{
		_managerUI.ShowNotificationWindow(_thanksForReportMsg, 4f).Forget();
		if (CheckReportValid())
		{
			_mpc.Reported = true;
			_freeDriveProvider.MultiPlayer.SendValueStrToPlayer(49, _onePlayerInListData.PlayerId, MultiPlayer.EncryptName(_playerDataStorage.PlayerID), _onePlayerInListData.PlayerId);
		}
		else
		{
			Debug.LogWarning("Report was skipped - user not valid");
		}
		ShowButtons(state: true);
	}

	private bool CheckReportValid()
	{
		if (_mpc.Reported)
		{
			Debug.LogWarning("Report was skipped - already reported");
			return false;
		}
		if (!_accountController.Authorized)
		{
			Debug.LogWarning("Report was skipped - not authorized");
			return false;
		}
		if (_playerDataStorage.IsBanned)
		{
			Debug.LogWarning("Report was skipped - is banned");
			return false;
		}
		_connectionManager.CorrectHash.Get(out var value);
		if (!value)
		{
			Debug.LogWarning("Report was skipped - wrong app hash");
			return false;
		}
		return true;
	}

	public void OnRemoveConfirmed()
	{
		StartRemove();
		ShowButtons(state: true);
	}

	private void StartRemove()
	{
		if (_onePlayerInListData.Person == null)
		{
			_freeDriveProvider.PlayerListControl.UpdatePlayersList();
			_managerUI.ShowNotificationWindow(_playerLeftMsg, 2f, Color.red);
			return;
		}
		int num = (int)PhotonNetwork.CurrentRoom.CustomProperties["P"];
		if (PhotonNetwork.CurrentRoom.Name.Contains(_playerDataStorage.PlayerID) && num != 0)
		{
			string thisPlayerID = _mpc.ThisPlayerID;
			ButtonReport.interactable = false;
			_managerUI.ShowNotificationWindow(_onePlayerInListData.PlayerName + new LocalizedString("PlayerProfile.WillBeRemovedFromRoom"), 2f).Forget();
			_freeDriveProvider.MultiPlayer.SendValueStr(50, thisPlayerID.ToString(), "");
		}
	}

	private bool CanSendOffer()
	{
		if (!FlagsStorage.RacingGameMode && !FlagsStorage.ChasingRaceModeStarted)
		{
			return _timeAttackController.RaceStatus == TimeAttackStatus.Finish;
		}
		return false;
	}

	public void InviteToFriend()
	{
		if (!CanSendOffer())
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("MultiPlayerPosControl.PlayerIsBusy"), 3f).Forget();
			return;
		}
		if (!_onePlayerInListData.Person)
		{
			_freeDriveProvider.PlayerListControl.UpdatePlayersList();
			_managerUI.ShowNotificationWindow(_playerLeftMsg, 4f, Color.red);
			return;
		}
		if (_friendsUtility.HasFriendWithID(_onePlayerInListData.PlayerId))
		{
			_managerUI.ShowNotificationWindow(_alreadyAddedMsg, 2f).Forget();
		}
		else
		{
			_addFriendService.SendRequestToAddFriendList(_onePlayerInListData.Person);
			ButtonAddFriend.interactable = false;
			_managerUI.ShowNotificationWindow(_requestSendMsg, 2f).Forget();
		}
		Observable.Timer(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
		{
			ButtonAddFriend.interactable = true;
		}).AddTo(this);
	}

	public void ExchangeCarConfirmed()
	{
		PhotonView component = _onePlayerInListData.Person.GetComponent<PhotonView>();
		int ownerActorNr = component.OwnerActorNr;
		int ownerActorNr2 = V.DriverPerson.GetComponent<PhotonView>().OwnerActorNr;
		int viewID = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<PhotonView>().ViewID;
		string text = $"{0}?{_playerDataStorage.PlayerName}?{ownerActorNr2}?{viewID}";
		_multiPlayer.lastOfferSendID = ownerActorNr;
		_multiPlayer.SendValueStrToPlayer(45, ownerActorNr.ToString(), text, component.Owner);
		_managerUI.ShowNotificationWindow(_carExchangeOfferSentMsg, 4f).Forget();
		ShowButtons(state: true);
	}

	private async void SendMoneyConfirmed()
	{
		if (!_onePlayerInListData.Person)
		{
			_managerUI.ShowNotificationWindow(_playerLeftMsg, 3f).Forget();
			return;
		}
		if ((bool)_onePlayerInListData.MultiPersonControl.moneySend)
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("PlayerProfile.Youalreadysentmoneytothisplayer"), 3f).Forget();
			return;
		}
		if (!int.TryParse(InputFieldMoneyToSend.text, out var result))
		{
			_managerUI.ShowNotificationWindow(_enterCorrectNumberMsg, 3f).Forget();
			return;
		}
		ObscuredInt moneySend = Mathf.Clamp(result, 0, 500000);
		int sendOVerAll = ObscuredPrefs.GetInt("moneySendDay" + V.PassedDay);
		if (!_accountController.HasEmail)
		{
			_managerUI.ShowNotificationWindow(_youMustHaveAccToSendMoney, 3f).Forget();
		}
		else
		{
			if (!CanSendMoney(moneySend, sendOVerAll))
			{
				return;
			}
			ShowButtons(state: true);
			if (!(await _accountController.CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType.SendMoney)))
			{
				new AdvancedInt(moneySend).Get(out int value);
				if (_currencyManagerController.IsMoneyEnough(value))
				{
					_onePlayerInListData.Person.GetComponent<PhotonView>();
					_moneySendService.Send(moneySend, _onePlayerInListData.PlayerId);
				}
				else
				{
					_managerUI.ShowNotificationWindow(_youDontHaveSuchMoneyMsg, 3f).Forget();
				}
			}
		}
	}

	private bool CanSendMoney(ObscuredInt moneySend, int sendOVerAll)
	{
		if ((int)moneySend < 50000)
		{
			string text = new LocalizedString("PlayerProfile.PleaseEnterMoreThanSendminlimi");
			_managerUI.ShowNotificationWindow(text.Replace("{SendMinLimit}", 50000.ToString()), 3f).Forget();
			return false;
		}
		if ((int)moneySend > 500000)
		{
			string text2 = new LocalizedString("PlayerProfile.TransferLimitNSendminlimitperd");
			_managerUI.ShowNotificationWindow(text2.Replace("{SendMinLimitPerDay}", 500000.ToString()), 3f).Forget();
			return false;
		}
		if (sendOVerAll + (int)moneySend > 500000)
		{
			string text3 = new LocalizedString("PlayerProfile.TransferLimitSendminlimitperda");
			_managerUI.ShowNotificationWindow(text3.Replace("{SendMinLimitPerDay}", 500000.ToString()), 3f).Forget();
			return false;
		}
		return true;
	}

	private void HideWindow()
	{
		CurrentCanvas.blocksRaycasts = false;
		ShowUI(state: false);
	}

	private void ShowWindow()
	{
		if ((bool)_managerUI.AchievementsWindow)
		{
			AchievementsWindow achievementsWindow = _managerUI.AchievementsWindow;
			achievementsWindow.OnAchievementsWindowClosed = (Action)Delegate.Remove(achievementsWindow.OnAchievementsWindowClosed, new Action(ShowWindow));
		}
		if (IsFromChat)
		{
			_managerUI.CloseWindow("UserBalanceOnlyWindow");
			_managerUI.OnShowScreen?.Invoke(obj: true);
			_chatControl.OnOpenChatWithProfileWindow?.Invoke();
		}
		CurrentCanvas.blocksRaycasts = true;
		ShowUI(state: true);
	}

	private void CloseMainPanel()
	{
		ChatControl chatControl = _chatControl;
		chatControl.OnProfileWindowClosedFromChat = (Action)Delegate.Remove(chatControl.OnProfileWindowClosedFromChat, new Action(CloseMainPanel));
		MainPanel.SetActive(value: false);
	}

	public void KeepOpenAfterSpectatorExit()
	{
		ChatControl chatControl = _chatControl;
		chatControl.OnProfileWindowClosedFromChat = (Action)Delegate.Remove(chatControl.OnProfileWindowClosedFromChat, new Action(CloseMainPanel));
		if (_backActionOnProfileWindow != null)
		{
			ButtonClose.onClick.RemoveListener(_backActionOnProfileWindow);
		}
		_managerUI.CloseWindow("PlayersListWindow");
		MainPanel.SetActive(value: true);
		CloseWindowFromChatButton.SetActive(active: false);
		IsFromChat = false;
		_fromPlayerList = true;
		_backActionOnProfileWindow = CloseWithUnclean;
		InitBackButton();
	}
}
