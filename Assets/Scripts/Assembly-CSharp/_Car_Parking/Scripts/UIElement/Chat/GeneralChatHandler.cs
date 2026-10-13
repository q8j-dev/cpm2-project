using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Multiplayer.Chat;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class GeneralChatHandler : ChatHandlerBase
	{
		private const int DelayFrameCount = 2;

		private readonly List<ChatMessage> _messages = new List<ChatMessage>();

		private readonly List<PlayerNameColoredMessageView> _chatItems = new List<PlayerNameColoredMessageView>();

		[SerializeField]
		private ContentSizeFitter ContainerSizeFitter;

		private GeneralChatMessagePool _messagePool;

		public void Init(GeneralChatMessagePool messagePool)
		{
			_messagePool = messagePool;
		}

		public override void OpenChat()
		{
			base.OpenChat();
			ReturnAllToPool();
			CommonExtensions.CancelAndDisposeToken(ref DelayedSpawnCancellation);
			DelayedSpawnCancellation = new CancellationTokenSource();
			SpawnAllMessages(DelayedSpawnCancellation.Token).Forget();
			Show(show: true);
			UnreadCount = 0;
			SetUnreadCount(UnreadCount);
		}

		public override void CloseChat()
		{
			base.CloseChat();
			Show(show: false);
			ReturnAllToPool();
		}

		public override void NewMessage(ChatMessage chatMessage)
		{
			AddMessageToHistory(chatMessage);
			RemoveOldestChatItem();
			if (!base.ActiveNow)
			{
				UnreadCount++;
				SetUnreadCount(UnreadCount);
			}
			else
			{
				SpawnMessageView(chatMessage);
				DelayedEnableSizeFitter().Forget();
			}
		}

		private void AddMessageToHistory(ChatMessage chatMessage)
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
					_messagePool.Return(_chatItems[index]);
					_chatItems.RemoveAt(index);
				}
			}
		}

		public override void OnChatWindowClosed()
		{
			ReturnAllToPool();
		}

		private void SpawnMessageView(ChatMessage chatMessage)
		{
			ContainerSizeFitter.enabled = false;
			PlayerNameColoredMessageView playerNameColoredMessageView = null;
			if (chatMessage.MessageType == ChatMessageType.Text)
			{
				playerNameColoredMessageView = _messagePool.GetTextMessage(Container);
			}
			else if (chatMessage.MessageType == ChatMessageType.Smile)
			{
				playerNameColoredMessageView = _messagePool.GetSmileMessage(Container);
			}
			if ((bool)playerNameColoredMessageView)
			{
				playerNameColoredMessageView.Init(ChatWindow, chatMessage);
				playerNameColoredMessageView.transform.SetAsLastSibling();
				_chatItems.Add(playerNameColoredMessageView);
			}
		}

		private async UniTask SpawnAllMessages(CancellationToken cancellationToken)
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
				ChatMessage chatMessage = _messages[i];
				SpawnMessageView(chatMessage);
			}
			DelayedEnableSizeFitter().Forget();
		}

		private void ReturnAllToPool()
		{
			CommonExtensions.CancelAndDisposeToken(ref DelayedSpawnCancellation);
			foreach (PlayerNameColoredMessageView chatItem in _chatItems)
			{
				_messagePool.Return(chatItem);
			}
			_chatItems.Clear();
		}

		private async UniTask DelayedEnableSizeFitter()
		{
			await UniTask.DelayFrame(2);
			if (!StopAsyncOperation)
			{
				ContainerSizeFitter.enabled = true;
				UIHelper.DelayedRebuildLayout(Container).Forget();
			}
		}

		private void Show(bool show)
		{
			Content.SetActive(show);
			InputPanel.SetActive(show);
			ContainerSizeFitter.enabled = false;
		}
	}
}
