using DG.Tweening;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public abstract class LampLight : MonoBehaviour
	{
		[SerializeField]
		private bool ForceEnableDisable;

		private Tween _tweenIntensity;

		private Sequence _enableDisableAnimation;

		public abstract float Intensity { get; protected set; }

		public void EnableDisableAnimation(float timeToEnable, float duration, float timeToDisable, TweenCallback onComplete = null)
		{
			_enableDisableAnimation?.Kill();
			Disable(-1f);
			_enableDisableAnimation = DOTween.Sequence();
			_enableDisableAnimation.AppendCallback(() =>
			{
				Enable(timeToEnable);
			});
			_enableDisableAnimation.AppendInterval(timeToEnable + duration);
			_enableDisableAnimation.AppendCallback(() =>
			{
				Disable(timeToDisable, onComplete);
			});
		}

		public void Enable(float time, TweenCallback onComplete = null)
		{
			UpdateIntensity(1f, time, onComplete);
		}

		public void Disable(float time, TweenCallback onComplete = null)
		{
			UpdateIntensity(0f, time, onComplete);
		}

		public void UpdateIntensity(float intensity, float time, TweenCallback completionCallback = null)
		{
			if (ForceEnableDisable)
			{
				base.gameObject.SetActive(value: true);
			}
			_tweenIntensity?.Kill();
			if (time > 0f)
			{
				_tweenIntensity = DOTween.To(() => Intensity, (float x) =>
				{
					Intensity = x;
				}, intensity, time);
				_tweenIntensity.OnComplete(() =>
				{
					OnComplete(completionCallback);
				});
			}
			else
			{
				Intensity = intensity;
				OnComplete(completionCallback);
			}
		}

		private void OnComplete(TweenCallback onComplete)
		{
			if (ForceEnableDisable && Intensity < 0.001f)
			{
				base.gameObject.SetActive(value: false);
			}
			onComplete?.Invoke();
		}

		protected virtual void OnDestroy()
		{
			_tweenIntensity?.Kill();
			_enableDisableAnimation?.Kill();
		}
	}
}
