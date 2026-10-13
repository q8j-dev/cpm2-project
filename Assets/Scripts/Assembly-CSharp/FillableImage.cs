using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FillableImage : MonoBehaviour
{
	[SerializeField]
	private Image FillImage;

	[SerializeField]
	private float Duration;

	[SerializeField]
	private float StartDelay;

	[SerializeField]
	private float EndDelay;

	[SerializeField]
	private float StartValue;

	[SerializeField]
	private float EndValue;

	private Tween _tween;

	public void StartFilling()
	{
		_tween?.Kill();
		_tween = FillImage.DOFillAmount(EndValue, Duration).SetDelay(StartDelay);
	}

	public void EndFilling()
	{
		_tween?.Kill();
		_tween = FillImage.DOFillAmount(StartValue, Duration).SetDelay(EndDelay);
	}

	private void OnDestroy()
	{
		Reset();
	}

	private void Reset()
	{
		_tween?.Kill();
		FillImage.fillAmount = StartValue;
	}
}
