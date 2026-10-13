using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TweenPulseAnimator : MonoBehaviour
{
	private Tween _tween;

	[SerializeField]
	private float duration;

	[SerializeField]
	private float delay;

	[SerializeField]
	private float reloadDelay;

	[SerializeField]
	private float startValue;

	[SerializeField]
	private float endValue;

	[SerializeField]
	private bool UseFade;

	[SerializeField]
	private CanvasGroup CanvasGroup;

	[SerializeField]
	private Texture2D[] textures;

	[SerializeField]
	private RawImage Image;

	private WarningState _currentWarningState;

	public bool IgnoreAnimation;

	public void ChangeState(WarningState warningState)
	{
		bool flag = warningState == WarningState.None;
		base.gameObject.SetActive(!flag);
		switch (warningState)
		{
		case WarningState.Warning:
			Image.texture = textures[0];
			break;
		case WarningState.Error:
			Image.texture = textures[1];
			break;
		}
		if (!flag && _currentWarningState != warningState)
		{
			Init();
		}
	}

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		if (!CanvasGroup)
		{
			return;
		}
		CanvasGroup.alpha = 1f;
		if (!IgnoreAnimation)
		{
			_tween?.Kill();
			if (UseFade)
			{
				_tween = DOTween.Sequence().Append(CanvasGroup.DOFade(endValue, duration).SetDelay(delay)).Append(CanvasGroup.DOFade(startValue, reloadDelay))
					.SetLoops(-1, LoopType.Restart)
					.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			}
			else
			{
				_tween = DOTween.Sequence().Append(base.transform.DOScale(endValue, duration).SetDelay(delay)).Append(base.transform.DOScale(startValue, reloadDelay))
					.SetLoops(-1, LoopType.Restart)
					.SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
			}
		}
	}

	private void OnDisable()
	{
		_tween?.Kill();
	}
}
