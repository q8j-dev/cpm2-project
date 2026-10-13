using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class SmileMessageView : PlayerNameColoredMessageView
	{
		[SerializeField]
		private Image SmileImage;

		protected override void SetMessage()
		{
			if (int.TryParse(CurrentMessage.EnteredText, out var result))
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
