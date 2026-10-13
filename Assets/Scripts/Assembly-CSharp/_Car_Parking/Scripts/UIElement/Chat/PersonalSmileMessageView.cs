using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class PersonalSmileMessageView : ChatMessageView
	{
		[SerializeField]
		private Image SmileImage;

		public override void Init(ChatWindow chatWindow, ChatMessage chatMessage, string chatId)
		{
			base.Init(chatWindow, chatMessage, chatId);
			if (int.TryParse(chatMessage.EnteredText, out var result))
			{
				Sprite smileById = MainChatWindow.ChatConfig.GetSmileById(result);
				if ((bool)smileById)
				{
					SmileImage.sprite = smileById;
					SmileImage.SetActive(active: true);
				}
				else
				{
					SmileImage.SetActive(active: false);
				}
			}
		}
	}
}
