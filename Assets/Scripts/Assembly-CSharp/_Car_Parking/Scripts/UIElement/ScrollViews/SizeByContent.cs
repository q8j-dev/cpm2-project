using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class SizeByContent : MonoBehaviour
	{
		private enum Mode
		{
			Vertical = 0,
			Horizontal = 1,
			Both = 2
		}

		[SerializeField]
		private RectTransform RectTransformSelf;

		[SerializeField]
		private RectTransformEventSender ContentRectTransformEventSender;

		[SerializeField]
		private Mode ModeUpdate = default;

		private CancellationTokenSource _cancellationTokenSource;

		private Vector2 _originalSize;

		private void OnEnable()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSource);
			_cancellationTokenSource = new CancellationTokenSource();
			Init(_cancellationTokenSource.Token).Forget();
		}

		private void OnDisable()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSource);
			if ((bool)ContentRectTransformEventSender)
			{
				ContentRectTransformEventSender.OnSizeChanged -= ContentSizeChangedCallback;
			}
		}

		private void ContentSizeChangedCallback(Vector2 size)
		{
			if ((bool)RectTransformSelf)
			{
				if (ModeUpdate == Mode.Horizontal || ModeUpdate == Mode.Both)
				{
					RectTransformSelf.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Min(_originalSize.x, size.x));
				}
				if (ModeUpdate == Mode.Vertical || ModeUpdate == Mode.Both)
				{
					RectTransformSelf.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Min(_originalSize.y, size.y));
				}
			}
		}

		private async UniTaskVoid Init(CancellationToken cancellationToken)
		{
			await UniTask.DelayFrame(2, PlayerLoopTiming.Update, cancellationToken);
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			if (!RectTransformSelf)
			{
				RectTransformSelf = GetComponent<RectTransform>();
			}
			if (!ContentRectTransformEventSender)
			{
				ContentRectTransformEventSender = GetComponentInChildren<RectTransformEventSender>();
			}
			if ((bool)RectTransformSelf && (bool)ContentRectTransformEventSender)
			{
				if (_originalSize.x < 0.01f && _originalSize.y < 0.01f)
				{
					_originalSize = RectTransformSelf.rect.size;
				}
				ContentRectTransformEventSender.OnSizeChanged += ContentSizeChangedCallback;
				ContentSizeChangedCallback(ContentRectTransformEventSender.Size);
			}
		}
	}
}
