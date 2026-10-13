using DG.Tweening;
using UnityEngine;

public class TweenColor : MonoBehaviour
{
	[SerializeField]
	private Color colorFrom = Color.white;

	[SerializeField]
	private Color colorTo = Color.white;

	private Material mMat;

	[SerializeField]
	private float duration = default;

	private Tween _tween;

	private void Start()
	{
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			mMat = component.material;
		}
		StartColorTween();
	}

	private void StartColorTween()
	{
		if ((bool)mMat)
		{
			mMat.color = colorFrom;
			_tween = mMat.DOColor(colorTo, duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(isIndependentUpdate: true);
		}
	}

	private void OnDestroy()
	{
		_tween?.Kill();
	}
}
