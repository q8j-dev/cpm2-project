using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class WalletDataHashes : HashesData<WalletData>
	{
		public override void CalculateChangedValues(WalletData data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<WalletData>.Data;
			data2.Clear();
			CacheData(data.Money, "Money", data2, fullData);
			changedData = data2;
		}
	}
}
