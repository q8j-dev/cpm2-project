using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.UIElement
{
	public class ScrollSnapBar : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
	{
		public SimpleScrollSnap ScrollSnap;

		public void OnDrag(PointerEventData eventData)
		{
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnDrag(eventData);
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnBeginDrag(eventData);
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnEndDrag(eventData);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnPointerDown(eventData);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if ((bool)ScrollSnap)
			{
				ScrollSnap.OnPointerUp(eventData);
			}
		}
	}
}
