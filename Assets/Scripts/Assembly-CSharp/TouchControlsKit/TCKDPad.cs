using TouchControlsKit.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouchControlsKit
{
	[RequireComponent(typeof(Image))]
	public class TCKDPad : ControllerBase, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IDragHandler, IPointerClickHandler
	{
		public Sprite normalSprite;

		public Sprite pressedSprite;

		public Color32 normalColor;

		public Color32 pressedColor;

		[SerializeField]
		private TCKDPadArrow[] myArrows;

		private Vector2 borderPosition = Vector2.zero;

		private float sizeX;

		private float sizeY;

		public override void ControlAwake()
		{
			base.ControlAwake();
			myArrows = GetComponentsInChildren<TCKDPadArrow>();
			for (int i = 0; i < myArrows.Length; i++)
			{
				myArrows[i].DPadArrowAwake(normalSprite, normalColor);
			}
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
				float calcX = 0f;
				float calcY = 0f;
				CalculateBorderSize(out calcX, out calcY);
				borderPosition = defaultPosition;
				borderPosition.x += currentDirection.normalized.x * calcX;
				borderPosition.y += currentDirection.normalized.y * calcY;
				float num = Vector2.Distance(defaultPosition, currentPosition);
				if (num > calcX || num > calcY)
				{
					currentPosition = borderPosition;
				}
				float num2 = 0f;
				float num3 = 0f;
				for (int i = 0; i < myArrows.Length; i++)
				{
					if (myArrows[i].CheckTouchPosition(currentPosition, sizeX, sizeY))
					{
						ArrowDown(i);
						if (myArrows[i].ArrowType == TCKDPadArrow.ArrowTypes.UP || myArrows[i].ArrowType == TCKDPadArrow.ArrowTypes.DOWN)
						{
							num3 = myArrows[i].INDEX * sensitivity;
						}
						if (myArrows[i].ArrowType == TCKDPadArrow.ArrowTypes.RIGHT || myArrows[i].ArrowType == TCKDPadArrow.ArrowTypes.LEFT)
						{
							num2 = myArrows[i].INDEX * sensitivity;
						}
						continue;
					}
					ArrowUp(i);
					if (myArrows[i].ArrowType == TCKDPadArrow.ArrowTypes.UP && myArrows[i].INDEX == 0f)
					{
						for (int j = 0; j < myArrows.Length; j++)
						{
							if (myArrows[j].ArrowType == TCKDPadArrow.ArrowTypes.DOWN && myArrows[j].INDEX == 0f)
							{
								num3 = myArrows[j].INDEX * sensitivity;
							}
						}
					}
					if (myArrows[i].ArrowType != TCKDPadArrow.ArrowTypes.RIGHT || myArrows[i].INDEX != 0f)
					{
						continue;
					}
					for (int k = 0; k < myArrows.Length; k++)
					{
						if (myArrows[k].ArrowType == TCKDPadArrow.ArrowTypes.LEFT && myArrows[k].INDEX == 0f)
						{
							num2 = myArrows[k].INDEX * sensitivity;
						}
					}
				}
				num2 = (axisX.inverse ? (0f - num2) : num2);
				num3 = (axisX.inverse ? (0f - num3) : num3);
				SetAxis(num2, num3);
			}
			else
			{
				touchDown = true;
				UpdatePosition(touchPos);
				DownHandler();
			}
		}

		protected void GetCurrentPosition(Vector2 touchPos)
		{
			if (axisX.enabled)
			{
				currentPosition.x = GuiCamera.ScreenToWorldPoint(touchPos).x;
			}
			if (axisY.enabled)
			{
				currentPosition.y = GuiCamera.ScreenToWorldPoint(touchPos).y;
			}
			sizeX = touchzoneRect.sizeDelta.x / 2f;
			sizeY = touchzoneRect.sizeDelta.y / 2f;
			defaultPosition = touchzoneRect.position;
		}

		protected void CalculateBorderSize(out float calcX, out float calcY)
		{
			calcX = touchzoneRect.sizeDelta.x / 6f;
			calcY = touchzoneRect.sizeDelta.y / 6f;
		}

		protected void ArrowDown(int index)
		{
			if (!myArrows[index].isPressed)
			{
				myArrows[index].SetArrowActive(pressedSprite, pressedColor, pressed: true);
			}
		}

		protected void ArrowUp(int index)
		{
			if (myArrows[index].isPressed)
			{
				myArrows[index].SetArrowActive(normalSprite, normalColor, pressed: false);
			}
		}

		internal override void ControlReset()
		{
			base.ControlReset();
			for (int i = 0; i < myArrows.Length; i++)
			{
				ArrowUp(i);
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
