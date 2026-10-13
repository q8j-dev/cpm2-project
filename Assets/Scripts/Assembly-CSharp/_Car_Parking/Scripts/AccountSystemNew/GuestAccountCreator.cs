using System;
using System.Threading;
using CPM.Account;
using CPM.Balance;
using CPM.Connection;
using CPM.Connection.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	[Preserve]
	public class GuestAccountCreator : IGuestAccountCreator, IInitializable, IDisposable
	{
		private readonly SemaphoreSlim _semaphoreCreateGuest;

		private readonly IAccountController _accountController;

		private readonly IGameDataRepository _gameDataRepository;

		private readonly IConnectionDataHandler _connectionDataHandler;

		private readonly IBalanceDocumentSynchronizer _balanceDocumentSynchronizer;

		private CancellationTokenSource _cts;

		private CancellationToken _mainCancellationToken;

		[Preserve]
		public GuestAccountCreator(IAccountController accountController, IGameDataRepository gameDataRepository, IConnectionDataHandler connectionDataHandler, IBalanceDocumentSynchronizer balanceDocumentSynchronizer)
		{
			_semaphoreCreateGuest = new SemaphoreSlim(1, 1);
			_accountController = accountController;
			_gameDataRepository = gameDataRepository;
			_connectionDataHandler = connectionDataHandler;
			_balanceDocumentSynchronizer = balanceDocumentSynchronizer;
			_cts = new CancellationTokenSource();
			_mainCancellationToken = _cts.Token;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cts);
			_semaphoreCreateGuest?.Dispose();
		}

		public async UniTask<bool> CreateGuestAccountIfNeeds(bool showWaitingScreen = false, bool showErrorMessage = false)
		{
			bool acquired = false;
			try
			{
				if (showWaitingScreen)
				{
					await ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false, waitFrameAfterShow: true);
				}
				await _semaphoreCreateGuest.WaitAsync(_mainCancellationToken);
				acquired = true;
				if (_mainCancellationToken.IsCancellationRequested)
				{
					return false;
				}
				if (showWaitingScreen)
				{
					await ManagerUIHelper.CloseWaitingWindow(waitFrameAfterHide: true, canUseDelay: false);
				}
				if (_accountController.HasEmail)
				{
					return true;
				}
				if (showWaitingScreen)
				{
					await ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false, waitFrameAfterShow: true);
				}
				if ((await _accountController.SignUpGuest()).result == SignUpResult.Success)
				{
					_connectionDataHandler.ProcessUserConnectionData(UserConnectionData.Default);
					await _balanceDocumentSynchronizer.EnsureFreshAsync(_mainCancellationToken);
					if (showWaitingScreen)
					{
						await ManagerUIHelper.CloseWaitingWindow(waitFrameAfterHide: true, canUseDelay: false);
					}
					if (_mainCancellationToken.IsCancellationRequested)
					{
						return false;
					}
					_gameDataRepository.ExecuteAsync(GameDataRepositoryDataType.Part).Forget();
					return true;
				}
				if (showWaitingScreen)
				{
					await ManagerUIHelper.CloseWaitingWindow(waitFrameAfterHide: true, canUseDelay: false);
				}
				if (showErrorMessage && (bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
				{
					V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f, NotificationType.Error).Forget();
				}
				return false;
			}
			finally
			{
				if (showWaitingScreen)
				{
					ManagerUIHelper.CloseWaitingWindow(waitFrameAfterHide: true, canUseDelay: false).Forget();
				}
				if (acquired)
				{
					try
					{
						_semaphoreCreateGuest.Release();
					}
					catch (ObjectDisposedException)
					{
					}
				}
			}
		}
	}
}
