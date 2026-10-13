using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GrantService;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Rally.Data;

public class RallyProgressController : IDisposable
{
	private RallyData _rallyData;

	private RallyController _rallyController;

	private RallySendDataService _rallySendDataService;

	private ManagerUI _managerUI;

	private CurrencyManagerController _currencyManagerController;

	private RewardGranter _rewardGranter;

	private RallyMapDataSO _mapData;

	private bool _isDestroyed;

	public RallyData RallyData => _rallyData;

	public RallyProgressController(RallyMapDataSO mapData)
	{
		_mapData = mapData;
		_rallySendDataService = new RallySendDataService();
	}

	public void Init(RallyController rallyController, ManagerUI managerUI, CurrencyManagerController currencyManagerController, RallyData rallyData, RewardGranter rewardGranter = null)
	{
		_currencyManagerController = currencyManagerController;
		_rallyController = rallyController;
		_managerUI = managerUI;
		_rallyData = rallyData;
		_rewardGranter = rewardGranter;
	}

	public async UniTask GetAllLocationsProgressAsync(RallyData rallyData)
	{
		_rallyData = rallyData;
		if (!rallyData.DataUpdated)
		{
			RallyData rallyData2 = await _rallySendDataService.GetRallyProgress();
			if (rallyData2 != null)
			{
				rallyData.CopyFrom(rallyData2);
				rallyData.DataUpdated = true;
			}
		}
	}

	public async UniTask<RallyProgressResult> SetProgress(float time)
	{
		int locationPosition = RallyController.LocationIndex;
		int trajectoryPosition = RallyController.TrajectoryIndex;
		if (!TryGetProgressIds(locationPosition, trajectoryPosition, out var locationId, out var trajectoryId))
		{
			return null;
		}
		RallyProgressData raceResutsAsync = new RallyProgressData
		{
			Time = time,
			LocationId = locationId,
			TrajectoryId = trajectoryId
		};
		RallyProgressResult raceResult = await _rallySendDataService.SetRaceResutsAsync(raceResutsAsync);
		if (raceResult == null)
		{
			return null;
		}
		bool ngAvailable = raceResult.NGAvailable == 1;
		_rallyData.NgAvailable = ngAvailable;
		UpdateCachedProgress(locationPosition, trajectoryPosition, time, raceResult.Progress);
		_rallyController.OnLocalProgressUpdated?.Invoke();
		if (raceResult.AwardCoins > 0)
		{
			CurrencyManagerController.SetCoins(new AdvancedInt((int)CurrencyManagerController.Coins + raceResult.AwardCoins));
			CurrencyManagerController.UpdateCoinsVisuals();
		}
		if (raceResult.AwardMoney > 0)
		{
			_currencyManagerController.AddMoney(new AdvancedInt(raceResult.AwardMoney));
		}
		if (raceResult.AwardSpecific == 1 && (bool)_rallyController.RallyMapDataSO)
		{
			RallySequenceData rallySequenceData = _rallyController.RallyMapDataSO.GetRallySequenceData(locationId, trajectoryId);
			if (rallySequenceData != null && rallySequenceData.HaveReward && _rewardGranter != null)
			{
				await _rewardGranter.GrantAsync(rallySequenceData.Reward.RewardItems);
			}
		}
		if ((bool)_managerUI)
		{
			_managerUI.ReloadActiveWindows();
			if (raceResult.AwardCoins > 0 || raceResult.AwardMoney > 0 || raceResult.AwardSpecific == 1)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("EventService.RewardRecieved"), 4f).Forget();
			}
		}
		return raceResult;
	}

	public async UniTask<bool> SetNewGamePlus()
	{
		float previousNgCount = _rallyData.NgCount;
		_rallyData.NgCount = previousNgCount + 1f;
		RallyData rallyData = await _rallySendDataService.SetNewGamePlusResutsAsync();
		if (rallyData != null)
		{
			_rallyData.NgTimeFactor = rallyData.NgTimeFactor;
			long lastNgPlusAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			_rallyData.LastNgPlusAt = lastNgPlusAt;
			_rallyData.NgAvailable = false;
			_rallyData.DataUpdated = false;
			_rallyData.RallyProgress = new Dictionary<string, RallyProgress>();
			return true;
		}
		_rallyData.NgCount = previousNgCount;
		return false;
	}

	public (int locationId, int trajectoryId, bool allCompleted) GetUnlockFrontier()
	{
		RallyMapDataSO mapData = _mapData;
		if (mapData?.RallyLocationDatas == null)
		{
			return (locationId: -1, trajectoryId: -1, allCompleted: false);
		}
		for (int i = 0; i < mapData.RallyLocationDatas.Count; i++)
		{
			List<RallySequenceData> list = mapData.RallyLocationDatas[i]?.TrajectoryData?.RallySequenceDataList;
			if (list == null)
			{
				continue;
			}
			for (int j = 0; j < list.Count; j++)
			{
				RallyProgress progress = GetProgress(i, j);
				if ((progress == null || progress.PersonalBest <= 0f) && !list[j].IsSpecial)
				{
					return (locationId: i, trajectoryId: j, allCompleted: false);
				}
			}
		}
		return (locationId: -1, trajectoryId: -1, allCompleted: true);
	}

	private bool TryGetProgressIds(int locationPosition, int trajectoryPosition, out int locationId, out int trajectoryId)
	{
		locationId = -1;
		trajectoryId = -1;
		if (_mapData != null)
		{
			return _mapData.TryGetConfigIds(locationPosition, trajectoryPosition, out locationId, out trajectoryId);
		}
		return false;
	}

	public RallyProgress GetProgress(int locationPosition, int trajectoryPosition)
	{
		Dictionary<string, RallyProgress> dictionary = _rallyData?.RallyProgress;
		if (dictionary == null)
		{
			return null;
		}
		if (!TryGetProgressIds(locationPosition, trajectoryPosition, out var locationId, out var trajectoryId))
		{
			return null;
		}
		if (!dictionary.TryGetValue(RallyData.ProgressKey(locationId, trajectoryId), out var value))
		{
			return null;
		}
		return value;
	}

	private RallyProgress GetOrCreateProgress(int locationPosition, int trajectoryPosition)
	{
		if (_rallyData == null)
		{
			return null;
		}
		if (!TryGetProgressIds(locationPosition, trajectoryPosition, out var locationId, out var trajectoryId))
		{
			return null;
		}
		RallyData rallyData = _rallyData;
		if (rallyData.RallyProgress == null)
		{
			rallyData.RallyProgress = new Dictionary<string, RallyProgress>();
		}
		string key = RallyData.ProgressKey(locationId, trajectoryId);
		if (!_rallyData.RallyProgress.TryGetValue(key, out var value) || value == null)
		{
			value = new RallyProgress();
			_rallyData.RallyProgress[key] = value;
		}
		return value;
	}

	private void UpdateCachedProgress(int locationId, int trajectoryId, float time, int result)
	{
		RallyProgress orCreateProgress = GetOrCreateProgress(locationId, trajectoryId);
		if (orCreateProgress == null)
		{
			return;
		}
		if (orCreateProgress.PersonalBest <= 0f || time < orCreateProgress.PersonalBest)
		{
			orCreateProgress.PersonalBest = time;
		}
		int rallyTier = orCreateProgress.RallyTier;
		if (result <= rallyTier)
		{
			return;
		}
		orCreateProgress.RallyTier = result;
		if (_rallyData?.TotalCups != null)
		{
			int num = 3 - result;
			int num2 = 3 - rallyTier;
			for (int i = num; i < num2 && i < _rallyData.TotalCups.Length; i++)
			{
				_rallyData.TotalCups[i]++;
			}
		}
	}

	public void Dispose()
	{
	}
}
