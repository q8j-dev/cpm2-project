using UnityEngine;
using UnityEngine.EventSystems;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement
{
	public class ServiceMapTouchHandler : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler
	{
		[SerializeField]
		private PhoneWindow PhoneWindow = default;

		public void OnDrag(PointerEventData eventData)
		{
			PhoneWindow.IsMapDrag = true;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (PhoneWindow.IsMapDrag)
			{
				PhoneWindow.IsMapDrag = false;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			PhoneWindow.IsMapDrag = true;
		}
	}
}
