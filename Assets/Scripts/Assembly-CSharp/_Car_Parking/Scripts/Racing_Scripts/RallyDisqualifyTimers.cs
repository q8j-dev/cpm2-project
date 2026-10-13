using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyDisqualifyTimers : IDisposable
	{
		private const float IdleDisqualifyTime = 60f;

		private const float StageCountdownTime = 30f;

		private const float IdleWarningTime = 20f;

		private const float FinishCountdownTime = 60f;

		private readonly Action _onIdleDisqualify;

		private readonly Action _onIdleWarning;

		private readonly Action _onStageTimeout;

		private CancellationTokenSource _idleCheckCts;

		private CancellationTokenSource _stageCountdownCts;

		private CancellationTokenSource _finishCountdownCts;

		public RallyDisqualifyTimers(Action onIdleDisqualify, Action onStageTimeout, Action onIdleWarning = null)
		{
			_onIdleDisqualify = onIdleDisqualify;
			_onStageTimeout = onStageTimeout;
			_onIdleWarning = onIdleWarning;
		}

		public void StartIdleCheck(Rigidbody rb)
		{
			StopIdleCheck();
			_idleCheckCts = new CancellationTokenSource();
			CheckIdleDisqualify(_idleCheckCts.Token, rb).Forget();
		}

		public void StopIdleCheck()
		{
			_idleCheckCts?.Cancel();
			_idleCheckCts?.Dispose();
			_idleCheckCts = null;
		}

		public void StartStageCountdown()
		{
			StopStageCountdown();
			_stageCountdownCts = new CancellationTokenSource();
			RunStageCountdown(_stageCountdownCts.Token).Forget();
		}

		public void StopStageCountdown()
		{
			_stageCountdownCts?.Cancel();
			_stageCountdownCts?.Dispose();
			_stageCountdownCts = null;
		}

		private async UniTaskVoid RunStageCountdown(CancellationToken ct)
		{
			if (!(await UniTask.Delay(TimeSpan.FromSeconds(30.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow()))
			{
				_onStageTimeout?.Invoke();
			}
		}

		public void StartFinishCountdown(Action onTimeout)
		{
			StopFinishCountdown();
			_finishCountdownCts = new CancellationTokenSource();
			RunFinishCountdown(_finishCountdownCts.Token, onTimeout).Forget();
		}

		public void StopFinishCountdown()
		{
			_finishCountdownCts?.Cancel();
			_finishCountdownCts?.Dispose();
			_finishCountdownCts = null;
		}

		private async UniTaskVoid RunFinishCountdown(CancellationToken ct, Action onTimeout)
		{
			if (!(await UniTask.Delay(TimeSpan.FromSeconds(60.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow()))
			{
				onTimeout?.Invoke();
			}
		}

		private async UniTaskVoid CheckIdleDisqualify(CancellationToken ct, Rigidbody rb)
		{
			float idleTime = 0f;
			bool warningSent = false;
			while (!ct.IsCancellationRequested && !(await UniTask.Delay(TimeSpan.FromMilliseconds(200.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct).SuppressCancellationThrow()) && !(rb == null))
			{
				if (rb.velocity.magnitude < 0.3f)
				{
					idleTime += 0.2f;
					if (!warningSent && idleTime >= 20f)
					{
						warningSent = true;
						_onIdleWarning?.Invoke();
					}
					if (idleTime >= 60f)
					{
						_onIdleDisqualify?.Invoke();
						break;
					}
				}
				else
				{
					idleTime = 0f;
					warningSent = false;
				}
			}
		}

		public void Dispose()
		{
			StopIdleCheck();
			StopStageCountdown();
			StopFinishCountdown();
		}
	}
}
