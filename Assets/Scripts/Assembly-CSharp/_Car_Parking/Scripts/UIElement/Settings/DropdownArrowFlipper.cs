using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class DropdownArrowFlipper : TMP_Dropdown
	{
		private const string ArrowChildName = "Arrow";

		private RectTransform _arrow;

		private bool _searched;

		private RectTransform Arrow
		{
			get
			{
				if ((bool)_arrow || _searched)
				{
					return _arrow;
				}
				_searched = true;
				Transform transform = base.transform.Find("Arrow");
				_arrow = transform as RectTransform;
				return _arrow;
			}
		}

		protected override GameObject CreateDropdownList(GameObject template)
		{
			FlipArrow(open: true);
			return base.CreateDropdownList(template);
		}

		protected override void DestroyDropdownList(GameObject dropdownList)
		{
			FlipArrow(open: false);
			base.DestroyDropdownList(dropdownList);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			FlipArrow(open: false);
		}

		private void FlipArrow(bool open)
		{
			if ((bool)Arrow)
			{
				Vector3 localScale = Arrow.localScale;
				localScale.y = (open ? (0f - Mathf.Abs(localScale.y)) : Mathf.Abs(localScale.y));
				Arrow.localScale = localScale;
			}
		}
	}
}
