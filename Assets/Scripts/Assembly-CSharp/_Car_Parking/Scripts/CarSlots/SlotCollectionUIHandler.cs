using System;
using System.Linq;
using CPM.Scripts.Garage;
using Racing.Scripts.UIGrid;
using UnityEngine.Events;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.CarSlots
{
	public class SlotCollectionUIHandler : GridController<SlotUI>
	{
		private ISlotService _slotService;

		private GarageController _garageController;

		public UnityEvent<int> OnSlotRemoveClick;

		public UnityEvent<bool> OnSlotAddOperationFinish;

		public CarSlot this[int uiIndex] => _slotService.SlotCollection[uiIndex];

		protected override int ElementCount => _slotService.SlotCollection.Count;

		protected override int MaxElementCount => _slotService.MaxSlotCount;

		[Inject]
		public void Construct(ISlotService slotService, IPrefabFactory prefabFactory, GaragePositionsConfig garagePositionsConfig, GarageController garageController)
		{
			_slotService = slotService;
			_prefabFactory = prefabFactory;
			_garageController = garageController;
		}

		private void OnEnable()
		{
			_slotService.OnSlotCreated += SlotCreatedHandle;
			_slotService.OnSlotChanged += RepopulateGridData;
			_slotService.OnSlotRemoved += RepopulateGridData;
			GarageController garageController = _garageController;
			garageController.CarFocused = (Action<int>)Delegate.Combine(garageController.CarFocused, new Action<int>(ReselectedItem));
			PopulateGrid();
		}

		private void OnDisable()
		{
			_slotService.OnSlotCreated -= SlotCreatedHandle;
			_slotService.OnSlotChanged -= RepopulateGridData;
			_slotService.OnSlotRemoved -= RepopulateGridData;
			GarageController garageController = _garageController;
			garageController.CarFocused = (Action<int>)Delegate.Remove(garageController.CarFocused, new Action<int>(ReselectedItem));
		}

		private void RepopulateGridData(int dump = 0)
		{
			RepopulateGrid();
		}

		private void ReselectedItem(int slotIndex)
		{
			RepopulateGridData();
		}

		private void SlotCreatedHandle(int slotId)
		{
			OnElementCreated(slotId % 1000);
			_gridElements[slotId % 1000].SetSelected(isSelected: false);
		}

		public int GetElementIndexBySlotId(int slotId)
		{
			SlotUI slotUI = _gridElements.First((SlotUI element) => element.ID == slotId);
			if ((bool)slotUI)
			{
				return _gridElements.IndexOf(slotUI);
			}
			return 0;
		}

		public int GetSlotIdByElementIndex(int elementIndex)
		{
			if (elementIndex < 0 || elementIndex >= _gridElements.Count)
			{
				return 0;
			}
			return _gridElements[elementIndex].ID;
		}
	}
}
