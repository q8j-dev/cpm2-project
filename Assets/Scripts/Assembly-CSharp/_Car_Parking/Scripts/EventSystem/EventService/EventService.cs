using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CPM.Database;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService.Processes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Notification;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.EventSystem.EventService
{
	[Preserve]
	public class EventService : IEventService, IInitializable, IDisposable
	{
		public const string GetAllCurrentEventsFn = "GetAllCurrentEvents24_1";

		public const string ClaimRewardFn = "ClaimEventReward24_1";

		public const string SubmitEventRecordFn = "SubmitEventRecord24_1";

		private readonly EventStorage _eventStorage;

		private readonly PlayerDataStorage _playerDataStorage;

		private readonly EventsAnalytic _eventsAnalytic;

		private readonly INotificationService _notificationService;

		private readonly AllCarsInfo _allCarsInfo;

		private readonly CommonProvider _commonProvider;

		private readonly CachedEventStorage _cachedEventStorage = new CachedEventStorage();

		private readonly UniTaskCompletionSource _initializedTcs = new UniTaskCompletionSource();

		private Dictionary<string, RewardProcessor> _rewardProcessors;

		private Task<List<_Car_Parking.Scripts.EventSystem.Data.Event>> _currentEventsTask;

		private List<_Car_Parking.Scripts.EventSystem.Data.Event> _currentEvents;

		public UniTask Initialized => _initializedTcs?.Task ?? UniTask.CompletedTask;

		public IReadOnlyList<_Car_Parking.Scripts.EventSystem.Data.Event> CurrentEvents
		{
			get
			{
				IReadOnlyList<_Car_Parking.Scripts.EventSystem.Data.Event> currentEvents = _currentEvents;
				return currentEvents ?? Array.Empty<_Car_Parking.Scripts.EventSystem.Data.Event>();
			}
		}

		private Dictionary<string, RewardProcessor> RewardProcessors
		{
			get
			{
				Dictionary<string, RewardProcessor> dictionary = _rewardProcessors;
				if (dictionary == null)
				{
					Dictionary<string, RewardProcessor> obj = new Dictionary<string, RewardProcessor>
					{
						{
							"car",
							new CarProcessor(_eventsAnalytic, _commonProvider, _allCarsInfo)
						},
						{
							"suit",
							new SuitProcessor(_eventsAnalytic, _notificationService, _commonProvider)
						},
						{
							"message",
							new MessageProcessor(_eventsAnalytic)
						}
					};
					Dictionary<string, RewardProcessor> dictionary2 = obj;
					_rewardProcessors = obj;
					dictionary = dictionary2;
				}
				return dictionary;
			}
		}

		public event Action<EventType> OnEventClaimed;

		public event Action OnEventsRetrieved;

		public event Action<EventType> OnEventUpdated;

		public EventService(EventStorage eventStorage, PlayerDataStorage playerDataStorage, EventsAnalytic eventsAnalytic, INotificationService notificationService, AllCarsInfo allCarsInfo, CommonProvider commonProvider)
		{
			_eventStorage = eventStorage;
			_playerDataStorage = playerDataStorage;
			_eventsAnalytic = eventsAnalytic;
			_notificationService = notificationService;
			_allCarsInfo = allCarsInfo;
			_commonProvider = commonProvider;
		}

		public void Initialize()
		{
			InitializeAsync().Forget();
		}

		private async UniTask InitializeAsync()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(1.0));
			_initializedTcs.TrySetResult();
		}

		public async UniTask<List<_Car_Parking.Scripts.EventSystem.Data.Event>> RetrieveCurrentEvents(bool forceRefresh = false)
		{
			if (!forceRefresh && _currentEvents != null)
			{
				return _currentEvents;
			}
			if (_currentEventsTask != null && !_currentEventsTask.IsCompleted)
			{
				return await _currentEventsTask;
			}
			_currentEventsTask = InternalRetrieveCurrentEvents().AsTask();
			_currentEvents = await _currentEventsTask;
			_currentEventsTask = null;
			CorrectEvents();
			OnEventsRetrieved?.Invoke();
			return _currentEvents ?? new List<_Car_Parking.Scripts.EventSystem.Data.Event>();
		}

		private async UniTask<List<_Car_Parking.Scripts.EventSystem.Data.Event>> InternalRetrieveCurrentEvents()
		{
			if (string.IsNullOrEmpty(_playerDataStorage.AccountID) || _playerDataStorage.AccountID == (ObscuredString)"1" || _playerDataStorage.AccountID == (ObscuredString)"0")
			{
				return null;
			}
			FirebaseWrappedFunction firebaseWrappedFunction = new FirebaseWrappedFunctionFactory().Create("GetAllCurrentEvents24_1");
			GetEventRequest getEventRequest = new GetEventRequest
			{
				Version = Application.version,
				Platform = DefinePlatform()
			};
			FunctionCallResult<List<EventResponse>> functionCallResult = await firebaseWrappedFunction.CallAsyncResult<List<EventResponse>>(JsonConvert.SerializeObject(getEventRequest));
			if (functionCallResult.IsFail)
			{
				return null;
			}
			return BuildEvents(functionCallResult.Data);
		}

		private List<_Car_Parking.Scripts.EventSystem.Data.Event> BuildEvents(List<EventResponse> responses)
		{
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = new List<_Car_Parking.Scripts.EventSystem.Data.Event>();
			if (responses == null)
			{
				return list;
			}
			foreach (EventResponse response in responses)
			{
				if (response?.Data != null && _eventStorage.DefineEvent(response.Data.Type, out var eventType))
				{
					eventType = ResolvePromoType(response.Data, eventType);
					EventConfig eventConfig = _eventStorage.EventsConfigs.FirstOrDefault((EventConfig x) => x.Type == eventType);
					if (eventConfig != null)
					{
						list.Add(new _Car_Parking.Scripts.EventSystem.Data.Event
						{
							Id = response.Id,
							Type = eventType,
							NameLocalizeKey = eventConfig.NameKey,
							DescriptionLocalizeKey = eventConfig.DescriptionKey,
							Progress = response.Progress,
							Status = (EventStatus)response.Status,
							EndTime = response.EndTime,
							MaxProgress = eventConfig.MaxProgress,
							UnlockCarId = eventConfig.UnlockCarId,
							Data = response.Data,
							Images = response.Images
						});
					}
				}
			}
			return list;
		}

		private EventType ResolvePromoType(EvenDataBase data, EventType mappedType)
		{
			if (!(data is PromoEventData promoEventData))
			{
				return mappedType;
			}
			if (!promoEventData.Hyperlink)
			{
				return EventType.PromoSteamWishList;
			}
			return EventType.PromoSteamPurchase;
		}

		public void CacheCurrentEvents()
		{
			if (_currentEvents == null)
			{
				return;
			}
			foreach (_Car_Parking.Scripts.EventSystem.Data.Event currentEvent in _currentEvents)
			{
				_cachedEventStorage.CacheEvent(currentEvent);
			}
		}

		public void CacheEventByName(EventType eventType)
		{
			_Car_Parking.Scripts.EventSystem.Data.Event obj = _currentEvents?.FirstOrDefault((_Car_Parking.Scripts.EventSystem.Data.Event x) => x.Type == eventType);
			if (obj != null)
			{
				_cachedEventStorage.CacheEvent(obj);
			}
		}

		public void RemoveCacheEventByName(EventType eventType)
		{
			_Car_Parking.Scripts.EventSystem.Data.Event obj = _currentEvents?.FirstOrDefault((_Car_Parking.Scripts.EventSystem.Data.Event x) => x.Type == eventType);
			if (obj != null)
			{
				_cachedEventStorage.RemoveCachedEvent(obj.Type);
			}
		}

		public async UniTask<List<_Car_Parking.Scripts.EventSystem.Data.Event>> CheckForUncachedEvents(bool forceRefresh = false)
		{
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = await RetrieveCurrentEvents(forceRefresh);
			if (list == null)
			{
				return null;
			}
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list2 = new List<_Car_Parking.Scripts.EventSystem.Data.Event>(list);
			for (int num = list2.Count - 1; num >= 0; num--)
			{
				if (_cachedEventStorage.CachedEventExists(list2[num].Type))
				{
					list2.Remove(list2[num]);
				}
			}
			return list2;
		}

		public async UniTask<List<_Car_Parking.Scripts.EventSystem.Data.Event>> CheckForFinishedEvents()
		{
			List<_Car_Parking.Scripts.EventSystem.Data.Event> obj = await RetrieveCurrentEvents(forceRefresh: true);
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = new List<_Car_Parking.Scripts.EventSystem.Data.Event>();
			foreach (_Car_Parking.Scripts.EventSystem.Data.Event item in obj)
			{
				_Car_Parking.Scripts.EventSystem.Data.Event cachedEventDataById = _cachedEventStorage.GetCachedEventDataById(item.Type);
				if (cachedEventDataById != null && cachedEventDataById.Status < item.Status)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public _Car_Parking.Scripts.EventSystem.Data.Event GetEventByName(EventType eventType)
		{
			if (_currentEvents != null)
			{
				foreach (_Car_Parking.Scripts.EventSystem.Data.Event currentEvent in _currentEvents)
				{
					if (currentEvent.Type == eventType)
					{
						return currentEvent;
					}
				}
			}
			return null;
		}

		public bool TryUpdateEvent(EventType eventType, Action<_Car_Parking.Scripts.EventSystem.Data.Event> mutate)
		{
			if (mutate == null)
			{
				return false;
			}
			_Car_Parking.Scripts.EventSystem.Data.Event eventByName = GetEventByName(eventType);
			if (eventByName == null)
			{
				return false;
			}
			mutate(eventByName);
			OnEventUpdated?.Invoke(eventType);
			return true;
		}

		public async UniTask<int> SubmitEventRecord(EventType eventType, PayloadData data)
		{
			if (!_currentEvents.Exists((_Car_Parking.Scripts.EventSystem.Data.Event x) => eventType.Equals(x.Type)))
			{
				return 0;
			}
			string text = _currentEvents.FirstOrDefault((_Car_Parking.Scripts.EventSystem.Data.Event x) => x.Type == eventType)?.Id;
			if (!string.IsNullOrEmpty(text))
			{
				EventPayload value = new EventPayload
				{
					EventId = text,
					Data = data
				};
				return (await new FirebaseWrappedFunctionFactory().Create("SubmitEventRecord24_1").CallAsyncResult<EmptyClass>(WalletCypher.Encode(JsonConvert.SerializeObject(value), ""))).IsSuccess ? 1 : 0;
			}
			return 0;
		}

		public async UniTask<ClaimRewardResult> ClaimReward(EventType eventType)
		{
			V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true).Forget();
			RewardResponseBase[] granted = null;
			try
			{
				if (_eventStorage.EventsConfigs.FirstOrDefault((EventConfig x) => x.Type == eventType) != null)
				{
					string id = _currentEvents.FirstOrDefault((_Car_Parking.Scripts.EventSystem.Data.Event x) => x.Type == eventType)?.Id;
					FunctionCallResult<RewardResponseBase[]> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("ClaimEventReward24_1").CallAsyncResult<RewardResponseBase[]>(id, 1, new Dictionary<string, string> { ["X-Client-Platform"] = "STEAM" });
					if (functionCallResult.Status == FunctionStatus.EventsNoSlots)
					{
						_eventsAnalytic.LogClaimReward(id, rewardRecieved: false);
						ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new CombinedPrice(VehicleSlotPaymentService.Price, 0));
						string text = new LocalizedString("SlotUI.DoYouWantToBuyANewSlotForSlots");
						V.CoreGameManager.ManagerUI.ShowConfirmWindow(() =>
						{
							_commonProvider.SlotService.TryBuySlot();
						}, new LocalizedString("SlotUI.SlotPurchase"), text.Replace("{0}", VehicleSlotPaymentService.Price.ToString()), null, "Confirm", null, additionalData).Forget();
						return ClaimRewardResult.Failed;
					}
					if (functionCallResult.IsFail)
					{
						return ClaimRewardResult.FailedWith((int)functionCallResult.Status);
					}
					RewardResponseBase[] rewards = functionCallResult.Data;
					if (rewards == null)
					{
						V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f, NotificationType.Error).Forget();
						return ClaimRewardResult.Failed;
					}
					granted = rewards;
					List<UniTask> list = new List<UniTask>();
					RewardResponseBase[] array = rewards;
					foreach (RewardResponseBase responseBase in array)
					{
						list.Add(ProcessReward(responseBase, id));
					}
					await UniTask.WhenAll(list);
					if (rewards.Any((RewardResponseBase x) => x.Type == "suit"))
					{
						_commonProvider.CharactersService.Save();
						if (_commonProvider.DataBase != null)
						{
							await _commonProvider.DataBase.SendAllRecords(saveCars: false);
						}
					}
					TryUpdateEvent(eventType, ApplyClaimedState);
					if (!rewards.Any((RewardResponseBase x) => x is RewardCarResponse rewardCarResponse && CarIdStorage.IsRewardExclusiveCar(rewardCarResponse.CarId)) && rewards.Any((RewardResponseBase x) => x.Type != "message"))
					{
						V.CoreGameManager.ManagerUI.ShowNotificationWindow(new LocalizedString("EventService.RewardRecieved"), 3f).Forget();
						_ = RateAppHelper.RateAppAsync();
					}
				}
			}
			finally
			{
				V.CoreGameManager.ManagerUI.CloseWaitingWindow();
			}
			OnEventClaimed?.Invoke(eventType);
			return new ClaimRewardResult(success: true, granted);
		}

		private static void ApplyClaimedState(_Car_Parking.Scripts.EventSystem.Data.Event claimedEvent)
		{
			if (claimedEvent.Status == EventStatus.Finished)
			{
				claimedEvent.Status = EventStatus.Claimed;
			}
			if (claimedEvent.Data is ObstacleRaceEventData obstacleRaceEventData)
			{
				obstacleRaceEventData.Claimable = Math.Max(0, obstacleRaceEventData.Claimable - 1);
				obstacleRaceEventData.Unclaimed = Math.Max(0, obstacleRaceEventData.Unclaimed - 1);
			}
		}

		public async UniTask WaitUntilInitializedAsync()
		{
			await Initialized;
		}

		public void Reset()
		{
			_currentEvents = null;
		}

		private async UniTask ProcessReward(RewardResponseBase responseBase, string eventId)
		{
			await RewardProcessors[responseBase.Type].Execute(responseBase, eventId);
		}

		private string DefinePlatform()
		{
			return "Steam";
		}

		public void Dispose()
		{
			_currentEventsTask?.Dispose();
		}

		private void CorrectEvents()
		{
			if (_currentEvents == null)
			{
				return;
			}
			for (int i = 0; i < _currentEvents.Count; i++)
			{
				if (_currentEvents[i] == null || _currentEvents[i].Data == null || !(_currentEvents[i].Data is CollectablesEventData))
				{
					continue;
				}
				CollectablesEventData collectablesEventData = (CollectablesEventData)_currentEvents[i].Data;
				if (collectablesEventData.Items == null)
				{
					collectablesEventData.Items = new List<int>();
				}
				if (_currentEvents[i].Status != EventStatus.Claimed && _currentEvents[i].Status != EventStatus.Finished)
				{
					continue;
				}
				EventConfig eventConfig = _eventStorage.GetEventConfig(_currentEvents[i].Type);
				if (eventConfig != null)
				{
					collectablesEventData.ItemCount = eventConfig.MaxProgress;
					collectablesEventData.Items.Clear();
					for (int j = 0; j < eventConfig.MaxProgress; j++)
					{
						collectablesEventData.Items.Add(j);
					}
				}
			}
		}
	}
}
