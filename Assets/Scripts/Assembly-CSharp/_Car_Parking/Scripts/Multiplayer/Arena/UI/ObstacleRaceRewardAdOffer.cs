using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Fallout;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Multiplayer.Arena.UI
{
	public sealed class ObstacleRaceRewardAdOffer
	{
		private const string WindowName = "RacingRewardAdWindow";

		private const float DelaySeconds = 1f;

		private readonly ManagerUI _managerUI;

		private readonly FalloutRaceSession _session;

		private readonly IRacingRewardAdService _adService;

		private readonly ObstacleRaceSpectatorPanel _spectator;

		private CancellationTokenSource _cts;

		public ObstacleRaceRewardAdOffer(ManagerUI managerUI, FalloutRaceSession session, IRacingRewardAdService adService, ObstacleRaceSpectatorPanel spectator)
		{
			_managerUI = managerUI;
			_session = session;
			_adService = adService;
			_spectator = spectator;
		}

		public void Begin(string raceId, IReadOnlyCollection<CombinedPrice> rewards, CancellationToken ct)
		{
			Dismiss();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
			OfferAsync(raceId, new List<CombinedPrice>(rewards ?? Array.Empty<CombinedPrice>()), _cts.Token).Forget();
		}

		public void Dismiss()
		{
			BaseCommonExtensions.CancelAndDisposeToken(ref _cts);
			CloseWindow();
		}

		private async UniTaskVoid OfferAsync(string raceId, List<CombinedPrice> table, CancellationToken ct)
		{
			_ = 3;
			try
			{
				if (string.IsNullOrEmpty(raceId) || table.Count == 0 || !_managerUI || _adService == null || !_adService.CanOfferForNewRace())
				{
					return;
				}
				FalloutRaceResult claim = await AwaitClaimAsync(raceId, ct);
				if (claim == null || claim.Coins + claim.Money <= 0)
				{
					return;
				}
				do
				{
					await UniTask.WaitUntil(() => !IsSpectating(), PlayerLoopTiming.Update, ct);
					await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				}
				while (IsSpectating());
				WindowScript windowScript = await _managerUI.ShowWindow("RacingRewardAdWindow", alwaysOnTop: true, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: false);
				if (ct.IsCancellationRequested)
				{
					CloseWindow();
				}
				else if (windowScript is RacingRewardAdWindow racingRewardAdWindow)
				{
					racingRewardAdWindow.InitReward(RacingRewardMode.ObstacleRace, PlaceIncrements(table), WonPlaces(table.Count, claim.Place));
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private async UniTask<FalloutRaceResult> AwaitClaimAsync(string raceId, CancellationToken ct)
		{
			UniTaskCompletionSource<FalloutRaceResult> landed = new UniTaskCompletionSource<FalloutRaceResult>();
			_session.ResultUpdated += OnResultUpdated;
			try
			{
				FalloutRaceResult falloutRaceResult = ((!_session.TryGetResult(raceId, out var result) || !IsSettled(result)) ? (await landed.Task.AttachExternalCancellation(ct)) : result);
				FalloutRaceResult falloutRaceResult2 = falloutRaceResult;
				return (falloutRaceResult2.State == FalloutRaceState.Claimed) ? falloutRaceResult2 : null;
			}
			finally
			{
				_session.ResultUpdated -= OnResultUpdated;
			}
			void OnResultUpdated(FalloutRaceResult falloutRaceResult3)
			{
				if (falloutRaceResult3 != null && falloutRaceResult3.RaceId == raceId && IsSettled(falloutRaceResult3))
				{
					landed.TrySetResult(falloutRaceResult3);
				}
			}
		}

		private bool IsSpectating()
		{
			if ((bool)_spectator)
			{
				return _spectator.IsWatching;
			}
			return false;
		}

		private static bool IsSettled(FalloutRaceResult result)
		{
			if (result.State != FalloutRaceState.Claimed)
			{
				return result.State == FalloutRaceState.Failed;
			}
			return true;
		}

		private static int[] PlaceIncrements(List<CombinedPrice> table)
		{
			int count = table.Count;
			int[] array = new int[count];
			for (int num = count; num >= 1; num--)
			{
				CombinedPrice combinedPrice = table[num - 1];
				CombinedPrice combinedPrice2 = ((num < count) ? table[num] : null);
				if (!(combinedPrice == null))
				{
					int num2 = (int)combinedPrice.Coin - ((combinedPrice2 != null) ? ((int)combinedPrice2.Coin) : 0);
					int num3 = (int)combinedPrice.Money - ((combinedPrice2 != null) ? ((int)combinedPrice2.Money) : 0);
					array[count - num] = ((num2 > 0) ? num2 : num3);
				}
			}
			return array;
		}

		private static int WonPlaces(int count, int place)
		{
			if (place < 1 || place > count)
			{
				return 0;
			}
			return count - place + 1;
		}

		private void CloseWindow()
		{
			if ((bool)_managerUI)
			{
				_managerUI.CloseWindow("RacingRewardAdWindow");
			}
		}
	}
}
