using DG.Tweening;
using UnityEngine;

public class TweenTest : MonoBehaviour
{
	private void Start()
	{
		base.transform.DOMoveX(base.transform.position.x + 10f, 2f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo)
			.SetUpdate(UpdateType.Normal);
		base.transform.DORotate(new Vector3(0f, 360f, 0f), 6f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental)
			.SetUpdate(UpdateType.Normal);
	}

	private void Update()
	{
	}
}
