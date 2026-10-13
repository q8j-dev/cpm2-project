using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.WindowSystem
{
	[Preserve]
	public class ScreenFader : MonoBehaviour
	{
		[SerializeField]
		private CanvasGroup FaderCanvasGroup;

		private float _leftTime;

		private readonly float _autoFadeOutTime = 5f;

		private readonly float _fadeTime = 0.1f;

		private UniTask _fadeOutTask;

		private Tweener _lastTweener;

		private float _currentFadeTime;

		private void Awake()
		{
			FaderCanvasGroup.alpha = 0f;
		}

		public void FadeOut(float autoFadeInDelay = 0f, float? fadeDuration = null)
		{
		}

		public void FadeIn()
		{
		}

		private async UniTask FadeInAsync()
		{
			_lastTweener?.Kill();
			if (!(FaderCanvasGroup == null))
			{
				if (_leftTime < _currentFadeTime || _currentFadeTime == 0f)
				{
					FaderCanvasGroup.alpha = 1f;
				}
				else
				{
					_lastTweener = FaderCanvasGroup.DOFade(1f, _currentFadeTime).SetLink(FaderCanvasGroup.gameObject, LinkBehaviour.KillOnDestroy);
				}
				while (_leftTime > 0f)
				{
					_leftTime -= Time.deltaTime;
					await UniTask.Yield();
				}
				_lastTweener?.Kill();
				_lastTweener = FaderCanvasGroup.DOFade(0f, _fadeTime).SetLink(FaderCanvasGroup.gameObject, LinkBehaviour.KillOnDestroy);
			}
		}

		private void OnDestroy()
		{
			_lastTweener?.Kill();
			if (FaderCanvasGroup != null)
			{
				FaderCanvasGroup.DOKill();
			}
		}
	}
}
