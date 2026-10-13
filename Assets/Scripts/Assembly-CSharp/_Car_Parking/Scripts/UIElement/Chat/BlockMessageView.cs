using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class BlockMessageView : ChatMessageView
	{
		[SerializeField]
		private GameObject BlockPanel;

		[SerializeField]
		private GameObject UnblockPanel;

		public override void Init(ChatWindow chatWindow, ChatMessage chatMessage, string chatId)
		{
			CurrentMessage = chatMessage;
			MainChatWindow = chatWindow;
			ChatId = chatId;
			if (int.TryParse(chatMessage.EnteredText, out var result))
			{
				bool flag = result == 1;
				BlockPanel.SetActive(flag);
				UnblockPanel.SetActive(!flag);
			}
		}
	}
}
