using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TMPRevealAnimation : MonoBehaviour
{
	public float StartDelay;

	public float AppearDuration = 0.7f;

	public float StayDuration = 1f;

	public float FinishDuration = 1f;

	public float DisappearDuration = 0.7f;

	public float RightSlideOffset = 400f;

	public float LeftSlideOffset = 400f;

	public float EndScale = 2f;

	public CanvasGroup CanvasGroup;

	public RectTransform Rect;

	public TMP_Text TmpText;

	private Sequence _sequence;

	private Tween _showTween;

	public UnityEvent OnBackAnimationStart;

	private Action _onAnimationEnd;

	private float FullAnimationDuration;

	private void Start()
	{
		FullAnimationDuration = StartDelay + AppearDuration + StayDuration + FinishDuration + DisappearDuration;
	}

	private void OnEnable()
	{
		Rect.localScale = new Vector3(0f, 1f, 1f);
		Rect.pivot = new Vector2(0f, 0.5f);
		TmpText.alpha = 0f;
		ShowBackground(show: true, PlayAnimation);
	}

	public void Init(string text, Action onAnimationEnd)
	{
		_onAnimationEnd = onAnimationEnd;
		TmpText.text = text;
		base.gameObject.SetActive(value: true);
	}

	private void ShowBackground(bool show, Action callback)
	{
		_showTween?.Kill();
		_showTween = CanvasGroup.DOFade(show ? 1 : 0, 0.8f).SetLink(base.gameObject, LinkBehaviour.KillOnDestroy).OnComplete(() =>
		{
			callback?.Invoke();
		});
	}

	public void PlayAnimation()
	{
		_sequence?.Kill();
		_sequence = DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
		Vector2 rightPos = new Vector2(RightSlideOffset, 0f);
		Vector2 zero = Vector2.zero;
		Vector2 endValue = new Vector2(LeftSlideOffset, 0f);
		Rect.anchoredPosition = rightPos;
		_sequence.AppendInterval(StartDelay);
		_sequence.AppendCallback(() =>
		{
			Rect.pivot = new Vector2(0f, 0.5f);
		});
		_sequence.Join(Rect.DOAnchorPos(zero, AppearDuration));
		_sequence.Join(Rect.DOScaleX(EndScale, AppearDuration * 0.7f).From(0f));
		_sequence.Join(TmpText.DOFade(1f, AppearDuration * 0.8f));
		_sequence.Append(Rect.DOScaleX(1f, 0.25f));
		_sequence.AppendInterval(StayDuration);
		_sequence.AppendCallback(() =>
		{
			OnBackAnimationStart?.Invoke();
			Rect.pivot = new Vector2(0f, 0.5f);
		});
		_sequence.Join(Rect.DOAnchorPos(endValue, DisappearDuration));
		_sequence.Join(Rect.DOScaleX(EndScale, DisappearDuration * 0.7f).From(1f));
		_sequence.Append(Rect.DOScaleX(1f, 0.25f));
		_sequence.AppendInterval(FinishDuration);
		_sequence.AppendCallback(() =>
		{
			Rect.anchoredPosition = rightPos;
			Rect.localScale = new Vector3(0f, 1f, 1f);
			Rect.pivot = new Vector2(0f, 0.5f);
			TmpText.alpha = 0f;
			ShowBackground(show: false, () =>
			{
				base.gameObject.SetActive(value: false);
				_onAnimationEnd?.Invoke();
				_onAnimationEnd = null;
			});
		});
	}

	private void OnDisable()
	{
		_sequence?.Kill();
		_showTween?.Kill();
	}
}
