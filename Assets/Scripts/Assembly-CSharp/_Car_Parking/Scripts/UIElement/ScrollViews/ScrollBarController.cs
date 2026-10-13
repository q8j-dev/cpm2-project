using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class ScrollBarController : MonoBehaviour
	{
		private enum MoveAxisType
		{
			Horizontal = 0,
			Vertical = 1
		}

		[SerializeField]
		private Scrollbar Scrollbar = default;

		[SerializeField]
		private RectTransformEventSender Content = default;

		[SerializeField]
		private RectTransformEventSender Root = default;

		[SerializeField]
		private MoveAxisType MoveAxis = default;

		private float RootSize
		{
			get
			{
				if ((bool)Root)
				{
					if (MoveAxis != MoveAxisType.Horizontal)
					{
						return Root.Size.y;
					}
					return Root.Size.x;
				}
				return 1f;
			}
		}

		private float ContentSize
		{
			get
			{
				if ((bool)Content)
				{
					if (MoveAxis != MoveAxisType.Horizontal)
					{
						return Content.Size.y;
					}
					return Content.Size.x;
				}
				return 1f;
			}
		}

		private float ContentPosition
		{
			get
			{
				if ((bool)Content)
				{
					if (MoveAxis != MoveAxisType.Horizontal)
					{
						return Content.AnchoredPosition.y;
					}
					return 0f - Content.AnchoredPosition.x;
				}
				return 1f;
			}
		}

		private void OnEnable()
		{
			if ((bool)Content)
			{
				Content.OnSizeChanged += ContentSizeChangedCallback;
				Content.OnAnchoredPositionChanged += ContentAnchoredPositionChangedCallback;
			}
			if ((bool)Root)
			{
				Root.OnSizeChanged += RootSizeChangedCallback;
			}
			if ((bool)Scrollbar)
			{
				Scrollbar.onValueChanged.AddListener(ScrollbarValueChangedCallback);
			}
			UpdateSize();
		}

		private void OnDisable()
		{
			if ((bool)Content)
			{
				Content.OnSizeChanged -= ContentSizeChangedCallback;
				Content.OnAnchoredPositionChanged -= ContentAnchoredPositionChangedCallback;
			}
			if ((bool)Root)
			{
				Root.OnSizeChanged -= RootSizeChangedCallback;
			}
			if ((bool)Scrollbar)
			{
				Scrollbar.onValueChanged.RemoveListener(ScrollbarValueChangedCallback);
			}
		}

		private void ContentSizeChangedCallback(Vector2 size)
		{
			UpdateSize();
		}

		private void RootSizeChangedCallback(Vector2 size)
		{
			UpdateSize();
		}

		private void ContentAnchoredPositionChangedCallback(Vector2 anchoredPosition)
		{
			UpdatePosition();
		}

		private void ScrollbarValueChangedCallback(float value)
		{
			float num = ContentSize - RootSize;
			float num2 = value * num;
			if ((bool)Content && MoveAxis == MoveAxisType.Horizontal)
			{
				Content.AnchoredPosition = new Vector2(0f - num2, Content.AnchoredPosition.y);
			}
			else if ((bool)Content && MoveAxis == MoveAxisType.Vertical)
			{
				Content.AnchoredPosition = new Vector2(Content.AnchoredPosition.x, num2);
			}
		}

		private void UpdateSize()
		{
			if ((bool)Scrollbar && RootSize > 1f && ContentSize > 1f)
			{
				Scrollbar.size = RootSize / ContentSize;
				UpdatePosition();
			}
		}

		private void UpdatePosition()
		{
			if ((bool)Scrollbar && RootSize > 1f && ContentSize > 1f)
			{
				float num = ContentSize - RootSize;
				if (num > 0.01f)
				{
					Scrollbar.SetValueWithoutNotify(ContentPosition / num);
				}
				else
				{
					Scrollbar.SetValueWithoutNotify(0f);
				}
			}
		}
	}
}
