#nullable enable annotations
using System;
using System.Collections.Generic;
using System.Threading;
using CPM.DailyTasks.Daily;
using CPM.Rating;
using CPM.Rating.Entities;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.CPMRating.DailyTasks.Scripts;
using _Car_Parking.Scripts.CPMRating.PopupNotifications;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.PrefsKeys;

namespace CPM.DailyTasks
{
	public class DailyTasksManager
	{
		public delegate void OnValueChangeDelegate(float val);

		private static string[] TypeToString = new string[29]
		{
			"t_distance", "time", "speed_banner", "gifts", "treasure", "cars", "race_win", "levels", "drift", "run",
			"police", "block_post", "real_estate", "fuel", "car_trade", "car_exchange", "burnt_tire", "car_fix", "car_wash", "offroad",
			"passanger_distance", "reactions", "drift_max", "taxi", "delivery", "cargo", "push_ups", "slicer_cut", "car_collided"
		};

		public CPM.DailyTasks.Daily.DailyTasks dailyTasks;

		public Dictionary<int, OnValueChangeDelegate> events = new Dictionary<int, OnValueChangeDelegate>();

		public DailyTaskWindowManager dailyTaskWindowManager;

		public double? timeMilliseconds;

		public int[]? finished;

		public DailyTask[] CachedDailyTasks;

		public bool gotNewTasks;

		private const string RewardedTasksPrefsKey = "DailyTaskRewardedIndices";

		private CurrencyManagerController _currencyManagerController;

		private readonly HashSet<int> _rewardedTaskIndices = new HashSet<int>();

		private string _rewardedTasksWindowKey = string.Empty;

		private string _rewardedTasksAccountKey = string.Empty;

		private readonly DailyTasksInfoContainer _dailyTasksInfoContainer;

		private readonly IDailyTasksFetchService _dailyTasksFetchService;

		private readonly DailyTaskAnalytic _dailyTaskAnalytic;

		private readonly AchievementPopupChannel _popupChannel;

		[Inject]
		public DailyTasksManager(DailyTasksInfoContainer dailyTasksInfoContainer, IDailyTasksFetchService dailyTasksFetchService, DailyTaskAnalytic dailyTaskAnalytic, AchievementPopupChannel popupChannel)
		{
			_dailyTasksInfoContainer = dailyTasksInfoContainer;
			_dailyTasksFetchService = dailyTasksFetchService;
			_dailyTaskAnalytic = dailyTaskAnalytic;
			_popupChannel = popupChannel;
		}

		public async UniTask<bool> SyncAsync(CancellationToken token = default(CancellationToken))
		{
			FunctionCallResult<CPM.DailyTasks.Daily.DailyTasks> functionCallResult = await _dailyTasksFetchService.GetAsync(token);
			if (functionCallResult.IsSuccess)
			{
				SetTasks(functionCallResult.Data);
			}
			return functionCallResult.IsSuccess;
		}

		public void SetTasks(CPM.DailyTasks.Daily.DailyTasks? result)
		{
			if (gotNewTasks || result == null)
			{
				return;
			}
			if (dailyTasks == null || dailyTasks.id != result.id || _rewardedTasksWindowKey != GetDailyWindowKey() || _rewardedTasksAccountKey != ManagerKeysForPrefs.Email)
			{
				LoadRewardedTaskIndices(result.id);
			}
			dailyTasks = result;
			if (dailyTasks.tasks == null)
			{
				Debug.LogWarning("Daily task null fix error");
				return;
			}
			DailyTask[] tasks = dailyTasks.tasks;
			for (int i = 0; i < tasks.Length; i++)
			{
				if (tasks[i].type >= TYPE.new_type)
				{
					gotNewTasks = true;
					Debug.LogError("new task type");
					return;
				}
			}
			try
			{
				CachedDailyTasks = dailyTasks?.tasks;
				if ((bool)dailyTaskWindowManager)
				{
					dailyTaskWindowManager?.Init(dailyTasks?.tasks);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			if (_dailyTasksInfoContainer != null)
			{
				_dailyTasksInfoContainer.Set(dailyTasks?.tasks);
			}
		}

		public async UniTaskVoid UpdateData(RatingData rating)
		{
			if (gotNewTasks)
			{
				return;
			}
			Dictionary<string, float> dict = rating.ToDictionary();
			int[] callback = finished;
			if (finished != null)
			{
				Debug.Log("finished: " + finished.Length);
			}
			int i = -1;
			if (dailyTasks == null)
			{
				return;
			}
			DailyTask[] tasks = dailyTasks.tasks;
			foreach (DailyTask task in tasks)
			{
				i++;
				string text = TypeToString[(int)task.type];
				bool flag = dict.ContainsKey(text);
				if (flag)
				{
					task.current = ((task.type == TYPE.drift_max) ? Mathf.Max(task.current, dict[text]) : (task.current + dict[text]));
					if (events.TryGetValue((int)task.type, out OnValueChangeDelegate value) && value != null)
					{
						try
						{
							value(task.current);
						}
						catch (Exception)
						{
						}
					}
				}
				if (callback != null && Array.IndexOf(callback, i) != -1 && _rewardedTaskIndices.Add(i))
				{
					if (!TryGiveReward(task))
					{
						_rewardedTaskIndices.Remove(i);
						continue;
					}
					SaveRewardedTaskIndices(dailyTasks.id);
					string poolId = dailyTasks.id.ToString();
					int progressTarget = GetProgressTarget(task);
					int? progressCurrent = (flag ? new int?((int)task.current) : ((int?)null));
					_dailyTaskAnalytic.LogCompleted(text, poolId, progressCurrent, progressTarget, task.prize.cash, task.prize.coin);
					_dailyTaskAnalytic.LogClaimed(text, poolId, progressTarget, task.prize.cash, task.prize.coin, CountCompletedTasks());
					PopupDailyTaskData data = new PopupDailyTaskData(task, await GetTexture(task.type));
					_popupChannel.Enqueue(data);
				}
			}
		}

		public async UniTask<Texture> GetTexture(TYPE type)
		{
			string name = Enum.GetName(type.GetType(), type);
			Texture texture = (await Resources.LoadAsync<Texture>("BattlePass/Textures/" + name + "_icon")) as Texture;
			if (texture == null)
			{
				texture = (await Resources.LoadAsync<Texture>("BattlePass/Textures/random_icon")) as Texture;
			}
			return texture;
		}

		public void Save()
		{
			if (!(ManagerKeysForPrefs.Email != "0"))
			{
				ObscuredPrefs.SetString("DailyTask", JsonConvert.SerializeObject(dailyTasks));
				ObscuredPrefs.DeleteKey("DailyTask");
			}
		}

		public void SetResourcesController(CurrencyManagerController currencyManagerController)
		{
			_currencyManagerController = currencyManagerController;
		}

		private int GetProgressTarget(DailyTask task)
		{
			return Mathf.RoundToInt((task.type == TYPE.drift_max) ? (task.goal * 1000f) : task.goal);
		}

		private int CountCompletedTasks()
		{
			if (dailyTasks?.tasks == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < dailyTasks.tasks.Length; i++)
			{
				if (dailyTasks.tasks[i].status == 1 || _rewardedTaskIndices.Contains(i))
				{
					num++;
				}
			}
			return num;
		}

		private void LoadRewardedTaskIndices(int poolId)
		{
			_rewardedTaskIndices.Clear();
			_rewardedTasksWindowKey = GetDailyWindowKey();
			_rewardedTasksAccountKey = ManagerKeysForPrefs.Email;
			string[] array = ObscuredPrefs.GetString("DailyTaskRewardedIndices", string.Empty).Split('|');
			if (array.Length != 4 || array[0] != poolId.ToString() || array[1] != _rewardedTasksWindowKey || array[2] != _rewardedTasksAccountKey)
			{
				return;
			}
			string[] array2 = array[3].Split(',');
			for (int i = 0; i < array2.Length; i++)
			{
				if (int.TryParse(array2[i], out var result))
				{
					_rewardedTaskIndices.Add(result);
				}
			}
		}

		private void SaveRewardedTaskIndices(int poolId)
		{
			string text = string.Join(",", _rewardedTaskIndices);
			ObscuredPrefs.SetString("DailyTaskRewardedIndices", $"{poolId}|{_rewardedTasksWindowKey}|{_rewardedTasksAccountKey}|{text}");
			ObscuredPrefs.Save();
		}

		private string GetDailyWindowKey()
		{
			return (timeMilliseconds.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds((long)timeMilliseconds.Value).UtcDateTime.Date : DateTime.UtcNow.Date).ToString("yyyyMMdd");
		}

		private bool TryGiveReward(DailyTask task)
		{
			if (task.prize == null)
			{
				return false;
			}
			int num = CurrencyManagerController.Money;
			try
			{
				GiveReward(task.prize);
				return true;
			}
			catch (Exception)
			{
				return (int)CurrencyManagerController.Money > num;
			}
		}

		public void GiveReward(Prize prize)
		{
			_currencyManagerController.AddMoney(new AdvancedInt(prize.cash));
			if (prize.coin < 0)
			{
				prize.coin = 0;
			}
			_currencyManagerController.UpdateCoinsFromDatabase().Forget();
		}
	}
}
