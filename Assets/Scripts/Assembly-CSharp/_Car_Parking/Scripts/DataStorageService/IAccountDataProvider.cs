using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.DataStorageService
{
	public interface IAccountDataProvider
	{
		ObscuredString Name { get; set; }

		ObscuredString AccountID { get; set; }

		ObscuredString LocalID { get; set; }

		void SetBasicPlayerData(string playerName, string localID);
	}
}
