using System.Collections.Generic;
using CPM.Database;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class PlatesHashes : HashesData<PlatesDataForDatabase>
	{
		public override void CalculateChangedValues(PlatesDataForDatabase data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<PlatesDataForDatabase>.Data;
			data2.Clear();
			CacheData(data.PlateData, "PlateData", data2, fullData);
			CacheData(data.Plate1, "Plate1", data2, fullData);
			CacheData(data.Plate2, "Plate2", data2, fullData);
			CacheData(data.Plate3, "Plate3", data2, fullData);
			CacheData(data.Plate4, "Plate4", data2, fullData);
			CacheData(data.Plate5, "Plate5", data2, fullData);
			CacheData(data.Plate6, "Plate6", data2, fullData);
			changedData = data2;
		}
	}
}
