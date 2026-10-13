using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement.StepProgress;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public sealed class ObstacleRacePresentsPresenter
	{
		private const string GetLabelKey = "DailyRewardIcon.Get";

		private const float DimmedCounterAlpha = 0.6f;

		private readonly StepProgressView _progress;

		private readonly TMP_Text _counter;

		private readonly Button _claimButton;

		public ObstacleRacePresentsPresenter(StepProgressView progress, TMP_Text counter, Button claimButton)
		{
			_progress = progress;
			_counter = counter;
			_claimButton = claimButton;
		}

		public void Render(int progress, ObstacleRaceEventData data)
		{
			int filled = RenderCounter(progress, data);
			if ((bool)_progress)
			{
				_progress.SetInstant(filled);
			}
		}

		public async UniTask PlayAsync(int fromProgress, int toProgress, ObstacleRaceEventData data, CancellationToken token)
		{
			int[] steps = data?.Steps;
			int num = RenderCounter(toProgress, data);
			int num2 = StepFill(steps, EarnedPresents(steps, fromProgress), fromProgress, claimable: false);
			if ((bool)_progress)
			{
				if (num > num2)
				{
					await _progress.PlayAsync(num2, num, -1, token).SuppressCancellationThrow();
				}
				else
				{
					_progress.SetInstant(num);
				}
			}
		}

		private int RenderCounter(int progress, ObstacleRaceEventData data)
		{
			int[] array = data?.Steps;
			int num = EarnedPresents(array, progress);
			bool flag = data != null && data.Claimable > 0;
			if ((bool)_claimButton)
			{
				_claimButton.interactable = flag;
			}
			if ((bool)_counter)
			{
				_counter.alpha = (flag ? 1f : 0.6f);
				int num2 = ((array != null && array.Length != 0) ? array.Length : (data?.TotalPresents ?? 0));
				_counter.text = string.Format("{0} {1}/{2}", new LocalizedString("DailyRewardIcon.Get"), num, num2);
			}
			return StepFill(array, num, progress, flag);
		}

		private static int EarnedPresents(int[] steps, int progress)
		{
			if (steps == null)
			{
				return 0;
			}
			int i;
			for (i = 0; i < steps.Length && steps[i] <= progress; i++)
			{
			}
			return i;
		}

		private int StepFill(int[] steps, int earned, int progress, bool claimable)
		{
			int num = (_progress ? _progress.CellCount : 0);
			if (num == 0)
			{
				return 0;
			}
			if (claimable)
			{
				return num;
			}
			if (steps == null || steps.Length == 0)
			{
				return 0;
			}
			if (earned >= steps.Length)
			{
				return num;
			}
			int num2 = ((earned > 0) ? steps[earned - 1] : 0);
			int num3 = steps[earned] - num2;
			if (num3 <= 0)
			{
				return 0;
			}
			return Mathf.Clamp(Mathf.Clamp(progress - num2, 0, num3) * num / num3, 0, num);
		}
	}
}
