using System;
using System.Threading;
using CPM.Connection;
using CPM.DailyTasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.DailyRewards;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PlayerProfile;
using _Car_Parking.Scripts.Racing_Scripts;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	[Preserve]
	public class GameDataRepository : IGameDataRepository, IInitializable, IDisposable
	{
		private const float DelayToCheckHashMax = 120f;

		private readonly IOfferSystem _offerSystem;

		private readonly IEventService _eventService;

		private readonly IDailyRewardsServer _dailyRewardsServer;

		private readonly DailyTasksManager _dailyTasksManager;

		private readonly IConnectionManager _connectionManager;

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly IPlayerProfileService _playerProfileService;

		private readonly IRacingAdRewardReclaimService _racingAdRewardReclaim;

		private readonly IAdsDailyLimitsService _adsDailyLimits;

		private CancellationTokenSource _mainCts;

		private RallyData _rallyData;

		[Preserve]
		public GameDataRepository(IOfferSystem offerSystem, IEventService eventService, IDailyRewardsServer dailyRewardsServer, DailyTasksManager dailyTasksManager, IConnectionManager connectionManager, CurrencyManagerController currencyManagerController, RallyData rallyData, IPlayerProfileService playerProfileService, IRacingAdRewardReclaimService racingAdRewardReclaim, IAdsDailyLimitsService adsDailyLimits)
		{
			_offerSystem = offerSystem;
			_eventService = eventService;
			_dailyRewardsServer = dailyRewardsServer;
			_dailyTasksManager = dailyTasksManager;
			_connectionManager = connectionManager;
			_currencyManagerController = currencyManagerController;
			_rallyData = rallyData;
			_playerProfileService = playerProfileService;
			_racingAdRewardReclaim = racingAdRewardReclaim;
			_adsDailyLimits = adsDailyLimits;
		}

		public void Initialize()
		{
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _mainCts);
		}

		public async UniTask ExecuteAsync(GameDataRepositoryDataType dataType)
		{
			Clear();
			CommonExtensions.CancelAndDisposeToken(ref _mainCts);
			_mainCts = new CancellationTokenSource();
			await ExecuteAsync(dataType, _mainCts.Token);
		}

		public void Clear()
		{
			_eventService.Reset();
		}

		private async UniTask ExecuteAsync(GameDataRepositoryDataType dataType, CancellationToken cancellationToken)
		{
			if (!(await UniTask.WaitUntil(() =>
			{
				_connectionManager.CorrectHash.Get(out var value);
				return value;
			}, PlayerLoopTiming.Update, cancellationToken).TimeoutWithoutException(TimeSpan.FromSeconds(120.0))) && !cancellationToken.IsCancellationRequested)
			{
				await UniTask.WhenAll(UpdateCoinsAsync(), UpdateOffersAsync(), UpdateEventsAsync(), UpdateRewardsAsync(), UpdateDailyTasksAsync(cancellationToken), UpdateRallyDataAsync(), UpdateProfileAsync(cancellationToken), _racingAdRewardReclaim.ReclaimAsync(cancellationToken), _adsDailyLimits.RefreshAsync(cancellationToken));
			}
		}

		private async UniTask UpdateCoinsAsync()
		{
			await _currencyManagerController.UpdateCoinsFromDatabase();
		}

		private async UniTask UpdateOffersAsync()
		{
			await _offerSystem.SyncOffers();
		}

		private async UniTask UpdateEventsAsync()
		{
			await _eventService.RetrieveCurrentEvents();
		}

		private async UniTask UpdateRewardsAsync()
		{
			await _dailyRewardsServer.SyncRewardsAsync();
		}

		private async UniTask UpdateDailyTasksAsync(CancellationToken cancellationToken)
		{
			await _dailyTasksManager.SyncAsync(cancellationToken);
		}

		private async UniTask UpdateProfileAsync(CancellationToken cancellationToken)
		{
			await _playerProfileService.RefreshAsync(cancellationToken);
		}

		private async UniTask UpdateRallyDataAsync()
		{
			try
			{
				RallyData rallyData = await new RallySendDataService().GetRallyProgress();
				if (rallyData != null)
				{
					_rallyData.CopyFrom(rallyData);
					_rallyData.DataUpdated = true;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[GameDataRepository] UpdateRallyDataAsync failed: " + ex.Message);
			}
		}
	}
}
