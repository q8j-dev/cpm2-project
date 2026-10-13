using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Other
{
	public class RectTransformSizeSetter : MonoBehaviour
	{
		[SerializeField]
		private RectTransform RectTransform = default;

		private CancellationTokenSource _cancellationTokenSourceSetSize;

		public void SetSizeZero(float duration, Action<RectTransformSizeSetter> onComplete = null)
		{
			SetSize(Vector2.zero, duration, onComplete);
		}

		public void SetSize(Vector2 size, float duration, Action<RectTransformSizeSetter> onComplete = null)
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceSetSize);
			_cancellationTokenSourceSetSize = new CancellationTokenSource();
			SetSizeAsync(size, duration, _cancellationTokenSourceSetSize.Token, onComplete).Forget();
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceSetSize);
		}

		private async UniTaskVoid SetSizeAsync(Vector2 size, float duration, CancellationToken cancellationToken, Action<RectTransformSizeSetter> onComplete)
		{
			float timeNow = Time.unscaledDeltaTime;
			Vector2 startSize = RectTransform.rect.size;
			for (; timeNow < duration; timeNow += Time.unscaledDeltaTime)
			{
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(startSize.x, size.x, timeNow / duration));
				RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(startSize.y, size.y, timeNow / duration));
				await UniTask.Yield();
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			onComplete?.Invoke(this);
		}
	}
}
