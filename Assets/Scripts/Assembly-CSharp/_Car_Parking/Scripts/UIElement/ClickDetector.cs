using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.UIElement
{
	public class ClickDetector : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		public event Action<bool> OnPressed;

		public void OnPointerDown(PointerEventData eventData)
		{
			OnPressed?.Invoke(obj: true);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			OnPressed?.Invoke(obj: false);
		}

		private void OnDestroy()
		{
			UnSubscribe();
		}

		public void UnSubscribe()
		{
			OnPressed = null;
		}
	}
}
