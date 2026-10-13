using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ProgressbarImage : MonoBehaviour
	{
		[SerializeField]
		private Image Progressbar;

		private Tween _fillTween;

		private UnityAction _onCompleteCallback;

		private void OnDestroy()
		{
			Stop();
		}

		public void StartProgress(float startValue, float endValue, float duration, UnityAction onCompleteCallback = null)
		{
			_fillTween?.Kill();
			Progressbar.fillAmount = startValue;
			_onCompleteCallback = onCompleteCallback;
			_fillTween = Progressbar.DOFillAmount(endValue, duration).OnComplete(OnProgressCompleted);
		}

		public void SetFillAmount(float value)
		{
			Progressbar.fillAmount = value;
		}

		public void Pause()
		{
			_fillTween?.Pause();
		}

		public void Continue()
		{
			_fillTween?.Play();
		}

		public void Stop()
		{
			_fillTween?.Kill();
			SetFillAmount(0f);
		}

		private void OnProgressCompleted()
		{
			_onCompleteCallback?.Invoke();
		}
	}
}
