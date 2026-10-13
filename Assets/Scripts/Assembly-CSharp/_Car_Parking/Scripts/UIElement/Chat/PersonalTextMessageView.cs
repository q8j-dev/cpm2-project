using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class PersonalTextMessageView : ChatMessageView
	{
		[SerializeField]
		private Text MessageText;

		[SerializeField]
		private Text FakeText;

		[SerializeField]
		private RectTransform FakeTextRect;

		[SerializeField]
		private RectTransform BackgroundRect;

		[SerializeField]
		private RectTransform MainRect;

		[SerializeField]
		private float BackgroundOffset = 84f;

		[SerializeField]
		private float BackgroundPadding = 28f;

		[SerializeField]
		private bool IsLocalMessage;

		public override void Init(ChatWindow chatWindow, ChatMessage chatMessage, string chatId)
		{
			base.Init(chatWindow, chatMessage, chatId);
			MessageText.text = chatMessage.EnteredText;
			FakeText.text = MessageText.text;
		}

		private void OnEnable()
		{
			SetBackgroundWidth().Forget();
		}

		private async UniTask SetBackgroundWidth()
		{
			await UniTask.Yield();
			float num = MainRect.sizeDelta.x - BackgroundOffset;
			float num2 = FakeTextRect.sizeDelta.x + BackgroundPadding;
			float width = Mathf.Clamp(num2, 10f, num);
			SetWidth(width);
			if (IsLocalMessage && num2 > num)
			{
				SetTextAlignmentLeft();
			}
		}

		private void SetWidth(float width)
		{
			Vector2 sizeDelta = BackgroundRect.sizeDelta;
			sizeDelta.x = width;
			BackgroundRect.sizeDelta = sizeDelta;
		}

		private void SetTextAlignmentLeft()
		{
			MessageText.alignment = TextAnchor.UpperLeft;
		}
	}
}
