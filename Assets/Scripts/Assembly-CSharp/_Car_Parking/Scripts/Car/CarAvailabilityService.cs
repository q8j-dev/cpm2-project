using System;
using System.Collections.Generic;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Car
{
	[Preserve]
	public class CarAvailabilityService : IInitializable, IDisposable
	{
		private readonly IEventService _eventService;

		private readonly CarShopVisualService _carShopVisualService;

		private readonly CarShopPurchaseService _carShopPurchaseService;

		private readonly ManagerUI _managerUI;

		private readonly Dictionary<int, EventType> _unlocks = new Dictionary<int, EventType>();

		private EventSteamBuyCarVisualUpdater _steamBuyCarVisualUpdater;

		private EventSteamCarPurchaseHandler _steamCarPurchaseHandler;

		public event Action OnChanged;

		public CarAvailabilityService(IEventService eventService, CarShopVisualService carShopVisualService, CarShopPurchaseService carShopPurchaseService, ManagerUI managerUI)
		{
			_eventService = eventService;
			_carShopVisualService = carShopVisualService;
			_carShopPurchaseService = carShopPurchaseService;
			_managerUI = managerUI;
		}

		public void Initialize()
		{
			_eventService.OnEventsRetrieved += Rebuild;
			if (_eventService.CurrentEvents.Count > 0)
			{
				Rebuild();
			}
		}

		public void Dispose()
		{
			_eventService.OnEventsRetrieved -= Rebuild;
			UnregisterSteamBuyCar();
		}

		public bool IsVisibleInStore(int carId)
		{
			if (CarIdStorage.IsCarLockedOnThisPlatform(carId))
			{
				return _unlocks.ContainsKey(carId);
			}
			return true;
		}

		public bool IsCarUnlockedByEvent(int carId, EventType eventType)
		{
			if (_unlocks.TryGetValue(carId, out var value))
			{
				return value == eventType;
			}
			return false;
		}

		public bool TryGetUnlockEvent(int carId, out EventType eventType)
		{
			return _unlocks.TryGetValue(carId, out eventType);
		}

		private void Rebuild()
		{
			Dictionary<int, EventType> before = new Dictionary<int, EventType>(_unlocks);
			bool flag = _steamBuyCarVisualUpdater != null;
			_unlocks.Clear();
			IReadOnlyList<Event> currentEvents = _eventService.CurrentEvents;
			bool flag2 = false;
			for (int i = 0; i < currentEvents.Count; i++)
			{
				Event obj = currentEvents[i];
				if (obj != null)
				{
					if (obj.Type == EventType.PromoSteamPurchase)
					{
						flag2 = true;
					}
					if (obj.UnlockCarId > 0)
					{
						_unlocks[obj.UnlockCarId] = obj.Type;
					}
				}
			}
			if (flag2)
			{
				RegisterSteamBuyCar();
			}
			else
			{
				UnregisterSteamBuyCar();
			}
			if (flag != (_steamBuyCarVisualUpdater != null) || !SameUnlocks(before))
			{
				OnChanged?.Invoke();
			}
		}

		private bool SameUnlocks(Dictionary<int, EventType> before)
		{
			if (before.Count != _unlocks.Count)
			{
				return false;
			}
			foreach (KeyValuePair<int, EventType> unlock in _unlocks)
			{
				if (!before.TryGetValue(unlock.Key, out var value) || value != unlock.Value)
				{
					return false;
				}
			}
			return true;
		}

		private void RegisterSteamBuyCar()
		{
			if (_steamBuyCarVisualUpdater == null)
			{
				_steamBuyCarVisualUpdater = new EventSteamBuyCarVisualUpdater(this);
				_carShopVisualService.Register(_steamBuyCarVisualUpdater);
			}
			if (_steamCarPurchaseHandler == null)
			{
				_steamCarPurchaseHandler = new EventSteamCarPurchaseHandler(this, _managerUI);
				_carShopPurchaseService.Register(_steamCarPurchaseHandler);
			}
		}

		private void UnregisterSteamBuyCar()
		{
			if (_steamBuyCarVisualUpdater != null)
			{
				_carShopVisualService.Unregister(_steamBuyCarVisualUpdater);
				_steamBuyCarVisualUpdater = null;
			}
			if (_steamCarPurchaseHandler != null)
			{
				_carShopPurchaseService.Unregister(_steamCarPurchaseHandler);
				_steamCarPurchaseHandler = null;
			}
		}
	}
}
