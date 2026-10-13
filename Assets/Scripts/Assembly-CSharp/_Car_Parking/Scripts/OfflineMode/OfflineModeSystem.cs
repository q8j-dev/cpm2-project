using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.OfflineMode
{
	[Preserve]
	public class OfflineModeSystem : IOfflineModeSystem, IInitializable, IDisposable
	{
		private enum Status
		{
			NeedSet = 0,
			Enabled = 1,
			Disabled = 2
		}

		private const float DelayToCheckInternet = 5f;

		private const float DelayToRestartGame = 5f;

		private Status _status;

		private CancellationTokenSource _cancellationTokenSourceCheckDisableStart;

		public bool Enabled
		{
			get
			{
				if (_status == Status.NeedSet)
				{
					_status = ((!InternetHelper.HasInternetConnection()) ? Status.Enabled : Status.Disabled);
				}
				return _status == Status.Enabled;
			}
		}

		public bool Disabled => !Enabled;

		public void Initialize()
		{
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckDisableStart);
		}

		public void CheckDisableStart()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckDisableStart);
			if (Enabled)
			{
				_cancellationTokenSourceCheckDisableStart = new CancellationTokenSource();
				CheckDisableStartAsync(_cancellationTokenSourceCheckDisableStart.Token).Forget();
			}
		}

		public void CheckDisableFinish()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckDisableStart);
		}

		private async UniTask CheckDisableStartAsync(CancellationToken cancellationToken)
		{
			while (!InternetHelper.HasInternetConnection())
			{
				await UniTask.Delay(TimeSpan.FromSeconds(5.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
			{
				V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("OfflineMode.InternetEnabled"), 4.5f).Forget();
				V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true).Forget();
			}
			await UniTask.Delay(TimeSpan.FromSeconds(5.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				_status = Status.NeedSet;
				SceneManager.LoadScene(0);
			}
		}
	}
}
