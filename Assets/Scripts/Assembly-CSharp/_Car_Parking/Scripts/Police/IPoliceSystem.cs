using System;
using CPM.Database;

namespace _Car_Parking.Scripts.Police
{
	public interface IPoliceSystem
	{
		PoliceCarBoughtBodyKit BoughtBodyKit { get; }

		PoliceCarBoughtSiren BoughtSiren { get; }

		PoliceSystemConfig PoliceSystemConfig { get; }

		event Action<CarInfo, bool> OnBoughtUpdated;

		bool IsBought(CarInfo carInfo);

		void TryBuy(CarInfo carInfo);

		void SaveInstalledData(CarInfo carInfo);
	}
}
