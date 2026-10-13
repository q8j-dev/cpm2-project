using DG.Tweening;
using UnityEngine;

public class SideMenuArrowTweener : MonoBehaviour
{
	[SerializeField]
	private Vector3 startPos;

	[SerializeField]
	private Vector3 endPos;

	[SerializeField]
	private float duration;

	private bool state;

	private Tween _tween;

	public void ChangeState()
	{
		state = !state;
		_tween = base.transform.DORotate(state ? endPos : startPos, duration);
	}

	private void OnDisable()
	{
		_tween?.Kill();
		ResetTween();
	}

	public void ResetTween()
	{
		state = false;
		base.transform.rotation = Quaternion.Euler(startPos);
	}
}
