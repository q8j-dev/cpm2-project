using System;
using UnityEngine;

namespace _Car_Parking.Scripts.CarSlots
{
	public class GarageSpotCollection
	{
		public int this[int spotId] => spotId;

		public event Action<int> OnGarageSpotOccupied;

		public event Action<int> OnGarageSpotEmptied;

		public int GetSpotBySlotId(int slotId, ISlotService slotService)
		{
			if (!slotService.IsSlotIdCorrect(slotId))
			{
				Debug.LogError($"No garage spot with id: {slotId}");
				return -1;
			}
			return slotId;
		}

		public int GetSpotBySlotIdOrEmpty(int slotId, ISlotService slotService)
		{
			if (!slotService.IsSlotIdCorrect(slotId))
			{
				Debug.LogError($"No garage spot with id: {slotId}");
				return -1;
			}
			return slotId;
		}

		public void SetSlotIntoGarage(int slotId, ISlotService slotService)
		{
			if (!slotService.IsSlotIdCorrect(slotId))
			{
				Debug.LogError($"No garage spot with id: {slotId}");
			}
			else
			{
				OnGarageSpotOccupied?.Invoke(slotId);
			}
		}

		public void RemoveSlotFromGarage(int slotId, ISlotService slotService)
		{
			if (!slotService.IsSlotIdCorrect(slotId))
			{
				Debug.LogError($"No garage spot with id: {slotId}");
			}
			else
			{
				OnGarageSpotEmptied?.Invoke(slotId);
			}
		}
	}
}
