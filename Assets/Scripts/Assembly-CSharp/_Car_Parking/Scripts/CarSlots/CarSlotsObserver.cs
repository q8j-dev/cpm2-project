using System;
using CPM.Scripts.Garage.GarageBuyer;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.CarSlots
{
	public class CarSlotsObserver : IInitializable, IDisposable
	{
		private readonly ISlotService _slotService;

		private readonly IGarageBuyService _garageBuyService;

		public GarageSlotsInfo Current { get; private set; }

		public event Action<GarageSlotsInfo> OnChanged;

		[Inject]
		public CarSlotsObserver(ISlotService slotService, IGarageBuyService garageBuyService)
		{
			_slotService = slotService;
			_garageBuyService = garageBuyService;
		}

		public void Initialize()
		{
			_slotService.OnSlotCreated += OnSlotEvent;
			_slotService.OnSlotChanged += OnSlotEvent;
			_slotService.OnSlotRemoved += OnSlotEvent;
			_garageBuyService.GarageBought += OnGarageBought;
			Recalculate();
		}

		public void Dispose()
		{
			_slotService.OnSlotCreated -= OnSlotEvent;
			_slotService.OnSlotChanged -= OnSlotEvent;
			_slotService.OnSlotRemoved -= OnSlotEvent;
			_garageBuyService.GarageBought -= OnGarageBought;
		}

		public void Recalculate()
		{
			CarSlotsInfo[] array = new CarSlotsInfo[2];
			for (int i = 0; i < 2; i++)
			{
				SlotCollection slotCollection = _slotService.GetSlotCollection(i);
				array[i] = new CarSlotsInfo(slotCollection.OccupiedSlotCount, slotCollection.Count, slotCollection.IsGarageBought);
			}
			GarageSlotsInfo obj = (Current = new GarageSlotsInfo(array));
			OnChanged?.Invoke(obj);
		}

		private void OnSlotEvent(int _)
		{
			Recalculate();
		}

		private void OnGarageBought(int _)
		{
			Recalculate();
		}
	}
}
