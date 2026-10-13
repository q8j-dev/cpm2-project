using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.DataEntities.Car;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Serializable]
	public class OneCarHashes : HashesData<CarInfoForDatabase>
	{
		public override void CalculateChangedValues(CarInfoForDatabase data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			HashesData<CarInfoForDatabase>.Data.Clear();
			CacheData(data.CarID, "CarID", HashesData<CarInfoForDatabase>.Data, alwaysSend: true);
			CacheData(data.CarInstanceId, "CarInstanceId", HashesData<CarInfoForDatabase>.Data, alwaysSend: true);
			CacheData(data.BoughtBodyKits, "BoughtBodyKits", HashesData<CarInfoForDatabase>.Data, fullData);
			CacheData(data.Colors, "Colors", HashesData<CarInfoForDatabase>.Data, fullData);
			CacheData(data.TuningData, "TuningData", HashesData<CarInfoForDatabase>.Data, fullData);
			CacheData(data.AdditionalData, "AdditionalData", HashesData<CarInfoForDatabase>.Data, fullData);
			CacheData(data.InstalledBodyKits, "InstalledBodyKits", HashesData<CarInfoForDatabase>.Data, fullData);
			CacheData(data.Vinyls, "Vinyls", HashesData<CarInfoForDatabase>.Data, fullData);
			CacheData(data.Window, "Window", HashesData<CarInfoForDatabase>.Data, fullData);
			if (HashesData<CarInfoForDatabase>.Data.ContainsKey("data.TuningData.FrontWheelData"))
			{
				CacheData("", "TuningData.FrontWheelData.Limits", HashesData<CarInfoForDatabase>.Data, fullData);
			}
			if (HashesData<CarInfoForDatabase>.Data.ContainsKey("data.TuningData.RearWheelData"))
			{
				CacheData("", "TuningData.RearWheelData.Limits", HashesData<CarInfoForDatabase>.Data, fullData);
			}
			if (HashesData<CarInfoForDatabase>.Data.Count == 2 && HashesData<CarInfoForDatabase>.Data.ContainsKey("CarID") && HashesData<CarInfoForDatabase>.Data.ContainsKey("CarInstanceId"))
			{
				HashesData<CarInfoForDatabase>.Data.Clear();
			}
			changedData = HashesData<CarInfoForDatabase>.Data;
		}
	}
}
