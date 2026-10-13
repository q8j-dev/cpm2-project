using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class TextMessageView : PlayerNameColoredMessageView
	{
		[SerializeField]
		private Text Message;

		protected override void SetMessage()
		{
			string text = "<color=#00000000>" + NickName.text + "</color> " + CurrentMessage.EnteredText;
			Message.text = text;
		}
	}
}
