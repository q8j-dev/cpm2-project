using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ChatHandlerBase : MonoBehaviour
	{
		public const int OneFrameSpawningMessagesMaxCount = 40;

		[SerializeField]
		protected ChatWindow ChatWindow;

		[SerializeField]
		protected GameObject Content;

		[SerializeField]
		protected ChatNotification Notification;

		[SerializeField]
		protected RectTransform Container;

		[SerializeField]
		protected GameObject Unread;

		[SerializeField]
		protected TextMeshProUGUI UnreadText;

		[SerializeField]
		protected GameObject InputPanel;

		[SerializeField]
		protected Toggle SmileToggle;

		[SerializeField]
		protected RectTransform ContentRect;

		protected int UnreadCount;

		protected bool StopAsyncOperation;

		protected CancellationTokenSource DelayedSpawnCancellation;

		[field: SerializeField]
		public ChatType ChatType { get; private set; }

		public bool ActiveNow { get; private set; }

		protected virtual void Start()
		{
			Notification.Init(ChatWindow);
		}

		protected virtual void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref DelayedSpawnCancellation);
			StopAsyncOperation = true;
		}

		public virtual void OpenChat()
		{
			ActiveNow = true;
			SmileToggle.isOn = false;
			Notification.Hide();
			StopAsyncOperation = false;
		}

		public virtual void CloseChat()
		{
			ActiveNow = false;
			Notification.Hide();
			StopAsyncOperation = true;
		}

		public virtual void NewMessage(ChatMessage chatMessage)
		{
		}

		public virtual void ShowNotification(ChatMessage chatMessage, string chatId)
		{
			Notification.Show(chatMessage, chatId);
		}

		public virtual void OnChatWindowClosed()
		{
		}

		public virtual void SetUnreadCount(int unreadCount)
		{
			unreadCount = ((unreadCount >= 0) ? unreadCount : 0);
			UnreadText.text = unreadCount.ToString();
			Unread.SetActive(unreadCount > 0);
		}

		public virtual void RebuildContainerLayout()
		{
			UIHelper.DelayedRebuildLayout(Container).Forget();
			Debug.Log("RebuildContainerLayout");
		}
	}
}
