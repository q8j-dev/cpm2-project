using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouchControlsKit
{
	[RequireComponent(typeof(Image))]
	public class TCKTouchpad : ControllerBase, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerClickHandler
	{
		private GameObject prevPointerPressGO;

		internal override void UpdatePosition(Vector2 touchPos)
		{
			if (!axisX.enabled && !axisY.enabled)
			{
				return;
			}
			if (touchDown)
			{
				if (axisX.enabled)
				{
					currentPosition.x = touchPos.x;
				}
				if (axisY.enabled)
				{
					currentPosition.y = touchPos.y;
				}
				currentDirection = currentPosition - defaultPosition;
				float num = Vector2.Distance(defaultPosition, currentPosition) * 2f;
				defaultPosition = currentPosition;
				float num2 = currentDirection.normalized.x * num / 100f * sensitivity;
				float num3 = currentDirection.normalized.y * num / 100f * sensitivity;
				num2 = (axisX.inverse ? (0f - num2) : num2);
				num3 = (axisX.inverse ? (0f - num3) : num3);
				SetAxis(num2, num3);
			}
			else
			{
				touchDown = true;
				currentPosition = (defaultPosition = touchPos);
				UpdatePosition(touchPos);
				DownHandler();
			}
		}

		internal override void ControlReset()
		{
			base.ControlReset();
			UpHandler();
		}

		public void OnPointerEnter(PointerEventData pointerData)
		{
			if (!pointerData.pointerPress)
			{
				return;
			}
			if (pointerData.pointerPress == base.gameObject)
			{
				OnPointerDown(pointerData);
				return;
			}
			TCKButton component = pointerData.pointerPress.GetComponent<TCKButton>();
			if ((bool)component && component.swipeOut)
			{
				prevPointerPressGO = pointerData.pointerPress;
				pointerData.pointerDrag = base.gameObject;
				pointerData.pointerPress = base.gameObject;
				OnPointerDown(pointerData);
			}
		}

		public void OnPointerDown(PointerEventData pointerData)
		{
			if (!touchDown)
			{
				touchId = pointerData.pointerId;
				UpdatePosition(pointerData.position);
			}
		}

		public void OnDrag(PointerEventData pointerData)
		{
			if (Input.touchCount >= touchId && touchDown)
			{
				UpdatePosition(pointerData.position);
				StopCoroutine("UpdateEndPosition");
				StartCoroutine("UpdateEndPosition", pointerData.position);
			}
		}

		private IEnumerator UpdateEndPosition(Vector2 position)
		{
			yield return new WaitForSeconds(0.0025f);
			if (touchDown)
			{
				UpdatePosition(position);
			}
			else
			{
				ControlReset();
			}
		}

		public void OnPointerUp(PointerEventData pointerData)
		{
			if ((bool)prevPointerPressGO)
			{
				ExecuteEvents.Execute(prevPointerPressGO, pointerData, ExecuteEvents.pointerUpHandler);
				prevPointerPressGO = null;
			}
			ControlReset();
		}

		public void OnPointerClick(PointerEventData pointerData)
		{
			ClickHandler();
		}
	}
}
