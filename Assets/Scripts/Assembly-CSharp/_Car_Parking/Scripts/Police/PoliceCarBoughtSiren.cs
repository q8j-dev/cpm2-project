using System;
using System.Collections.Generic;
using CPM.Database;
using _Car_Parking.Scripts.Other;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarBoughtSiren
	{
		private const string PlayerPrefsKey = "PoliceCarBoughtSiren01";

		private PrefsArrayInt _bought;

		public event Action<int> OnBoughted;

		public PoliceCarBoughtSiren()
		{
			_bought = new PrefsArrayInt("PoliceCarBoughtSiren01", PrefsArrayInt.TypeSave.ObscuredPrefs);
		}

		public void SaveInstalledBodyKitsFromCarInfo(CarInfo carInfo)
		{
			if (carInfo != null && carInfo.AdditionalData != null && carInfo.AdditionalData.Police != null && carInfo.AdditionalData.Police.SirenSound != null && carInfo.AdditionalData.Police.SirenSound.InstalledIndex >= 0)
			{
				AddBought(carInfo.AdditionalData.Police.SirenSound.InstalledIndex);
			}
		}

		public List<int> GetDataForDataBase()
		{
			return _bought.GetIntList();
		}

		public void LoadDataFromDataBase(List<int> data)
		{
			if (data != null)
			{
				for (int i = 0; i < data.Count; i++)
				{
					_bought.AddIfNotContains(data[i]);
				}
			}
		}

		public bool AddBought(int index)
		{
			if (!Contains(index))
			{
				_bought.Add(index);
				OnBoughted?.Invoke(index);
				return true;
			}
			return false;
		}

		public bool Contains(int index)
		{
			return _bought.Contains(index);
		}
	}
}
