using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Fallout;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Racing_Scripts.RewardUI;

namespace _Car_Parking.Scripts.UIElement
{
	public class FalloutRaceSessionClaimResult : MonoBehaviour
	{
		[SerializeField]
		private RaceRewardItem[] RaceRewardItems = default;

		private const float RepaintDelaySeconds = 2f;

		private readonly List<CombinedPrice> _rewards = new List<CombinedPrice>();

		private FalloutRaceSession _raceSession;

		private string _raceId;

		private int _celebratedPlace;

		private CancellationTokenSource _animCts;

		[Inject]
		public void Construct(FalloutRaceSession raceSession)
		{
			_raceSession = raceSession;
		}

		public void Init(string raceId, IReadOnlyCollection<CombinedPrice> rewards)
		{
			_raceId = raceId;
			_rewards.Clear();
			if (rewards != null)
			{
				_rewards.AddRange(rewards);
			}
			_celebratedPlace = 0;
			UpdateRewardText();
			ScheduleAnimateResult();
		}

		private void OnEnable()
		{
			if (_raceSession != null)
			{
				_raceSession.ResultUpdated += OnResultUpdated;
			}
			if (!string.IsNullOrEmpty(_raceId))
			{
				ScheduleAnimateResult();
			}
		}

		private void OnDisable()
		{
			if (_raceSession != null)
			{
				_raceSession.ResultUpdated -= OnResultUpdated;
			}
			BaseCommonExtensions.CancelAndDisposeToken(ref _animCts);
		}

		private void OnDestroy()
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _animCts);
		}

		private void OnResultUpdated(FalloutRaceResult result)
		{
			if (result != null && !string.IsNullOrEmpty(_raceId) && result.RaceId == _raceId)
			{
				ScheduleAnimateResult();
			}
		}

		private void ScheduleAnimateResult()
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _animCts);
			_animCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
			AnimateResultAsync(_animCts.Token).Forget();
		}

		private async UniTask AnimateResultAsync(CancellationToken ct)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			AnimateResult();
		}

		private void AnimateResult()
		{
			if (RaceRewardItems == null || !base.gameObject.activeInHierarchy)
			{
				return;
			}
			FalloutRaceResult falloutRaceResult = CurrentResult();
			int num = falloutRaceResult?.Place ?? 0;
			for (int i = 1; i <= RaceRewardItems.Length; i++)
			{
				RaceRewardItem raceRewardItem = RaceRewardItems[i - 1];
				if (!raceRewardItem)
				{
					continue;
				}
				CombinedPrice combinedPrice = ((i <= _rewards.Count) ? _rewards[i - 1] : null);
				bool flag = combinedPrice != null && !combinedPrice.IsZero;
				raceRewardItem.SetActive(flag);
				if (flag)
				{
					bool num2 = i == num;
					bool state = num > 0 && i >= num;
					if (num2 && falloutRaceResult.State == FalloutRaceState.Claimed)
					{
						combinedPrice = Credited(falloutRaceResult);
					}
					raceRewardItem.SetText(PlainAmount(combinedPrice), 0, combinedPrice);
					raceRewardItem.SetState(state);
					if (num2 && _celebratedPlace != i)
					{
						_celebratedPlace = i;
						raceRewardItem.AnimateWin();
					}
				}
			}
		}

		private FalloutRaceResult CurrentResult()
		{
			if (_raceSession == null || string.IsNullOrEmpty(_raceId))
			{
				return null;
			}
			if (!_raceSession.TryGetResult(_raceId, out var result))
			{
				return null;
			}
			return result;
		}

		private void UpdateRewardText()
		{
			if (RaceRewardItems == null)
			{
				return;
			}
			for (int i = 1; i <= RaceRewardItems.Length; i++)
			{
				RaceRewardItem raceRewardItem = RaceRewardItems[i - 1];
				if ((bool)raceRewardItem)
				{
					CombinedPrice combinedPrice = ((i <= _rewards.Count) ? _rewards[i - 1] : null);
					bool flag = combinedPrice != null && !combinedPrice.IsZero;
					raceRewardItem.SetActive(flag);
					if (flag)
					{
						raceRewardItem.SetText(PlainAmount(combinedPrice), 0, combinedPrice);
						raceRewardItem.SetState(win: false);
					}
				}
			}
		}

		private static CombinedPrice Credited(FalloutRaceResult result)
		{
			return new CombinedPrice((int)result.Coins, (int)result.Money);
		}

		private static int PlainAmount(CombinedPrice reward)
		{
			return reward.IsHaveMoney ? reward.Money : reward.Coin;
		}
	}
}
