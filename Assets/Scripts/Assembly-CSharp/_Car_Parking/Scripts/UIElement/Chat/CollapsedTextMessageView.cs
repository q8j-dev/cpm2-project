using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class CollapsedTextMessageView : PlayerNameColoredMessageView
	{
		[SerializeField]
		private Text Message;

		[SerializeField]
		private RectTransform LayoutGroupRect;

		protected override void SetMessage()
		{
			string text = "<color=#00000000>" + NickName.text + "</color> " + CurrentMessage.EnteredText;
			Message.text = text;
			LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutGroupRect);
		}
	}
}
