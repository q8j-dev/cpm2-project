using System;
using System.Collections.Generic;
using CPM.Database;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarBoughtBodyKit
	{
		private class BoughtData
		{
			private const string PrefsKey = "BoughtBodyKit_";

			public PoliceBodyKitType BodyKitType;

			public PrefsArrayInt Indexes;

			public BoughtData()
			{
			}

			public BoughtData(PoliceBodyKitType bodyKitType)
			{
				BodyKitType = bodyKitType;
				Indexes = new PrefsArrayInt("BoughtBodyKit_" + BodyKitType, PrefsArrayInt.TypeSave.ObscuredPrefs);
			}
		}

		private List<BoughtData> _boughtDatas = new List<BoughtData>();

		private Array AllPoliceBodyKitTypes => Enum.GetValues(typeof(PoliceBodyKitType));

		public event Action<PoliceBodyKitType, int> OnBoughted;

		public PoliceCarBoughtBodyKit()
		{
			foreach (PoliceBodyKitType allPoliceBodyKitType in AllPoliceBodyKitTypes)
			{
				_boughtDatas.Add(new BoughtData(allPoliceBodyKitType));
			}
		}

		public void SaveInstalledBodyKitsFromCarInfo(CarInfo carInfo)
		{
			if (carInfo == null || carInfo.AdditionalData == null || carInfo.AdditionalData.Police == null || carInfo.AdditionalData.Police.InstalledBodyKits == null)
			{
				return;
			}
			foreach (PoliceBodyKitType allPoliceBodyKitType in AllPoliceBodyKitTypes)
			{
				int index = carInfo.AdditionalData.Police.InstalledBodyKits.GetIndex(allPoliceBodyKitType);
				if (index >= 0)
				{
					AddBought(allPoliceBodyKitType, index);
				}
			}
		}

		public Dictionary<int, string> GetDataForDataBase()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			for (int i = 0; i < _boughtDatas.Count; i++)
			{
				dictionary.Add((int)_boughtDatas[i].BodyKitType, _boughtDatas[i].Indexes.PrefsValue);
			}
			return dictionary;
		}

		public void LoadDataFromDataBase(Dictionary<int, string> data)
		{
			if (data == null)
			{
				return;
			}
			foreach (KeyValuePair<int, string> datum in data)
			{
				PoliceBodyKitType key = (PoliceBodyKitType)datum.Key;
				GetBoughtData(key).Indexes.PrefsValue = datum.Value;
			}
		}

		public bool AddBought(PoliceBodyKitType bodyKitType, List<int> indexes)
		{
			bool result = false;
			for (int i = 0; i < indexes.Count; i++)
			{
				if (AddBought(bodyKitType, indexes[i]))
				{
					result = true;
				}
			}
			return result;
		}

		public bool AddBought(PoliceCarBodyKitsConfig.BodyKitData bodyKitData)
		{
			return AddBought(bodyKitData.BodyKitType, bodyKitData.Index);
		}

		public bool AddBought(PoliceBodyKitType bodyKitType, int index)
		{
			if (!Contains(bodyKitType, index))
			{
				GetBoughtData(bodyKitType).Indexes.Add(index);
				OnBoughted?.Invoke(bodyKitType, index);
				return true;
			}
			return false;
		}

		public bool Contains(PoliceCarBodyKitsConfig.BodyKitData bodyKitData)
		{
			return Contains(bodyKitData.BodyKitType, bodyKitData.Index);
		}

		public bool Contains(PoliceBodyKitType bodyKitType, int index)
		{
			return GetBoughtData(bodyKitType).Indexes.Contains(index);
		}

		public List<int> GetBoughtIndexes(PoliceBodyKitType bodyKitType)
		{
			return GetBoughtData(bodyKitType).Indexes.GetIntList();
		}

		private BoughtData GetBoughtData(PoliceBodyKitType bodyKitType)
		{
			for (int i = 0; i < _boughtDatas.Count; i++)
			{
				if (_boughtDatas[i].BodyKitType == bodyKitType)
				{
					return _boughtDatas[i];
				}
			}
			BoughtData boughtData = new BoughtData(bodyKitType);
			_boughtDatas.Add(boughtData);
			return boughtData;
		}
	}
}
