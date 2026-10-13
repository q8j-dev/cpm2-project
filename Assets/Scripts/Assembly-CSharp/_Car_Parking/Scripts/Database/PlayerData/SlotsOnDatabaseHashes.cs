using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class SlotsOnDatabaseHashes : HashesData<SlotsOnDatabaseCollection>
	{
		public override void CalculateChangedValues(SlotsOnDatabaseCollection data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<SlotsOnDatabaseCollection>.Data;
			data2.Clear();
			foreach (KeyValuePair<int, string> slot in data.Slots)
			{
				CacheData(slot.Value, slot.Key.ToString(), data2, fullData);
			}
			changedData = data2;
		}
	}
}
