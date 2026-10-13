using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class MaxContentSizeFitter : ContentSizeFitter
	{
		[SerializeField]
		private bool _useMaxHorizontal = default;

		[SerializeField]
		private float _maxHorizontal = default;

		[SerializeField]
		private bool _useMaxVertical = default;

		[SerializeField]
		private float _maxVertical = default;

		private RectTransform _rect;

		private RectTransform Rect
		{
			get
			{
				if (!(_rect != null))
				{
					return _rect = (RectTransform)base.transform;
				}
				return _rect;
			}
		}

		public override void SetLayoutHorizontal()
		{
			base.SetLayoutHorizontal();
			if (_useMaxHorizontal && Rect.rect.width > _maxHorizontal)
			{
				Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _maxHorizontal);
			}
		}

		public override void SetLayoutVertical()
		{
			base.SetLayoutVertical();
			if (_useMaxVertical && Rect.rect.height > _maxVertical)
			{
				Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _maxVertical);
			}
		}
	}
}
