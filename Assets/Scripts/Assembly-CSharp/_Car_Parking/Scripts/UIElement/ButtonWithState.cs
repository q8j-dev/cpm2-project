using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ButtonWithState : MonoBehaviour
	{
		[SerializeField]
		private Button Button;

		[SerializeField]
		private GameObjectStateUpdater StateUpdater;

		public bool Interactable
		{
			get
			{
				if ((bool)Button)
				{
					return Button.interactable;
				}
				return false;
			}
			set
			{
				Button.interactable = value;
				StateUpdater.UpdateState(Button.interactable);
			}
		}

		public event Action<ButtonWithState> ClickedEvent;

		private void OnEnable()
		{
			Button.onClick.AddListener(OnClickedCallback);
		}

		private void OnDisable()
		{
			Button.onClick.RemoveListener(OnClickedCallback);
		}

		private void OnClickedCallback()
		{
			ClickedEvent?.Invoke(this);
		}
	}
}
