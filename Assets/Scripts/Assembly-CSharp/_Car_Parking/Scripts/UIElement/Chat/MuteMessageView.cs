using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class MuteMessageView : ChatMessageView
	{
		[SerializeField]
		private GameObject MutePanel;

		[SerializeField]
		private GameObject UnmutePanel;

		public override void Init(ChatWindow chatWindow, ChatMessage chatMessage, string chatId)
		{
			CurrentMessage = chatMessage;
			MainChatWindow = chatWindow;
			ChatId = chatId;
			if (int.TryParse(chatMessage.EnteredText, out var result))
			{
				bool flag = result == 1;
				MutePanel.SetActive(flag);
				UnmutePanel.SetActive(!flag);
			}
		}
	}
}
