using UnityEngine;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ChatMessageView : MonoBehaviour
	{
		protected string ChatId;

		protected ChatMessage CurrentMessage;

		protected ChatWindow MainChatWindow;

		protected ManagerUI ManagerUI;

		public virtual void Init(ChatWindow chatWindow, ChatMessage chatMessage, string chatId)
		{
			CurrentMessage = chatMessage;
			MainChatWindow = chatWindow;
			ChatId = chatId;
		}
	}
}
