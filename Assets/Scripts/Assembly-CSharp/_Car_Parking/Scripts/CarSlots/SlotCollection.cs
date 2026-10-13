using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CPM.Scripts.Garage.GarageBuyer;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using _Car_Parking.Scripts.CustomDataTypes;

namespace _Car_Parking.Scripts.CarSlots
{
	public class SlotCollection
	{
		public const int IdBuffer = 1000;

		public ObscuredInt Count;

		private Dictionary<int, CarSlot> _slots;

		private string _prefsPrefixKey = "";

		private int _index;

		private readonly GaragePositionsConfig _config;

		private readonly IGarageBuyService _garageBuyService;

		public int Index => _index;

		public int OccupiedSlotCount
		{
			get
			{
				if (!IsGarageBought)
				{
					return 0;
				}
				return _slots.Values.Count((CarSlot slot) => slot != null && !slot.IsEmpty);
			}
		}

		public bool HaveOccupiedSlot => !IsAllSlotsEmpty();

		public CarSlot this[int id]
		{
			get
			{
				id = GetChangedSlotID(id);
				return _slots.GetValueOrDefault(id);
			}
		}

		public bool IsFull
		{
			get
			{
				if (IsGarageBought)
				{
					return (int)Count == MaxCarSpotCount;
				}
				return false;
			}
		}

		public int MaxCarSpotCount
		{
			get
			{
				if (!IsGarageBought)
				{
					return 0;
				}
				return _config.GaragePositionsDatas[_index].MaxCarSpotCount;
			}
		}

		public int DefaultCarSpotCount => _config.GaragePositionsDatas[_index].DefaultCarSpotCount;

		public int LastSlotId
		{
			get
			{
				return ObscuredPrefs.GetInt(LastSlotIdPrefsKey, GetChangedSlotID(0));
			}
			set
			{
				ObscuredPrefs.SetInt(LastSlotIdPrefsKey, value);
				ObscuredPrefs.Save();
			}
		}

		public int SlotCountPrefs
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return ObscuredPrefs.GetInt(SlotCountPrefsKey, DefaultCarSpotCount);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				ObscuredPrefs.SetInt(SlotCountPrefsKey, value);
			}
		}

		public bool IsGarageBought
		{
			get
			{
				_garageBuyService.GetGarageStatus(_index).Get(out var value);
				return value;
			}
		}

		private string LastSlotIdPrefsKey => "LastSlotId" + _prefsPrefixKey;

		private string SlotCountPrefsKey => "SlotCount" + _prefsPrefixKey;

		private string SlotPrefsKey => "Slot_" + _prefsPrefixKey;

		public SlotCollection(GaragePositionsConfig config, int garageIndex, IGarageBuyService garageBuyService)
		{
			_garageBuyService = garageBuyService;
			_config = config;
			Init(garageIndex);
		}

		public void Init(int garageIndex)
		{
			_index = garageIndex;
			_prefsPrefixKey = ((garageIndex == 0) ? "" : ("GarageIndex" + garageIndex));
			int slotCountPrefs = SlotCountPrefs;
			_slots = new Dictionary<int, CarSlot>(slotCountPrefs);
			Count = slotCountPrefs;
			for (int i = 0; i < slotCountPrefs; i++)
			{
				int changedSlotID = GetChangedSlotID(i);
				CarSlot carSlot = JsonConvert.DeserializeObject<CarSlot>(ObscuredPrefs.GetString(SlotPrefsKey + changedSlotID));
				if (carSlot != null && HasAlreadyCar(carSlot.CarFilePath, carSlot.ID))
				{
					carSlot = null;
				}
				_slots.Add(changedSlotID, carSlot ?? new CarSlot(changedSlotID));
			}
		}

		public bool IsSlotEmpty(int id)
		{
			return this[id]?.IsEmpty ?? false;
		}

		public bool TryGetEmptySlot(out CarSlot res)
		{
			res = null;
			if (!IsGarageBought)
			{
				return false;
			}
			foreach (KeyValuePair<int, CarSlot> slot in _slots)
			{
				if (string.IsNullOrEmpty(slot.Value.CarFilePath))
				{
					res = slot.Value;
					return true;
				}
			}
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CarSlot AddNewSlot(AdvancedBoolean buyResult)
		{
			buyResult.Get(out var value);
			if (!value)
			{
				return null;
			}
			int changedSlotID = GetChangedSlotID(Count);
			++Count;
			CarSlot carSlot = new CarSlot(changedSlotID);
			_slots.Add(carSlot.ID, carSlot);
			return carSlot;
		}

		public int GetFirstNotEmptySlot()
		{
			foreach (KeyValuePair<int, CarSlot> slot in _slots)
			{
				if (!string.IsNullOrEmpty(slot.Value.CarFilePath))
				{
					return slot.Key;
				}
			}
			return -1;
		}

		public CarSlot SetSlot(int id, string carFilePath, bool canAddNewSlot = false)
		{
			id = GetChangedSlotID(id);
			CarSlot carSlot = this[id];
			if ((carSlot == null) & canAddNewSlot)
			{
				carSlot = new CarSlot(id);
				_slots.Add(carSlot.ID, carSlot);
				Count = _slots.Count;
			}
			if (carSlot != null)
			{
				carSlot.CarFilePath = carFilePath;
			}
			return carSlot;
		}

		public CarSlot GetSlot(string carFilePath)
		{
			CarSlot result = null;
			foreach (KeyValuePair<int, CarSlot> slot in _slots)
			{
				if (slot.Value != null && slot.Value.CarFilePath == carFilePath)
				{
					result = slot.Value;
					break;
				}
			}
			return result;
		}

		public void DeleteAllSlots()
		{
			_slots.Clear();
			Count = 0;
			LastSlotId = GetChangedSlotID(0);
			for (int i = 0; i < DefaultCarSpotCount; i++)
			{
				AddNewSlot(new AdvancedBoolean(v: true));
			}
			SaveSlots();
		}

		public void ClearAllSlots()
		{
			for (int i = 0; i < MaxCarSpotCount; i++)
			{
				ClearSlot(i);
			}
			SaveSlots();
		}

		public void ClearToDefault()
		{
			ClearAllSlots();
			ObscuredPrefs.DeleteKey(LastSlotIdPrefsKey);
			ObscuredPrefs.DeleteKey(SlotCountPrefsKey);
			Init(_index);
		}

		public void ClearSlot(int id)
		{
			id = GetChangedSlotID(id);
			CarSlot carSlot = this[id];
			if (carSlot != null)
			{
				carSlot.CarFilePath = "";
			}
		}

		public bool IsSlotIdCorrect(int slotId)
		{
			int num = _index * 1000;
			int num2 = num + (int)Count;
			if (slotId >= num)
			{
				return slotId < num2;
			}
			return false;
		}

		public void SaveSlots()
		{
			SlotCountPrefs = Count;
			foreach (KeyValuePair<int, CarSlot> slot in _slots)
			{
				string value = JsonConvert.SerializeObject(slot.Value);
				ObscuredPrefs.SetString(SlotPrefsKey + slot.Value.ID, value);
			}
		}

		public Dictionary<int, string> GetSlotCollectionForDatabase()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			if (!IsGarageBought)
			{
				return dictionary;
			}
			foreach (KeyValuePair<int, CarSlot> slot in _slots)
			{
				dictionary.Add(slot.Key, slot.Value.CarFilePath);
			}
			return dictionary;
		}

		public bool IsAllSlotsEmpty()
		{
			return OccupiedSlotCount == 0;
		}

		public bool HasAlreadyCar(string carFilePath, int excludeSlotId = -1)
		{
			return false;
		}

		public int GetChangedSlotID(int id)
		{
			int num = _index * 1000;
			int num2 = num + 1000;
			if (id >= num && id < num2)
			{
				return id;
			}
			return _index * 1000 + id;
		}
	}
}
