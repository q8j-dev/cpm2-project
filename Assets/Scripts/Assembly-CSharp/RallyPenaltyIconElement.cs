using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using _Car_Parking.Scripts.Racing_Scripts;

public class RallyPenaltyIconElement : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI PenaltyText;

	[SerializeField]
	private float DisplayDuration = 2.5f;

	private float _accumulatedPenalty;

	private CancellationTokenSource _hideCts;

	public PenaltyType PenaltyType;

	public void SetPenalty(float time)
	{
		_accumulatedPenalty += time;
		SetText(_accumulatedPenalty);
		base.gameObject.SetActive(value: true);
		_hideCts?.Cancel();
		_hideCts?.Dispose();
		_hideCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
		HideAfterDelay(_hideCts.Token).Forget();
	}

	public void SetText(float time)
	{
		if (PenaltyText != null)
		{
			PenaltyText.text = $"+{time:0}s";
		}
	}

	private async UniTaskVoid HideAfterDelay(CancellationToken token)
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(DisplayDuration), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			_accumulatedPenalty = 0f;
			base.gameObject.SetActive(value: false);
		}
		catch (OperationCanceledException)
		{
		}
	}

	private void OnDestroy()
	{
		_hideCts?.Cancel();
		_hideCts?.Dispose();
	}
}
