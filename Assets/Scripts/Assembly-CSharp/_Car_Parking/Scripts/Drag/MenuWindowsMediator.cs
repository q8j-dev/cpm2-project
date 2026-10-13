using System.Collections.Generic;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.Drag
{
	public class MenuWindowsMediator : IMenuWindowsMediator
	{
		[Inject]
		public IInventoryPartsService InventoryPartsService;

		public void AddInventoryItem(PartData partData)
		{
			InventoryPartsService.Add(partData);
		}

		public List<PartData> GetAllItems(PartType partType)
		{
			return InventoryPartsService.GetAllParts(partType);
		}

		public void RemovePart(PartData partData)
		{
			InventoryPartsService.Remove(partData);
		}
	}
}
