using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other.TweenAnimations
{
	public class TweenAnimationScale : TweenAnimationBase
	{
		public Vector3 EndValue;

		private Vector3 _defaultScale;

		protected override void ApplyDefault()
		{
			base.transform.localScale = _defaultScale;
		}

		protected override void InitDefaultData()
		{
			_defaultScale = base.transform.localScale;
		}

		protected override void CreateTween()
		{
			tween = base.transform.DOScale(EndValue, Duration);
		}
	}
}
