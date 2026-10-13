using System;
using System.Collections.Generic;
using System.Threading;
using CommonUtility.Contracts;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.GameActivity;

namespace CPM.Fallout
{
	[Preserve]
	public sealed class FalloutRaceSession : IDisposable
	{
		public const int MaxRegisteredRaces = 4;

		public const int MaxStoredResults = 4;

		private const int PodiumPlaces = 3;

		private readonly IFalloutBackend _backend;

		private readonly FalloutLimitsCache _falloutLimitsCache;

		private readonly IRewardWallet _rewardWallet;

		private readonly List<string> _registered = new List<string>();

		private readonly List<FalloutRaceResult> _results = new List<FalloutRaceResult>();

		private readonly Dictionary<GameActivityType, IFalloutFinishHandler> _finishHandlers = new Dictionary<GameActivityType, IFalloutFinishHandler>();

		private CancellationTokenSource _cts = new CancellationTokenSource();

		private bool _disposed;

		public IReadOnlyList<string> RegisteredRaceIds => _registered;

		public IReadOnlyList<FalloutRaceResult> Results => _results;

		public FalloutRaceResult LastResult
		{
			get
			{
				if (_results.Count <= 0)
				{
					return null;
				}
				return _results[_results.Count - 1];
			}
		}

		public event Action<FalloutRaceResult> ResultUpdated;

		public FalloutRaceSession(IFalloutBackend backend, FalloutLimitsCache falloutLimitsCache, IRewardWallet rewardWallet, IReadOnlyList<IFalloutFinishHandler> finishHandlers)
		{
			_backend = backend;
			_falloutLimitsCache = falloutLimitsCache;
			_rewardWallet = rewardWallet;
			if (finishHandlers != null)
			{
				for (int i = 0; i < finishHandlers.Count; i++)
				{
					_finishHandlers[finishHandlers[i].ActivityType] = finishHandlers[i];
				}
			}
		}

		public void Register(string uniqueRaceId)
		{
			if (!_disposed && !string.IsNullOrEmpty(uniqueRaceId))
			{
				RegisterAsync(uniqueRaceId, _cts.Token).Forget();
			}
		}

		public void ReportFinish(GameActivityType activityType, string uniqueRaceId, long timeSpentMilliseconds, int place = -1, float delaySeconds = -1f)
		{
			if (_disposed)
			{
				return;
			}
			if (string.IsNullOrEmpty(uniqueRaceId) || !_registered.Remove(uniqueRaceId))
			{
				if (!string.IsNullOrEmpty(uniqueRaceId) && IndexOfResult(uniqueRaceId) < 0)
				{
					Publish(uniqueRaceId, FalloutRaceState.Failed, 0, 0L, 0L);
				}
			}
			else
			{
				FinishAndClaimAsync(activityType, uniqueRaceId, timeSpentMilliseconds, place, delaySeconds, _cts.Token).Forget();
			}
		}

		public bool TryGetResult(string raceId, out FalloutRaceResult result)
		{
			int num = IndexOfResult(raceId);
			result = ((num >= 0) ? _results[num] : null);
			return num >= 0;
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_registered.Clear();
				_results.Clear();
				BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
			}
		}

		private async UniTaskVoid RegisterAsync(string raceId, CancellationToken ct)
		{
			try
			{
				if (!(await _backend.StartRace(raceId, ct)).IsFail && !_disposed)
				{
					Remember(raceId);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void Remember(string raceId)
		{
			if (!_registered.Contains(raceId))
			{
				if (_registered.Count >= 4)
				{
					_registered.RemoveAt(0);
				}
				_registered.Add(raceId);
			}
		}

		private async UniTaskVoid FinishAndClaimAsync(GameActivityType activityType, string raceId, long timeSpentMilliseconds, int place, float delaySeconds, CancellationToken ct)
		{
			_ = 2;
			try
			{
				if (delaySeconds > 0f)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(delaySeconds), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
				Result<FalloutFinish> result = await _backend.FinishRace(raceId, timeSpentMilliseconds, place, ct);
				if (result.IsFail)
				{
					Publish(raceId, FalloutRaceState.Failed, 0, 0L, 0L);
					return;
				}
				int serverPlace = result.Value.Place;
				DispatchFinishData(result.Value.Data);
				Publish(raceId, FalloutRaceState.Finished, serverPlace, 0L, 0L);
				if (serverPlace < 1 || serverPlace > 3)
				{
					return;
				}
				Result<FalloutClaim> result2 = await _backend.ClaimReward(raceId, ct);
				if (result2.IsFail)
				{
					Publish(raceId, FalloutRaceState.Failed, serverPlace, 0L, 0L);
					return;
				}
				FalloutReward rewardData = result2.Value.RewardData;
				long coins = rewardData?.Coins ?? 0;
				long money = rewardData?.Money ?? 0;
				if (rewardData != null)
				{
					_falloutLimitsCache.AddEarned(activityType, coins, money);
					_rewardWallet.Credit(coins, money);
				}
				Publish(raceId, FalloutRaceState.Claimed, result2.Value.Place, coins, money);
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void DispatchFinishData(FalloutFinishData data)
		{
			if (data == null)
			{
				return;
			}
			GameActivityType type = (GameActivityType)data.Type;
			if (!_finishHandlers.TryGetValue(type, out var value))
			{
				return;
			}
			try
			{
				value.Handle(data);
			}
			catch (Exception)
			{
			}
		}

		private void Publish(string raceId, FalloutRaceState state, int place, long coins, long money)
		{
			if (_disposed)
			{
				return;
			}
			FalloutRaceResult falloutRaceResult = new FalloutRaceResult(raceId, state, place, coins, money);
			int num = IndexOfResult(raceId);
			if (num >= 0)
			{
				_results[num] = falloutRaceResult;
			}
			else
			{
				if (_results.Count >= 4)
				{
					_results.RemoveAt(0);
				}
				_results.Add(falloutRaceResult);
			}
			try
			{
				ResultUpdated?.Invoke(falloutRaceResult);
			}
			catch (Exception)
			{
			}
		}

		private int IndexOfResult(string raceId)
		{
			for (int i = 0; i < _results.Count; i++)
			{
				if (_results[i].RaceId == raceId)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
