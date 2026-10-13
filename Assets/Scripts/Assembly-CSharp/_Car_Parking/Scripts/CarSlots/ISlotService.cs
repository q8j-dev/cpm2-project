using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace _Car_Parking.Scripts.CarSlots
{
	[Preserve]
	public interface ISlotService
	{
		int SlotsCountInAllGarages { get; }

		List<CarSlot> AllSlotsInfos { get; }

		int MaxSlotCount { get; }

		int SlotCount { get; }

		int OccupiedSlotCount { get; }

		int OccupiedSlotCountInAllGarages { get; }

		bool HaveCarInAnyGarage { get; }

		SlotCollection SlotCollection { get; }

		bool AllGaragesFull { get; }

		event Action<int> OnSlotCreated;

		event Action<int> OnSlotChanged;

		event Action<int> OnSlotRemoved;

		SlotCollection GetSlotCollection(int garageIndex);

		SlotCollection GetSlotCollectionBySlotId(int slotId);

		CarSlot GetSlot(int id);

		CarSlot GetSlot(string carInstanceId);

		bool TryGetEmptySlot(out CarSlot res);

		int GetFirstGarageIndexNotFull();

		UniTask<bool> TryBuySlot(int garageIndex = -1, bool checkAllGarages = false, Action<int> onPurchaseResult = null);

		bool TryGetFreeSlot(int garageIndex = -1, bool checkAllGarages = false);

		bool IsSlotEmpty(int id);

		void SetSlot(int id, string carFilePath);

		void DeleteAllSlots();

		void ClearSlot(int id);

		int GetNotEmptySlotId();

		List<int> GetCurrentGarageOccupiedSlotIds();

		List<int> GetAllSlots();

		void ResizeCollection();

		SlotsOnDatabaseCollection GetSlotsForDatabase();

		void SetSlotsFromDatabase(SlotsOnDatabaseCollection recordsSlotsOnDatabaseCollection);

		bool IsAllSlotsEmpty();

		bool IsSlotIdCorrect(int slotId);

		void NotifySlotChanged(int slotId);
	}
}
