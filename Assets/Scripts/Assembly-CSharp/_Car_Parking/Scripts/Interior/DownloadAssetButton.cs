using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Interior
{
	public class DownloadAssetButton : MonoBehaviour
	{
		private float _initialSizeAssetMb;

		private Tween _tweenFirstPart = default;

		private Tween _tweenSecondPart = default;

		private bool _startContinue;

		[field: SerializeField]
		public Button Button { get; private set; }

		[field: SerializeField]
		public TMP_Text ProgressText { get; private set; }

		public CancellationTokenSource CancellationTokenSource { get; private set; } = new CancellationTokenSource();

		public event Action OnDownloadCompleted;

		public void Initialize(float initialSizeAssetMb)
		{
			_initialSizeAssetMb = initialSizeAssetMb;
			ProgressText.text = $"{_initialSizeAssetMb:F1} Mb";
		}

		public async UniTask StartDownload(CancellationToken token)
		{
			float targetMb = _initialSizeAssetMb * UnityEngine.Random.Range(0.4f, 0.7f);
			await PlayFloatTween(0f, targetMb, 2f, _tweenFirstPart, token);
			await UniTask.WaitUntil(() => _startContinue, PlayerLoopTiming.Update, token);
			await PlayFloatTween(targetMb, _initialSizeAssetMb, 1f, _tweenSecondPart, token);
			if (!token.IsCancellationRequested)
			{
				OnDownloadCompleted?.Invoke();
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public async UniTask PlayFloatTween(float startValue, float endValue, float duration, Tween part, CancellationToken token)
		{
			if (!token.IsCancellationRequested)
			{
				part?.Kill();
				part = DOTween.To(() => startValue, (float x) =>
				{
					startValue = x;
				}, endValue, duration).SetEase(Ease.Linear).OnUpdate(() =>
				{
					ProgressText.text = $"{_initialSizeAssetMb}({startValue:F1}) Mb";
				})
					.OnComplete(() =>
					{
						ProgressText.text = $"{_initialSizeAssetMb}({endValue:F1}) Mb";
					});
				await UniTask.Delay(TimeSpan.FromSeconds(duration), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
		}

		public void StopDownload()
		{
			_tweenFirstPart?.Kill();
			_tweenSecondPart?.Kill();
			CancellationTokenSource?.Cancel();
			CancellationTokenSource?.Dispose();
			CancellationTokenSource = null;
			CancellationTokenSource = new CancellationTokenSource();
			_startContinue = false;
			ProgressText.text = $"{_initialSizeAssetMb:F1} Mb";
		}

		public void ContinueDownload()
		{
			_startContinue = true;
		}

		private void OnDestroy()
		{
			StopDownload();
			OnDownloadCompleted = null;
			Button.onClick.RemoveAllListeners();
		}
	}
}
