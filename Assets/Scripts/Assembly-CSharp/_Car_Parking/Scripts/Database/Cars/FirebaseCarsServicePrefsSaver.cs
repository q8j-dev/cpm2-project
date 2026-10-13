using CodeStage.AntiCheat.Storage;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Database.Cars
{
	public class FirebaseCarsServicePrefsSaver : IAllPrefsSaver
	{
		private int _version;

		public void Save()
		{
			_version = ObscuredPrefs.GetInt("CarSaveVersion0");
		}

		public void Load()
		{
			ObscuredPrefs.SetInt("CarSaveVersion0", _version);
		}
	}
}
