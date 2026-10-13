using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class InventoryPartsHashes : HashesData<InventoryData>
	{
		public override void CalculateChangedValues(InventoryData data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<InventoryData>.Data;
			data2.Clear();
			CacheData(data.Inventory, "Inventory", data2, fullData);
			changedData = data2;
		}
	}
}
