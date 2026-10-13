using DG.Tweening;
using UnityEngine;

public class TweenMover : MonoBehaviour
{
	private Vector3 _startPosition;

	public Transform StartPositionTransform;

	public Transform EndPositionTransform;

	public float duration;

	public float delay;

	public AnimationCurve AnimationCurve;

	private Tween _tween;

	[SerializeField]
	private bool ActivateOnEnable;

	private bool _state;

	private void Start()
	{
		if (!StartPositionTransform)
		{
			_startPosition = base.transform.position;
		}
	}

	private void OnEnable()
	{
		if (ActivateOnEnable)
		{
			_tween = base.transform.DOMove(EndPositionTransform.position, duration).SetEase(AnimationCurve).SetDelay(delay);
		}
	}

	public void ChangeState(bool open)
	{
		if ((bool)StartPositionTransform & open)
		{
			_startPosition = StartPositionTransform.transform.position;
		}
		_tween = base.transform.DOMove(open ? EndPositionTransform.position : _startPosition, duration).SetEase(AnimationCurve).SetDelay(delay);
		_state = open;
	}

	public void ChangeState()
	{
		_state = !_state;
		ChangeState(_state);
	}

	private void OnDisable()
	{
		ResetTween();
	}

	public void ResetTween()
	{
		_tween.Kill();
		_state = false;
		base.transform.position = (StartPositionTransform ? StartPositionTransform.position : _startPosition);
	}
}
