using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CircleProgressBarItem : MonoBehaviour
{
	[SerializeField]
	private Image ProgressBarImage;

	private float _duration;

	private Tween _tween;

	private bool _isDestroyed;

	public void Init(float duration)
	{
		_tween?.Kill();
		_tween = ProgressBarImage.DOFillAmount(1f, duration).OnComplete(Close);
	}

	private void OnDisable()
	{
		_tween?.Kill();
	}

	public void Close()
	{
		_tween?.Kill();
		if (!_isDestroyed)
		{
			_isDestroyed = true;
			Object.Destroy(base.gameObject);
		}
	}
}
