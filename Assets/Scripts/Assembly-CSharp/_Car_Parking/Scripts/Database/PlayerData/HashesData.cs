using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using _Car_Parking.Scripts.DataEntities;

namespace _Car_Parking.Scripts.Database.PlayerData
{
	public abstract class HashesData<T>
	{
		protected static Dictionary<string, object> Data = new Dictionary<string, object>();

		public Dictionary<string, int> LastSentHashes = new Dictionary<string, int>();

		public Dictionary<string, int> TempNewHashes = new Dictionary<string, int>();

		public abstract void CalculateChangedValues(T data, out Dictionary<string, object> changedData, bool fullData = false);

		protected void CacheData(object data, string dataName, Dictionary<string, object> dataDict, bool alwaysSend = false)
		{
			int hashCode = JsonConvert.SerializeObject(data, JsonConvertCustomSettingsForDatabase.JsonSettingsForDataBase).GetHashCode();
			if (alwaysSend)
			{
				dataDict[dataName] = data;
				TempNewHashes[dataName] = hashCode;
				return;
			}
			if (LastSentHashes.TryGetValue(dataName, out var value))
			{
				if (value != hashCode)
				{
					dataDict[dataName] = data;
				}
			}
			else
			{
				dataDict[dataName] = data;
			}
			TempNewHashes[dataName] = hashCode;
		}

		public void ConfirmSuccess()
		{
			LastSentHashes = TempNewHashes.ToDictionary((KeyValuePair<string, int> entry) => entry.Key, (KeyValuePair<string, int> entry) => entry.Value);
			TempNewHashes.Clear();
		}
	}
}
