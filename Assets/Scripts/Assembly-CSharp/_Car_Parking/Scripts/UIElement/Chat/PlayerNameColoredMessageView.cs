using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class PlayerNameColoredMessageView : ChatMessageView
	{
		public static readonly Color DefaultNameColor = new Color(0.93f, 0.77f, 0f);

		public static readonly Color FriendNameColor = new Color(0.64f, 0.96f, 0.16f);

		private static readonly Color OwnNameColor = new Color(0.4f, 0.62f, 0.95f);

		[SerializeField]
		protected Button NickNameButton;

		[SerializeField]
		protected TextMeshProUGUI NickName;

		public void Init(ChatWindow chatWindow, ChatMessage chatMessage)
		{
			CurrentMessage = chatMessage;
			MainChatWindow = chatWindow;
			string text = ChatHelper.RemoveRichText(CurrentMessage.FromPlayerName);
			TMP_FontAsset_Placeholder.EnsureFor(text);
			NickName.text = text + ":";
			NickName.color = DefaultNameColor;
			if (MainChatWindow.MessageFromFriend(chatMessage))
			{
				NickName.color = FriendNameColor;
			}
			else if (MainChatWindow.IsOwnMessage(chatMessage))
			{
				NickName.color = OwnNameColor;
			}
			if ((bool)NickNameButton)
			{
				NickNameButton.onClick.RemoveAllListeners();
				NickNameButton.onClick.AddListener(OnNickNameClicked);
			}
			SetMessage();
		}

		private void OnNickNameClicked()
		{
			MainChatWindow.OnNickClicked(CurrentMessage.FromPlayerId, CurrentMessage.FromPlayerName);
		}

		protected virtual void SetMessage()
		{
		}
	}
}
