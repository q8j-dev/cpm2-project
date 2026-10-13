using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.TweenAnimations
{
	public class TweenAnimationAnchoredPosition : TweenAnimationBase
	{
		public RectTransform RectTransform;

		public Vector2 EndValue;

		private Vector2 _defaultAnchoredPosition;

		protected override void ApplyDefault()
		{
			if (RectTransform != null)
			{
				RectTransform.anchoredPosition = _defaultAnchoredPosition;
			}
		}

		protected override void InitDefaultData()
		{
			if (RectTransform != null)
			{
				_defaultAnchoredPosition = RectTransform.anchoredPosition;
			}
		}

		protected override void CreateTween()
		{
			if (RectTransform != null)
			{
				tween = DOTween.To(() => RectTransform.anchoredPosition, (Vector2 x) =>
				{
					RectTransform.anchoredPosition = x;
				}, EndValue, Duration);
			}
		}
	}
}
