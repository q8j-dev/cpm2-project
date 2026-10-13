using System.Collections.Generic;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	[Preserve]
	public class FriendsCollectionHashes : HashesData<FriendsCollection>
	{
		public override void CalculateChangedValues(FriendsCollection data, out Dictionary<string, object> changedData, bool fullData = false)
		{
			if (data == null)
			{
				changedData = new Dictionary<string, object>();
				return;
			}
			Dictionary<string, object> data2 = HashesData<FriendsCollection>.Data;
			data2.Clear();
			CacheData(data.FriendsID, "FriendsID", data2, fullData);
			changedData = data2;
		}
	}
}
