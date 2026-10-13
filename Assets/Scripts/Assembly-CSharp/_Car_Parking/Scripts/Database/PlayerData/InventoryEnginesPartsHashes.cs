using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class InventoryEnginesPartsHashes : HashesData<EngineKitsInventoryData>
	{
		public override void CalculateChangedValues(EngineKitsInventoryData data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<EngineKitsInventoryData>.Data;
			data2.Clear();
			CacheData(data.Inventory, "Inventory", data2, fullData);
			changedData = data2;
		}
	}
}
