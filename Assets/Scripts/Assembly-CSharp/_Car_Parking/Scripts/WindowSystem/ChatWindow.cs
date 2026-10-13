using System;
using System.Collections.Generic;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.UIElement.Chat;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class ChatWindow : WindowScript, IBackableWindow
	{
		private const float ChatMinimizeFadeDuration = 0.08f;

		private const float ChatMaximizeDuration = 0.1f;

		private readonly Vector2 _anchoredSize = new Vector2(0.5f, 1f);

		[Header("Collapsed chat")]
		[SerializeField]
		private CanvasGroup MinimizedCanvasGroup;

		[SerializeField]
		private CollapsedTextMessageView CollapsedChatTextMessage;

		[SerializeField]
		private SmileMessageView CollapsedChatSmileMessage;

		[SerializeField]
		private Button MaximizeButton;

		[SerializeField]
		private RectTransform MinimizedRect;

		[SerializeField]
		private GameObject SpectatorInputHintObject;

		[Header("Expanded chat")]
		[SerializeField]
		private RectTransform MaximizedChat;

		[SerializeField]
		private CanvasGroup MaximizedCanvasGroup;

		[SerializeField]
		private Button HideButton;

		[SerializeField]
		private Button HideButtonBackground;

		[SerializeField]
		private Button SendButton;

		[SerializeField]
		private Image SendMessageIcon;

		[SerializeField]
		private Toggle RoomChatToggle;

		[SerializeField]
		private Toggle FriendsChatToggle;

		[SerializeField]
		private Toggle PersonalChatToggle;

		[SerializeField]
		private Toggle SmileToggle;

		[SerializeField]
		private InputField InputText;

		[SerializeField]
		private CanvasGroup SmilesPanel;

		[SerializeField]
		private TextMeshProUGUI TimerText;

		[SerializeField]
		private GameObject TimerPanel;

		[SerializeField]
		private TextMessageView TextMessageView;

		[SerializeField]
		private SmileMessageView SmileMessageView;

		[SerializeField]
		private PersonalChatPool PersonalChatPool;

		[SerializeField]
		private Button ShowPersonalChatListButton;

		[SerializeField]
		private Toggle ShowFriendsPlayerList;

		[SerializeField]
		private GeneralChatHandler RoomChatHandler;

		[SerializeField]
		private GeneralChatHandler FriendsChatHandler;

		[SerializeField]
		private PersonalChatHandler PersonalChatHandler;

		[SerializeField]
		private GameObject MainContentPanel;

		[SerializeField]
		private Button BackButton;

		[SerializeField]
		private ChatNotification MainNotification;

		private ChatSmilesInteractableService _smilesInteractableService;

		private ChatHandlerBase _currentChatHandler;

		private Vector2 _anchorMin;

		private bool _isMaximizedChat;

		private ChatType _chatType;

		private ChatControl _chatControl;

		private PlayerDataStorage _playerDataStorage;

		private PlayerDatas.FriendsUtility _friendsUtility;

		private MultiPlayer _multiPlayer;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private IPrefabFactory _prefabFactory;

		private MoneySendService _moneySendService;

		private AddFriendService _addFriendService;

		private ITradeService _tradeService;

		private TradeAnalytic _tradeAnalytic;

		private RegistryHandle<BoolEvent> _onOpenChatHandle = new RegistryHandle<BoolEvent>(EventRegistryIDs.OnOpenChat);

		private EventRegistry _eventRegistry;

		private bool _didPushUiGroup;

		private static readonly HashSet<string> ChatFullHideWindows = new HashSet<string> { "SettingsMenuWindow" };

		private static readonly HashSet<string> ChatDetourWindows = new HashSet<string> { "BuyThisCarWindow" };

		private bool _hiddenByOtherWindow;

		[field: Header("Configs")]
		[field: SerializeField]
		public ChatDataSO ChatConfig { get; private set; }

		public PersonalChatPool GetPersonalChatPool => PersonalChatPool;

		public ChatControl GetChatControl => _chatControl;

		public bool IsMaximizedChat => _isMaximizedChat;

		[Inject]
		public void Construct(ChatControl chatControl, PlayerDataStorage playerDataStorage, MultiPlayer multiPlayer, OnlineCollectionsProvider onlineCollectionsProvider, EventRegistry eventRegistry, IPrefabFactory prefabFactory, MoneySendService moneySendService, AddFriendService addFriendService, ITradeService tradeService, TradeAnalytic tradeAnalytic)
		{
			_tradeAnalytic = tradeAnalytic;
			_tradeService = tradeService;
			_addFriendService = addFriendService;
			_moneySendService = moneySendService;
			_prefabFactory = prefabFactory;
			_eventRegistry = eventRegistry;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_multiPlayer = multiPlayer;
			_playerDataStorage = playerDataStorage;
			_chatControl = chatControl;
		}

		[Inject]
		public void Initialize(PlayerDatas.FriendsUtility friendsUtility)
		{
			_friendsUtility = friendsUtility;
		}

		protected override void Start()
		{
			base.Start();
			MaximizeButton.onClick.AddListener(OpenChatForSubscriptionVoid);
			HideButton.onClick.AddListener(HideChatForSubscription);
			HideButtonBackground.onClick.AddListener(HideChatForSubscription);
			SendButton.onClick.AddListener(NewTextMessage);
			InputText.onSubmit.AddListener(OnInputFieldSubmit);
			RoomChatToggle.onValueChanged.AddListener(ShowRoomChat);
			FriendsChatToggle.onValueChanged.AddListener(ShowFriendsChat);
			PersonalChatToggle.onValueChanged.AddListener(ShowPersonalChats);
			InputText.onValueChanged.AddListener(CheckMessageLength);
			ShowPersonalChatListButton.onClick.AddListener(ShowPersonalChatsForSubscription);
			SmileToggle.onValueChanged.AddListener(ShowSmilesPanel);
			ShowFriendsPlayerList.isOn = false;
			GeneralChatMessagePool messagePool = new GeneralChatMessagePool(_prefabFactory, TextMessageView, SmileMessageView);
			RoomChatHandler.Init(messagePool);
			FriendsChatHandler.Init(messagePool);
			PersonalChatHandler.Init(_chatControl, _playerDataStorage, _onlineCollectionsProvider, _prefabFactory, _moneySendService, _addFriendService, _friendsUtility, _tradeAnalytic);
			_currentChatHandler = RoomChatHandler;
			RoomChatToggle.isOn = true;
			_anchorMin = MaximizedChat.anchorMin;
			_smilesInteractableService = new ChatSmilesInteractableService(SmilesPanel, TimerPanel, TimerText);
			MainNotification.Init(this);
			_isMaximizedChat = false;
			if ((bool)_multiPlayer && _multiPlayer.hideAllChat)
			{
				ShowWindow(isShow: false);
			}
			MinimizeChat(immediate: true);
			_chatControl.ChatWindow = this;
			ChatControl chatControl = _chatControl;
			chatControl.GlobalMessageEvent = (Action<ChatMessage>)Delegate.Combine(chatControl.GlobalMessageEvent, new Action<ChatMessage>(OnNewMessage));
			_chatControl.OnPersonalChatOpen += OpenPersonalChat;
			ChatControl chatControl2 = _chatControl;
			chatControl2.OnOpenChatWithProfileWindow = (Action)Delegate.Combine(chatControl2.OnOpenChatWithProfileWindow, new Action(OpenForPlayerProfileWindow));
			ChatControl chatControl3 = _chatControl;
			chatControl3.OnProfileWindowCloseClicked = (Action)Delegate.Combine(chatControl3.OnProfileWindowCloseClicked, new Action(CloseProfileWindowWithRebuild));
			ManagerUI managerUI = _managerUI;
			managerUI.OnShowScreen = (Action<bool>)Delegate.Combine(managerUI.OnShowScreen, new Action<bool>(ShowUI));
			GlobalEventSystem.OnHudClicked = (Action)Delegate.Combine(GlobalEventSystem.OnHudClicked, new Action(OnHudClicked));
			GlobalEventSystem.OnCleanUI = (Action<bool>)Delegate.Combine(GlobalEventSystem.OnCleanUI, new Action<bool>(OnCleanUiScreen));
			_onOpenChatHandle.Get(_eventRegistry).Subscribe(OpenChatForSubscription);
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnRegularWindowOpening = (Action<string>)Delegate.Combine(managerUI2.OnRegularWindowOpening, new Action<string>(OnOtherRegularWindowOpening));
			ManagerUI managerUI3 = _managerUI;
			managerUI3.OnRegularWindowClosed = (Action<string>)Delegate.Combine(managerUI3.OnRegularWindowClosed, new Action<string>(OnOtherRegularWindowClosed));
		}

		private void HideChatForSubscription()
		{
			MinimizeChat();
		}

		private void OpenChatForSubscriptionVoid()
		{
			OpenChatForSubscription(isKeyboard: false);
		}

		private void OpenChatForSubscription(bool isKeyboard)
		{
			if (_isMaximizedChat || ((bool)_multiPlayer && _multiPlayer.hideAllChat))
			{
				return;
			}
			MaximizeChat();
			if (isKeyboard)
			{
				UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
				if (current != null)
				{
					current.SetSelectedGameObject(InputText.gameObject);
				}
			}
		}

		private void ShowPersonalChatsForSubscription()
		{
			ShowPersonalChats(isOn: true);
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			MinimizeChat();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_smilesInteractableService.Dispose();
			_smilesInteractableService = null;
			ChatControl chatControl = _chatControl;
			chatControl.GlobalMessageEvent = (Action<ChatMessage>)Delegate.Remove(chatControl.GlobalMessageEvent, new Action<ChatMessage>(OnNewMessage));
			_chatControl.OnPersonalChatOpen -= OpenPersonalChat;
			ChatControl chatControl2 = _chatControl;
			chatControl2.OnOpenChatWithProfileWindow = (Action)Delegate.Remove(chatControl2.OnOpenChatWithProfileWindow, new Action(OpenForPlayerProfileWindow));
			ChatControl chatControl3 = _chatControl;
			chatControl3.OnProfileWindowCloseClicked = (Action)Delegate.Remove(chatControl3.OnProfileWindowCloseClicked, new Action(CloseProfileWindowWithRebuild));
			ManagerUI managerUI = _managerUI;
			managerUI.OnShowScreen = (Action<bool>)Delegate.Remove(managerUI.OnShowScreen, new Action<bool>(ShowUI));
			GlobalEventSystem.OnBuyThisCarWindowClosed = (Action)Delegate.Remove(GlobalEventSystem.OnBuyThisCarWindowClosed, new Action(ShowUI));
			GlobalEventSystem.OnHudClicked = (Action)Delegate.Remove(GlobalEventSystem.OnHudClicked, new Action(OnHudClicked));
			GlobalEventSystem.OnCleanUI = (Action<bool>)Delegate.Remove(GlobalEventSystem.OnCleanUI, new Action<bool>(OnCleanUiScreen));
			MaximizeButton.onClick.RemoveListener(OpenChatForSubscriptionVoid);
			HideButton.onClick.RemoveListener(HideChatForSubscription);
			HideButtonBackground.onClick.RemoveListener(HideChatForSubscription);
			SendButton.onClick.RemoveListener(NewTextMessage);
			InputText.onSubmit.RemoveListener(OnInputFieldSubmit);
			RoomChatToggle.onValueChanged.RemoveListener(ShowRoomChat);
			FriendsChatToggle.onValueChanged.RemoveListener(ShowFriendsChat);
			PersonalChatToggle.onValueChanged.RemoveListener(ShowPersonalChats);
			InputText.onValueChanged.RemoveListener(CheckMessageLength);
			ShowPersonalChatListButton.onClick.RemoveListener(ShowPersonalChatsForSubscription);
			SmileToggle.onValueChanged.RemoveListener(ShowSmilesPanel);
			_onOpenChatHandle.Get(_eventRegistry)?.Unsubscribe(OpenChatForSubscription);
			ManagerUI managerUI2 = _managerUI;
			managerUI2.OnRegularWindowOpening = (Action<string>)Delegate.Remove(managerUI2.OnRegularWindowOpening, new Action<string>(OnOtherRegularWindowOpening));
			ManagerUI managerUI3 = _managerUI;
			managerUI3.OnRegularWindowClosed = (Action<string>)Delegate.Remove(managerUI3.OnRegularWindowClosed, new Action<string>(OnOtherRegularWindowClosed));
		}

		private void OnCleanUiScreen(bool isClean)
		{
			if (isClean)
			{
				if (_isMaximizedChat && (bool)_multiPlayer)
				{
					MinimizeChat(immediate: true);
				}
				ShowWindow(isShow: false);
				ShowMinimizedChat(isShow: false);
			}
			else if (FlagsStorage.IsChatAllowed && (bool)_multiPlayer && !_multiPlayer.hideAllChat)
			{
				ShowMinimizedChat(isShow: true);
			}
		}

		public void HideAllChatCollapsed()
		{
			if (_isMaximizedChat && (bool)_multiPlayer)
			{
				MinimizeChat(immediate: true);
			}
			ShowWindow(isShow: false);
			MinimizedCanvasGroup.DOKill();
			ShowMinimizedChat(isShow: false);
		}

		private void OnOtherRegularWindowOpening(string guid)
		{
			if (ChatFullHideWindows.Contains(guid))
			{
				if (_isMaximizedChat && (bool)_multiPlayer)
				{
					MinimizeChat(immediate: true);
				}
				ShowWindow(isShow: false);
				ShowMinimizedChat(isShow: false);
				_hiddenByOtherWindow = true;
			}
			else if (_isMaximizedChat && MainContentPanel.activeSelf && !ChatDetourWindows.Contains(guid))
			{
				MinimizeChat(immediate: true);
			}
		}

		private void OnOtherRegularWindowClosed(string guid)
		{
			if (_hiddenByOtherWindow && ChatFullHideWindows.Contains(guid))
			{
				_hiddenByOtherWindow = false;
				if (FlagsStorage.IsChatAllowed && (bool)_multiPlayer && !_multiPlayer.hideAllChat)
				{
					ShowMinimizedChat(isShow: true);
				}
			}
		}

		private void ReleaseOwnUiGroupPush()
		{
			if (_didPushUiGroup)
			{
				_didPushUiGroup = false;
				if (InputMapSetEvent.CurrentInputGroup == InputGroupsEnum.Ui)
				{
					InputMapSetEvent.TrySetPreviousGroup();
				}
			}
		}

		public void MaximizeChat(bool showRoomChat = true)
		{
			MinimizedCanvasGroup.DOKill();
			MaximizedCanvasGroup.DOKill();
			MaximizedChat.DOKill();
			if (showRoomChat)
			{
				RoomChatToggle.isOn = true;
				_currentChatHandler.OpenChat();
			}
			_isMaximizedChat = true;
			if (InputMapSetEvent.TrySetGroup(InputGroupsEnum.Ui))
			{
				_didPushUiGroup = true;
			}
			MaximizeButton.interactable = false;
			MinimizedCanvasGroup.DOFade(0f, 0.1f);
			UpdateSpectatorInputHint(pillShown: false);
			MaximizedChat.anchorMin = _anchoredSize;
			MaximizedChat.DOAnchorMin(_anchorMin, 0.1f).SetEase(Ease.OutSine);
			MaximizedCanvasGroup.DOFade(1f, 0.1f).OnComplete(() =>
			{
				if (_isMaximizedChat)
				{
					_chatControl.OnChatOpened?.Invoke(MaximizedChat, arg2: true);
					MaximizedCanvasGroup.blocksRaycasts = true;
					MaximizedCanvasGroup.interactable = true;
					MinimizedCanvasGroup.blocksRaycasts = false;
					MinimizedCanvasGroup.interactable = false;
					UIHelper.DelayedRebuildLayout(MaximizedChat).Forget();
					InitBackButton();
				}
			});
		}

		public void MinimizeChat(bool immediate = false)
		{
			MinimizedCanvasGroup.DOKill();
			MaximizedCanvasGroup.DOKill();
			MaximizedChat.DOKill();
			_isMaximizedChat = false;
			UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
			if ((bool)current && current.currentSelectedGameObject == InputText.gameObject)
			{
				current.SetSelectedGameObject(null);
			}
			float duration = (immediate ? 0f : 0.1f);
			HideButtonBackground.SetActive(active: false);
			MaximizedChat.DOAnchorMin(_anchoredSize, duration).SetEase(Ease.OutSine);
			MaximizedCanvasGroup.DOFade(0f, immediate ? 0f : 0.08f).OnComplete(() =>
			{
				if (!_isMaximizedChat)
				{
					MaximizeButton.interactable = true;
					MaximizedCanvasGroup.blocksRaycasts = false;
					if (!_multiPlayer.hideAllChat)
					{
						MinimizedCanvasGroup.blocksRaycasts = true;
						MinimizedCanvasGroup.interactable = true;
					}
				}
			});
			if (!_multiPlayer.hideAllChat)
			{
				MinimizedCanvasGroup.DOFade(1f, duration);
			}
			UpdateSpectatorInputHint(!_multiPlayer.hideAllChat);
			SmileToggle.isOn = false;
			_currentChatHandler.OnChatWindowClosed();
			_chatControl.OnChatOpened?.Invoke(MaximizedChat, arg2: false);
			CloseProfileWindow();
			UIHelper.DelayedRebuildLayout(MinimizedRect).Forget();
			ReleaseOwnUiGroupPush();
		}

		private void OnHudClicked()
		{
			MinimizeChat(immediate: true);
		}

		private void CloseProfileWindowWithRebuild()
		{
			CloseProfileWindow();
			_currentChatHandler.RebuildContainerLayout();
		}

		private void CloseProfileWindow()
		{
			if (FlagsStorage.IsSpectatorMode)
			{
				_chatControl.OnProfileWindowClosedFromChat?.Invoke();
			}
			else
			{
				TryCloseProfileWindow();
			}
			ShowContentForOtherWindow(show: true);
		}

		private void TryCloseProfileWindow()
		{
			if (_isMaximizedChat)
			{
				_managerUI.CloseWindow("PlayerProfileWindow");
			}
		}

		private void OpenForPlayerProfileWindow()
		{
			ShowContentForOtherWindow(show: false);
			MaximizeChat(showRoomChat: false);
		}

		private void ShowContentForOtherWindow(bool show)
		{
			MainContentPanel.SetActive(show);
			HideButton.SetActive(show);
		}

		private void ShowRoomChat(bool isOn)
		{
			if (isOn && (!_currentChatHandler || !RoomChatHandler.ActiveNow))
			{
				CloseProfileWindow();
				_chatType = ChatType.FullChat;
				_currentChatHandler?.CloseChat();
				_currentChatHandler = RoomChatHandler;
				_currentChatHandler.OpenChat();
				UIHelper.DelayedRebuildLayout(MaximizedChat).Forget();
			}
		}

		private void ShowFriendsChat(bool isOn)
		{
			if (isOn && (!_currentChatHandler || !FriendsChatHandler.ActiveNow))
			{
				CloseProfileWindow();
				_chatType = ChatType.FriendsChat;
				_currentChatHandler?.CloseChat();
				_currentChatHandler = FriendsChatHandler;
				_currentChatHandler.OpenChat();
				UIHelper.DelayedRebuildLayout(MaximizedChat).Forget();
			}
		}

		private void ShowPersonalChats(bool isOn)
		{
			if (isOn && (!_currentChatHandler || !PersonalChatHandler.ActiveNow))
			{
				CloseProfileWindow();
				SmileToggle.isOn = false;
				_chatType = ChatType.Personal;
				_currentChatHandler?.CloseChat();
				_currentChatHandler = PersonalChatHandler;
				_currentChatHandler.OpenChat();
				UIHelper.DelayedRebuildLayout(MaximizedChat).Forget();
			}
		}

		public void OpenPersonalChat(string playerId)
		{
			PersonalChatToggle.isOn = true;
			ShowContentForOtherWindow(show: true);
			DelayedOpenChosenChat(playerId).Forget();
			ShowWindow(isShow: true);
			MaximizeChat(showRoomChat: false);
		}

		private async UniTask DelayedOpenChosenChat(string playerId)
		{
			await UniTask.DelayFrame(1);
			PersonalChatHandler personalChatHandler = _currentChatHandler as PersonalChatHandler;
			if ((bool)personalChatHandler)
			{
				personalChatHandler.TryShowPersonalChatById(playerId);
			}
		}

		private void OnNewMessage(ChatMessage chatMessage)
		{
			ChatHandlerBase chatHandlerBase = null;
			switch (chatMessage.ChatType)
			{
			case ChatType.FullChat:
				chatHandlerBase = RoomChatHandler;
				break;
			case ChatType.FriendsChat:
				chatHandlerBase = FriendsChatHandler;
				break;
			case ChatType.Personal:
				chatHandlerBase = PersonalChatHandler;
				break;
			}
			if ((bool)chatHandlerBase)
			{
				chatHandlerBase.NewMessage(chatMessage);
				SetMinimizedChatText(chatMessage);
			}
		}

		private void SetMinimizedChatText(ChatMessage chatMessage)
		{
			SetCollapsedChatMessage(chatMessage);
			UIHelper.DelayedRebuildLayout(MinimizedRect).Forget();
		}

		private void SetCollapsedChatMessage(ChatMessage chatMessage)
		{
			if (chatMessage != null)
			{
				if (chatMessage.MessageType == ChatMessageType.Text)
				{
					CollapsedChatSmileMessage.SetActive(active: false);
					CollapsedChatTextMessage.SetActive(active: true);
					CollapsedChatTextMessage.Init(this, chatMessage);
				}
				else if (chatMessage.MessageType == ChatMessageType.Smile)
				{
					CollapsedChatTextMessage.SetActive(active: false);
					CollapsedChatSmileMessage.SetActive(active: true);
					CollapsedChatSmileMessage.Init(this, chatMessage);
				}
			}
		}

		private void OnInputFieldSubmit(string text)
		{
			NewTextMessage();
			UnityEngine.EventSystems.EventSystem.current?.SetSelectedGameObject(null);
		}

		private void NewTextMessage()
		{
			if (!string.IsNullOrEmpty(InputText.text))
			{
				_chatControl.MessageType = ChatMessageType.Text;
				_chatControl.OnChatEntered(InputText.text, _chatType);
				InputText.text = "";
			}
		}

		public void NewSmileMessage(int id)
		{
			_chatControl.MessageType = ChatMessageType.Smile;
			_chatControl.OnChatEntered(id.ToString(), _chatType);
			_smilesInteractableService.OnNewSmile();
		}

		private void CheckMessageLength(string message)
		{
			if (message.Length > 100)
			{
				message = message.Remove(100, message.Length - 100);
				InputText.text = message;
			}
			float a = ((message.Length > 0) ? 1f : 0.4f);
			Color color = SendMessageIcon.color;
			SendMessageIcon.color = new Color(color.r, color.g, color.b, a);
		}

		public bool IsOwnMessage(ChatMessage chatMessage)
		{
			return (ObscuredString)chatMessage.FromPlayerId == _playerDataStorage.PlayerID;
		}

		public bool MessageFromFriend(ChatMessage chatMessage)
		{
			return IsFriend(chatMessage.FromPlayerId);
		}

		public bool IsFriend(string playerId)
		{
			return _friendsUtility.HasFriendWithID(playerId);
		}

		public void OnNickClicked(string playerId, string playerName)
		{
			if (!((ObscuredString)playerId == _playerDataStorage.PlayerID))
			{
				_chatControl.OnClickLink(playerId, playerName);
				if ((bool)_onlineCollectionsProvider.GetPlayerByPlayerID(playerId))
				{
					ShowContentForOtherWindow(show: false);
				}
			}
		}

		public void ShowWindow(bool isShow)
		{
			MaximizedCanvasGroup.alpha = (isShow ? 1 : 0);
			MaximizedCanvasGroup.blocksRaycasts = isShow;
			MaximizedCanvasGroup.interactable = isShow;
			CloseProfileWindow();
		}

		public void ShowMinimizedChat(bool isShow)
		{
			MinimizedCanvasGroup.alpha = (isShow ? 1 : 0);
			MinimizedCanvasGroup.blocksRaycasts = isShow;
			MinimizedCanvasGroup.interactable = isShow;
			UpdateSpectatorInputHint(isShow);
		}

		private void UpdateSpectatorInputHint(bool pillShown)
		{
			if ((bool)SpectatorInputHintObject)
			{
				SpectatorInputHintObject.SetActive(pillShown && FlagsStorage.IsSpectatorMode);
			}
		}

		public void RefreshSpectatorInputHint()
		{
			UpdateSpectatorInputHint(Mathf.Approximately(MinimizedCanvasGroup.alpha, 1f));
		}

		public bool GetChatMaximizeButtonActiveState()
		{
			return Mathf.Approximately(MinimizedCanvasGroup.alpha, 1f);
		}

		public void OnBuyingCar(TradeOffer offer)
		{
			_tradeService.TryTradeAsync(offer.TradeID);
			ShowUI(state: false);
			GlobalEventSystem.OnBuyThisCarWindowClosed = (Action)Delegate.Combine(GlobalEventSystem.OnBuyThisCarWindowClosed, new Action(ShowUI));
		}

		public PersonalChatView GetPersonalChatByPlayerId(string playerId)
		{
			PersonalChatView personalChatView = PersonalChatHandler.GetPersonalChatView(playerId);
			if ((bool)personalChatView)
			{
				return personalChatView;
			}
			return null;
		}

		public void ShowNotification(ChatMessage chatMessage, string chatId)
		{
			if (_isMaximizedChat)
			{
				_currentChatHandler.ShowNotification(chatMessage, chatId);
			}
			else
			{
				MainNotification.Show(chatMessage, chatId);
			}
		}

		private void ShowUI()
		{
			GlobalEventSystem.OnBuyThisCarWindowClosed = (Action)Delegate.Remove(GlobalEventSystem.OnBuyThisCarWindowClosed, new Action(ShowUI));
			ShowUI(state: true);
		}

		private void ShowSmilesPanel(bool isShow)
		{
			_smilesInteractableService.ShowSmiles(isShow);
		}
	}
}
