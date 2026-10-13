using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement
{
	public class CrossedLine : MonoBehaviour
	{
		[SerializeField]
		private RectTransform RectTransformParent;

		[SerializeField]
		private RectTransform RectTransformSelf;

		private CancellationTokenSource _cancellationTokenSource;

		public void UpdateLine()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSource);
			_cancellationTokenSource = new CancellationTokenSource();
			UpdateLineAsync(_cancellationTokenSource.Token).Forget();
		}

		private void Awake()
		{
			if (!RectTransformParent)
			{
				RectTransformParent = base.transform.parent.GetComponent<RectTransform>();
			}
			if (!RectTransformSelf)
			{
				RectTransformSelf = GetComponent<RectTransform>();
			}
		}

		private void OnEnable()
		{
			UpdateLine();
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSource);
		}

		private async UniTaskVoid UpdateLineAsync(CancellationToken cancellationToken)
		{
			for (int i = 0; i < 5; i++)
			{
				if ((bool)RectTransformParent && (bool)RectTransformSelf && RectTransformParent.rect.width > 0f && RectTransformParent.rect.height > 0f)
				{
					float num = 0f - (float)Math.Atan2(RectTransformParent.rect.width, RectTransformParent.rect.height);
					num *= 57.29578f;
					base.transform.localEulerAngles = new Vector3(0f, 0f, num);
					RectTransformSelf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)Math.Sqrt(RectTransformParent.rect.width * RectTransformParent.rect.width + RectTransformParent.rect.height * RectTransformParent.rect.height));
				}
				await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
			}
		}
	}
}
