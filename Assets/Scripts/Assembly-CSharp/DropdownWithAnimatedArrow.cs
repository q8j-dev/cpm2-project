using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DropdownWithAnimatedArrow : Dropdown
{
	public RectTransform Arrow;

	public float Duration = 0.3f;

	private Tween _tween;

	private bool isOpen;

	protected override GameObject CreateDropdownList(GameObject template)
	{
		RotateArrow(open: true);
		return base.CreateDropdownList(template);
	}

	protected override void DestroyDropdownList(GameObject dropdownList)
	{
		RotateArrow(open: false);
		base.DestroyDropdownList(dropdownList);
	}

	public void RotateArrow(bool open)
	{
		_tween?.Kill();
		isOpen = open;
		_tween = Arrow.DORotate(new Vector3(0f, 0f, open ? 180 : 0), Duration).SetEase(Ease.InOutSine);
	}
}
