using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.StepProgress
{
	public class StepProgressView : MonoBehaviour
	{
		[SerializeField]
		private List<ProgressCellView> Cells = new List<ProgressCellView>();

		[SerializeField]
		[Tooltip("Pause before the first cell lights up, in seconds.")]
		private float StartDelay = default;

		[SerializeField]
		[Tooltip("Pause between two consecutive cells lighting up, in seconds.")]
		private float DelayBetweenCells = 0.25f;

		[SerializeField]
		[Tooltip("Ignore Time.timeScale so the sequence keeps playing while the game is paused.")]
		private bool UnscaledTime = true;

		private CancellationTokenSource _playCts;

		public int CellCount => Cells.Count;

		public void SetInstant(int filled, int max = -1)
		{
			StopPlaying();
			ApplyPlan(filled, filled, ResolveMax(max), null);
		}

		public async UniTask PlayAsync(int from, int to, int max = -1, CancellationToken token = default(CancellationToken))
		{
			StopPlaying();
			List<ProgressCellView> animated = new List<ProgressCellView>();
			ApplyPlan(from, to, ResolveMax(max), animated);
			if (animated.Count == 0)
			{
				return;
			}
			CancellationTokenSource cts = (_playCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy(), token));
			try
			{
				CancellationToken playToken = cts.Token;
				if (StartDelay > 0f)
				{
					await Delay(StartDelay, playToken);
				}
				List<UniTask> running = new List<UniTask>(animated.Count);
				for (int i = 0; i < animated.Count; i++)
				{
					if (i > 0 && DelayBetweenCells > 0f)
					{
						await Delay(DelayBetweenCells, playToken);
					}
					running.Add(animated[i].PlayFillAsync(playToken));
				}
				await UniTask.WhenAll(running);
			}
			finally
			{
				if (_playCts == cts)
				{
					_playCts = null;
				}
				cts.Dispose();
			}
		}

		private void OnDisable()
		{
			StopPlaying();
		}

		private void OnDestroy()
		{
			StopPlaying();
		}

		private void StopPlaying()
		{
			if (_playCts != null)
			{
				CancellationTokenSource playCts = _playCts;
				_playCts = null;
				playCts.Cancel();
			}
		}

		private void ApplyPlan(int from, int to, int max, List<ProgressCellView> animated)
		{
			for (int i = 0; i < Cells.Count; i++)
			{
				ProgressCellView progressCellView = Cells[i];
				if ((bool)progressCellView)
				{
					ProgressCellPlan plan = StepProgressPlanner.GetPlan(i, from, to, max);
					progressCellView.SetHidden(plan == ProgressCellPlan.Hidden);
					if (plan == ProgressCellPlan.Animated && animated != null)
					{
						progressCellView.SetFilled(filled: false);
						animated.Add(progressCellView);
					}
					else
					{
						progressCellView.SetFilled(plan == ProgressCellPlan.Filled);
					}
				}
			}
		}

		private int ResolveMax(int max)
		{
			int num = ((max < 0) ? Cells.Count : max);
			if (num > Cells.Count)
			{
				num = Cells.Count;
			}
			return num;
		}

		private UniTask Delay(float seconds, CancellationToken token)
		{
			return UniTask.Delay(TimeSpan.FromSeconds(seconds), UnscaledTime, PlayerLoopTiming.Update, token);
		}
	}
}
