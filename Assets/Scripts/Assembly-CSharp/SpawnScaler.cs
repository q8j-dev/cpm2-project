using DG.Tweening;
using UnityEngine;

public class SpawnScaler : MonoBehaviour
{
	[SerializeField]
	private float EndScale;

	[SerializeField]
	private float Duration;

	[SerializeField]
	private float Delay;

	private Tween _spawnTween;

	private void OnEnable()
	{
		_spawnTween = base.transform.DOScale(EndScale, Duration).SetEase(Ease.InOutBack);
	}

	private void OnDestroy()
	{
		_spawnTween?.Kill();
	}

	private void OnDisable()
	{
		base.transform.localScale = Vector3.zero;
	}
}
