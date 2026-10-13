using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Account;
using CPM.Ads.Policy;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;

namespace _Car_Parking.Scripts.Drag
{
	[Preserve]
	public sealed class ClientAdRewardRecoveryService : IInitializable, IDisposable
	{
		private const float CarDataTimeoutSeconds = 60f;

		private const float AuthorizationTimeoutSeconds = 120f;

		private static bool _recoveredThisSession;

		private readonly IAdsRewardsBackend _backend;

		private readonly ITimerTuningService _timers;

		private readonly IEngineTuningTimerService _engineTimers;

		private readonly ICarInfoService _carInfoService;

		private readonly IAccountController _accountController;

		private readonly IRacingAdRewardReclaimService _racingAdRewardReclaim;

		private readonly AdsAnalytic _adsAnalytic;

		private CancellationTokenSource _cts;

		public ClientAdRewardRecoveryService(IAdsRewardsBackend backend, ITimerTuningService timers, IEngineTuningTimerService engineTimers, ICarInfoService carInfoService, IAccountController accountController, IRacingAdRewardReclaimService racingAdRewardReclaim, AdsAnalytic adsAnalytic)
		{
			_backend = backend;
			_timers = timers;
			_engineTimers = engineTimers;
			_carInfoService = carInfoService;
			_accountController = accountController;
			_racingAdRewardReclaim = racingAdRewardReclaim;
			_adsAnalytic = adsAnalytic;
		}

		public void Initialize()
		{
			_cts = new CancellationTokenSource();
			RecoverAsync(_cts.Token).Forget();
		}

		public void Dispose()
		{
			_cts?.Cancel();
			_cts?.Dispose();
			_cts = null;
		}

		private async UniTask RecoverAsync(CancellationToken ct)
		{
			_ = 5;
			try
			{
				if (_recoveredThisSession || await UniTask.WaitUntil(() => _accountController.Authorized, PlayerLoopTiming.Update, ct).TimeoutWithoutException(TimeSpan.FromSeconds(120.0)))
				{
					return;
				}
				Result<List<UnclaimedAdReward>> result = await _backend.GetUnclaimed(ct);
				if (result.IsFail)
				{
					return;
				}
				_recoveredThisSession = true;
				string competitionAdId = null;
				foreach (UnclaimedAdReward item in result.Value)
				{
					if (item == null || string.IsNullOrEmpty(item.AdId))
					{
						continue;
					}
					switch (item.Placement)
					{
					case "EngineTimerReduce":
						await RecoverOneAsync(item.AdId, AdPlacement.EngineTimerReduce, FinishTuningTimer, ct);
						break;
					case "FuelRefill":
						await RecoverOneAsync(item.AdId, AdPlacement.FuelRefill, RefillFuelTank, ct);
						break;
					case "ParkingContinue":
						await RecoverOneAsync(item.AdId, AdPlacement.ParkingContinue, (int _, int __) => true, ct);
						break;
					case "Competition":
						if (competitionAdId == null)
						{
							competitionAdId = item.AdId;
						}
						break;
					}
				}
				if (competitionAdId != null)
				{
					await _racingAdRewardReclaim.ReclaimAsync(competitionAdId, ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception)
			{
			}
		}

		private async UniTask RecoverOneAsync(string adId, AdPlacement placement, Func<int, int, bool> apply, CancellationToken ct)
		{
			string attemptAdId = PendingAdRewardPrefs.GetAdId(placement);
			bool hasContext = PendingAdRewardPrefs.TryGet(placement, out var slotId, out var carId);
			bool benchContext = placement == AdPlacement.EngineTimerReduce && PendingAdRewardPrefs.TryGetEngine(out var _);
			hasContext |= benchContext;
			if (hasContext && !benchContext && await UniTask.WaitUntil(() => _carInfoService.TryGetFromSlot(slotId, out var _), PlayerLoopTiming.Update, ct).TimeoutWithoutException(TimeSpan.FromSeconds(60.0)))
			{
				return;
			}
			Result<List<AdRewardClaimResult>> result = await _backend.Claim(new List<AdRewardClaimEntry>
			{
				new AdRewardClaimEntry
				{
					AdId = adId,
					Type = "None",
					Id = placement.ToString()
				}
			}, ct);
			if (result.IsFail || result.Value.Count == 0 || result.Value[0] == null)
			{
				return;
			}
			string status = result.Value[0].Status;
			if (!(status == "FAILED") && hasContext)
			{
				if (status == "CONSUMED")
				{
					bool flag = apply(slotId, carId);
					_adsAnalytic.ReportRecoveredReward(placement, attemptAdId, adId, flag, flag ? null : "GRANT_ERROR", flag ? null : "the pending effect could not be applied after restart");
				}
				else
				{
					_adsAnalytic.ReportRecoveredReward(placement, attemptAdId, adId, granted: false, AdRewardErrorCode.FromClaimStatus(status), status);
				}
				PendingAdRewardPrefs.Clear(placement);
				if (benchContext)
				{
					PendingAdRewardPrefs.ClearEngine();
				}
			}
		}

		private bool FinishTuningTimer(int slotId, int carId)
		{
			if (PendingAdRewardPrefs.TryGetEngine(out var engineId))
			{
				PendingAdRewardPrefs.ClearEngine();
				if (!_engineTimers.TryGetTimerInfo(engineId, out var info))
				{
					return true;
				}
				return _engineTimers.TryFinishNow(engineId, info.TimerId);
			}
			if (!IsSameCar(slotId, carId))
			{
				return false;
			}
			if (!_timers.TryGetEngineTimerInfo(slotId, out var info2))
			{
				return true;
			}
			return _timers.TryFinishNow(slotId, info2.TimerId);
		}

		private bool RefillFuelTank(int slotId, int carId)
		{
			if (!IsSameCar(slotId, carId))
			{
				return false;
			}
			if (!_carInfoService.TryGetFromSlot(slotId, out var carInfo))
			{
				return false;
			}
			CarDefaultTuningDataConfig defaultTuningConfig = carInfo.GetDefaultTuningConfig();
			if (defaultTuningConfig != null)
			{
				CarTuningData tuningData = carInfo.TuningData;
				if (tuningData != null)
				{
					_ = tuningData.Maintenance;
					if (0 == 0)
					{
						carInfo.TuningData.Maintenance.FuelInTank = defaultTuningConfig.ChassisData.FuelTankCapacity;
						return true;
					}
				}
			}
			return false;
		}

		private bool IsSameCar(int slotId, int carId)
		{
			if (_carInfoService.TryGetFromSlot(slotId, out var carInfo))
			{
				return carInfo.CarID == carId;
			}
			return false;
		}
	}
}
