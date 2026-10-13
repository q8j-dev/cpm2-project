using System.Collections.Generic;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Inventory
{
	public interface IInventoryPartsService : IInventoryBaseService<PartData>
	{
		InventoryData InventoryData { get; }

		List<PartData> GetAllParts(PartType partType);

		void SetData(InventoryData inventoryData);

		PartData GetCompatibleECU(int engineModelId, int carId);
	}
}
