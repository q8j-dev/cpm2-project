using System.Collections;
using TouchControlsKit.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouchControlsKit
{
	[RequireComponent(typeof(Image))]
	public class TCKJoystick : ControllerBase, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IDragHandler, IPointerClickHandler
	{
		public Image joystickImage;

		public Image joystickBackgroundImage;

		public RectTransform joystickRT;

		public RectTransform joystickBackgroundRT;

		[SerializeField]
		private bool isStatic = true;

		public float borderSize = 5.85f;

		private Vector2 borderPosition = Vector2.zero;

		public bool smoothReturn;

		public float smoothFactor = 7f;

		private float xVel;

		private float yVel;

		public bool IsStatic
		{
			get
			{
				return isStatic;
			}
			set
			{
				if (isStatic != value)
				{
					isStatic = value;
				}
			}
		}

		public override void ControlAwake()
		{
			base.ControlAwake();
			SetTransparency();
		}

		internal override void UpdatePosition(Vector2 touchPos)
		{
			if (!axisX.enabled && !axisY.enabled)
			{
				return;
			}
			if (touchDown)
			{
				GetCurrentPosition(touchPos);
				currentDirection = currentPosition - defaultPosition;
				float num = Vector2.Distance(defaultPosition, currentPosition);
				float num2 = 100f;
				float num3 = joystickBackgroundRT.sizeDelta.magnitude / 2f * borderSize / 8f;
				borderPosition = defaultPosition;
				borderPosition += currentDirection.normalized * num3;
				if (num > num3)
				{
					currentPosition = borderPosition;
				}
				else
				{
					num2 = num / num3 * 100f;
				}
				UpdateJoystickPosition();
				float num4 = currentDirection.normalized.x * num2 / 100f * sensitivity;
				float num5 = currentDirection.normalized.y * num2 / 100f * sensitivity;
				num4 = (axisX.inverse ? (0f - num4) : num4);
				num5 = (axisX.inverse ? (0f - num5) : num5);
				SetAxis(num4, num5);
			}
			else
			{
				touchDown = true;
				UpdatePosition(touchPos);
				SetAxis(0f, 0f);
				if (!isStatic)
				{
					UpdateTransparencyAndPosition(touchPos);
				}
				DownHandler();
			}
		}

		private void GetCurrentPosition(Vector2 touchPos)
		{
			defaultPosition = (currentPosition = joystickBackgroundRT.position);
			if (axisX.enabled)
			{
				currentPosition.x = GuiCamera.ScreenToWorldPoint(touchPos).x;
			}
			if (axisY.enabled)
			{
				currentPosition.y = GuiCamera.ScreenToWorldPoint(touchPos).y;
			}
		}

		private void UpdateJoystickPosition()
		{
			joystickRT.position = currentPosition;
		}

		private void UpdateTransparencyAndPosition(Vector2 touchPos)
		{
			Image image = joystickImage;
			bool flag = (joystickBackgroundImage.enabled = true);
			image.enabled = flag;
			RectTransform rectTransform = joystickRT;
			Vector3 position = (joystickBackgroundRT.position = GuiCamera.ScreenToWorldPoint(touchPos));
			rectTransform.position = position;
		}

		private IEnumerator SmoothReturnRun()
		{
			bool smoothReturnIsRun = true;
			while (smoothReturnIsRun && !touchDown && isStatic)
			{
				int num = Mathf.RoundToInt(defaultPosition.sqrMagnitude);
				int num2 = Mathf.RoundToInt(currentPosition.sqrMagnitude);
				currentPosition.x = Mathf.SmoothDamp(currentPosition.x, defaultPosition.x, ref xVel, smoothFactor * Time.smoothDeltaTime);
				currentPosition.y = Mathf.SmoothDamp(currentPosition.y, defaultPosition.y, ref yVel, smoothFactor * Time.smoothDeltaTime);
				if (num2 == num)
				{
					currentPosition = defaultPosition;
					smoothReturnIsRun = false;
					xVel = (yVel = 0f);
				}
				UpdateJoystickPosition();
				yield return null;
			}
		}

		private void SetTransparency()
		{
			Image image = joystickImage;
			bool flag = (joystickBackgroundImage.enabled = isStatic);
			image.enabled = flag;
		}

		internal override void ControlReset()
		{
			base.ControlReset();
			SetTransparency();
			if (smoothReturn && isStatic)
			{
				StopCoroutine("SmoothReturnRun");
				StartCoroutine("SmoothReturnRun");
			}
			else
			{
				currentPosition = defaultPosition;
				UpdateJoystickPosition();
			}
			UpHandler();
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
