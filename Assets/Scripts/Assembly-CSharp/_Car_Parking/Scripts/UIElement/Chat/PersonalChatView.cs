using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class PersonalChatView : MonoBehaviour
	{
		private static long _messageOrderCounter;

		private readonly Color _counterMuteColor = new Color(0.43f, 0.36f, 0.14f, 1f);

		private readonly Color _counterDefaultColor = new Color(1f, 0.75f, 0f, 1f);

		private readonly List<ChatMessageView> _chatItems = new List<ChatMessageView>();

		private readonly List<ChatMessage> _messages = new List<ChatMessage>();

		[Header("Main Settings")]
		[SerializeField]
		private Image Background;

		[SerializeField]
		private Button ChooseButton;

		[SerializeField]
		private Image Rank;

		[SerializeField]
		private TextMeshProUGUI PlayerNameText;

		[SerializeField]
		private TextMeshProUGUI MessageText;

		[SerializeField]
		private Image Smile;

		[SerializeField]
		private Sprite DefaultSprite;

		[SerializeField]
		private Sprite DisabledSprite;

		[Header("Service Settings")]
		[SerializeField]
		private TextMeshProUGUI ServiceDescription;

		[SerializeField]
		private Image ServiceIcon;

		[SerializeField]
		private Image ServiceSign;

		[SerializeField]
		private Image ServiceSignBackground;

		[SerializeField]
		private ProgressbarImage Progressbar;

		[SerializeField]
		[Header("Unread Messages Settings")]
		private Image UnreadCounter;

		[SerializeField]
		private TextMeshProUGUI UnreadCount;

		[SerializeField]
		[Header("Mute Settings")]
		private Image MuteIcon;

		[SerializeField]
		private GameObject DisconnectedPanel;

		private int _unreadCount;

		private GameObject _lastMessageObject;

		private ChatWindow _chatWindow;

		private PersonalChatHandler _chatHandler;

		private Offer _lastOffer;

		private TradeAnalytic _tradeAnalytic;

		private PersonalChatStatus _status;

		public bool IsChatEmpty => _messages.Count == 0;

		public long LastMessageOrder { get; private set; }

		public int UnreadMessageCount => _unreadCount;

		public string PlayerId { get; private set; }

		public string PlayerName { get; private set; }

		public UtilsRank.RankType RankType { get; private set; }

		public bool IsMuted { get; private set; }

		public bool IsBlocked { get; private set; }

		public PersonalChatStatus Status
		{
			get
			{
				return _status;
			}
			private set
			{
				_status = value;
				OnStatusChanged?.Invoke(this);
			}
		}

		public event Action<PersonalChatView> OnStatusChanged;

		public void Init(ChatWindow chatWindow, PersonalChatHandler chatHandler, TradeAnalytic tradeAnalytic, string playerName, int rank, string playerId, bool isMuted)
		{
			_tradeAnalytic = tradeAnalytic;
			_chatHandler = chatHandler;
			_chatWindow = chatWindow;
			PlayerNameText.text = playerName;
			string text = ChatHelper.RemoveRichText(playerName);
			TMP_FontAsset_Placeholder.EnsureFor(text);
			PlayerNameText.text = text;
			PlayerNameText.color = PlayerNameColoredMessageView.DefaultNameColor;
			if (_chatWindow.IsFriend(playerId))
			{
				PlayerNameText.color = PlayerNameColoredMessageView.FriendNameColor;
			}
			Rank.sprite = UtilsRank.LoadRankSpriteFromResources((UtilsRank.RankType)rank);
			RankType = (UtilsRank.RankType)rank;
			PlayerId = playerId;
			PlayerName = playerName;
			IsMuted = isMuted;
		}

		private void Start()
		{
			ChooseButton.onClick.AddListener(OnPlayerClicked);
			if (!IsMuted && !IsBlocked && IsChatEmpty)
			{
				Status = PersonalChatStatus.Online;
			}
		}

		private void OnDestroy()
		{
			TryUnsubscribeLastOffer();
			ClearCollections();
		}

		private void ClearCollections()
		{
			foreach (ChatMessageView chatItem in _chatItems)
			{
				_chatWindow.GetPersonalChatPool.ReturnMessage(chatItem);
			}
			_messages.Clear();
			_chatItems.Clear();
		}

		public void NewMessage(ChatMessage chatMessage, bool messageInActiveChat)
		{
			AddMessage(chatMessage);
			RemoveOldestChatItem();
			LastMessageOrder = ++_messageOrderCounter;
			if (messageInActiveChat)
			{
				SpawnNewMessage(chatMessage, forceRebuild: true);
			}
			if (Status != PersonalChatStatus.Offline)
			{
				Status = ((Status == PersonalChatStatus.Online) ? PersonalChatStatus.Active : Status);
			}
			switch (chatMessage.MessageType)
			{
			default:
				return;
			case ChatMessageType.Text:
				ShowTextPreview(chatMessage);
				break;
			case ChatMessageType.Smile:
				ShowSmilePreview(chatMessage);
				break;
			case ChatMessageType.CarExchange:
				SetCurrentService(ChatMessageType.CarExchange, 30f);
				if (chatMessage.ToPlayerId == ManagerKeysForPrefs.LocalId)
				{
					_tradeAnalytic.LogCarTradeRequestStatus(chatMessage.EnteredText, CarTradeRequestStatusType.RequestShown);
				}
				break;
			case ChatMessageType.CarSell:
				SetCurrentService(ChatMessageType.CarSell, 30f);
				if (chatMessage.ToPlayerId == ManagerKeysForPrefs.LocalId)
				{
					_tradeAnalytic.LogCarTradeRequestStatus(chatMessage.EnteredText, CarTradeRequestStatusType.RequestShown);
				}
				break;
			case ChatMessageType.SendMoney:
				SetCurrentService(ChatMessageType.SendMoney, 30f);
				break;
			case ChatMessageType.InviteToFriends:
				SetCurrentService(ChatMessageType.InviteToFriends, 30f);
				break;
			case ChatMessageType.ConnectionState:
				HandleConnectionState(chatMessage);
				break;
			}
			if (!_chatWindow.IsOwnMessage(chatMessage) && !messageInActiveChat && chatMessage.MessageType != ChatMessageType.ConnectionState)
			{
				_unreadCount++;
			}
			SetUnreadCount(_unreadCount);
		}

		public async UniTask SpawnPersonalChatMessages(CancellationToken cancellationToken)
		{
			int maxCount = 40;
			for (int i = 0; i < _messages.Count; i++)
			{
				if (i > maxCount)
				{
					await UniTask.Yield(cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
					maxCount += 40;
				}
				ChatMessage message = _messages[i];
				SpawnNewMessage(message);
			}
			_unreadCount = 0;
			SetUnreadCount(_unreadCount);
		}

		public void ReturnPersonalChatMessages()
		{
			_chatWindow.GetPersonalChatPool.ClearAllMessages();
			_chatItems.Clear();
		}

		public void NewOffer(Offer offer)
		{
			TryUnsubscribeLastOffer();
			_lastOffer = offer;
			_lastOffer.OnStateChangedEvent += OnLastOfferStateChanged;
		}

		public void Mute(bool mute)
		{
			MuteIcon.SetActive(mute);
			DisabledView(mute);
			IsMuted = mute;
			if (mute)
			{
				if (Status != PersonalChatStatus.Offline && Status != PersonalChatStatus.Blocked)
				{
					Status = PersonalChatStatus.Muted;
				}
			}
			else
			{
				Status = PersonalChatStatus.Active;
			}
		}

		public void Block(bool block)
		{
			DisabledView(block);
			IsBlocked = block;
			if (block)
			{
				if (Status != PersonalChatStatus.Offline)
				{
					Status = PersonalChatStatus.Blocked;
				}
			}
			else
			{
				Status = PersonalChatStatus.Active;
			}
		}

		private void AddMessage(ChatMessage chatMessage)
		{
			_messages.Add(chatMessage);
		}

		private void RemoveOldestChatItem()
		{
			if (_messages.Count > 100)
			{
				int index = 0;
				_messages.RemoveAt(index);
				if (_chatItems.Count > 0)
				{
					_chatWindow.GetPersonalChatPool.ReturnMessage(_chatItems[index]);
					_chatItems.RemoveAt(index);
				}
			}
		}

		private void SpawnNewMessage(ChatMessage message, bool forceRebuild = false)
		{
			ChatMessageView chatMessageView = _chatWindow.GetPersonalChatPool.AddNewMessage(message, PlayerId);
			chatMessageView.SetActive(active: true);
			_chatItems.Add(chatMessageView);
			if (forceRebuild)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(chatMessageView.GetComponent<RectTransform>());
			}
		}

		private void SetUnreadCount(int unreadCount)
		{
			UnreadCount.text = unreadCount.ToString();
			UnreadCounter.SetActive(unreadCount > 0);
		}

		private void ShowSmilePreview(ChatMessage chatMessage)
		{
			if (int.TryParse(chatMessage.EnteredText, out var result))
			{
				Sprite smileById = _chatWindow.ChatConfig.GetSmileById(result);
				if ((bool)smileById)
				{
					Smile.sprite = smileById;
					Smile.SetActive(active: true);
					_lastMessageObject = Smile.gameObject;
				}
				else
				{
					Smile.SetActive(active: false);
				}
				MessageText.SetActive(active: false);
				DisconnectedPanel.SetActive(value: false);
				ShowServiceItems(show: false);
			}
		}

		private void ShowTextPreview(ChatMessage chatMessage)
		{
			TMP_FontAsset_Placeholder.EnsureFor(chatMessage.EnteredText);
			MessageText.text = chatMessage.EnteredText;
			MessageText.SetActive(active: true);
			_lastMessageObject = MessageText.gameObject;
			Smile.SetActive(active: false);
			DisconnectedPanel.SetActive(value: false);
			ShowServiceItems(show: false);
		}

		private void HandleConnectionState(ChatMessage chatMessage)
		{
			if (!int.TryParse(chatMessage.EnteredText, out var result))
			{
				return;
			}
			bool flag = result == 1;
			DisconnectedPanel.SetActive(!flag);
			if (flag)
			{
				if (IsBlocked)
				{
					Status = PersonalChatStatus.Blocked;
				}
				else if (IsMuted)
				{
					Status = PersonalChatStatus.Muted;
				}
				else if (IsChatEmpty)
				{
					Status = PersonalChatStatus.Online;
				}
				else
				{
					Status = PersonalChatStatus.Active;
				}
				if ((bool)_lastMessageObject)
				{
					_lastMessageObject.SetActive(value: true);
					if (_lastMessageObject == ServiceIcon.gameObject)
					{
						ServiceDescription.SetActive(active: true);
					}
				}
			}
			else
			{
				MessageText.SetActive(active: false);
				Smile.SetActive(active: false);
				ShowServiceItems(show: false);
				Status = PersonalChatStatus.Offline;
			}
			DisabledView(!flag);
		}

		private void SetCurrentService(ChatMessageType messageType, float progressDuration)
		{
			PersonalChatData personalChatData = _chatWindow.ChatConfig.GetPersonalChatData(messageType);
			SetServiceData(personalChatData);
			Progressbar.StartProgress(1f, 0f, progressDuration);
		}

		private void SetServiceData(PersonalChatData data)
		{
			if (data != null)
			{
				ServiceDescription.text = new LocalizedString(data.ServiceName);
				ServiceIcon.sprite = data.ServiceIcon;
				ServiceSign.sprite = data.ServiceIcon;
				ServiceSignBackground.color = data.ServiceColor;
				MessageText.SetActive(active: false);
				Smile.SetActive(active: false);
				DisconnectedPanel.SetActive(value: false);
				ShowServiceItems(show: true);
				_lastMessageObject = ServiceIcon.gameObject;
			}
		}

		private void ShowServiceItems(bool show)
		{
			ServiceDescription.SetActive(show);
			ServiceIcon.SetActive(show);
			ServiceSign.SetActive(show);
			ServiceSignBackground.SetActive(show);
			Progressbar.SetActive(show);
		}

		private void DisabledView(bool disable)
		{
			if (!IsMuted && !IsBlocked)
			{
				Background.sprite = (disable ? DisabledSprite : DefaultSprite);
				UnreadCounter.color = (disable ? _counterMuteColor : _counterDefaultColor);
			}
		}

		private void OnPlayerClicked()
		{
			_chatHandler.TryShowPersonalChatById(PlayerId);
		}

		private void OnLastOfferStateChanged(TradeOfferState state)
		{
			switch (state)
			{
			case TradeOfferState.Pending:
			case TradeOfferState.Processing:
				Progressbar.Pause();
				break;
			default:
				Progressbar.Stop();
				Progressbar.SetActive(active: false);
				break;
			case TradeOfferState.Enabled:
				break;
			}
		}

		private void TryUnsubscribeLastOffer()
		{
			if (_lastOffer != null)
			{
				_lastOffer.OnStateChangedEvent -= OnLastOfferStateChanged;
			}
		}
	}
}
