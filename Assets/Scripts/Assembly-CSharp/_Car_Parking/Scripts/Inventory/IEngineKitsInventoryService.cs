using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	public interface IEngineKitsInventoryService : IInventoryBaseService<EngineKitData>
	{
		EngineKitsInventoryData InventoryData { get; }

		event Action<EngineKitData> OnEngineRemoved;

		List<EngineKitData> GetAllEngines();

		EngineKitData GetEngineById(int engineID);

		bool IsInInventory(EngineKitData partData);

		void SetData(EngineKitsInventoryData inventoryData);
	}
}
