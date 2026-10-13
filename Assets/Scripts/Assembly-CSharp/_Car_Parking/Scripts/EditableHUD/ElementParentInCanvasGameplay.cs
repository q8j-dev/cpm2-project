using System.Threading;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class ElementParentInCanvasGameplay : ElementParentInCanvasBase
	{
		private CancellationTokenSource _cancellationTokenUpdateVisibility;

		public override bool IsVisible
		{
			get
			{
				if ((bool)element && element.IsVisible)
				{
					return element.IsActive;
				}
				return false;
			}
		}

		public void InitParent(EditableHUDElement elementValue, FakeElement fakeElementPrefab, EditableHUDConfig config)
		{
			Init(elementValue, fakeElementPrefab, config);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			_cancellationTokenUpdateVisibility?.Cancel();
			_cancellationTokenUpdateVisibility?.Dispose();
		}

		protected override void OnHUDChangesUpdated(EditableHUDElement element)
		{
			base.OnHUDChangesUpdated(element);
			UpdateVisibility();
		}

		protected override void OnIsActiveChanged(EditableHUDElement element)
		{
			base.OnIsActiveChanged(element);
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			_cancellationTokenUpdateVisibility?.Cancel();
			_cancellationTokenUpdateVisibility?.Dispose();
			_cancellationTokenUpdateVisibility = new CancellationTokenSource();
			UpdateVisibilityAsync(_cancellationTokenUpdateVisibility.Token).Forget();
		}

		private async UniTaskVoid UpdateVisibilityAsync(CancellationToken cancellationToken)
		{
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested && (bool)base.gameObject && (bool)element)
			{
				VisibleChanged(IsVisible);
			}
		}
	}
}
