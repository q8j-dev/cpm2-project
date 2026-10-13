using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class GameObjectStateUpdaterByButton : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		[SerializeField]
		private GameObjectStateUpdater GameObjectStateUpdater;

		private void OnEnable()
		{
			if ((bool)Button)
			{
				Button.onClick.AddListener(ButtonClickCallback);
			}
		}

		private void OnDisable()
		{
			if ((bool)Button)
			{
				Button.onClick.RemoveListener(ButtonClickCallback);
			}
		}

		private void ButtonClickCallback()
		{
			if ((bool)GameObjectStateUpdater)
			{
				GameObjectStateUpdater.RevertState();
			}
		}
	}
}
