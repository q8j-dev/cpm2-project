using System.Collections.Generic;
using CPMEngine;

namespace _Car_Parking.Scripts.DataStorageService
{
	public interface IWheelsDataService
	{
		List<int> BoughtWheels { get; }

		bool IsBrakeStock(BrakeDiscType type, int carId);

		bool IsBrakeBought(int index);

		void BuyBrake(int index);

		void BuyCaliper(int index);

		bool IsCaliperBought(int index);

		void Save();
	}
}
