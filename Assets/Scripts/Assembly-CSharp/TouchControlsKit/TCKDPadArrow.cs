using UnityEngine;
using UnityEngine.UI;

namespace TouchControlsKit
{
	[RequireComponent(typeof(Image))]
	public class TCKDPadArrow : MonoBehaviour
	{
		public enum ArrowTypes
		{
			none = 0,
			UP = 1,
			DOWN = 2,
			LEFT = 3,
			RIGHT = 4
		}

		public ArrowTypes ArrowType;

		[SerializeField]
		private RectTransform touchzoneRect;

		[SerializeField]
		private Image touchzoneImage;

		internal bool isPressed { get; private set; }

		internal float INDEX { get; private set; }

		internal void DPadArrowAwake(Sprite sprite, Color32 color)
		{
			touchzoneRect = base.gameObject.GetComponent<RectTransform>();
			touchzoneImage = base.gameObject.GetComponent<Image>();
			touchzoneImage.sprite = sprite;
			touchzoneImage.color = color;
		}

		internal void SetArrowActive(Sprite sprite, Color32 color, bool pressed)
		{
			touchzoneImage.sprite = sprite;
			touchzoneImage.color = color;
			isPressed = pressed;
		}

		private bool CheckBoolPosition(Vector2 touchPos, float sizeX, float sizeY)
		{
			float num = touchzoneRect.sizeDelta.x / 2f;
			float num2 = touchzoneRect.sizeDelta.x / 2f;
			switch (ArrowType)
			{
			case ArrowTypes.UP:
			case ArrowTypes.DOWN:
				if (touchPos.x < touchzoneRect.position.x + sizeX / 2f && touchPos.y < touchzoneRect.position.y + num2 / 2f && touchPos.x > touchzoneRect.position.x - sizeX / 2f && touchPos.y > touchzoneRect.position.y - num2 / 2f)
				{
					return true;
				}
				break;
			case ArrowTypes.LEFT:
			case ArrowTypes.RIGHT:
				if (touchPos.x < touchzoneRect.position.x + num / 2f && touchPos.y < touchzoneRect.position.y + sizeY / 2f && touchPos.x > touchzoneRect.position.x - num / 2f && touchPos.y > touchzoneRect.position.y - sizeY / 2f)
				{
					return true;
				}
				break;
			}
			return false;
		}

		internal bool CheckTouchPosition(Vector2 touchPos, float sizeX, float sizeY)
		{
			if (CheckBoolPosition(touchPos, sizeX, sizeY))
			{
				switch (ArrowType)
				{
				case ArrowTypes.UP:
				case ArrowTypes.RIGHT:
					INDEX = 1f;
					break;
				case ArrowTypes.DOWN:
				case ArrowTypes.LEFT:
					INDEX = -1f;
					break;
				case ArrowTypes.none:
					Debug.LogError("ERROR: Arrow type " + base.gameObject.name + " is not assigned!");
					INDEX = 0f;
					return false;
				}
				return true;
			}
			INDEX = 0f;
			return false;
		}
	}
}
