using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TouchControlsKit
{
	[RequireComponent(typeof(Image))]
	public class TCKButton : ControllerBase, IPointerExitHandler, IEventSystemHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		public bool swipeOut;

		[SerializeField]
		private Sprite normalsprite;

		public Sprite pressedSprite;

		public Color32 normalColor;

		public Color32 pressedColor;

		private int pressedFrame = -1;

		private int releasedFrame = -1;

		public Sprite normalSprite
		{
			get
			{
				return normalsprite;
			}
			set
			{
				if (!(normalsprite == value))
				{
					normalsprite = value;
					touchzoneImage.sprite = normalsprite;
				}
			}
		}

		internal bool ButtonPRESSED => touchDown;

		internal bool ButtonDOWN => pressedFrame == Time.frameCount - 1;

		internal bool ButtonUP => releasedFrame == Time.frameCount - 1;

		internal override void UpdatePosition(Vector2 touchPos)
		{
			if (!touchDown)
			{
				touchDown = true;
				pressedFrame = Time.frameCount;
				ButtonDown();
				DownHandler();
			}
		}

		protected void ButtonDown()
		{
			touchzoneImage.sprite = pressedSprite;
			touchzoneImage.color = pressedColor;
		}

		protected void ButtonUp()
		{
			touchzoneImage.sprite = normalSprite;
			touchzoneImage.color = normalColor;
		}

		internal override void ControlReset()
		{
			base.ControlReset();
			releasedFrame = Time.frameCount;
			ButtonUp();
			UpHandler();
		}

		protected override void ShowHideTouchZone()
		{
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

		public void OnPointerExit(PointerEventData pointerData)
		{
			if (!swipeOut)
			{
				OnPointerUp(pointerData);
			}
		}

		public void OnPointerUp(PointerEventData pointerData)
		{
			ControlReset();
		}

		public void OnPointerClick(PointerEventData pointerData)
		{
			ClickHandler();
		}
	}
}
