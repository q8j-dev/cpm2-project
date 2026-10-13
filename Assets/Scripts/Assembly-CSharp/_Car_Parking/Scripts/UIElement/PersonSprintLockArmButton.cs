using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class PersonSprintLockArmButton : MonoBehaviour
	{
		[SerializeField]
		private float visibleAlpha = 1f;

		[SerializeField]
		private float fadeInDuration = 0.2f;

		[SerializeField]
		private float fadeOutDuration = 0.2f;

		[SerializeField]
		private Image Image;

		private CanvasGroup _selfCanvasGroup;

		private RectTransform _rect;

		private Canvas _canvas;

		private bool _visible;

		private Tween _fadeTween;

		private void Awake()
		{
			_rect = GetComponent<RectTransform>();
			_selfCanvasGroup = GetComponent<CanvasGroup>();
			_canvas = GetComponentInParent<Canvas>();
			_visible = false;
			if ((bool)_selfCanvasGroup)
			{
				_selfCanvasGroup.alpha = 0f;
			}
			SetFill(0f);
		}

		private void OnDisable()
		{
			_fadeTween?.Kill();
			_fadeTween = null;
			_visible = false;
			InputStorage.SprintLockAiming = false;
			SetFill(0f);
		}

		private void Update()
		{
			bool flag = FlagsStorage.OutOfCar && InputStorage.SprintTierActive;
			if (flag != _visible)
			{
				_visible = flag;
				Fade(flag ? visibleAlpha : 0f, flag ? fadeInDuration : fadeOutDuration);
			}
			if (!flag)
			{
				InputStorage.SprintLockAiming = false;
				SetFill(0f);
			}
			else
			{
				Camera cam = (((bool)_canvas && _canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? _canvas.worldCamera : null);
				bool flag2 = (InputStorage.SprintLockAiming = InputStorage.LeftJoyIsDrag && RectTransformUtility.RectangleContainsScreenPoint(_rect, InputStorage.LeftJoyTouchScreenPos, cam));
				SetFill(InputStorage.SprintLocked ? 1f : (flag2 ? InputStorage.SprintLockArmProgress : 0f));
			}
		}

		private void Fade(float target, float duration)
		{
			if ((bool)_selfCanvasGroup)
			{
				_fadeTween?.Kill();
				_fadeTween = _selfCanvasGroup.DOFade(target, duration).SetUpdate(isIndependentUpdate: true);
			}
		}

		private void SetFill(float amount)
		{
			if ((bool)Image && Image.type == Image.Type.Filled)
			{
				Image.fillAmount = amount;
			}
		}
	}
}
