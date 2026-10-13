using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class ChatWorldMessage : MonoBehaviour
	{
		public Button messageButton;

		public string playerID;

		public string chatTypeString;

		public string playerName;

		public bool openChatOnClick;

		public ChatControl ChatControl { get; set; }

		public void Start()
		{
			messageButton.onClick.AddListener(OpenChat);
		}

		private void OpenChat()
		{
			if (openChatOnClick && !(chatTypeString == playerID))
			{
				ChatControl.SetActiveChatType(chatTypeString, playerName);
				ChatControl.ShowChatFull();
			}
		}
	}
}
