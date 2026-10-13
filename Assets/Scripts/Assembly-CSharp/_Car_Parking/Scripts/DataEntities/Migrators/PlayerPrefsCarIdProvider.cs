using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Persistence.Migration;

namespace _Car_Parking.Scripts.DataEntities.Migrators
{
	public class PlayerPrefsCarIdProvider : IMigrationDataProvider
	{
		public List<object> GetMigrationInputData()
		{
			int num = CarIdStorage.MAX_CAR_ID;
			List<object> list = new List<object>();
			for (int i = 0; i <= num; i++)
			{
				if (ObscuredPrefs.GetInt("car" + i) == 1)
				{
					list.Add(i);
				}
			}
			return list;
		}
	}
}
