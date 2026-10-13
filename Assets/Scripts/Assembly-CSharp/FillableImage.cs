using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FillableImage : MonoBehaviour
{
	[SerializeField]
	private Image FillImage = default;

	[SerializeField]
	private float Duration = default;

	[SerializeField]
	private float StartDelay = default;

	[SerializeField]
	private float EndDelay = default;

	[SerializeField]
	private float StartValue = default;

	[SerializeField]
	private float EndValue = default;

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
