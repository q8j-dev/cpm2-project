using DG.Tweening;
using UnityEngine;

public class TweenRotator : MonoBehaviour
{
	private Tween _tweener;

	[SerializeField]
	private Transform Transform;

	[SerializeField]
	private float Duration;

	[SerializeField]
	private RotateMode RotateMode;

	private void OnEnable()
	{
		Rotate();
	}

	private void OnDisable()
	{
		_tweener?.Kill();
	}

	private void Rotate()
	{
		_tweener?.Kill();
		_tweener = Transform.DORotate(Vector3.back * 360f, Duration, RotateMode).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear)
			.SetUpdate(isIndependentUpdate: true);
	}
}
