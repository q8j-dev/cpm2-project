using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class PersonalChatHandler : ChatHandlerBase
	{
		private const int MaxDisconnectedChats = 10;

		private readonly Dictionary<string, PersonalChatView> _personalChatDictionary = new Dictionary<string, PersonalChatView>();

		private readonly List<PersonalChatView> _disconnectedChats = new List<PersonalChatView>();

		[SerializeField]
		private GameObject PersonalChatsChoosePanel;

		[SerializeField]
		private RectTransform PersonalChatsContainer;

		[SerializeField]
		private PersonalChatView PersonalChatViewPrefab;

		[SerializeField]
		private PersonalChatPool PersonalChatPool;

		[SerializeField]
		private Toggle ShowDmPlayerList;

		[SerializeField]
		private Toggle ShowFriendsPlayerList;

		[SerializeField]
		private Toggle ShowAllPlayerList;

		[SerializeField]
		private Button ShowPersonalChatListButton;

		[SerializeField]
		private ActiveChatPlayerInfo ActiveChatPlayerInfo;

		private List<string> _blockedPlayers = new List<string>();

		private PersonalChatView _activePersonalChat;

		private PersonalChatView _currentPersonalChat;

		private ChatControl _chatControl;

		private PlayerDataStorage _playerDataStorage;

		private PlayerDatas.FriendsUtility _friendsUtility;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private IPrefabFactory _prefabFactory;

		private MoneySendService _moneySendService;

		private AddFriendService _addFriendService;

		private TradeAnalytic _tradeAnalytic;

		public void Init(ChatControl chatControl, PlayerDataStorage playerDataStorage, OnlineCollectionsProvider onlineCollectionsProvider, IPrefabFactory prefabFactory, MoneySendService moneySendService, AddFriendService addFriendService, PlayerDatas.FriendsUtility friendsUtility, TradeAnalytic tradeAnalytic)
		{
			_tradeAnalytic = tradeAnalytic;
			_addFriendService = addFriendService;
			_moneySendService = moneySendService;
			_prefabFactory = prefabFactory;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_playerDataStorage = playerDataStorage;
			_chatControl = chatControl;
			_friendsUtility = friendsUtility;
		}

		protected override void Start()
		{
			base.Start();
			ShowPersonalChatListButton.onClick.AddListener(OpenChat);
			ShowDmPlayerList.onValueChanged.AddListener(ShowDmList);
			ShowFriendsPlayerList.onValueChanged.AddListener(ShowFriendsList);
			ShowAllPlayerList.onValueChanged.AddListener(ShowAllList);
			ShowDmPlayerList.isOn = true;
			GlobalEventSystem.OnNewOffer = (Action<TradeOffer>)Delegate.Combine(GlobalEventSystem.OnNewOffer, new Action<TradeOffer>(NewOfferMessage));
			MoneySendService moneySendService = _moneySendService;
			moneySendService.OnMoneySend = (Action<ChatOffer>)Delegate.Combine(moneySendService.OnMoneySend, new Action<ChatOffer>(NewMoneySendMessage));
			MoneySendService moneySendService2 = _moneySendService;
			moneySendService2.OnNewOfferReceived = (Action<ChatOffer>)Delegate.Combine(moneySendService2.OnNewOfferReceived, new Action<ChatOffer>(NewMoneyReceiveMessage));
			AddFriendService addFriendService = _addFriendService;
			addFriendService.OnSendRequestToAddFriend = (Action<ChatOffer>)Delegate.Combine(addFriendService.OnSendRequestToAddFriend, new Action<ChatOffer>(NewInviteToFriendsMessageSend));
			AddFriendService addFriendService2 = _addFriendService;
			addFriendService2.OnReceiveRequestToAddFriend = (Action<ChatOffer>)Delegate.Combine(addFriendService2.OnReceiveRequestToAddFriend, new Action<ChatOffer>(NewInviteToFriendsMessageReceive));
			ChatControl chatControl = _chatControl;
			chatControl.OnPlayerMuted = (Action<string, bool>)Delegate.Combine(chatControl.OnPlayerMuted, new Action<string, bool>(MuteMessage));
			ChatControl chatControl2 = _chatControl;
			chatControl2.OnPlayerBlocked = (Action<MultiPersonControl>)Delegate.Combine(chatControl2.OnPlayerBlocked, new Action<MultiPersonControl>(BlockMessage));
			PersonMultiplayerDataSending.OnPersonDataApplied += CreatePersonalChatView;
			PersonMultiplayerDataSending.OnPersonDataRemoved += OnOtherPlayerDisconnected;
			InitChats();
		}

		protected override void OnDestroy()
		{
			GlobalEventSystem.OnNewOffer = (Action<TradeOffer>)Delegate.Remove(GlobalEventSystem.OnNewOffer, new Action<TradeOffer>(NewOfferMessage));
			MoneySendService moneySendService = _moneySendService;
			moneySendService.OnMoneySend = (Action<ChatOffer>)Delegate.Remove(moneySendService.OnMoneySend, new Action<ChatOffer>(NewMoneySendMessage));
			MoneySendService moneySendService2 = _moneySendService;
			moneySendService2.OnNewOfferReceived = (Action<ChatOffer>)Delegate.Remove(moneySendService2.OnNewOfferReceived, new Action<ChatOffer>(NewMoneyReceiveMessage));
			AddFriendService addFriendService = _addFriendService;
			addFriendService.OnSendRequestToAddFriend = (Action<ChatOffer>)Delegate.Remove(addFriendService.OnSendRequestToAddFriend, new Action<ChatOffer>(NewInviteToFriendsMessageSend));
			AddFriendService addFriendService2 = _addFriendService;
			addFriendService2.OnReceiveRequestToAddFriend = (Action<ChatOffer>)Delegate.Remove(addFriendService2.OnReceiveRequestToAddFriend, new Action<ChatOffer>(NewInviteToFriendsMessageReceive));
			ChatControl chatControl = _chatControl;
			chatControl.OnPlayerMuted = (Action<string, bool>)Delegate.Remove(chatControl.OnPlayerMuted, new Action<string, bool>(MuteMessage));
			ChatControl chatControl2 = _chatControl;
			chatControl2.OnPlayerBlocked = (Action<MultiPersonControl>)Delegate.Remove(chatControl2.OnPlayerBlocked, new Action<MultiPersonControl>(BlockMessage));
			PersonMultiplayerDataSending.OnPersonDataApplied -= CreatePersonalChatView;
			PersonMultiplayerDataSending.OnPersonDataRemoved -= OnOtherPlayerDisconnected;
			foreach (PersonalChatView value in _personalChatDictionary.Values)
			{
				value.OnStatusChanged -= SortByStatus;
			}
			_personalChatDictionary.Clear();
			_disconnectedChats.Clear();
		}

		public override void OpenChat()
		{
			base.OpenChat();
			Show(show: true);
			InputPanel.SetActive(value: false);
			Content.SetActive(value: false);
			TryReturnActiveChatMessages();
			_chatControl.ActivePersonalChatId = "";
			_activePersonalChat = null;
		}

		public override void CloseChat()
		{
			base.CloseChat();
			Show(show: false);
			Content.SetActive(value: false);
			TryReturnActiveChatMessages();
			_chatControl.ActivePersonalChatId = "";
			_activePersonalChat = null;
		}

		private void Show(bool show)
		{
			PersonalChatsChoosePanel.SetActive(show);
		}

		public override void NewMessage(ChatMessage chatMessage)
		{
			_currentPersonalChat = null;
			bool flag = (ObscuredString)chatMessage.ToPlayerId == _playerDataStorage.PlayerID;
			string text = (flag ? chatMessage.FromPlayerId : chatMessage.ToPlayerId);
			if (_blockedPlayers.Contains(text))
			{
				ChatMessageType messageType = chatMessage.MessageType;
				if (messageType == ChatMessageType.CarExchange || messageType == ChatMessageType.CarSell)
				{
					_tradeAnalytic.LogCarTradeRequestStatus(chatMessage.EnteredText, CarTradeRequestStatusType.BlockedByIgnore);
				}
				return;
			}
			bool flag2 = (bool)_activePersonalChat && _activePersonalChat.PlayerId == text;
			_currentPersonalChat = (flag2 ? _activePersonalChat : GetPersonalChatView(text));
			if (!_currentPersonalChat)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("PlayersListControl.Playeralreadylefttheroom"), 2f);
			}
			else
			{
				if (!CheckFromToId(chatMessage, _currentPersonalChat))
				{
					return;
				}
				if (chatMessage.MessageType == ChatMessageType.ConnectionState && _currentPersonalChat.IsChatEmpty && int.TryParse(chatMessage.EnteredText, out var result))
				{
					if (result != 1)
					{
						_personalChatDictionary.Remove(text);
						UnityEngine.Object.Destroy(_currentPersonalChat.gameObject);
						_currentPersonalChat = null;
					}
					return;
				}
				_currentPersonalChat.NewMessage(chatMessage, flag2);
				if (ShowDmPlayerList.isOn)
				{
					_currentPersonalChat.SetActive(active: true);
				}
				if (flag2)
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(ContentRect);
					UIHelper.DelayedRebuildLayout(Container).Forget();
				}
				else if (flag && chatMessage.MessageType != ChatMessageType.ConnectionState)
				{
					UnreadCount++;
					SetUnreadCount(UnreadCount);
				}
				if (CanShowNotification(chatMessage))
				{
					ChatWindow.ShowNotification(chatMessage, text);
				}
			}
		}

		private bool CanShowNotification(ChatMessage chatMessage)
		{
			if (chatMessage.MessageType != ChatMessageType.Block && chatMessage.MessageType != ChatMessageType.Mute && chatMessage.MessageType != ChatMessageType.ConnectionState)
			{
				return (ObscuredString)chatMessage.FromPlayerId != _playerDataStorage.PlayerID;
			}
			return false;
		}

		public override void ShowNotification(ChatMessage chatMessage, string chatId)
		{
			if (!_activePersonalChat || !(_activePersonalChat.PlayerId == chatId))
			{
				Notification.Show(chatMessage, chatId);
			}
		}

		public void TryShowPersonalChatById(string playerId)
		{
			Notification.Hide();
			_activePersonalChat = GetPersonalChatView(playerId);
			if ((bool)_activePersonalChat)
			{
				ShowChosenChat(_activePersonalChat);
			}
		}

		public void TryReturnActiveChatMessages()
		{
			CommonExtensions.CancelAndDisposeToken(ref DelayedSpawnCancellation);
			if ((bool)_activePersonalChat)
			{
				_activePersonalChat.ReturnPersonalChatMessages();
			}
		}

		public override void OnChatWindowClosed()
		{
			TryReturnActiveChatMessages();
			_activePersonalChat = null;
		}

		public void ShowChosenChat(PersonalChatView chatView)
		{
			_chatControl.ActivePersonalChatId = chatView.PlayerId;
			UnreadCount -= chatView.UnreadMessageCount;
			if (UnreadCount < 0)
			{
				UnreadCount = 0;
			}
			SetUnreadCount(UnreadCount);
			CommonExtensions.CancelAndDisposeToken(ref DelayedSpawnCancellation);
			DelayedSpawnCancellation = new CancellationTokenSource();
			chatView.SpawnPersonalChatMessages(DelayedSpawnCancellation.Token).Forget();
			MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(chatView.PlayerId);
			ActiveChatPlayerInfo.SetInfo(playerByPlayerID, chatView, ChatWindow);
			PersonalChatsChoosePanel.SetActive(value: false);
			Content.SetActive(value: true);
			InputPanel.SetActive(value: true);
			UIHelper.DelayedRebuildLayout(Container).Forget();
		}

		private void InitChats()
		{
			MultiPersonControl[] playersOnlineNow = _onlineCollectionsProvider.PlayersOnlineNow;
			foreach (MultiPersonControl multiPersonControl in playersOnlineNow)
			{
				if ((bool)multiPersonControl)
				{
					CreatePersonalChatView(multiPersonControl);
				}
			}
		}

		public PersonalChatView GetPersonalChatView(string playerId)
		{
			if (_personalChatDictionary.TryGetValue(playerId, out var value))
			{
				return value;
			}
			return null;
		}

		private void CreatePersonalChatView(MultiPersonControl person)
		{
			if (!_personalChatDictionary.ContainsKey(person.ThisPlayerID))
			{
				PersonalChatView personalChatView = _prefabFactory.Instantiate(PersonalChatViewPrefab, PersonalChatsContainer);
				personalChatView.OnStatusChanged += SortByStatus;
				personalChatView.Init(ChatWindow, this, _tradeAnalytic, person.ThisPlayerName, person.rank.GetValueOrDefault(), person.ThisPlayerID, person.isMuted);
				_personalChatDictionary.Add(person.ThisPlayerID, personalChatView);
				if (ShowDmPlayerList.isOn)
				{
					personalChatView.SetActive(!personalChatView.IsChatEmpty);
				}
				else if (ShowFriendsPlayerList.isOn)
				{
					personalChatView.SetActive(_friendsUtility.HasFriendWithID(personalChatView.PlayerId));
				}
				else
				{
					personalChatView.SetActive(active: true);
				}
			}
			RemoveDisconnectedChat(person.ThisPlayerID);
			if (person.IsBlocked)
			{
				_blockedPlayers.Add(person.ThisPlayerID);
			}
		}

		private void OnOtherPlayerDisconnected(MultiPersonControl person)
		{
			if (person.IsBlocked && _blockedPlayers.Contains(person.ThisPlayerID))
			{
				_blockedPlayers.Remove(person.ThisPlayerID);
			}
			if (_personalChatDictionary.TryGetValue(person.ThisPlayerID, out var value))
			{
				AddDisconnectedChat(value);
			}
		}

		private void AddDisconnectedChat(PersonalChatView chatView)
		{
			if (_disconnectedChats.Contains(chatView))
			{
				return;
			}
			if (_disconnectedChats.Count >= 10)
			{
				PersonalChatView personalChatView = _disconnectedChats[0];
				_disconnectedChats.RemoveAt(0);
				_personalChatDictionary.Remove(personalChatView.PlayerId);
				if ((bool)personalChatView && (bool)personalChatView.gameObject)
				{
					UnityEngine.Object.Destroy(personalChatView.gameObject);
				}
			}
			_disconnectedChats.Add(chatView);
		}

		private void RemoveDisconnectedChat(string playerId)
		{
			for (int i = 0; i < _disconnectedChats.Count; i++)
			{
				if (_disconnectedChats[i].PlayerId == playerId)
				{
					_disconnectedChats.RemoveAt(i);
					break;
				}
			}
		}

		private bool CheckFromToId(ChatMessage chatMessage, PersonalChatView chatView)
		{
			bool num = (ObscuredString)chatMessage.FromPlayerId == _playerDataStorage.PlayerID && chatView.PlayerId == chatMessage.ToPlayerId;
			bool flag = (ObscuredString)chatMessage.ToPlayerId == _playerDataStorage.PlayerID && chatView.PlayerId == chatMessage.FromPlayerId;
			return num | flag;
		}

		private void NewOfferMessage(TradeOffer offer)
		{
			bool num = offer.OfferActorNr == PhotonNetwork.LocalPlayer.ActorNumber;
			MultiPersonControl multiPersonControl = (num ? V.DriverPerson.GetComponent<MultiPersonControl>() : _onlineCollectionsProvider.GetPlayerByOwnerActorNr(offer.OfferActorNr));
			MultiPersonControl obj = (num ? _onlineCollectionsProvider.GetPlayerByOwnerActorNr(offer.RequestActorNr) : V.DriverPerson.GetComponent<MultiPersonControl>());
			string thisPlayerID = multiPersonControl.ThisPlayerID;
			string thisPlayerName = multiPersonControl.ThisPlayerName;
			string thisPlayerID2 = obj.ThisPlayerID;
			string thisPlayerName2 = obj.ThisPlayerName;
			string additionData = ChatMessage.AdditionalDataForPersonalChat(thisPlayerID, thisPlayerName, thisPlayerID2, thisPlayerName2);
			ChatMessageType messageType = ((offer.RequestItem is TradeItemCar) ? ChatMessageType.CarExchange : ChatMessageType.CarSell);
			ChatMessage chatMessage = new ChatMessage(ChatType.Personal, messageType, offer.TradeID, additionData);
			chatMessage = new ChatMessage(chatMessage.ToString());
			NewMessage(chatMessage);
			if ((bool)_currentPersonalChat)
			{
				_currentPersonalChat.NewOffer(offer);
			}
		}

		private void NewInviteToFriendsMessageSend(ChatOffer offer)
		{
			PrepareNewMessage(ChatMessageType.InviteToFriends, offer.Message, offer.ReceiverId, isLocal: true, offer);
		}

		private void NewInviteToFriendsMessageReceive(ChatOffer offer)
		{
			PrepareNewMessage(ChatMessageType.InviteToFriends, offer.Message, offer.OfferCreatorId, isLocal: false, offer);
		}

		private void NewMoneySendMessage(ChatOffer offer)
		{
			PrepareNewMessage(ChatMessageType.SendMoney, offer.Message, offer.ReceiverId, isLocal: true, offer);
		}

		private void NewMoneyReceiveMessage(ChatOffer offer)
		{
			PrepareNewMessage(ChatMessageType.SendMoney, offer.Message, offer.OfferCreatorId, isLocal: false, offer);
		}

		private void MuteMessage(string otherPlayerId, bool muted)
		{
			PrepareNewMessage(ChatMessageType.Mute, (muted ? 1 : 0).ToString(), otherPlayerId, isLocal: true);
			if (_personalChatDictionary.TryGetValue(otherPlayerId, out var value))
			{
				value.Mute(muted);
			}
		}

		private void BlockMessage(MultiPersonControl person)
		{
			PrepareNewMessage(ChatMessageType.Block, (person.IsBlocked ? 1 : 0).ToString(), person.ThisPlayerID, isLocal: true);
			if (_personalChatDictionary.TryGetValue(person.ThisPlayerID, out var value))
			{
				value.Block(person.IsBlocked);
			}
			if (person.IsBlocked)
			{
				_blockedPlayers.Add(person.ThisPlayerID);
			}
			else if (_blockedPlayers.Contains(person.ThisPlayerID))
			{
				_blockedPlayers.Remove(person.ThisPlayerID);
			}
		}

		private void PrepareNewMessage(ChatMessageType messageType, string message, string otherPlayerId, bool isLocal, Offer offer = null)
		{
			string fromPlayerId = (isLocal ? ((string)_playerDataStorage.PlayerID) : otherPlayerId);
			string toPlayerId = (isLocal ? otherPlayerId : ((string)_playerDataStorage.PlayerID));
			string additionData = ChatMessage.AdditionalDataForPersonalChat(fromPlayerId, "", toPlayerId, "");
			ChatMessage chatMessage = new ChatMessage(ChatType.Personal, messageType, message, additionData);
			chatMessage.FromPlayerId = fromPlayerId;
			chatMessage.ToPlayerId = toPlayerId;
			NewMessage(chatMessage);
			if (offer != null && (bool)_currentPersonalChat)
			{
				_currentPersonalChat.NewOffer(offer);
			}
		}

		private void ShowDmList(bool show)
		{
			if (!show)
			{
				return;
			}
			foreach (PersonalChatView value in _personalChatDictionary.Values)
			{
				if ((bool)value)
				{
					value.SetActive(!value.IsChatEmpty);
				}
			}
		}

		private void ShowFriendsList(bool show)
		{
			if (!show)
			{
				return;
			}
			foreach (PersonalChatView value in _personalChatDictionary.Values)
			{
				if ((bool)value)
				{
					value.SetActive(_friendsUtility.HasFriendWithID(value.PlayerId));
				}
			}
		}

		private void ShowAllList(bool show)
		{
			if (!show)
			{
				return;
			}
			foreach (PersonalChatView value in _personalChatDictionary.Values)
			{
				if ((bool)value)
				{
					value.SetActive(active: true);
				}
			}
		}

		private void SortByStatus(PersonalChatView chatView)
		{
			List<PersonalChatView> list = (from personalChatView in _personalChatDictionary.Values
				orderby personalChatView.Status descending, personalChatView.LastMessageOrder descending
				select personalChatView).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				list[num].transform.SetSiblingIndex(num);
			}
		}
	}
}
