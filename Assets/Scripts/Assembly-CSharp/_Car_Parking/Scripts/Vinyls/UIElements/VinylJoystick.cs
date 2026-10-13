using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Vinyls.UIElements
{
	public class VinylJoystick : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler
	{
		public bool isDrag;

		private Image backPanel;

		private Image knob;

		public Vector2 InputDirection { get; set; }

		private void Start()
		{
			backPanel = GetComponent<Image>();
			knob = base.transform.GetChild(0).GetComponent<Image>();
		}

		public void OnDrag(PointerEventData pointerEventData)
		{
			isDrag = true;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(backPanel.rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out var localPoint))
			{
				localPoint.x /= backPanel.rectTransform.sizeDelta.x;
				localPoint.y /= backPanel.rectTransform.sizeDelta.y;
				InputDirection = localPoint;
				InputDirection = ((InputDirection.magnitude > 1f) ? InputDirection.normalized : InputDirection);
				knob.rectTransform.anchoredPosition = new Vector3(InputDirection.x * (backPanel.rectTransform.sizeDelta.x / 3f), InputDirection.y * (backPanel.rectTransform.sizeDelta.y / 3f));
			}
		}

		public void OnPointerDown(PointerEventData pointerEventData)
		{
			OnDrag(pointerEventData);
		}

		public void OnPointerUp(PointerEventData pointerEventData)
		{
			isDrag = false;
			InputDirection = Vector3.zero;
			knob.rectTransform.anchoredPosition = Vector3.zero;
		}
	}
}
