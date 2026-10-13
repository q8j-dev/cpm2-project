using System;
using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public abstract class Fader : MonoBehaviour
	{
		private Tween _tween;

		public abstract float Alpha { get; set; }

		private void OnDisable()
		{
			_tween?.Kill();
		}

		public void FadeInFromZero(float duration)
		{
			float alpha = Alpha;
			Fade(0f, -1f);
			Fade(alpha, duration);
		}

		public void Fade(float value, float duration, Action onComplete = null)
		{
			_tween?.Kill();
			if (duration > 0f)
			{
				_tween = DOTween.To(() => Alpha, (float x) =>
				{
					Alpha = x;
				}, value, duration).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
				{
					onComplete?.Invoke();
				});
			}
			else
			{
				Alpha = value;
				onComplete?.Invoke();
			}
		}
	}
}
