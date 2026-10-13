using System.Collections;
using DG.Tweening;
using UnityEngine;

public class UGUITweenRotation : UGUITweener
{
	[SerializeField]
	private Vector3 from;

	[SerializeField]
	private Vector3 to;

	private Tween _tween;

	private void OnEnable()
	{
		StartCoroutine(StartAnimation());
	}

	private IEnumerator StartAnimation()
	{
		yield return new WaitForSeconds(startDelay);
		switch (playStyle)
		{
		case PlayStyle.Once:
			_tween = base.transform.DORotate(to, duration).SetEase(animationCurve);
			break;
		case PlayStyle.Loop:
			_tween = base.transform.DORotate(to, duration).SetEase(animationCurve).SetLoops(-1, LoopType.Yoyo);
			break;
		case PlayStyle.PingPong:
			base.transform.DORotate(to, duration).SetEase(animationCurve).OnComplete(() =>
			{
				base.transform.DORotate(from, duration).SetEase(animationCurve).SetDelay(duration / 2f)
					.Play();
			});
			break;
		}
	}

	private void OnDisable()
	{
		if (_tween != null)
		{
			_tween.Kill();
		}
	}
}
