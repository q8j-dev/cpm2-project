using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class TweenerCanvas : MonoBehaviour
{
	[SerializeField]
	public CanvasGroup _canvasGroup;

	[SerializeField]
	private float duration;

	[SerializeField]
	private float startVal;

	[SerializeField]
	private float endVal;

	[SerializeField]
	private float delay;

	[SerializeField]
	private bool ignoreEnable;

	public UnityEvent onComplete;

	private Tween _tween;

	private Sequence _sequence;

	private bool _currentState;

	private void OnEnable()
	{
		if (ignoreEnable)
		{
			return;
		}
		_tween?.Kill();
		if ((bool)_canvasGroup)
		{
			_tween = _canvasGroup.DOFade(endVal, endVal).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
			{
				onComplete?.Invoke();
			});
		}
	}

	private void OnDisable()
	{
		if (!ignoreEnable)
		{
			_tween?.Kill();
			if ((bool)_canvasGroup)
			{
				_canvasGroup.alpha = 0f;
			}
		}
	}

	public void Show()
	{
		_sequence?.Kill();
		_tween?.Kill();
		_tween = _canvasGroup.DOFade(endVal, endVal).SetUpdate(isIndependentUpdate: true).SetDelay(delay);
	}

	public void Hide()
	{
		_tween?.Kill();
		_tween = _canvasGroup.DOFade(startVal, endVal).SetUpdate(isIndependentUpdate: true).SetDelay(delay);
	}

	public void ChangeState()
	{
		_currentState = !_currentState;
		if (_currentState)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public void HideAndShow(float endValue, float startDuration, float endDuration, float startDelay, float endDelay)
	{
		_sequence?.Kill();
		_tween?.Kill();
		_sequence = DOTween.Sequence();
		_sequence.Append(_canvasGroup.DOFade(0f, startDuration).SetUpdate(isIndependentUpdate: true).SetDelay(startDelay));
		_sequence.Append(_canvasGroup.DOFade(endValue, startDuration).SetUpdate(isIndependentUpdate: true).SetDelay(endDelay));
		_sequence.Play();
	}
}
