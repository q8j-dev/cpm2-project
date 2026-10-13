using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.CPMRating.PopupNotifications;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.WindowSystem;

namespace CPM.Rating
{
	public class AchievementPopupPresenter : IInitializable, IDisposable
	{
		private const int InitialDrainDelayFrames = 100;

		private readonly ManagerUI _managerUI;

		private readonly AchievementPopupChannel _channel;

		private readonly CancellationTokenSource _cts = new CancellationTokenSource();

		private bool _isShowing;

		[Inject]
		public AchievementPopupPresenter(ManagerUI managerUI, AchievementPopupChannel channel)
		{
			_managerUI = managerUI;
			_channel = channel;
		}

		public void Initialize()
		{
			_channel.OnEnqueued += TryShowNext;
			DrainAfterWarmup(_cts.Token).Forget();
		}

		public void Dispose()
		{
			_channel.OnEnqueued -= TryShowNext;
			_cts.Cancel();
			_cts.Dispose();
		}

		private async UniTaskVoid DrainAfterWarmup(CancellationToken token)
		{
			try
			{
				await UniTask.DelayFrame(100, PlayerLoopTiming.Update, token);
				TryShowNext();
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void TryShowNext()
		{
			if (!_cts.IsCancellationRequested && !_isShowing && !(_managerUI == null) && _channel.TryPeek(out var data))
			{
				_isShowing = true;
				ShowAchievementNotificationPopup(data, _cts.Token).Forget();
			}
		}

		private async UniTaskVoid ShowAchievementNotificationPopup(PopupNotificationData data, CancellationToken token)
		{
			try
			{
				PopupNotificationWindow popupNotificationWindow = await _managerUI.ShowPopupNotificationWindow().AttachExternalCancellation(token);
				if (!token.IsCancellationRequested && !(_managerUI == null))
				{
					data.InitPrefab(popupNotificationWindow);
					popupNotificationWindow.OnDestroyAsObservable().Subscribe((Unit _) =>
					{
						ShowNotificationEnded();
					}).AddTo(_managerUI);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
				_channel.Dequeue();
				_isShowing = false;
			}
		}

		private void ShowNotificationEnded()
		{
			_channel.Dequeue();
			_isShowing = false;
			TryShowNext();
		}
	}
}
