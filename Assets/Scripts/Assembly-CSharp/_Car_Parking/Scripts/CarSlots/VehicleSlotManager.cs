using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CPM.Account;
using CPM.Scripts.Garage;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace _Car_Parking.Scripts.CarSlots
{
	public class VehicleSlotManager : ISlotService, IDisposable, IDeleteUserData
	{
		public const int GarageCount = 2;

		public const string ForceSaveSlotsPrefsKey = "ForceSaveSlots";

		private readonly IStorageService _storageService;

		private readonly CurrencyManagerController _currencyManagerController;

		private readonly VehicleSlotPaymentService _vehicleSlotPaymentService;

		private readonly AnalyticEventFactory _analyticEventFactory;

		private readonly IDeleterUserDataGetter _deleterUserDataGetter;

		private List<SlotCollection> _slotCollections;

		private GaragePositionsConfig _garagePositionsConfig;

		private IGarageSelectorService _garageSelectorService;

		private IGarageBuyService _garageBuyService;

		public int CurrentGarageIndex => _garageSelectorService.CurrentGarageIndex;

		public SlotCollection SlotCollection => GetSlotCollection(CurrentGarageIndex);

		public List<CarSlot> AllSlotsInfos
		{
			get
			{
				List<CarSlot> list = new List<CarSlot>();
				for (int i = 0; i < _slotCollections.Count; i++)
				{
					SlotCollection slotCollection = _slotCollections[i];
					for (int j = 0; j < _garagePositionsConfig.GaragePositionsDatas[i].MaxCarSpotCount; j++)
					{
						list.Add(slotCollection[j]);
					}
				}
				return list;
			}
		}

		public int MaxSlotCount => _garagePositionsConfig.GaragePositionsDatas[CurrentGarageIndex].MaxCarSpotCount;

		public int OccupiedSlotCount => SlotCollection.OccupiedSlotCount;

		public int SlotCount => SlotCollection.Count;

		public bool AllGaragesFull
		{
			get
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < _slotCollections.Count; i++)
				{
					num += _slotCollections[i].OccupiedSlotCount;
					num2 += _slotCollections[i].MaxCarSpotCount;
				}
				return num >= num2;
			}
		}

		public int OccupiedSlotCountInAllGarages
		{
			get
			{
				int num = 0;
				for (int i = 0; i < _slotCollections.Count; i++)
				{
					num += _slotCollections[i].OccupiedSlotCount;
				}
				return num;
			}
		}

		public int SlotsCountInAllGarages
		{
			get
			{
				int num = 0;
				for (int i = 0; i < _slotCollections.Count; i++)
				{
					if (_slotCollections[i].IsGarageBought)
					{
						num += (int)_slotCollections[i].Count;
					}
				}
				return num;
			}
		}

		public bool HaveCarInAnyGarage => OccupiedSlotCountInAllGarages > 0;

		public event Action<int> OnSlotCreated;

		public event Action<int> OnSlotChanged;

		public event Action<int> OnSlotRemoved;

		[Inject]
		public VehicleSlotManager(IStorageService storageService, CurrencyManagerController currencyManagerController, GaragePositionsConfig garagePositionsConfig, VehicleSlotPaymentService vehicleSlotPaymentService, IGarageSelectorService garageSelectorService, IGarageBuyService garageBuyService, AnalyticEventFactory analyticEventFactory, IDeleterUserDataGetter deleterUserDataGetter)
		{
			_vehicleSlotPaymentService = vehicleSlotPaymentService;
			_garagePositionsConfig = garagePositionsConfig;
			_currencyManagerController = currencyManagerController;
			_storageService = storageService;
			_garageSelectorService = garageSelectorService;
			_garageBuyService = garageBuyService;
			_analyticEventFactory = analyticEventFactory;
			_deleterUserDataGetter = deleterUserDataGetter;
			BuildEmptySlotCollections();
			_deleterUserDataGetter.Add(this);
		}

		public void Dispose()
		{
			_deleterUserDataGetter?.Remove(this);
		}

		public void DeleteData()
		{
			BuildEmptySlotCollections();
		}

		private void BuildEmptySlotCollections()
		{
			_slotCollections = new List<SlotCollection>();
			for (int i = 0; i < 2; i++)
			{
				_slotCollections.Add(new SlotCollection(_garagePositionsConfig, i, _garageBuyService));
			}
		}

		public SlotCollection GetSlotCollection(int garageIndex)
		{
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				if (_slotCollections[i].Index == garageIndex)
				{
					return _slotCollections[i];
				}
			}
			SlotCollection slotCollection = new SlotCollection(_garagePositionsConfig, garageIndex, _garageBuyService);
			_slotCollections.Add(slotCollection);
			return slotCollection;
		}

		public SlotCollection GetSlotCollectionBySlotId(int slotId)
		{
			return GetSlotCollection(slotId / 1000);
		}

		public int GetNotEmptySlotId()
		{
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				int firstNotEmptySlot = _slotCollections[(i + CurrentGarageIndex) % 2].GetFirstNotEmptySlot();
				if (firstNotEmptySlot >= 0)
				{
					return firstNotEmptySlot;
				}
			}
			return -1;
		}

		public List<int> GetCurrentGarageOccupiedSlotIds()
		{
			List<int> list = new List<int>();
			SlotCollection slotCollection = _slotCollections[CurrentGarageIndex];
			for (int i = 0; i < (int)slotCollection.Count; i++)
			{
				int changedSlotID = slotCollection.GetChangedSlotID(i);
				if (!slotCollection[i].IsEmpty)
				{
					list.Add(changedSlotID);
				}
			}
			return list;
		}

		public List<int> GetAllSlots()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				SlotCollection slotCollection = _slotCollections[(i + CurrentGarageIndex) % 2];
				for (int j = 0; j < (int)slotCollection.Count; j++)
				{
					int changedSlotID = slotCollection.GetChangedSlotID(j);
					if (!slotCollection[j].IsEmpty)
					{
						list.Add(changedSlotID);
					}
				}
			}
			return list;
		}

		public void ResizeCollection()
		{
		}

		public SlotsOnDatabaseCollection GetSlotsForDatabase()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				foreach (KeyValuePair<int, string> item in GetSlotCollection(i).GetSlotCollectionForDatabase())
				{
					dictionary.Add(item.Key, item.Value);
				}
			}
			if (PlayerPrefs.GetInt("ForceSaveSlots", 0) == 1)
			{
				dictionary.Add(9999, "");
			}
			return new SlotsOnDatabaseCollection(dictionary);
		}

		public void DeleteAllSlots()
		{
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				_slotCollections[i].DeleteAllSlots();
			}
		}

		public void SetSlotsFromDatabase(SlotsOnDatabaseCollection recordsSlotsOnDatabaseCollection)
		{
			int[] array = new int[_slotCollections.Count];
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				array[i] = _slotCollections[i].LastSlotId;
			}
			DeleteAllSlots();
			foreach (KeyValuePair<int, string> slot in recordsSlotsOnDatabaseCollection.Slots)
			{
				try
				{
					if (slot.Key != 9999)
					{
						GetSlotCollectionBySlotId(slot.Key).SetSlot(slot.Key, slot.Value, canAddNewSlot: true);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("SetSlot:" + ex);
				}
			}
			for (int j = 0; j < _slotCollections.Count; j++)
			{
				int num = array[j];
				if (!_slotCollections[j].IsSlotEmpty(num))
				{
					_slotCollections[j].LastSlotId = num;
				}
			}
			SaveSlots();
		}

		public bool IsAllSlotsEmpty()
		{
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				if (!GetSlotCollection(i).IsAllSlotsEmpty())
				{
					return false;
				}
			}
			return true;
		}

		public CarSlot GetSlot(int id)
		{
			return GetSlotCollectionBySlotId(id)[id];
		}

		public CarSlot GetSlot(string carInstanceId)
		{
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				CarSlot slot = GetSlotCollection(i).GetSlot(carInstanceId);
				if (slot != null)
				{
					return slot;
				}
			}
			return null;
		}

		public bool TryGetEmptySlot(out CarSlot res)
		{
			res = null;
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				if (_slotCollections[(i + CurrentGarageIndex) % _slotCollections.Count].TryGetEmptySlot(out res))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSlotEmpty(int id)
		{
			return GetSlotCollectionBySlotId(id)[id]?.IsEmpty ?? false;
		}

		public void ClearSlot(int id)
		{
			GetSlotCollectionBySlotId(id).ClearSlot(id);
			if (SlotCollection.Index == GetSlotCollectionBySlotId(id).Index)
			{
				_storageService.Data.GarageSpotCollection.RemoveSlotFromGarage(id, this);
			}
			SaveSlots();
			OnSlotRemoved?.Invoke(id);
			LogAnalytic();
		}

		public int GetFirstGarageIndexNotFull()
		{
			int num = CurrentGarageIndex;
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				num %= 2;
				SlotCollection slotCollection = GetSlotCollection(num);
				if (slotCollection.IsGarageBought && !slotCollection.IsFull)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public async UniTask<bool> TryBuySlot(int garageIndex = -1, bool checkAllGarages = false, Action<int> onPurchaseResult = null)
		{
			SlotCollection slotCollection = null;
			garageIndex = ((garageIndex >= 0) ? garageIndex : CurrentGarageIndex);
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				garageIndex %= 2;
				slotCollection = GetSlotCollection(garageIndex);
				if ((int)slotCollection.Count < slotCollection.MaxCarSpotCount)
				{
					break;
				}
				if (!checkAllGarages || i == _slotCollections.Count - 1)
				{
					return false;
				}
				garageIndex++;
			}
			bool value = false;
			try
			{
				_currencyManagerController.ShowWaitingWindowMethod();
				AdvancedBoolean advancedBoolean = await _vehicleSlotPaymentService.TryPurchase(garageIndex, onPurchaseResult);
				advancedBoolean.Get(out value);
				if (value)
				{
					CreateSlot(advancedBoolean, slotCollection);
					_currencyManagerController.UpdateCoinsFromDatabase().Forget();
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				value = false;
			}
			finally
			{
				_currencyManagerController.CloseWaitingWindowMethod();
			}
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetFreeSlot(int garageIndex = -1, bool checkAllGarages = false)
		{
			SlotCollection slotCollection = null;
			garageIndex = ((garageIndex >= 0) ? garageIndex : CurrentGarageIndex);
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				garageIndex %= 2;
				slotCollection = GetSlotCollection(garageIndex);
				if ((int)slotCollection.Count < slotCollection.MaxCarSpotCount)
				{
					break;
				}
				if (!checkAllGarages || i == _slotCollections.Count - 1)
				{
					return false;
				}
				garageIndex++;
			}
			CreateSlot(new AdvancedBoolean(v: true), slotCollection);
			return true;
		}

		public void SetSlot(int id, string carFilePath)
		{
			if (!string.IsNullOrEmpty(carFilePath) && !SlotCollection.HasAlreadyCar(carFilePath, id))
			{
				GetSlotCollectionBySlotId(id).SetSlot(id, carFilePath);
				if (SlotCollection.Index == GetSlotCollectionBySlotId(id).Index)
				{
					_storageService.Data.GarageSpotCollection.SetSlotIntoGarage(id, this);
				}
				SaveSlots();
				OnSlotChanged?.Invoke(id);
			}
		}

		public void NotifySlotChanged(int slotId)
		{
			OnSlotChanged?.Invoke(slotId);
		}

		public bool IsSlotIdCorrect(int slotId)
		{
			return GetSlotCollectionBySlotId(slotId).IsSlotIdCorrect(slotId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SaveSlots()
		{
			for (int i = 0; i < _slotCollections.Count; i++)
			{
				_slotCollections[i].SaveSlots();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CreateSlot(AdvancedBoolean buyResult, SlotCollection slotCollection)
		{
			buyResult.Get(out var value);
			if (value)
			{
				CarSlot carSlot = null;
				carSlot = slotCollection.AddNewSlot(buyResult);
				if (carSlot != null)
				{
					SaveSlots();
					OnSlotCreated?.Invoke(carSlot.ID);
					LogAnalytic();
				}
			}
		}

		private void LogAnalytic()
		{
			if (FlagsStorage.InGarageMenu)
			{
				_analyticEventFactory.CreateEvent<GarageInfoEvent, DefaultContext>(new DefaultContext()).Send();
			}
		}
	}
}
