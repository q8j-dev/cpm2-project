using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Account;
using CPM.Ads.Policy;
using CPM.TuningTimer.Integrity;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	public class EngineTuningTimerService : IEngineTuningTimerService, IInitializable, IDisposable, IAuthorizationDependant
	{
		private const string TuningTimerKey = "EngineTuningTimer";

		private const float CheckTimerIntervalInSec = 0.1f;

		private readonly SortedLinkedList<int, EngineTuningTimer> _timers = new SortedLinkedList<int, EngineTuningTimer>();

		private IDisposable _timerCheck;

		private int _currentEngineId = -1;

		private readonly TimesToWaitTuningConfig _timesToWaitTuningConfig;

		private readonly IInstallTimeCatalog _installTimes;

		private readonly ManagerUI _managerUI;

		private readonly ICarTuningFactory _carTuningFactory;

		private readonly IEngineKitsInventoryService _engineKitsInventoryService;

		private readonly IInventoryPartsService _inventoryPartsService;

		private readonly IAccountController _accountController;

		private readonly double _wallClockAnchor = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		private readonly double _realtimeAnchor = Time.realtimeSinceStartupAsDouble;

		private double CurrentUnixTimeSeconds => _wallClockAnchor + (Time.realtimeSinceStartupAsDouble - _realtimeAnchor);

		public bool IsOnPause { get; set; }

		public int ObservingEngineId
		{
			get
			{
				return _currentEngineId;
			}
			set
			{
				_currentEngineId = value;
				SwitchToEngineProgressEvent?.Invoke(_currentEngineId > 0);
			}
		}

		public event Action<TimerEnums, float> ProgressEvent;

		public event Action<bool> HideEvent;

		public event Action<bool> SwitchToEngineProgressEvent;

		public event Action<int> OnTimerCompleted;

		public EngineTuningTimerService(TimesToWaitTuningConfig timesToWaitTuningConfig, ICarTuningFactory carTuningFactory, IEngineKitsInventoryService engineKitsInventoryService, IInventoryPartsService inventoryPartsService, OptionalDependency<ManagerUI> managerUI, IInstallTimeCatalog installTimes, IAccountController accountController)
		{
			_installTimes = installTimes;
			_inventoryPartsService = inventoryPartsService;
			_engineKitsInventoryService = engineKitsInventoryService;
			_carTuningFactory = carTuningFactory;
			_managerUI = managerUI.Value;
			_timesToWaitTuningConfig = timesToWaitTuningConfig;
			_accountController = accountController;
		}

		public bool CheckEngineBlocked(int engineId)
		{
			return _timers.ContainsKey(engineId);
		}

		public void CreateTimer(int engineId, params PartData[] partsToSet)
		{
			if (partsToSet == null || partsToSet.Length == 0)
			{
				Debug.LogError("Parts to set is null or empty.");
				return;
			}
			if (CheckEngineBlocked(engineId))
			{
				_ = _managerUI.ShowNotificationWindow(new LocalizedString("EngineTuningTimerService.Thisengineisintheservicenow.Wait."), 3f);
				return;
			}
			foreach (PartData partData in partsToSet)
			{
				if (partData != null)
				{
					_inventoryPartsService.Remove(partData);
				}
			}
			double time = GetTime(partsToSet);
			EngineTuningTimer engineTuningTimer = new EngineTuningTimer
			{
				EngineId = engineId,
				PartsToSet = partsToSet.ToList(),
				TimeOfStart = CurrentUnixTimeSeconds
			};
			Sign(engineTuningTimer, time);
			_timers.Add(engineId, engineTuningTimer);
			SaveTimer(engineTuningTimer);
		}

		private double ExpectedDuration(EngineTuningTimer timer)
		{
			double baseDuration = BaseDuration(timer);
			return AdTimerReductionRules.ExpectedDuration(baseDuration, AdTimerReductionRules.ClampReduction(timer.AdReductionSeconds, baseDuration));
		}

		private double BaseDuration(EngineTuningTimer timer)
		{
			return GetTime((timer.PartsToSet ?? new List<PartData>()).ToArray());
		}

		private static string BuildTimerId(EngineTuningTimer timer)
		{
			return $"{timer.EngineId}:{timer.TimeOfStart:F0}";
		}

		private static List<int> PayloadIds(EngineTuningTimer timer)
		{
			List<int> list = new List<int>();
			if (timer.PartsToSet != null)
			{
				foreach (PartData item in timer.PartsToSet)
				{
					if (item != null)
					{
						list.Add(item.InstanceID);
						list.Add(item.ModelId);
					}
				}
			}
			return list;
		}

		private static TuningTimerFacts FactsOf(EngineTuningTimer timer, double duration)
		{
			return new TuningTimerFacts(timer.EngineId, timer.TimeOfStart, duration, PayloadIds(timer));
		}

		private static void Sign(EngineTuningTimer timer, double duration)
		{
			timer.TimeOfEnd = timer.TimeOfStart + duration;
			string current = TuningTimerSecret.Current;
			if (current != null)
			{
				timer.Signature = TuningTimerSignature.Compute(current, FactsOf(timer, duration));
			}
		}

		private bool EnsureIntegrity(EngineTuningTimer timer)
		{
			double num = AdTimerReductionRules.ClampReduction(timer.AdReductionSeconds, BaseDuration(timer));
			bool flag = Math.Abs(num - timer.AdReductionSeconds) > 0.001;
			if (flag)
			{
				timer.AdReductionSeconds = num;
			}
			double num2 = ExpectedDuration(timer);
			string current = TuningTimerSecret.Current;
			if (current == null)
			{
				timer.TimeOfEnd = timer.TimeOfStart + num2;
				if (flag)
				{
					SaveTimer(timer);
				}
				return true;
			}
			if (TuningTimerSignature.Verify(timer.Signature, current, FactsOf(timer, num2)))
			{
				timer.TimeOfEnd = timer.TimeOfStart + num2;
				if (flag)
				{
					SaveTimer(timer);
				}
				return true;
			}
			Debug.LogError($"Engine tuning timer {timer.EngineId} failed its signature check. Restarting it.");
			timer.TimeOfStart = CurrentUnixTimeSeconds;
			timer.AdReductionSeconds = 0.0;
			Sign(timer, ExpectedDuration(timer));
			SaveTimer(timer);
			return false;
		}

		public bool TryGetTimerInfo(int engineId, out EngineTimerInfo info)
		{
			info = default;
			if (!_timers.TryGetByKey(engineId, out var value) || value == null)
			{
				return false;
			}
			EnsureIntegrity(value);
			info = new EngineTimerInfo(BuildTimerId(value), Math.Max(0.0, value.TimeOfEnd - CurrentUnixTimeSeconds), AdTimerReductionRules.ClampReduction(value.AdReductionSeconds, BaseDuration(value)), isEngineInstall: true, hasGearBox: false);
			return true;
		}

		public List<TuningTimerEntry> GetTimerEntries(int engineId)
		{
			List<TuningTimerEntry> list = new List<TuningTimerEntry>();
			if (!_timers.TryGetByKey(engineId, out var value) || value?.PartsToSet == null)
			{
				return list;
			}
			EnsureIntegrity(value);
			double val = Math.Max(0.0, value.TimeOfEnd - CurrentUnixTimeSeconds);
			foreach (PartData item in value.PartsToSet)
			{
				if (item != null)
				{
					double num = GetTimeFromConfig(item.PartType);
					list.Add(new TuningTimerEntry(item, null, num, Math.Min(num, val)));
				}
			}
			return list;
		}

		public bool TryApplyAdReduction(int engineId, string timerId, double seconds)
		{
			if (seconds <= 0.0)
			{
				return false;
			}
			if (!_timers.TryGetByKey(engineId, out var value) || value == null)
			{
				return false;
			}
			EnsureIntegrity(value);
			if (BuildTimerId(value) != timerId)
			{
				return false;
			}
			double baseDuration = BaseDuration(value);
			double num = AdTimerReductionRules.ClampReduction(value.AdReductionSeconds, baseDuration);
			double num2 = AdTimerReductionRules.ClampReduction(num + seconds, baseDuration);
			if (num2 - num <= 0.0)
			{
				return false;
			}
			value.AdReductionSeconds = num2;
			Sign(value, ExpectedDuration(value));
			SaveTimer(value);
			Resort(value);
			return true;
		}

		public bool TryFinishNow(int engineId, string timerId)
		{
			if (!_timers.TryGetByKey(engineId, out var value) || value == null)
			{
				return false;
			}
			EnsureIntegrity(value);
			if (BuildTimerId(value) != timerId)
			{
				return false;
			}
			double num = ExpectedDuration(value);
			value.TimeOfStart = CurrentUnixTimeSeconds - num;
			Sign(value, num);
			SaveTimer(value);
			Resort(value);
			return true;
		}

		private void Resort(EngineTuningTimer timer)
		{
			_timers.Remove(timer.EngineId);
			_timers.Add(timer.EngineId, timer);
		}

		public List<PartData> GetPartsInTimers()
		{
			List<PartData> list = new List<PartData>();
			foreach (EngineTuningTimer timer in _timers)
			{
				list.AddRange(timer.PartsToSet);
			}
			return list;
		}

		public void Initialize()
		{
			LoadData();
			_timerCheck = Observable.Interval(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
			{
				Update();
			});
			_engineKitsInventoryService.OnEngineRemoved += OnEngineRemoved;
			_accountController.RegisterAuthorizationDependant(this);
		}

		public void Dispose()
		{
			_accountController.UnregisterAuthorizationDependant(this);
			_engineKitsInventoryService.OnEngineRemoved -= OnEngineRemoved;
			DisposeTimer();
			_timers.Clear();
		}

		public UniTaskVoid OnSuccessfulAuthorization()
		{
			DisposeTimer();
			return default;
		}

		public void OnSignOut()
		{
			DisposeTimer();
		}

		private void Update()
		{
			if (IsOnPause)
			{
				return;
			}
			double currentUnixTimeSeconds = CurrentUnixTimeSeconds;
			if (!_timers.TryGetFirst(out var value))
			{
				return;
			}
			double timeOfEnd = value.TimeOfEnd;
			EnsureIntegrity(value);
			if (currentUnixTimeSeconds < value.TimeOfStart)
			{
				value.TimeOfStart = currentUnixTimeSeconds;
				Sign(value, ExpectedDuration(value));
				SaveTimer(value);
			}
			if (value.TimeOfEnd != timeOfEnd)
			{
				_timers.Remove(value.EngineId);
				_timers.Add(value.EngineId, value);
			}
			else if (currentUnixTimeSeconds >= value.TimeOfEnd)
			{
				if (ObservingEngineId == value.EngineId)
				{
					HideEvent?.Invoke(obj: true);
				}
				EngineKitData engineById = _engineKitsInventoryService.GetEngineById(value.EngineId);
				if (engineById == null)
				{
					DeleteTimer(value.EngineId);
					return;
				}
				OnTimerCompleted?.Invoke(value.EngineId);
				foreach (PartData item in value.PartsToSet)
				{
					_carTuningFactory.SetOnePart(item, engineById);
				}
				_engineKitsInventoryService.Save();
				DeleteTimer(value.EngineId);
			}
			else if (ObservingEngineId == value.EngineId)
			{
				float arg = (float)((currentUnixTimeSeconds - value.TimeOfStart) / (value.TimeOfEnd - value.TimeOfStart));
				ProgressEvent?.Invoke(TimerEnums.Engine, arg);
			}
		}

		private double GetTime(PartData[] partsToSet)
		{
			List<float> list = (from x in partsToSet
				where x != null
				select GetTimeFromConfig(x.PartType)).ToList();
			if (list.Count == 0)
			{
				return 5.0;
			}
			return list.Max();
		}

		private float GetTimeFromConfig(PartType partType)
		{
			if (_installTimes.TryGet(partType, out var seconds))
			{
				return seconds;
			}
			return _timesToWaitTuningConfig.Times.Find((WaitTimer x) => x.PartType == partType)?.Time ?? 5f;
		}

		private string GetPath(int engineId)
		{
			return "TuningTimers/EngineTuning/" + engineId;
		}

		private void SaveTimer(EngineTuningTimer engineTuningTimer)
		{
			EasySaveDataHandler.SaveValues(new Dictionary<string, object> { { "EngineTuningTimer", engineTuningTimer } }, GetPath(engineTuningTimer.EngineId));
			Debug.Log($"Try save engine timer {engineTuningTimer.EngineId}");
		}

		private void DeleteTimer(int engineId, bool returnPartsToInventory = false)
		{
			string path = GetPath(engineId);
			if (returnPartsToInventory && EasySaveDataHandler.TryGetFile(path, out var file))
			{
				foreach (PartData item in file.Load<EngineTuningTimer>("EngineTuningTimer").PartsToSet)
				{
					if (item != null)
					{
						_inventoryPartsService.Add(item);
					}
				}
			}
			_timers.Remove(engineId);
			EasySaveDataHandler.DeleteFile(path);
			Debug.Log($"Try delete engine timer {engineId}");
		}

		private void DisposeTimer()
		{
			_timerCheck?.Dispose();
		}

		private void OnEngineRemoved(EngineKitData engine)
		{
			DeleteTimer(engine.EngineId.InstanceID, returnPartsToInventory: true);
		}

		private void LoadData()
		{
			if (!EasySaveDataHandler.TryGetFile("InventoryEnginesData", out var file))
			{
				return;
			}
			foreach (string item in InventoryExtensions.ReadEnginesSetData(file).Inventory.Select((EngineKitData engine) => GetPath(engine.EngineId.InstanceID)))
			{
				if (EasySaveDataHandler.TryGetFile(item, out var file2))
				{
					EngineTuningTimer engineTuningTimer = file2.Load<EngineTuningTimer>("EngineTuningTimer");
					EnsureIntegrity(engineTuningTimer);
					_timers.Add(engineTuningTimer.EngineId, engineTuningTimer);
				}
			}
		}
	}
}
