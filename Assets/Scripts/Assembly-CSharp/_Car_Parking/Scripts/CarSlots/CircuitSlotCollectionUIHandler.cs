using System;
using CPM.Scripts.Garage;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.CarSlots
{
	public class CircuitSlotCollectionUIHandler : SlotCollectionUIHandler
	{
		private _Car_Parking.Scripts.EventSystem.Data.Event _event;

		private ISlotService _slotService;

		private IGarageEntity _currentGarage;

		private EventRestrictionService _eventRestrictionService;

		protected override int ElementCount => _slotService.OccupiedSlotCount;

		[Inject]
		public void Construct(ISlotService slotService, EventRestrictionService eventRestrictionService, IGarageEntity garageEntity)
		{
			_slotService = slotService;
			_eventRestrictionService = eventRestrictionService;
			_currentGarage = garageEntity;
		}

		public void SetEvent(_Car_Parking.Scripts.EventSystem.Data.Event @event)
		{
			_event = @event;
		}

		protected override void PresetElement(SlotUI element)
		{
			base.PresetElement(element);
			PresetElementAsync(element).Forget();
		}

		private async UniTask PresetElementAsync(SlotUI element)
		{
			VehicleData carBySpot = _currentGarage.GetCarBySpot(element.ID);
			if (!(carBySpot != null))
			{
				return;
			}
			VehicleClass vehicleClass = Utils.GetClassVehicle(carBySpot.GetVehicleEntityMono.CarEntity.GetValue().Points, carBySpot.CarId);
			try
			{
				await UniTask.WaitUntil(() => _event != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				if (!_eventRestrictionService.IsVehicleAllowed(_event.Type, vehicleClass))
				{
					element.Deactivate();
				}
			}
			catch (Exception)
			{
				Debug.Log("Operation canceled by destroy");
			}
		}
	}
}
