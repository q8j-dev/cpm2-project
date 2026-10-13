using System;
using System.Threading;
using CPM.Account;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Backend.Session;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Steam.Session
{
	public sealed class DesktopSessionWatchdog : IStartable, ITickable, IDisposable
	{
		private enum Reason
		{
			Wake = 0,
			Focus = 1
		}

		private const double SleepGapThresholdSeconds = 30.0;

		private const double FocusThrottleSeconds = 30.0;

		private const double BusyWaitSeconds = 1.0;

		private const int MaxBusyWaits = 60;

		private const string LogTag = "[SessionWatchdog]";

		private readonly IAccountController _account;

		private readonly SessionBackend _session;

		private readonly SleepGapDetector _sleep = new SleepGapDetector(30.0);

		private readonly RevalidationBackoff _backoff = new RevalidationBackoff(RevalidationBackoff.DefaultDelaysSeconds);

		private readonly CancellationTokenSource _lifetimeCts = new CancellationTokenSource();

		private bool _inFlight;

		private bool _kickInProgress;

		private double _lastOkWallSeconds = double.NegativeInfinity;

		private double _lastMonotonicSeconds;

		private static double NowWall => (double)DateTime.UtcNow.Ticks / 10000000.0;

		public DesktopSessionWatchdog(IAccountController account, SessionBackend session)
		{
			_account = account;
			_session = session;
		}

		public void Start()
		{
			Application.focusChanged += OnFocusChanged;
			_account.OnForceLogout += OnForceLogout;
		}

		public void Tick()
		{
			if (_kickInProgress && !_account.Authorized)
			{
				_kickInProgress = false;
			}
			double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
			_ = _lastMonotonicSeconds;
			_lastMonotonicSeconds = realtimeSinceStartupAsDouble;
			if (_sleep.Observe(NowWall, out var _))
			{
				Request(Reason.Wake);
			}
		}

		public void Dispose()
		{
			Application.focusChanged -= OnFocusChanged;
			_account.OnForceLogout -= OnForceLogout;
			_lifetimeCts.Cancel();
			_lifetimeCts.Dispose();
		}

		private void OnFocusChanged(bool focused)
		{
			if (focused)
			{
				Request(Reason.Focus);
			}
		}

		private void OnForceLogout()
		{
			_kickInProgress = true;
		}

		private void Request(Reason reason)
		{
			if (!_inFlight && (reason != Reason.Focus || !(NowWall - _lastOkWallSeconds < 30.0)) && CanValidate())
			{
				RunAsync(reason, _lifetimeCts.Token).Forget();
			}
		}

		private bool CanValidate()
		{
			if (_account.Authorized && !_account.IsAnonymous)
			{
				return !_kickInProgress;
			}
			return false;
		}

		private async UniTask RunAsync(Reason reason, CancellationToken ct)
		{
			_inFlight = true;
			try
			{
				int failures = 0;
				int busyWaits = 0;
				while (!ct.IsCancellationRequested && CanValidate())
				{
					if (ScenesHelper.IsLoadingSceneActive() || LoginInProgressContainer.IsLoginInProgress)
					{
						int num = busyWaits + 1;
						busyWaits = num;
						if (num > 60)
						{
							break;
						}
						await UniTask.Delay(TimeSpan.FromSeconds(1.0), DelayType.Realtime, PlayerLoopTiming.Update, ct);
						continue;
					}
					Result<bool> result = await ExtendSafeAsync(ct);
					if (result.IsSuccess && result.Value)
					{
						_lastOkWallSeconds = NowWall;
						break;
					}
					if ((result.IsSuccess && (_kickInProgress || await _account.CheckShouldLogOutOnThisDevice(CheckLoggedDeviceInFreeType.Start) || _kickInProgress)) || !_backoff.TryGetDelay(failures, out var delaySeconds))
					{
						break;
					}
					failures++;
					await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), DelayType.Realtime, PlayerLoopTiming.Update, ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				_inFlight = false;
			}
		}

		private async UniTask<Result<bool>> ExtendSafeAsync(CancellationToken ct)
		{
			try
			{
				return await _session.ExtendSessionAsync(ct);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception)
			{
				return Result<bool>.Fail();
			}
		}
	}
}
