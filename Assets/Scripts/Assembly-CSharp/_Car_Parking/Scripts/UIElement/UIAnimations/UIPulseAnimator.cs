using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.UIAnimations
{
	[RequireComponent(typeof(Image))]
	public class UIPulseAnimator : MonoBehaviour
	{
		[SerializeField]
		private RectTransform Target;

		[SerializeField]
		private Image TargetImage;

		[Tooltip("Scale multiplier at the peak of the pulse, relative to the base scale.")]
		[SerializeField]
		private float PulseScale = 1.1f;

		[SerializeField]
		[Tooltip("Duration of a single ping (base -> peak / visible -> transparent) in seconds.")]
		private float Duration = 0.5f;

		[SerializeField]
		private Ease EaseType = Ease.OutSine;

		[SerializeField]
		[Tooltip("Ignore Time.timeScale so the pulse keeps playing while the game is paused.")]
		private bool UnscaledTime = true;

		[SerializeField]
		private bool PlayOnEnable = true;

		private Vector3 _baseScale;

		private float _baseAlpha;

		private Tween _pulseTween;

		private bool _baseCached;

		private void Awake()
		{
			if (!Target)
			{
				Target = base.transform as RectTransform;
			}
			if (!TargetImage)
			{
				TargetImage = GetComponent<Image>();
			}
			CacheBaseState();
		}

		private void OnEnable()
		{
			if (PlayOnEnable)
			{
				StartPulse();
			}
		}

		private void OnDisable()
		{
			StopPulse();
		}

		private void CacheBaseState()
		{
			if (!_baseCached)
			{
				if (!Target || !TargetImage)
				{
					Debug.LogError("UIPulseAnimator: Target or Image is not assigned.");
					return;
				}
				_baseScale = Target.localScale;
				_baseAlpha = TargetImage.color.a;
				_baseCached = true;
			}
		}

		public void StartPulse()
		{
			if (!Target || !TargetImage)
			{
				Debug.LogError("UIPulseAnimator: Target or Image is not assigned.");
				return;
			}
			CacheBaseState();
			_pulseTween?.Kill();
			Target.localScale = _baseScale;
			SetImageAlpha(_baseAlpha);
			_pulseTween = DOTween.Sequence().Join(Target.DOScale(_baseScale * PulseScale, Duration).SetEase(EaseType)).Join(TargetImage.DOFade(0f, Duration).SetEase(EaseType))
				.SetLoops(-1, LoopType.Restart)
				.SetUpdate(UnscaledTime)
				.SetLink(base.gameObject);
		}

		public void StopPulse()
		{
			_pulseTween?.Kill();
			_pulseTween = null;
			if (_baseCached)
			{
				if ((bool)Target)
				{
					Target.localScale = _baseScale;
				}
				if ((bool)TargetImage)
				{
					SetImageAlpha(_baseAlpha);
				}
			}
		}

		private void SetImageAlpha(float alpha)
		{
			Color color = TargetImage.color;
			color.a = alpha;
			TargetImage.color = color;
		}
	}
}
