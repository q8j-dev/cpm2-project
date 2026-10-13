using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class PlayerAnonStorageHashes : HashesData<PlayerStorage>
	{
		public override void CalculateChangedValues(PlayerStorage data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<PlayerStorage>.Data;
			data2.Clear();
			CacheData(data.LocalID, "LocalID", data2, alwaysSend: true);
			CacheData(data.Name, "Name", data2, fullData);
			if (data2.Count == 1 && data2.ContainsKey("LocalID"))
			{
				data2.Clear();
			}
			changedData = data2;
		}
	}
}
