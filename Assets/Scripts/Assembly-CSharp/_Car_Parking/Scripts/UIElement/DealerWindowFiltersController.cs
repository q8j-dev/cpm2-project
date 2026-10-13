using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Balance;
using CPM.Database;
using CPMEngine;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.UIElement
{
	public class DealerWindowFiltersController : MonoBehaviour
	{
		[SerializeField]
		private NewCarsFilterElement[] FilterElements;

		[SerializeField]
		private Toggle AllCars;

		[SerializeField]
		private Toggle SpecialCars;

		private AllCarsInfo _allCarsInfo;

		private CarFilter _carFilter;

		private CarAvailabilityService _carAvailabilityService;

		private CarPriceHelperV2 _carPriceHelper;

		private IPurchaseSystem _purchaseSystem;

		private bool _waitsForStore;

		public event Action OnFilterChanged;

		[Inject]
		public void Construct(AllCarsInfo allCarsInfo, IOfferSystem offerSystem, CarAvailabilityService carAvailabilityService, CarPriceHelperV2 carPriceHelper, IPurchaseSystem purchaseSystem)
		{
			_allCarsInfo = allCarsInfo;
			_carFilter = new CarFilter(_allCarsInfo.AllCars, offerSystem);
			_carAvailabilityService = carAvailabilityService;
			_carAvailabilityService.OnChanged += CarAvailabilityChangedCallback;
			_carPriceHelper = carPriceHelper;
			_purchaseSystem = purchaseSystem;
			if (!_purchaseSystem.IsInitialized)
			{
				_purchaseSystem.OnInitializedCompletedEvent += StoreInitializedCallback;
				_waitsForStore = true;
			}
		}

		private void OnDestroy()
		{
			if (_carAvailabilityService != null)
			{
				_carAvailabilityService.OnChanged -= CarAvailabilityChangedCallback;
			}
			UnsubscribeFromStore();
		}

		private void StoreInitializedCallback()
		{
			UnsubscribeFromStore();
			OnFilterChanged?.Invoke();
		}

		private void UnsubscribeFromStore()
		{
			if (_waitsForStore)
			{
				_waitsForStore = false;
				if (_purchaseSystem != null)
				{
					_purchaseSystem.OnInitializedCompletedEvent -= StoreInitializedCallback;
				}
			}
		}

		public List<VehicleStockTuningData> GetCars()
		{
			NewCarsFilterElement isOnFilterElement = GetIsOnFilterElement(DealerWindow.FilterType.CarType);
			NewCarsFilterElement isOnFilterElement2 = GetIsOnFilterElement(DealerWindow.FilterType.CarRegion);
			NewCarsFilterElement isOnFilterElement3 = GetIsOnFilterElement(DealerWindow.FilterType.CarClass);
			bool specialTab = SpecialCars.isOn;
			List<VehicleStockTuningData> list = (from car in _carFilter.CreateFilter(isOnFilterElement2 ? new CarRegion?(isOnFilterElement2.Region) : ((CarRegion?)null), isOnFilterElement ? new CarType?(isOnFilterElement.CarType) : ((CarType?)null), isOnFilterElement3 ? new VehicleClass?(isOnFilterElement3.VehicleClass) : ((VehicleClass?)null), specialTab)()
				where _carAvailabilityService.IsVisibleInStore(car.Serial)
				orderby (!specialTab) ? SoftPriceOrder(car) : RealPriceOrder(car)
				select car).ToList();
			list.RemoveAll((VehicleStockTuningData car) => CarIdStorage.IsRewardExclusiveCar(car.Serial) || CarIdStorage.IsSteamExcludedCar(car.Serial));
			return list;
		}

		private CarShopOrder SoftPriceOrder(VehicleStockTuningData car)
		{
			car.GetPriceUI.Get(out int value);
			car.GetPriceTypeUI.Get(out var value2);
			return CarShopOrder.ForSoftPrice(car.Serial, value2, value);
		}

		private CarShopOrder RealPriceOrder(VehicleStockTuningData car)
		{
			RealСurrencyPrice item = _carPriceHelper.GetRealСurrencyPrice(car.Serial).Item1;
			return CarShopOrder.ForRealPrice(car.Serial, item?.Price);
		}

		private void CarAvailabilityChangedCallback()
		{
			OnFilterChanged?.Invoke();
		}

		public void ShowOnlySpecial()
		{
			SpecialCars.SetIsOnWithoutNotify(value: true);
			SpecialCarValueUpdated(state: true);
		}

		public void ShowAll()
		{
			AllCars.SetIsOnWithoutNotify(value: true);
		}

		public NewCarsFilterElement GetIsOnFilterElement(DealerWindow.FilterType filterType)
		{
			for (int i = 0; i < FilterElements.Length; i++)
			{
				if (FilterElements[i].FilterType == filterType && FilterElements[i].IsOn)
				{
					return FilterElements[i];
				}
			}
			return null;
		}

		private void OnEnable()
		{
			AllCars.onValueChanged.AddListener(OnAllCarValueChangedCallback);
			SpecialCars.onValueChanged.AddListener(OnSpecialCarValueChangedCallback);
			for (int i = 0; i < FilterElements.Length; i++)
			{
				FilterElements[i].OnToggleChanged += OnToggleChangedCallback;
			}
		}

		private void OnDisable()
		{
			AllCars.onValueChanged.RemoveListener(OnAllCarValueChangedCallback);
			SpecialCars.onValueChanged.RemoveListener(OnSpecialCarValueChangedCallback);
			for (int i = 0; i < FilterElements.Length; i++)
			{
				FilterElements[i].OnToggleChanged -= OnToggleChangedCallback;
			}
		}

		private void OnToggleChangedCallback(NewCarsFilterElement filterElement, bool state)
		{
			for (int i = 0; i < FilterElements.Length; i++)
			{
				if (filterElement != FilterElements[i] && FilterElements[i].FilterType == filterElement.FilterType)
				{
					FilterElements[i].IsOn = false;
				}
			}
			bool flag = false;
			for (int j = 0; j < FilterElements.Length; j++)
			{
				if (FilterElements[j].IsOn)
				{
					flag = true;
					break;
				}
			}
			if (SpecialCars.isOn)
			{
				flag = true;
			}
			AllCars.SetIsOnWithoutNotify(!flag);
			OnFilterChanged?.Invoke();
		}

		private void OnAllCarValueChangedCallback(bool state)
		{
			if (state)
			{
				for (int i = 0; i < FilterElements.Length; i++)
				{
					FilterElements[i].IsOn = false;
				}
				SpecialCars.SetIsOnWithoutNotify(value: false);
			}
			else
			{
				AllCars.SetIsOnWithoutNotify(value: true);
			}
			OnFilterChanged?.Invoke();
		}

		private void OnSpecialCarValueChangedCallback(bool state)
		{
			SpecialCarValueUpdated(state);
			OnFilterChanged?.Invoke();
		}

		private void SpecialCarValueUpdated(bool state)
		{
			if (state)
			{
				AllCars.SetIsOnWithoutNotify(value: false);
			}
			else
			{
				OnFilterChanged?.Invoke();
			}
		}
	}
}
