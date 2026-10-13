using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Car_Parking.Scripts;

public class FloatingJoystick : Joystick
{
	[SerializeField]
	private bool IsHidable = true;

	[SerializeField]
	private float DefaultAlpha = 0.5f;

	[SerializeField]
	private CanvasGroup BackgroundCanvasGroup;

	[SerializeField]
	private bool disableFloat;

	private Vector3 _defaultPosition;

	private Image _ringImage;

	private Image _knobImage;

	protected override void Start()
	{
		base.Start();
		_defaultPosition = background.transform.position;
		_ringImage = background.GetComponent<Image>();
		_knobImage = (base.knobRect ? base.knobRect.GetComponent<Image>() : null);
		if (disableFloat)
		{
			background.gameObject.SetActive(value: true);
		}
		else if (IsHidable)
		{
			background.gameObject.SetActive(value: false);
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		if (disableFloat)
		{
			base.OnPointerDown(eventData);
			return;
		}
		background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
		SetJoystickGraphics(visible: true);
		if (IsHidable)
		{
			background.gameObject.SetActive(value: true);
		}
		else if ((bool)BackgroundCanvasGroup)
		{
			BackgroundCanvasGroup.alpha = 1f;
		}
		base.OnPointerDown(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		if (disableFloat)
		{
			base.OnPointerUp(eventData);
			return;
		}
		if (IsHidable)
		{
			if ((InputStorage.SprintLocked || InputStorage.SprintLockArmed) && InputStorage.SprintTierActive)
			{
				SetJoystickGraphics(visible: false);
			}
			else
			{
				background.gameObject.SetActive(value: false);
			}
		}
		else
		{
			background.DOKill();
			background.DOMove(_defaultPosition, 0.1f).SetUpdate(isIndependentUpdate: true);
			if ((bool)BackgroundCanvasGroup)
			{
				BackgroundCanvasGroup.alpha = DefaultAlpha;
			}
		}
		base.OnPointerUp(eventData);
	}

	private void SetJoystickGraphics(bool visible)
	{
		if ((bool)_ringImage)
		{
			_ringImage.enabled = visible;
		}
		if ((bool)_knobImage)
		{
			_knobImage.enabled = visible;
		}
	}
}
