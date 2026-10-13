using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.MenuUI
{
	public class PointerEnterExitHandler : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler
	{
		public event Action PointerEnter;

		public event Action PointerExit;

		public void OnPointerUp(PointerEventData eventData)
		{
			PointerExit?.Invoke();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			PointerEnter?.Invoke();
		}

		private void OnDestroy()
		{
			PointerEnter = null;
			PointerExit = null;
		}
	}
}
