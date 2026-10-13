using System.Collections.Generic;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Drag
{
	public interface IMenuWindowsMediator
	{
		void AddInventoryItem(PartData partData);

		List<PartData> GetAllItems(PartType partType);

		void RemovePart(PartData partData);
	}
}
