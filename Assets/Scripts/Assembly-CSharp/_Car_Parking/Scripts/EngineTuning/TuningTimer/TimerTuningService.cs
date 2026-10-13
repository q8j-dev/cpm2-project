using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Account;
using CPM.Ads.Policy;
using CPM.Database;
using CPM.TuningTimer.Integrity;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Persistence.Systems;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Inventory;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	[Preserve]
	public class TimerTuningService : ITimerTuningService, IInitializable, IDisposable, IAuthorizationDependant
	{
		private const string TuningTimerKey = "TuningTimer";

		private const float CheckTimerIntervalInSec = 0.1f;

		private const float DebugTuningTimerTime = 1.5f;

		private readonly ICarInfoService _carInfoService;

		private readonly ICarTuningFactory _carTuningFactory;

		private readonly IEngineKitsInventoryService _engineKitsInventoryService;

		private readonly IInventoryPartsService _inventoryPartsService;

		private readonly ISlotService _slotService;

		private readonly IAccountController _accountController;

		private readonly TimesToWaitTuningConfig _timesToWaitTuningConfig;

		private readonly IInstallTimeCatalog _installTimes;

		private IDisposable _timerCheck;

		private int prevSlotId = -1;

		private ISteamAchievementService _steamAchievementService;

		private readonly double _wallClockAnchor = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		private readonly double _realtimeAnchor = Time.realtimeSinceStartupAsDouble;

		private double CurrentUnixTimeSeconds => _wallClockAnchor + (Time.realtimeSinceStartupAsDouble - _realtimeAnchor);

		public bool IsPause { get; set; }

		public event Action<int, List<PartData>> TimerCompletedEvent;

		public event Action<TimerEnums, float> ProgressEvent;

		public event Action<bool> HideEvent;

		public TimerTuningService(TimesToWaitTuningConfig timesToWaitTuningConfig, ICarTuningFactory carTuningFactory, ICarInfoService carInfoService, ISlotService slotService, IInventoryPartsService inventoryPartsService, IEngineKitsInventoryService engineKitsInventoryService, IAccountController accountController, IInstallTimeCatalog installTimes, ISteamAchievementService steamAchievementService)
		{
			_installTimes = installTimes;
			_inventoryPartsService = inventoryPartsService;
			_engineKitsInventoryService = engineKitsInventoryService;
			_slotService = slotService;
			_carInfoService = carInfoService;
			_carTuningFactory = carTuningFactory;
			_timesToWaitTuningConfig = timesToWaitTuningConfig;
			_steamAchievementService = steamAchievementService;
			_accountController = accountController;
		}

		public UniTaskVoid OnSuccessfulAuthorization()
		{
			StartTimerCheck();
			return default;
		}

		private void StartTimerCheck()
		{
			DisposeTimer();
			_timerCheck = Observable.Interval(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
			{
				CheckCar();
			});
		}

		public void OnSignOut()
		{
			DisposeTimer();
		}

		public void Dispose()
		{
			_accountController.UnregisterAuthorizationDependant(this);
			_slotService.OnSlotRemoved -= RemoveCarTimer;
			DisposeTimer();
		}

		public void Initialize()
		{
			StartTimerCheck();
			_slotService.OnSlotRemoved += RemoveCarTimer;
			_accountController.RegisterAuthorizationDependant(this);
		}

		public List<EngineKitData> GetEnginesInTimers()
		{
			List<EngineKitData> list = new List<EngineKitData>();
			for (int i = 0; i < _slotService.SlotCount; i++)
			{
				if (EasySaveDataHandler.TryGetFile(GetPath(i), out var file))
				{
					EngineKitData engineKitData = file.Load<CarTuningTimer>("TuningTimer").EngineKitData;
					if (engineKitData != null)
					{
						list.Add(engineKitData);
					}
				}
			}
			return list;
		}

		public List<PartData> GetPartsInTimers()
		{
			List<PartData> list = new List<PartData>();
			for (int i = 0; i < _slotService.SlotCount; i++)
			{
				if (EasySaveDataHandler.TryGetFile(GetPath(i), out var file))
				{
					List<PartData> partsToSet = file.Load<CarTuningTimer>("TuningTimer").PartsToSet;
					if (!partsToSet.IsNullOrEmpty())
					{
						list.AddRange(partsToSet);
					}
				}
			}
			return list;
		}

		public AdvancedBoolean TryCreateTimer(int slotId, params PartData[] partsToSet)
		{
			if (HasTimerRecord(slotId))
			{
				return new AdvancedBoolean(v: false);
			}
			double currentUnixTimeSeconds = CurrentUnixTimeSeconds;
			List<PartData> list = partsToSet.ToList();
			double time = GetTime(list);
			foreach (PartData item in list)
			{
				if (item != null)
				{
					_inventoryPartsService.Remove(item);
				}
			}
			CarTuningTimer carTuningTimer = new CarTuningTimer
			{
				PartsToSet = list,
				SlotId = slotId,
				TimeOfStart = currentUnixTimeSeconds
			};
			Sign(carTuningTimer, time);
			Save(carTuningTimer);
			return new AdvancedBoolean(v: true);
		}

		public AdvancedBoolean TryCreateTimer(int slotId, EngineKitData engineKit, PartData ecuToSet)
		{
			if (HasTimerRecord(slotId))
			{
				return new AdvancedBoolean(v: false);
			}
			double currentUnixTimeSeconds = CurrentUnixTimeSeconds;
			List<PartData> partsToSet = ((ecuToSet != null) ? new List<PartData> { ecuToSet } : null);
			double time = GetTime(engineKit);
			CarTuningTimer carTuningTimer = new CarTuningTimer
			{
				EngineKitData = engineKit,
				PartsToSet = partsToSet,
				SlotId = slotId,
				TimeOfStart = currentUnixTimeSeconds
			};
			Sign(carTuningTimer, time);
			Save(carTuningTimer);
			return new AdvancedBoolean(v: true);
		}

		public AdvancedBoolean TryCreateTimer(int slotId, EngineKitData engine)
		{
			if (HasTimerRecord(slotId))
			{
				return new AdvancedBoolean(v: false);
			}
			double currentUnixTimeSeconds = CurrentUnixTimeSeconds;
			CarInfo fromSlot = _carInfoService.GetFromSlot(slotId);
			PartData compatibleECU = _inventoryPartsService.GetCompatibleECU(engine.EngineId.ModelId, fromSlot.CarID);
			List<PartData> partsToSet = null;
			if (compatibleECU != null)
			{
				partsToSet = new List<PartData> { compatibleECU };
				_inventoryPartsService.Remove(compatibleECU);
			}
			_engineKitsInventoryService.Remove(engine);
			double time = GetTime(engine);
			CarTuningTimer carTuningTimer = new CarTuningTimer
			{
				EngineKitData = engine,
				PartsToSet = partsToSet,
				SlotId = slotId,
				TimeOfStart = currentUnixTimeSeconds
			};
			Sign(carTuningTimer, time);
			Save(carTuningTimer);
			return new AdvancedBoolean(v: true);
		}

		public TimerInstallResult TryInstallPart(int slotId, PartData part)
		{
			if (part == null)
			{
				return TimerInstallResult.Failed;
			}
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return StartTimerWith(slotId, part);
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			if (carTuningTimer == null)
			{
				return StartTimerWith(slotId, part);
			}
			if (carTuningTimer.EngineKitData != null)
			{
				return TimerInstallResult.BlockedByEngine;
			}
			EnsureIntegrity(carTuningTimer);
			CarTuningTimer carTuningTimer2 = carTuningTimer;
			if (carTuningTimer2.PartsToSet == null)
			{
				carTuningTimer2.PartsToSet = new List<PartData>();
			}
			if (carTuningTimer.PartsToSet.Any((PartData x) => x != null && x.PartType == part.PartType))
			{
				return TimerInstallResult.DuplicateType;
			}
			_inventoryPartsService.Remove(part);
			carTuningTimer.PartsToSet.Add(part);
			Sign(carTuningTimer, ExpectedDuration(carTuningTimer));
			Save(carTuningTimer);
			return TimerInstallResult.Extended;
		}

		private TimerInstallResult StartTimerWith(int slotId, PartData part)
		{
			TryCreateTimer(slotId, part).Get(out var value);
			if (!value)
			{
				return TimerInstallResult.Failed;
			}
			return TimerInstallResult.Started;
		}

		public AdvancedBoolean IsCarBlocked(int slotId)
		{
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return new AdvancedBoolean(v: false);
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			if (carTuningTimer == null)
			{
				return new AdvancedBoolean(v: false);
			}
			EnsureIntegrity(carTuningTimer);
			return new AdvancedBoolean(carTuningTimer.TimeOfEnd > CurrentUnixTimeSeconds);
		}

		private bool HasTimerRecord(int slotId)
		{
			ES3File file;
			return EasySaveDataHandler.TryGetFile(GetPath(slotId), out file);
		}

		private string GetPath(int slotId)
		{
			return "TuningTimers/" + slotId;
		}

		private float GetTimeFromConfig(PartType partType)
		{
			if (_installTimes.TryGet(partType, out var seconds))
			{
				return seconds;
			}
			float num = 5f;
			return _timesToWaitTuningConfig.Times.Find((WaitTimer x) => x.PartType == partType)?.Time ?? num;
		}

		private double GetTime(List<PartData> partsToSet)
		{
			List<float> list = (from x in partsToSet
				where x != null
				select GetTimeFromConfig(x.PartType)).ToList();
			if (list.Count == 0)
			{
				return 5.0;
			}
			return list.Sum();
		}

		public AdvancedBoolean SkipTimer(int slotId, out List<PartData> newPartDatas)
		{
			newPartDatas = null;
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return new AdvancedBoolean(v: false);
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			newPartDatas = carTuningTimer.PartsToSet;
			DeleteTimer(slotId);
			return new AdvancedBoolean(v: true);
		}

		public bool IsTimerCompleted(int slotId, out List<PartData> newPartDatas)
		{
			newPartDatas = null;
			double currentUnixTimeSeconds = CurrentUnixTimeSeconds;
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return false;
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			EnsureIntegrity(carTuningTimer);
			bool num = carTuningTimer.TimeOfEnd <= currentUnixTimeSeconds;
			double num2 = carTuningTimer.TimeOfEnd - carTuningTimer.TimeOfStart;
			float value = ((num2 > 0.0) ? ((float)((carTuningTimer.TimeOfEnd - currentUnixTimeSeconds) / num2)) : 0f);
			value = Mathf.Clamp(value, 0f, 1f);
			value = 1f - value;
			ProgressEvent?.Invoke(TimerEnums.Car, value);
			if (num)
			{
				newPartDatas = carTuningTimer.PartsToSet;
				DeleteTimer(slotId);
			}
			return num;
		}

		private double GetTime(EngineKitData partsToSet)
		{
			if (_installTimes.TryGet(PartType.Engine, out var seconds))
			{
				return seconds;
			}
			return _timesToWaitTuningConfig.Times.Find((WaitTimer x) => x.PartType == PartType.Engine)?.Time ?? 600f;
		}

		private double ExpectedDuration(CarTuningTimer timer)
		{
			double baseDuration = BaseDuration(timer);
			return AdTimerReductionRules.ExpectedDuration(baseDuration, AdTimerReductionRules.ClampReduction(timer.AdReductionSeconds, baseDuration));
		}

		private double BaseDuration(CarTuningTimer timer)
		{
			if (timer.EngineKitData == null)
			{
				return GetTime(timer.PartsToSet ?? new List<PartData>());
			}
			return GetTime(timer.EngineKitData);
		}

		private static List<int> PayloadIds(CarTuningTimer timer)
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
			if (timer.EngineKitData?.EngineId != null)
			{
				list.Add(timer.EngineKitData.EngineId.InstanceID);
				list.Add(timer.EngineKitData.EngineId.ModelId);
			}
			return list;
		}

		private static TuningTimerFacts FactsOf(CarTuningTimer timer, double duration)
		{
			return new TuningTimerFacts(timer.SlotId, timer.TimeOfStart, duration, PayloadIds(timer));
		}

		private static void Sign(CarTuningTimer timer, double duration)
		{
			timer.TimeOfEnd = timer.TimeOfStart + duration;
			string current = TuningTimerSecret.Current;
			if (current != null)
			{
				timer.Signature = TuningTimerSignature.Compute(current, FactsOf(timer, duration));
			}
		}

		private bool EnsureIntegrity(CarTuningTimer timer)
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
					Save(timer);
				}
				return true;
			}
			if (TuningTimerSignature.Verify(timer.Signature, current, FactsOf(timer, num2)))
			{
				timer.TimeOfEnd = timer.TimeOfStart + num2;
				if (flag)
				{
					Save(timer);
				}
				return true;
			}
			timer.TimeOfStart = CurrentUnixTimeSeconds;
			timer.AdReductionSeconds = 0.0;
			Sign(timer, ExpectedDuration(timer));
			Save(timer);
			return false;
		}

		public bool TryGetEngineTimerInfo(int slotId, out EngineTimerInfo info)
		{
			info = default;
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return false;
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			if (carTuningTimer == null)
			{
				return false;
			}
			EnsureIntegrity(carTuningTimer);
			info = new EngineTimerInfo(BuildTimerId(carTuningTimer), Math.Max(0.0, carTuningTimer.TimeOfEnd - CurrentUnixTimeSeconds), AdTimerReductionRules.ClampReduction(carTuningTimer.AdReductionSeconds, BaseDuration(carTuningTimer)), carTuningTimer.EngineKitData != null, CarriesGearBox(carTuningTimer));
			return true;
		}

		public bool TryApplyAdReduction(int slotId, string timerId, double seconds)
		{
			if (seconds <= 0.0)
			{
				return false;
			}
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return false;
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			if (carTuningTimer == null || (carTuningTimer.EngineKitData == null && !CarriesGearBox(carTuningTimer)))
			{
				return false;
			}
			EnsureIntegrity(carTuningTimer);
			if (BuildTimerId(carTuningTimer) != timerId)
			{
				return false;
			}
			double baseDuration = BaseDuration(carTuningTimer);
			double num = AdTimerReductionRules.ClampReduction(carTuningTimer.AdReductionSeconds, baseDuration);
			double num2 = AdTimerReductionRules.ClampReduction(num + seconds, baseDuration);
			if (num2 - num <= 0.0)
			{
				return false;
			}
			carTuningTimer.AdReductionSeconds = num2;
			Sign(carTuningTimer, ExpectedDuration(carTuningTimer));
			Save(carTuningTimer);
			return true;
		}

		public bool TryFinishNow(int slotId, string timerId)
		{
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return false;
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			if (carTuningTimer == null)
			{
				return false;
			}
			EnsureIntegrity(carTuningTimer);
			if (BuildTimerId(carTuningTimer) != timerId)
			{
				return false;
			}
			double num = ExpectedDuration(carTuningTimer);
			carTuningTimer.TimeOfStart = CurrentUnixTimeSeconds - num;
			Sign(carTuningTimer, num);
			Save(carTuningTimer);
			return true;
		}

		public List<TuningTimerEntry> GetTimerEntries(int slotId)
		{
			List<TuningTimerEntry> list = new List<TuningTimerEntry>();
			if (!EasySaveDataHandler.TryGetFile(GetPath(slotId), out var file))
			{
				return list;
			}
			CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
			if (carTuningTimer == null)
			{
				return list;
			}
			EnsureIntegrity(carTuningTimer);
			double num = Math.Max(0.0, carTuningTimer.TimeOfEnd - CurrentUnixTimeSeconds);
			if (carTuningTimer.EngineKitData != null)
			{
				list.Add(new TuningTimerEntry(null, carTuningTimer.EngineKitData, GetTime(carTuningTimer.EngineKitData), num));
				if (carTuningTimer.PartsToSet != null)
				{
					foreach (PartData item in carTuningTimer.PartsToSet)
					{
						if (item != null)
						{
							list.Add(new TuningTimerEntry(item, null, GetTimeFromConfig(item.PartType), num));
						}
					}
				}
				return list;
			}
			if (carTuningTimer.PartsToSet == null)
			{
				return list;
			}
			double num2 = 0.0;
			foreach (PartData item2 in carTuningTimer.PartsToSet)
			{
				if (item2 != null)
				{
					num2 += (double)GetTimeFromConfig(item2.PartType);
				}
			}
			double num3 = Math.Max(0.0, num2 - num);
			double num4 = 0.0;
			foreach (PartData item3 in carTuningTimer.PartsToSet)
			{
				if (item3 != null)
				{
					double num5 = GetTimeFromConfig(item3.PartType);
					num4 += num5;
					double remainingSeconds = Math.Min(num5, Math.Max(0.0, num4 - num3));
					list.Add(new TuningTimerEntry(item3, null, num5, remainingSeconds));
				}
			}
			return list;
		}

		private static bool CarriesGearBox(CarTuningTimer timer)
		{
			if (timer.PartsToSet == null)
			{
				return false;
			}
			return timer.PartsToSet.Any((PartData x) => x != null && x.PartType == PartType.GearBox);
		}

		private static string BuildTimerId(CarTuningTimer timer)
		{
			return $"{timer.SlotId}:{timer.TimeOfStart:F0}";
		}

		private void Save(CarTuningTimer carTuningTimer)
		{
			EasySaveDataHandler.SaveValues(new Dictionary<string, object> { { "TuningTimer", carTuningTimer } }, GetPath(carTuningTimer.SlotId));
			Debug.Log($"Try save timer {carTuningTimer.SlotId}");
		}

		private void DeleteTimer(int slotId, bool returnPartsToInventory = false)
		{
			string path = GetPath(slotId);
			if (returnPartsToInventory && EasySaveDataHandler.TryGetFile(path, out var file))
			{
				CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
				if (carTuningTimer.PartsToSet != null)
				{
					foreach (PartData item in carTuningTimer.PartsToSet)
					{
						if (item != null)
						{
							_inventoryPartsService.Add(item);
						}
					}
				}
				if (carTuningTimer.EngineKitData != null)
				{
					_engineKitsInventoryService.Add(carTuningTimer.EngineKitData);
				}
			}
			EasySaveDataHandler.DeleteFile(path);
		}

		private void RemoveCarTimer(int slotId)
		{
			DeleteTimer(slotId, returnPartsToInventory: true);
		}

		private void DisposeTimer()
		{
			_timerCheck?.Dispose();
		}

		private void CheckCar()
		{
			if (IsPause)
			{
				return;
			}
			ObscuredInt slotID = V.SlotID;
			if ((int)slotID < 0)
			{
				return;
			}
			if (!_carInfoService.TryGetFromSlot(slotID, out var carInfo))
			{
				DeleteTimer(slotID, returnPartsToInventory: true);
				HideEvent?.Invoke(obj: true);
				return;
			}
			List<PartData> list = null;
			EngineKitData engineKitData = null;
			double currentUnixTimeSeconds = CurrentUnixTimeSeconds;
			string path = GetPath(slotID);
			bool flag;
			if (!EasySaveDataHandler.TryGetFile(path, out var file))
			{
				if (prevSlotId != (int)slotID)
				{
					HideEvent?.Invoke(obj: true);
				}
				flag = false;
			}
			else
			{
				CarTuningTimer carTuningTimer = file.Load<CarTuningTimer>("TuningTimer");
				EnsureIntegrity(carTuningTimer);
				if (currentUnixTimeSeconds < carTuningTimer.TimeOfStart)
				{
					EasySaveDataHandler.DeleteFile(path);
					carTuningTimer.TimeOfStart = currentUnixTimeSeconds;
					Sign(carTuningTimer, ExpectedDuration(carTuningTimer));
					Save(carTuningTimer);
				}
				bool num = carTuningTimer.TimeOfEnd <= currentUnixTimeSeconds;
				double num2 = carTuningTimer.TimeOfEnd - carTuningTimer.TimeOfStart;
				float value = ((num2 > 0.0) ? ((float)((carTuningTimer.TimeOfEnd - currentUnixTimeSeconds) / num2)) : 0f);
				value = Mathf.Clamp(value, 0f, 1f);
				value = 1f - value;
				ProgressEvent?.Invoke(TimerEnums.Car, value);
				if (num)
				{
					if (carTuningTimer.PartsToSet != null)
					{
						list = carTuningTimer.PartsToSet;
					}
					if (carTuningTimer.EngineKitData != null)
					{
						engineKitData = carTuningTimer.EngineKitData;
					}
					HideEvent?.Invoke(obj: false);
				}
				flag = num;
			}
			prevSlotId = slotID;
			if (!flag || !V.Car)
			{
				return;
			}
			if (engineKitData != null)
			{
				PartData ecu = null;
				if (!list.IsNullOrEmpty())
				{
					ecu = list.FirstOrDefault((PartData x) => x.PartType == PartType.ECU);
				}
				_carTuningFactory.SetNewEngineInCar(carInfo, engineKitData, ecu);
			}
			else if (!list.IsNullOrEmpty())
			{
				_carTuningFactory.SetPartsInCar(carInfo, list.ToArray());
			}
			DeleteTimer(slotID);
			GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQDA", "engine_tuning");
			if (_steamAchievementService != null)
			{
				_steamAchievementService.Unlock("ACH_FIRST_WRENCH_TURN");
			}
		}
	}
}
