using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ChatNotification : MonoBehaviour
	{
		private const float LifeTime = 5f;

		[Header("Main Settings")]
		[SerializeField]
		private Image Background;

		[SerializeField]
		private Button OpenChatButton;

		[SerializeField]
		private TextMeshProUGUI PlayerNameText;

		[SerializeField]
		private TextMeshProUGUI MessageText;

		[SerializeField]
		private Image Smile;

		[SerializeField]
		[Header("Service Settings")]
		private TextMeshProUGUI ServiceDescriptionText;

		[SerializeField]
		private Image ServiceIcon;

		[SerializeField]
		private Image ServiceSign;

		[SerializeField]
		private Image ServiceSignBackground;

		[Header("Unread Messages Settings")]
		[SerializeField]
		private Image UnreadCounter;

		[SerializeField]
		private TextMeshProUGUI UnreadCount;

		private ChatWindow _chatWindow;

		private string _playerName;

		private string _chatId;

		private bool _isDestroyed;

		private void Start()
		{
			OpenChatButton.onClick.AddListener(OpenPersonalChat);
		}

		private void OnDestroy()
		{
			_isDestroyed = true;
		}

		public void Init(ChatWindow chatWindow)
		{
			_chatWindow = chatWindow;
		}

		public void Show(ChatMessage chatMessage, string chatId)
		{
			if (_chatWindow.GetChatControl.IsPlayerMuted(chatId))
			{
				return;
			}
			_chatId = chatId;
			int result;
			if (IsServiceMessage(chatMessage))
			{
				SetServiceData(_chatWindow.ChatConfig.GetPersonalChatData(chatMessage.MessageType));
			}
			else if (chatMessage.MessageType == ChatMessageType.Text)
			{
				MessageText.text = chatMessage.EnteredText;
				MessageText.SetActive(active: true);
				Smile.SetActive(active: false);
				ShowServiceItems(show: false);
			}
			else if (chatMessage.MessageType == ChatMessageType.Smile && int.TryParse(chatMessage.EnteredText, out result))
			{
				Sprite smileById = _chatWindow.ChatConfig.GetSmileById(result);
				if ((bool)smileById)
				{
					Smile.sprite = smileById;
					Smile.SetActive(active: true);
				}
				else
				{
					Smile.SetActive(active: false);
				}
				MessageText.SetActive(active: false);
				ShowServiceItems(show: false);
			}
			PersonalChatView personalChatByPlayerId = _chatWindow.GetPersonalChatByPlayerId(_chatId);
			SetUnreadCount(personalChatByPlayerId.UnreadMessageCount);
			PlayerNameText.text = personalChatByPlayerId.PlayerName;
			base.gameObject.SetActive(value: true);
			if (chatMessage.MessageType == ChatMessageType.InviteToFriends)
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ChatFriendRequestSwitch, base.gameObject);
			}
			else
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ChatMessageSwitch, base.gameObject);
			}
			DelayedHide().Forget();
		}

		public void Hide()
		{
			base.gameObject.SetActive(value: false);
		}

		private async UniTask DelayedHide()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(5.0));
			if (!_isDestroyed)
			{
				Hide();
			}
		}

		private void OpenPersonalChat()
		{
			Hide();
			_chatWindow.OpenPersonalChat(_chatId);
		}

		private void SetServiceData(PersonalChatData data)
		{
			if (data != null)
			{
				ServiceDescriptionText.text = new LocalizedString(data.ServiceName);
				ServiceIcon.sprite = data.ServiceIcon;
				ServiceSign.sprite = data.ServiceIcon;
				ServiceSignBackground.color = data.ServiceColor;
				Smile.SetActive(active: false);
				MessageText.SetActive(active: false);
				ShowServiceItems(show: true);
			}
		}

		private void ShowServiceItems(bool show)
		{
			ServiceDescriptionText.SetActive(show);
			ServiceIcon.SetActive(show);
			ServiceSign.SetActive(show);
			ServiceSignBackground.SetActive(show);
		}

		private void SetUnreadCount(int unreadCount)
		{
			UnreadCount.text = unreadCount.ToString();
			UnreadCounter.SetActive(unreadCount > 0);
		}

		private bool IsServiceMessage(ChatMessage message)
		{
			ChatMessageType messageType = message.MessageType;
			return messageType == ChatMessageType.CarExchange || messageType == ChatMessageType.CarSell || messageType == ChatMessageType.SendMoney || messageType == ChatMessageType.InviteToFriends;
		}
	}
}
