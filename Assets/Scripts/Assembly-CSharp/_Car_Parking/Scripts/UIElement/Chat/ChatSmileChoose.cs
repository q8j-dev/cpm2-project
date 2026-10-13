using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ChatSmileChoose : MonoBehaviour
	{
		[SerializeField]
		private int Id;

		[SerializeField]
		private Button SmileButton;

		[SerializeField]
		private ChatWindow ChatWindow;

		private void Start()
		{
			SmileButton.onClick.AddListener(ChooseSmile);
		}

		private void ChooseSmile()
		{
			ChatWindow.NewSmileMessage(Id);
		}
	}
}
