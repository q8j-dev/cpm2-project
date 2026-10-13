using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class RectTransformMaxWidth : MonoBehaviour, IRectTransformUpdated
	{
		[SerializeField]
		private RectTransform RectTransform = default;

		[SerializeField]
		private float MaxWidth = default;

		private CancellationTokenSource _cancellationTokenSourceUpdateWidth;

		private void OnEnable()
		{
			UpdateWidth();
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceUpdateWidth);
		}

		public void OnRectTransformUpdated()
		{
			UpdateWidth();
		}

		public void UpdateWidth()
		{
			base.transform.localScale = Vector3.one;
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceUpdateWidth);
			_cancellationTokenSourceUpdateWidth = new CancellationTokenSource();
			UpdateWidthAsync(_cancellationTokenSourceUpdateWidth.Token).Forget();
		}

		private async UniTaskVoid UpdateWidthAsync(CancellationToken cancellationToken)
		{
			await UniTask.DelayFrame(2, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				base.transform.localScale = new Vector3(Mathf.Min(1f, MaxWidth / RectTransform.rect.width), 1f, 1f);
			}
		}
	}
}
