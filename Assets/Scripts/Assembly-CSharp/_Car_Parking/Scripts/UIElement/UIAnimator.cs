using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public static class UIAnimator
	{
		public static Tween SetActiveWithPopFade(GameObject uiElement, bool show, float duration = 0.25f, Ease ease = Ease.OutBack)
		{
			if (!uiElement || uiElement.activeSelf == show)
			{
				return null;
			}
			if (!uiElement.TryGetComponent<CanvasGroup>(out var component))
			{
				component = uiElement.AddComponent<CanvasGroup>();
			}
			Transform transform = uiElement.transform;
			if (show)
			{
				uiElement.SetActive(value: true);
				component.alpha = 0f;
				transform.localScale = Vector3.zero;
				return DOTween.Sequence().Join(component.DOFade(1f, duration)).Join(transform.DOScale(1f, duration).SetEase(ease));
			}
			return DOTween.Sequence().Join(component.DOFade(0f, duration)).Join(transform.DOScale(0f, duration).SetEase(Ease.InBack))
				.OnComplete(() =>
				{
					uiElement.SetActive(value: false);
				});
		}
	}
}
