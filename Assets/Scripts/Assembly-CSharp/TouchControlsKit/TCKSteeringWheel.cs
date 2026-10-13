using System.Collections;
using TouchControlsKit.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouchControlsKit
{
	[RequireComponent(typeof(Image))]
	public class TCKSteeringWheel : ControllerBase, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IDragHandler, IPointerClickHandler
	{
		public float maxSteeringAngle = 120f;

		public float releasedSpeed = 45f;

		private Vector3 localEulerAngles = Vector3.zero;

		private float wheelAngle;

		private float wheelPrevAngle;

		internal override void UpdatePosition(Vector2 touchPos)
		{
			if (!axisX.enabled)
			{
				return;
			}
			GetCurrentPosition(touchPos);
			if (touchDown)
			{
				float num = Vector2.Angle(Vector2.up, currentPosition - defaultPosition);
				if (currentPosition.x > defaultPosition.x)
				{
					wheelAngle += num - wheelPrevAngle;
				}
				else
				{
					wheelAngle -= num - wheelPrevAngle;
				}
				wheelAngle = Mathf.Clamp(wheelAngle, 0f - maxSteeringAngle, maxSteeringAngle);
				wheelPrevAngle = num;
				UptateWheelRotation();
				float num2 = wheelAngle / maxSteeringAngle * sensitivity;
				num2 = (axisX.inverse ? (0f - num2) : num2);
				SetAxis(num2, 0f);
			}
			else
			{
				touchDown = true;
				StopCoroutine("WheelReturnRun");
				wheelPrevAngle = Vector2.Angle(Vector2.up, currentPosition - defaultPosition);
				UpdatePosition(touchPos);
				DownHandler();
			}
		}

		private void GetCurrentPosition(Vector2 touchPos)
		{
			defaultPosition = (currentPosition = touchzoneRect.position);
			currentPosition.x = GuiCamera.ScreenToWorldPoint(touchPos).x;
			currentPosition.y = GuiCamera.ScreenToWorldPoint(touchPos).y;
		}

		private void UptateWheelRotation()
		{
			localEulerAngles = Vector3.back * wheelAngle;
			touchzoneRect.localEulerAngles = localEulerAngles;
		}

		internal override void ControlReset()
		{
			base.ControlReset();
			StopCoroutine("WheelReturnRun");
			StartCoroutine("WheelReturnRun");
			UpHandler();
		}

		protected override void ShowHideTouchZone()
		{
		}

		private IEnumerator WheelReturnRun()
		{
			while (!Mathf.Approximately(0f, wheelAngle))
			{
				float num = releasedSpeed * Time.smoothDeltaTime * 10f;
				if (Mathf.Abs(num) > Mathf.Abs(wheelAngle))
				{
					wheelAngle = 0f;
				}
				else if (wheelAngle > 0f)
				{
					wheelAngle -= num;
				}
				else
				{
					wheelAngle += num;
				}
				UptateWheelRotation();
				yield return null;
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
			}
		}

		public void OnPointerUp(PointerEventData pointerData)
		{
			UpdatePosition(pointerData.position);
			ControlReset();
		}

		public void OnPointerClick(PointerEventData pointerData)
		{
			ClickHandler();
		}
	}
}
