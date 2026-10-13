using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderFillTweener : MonoBehaviour
{
	[SerializeField]
	private Slider Slider;

	[SerializeField]
	private float Duration;

	[SerializeField]
	private UnityEvent OnFinishEvent;

	[SerializeField]
	private UnityEvent OnDisableEvent;

	private Tween _tween;

	private void OnEnable()
	{
		Animate();
	}

	private void Animate()
	{
		_tween?.Kill();
		_tween = Slider.DOValue(1f, Duration).OnComplete(() =>
		{
			OnFinishEvent?.Invoke();
		});
	}

	private void OnDisable()
	{
		Slider.value = 0f;
		OnDisableEvent?.Invoke();
	}
}
