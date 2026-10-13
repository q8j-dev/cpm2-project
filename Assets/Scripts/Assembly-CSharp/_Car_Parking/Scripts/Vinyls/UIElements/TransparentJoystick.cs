using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Vinyls.UIElements
{
	public class TransparentJoystick : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerUpHandler, IPointerDownHandler
	{
		[SerializeField]
		private Image MainImage = default;

		private Color _originalColor;

		public bool IsDrag { get; private set; }

		public Vector2 InputDirection { get; set; }

		private void Awake()
		{
			_originalColor = MainImage.color;
		}

		public void OnDrag(PointerEventData pointerEventData)
		{
			IsDrag = true;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(MainImage.rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out var localPoint))
			{
				localPoint.x /= MainImage.rectTransform.rect.width;
				localPoint.y /= MainImage.rectTransform.rect.height;
				InputDirection = localPoint;
				InputDirection = ((InputDirection.magnitude > 1f) ? InputDirection.normalized : InputDirection);
			}
		}

		public void OnPointerDown(PointerEventData pointerEventData)
		{
			OnDrag(pointerEventData);
			MainImage.DOColor(Color.clear, 0.5f);
		}

		public void OnPointerUp(PointerEventData pointerEventData)
		{
			IsDrag = false;
			InputDirection = Vector3.zero;
			MainImage.DOColor(_originalColor, 0.5f);
		}
	}
}
