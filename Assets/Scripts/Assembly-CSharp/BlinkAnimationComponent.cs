using DG.Tweening;
using UnityEngine;

public class BlinkAnimationComponent : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup BackgroundCanvasGroup = default;

	[SerializeField]
	private RectTransform BackgroundCanvasRect = default;

	[SerializeField]
	private CanvasGroup BlinkCanvasGroup = default;

	[SerializeField]
	private CanvasGroup OutlineGameObject = default;

	[SerializeField]
	private RectTransform LineGameObject = default;

	[SerializeField]
	private Transform StartPos = default;

	[SerializeField]
	private float OffsetX = default;

	[SerializeField]
	private RectTransform ParentRectTransform = default;

	private Tween _tween;

	private void OnEnable()
	{
		StartAnimation();
	}

	private void StartAnimation()
	{
		_tween?.Kill();
		Sequence sequence = DOTween.Sequence().SetLink(base.gameObject, LinkBehaviour.KillOnDestroy);
		sequence.Append(BackgroundCanvasRect.DOSizeDelta(new Vector3(ParentRectTransform.rect.size.x * 1.5f, BackgroundCanvasRect.rect.size.y), 0.1f).SetEase(Ease.Flash));
		sequence.Append(BackgroundCanvasGroup.DOFade(0.6f, 0.08f));
		sequence.Join(OutlineGameObject.DOFade(0.7f, 0.08f));
		sequence.Append(BackgroundCanvasGroup.DOFade(0f, 0.08f));
		sequence.Join(BlinkCanvasGroup.DOFade(0f, 0.08f));
		sequence.Join(OutlineGameObject.DOFade(0f, 0.08f));
		sequence.Append(LineGameObject.DOAnchorPosX(StartPos.position.x + OffsetX, 1.5f).SetEase(Ease.Linear));
		LineGameObject.gameObject.SetActive(value: true);
		_tween = sequence;
	}

	private void OnDisable()
	{
		_tween?.Kill();
	}
}
